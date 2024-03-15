using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Electroimpact.Transformer
{
  public static class Program
  {
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      Application.Run(new frmBarrelTransformer());
    }

    public class cBTransformer
    {


      public double[] SolveTransform(string IPaddress,  int which_transform, out List<double> errorList)
      {
        List<int> outPts;
        return SolveTransform(IPaddress, true, which_transform, double.NaN, out errorList, out outPts, false);
      }

      public double[] SolveTransform(string IPaddress, bool RawDataInMillimeters, int which_transform, double maxPtErr_RawDataUnits, out List<double> errorList, out List<int> usedPoints, bool sendXformOnlyIfPass = true)
      {
        usedPoints = new List<int>();
        frmBarrelTransformer TformUI = new frmBarrelTransformer(false);

        TformUI.SetDataUnits(RawDataInMillimeters);

        //calculate the transform
        TformUI.InterfaceTransform(!double.IsNaN(maxPtErr_RawDataUnits), maxPtErr_RawDataUnits);

        errorList = TformUI.errorList;
        usedPoints = TformUI.myPoints.Where(p => p.bUseInXform == true).Select(p => p.point_number).ToList();

        if (TformUI.successfulSolve || !sendXformOnlyIfPass)
        {
          //send transform to cnc
          bool xFormSentSuccess = TformUI.SendTfrm2CNC_M38S_any(IPaddress, which_transform);

          string line = "Transform success!\n";
          if (xFormSentSuccess)
          {
            line += $"Transform sent successfully to CNC slot { which_transform}\n";
          }
          else
          {
            line += "Transform NOT sent to CNC\n";
          }
          int xx = 1;
          for (int i = 0; i < errorList.Count; i++)
          {
            if (usedPoints.Contains(xx))
            {
              double eb = Math.Round(errorList[i], 3);
              line += "\nPoint" + xx.ToString() + " error: " + eb.ToString() + "in.";
            }
            else
            {
              line += "\nPoint" + xx.ToString() + " - Not Used.";
            }
            xx++;
          }
          MessageBox.Show(line);

          //get Euler XYZABC values in input units and degrees
          return TformUI.GetEuler();
        }
        else
        {
          MessageBox.Show("Transform was not made. Too few usable points. Please try again.");
          return null;
        }
      }

      public double[] SolveTransform(string IPaddress, bool RawDataInMillimeters, int which_transform, out List<double> errorList)
      {
        List<int> outPts;
        return SolveTransform(IPaddress, RawDataInMillimeters, which_transform, double.NaN, out errorList, out outPts, false);
      }

      public bool SolveAndSendS41_BTform(string[] IPaddresses, string[] lines, out List<double> errorList)
      {
        int nArgs = 7;
        bool success = false;
        errorList = null;
        string[] lines2 = new string[lines.Length - 1];

        //skip the 1st line because it is a header
        for (int ii = 1; ii < lines.Length; ii++)
        {
          string[] items = lines[ii].Split(','); //Split('\t');
          if (items.Length == nArgs)
          {
            lines2[ii - 1] = lines[ii];
          }
          else
          {
            MessageBox.Show(lines[ii] + " wrong number of arguments. \n expected \"Xnom,Ynom,Znom,Upos,Xmeas,Ymeas,Zmeas\"\n");
            return false;
          }
        }

        if(lines2.Length < 11)
        {
            MessageBox.Show("Not Enough Data for Transform.  Need More Than 11 Data Points.  Get More Probe Data.");
            return false;
        }

        frmBarrelTransformer TformUI = new frmBarrelTransformer(false, true);

        //calculate the transform
        TformUI.InterfaceS41_BTform(lines2);


        errorList = TformUI.errorList;

        if (TformUI.ErrAve > 0.100)
        {
          MessageBox.Show("Bad Transform.  Average Err is " + TformUI.ErrAve.ToString("F3") + " Inches.");
        }
        else
        {

          //send transform to each slave cnc
          foreach (string IPAddr in IPaddresses)
          {
            TformUI.SendAutoS41_BTform(IPAddr);
          }
          
        }

        return success;// TformUI.GetEuler();

      }

    }
  }
}
