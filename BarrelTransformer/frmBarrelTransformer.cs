using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
//using SolverPlatform;
using Electroimpact;
using System.IO;
using Electroimpact.LinearAlgebra;
using Electroimpact.Utilities;

using System.Linq; //provides List.Max, List.Min, List.Average, etc

//==============================
//Copyright (c) 2004-2013, Regents of the University of California
//All rights reserved.

//Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:

//    * Redistributions of source code must retain the above copyright notice,
//       this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright
//      notice, this list of conditions and the following disclaimer in the
//      documentation and/or other materials provided with the distribution.
//    * Neither the name of the University of California, Berkeley
//nor the names of its contributors may be used to endorse or promote
//products derived from this software without specific prior written permission.

//THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
//"AS IS" AND Ay_nom EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
//LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
//FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE
//COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR Ay_nom DIRECT, INDIRECT,
//INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING,
//BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
//LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
//CAUSED AND ON Ay_nom THEORY OF LIABILITY, WHETHER IN CONTRACT,
//STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
//ARISING IN Ay_nom WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED
//OF THE POSSIBILITY OF SUCH DAMAGE.
//====================================

namespace Electroimpact.Transformer
{
  public partial class frmBarrelTransformer : Form
  {
    #region members
    const int AnlgePower = 6;
    const int OffsetPower = 3;
    const int MatrixPower = 6;
    double AnglePrecision = 1.0 * Math.Pow(10, (double)AnlgePower);
    double OffsetPrecision = 1.0 * Math.Pow(10, (double)OffsetPower);
    double MatrixPrecision = 1.0 * Math.Pow(10, (double)MatrixPower);
    TransformerInfo.cTool passedInTool = null;
    //public Electroimpact.LinearAlgebra.c6dof Euler = new Electroimpact.LinearAlgebra.c6dof();
    public cBarrelFunctions myBarrelFunction;
    public List<Electroimpact.cPoint> myPoints = new List<cPoint>();
    public cFormState formstate;
    public bool manualTransform = false;
    public List<double> errorList = new List<double>();
    public bool successfulSolve = false;

    //SiemensCNC _CNC; //siemens CNC connection, includes servers
    Timer tmr;
    private bool standAlone = true;
    List<rotationPoint> PointsForRot = new List<rotationPoint>();
    PrimarySpinAxis whichSpinAxis = PrimarySpinAxis.X; //X is the default spin axis
    bool SpinAxisOffset180 = false;
    public double ErrAve = 0.0;


    Electroimpact.FANUC.Err_Code err;
    Electroimpact.FANUC.OpenCNC CNC = null;// = new Electroimpact.FANUC.OpenCNC();


    public string[] EulerCheckLines;
    public SingleSiemensXform CalculatedSiemensXform = new SingleSiemensXform(32);
    public SingleSiemensXform CalculatedRobX = new SingleSiemensXform(3);
    public int selectedTransformIndex = -1;
    #endregion

    #region FormState
    [Serializable]
    [XmlRoot("XML")]
    public class cFormState
    {

      // TO DO
      // Set SaveLoc
      // in the constructor of parent class call cFormState.OpenMe
      // in the parent class, implement LocationChanged and copy the location into fsp
      // anytime you make a change you want to save, call cFormState.Save();

      #region Members
      public string mLastOpenedInput = "";
      public string cnc_ip = "192.168.100.10";
      public bool uses_uv_table_transform = false;
      public Point fsp = new Point();
      public cBarrelFunctions.cSetupBarrelTransformer LastTransformState = new cBarrelFunctions.cSetupBarrelTransformer();
      public bool uses_uofx_function = true;
      public bool uses_barrel_transform = false;
      public bool uses_mm_data = false;
      public bool uses_mult_mandrels = false;
      public bool remember_ip_address_input = true;
      public PrimarySpinAxis fs_spin_axis = PrimarySpinAxis.X;
      public double AveToleranceMM = 0.06 * 25.4;
      public double MaxToleranceMM = 0.1 * 25.4;
      public bool bGoodTransformFit = false;

      #endregion

      #region Constructor
      public cFormState()
      {

      }

      #endregion

      #region basic functions

      static string filename = null;
      public static string SaveLoc()
      {
        if (filename == null)
          filename = XmlSerialization.Serializer.GenerateDefaultFilename("Electroimpact", "BarrelTransformer");

        return filename;
      }

      public static void Save(cFormState fs)
      {
        XmlSerialization.Serializer.Save(fs, SaveLoc());
      }

      public static void OpenMe(ref cFormState fs)
      {
        if (System.IO.File.Exists(cFormState.SaveLoc()))
        {
          fs = XmlSerialization.Serializer.Load<cFormState>(SaveLoc());
        }
        else
        {
          fs = new cFormState();
          cFormState.Save(fs);
        }
      }

      public void SaveMe()
      {
        Save(this);
      }

      public void MoveToStartPos(System.Windows.Forms.Form doggy)
      {
        try
        {
          doggy.Location = IsBizarreLocation(this.fsp, doggy.Size) ? doggy.Location : this.fsp;
        }
        catch (Exception ex)
        {
          MessageBox.Show(ex.Message);
        }
      }

      private bool IsBizarreLocation(Point loc, Size size)
      {
        bool locOkay;

        if (!CheckIfInaScreen(loc, size))
          locOkay = false;
        else
        {
          locOkay = true;
        }
        return !locOkay;
      }

      private bool CheckIfInaScreen(Point loc, Size size)
      {
        bool[] HorizontalOK = new bool[Screen.AllScreens.Length];
        bool[] VerticalOK = new bool[Screen.AllScreens.Length];
        for (int ii = 0; ii < Screen.AllScreens.Length; ii++)
        {
          if ((loc.X > Screen.AllScreens[ii].Bounds.Left) && ((loc.X + size.Width) < Screen.AllScreens[ii].Bounds.Right))
            HorizontalOK[ii] = true;
          if ((loc.Y > Screen.AllScreens[ii].Bounds.Top) && ((loc.Y + size.Height) < Screen.AllScreens[ii].Bounds.Bottom))
            VerticalOK[ii] = true;
        }
        bool OK = false;
        for (int ii = 0; ii < Screen.AllScreens.Length; ii++)
        {
          if (HorizontalOK[ii] && VerticalOK[ii])
            OK = true;
        }
        return OK;
      }
      #endregion

      public bool trySetCNC_IP(string cnc_ip_in)
      {
        System.Net.IPAddress ip;
        if (System.Net.IPAddress.TryParse(cnc_ip_in, out ip))
        {
          cnc_ip = cnc_ip_in;
          SaveMe();
          return true;
        }
        MessageBox.Show(cnc_ip_in + " invalid ip address format.");
        return false;
      }
    }
    #endregion

    #region Constructor ETC
    public frmBarrelTransformer()
    {
      InitializeComponent();
      myBarrelFunction = new cBarrelFunctions();
      setupFormState();
      timerSetup();
    }

    public frmBarrelTransformer(bool NeedSettings)
    {
      InitializeComponent();
      myBarrelFunction = new cBarrelFunctions();
      setupFormState();
      //if (NeedSettings)
      //{
      //  //need this for stand alone Btransformer
      //  Settings.mainSettings = new Settings.cMainSettings();
      //  Settings.mainSettings.SiemensDDEFolderLocation = @"C:\Siemens\Sinumerik\HMI-Advanced";
      //}

    }

    public frmBarrelTransformer(bool NeedSettings, bool Remote)
    {
      InitializeComponent();
      myBarrelFunction = new cBarrelFunctions();
      if (!Remote)
        setupFormState();
      else
        setupFormStateRemote();
      //if (NeedSettings)
      //{
      //  //need this for stand alone Btransformer
      //  Settings.mainSettings = new Settings.cMainSettings();
      //  Settings.mainSettings.SiemensDDEFolderLocation = @"C:\Siemens\Sinumerik\HMI-Advanced";
      //}

    }


    private void setupFormStateRemote()
    {
      this.Size = new Size(425, 400);
      formstate = new cFormState();
      //cFormState.OpenMe(ref formstate); //remote transformers DO NOT like MikesXMLSerializer
      formstate.MoveToStartPos(this);
      myBarrelFunction = new cBarrelFunctions(formstate.LastTransformState);
      menustrip_Barrel_Transform.Checked = formstate.uses_barrel_transform;
      menustrip_v_argument_option.Checked = formstate.uses_uv_table_transform;
      menustrip_useXuFunction_Ax.Checked = formstate.uses_uofx_function;
      menustrip_mm_data.Checked = formstate.uses_mm_data;
      menustrip_multiple_mandrel_option.Checked = formstate.uses_mult_mandrels;
      whichSpinAxis = formstate.fs_spin_axis;
      //rememberIPAddressToolStripMenuItem.Checked = formstate.remember_ip_address_input;



      //update_optionlist();  //remote transformers DO NOT like MikesXMLSerializer
      grpBx_ManualTrans.Enabled = !menustrip_Barrel_Transform.Checked;
      txtbox_transform_args_update();
    }

    private void setupFormState()
    {
      this.Size = new Size(425, 400);
      formstate = new cFormState();
      cFormState.OpenMe(ref formstate);
      formstate.MoveToStartPos(this);
      myBarrelFunction = new cBarrelFunctions(formstate.LastTransformState);
      menustrip_Barrel_Transform.Checked = formstate.uses_barrel_transform;
      menustrip_v_argument_option.Checked = formstate.uses_uv_table_transform;
      menustrip_useXuFunction_Ax.Checked = formstate.uses_uofx_function;
      menustrip_mm_data.Checked = formstate.uses_mm_data;
      menustrip_multiple_mandrel_option.Checked = formstate.uses_mult_mandrels;
      whichSpinAxis = formstate.fs_spin_axis;
      //rememberIPAddressToolStripMenuItem.Checked = formstate.remember_ip_address_input;


      lblTolUnitsAve.Text = lblTolUnitsMax.Text = lblEulerBoxUnits.Text = lblManualUnits.Text = formstate.uses_mm_data ? "mm" : "inches";

      if (formstate.uses_mm_data)
      {
        lblAveErrMax.Text = formstate.AveToleranceMM.ToString("F3");
        lblMaxErrMax.Text = formstate.MaxToleranceMM.ToString("F3");
      }
      else
      {
        lblAveErrMax.Text = (formstate.AveToleranceMM / 25.4).ToString("F3");
        lblMaxErrMax.Text = (formstate.MaxToleranceMM / 25.4).ToString("F3");
      }

      update_optionlist();
      grpBx_ManualTrans.Enabled = !menustrip_Barrel_Transform.Checked;
      txtbox_transform_args_update();
    }

    private void timerSetup()
    {
      tmr = new Timer();
      tmr.Tick += tmr_Tick;
      tmr.Interval = 500;
      tmr.Enabled = true;
    }
    void tmr_Tick(object sender, EventArgs e)
    {
      tmr.Stop();

      if (formstate == null)
      {
        tmr.Start();
        return;
      }

      tmr.Start();
    }

    public bool ConnectToCNC(string IP)
    {
      bool bSuccess = false;

      CNC = new Electroimpact.FANUC.OpenCNC(IP, out err);
      if (err != Electroimpact.FANUC.Err_Code.EW_OK)
      {
        MessageBox.Show("Error connecting to the CNC.");
        bSuccess = false;
      }
      else
        bSuccess = true;

      return bSuccess;

    }

    public bool SendTfrm2CNC_M38S_any(string IPaddress, int which_transform)
    {

      bool bWorkingConnection = ConnectToCNC(IPaddress);
      if (!bWorkingConnection)
        return false;

      double mult = 25.4;
      mult = formstate.uses_mm_data ? 1.0 : 25.4;

      //double d2r = Math.PI / 180.0;

      if (CNC.Connected)
      {
        if (CNC.ReadPMCData("K2.2") == 0)  //if K2.2 is ON then new transform format, holds 18 transforms
        {
          if (which_transform > 0 && which_transform <= 6)
          {

            Int32[] values = new Int32[18];


            values[0] = (Int32)(Math.Round(myBarrelFunction.VaxisToFRC.X * mult, OffsetPower) * OffsetPrecision);
            values[1] = (Int32)(Math.Round(myBarrelFunction.VaxisToFRC.Y * mult, OffsetPower) * OffsetPrecision);
            values[2] = (Int32)(Math.Round(myBarrelFunction.VaxisToFRC.Z * mult, OffsetPower) * OffsetPrecision);
            values[3] = (Int32)(Math.Round(myBarrelFunction.VaxisToFRC.rX.RadiansToDegrees(), AnlgePower) * AnglePrecision);
            values[4] = (Int32)(Math.Round(myBarrelFunction.VaxisToFRC.rY.RadiansToDegrees(), AnlgePower) * AnglePrecision);
            values[5] = (Int32)(Math.Round(myBarrelFunction.VaxisToFRC.rZ.RadiansToDegrees(), AnlgePower) * AnglePrecision);

            Electroimpact.LinearAlgebra.cMatrix EulerInverse = new Electroimpact.LinearAlgebra.cMatrix(myBarrelFunction.PartToUaxis.Inverse());
            double[,] data = EulerInverse.GetMatrix;
            values[6] = (Int32)(Math.Round(data[0, 0], MatrixPower) * MatrixPrecision);
            values[7] = (Int32)(Math.Round(data[0, 1], MatrixPower) * MatrixPrecision);
            values[8] = (Int32)(Math.Round(data[0, 2], MatrixPower) * MatrixPrecision);
            values[9] = (Int32)(Math.Round(data[0, 3] * mult, OffsetPower) * OffsetPrecision);
            values[10] = (Int32)(Math.Round(data[1, 0], MatrixPower) * MatrixPrecision);
            values[11] = (Int32)(Math.Round(data[1, 1], MatrixPower) * MatrixPrecision);
            values[12] = (Int32)(Math.Round(data[1, 2], MatrixPower) * MatrixPrecision);
            values[13] = (Int32)(Math.Round(data[1, 3] * mult, OffsetPower) * OffsetPrecision);
            values[14] = (Int32)(Math.Round(data[2, 0], MatrixPower) * MatrixPrecision);
            values[15] = (Int32)(Math.Round(data[2, 1], MatrixPower) * MatrixPrecision);
            values[16] = (Int32)(Math.Round(data[2, 2], MatrixPower) * MatrixPrecision);
            values[17] = (Int32)(Math.Round(data[2, 3] * mult, OffsetPower) * OffsetPrecision);


            int offset = (which_transform - 1) * 96;
            string Address = "D" + (3300 + offset).ToString() + "*4*18";

            CNC.WritePMCData(Address, values, out err);

            if (err != Electroimpact.FANUC.Err_Code.EW_OK)
            {
              MessageBox.Show("Error communicating with CNC");
              return false;
            }
          }
        }
        else
        {
          //then K2.2 = true and we have 18 transforms:

          if (which_transform > 0 && which_transform <= 18)
          {
            Int32[] values = new Int32[6];


            values[0] = (Int32)(Math.Round(myBarrelFunction.VaxisToFRC.X * mult, OffsetPower) * OffsetPrecision);
            values[1] = (Int32)(Math.Round(myBarrelFunction.VaxisToFRC.Y * mult, OffsetPower) * OffsetPrecision);
            values[2] = (Int32)(Math.Round(myBarrelFunction.VaxisToFRC.Z * mult, OffsetPower) * OffsetPrecision);
            values[3] = (Int32)(Math.Round(myBarrelFunction.VaxisToFRC.rX.RadiansToDegrees(), AnlgePower) * AnglePrecision);
            values[4] = (Int32)(Math.Round(myBarrelFunction.VaxisToFRC.rY.RadiansToDegrees(), AnlgePower) * AnglePrecision);
            values[5] = (Int32)(Math.Round(myBarrelFunction.VaxisToFRC.rZ.RadiansToDegrees(), AnlgePower) * AnglePrecision);


            int offset = (which_transform - 1) * 24;
            string Address = "D" + (3300 + offset).ToString() + "*4*6";

            CNC.WritePMCData(Address, values, out err);

            if (err != Electroimpact.FANUC.Err_Code.EW_OK)
            {
              MessageBox.Show("Error communicating with CNC");
              return false;
            }

          }//end if (which_transform > 0 && which_transform <= 18)

        }//end !if (CNC.ReadPMCData("K2.2") == 0)  //if K2.2 is ON then new transform format, holds 18 transforms

      }//end if (CNC.Connected)
      return true;
    }//end public void SendTfrm2CNC_M38S1(string IPaddress, int which_transform)

    public void SendAutoS41_BTform(string IPaddress)
    {
      #region Barrel Transform
      this.Enabled = false;

      int barrelUnit = 0;
      List<Electroimpact.TransformerInfo.cBarrelTransform> BarrelTrans = TransformerInfo.getListOfBarrelT();

      if (false)// (formstate.uses_mult_mandrels)
      {
        DocuTrackProSE.ListBoxDialog lb = new DocuTrackProSE.ListBoxDialog();
        lb.FormPrompt = "Select the Mandrel Unit being transformed.";
        lb.FormCaption = "Select Mandrel Unit";

        lb.ShowDialog();

        if (lb.DialogResult == DialogResult.Cancel || lb.DialogResult == DialogResult.Yes)
        {
          this.Enabled = true;
          return;
        }

        barrelUnit = lb.InputResponse;
      }

      bool bWorkingConnection = ConnectToCNC(IPaddress);
      if (!bWorkingConnection)
        return;

      {
        {
          Int32[] dong = new Int32[18];
          if (CNC.Connected)
          {
            double mult = formstate.uses_mm_data ? 1.0 : 25.4;
            Int32[] values = new Int32[18];
            Int32[] valuesback = new Int32[18];

            values[0] = (Int32)(Math.Round(myBarrelFunction.PartToUaxis.X * mult, OffsetPower) * OffsetPrecision);
            values[1] = (Int32)(Math.Round(myBarrelFunction.PartToUaxis.Y * mult, OffsetPower) * OffsetPrecision);
            values[2] = (Int32)(Math.Round(myBarrelFunction.PartToUaxis.Z * mult, OffsetPower) * OffsetPrecision);
            values[3] = (Int32)(Math.Round(myBarrelFunction.PartToUaxis.rX.RadiansToDegrees(), AnlgePower) * AnglePrecision);
            values[4] = (Int32)(Math.Round(myBarrelFunction.PartToUaxis.rY.RadiansToDegrees(), AnlgePower) * AnglePrecision);
            values[5] = (Int32)(Math.Round(myBarrelFunction.PartToUaxis.rZ.RadiansToDegrees(), AnlgePower) * AnglePrecision);

            //Saving transform for PartToUaxis
            BarrelTrans[barrelUnit].MandrelToSpin.X = values[0];
            BarrelTrans[barrelUnit].MandrelToSpin.Y = values[1];
            BarrelTrans[barrelUnit].MandrelToSpin.Z = values[2];
            BarrelTrans[barrelUnit].MandrelToSpin.rX = values[3];
            BarrelTrans[barrelUnit].MandrelToSpin.rY = values[4];
            BarrelTrans[barrelUnit].MandrelToSpin.rZ = values[5];

            Electroimpact.LinearAlgebra.cMatrix EulerInverse = new Electroimpact.LinearAlgebra.cMatrix(myBarrelFunction.PartToUaxis.Inverse());
            double[,] data = EulerInverse.GetMatrix;
            values[6] = (Int32)(Math.Round(data[0, 0], MatrixPower) * MatrixPrecision);
            values[7] = (Int32)(Math.Round(data[0, 1], MatrixPower) * MatrixPrecision);
            values[8] = (Int32)(Math.Round(data[0, 2], MatrixPower) * MatrixPrecision);
            values[9] = (Int32)(Math.Round(data[0, 3] * mult, OffsetPower) * OffsetPrecision);
            values[10] = (Int32)(Math.Round(data[1, 0], MatrixPower) * MatrixPrecision);
            values[11] = (Int32)(Math.Round(data[1, 1], MatrixPower) * MatrixPrecision);
            values[12] = (Int32)(Math.Round(data[1, 2], MatrixPower) * MatrixPrecision);
            values[13] = (Int32)(Math.Round(data[1, 3] * mult, OffsetPower) * OffsetPrecision);
            values[14] = (Int32)(Math.Round(data[2, 0], MatrixPower) * MatrixPrecision);
            values[15] = (Int32)(Math.Round(data[2, 1], MatrixPower) * MatrixPrecision);
            values[16] = (Int32)(Math.Round(data[2, 2], MatrixPower) * MatrixPrecision);
            values[17] = (Int32)(Math.Round(data[2, 3] * mult, OffsetPower) * OffsetPrecision);

            string Address = "D7300*4*18";
            CNC.WritePMCData(Address, values, out err);
            if (err != Electroimpact.FANUC.Err_Code.EW_OK)
            {
              MessageBox.Show("Error communicating with CNC");
              this.Enabled = true;
              return;
            }

            valuesback = CNC.ReadPMCRange(Address, out err);
            if (err != Electroimpact.FANUC.Err_Code.EW_OK)
            {
              MessageBox.Show("Error communicating with CNC");
              this.Enabled = true;
              return;
            }
            if (valuesback.Length == values.Length)
            {
              for (int ii = 0; ii < values.Length; ii++)
              {
                if (!Equal2(valuesback[ii], values[ii], 1))
                {
                  MessageBox.Show("Bad Download");
                  this.Enabled = true;
                  return;
                }
              }
            }
            else
            {
              MessageBox.Show("Bad Download");
              this.Enabled = true;
              return;
            }

            if (err != Electroimpact.FANUC.Err_Code.EW_OK)
              MessageBox.Show("Error communicating with CNC");

            values[0] = (Int32)(Math.Round(myBarrelFunction.VaxisToFRC.X * mult, OffsetPower) * OffsetPrecision);
            values[1] = (Int32)(Math.Round(myBarrelFunction.VaxisToFRC.Y * mult, OffsetPower) * OffsetPrecision);
            values[2] = (Int32)(Math.Round(myBarrelFunction.VaxisToFRC.Z * mult, OffsetPower) * OffsetPrecision);
            values[3] = (Int32)(Math.Round(myBarrelFunction.VaxisToFRC.rX.RadiansToDegrees(), AnlgePower) * AnglePrecision);
            values[4] = (Int32)(Math.Round(myBarrelFunction.VaxisToFRC.rY.RadiansToDegrees(), AnlgePower) * AnglePrecision);
            values[5] = (Int32)(Math.Round(myBarrelFunction.VaxisToFRC.rZ.RadiansToDegrees(), AnlgePower) * AnglePrecision);

            //Saving transform for VaxisToFRC
            BarrelTrans[barrelUnit].SpinToFRC.X = values[0];
            BarrelTrans[barrelUnit].SpinToFRC.Y = values[1];
            BarrelTrans[barrelUnit].SpinToFRC.Z = values[2];
            BarrelTrans[barrelUnit].SpinToFRC.rX = values[3];
            BarrelTrans[barrelUnit].SpinToFRC.rY = values[4];
            BarrelTrans[barrelUnit].SpinToFRC.rZ = values[5];

            EulerInverse = new Electroimpact.LinearAlgebra.cMatrix(myBarrelFunction.VaxisToFRC.Inverse());
            data = EulerInverse.GetMatrix;
            values[6] = (Int32)(Math.Round(data[0, 0], MatrixPower) * MatrixPrecision);
            values[7] = (Int32)(Math.Round(data[0, 1], MatrixPower) * MatrixPrecision);
            values[8] = (Int32)(Math.Round(data[0, 2], MatrixPower) * MatrixPrecision);
            values[9] = (Int32)(Math.Round(data[0, 3] * mult, OffsetPower) * OffsetPrecision);
            values[10] = (Int32)(Math.Round(data[1, 0], MatrixPower) * MatrixPrecision);
            values[11] = (Int32)(Math.Round(data[1, 1], MatrixPower) * MatrixPrecision);
            values[12] = (Int32)(Math.Round(data[1, 2], MatrixPower) * MatrixPrecision);
            values[13] = (Int32)(Math.Round(data[1, 3] * mult, OffsetPower) * OffsetPrecision);
            values[14] = (Int32)(Math.Round(data[2, 0], MatrixPower) * MatrixPrecision);
            values[15] = (Int32)(Math.Round(data[2, 1], MatrixPower) * MatrixPrecision);
            values[16] = (Int32)(Math.Round(data[2, 2], MatrixPower) * MatrixPrecision);
            values[17] = (Int32)(Math.Round(data[2, 3] * mult, OffsetPower) * OffsetPrecision);

            Address = "D7396*4*18";
            CNC.WritePMCData(Address, values, out err);
            if (err != Electroimpact.FANUC.Err_Code.EW_OK)
            {
              MessageBox.Show("Error communicating with CNC");
              this.Enabled = true;
              return;
            }

            valuesback = CNC.ReadPMCRange(Address, out err);

            if (err != Electroimpact.FANUC.Err_Code.EW_OK)
            {
              MessageBox.Show("Error communicating with CNC");
              this.Enabled = true;
              return;
            }

            if (valuesback.Length == values.Length)
            {
              for (int ii = 0; ii < values.Length; ii++)
              {
                if (!Equal2(valuesback[ii], values[ii], 1))
                {
                  MessageBox.Show("Bad Download");
                  this.Enabled = true;
                  return;
                }
              }
            }
            else
            {
              MessageBox.Show("Bad Download");
              this.Enabled = true;
              return;
            }


            Address = "D7492*4";
            CNC.WritePMCData(Address, (Int32)(Math.Round(myBarrelFunction.A_x * mult, OffsetPower) * OffsetPrecision));
            if (err != Electroimpact.FANUC.Err_Code.EW_OK)
            {
              MessageBox.Show("Error communicating with CNC");
              this.Enabled = true;
              return;
            }
            int down = (Int32)(Math.Round(myBarrelFunction.A_x * mult, OffsetPower) * OffsetPrecision);
            int back = CNC.ReadPMCData(Address, out err);
            if (err != Electroimpact.FANUC.Err_Code.EW_OK)
            {
              MessageBox.Show("Error communicating with CNC");
              this.Enabled = true;
              return;
            }

            BarrelTrans[barrelUnit].Ax = down;
            TransformerInfo.SaveListOfBarrelT(BarrelTrans);

            if (!Equal2(down, back, 1))
            {
              MessageBox.Show("Bad Download");
              this.Enabled = true;
              return;
            }
          }
        }
      }
      MessageBox.Show("Transform Accepted");
      this.Enabled = true;
      #endregion
    }


    /// <summary>
    /// for use by other applications that reference tranformer.dll"
    /// </summary>
    /// <param name=""></param>
    public void InterfaceTransform(bool useMaxPtErr = false, double maxPtErr_RawUnits = 0.1) //for use by other applications that reference tranformer.dll
    {
      RemoteSolve(useMaxPtErr, maxPtErr_RawUnits, useMaxPtErr); //currently set up to only ignore error points in file if we also use max pt error function too. All others will be unchanged
    }

    /// <summary>
    /// for use by other applications that reference tranformer.dll"
    /// </summary>
    /// <param name=""></param>
    public void InterfaceS41_BTform(string[] lines) //for use by other applications that reference tranformer.dll
    {
      RemoteSolveS41_BTform(lines);
    }




    public void mikesCall(TransformerInfo.cTool tool)
    {
      MessageBox.Show("Method not yet implemented");
      //passedInTool = tool;
      ////Do something to auto solve
    }
    #endregion

    #region stuff related to trasnforming

    public void Transform(double[,] points)
    {
      InitializeComponent();
      myBarrelFunction = new cBarrelFunctions();

      int nRows = points.GetLength(0);
      int nCols = points.GetLength(1);


      //this has a chance of actually working
      if (nCols == 7)
      {
        string spoints = "";
        for (int row = 0; row < nRows; row++)
        {
          string sline = "";
          for (int col = 0; col < 6; col++)
          {
            sline += points[row, col].ToString() + "\t";
          }
          sline += points[row, 6].ToString() + "\n";
          spoints += sline;
        }
        System.Windows.Forms.Clipboard.SetText(spoints);
        //btnPasteInPointsToTransform_Click(this, null);
      }
    }
    //Transform
    //bool b_FirstTimeThru = true;
    public void btnTransform_Click(object sender, EventArgs e)
    {
      this.Enabled = false;
      try
      {
        bool exit = false;
        bool useMaxPtErr = cb_RemoveErrPts.Checked;
        double maxPtErr = double.Parse(lblMaxErrMax.Text);
        while (!exit)
        {
          errorList.Clear();
          txtEulerCheck.Clear();
          this.Refresh();
          System.Threading.Thread.Sleep(50);

          ////if (b_FirstTimeThru)
          //{
          //  myPoints.Clear();
          //  ReadInPoints(Electroimpact.FileIO.cFileOther.GetExecutionDirectoryMethod() + "points.csv");//"points.csv");
          //  label10.Text = Electroimpact.FileIO.cFileOther.GetExecutionDirectoryMethod() + "points.csv";////Directory.//Directory(filename);
          //  //string filename = Electroimpact.FileIO.cFileOther.GetExecutionDirectoryMethod() + "points.csv";
          //}
          ////b_FirstTimeThru = false;


          myBarrelFunction.ClearTransform();

          SolveMeII();


          double rY = Math.Abs(myBarrelFunction.PartToUaxis.rY) * 180.0 / Math.PI;
          SpinAxisOffset180 = false;



          formstate.LastTransformState.setArgs(myBarrelFunction);
          formstate.SaveMe();

          txtbox_transform_args_update();

          #region Output The Results

          using (Electroimpact.FileIO.cFileWriter fw = new FileIO.cFileWriter())
          {
            if (!Electroimpact.FileIO.cFileOther.FileExistsMethod("Transforms.csv"))
            {
              fw.WriteLine("Transforms.csv", "MtoS.X,MtoS.Y,MtoS.Z,MtoS.rX,MtoS.rY,MtoS.rZ,A_x,StoFRC.X,StoFRC.Y,StoFRC.Z,StoFRC.rX,StoFRC.rY,StoFRC.rZ", true);
            }
            fw.WriteLine("Transforms.csv",
                                          myBarrelFunction.PartToUaxis.X.ToString("F6") + "," +
                                          myBarrelFunction.PartToUaxis.Y.ToString("F6") + "," +
                                          myBarrelFunction.PartToUaxis.Z.ToString("F6") + "," +
                                          (myBarrelFunction.PartToUaxis.rX.RadiansToDegrees()).ToString("F6") + "," +
                                          (myBarrelFunction.PartToUaxis.rY.RadiansToDegrees()).ToString("F6") + "," +
                                          (myBarrelFunction.PartToUaxis.rZ.RadiansToDegrees()).ToString("F6") + "," +
                                          myBarrelFunction.A_x.ToString("F6") + "," +
                                          myBarrelFunction.VaxisToFRC.X.ToString("F6") + "," +
                                          myBarrelFunction.VaxisToFRC.Y.ToString("F6") + "," +
                                          myBarrelFunction.VaxisToFRC.Z.ToString("F6") + "," +
                                          (myBarrelFunction.VaxisToFRC.rX.RadiansToDegrees()).ToString("F6") + "," +
                                          (myBarrelFunction.VaxisToFRC.rY.RadiansToDegrees()).ToString("F6") + "," +
                                          (myBarrelFunction.VaxisToFRC.rZ.RadiansToDegrees()).ToString("F6") + ","
                                          , true);
          }
          #endregion

          CheckEuler(sender, e);


          if (useMaxPtErr)
          {
            int maxPtNum = -1;
            double maxErr = 0;
            foreach (cPoint pt in myPoints)
            {
              if (pt.errInXform > maxErr && pt.bUseInXform)
              {
                maxPtNum = pt.point_number;
                maxErr = pt.errInXform;
              }
            }
            if (maxErr > maxPtErr)
            {
              myPoints.First(p => p.point_number == maxPtNum).bUseInXform = false;
              exit = false;
              int remainingPtsToUse = myPoints.Count(p => p.bUseInXform == true);
              if (remainingPtsToUse < 3 || remainingPtsToUse <= Math.Ceiling(myPoints.Count / 2.0))
              {
                exit = true;
                successfulSolve = false;
              }
            }
            else
            {
              exit = true;
            }
          }
          else
          {
            exit = true;
          }
        }
      }
      catch
      {
      }
      this.Enabled = true;
    }

    public double[,] RemoteSolve(bool Refreshpoints, out double RadialError)
    {
      //b_FirstTimeThru = Refreshpoints;
      double[,] ret = RemoteSolve();
      RadialError = CheckEuler(null, null);
      return ret;
    }


    /// <summary>
    /// put your point list in c:\probing\points.csv
    /// </summary>
    public double[,] RemoteSolve(bool useMaxPtErr = false, double maxPtErr_RawUnits = 0.1, bool ignoreReadErrPts = false)
    {
      bool exit = false;
      txtEulerCheck.Clear();
      this.Refresh();
      System.Threading.Thread.Sleep(50);

      //if (b_FirstTimeThru)
      {
        myPoints.Clear();
        ReadInPointsII("C:\\Probing\\points.csv", ignoreReadErrPts);
      }
      //b_FirstTimeThru = false;

      while (!exit)
      {
        txtEulerCheck.Clear();
        errorList.Clear();
        this.Refresh();
        System.Threading.Thread.Sleep(50);
        myBarrelFunction.ClearTransform();

        SolveMeII();


        #region Output The Results

        Electroimpact.FileIO.cFileWriter fw = new Electroimpact.FileIO.cFileWriter();

        if (!Electroimpact.FileIO.cFileOther.FileExistsMethod("Transforms.csv"))
        {
          fw.WriteLine("Transforms.csv", "MtoS.X,MtoS.Y,MtoS.Z,MtoS.rX,MtoS.rY,MtoS.rZ,A_x,StoFRC.X,StoFRC.Y,StoFRC.Z,StoFRC.rX,StoFRC.rY,StoFRC.rZ", true);
        }
        fw.WriteLine("Transforms.csv",
                                      myBarrelFunction.PartToUaxis.X.ToString("F6") + "," +
                                      myBarrelFunction.PartToUaxis.Y.ToString("F6") + "," +
                                      myBarrelFunction.PartToUaxis.Z.ToString("F6") + "," +
                                      (myBarrelFunction.PartToUaxis.rX.RadiansToDegrees()).ToString("F6") + "," +
                                      (myBarrelFunction.PartToUaxis.rY.RadiansToDegrees()).ToString("F6") + "," +
                                      (myBarrelFunction.PartToUaxis.rZ.RadiansToDegrees()).ToString("F6") + "," +
                                      myBarrelFunction.A_x.ToString("F6") + "," +
                                      myBarrelFunction.VaxisToFRC.X.ToString("F6") + "," +
                                      myBarrelFunction.VaxisToFRC.Y.ToString("F6") + "," +
                                      myBarrelFunction.VaxisToFRC.Z.ToString("F6") + "," +
                                      (myBarrelFunction.VaxisToFRC.rX.RadiansToDegrees()).ToString("F6") + "," +
                                      (myBarrelFunction.VaxisToFRC.rY.RadiansToDegrees()).ToString("F6") + "," +
                                      (myBarrelFunction.VaxisToFRC.rZ.RadiansToDegrees()).ToString("F6") + ","
                                      , true);
        #endregion

        CheckEuler(null, null);

        if (useMaxPtErr)
        {
          int maxPtNum = -1;
          double maxErr = 0;
          foreach (cPoint pt in myPoints)
          {
            if (pt.errInXform > maxErr && pt.bUseInXform)
            {
              maxPtNum = pt.point_number;
              maxErr = pt.errInXform;
            }
          }
          if (maxErr > maxPtErr_RawUnits)
          {
            myPoints.First(p => p.point_number == maxPtNum).bUseInXform = false;
            exit = false;
            int remainingPtsToUse = myPoints.Count(p => p.bUseInXform == true);
            int removedPtsDueToTolerance = myPoints.Count(p => p.bUseInXform == false && p.readError == false);
            int totalPtsWithoutReadErr = myPoints.Count(p => p.readError == false);
            if (remainingPtsToUse < 3 || remainingPtsToUse <= Math.Floor(totalPtsWithoutReadErr / 2.0))
            {
              exit = true;
              successfulSolve = false;
            }
          }
          else
          {
            exit = true;
          }
        }
        else
        {
          exit = true;
        }
      }
      return myBarrelFunction.VaxisToFRC.GetMatrix();

    }

    //use this function for Section41 only, if you use it on anything else it will need updates
    public void RemoteSolveS41_BTform(string[] lines)
    {
      txtEulerCheck.Clear();
      this.Refresh();
      System.Threading.Thread.Sleep(50);

      //if (b_FirstTimeThru)
      {
        myPoints.Clear();

        foreach (string line in lines)
        {
          if (line != null)
          {
            cPoint temp = new cPoint(line, false);
            if (temp != null)
              myPoints.Add(temp);
          }
        }
      }
      //b_FirstTimeThru = false;

      menustrip_Barrel_Transform.Checked = true;
      menustrip_v_argument_option.Checked = false;
      formstate.uses_mm_data = false; //Section 41 data is in inches

      myBarrelFunction.ClearTransform();

      SolveMeII();

      CheckEuler(null, null);
    }

    private void txtbox_transform_args_update()
    {
      try
      {
        string[] lines = new string[13];
        lines[0] = myBarrelFunction.PartToUaxis.X.ToString("F6").PadLeft(14); //0; //this can be a huge number that cancels out with myBarrelFunction.VaxisToFRC.X.
        lines[1] = myBarrelFunction.PartToUaxis.Y.ToString("F6").PadLeft(14);
        lines[2] = myBarrelFunction.PartToUaxis.Z.ToString("F6").PadLeft(14);
        lines[3] = (myBarrelFunction.PartToUaxis.rX.RadiansToDegrees()).ToString("F6").PadLeft(14);
        lines[4] = (myBarrelFunction.PartToUaxis.rY.RadiansToDegrees()).ToString("F6").PadLeft(14);
        lines[5] = (myBarrelFunction.PartToUaxis.rZ.RadiansToDegrees()).ToString("F6").PadLeft(14);
        lines[6] = "A_x: " + myBarrelFunction.A_x.ToString("F6").PadLeft(14);
        lines[7] = myBarrelFunction.VaxisToFRC.X.ToString("F6").PadLeft(14);
        lines[8] = myBarrelFunction.VaxisToFRC.Y.ToString("F6").PadLeft(14);
        lines[9] = myBarrelFunction.VaxisToFRC.Z.ToString("F6").PadLeft(14);
        lines[10] = (myBarrelFunction.VaxisToFRC.rX.RadiansToDegrees()).ToString("F6").PadLeft(14);
        lines[11] = (myBarrelFunction.VaxisToFRC.rY.RadiansToDegrees()).ToString("F6").PadLeft(14);
        lines[12] = (myBarrelFunction.VaxisToFRC.rZ.RadiansToDegrees()).ToString("F6").PadLeft(14);

        txtbox_transform_args.Lines = lines;

        txtBx_Manual_X.Text = lines[7].ToString();
        txtBx_Manual_Y.Text = lines[8].ToString();
        txtBx_Manual_Z.Text = lines[9].ToString();
        txtBx_Manual_rX.Text = lines[10].ToString();
        txtBx_Manual_rY.Text = lines[11].ToString();
        txtBx_Manual_rZ.Text = lines[12].ToString();

        double mul = 1.0;
        if( !menustrip_mm_data.Checked )
        {
          mul = 25.4;
        }
        List<string> strFanucBot = new List<string>();
        strFanucBot.Add((myBarrelFunction.VaxisToFRC.X * mul).ToString("F3"));
        strFanucBot.Add((myBarrelFunction.VaxisToFRC.Y * mul).ToString("F3"));
        strFanucBot.Add((myBarrelFunction.VaxisToFRC.Z * mul).ToString("F3"));
        strFanucBot.Add((myBarrelFunction.VaxisToFRC.myMatixClass.rXrYrZFixedAngle[0]).ToString("F3"));
        strFanucBot.Add((myBarrelFunction.VaxisToFRC.myMatixClass.rXrYrZFixedAngle[1]).ToString("F3"));
        strFanucBot.Add((myBarrelFunction.VaxisToFRC.myMatixClass.rXrYrZFixedAngle[2]).ToString("F3"));
        txtFBOT.Lines = strFanucBot.ToArray();

        if (CalculatedSiemensXform.Linear.Length >= 3)
        {
          CalculatedSiemensXform.Linear[0] = myBarrelFunction.VaxisToFRC.X;
          CalculatedSiemensXform.Linear[1] = myBarrelFunction.VaxisToFRC.Y;
          CalculatedSiemensXform.Linear[2] = myBarrelFunction.VaxisToFRC.Z;
        }
        if (CalculatedSiemensXform.Rotational.Length >= 3)
        {
          CalculatedSiemensXform.Rotational[0] = myBarrelFunction.VaxisToFRC.rX;
          CalculatedSiemensXform.Rotational[1] = myBarrelFunction.VaxisToFRC.rY;
          CalculatedSiemensXform.Rotational[2] = myBarrelFunction.VaxisToFRC.rZ;
        }

      }
      catch
      {
      }
    }

    private void ReadInPoints(string FileName)
    {
      using (StreamReader sr = new StreamReader(FileName))
      {
        sr.ReadLine();//throw out the first line.
        while (!sr.EndOfStream)
        {
          string line = sr.ReadLine();
          if (formstate.uses_barrel_transform)
            myPoints.Add(new cPoint(line, formstate.uses_uv_table_transform));
          else
            myPoints.Add(new cPoint(line));
        }
      }
    }

    private void ReadInPointsII(string FileName, bool ignoreReadErrPts = false)
    {
      Electroimpact.FileIO.cFileReader fr = new Electroimpact.FileIO.cFileReader();
      fr.OpenFile(FileName);
      fr.ReadLine();//throw out the first line.
      while (fr.Peek())
      {
        string line = fr.ReadLine();
        myPoints.Add(new cPoint(line));
        if (ignoreReadErrPts && myPoints.Last().readError)
        {
          myPoints.Last().bUseInXform = false;
        }
      }
      fr.CloseFile();
    }

    //this function solves the rotational components of the Spin2FRC transform
    //only returns meaningful data when have > 1 nominal point at > 2 U axis rotations
    //not useable with V axis/double rotator
    private bool SolveForSpinAxis(out double MinA, out double MaxA, out double MinB, out double MaxB, out double MinC, out double MaxC)
    {
      MinA = MinB = MinC = -90.0;
      MaxA = MaxB = MaxC = 90.0;


      int maxIterations = 10000;
      double tol = 0.000001;//error is the average of the radial error squared: 0.001" * 0.001" = 0.000001"


      Boolean success = false;
      int indexBest = 0;
      double[] BestPars;


      //list of points at the center of rotation, should make an approximate line
      List<double[]> centerPoints = new List<double[]>();





      //create list of rotated points to solve for rotation axis vector
      PointsForRot.Clear();
      for (int ii = 0; ii < myPoints.Count; ii++)
      {
        bool bAlreadyInList = false;
        //find center of rotation, then make vector out of it
        //need 3 rotations

        double x, y, z, u;
        //add to list of nominals
        x = myPoints[ii].x_nom;
        y = myPoints[ii].y_nom;
        z = myPoints[ii].z_nom;
        u = myPoints[ii].upos;

        //put the first point in the list
        if (ii == 0)
        {
          //add to list
          rotationPoint newPoint = new rotationPoint();
          newPoint.x_nom = x;
          newPoint.y_nom = y;
          newPoint.z_nom = z;
          newPoint.x_meas.Add(myPoints[ii].x_meas);
          newPoint.y_meas.Add(myPoints[ii].y_meas);
          newPoint.z_meas.Add(myPoints[ii].z_meas);
          newPoint.u_meas.Add(myPoints[ii].upos);
          newPoint.v_meas.Add(myPoints[ii].vpos);
          PointsForRot.Add(newPoint);
        }
        else
        {
          //cycle through list to see if adding a new nominal point
          for (int j = 0; j < PointsForRot.Count; j++)
          {
            if (PointsForRot[j].x_nom == x && PointsForRot[j].y_nom == y && PointsForRot[j].z_nom == z)
            {
              bAlreadyInList = true;
            }
          }

          //if already in the list then add a new rotated value
          if (bAlreadyInList)
          {
            for (int j = 0; j < PointsForRot.Count; j++)
            {
              if (PointsForRot[j].x_nom == x && PointsForRot[j].y_nom == y && PointsForRot[j].z_nom == z)
              {
                bool bUcopy = false;

                for (int k = 0; k < PointsForRot[j].u_meas.Count; k++)
                {
                  if (PointsForRot[j].u_meas[k] == u)
                  {
                    bUcopy = true;
                  }
                }

                //somtimes some idiot puts in a nominal at repeated values of U
                if (!bUcopy)
                {
                  //add to meas lists
                  PointsForRot[j].x_meas.Add(myPoints[ii].x_meas);
                  PointsForRot[j].y_meas.Add(myPoints[ii].y_meas);
                  PointsForRot[j].z_meas.Add(myPoints[ii].z_meas);
                  PointsForRot[j].u_meas.Add(myPoints[ii].upos);
                  PointsForRot[j].v_meas.Add(myPoints[ii].vpos);
                }
              }
            }
          }
          else
          {
            //add new nominal point to list
            rotationPoint newPoint = new rotationPoint();
            newPoint.x_nom = x;
            newPoint.y_nom = y;
            newPoint.z_nom = z;
            newPoint.x_meas.Add(myPoints[ii].x_meas);
            newPoint.y_meas.Add(myPoints[ii].y_meas);
            newPoint.z_meas.Add(myPoints[ii].z_meas);
            newPoint.u_meas.Add(myPoints[ii].upos);
            newPoint.v_meas.Add(myPoints[ii].vpos);
            PointsForRot.Add(newPoint);
          }
        }
      }

      //now have list of all points and their rotations
      //calculate axis of rotation
      foreach (rotationPoint nomPoint in PointsForRot)
      {

        //need 3+ rotations to find center
        if (nomPoint.x_meas.Count < 3)
          continue;

        double[] xsolve = nomPoint.x_meas.ToArray();
        double[] ysolve = nomPoint.y_meas.ToArray();
        double[] zsolve = nomPoint.z_meas.ToArray();

        //find which axis has the least variation, that is the primary spin axis, use later
        double RangeX = xsolve.Max() - xsolve.Min();
        double RangeY = ysolve.Max() - ysolve.Min();
        double RangeZ = zsolve.Max() - zsolve.Min();
        if (RangeX < RangeY && RangeX < RangeZ)
        {
          whichSpinAxis = PrimarySpinAxis.X; //X
        }
        else if (RangeY < RangeX && RangeY < RangeZ)
        {
          whichSpinAxis = PrimarySpinAxis.Y; //Y
        }
        else
        {
          whichSpinAxis = PrimarySpinAxis.Z; //Z
        }



        //solve for center of circle for each point
        //minimize abs(R1^2 - R2^2) + abs(R2^2 - R3^2) + abs(R3^2 - R1^2) ...
        NelderMeade.CRotation myCRot = new NelderMeade.CRotation(tol, xsolve, ysolve, zsolve);
        NelderMeade.SimplexSearch myCRotSimplex = new NelderMeade.SimplexSearch(myCRot, maxIterations);
        ////idea for solving shitty minimal data sets:

        double diffX = nomPoint.x_meas.Max() - nomPoint.x_meas.Min();
        double diffY = nomPoint.y_meas.Max() - nomPoint.y_meas.Min();
        double diffZ = nomPoint.z_meas.Max() - nomPoint.z_meas.Min();

        double dist = Math.Sqrt(diffX * diffX + diffY * diffY + diffZ * diffZ);

        double MinX = 0, MaxX = 0, MinY = 0, MaxY = 0, MinZ = 0, MaxZ = 0;

        double DistFactor = 10.0;
        if (whichSpinAxis == PrimarySpinAxis.X)
        {
          MinX = nomPoint.x_meas.Min();
          MaxX = nomPoint.x_meas.Max();

          MinY = nomPoint.y_meas.Min() - DistFactor * dist;
          MaxY = nomPoint.y_meas.Min() + DistFactor * dist;

          MinZ = nomPoint.z_meas.Min() - DistFactor * dist;
          MaxZ = nomPoint.z_meas.Min() + DistFactor * dist;
        }

        if (whichSpinAxis == PrimarySpinAxis.Y)
        {
          MinX = nomPoint.x_meas.Min() - DistFactor * dist;
          MaxX = nomPoint.x_meas.Max() + DistFactor * dist;

          MinY = nomPoint.y_meas.Min();
          MaxY = nomPoint.y_meas.Min();

          MinZ = nomPoint.z_meas.Min() - DistFactor * dist;
          MaxZ = nomPoint.z_meas.Min() + DistFactor * dist;
        }

        if (whichSpinAxis == PrimarySpinAxis.Z)
        {
          MinX = nomPoint.x_meas.Min() - DistFactor * dist;
          MaxX = nomPoint.x_meas.Max() + DistFactor * dist;

          MinY = nomPoint.y_meas.Min() - DistFactor * dist;
          MaxY = nomPoint.y_meas.Min() + DistFactor * dist;

          MinZ = nomPoint.z_meas.Min();
          MaxZ = nomPoint.z_meas.Min();
        }

        //double[] xMin1 = { nomPoint.x_meas.Min(), nomPoint.y_meas.Min(), nomPoint.z_meas.Min() };
        //double[] xMax1 = { nomPoint.x_meas.Max(), nomPoint.y_meas.Max(), nomPoint.z_meas.Max() };
        double[] xMin1 = { MinX, MinY, MinZ };
        double[] xMax1 = { MaxX, MaxY, MaxZ };

        //execute the optimization search
        success = myCRotSimplex.minimize(xMin1, xMax1);

        indexBest = myCRotSimplex.getBest();
        BestPars = myCRotSimplex.getSimplex(indexBest);

        //add to the list of center points
        centerPoints.Add(BestPars);

        //double err1 = myCRot.performanceFunc(BestPars);
        //BestPars[0] = err1;// = 999;
      }

      if (centerPoints.Count < 2)
      {
        //skip calculation of spin center
        //just solve for solution arbitrarily, which can lead to wonky, yet accurate results...that won't work for Siemens where we must specify the spin axis


        //FANUC: force spin about X if not enough data to solve for spin axis

        //spin about X
        whichSpinAxis = PrimarySpinAxis.X;
        formstate.fs_spin_axis = whichSpinAxis;

        return true;


      }
      else
      {



        //solve for vector in FRC from center points:
        //take 2 points furthest apart in primary axis
        double maxD = 0.0;
        int maxP1 = -1;
        int maxP2 = -1;
        for (int i = 0; i < centerPoints.Count; i++)
        {
          double[] temp1 = centerPoints[i];

          for (int j = 0; j < centerPoints.Count; j++)
          {
            double[] temp2 = centerPoints[j];

            double tempX = Math.Abs(temp2[0] - temp1[0]);
            double tempY = Math.Abs(temp2[1] - temp1[1]);
            double tempZ = Math.Abs(temp2[2] - temp1[2]);
            //double dist_square = tempX * tempX + tempY * tempY + tempZ * tempZ;
            double dist = 0;


            switch (whichSpinAxis)
            {
              case PrimarySpinAxis.X:
                dist = tempX;
                break;
              case PrimarySpinAxis.Y:
                dist = tempY;
                break;
              case PrimarySpinAxis.Z:
                dist = tempZ;
                break;
              default:
                MessageBox.Show("Invalid Siemens Xform Data for Spin Axis");
                dist = tempX;
                break;
            }

            if (dist > maxD)
            {
              maxD = dist;
              maxP1 = i;
              maxP2 = j;
            }
          }
        }

        //find vector
        double[] SpinVector = new double[3];
        double[] p2 = centerPoints[maxP2];
        double[] p1 = centerPoints[maxP1];



        SpinVector[0] = p2[0] - p1[0];
        SpinVector[1] = p2[1] - p1[1];
        SpinVector[2] = p2[2] - p1[2];

        //to make a spin coord system need 3 axes...

        Electroimpact.LinearAlgebra.Vector Xspin, Yspin, Zspin;
        // I put this in because if you input 0 to the vector class with out declaring it as a double variable, it is amibiguous which vector overload to use.  
        double avoidAmbiguity = 0;
        Xspin = new Vector(0, 0, avoidAmbiguity);
        Yspin = new Vector(0, 0, avoidAmbiguity);
        //define a vector to cross with the spin axis to make other 2 axes of spin coord system
        Electroimpact.LinearAlgebra.Vector Zfrc = new Vector(0, 0, 1);//take cross product with FRC [0,0,1] to define our arbitrary Y axis

        if (Math.Abs(SpinVector[0]) > Math.Abs(SpinVector[1]) && Math.Abs(SpinVector[0]) > Math.Abs(SpinVector[2]))
        {
          //then X is biggest component, make it positive
          if (SpinVector[0] < 0)
          {
            SpinVector[0] = -SpinVector[0];
            SpinVector[1] = -SpinVector[1];
            SpinVector[2] = -SpinVector[2];
          }

          //spin about X
          whichSpinAxis = PrimarySpinAxis.X;
          formstate.fs_spin_axis = whichSpinAxis;
          Xspin = new Vector(SpinVector[0], SpinVector[1], SpinVector[2]);
          //Z cross X makes Y
          Yspin = Zfrc.Cross(Xspin);


        }
        else if (Math.Abs(SpinVector[1]) > Math.Abs(SpinVector[0]) && Math.Abs(SpinVector[1]) > Math.Abs(SpinVector[2]))
        {
          //then Y is biggest component, make it positive
          if (SpinVector[1] < 0)
          {
            SpinVector[0] = -SpinVector[0];
            SpinVector[1] = -SpinVector[1];
            SpinVector[2] = -SpinVector[2];
          }

          //spin about Y
          whichSpinAxis = PrimarySpinAxis.Y;
          formstate.fs_spin_axis = whichSpinAxis;
          Yspin = new Vector(SpinVector[0], SpinVector[1], SpinVector[2]);
          //Y cross Z makes X
          Xspin = Yspin.Cross(Zfrc);


        }
        else if (Math.Abs(SpinVector[2]) > Math.Abs(SpinVector[0]) && Math.Abs(SpinVector[2]) > Math.Abs(SpinVector[1]))
        {
          //then Z is biggest component, make it positive
          if (SpinVector[2] < 0)
          {
            SpinVector[0] = -SpinVector[0];
            SpinVector[1] = -SpinVector[1];
            SpinVector[2] = -SpinVector[2];
          }

          //spin about Z
          whichSpinAxis = PrimarySpinAxis.Z;
          formstate.fs_spin_axis = whichSpinAxis;

          MessageBox.Show("Error, Cannot spin about FRC Z axis, Aborting Transform");
          return false;
        }

        //cross SpinX and SpinY to get SpinZ
        Zspin = Xspin.Cross(Yspin);


        //make unit vector
        double denom = Math.Sqrt(SpinVector[0] * SpinVector[0] + SpinVector[1] * SpinVector[1] + SpinVector[2] * SpinVector[2]);
        if (denom == 0)
        {
          MessageBox.Show("Failed to solve for spin axis. Transform aborted.");
          return false;
        }
        SpinVector[0] /= denom;
        SpinVector[1] /= denom;
        SpinVector[2] /= denom;





        //make a cmatrix and pull out angles of rotation
        double[,] dSpin = { { Xspin[0], Yspin[0], Zspin[0], 0 }, { Xspin[1], Yspin[1], Zspin[1], 0 }, { Xspin[2], Yspin[2], Zspin[2], 0 }, { 0, 0, 0, 1 } };
        Electroimpact.LinearAlgebra.cMatrix SpinMatrix = new cMatrix(dSpin);
        double A_angle = SpinMatrix.rXrYrZ[0, 0];
        double B_angle = SpinMatrix.rXrYrZ[1, 0];
        double C_angle = SpinMatrix.rXrYrZ[2, 0];



        //set min and max values to solve
        MinA = MaxA = A_angle;
        MinB = MaxB = B_angle;
        MinC = MaxC = C_angle;

        double Margin = 0;//degrees, still optimize within this

        MinA -= Margin;
        MinB -= Margin;
        MinC -= Margin;

        MaxA += Margin;
        MaxB += Margin;
        MaxC += Margin;

        return true;
      }


    }

    private void SolveMeII()
    {

      int maxIterations = 10000;
      double tol = 0.000001;//error is the average of the radial error squared: 0.001" * 0.001" = 0.000001"


      Boolean success = false;
      int indexBest = 0;
      double[] BestPars;



      //Spin2FRC maxs and mins
      //when possible set these values using SolveForSpinAxis()
      double Amin = -90.0;
      double Amax = 90.0;
      double Bmin = -90.0;
      double Bmax = 90.0;
      double Cmin = -90.0;
      double Cmax = 90.0;

      //joshc
      if (menustrip_Barrel_Transform.Checked && !menustrip_v_argument_option.Checked)
      {
        bool bSpinSolved = false;
        bSpinSolved = SolveForSpinAxis(out Amin, out Amax, out Bmin, out Bmax, out Cmin, out Cmax);
        if (!bSpinSolved)
          return; //do not solve, could not solve spin axis for Siemens
      }


      //create object to solve
      NelderMeade.BTformer myTform = new NelderMeade.BTformer(tol, this);
      //assign object to the simplex search
      NelderMeade.SimplexSearch mySimplex = new NelderMeade.SimplexSearch(myTform, maxIterations);



      //initial min/max are not hard limits, but are just used to create initial guesses
      //           U2FRC.rX, rY,  rZ, A_x,Part2U.X, .Y, .Z, .rX, .rY, .rZ
      //restrict 0<Part2U.x < 0 because can get all X translation in U2FRC
      //double[] xMin = { -90, -90, -90, 0, 0, -1000, -1000, -90, -90, -90 };
      //double[] xMax = {  90,   0,   0, 0, 0,  1000,  1000,  90,  90,  90 };
      //double[] xMin = { Amin, Bmin, Cmin, 0, 0, -1000, -1000, -90, -90, -90 };
      //double[] xMax = { Amax, Bmax, Cmax, 0, 0, 1000, 1000, 90, 90, 90 };
      double[] xMin = { Amin, Bmin, Cmin, -100, 0, -1000, -1000, -10, -10, -10 };
      double[] xMax = { Amax, Bmax, Cmax, 100, 0, 1000, 1000, 10, 10, 10 };



      //change which Part2U translation to lock
      if (whichSpinAxis == PrimarySpinAxis.Y)
      {
        //allow X translation to be nonzero
        xMin[4] = -1000;
        xMax[4] = 1000;

        //lock the Y translation to 0
        xMin[5] = 0;
        xMax[5] = 0;
      }

      if (whichSpinAxis == PrimarySpinAxis.Z)
      {
        MessageBox.Show("Spin about FRC Z not developed.  Transform Aborted.");
        return;
      }

      ////removing this, now trying to do vector first...
      //if (menu_strip_SiemensCNC.Checked && menustrip_Barrel_Transform.Checked)
      //{
      //  //if Siemens then take out the rX in the RotatorToFRC transform, G54
      //  xMin[0] = 0.0;
      //  xMax[0] = 0.0;
      //}

      //double[] ret = myBarrelFunction.BarrelToFRC(myPoints[ii]);
      //double xerr = (ret[0] - myPoints[ii].x_meas);
      //double yerr = (ret[1] - myPoints[ii].y_meas);
      //double zerr = (ret[2] - myPoints[ii].z_meas);

      //execute the optimization search
      success = mySimplex.minimize(xMin, xMax);

      indexBest = mySimplex.getBest();
      BestPars = mySimplex.getSimplex(indexBest);

      //final update to put the best values in barrel transformer
      CalculateError(BestPars);
      successfulSolve = myPoints.Count(p => p.bUseInXform) >= 3;
      return;
    }

    public double CalculateError(double[] Vars)
    {
      myBarrelFunction.VaxisToFRC.X = myBarrelFunction.VaxisToFRC.Y = myBarrelFunction.VaxisToFRC.Z = 0;
      myBarrelFunction.VaxisToFRC.rX = Vars[0].DegreesToRadians();
      myBarrelFunction.VaxisToFRC.rY = Vars[1].DegreesToRadians();
      myBarrelFunction.VaxisToFRC.rZ = Vars[2].DegreesToRadians();

      if (menustrip_Barrel_Transform.Checked)
      {
        if (formstate.uses_uofx_function)
          myBarrelFunction.A_x = Vars[3];
        else
          myBarrelFunction.A_x = 0.0;


        myBarrelFunction.PartToUaxis.X = Vars[4];
        myBarrelFunction.PartToUaxis.Y = Vars[5];
        myBarrelFunction.PartToUaxis.Z = Vars[6];
        myBarrelFunction.PartToUaxis.rX = Vars[7].DegreesToRadians();
        myBarrelFunction.PartToUaxis.rY = Vars[8].DegreesToRadians();
        myBarrelFunction.PartToUaxis.rZ = Vars[9].DegreesToRadians();



        if (chkStupid.Checked)
        {
          myBarrelFunction.A_x = 0;
          myBarrelFunction.PartToUaxis.X = 0;
          myBarrelFunction.PartToUaxis.Y = 0;
          myBarrelFunction.PartToUaxis.Z = 0;
          myBarrelFunction.PartToUaxis.rX = 0;
          myBarrelFunction.PartToUaxis.rY = 0;
          myBarrelFunction.PartToUaxis.rZ = 0;
        }
      }
      else
      {
        myBarrelFunction.A_x = 0;
        myBarrelFunction.PartToUaxis.X = 0;
        myBarrelFunction.PartToUaxis.Y = 0;
        myBarrelFunction.PartToUaxis.Z = 0;
        myBarrelFunction.PartToUaxis.rX = 0;
        myBarrelFunction.PartToUaxis.rY = 0;
        myBarrelFunction.PartToUaxis.rZ = 0;
      }


      double error = 0.0;
      double Txerr = 0;
      double Tyerr = 0;
      double Tzerr = 0;
      int number_of_points_to_solve = 0;
      for (int ii = 0; ii < myPoints.Count; ii++)
      {

        double[] ret;
        if (whichSpinAxis == PrimarySpinAxis.X)
          ret = myBarrelFunction.BarrelToFRC(myPoints[ii]);
        else
          ret = myBarrelFunction.BarrelToFRC_AboutY(myPoints[ii]);
        //double[] ret = myBarrelFunction.BarrelToFRC_AboutY(myPoints[ii]);
        double xerr = (ret[0] - myPoints[ii].x_meas);
        double yerr = (ret[1] - myPoints[ii].y_meas);
        double zerr = (ret[2] - myPoints[ii].z_meas);

        if (myPoints[ii].bUseInXform)
          number_of_points_to_solve++;
        else
          continue;

        Txerr += xerr;
        Tyerr += yerr;
        Tzerr += zerr;

        error += (xerr * xerr + yerr * yerr + zerr * zerr);
      }

      Txerr /= (double)number_of_points_to_solve;
      Tyerr /= (double)number_of_points_to_solve;
      Tzerr /= (double)number_of_points_to_solve;

      myBarrelFunction.VaxisToFRC.X = -Txerr;
      myBarrelFunction.VaxisToFRC.Y = -Tyerr;
      myBarrelFunction.VaxisToFRC.Z = -Tzerr;


      error = 0.0;
      for (int ii = 0; ii < myPoints.Count; ii++)
      {
        if (!myPoints[ii].bUseInXform)
          continue;

        double[] ret;
        if (whichSpinAxis == PrimarySpinAxis.X)
          ret = myBarrelFunction.BarrelToFRC(myPoints[ii]);
        else
          ret = myBarrelFunction.BarrelToFRC_AboutY(myPoints[ii]);
        //double[] ret = myBarrelFunction.BarrelToFRC_AboutY(myPoints[ii]);
        double xerr = (ret[0] - myPoints[ii].x_meas);
        double yerr = (ret[1] - myPoints[ii].y_meas);
        double zerr = (ret[2] - myPoints[ii].z_meas);
        error += (xerr * xerr + yerr * yerr + zerr * zerr);
      }


      //return error modulated by number of points 
      //this is NOT the average error, it is the average of the squares
      //the radial error is 
      error /= (double)number_of_points_to_solve;

      return error;

    }

    #endregion

    #region Button Clicks etc

    private double CheckEuler(object sender, EventArgs e)
    {
      checked_points_list.Items.Clear();

      if (sender != null)
      {
        lblPointsInXform.Text = "Points in Transform.  Units are " + (formstate.uses_mm_data ? "mm." : "inch.");
      }
      else
      {
        lblPointsInXform.Text = "Points in Transform.  Units are " + "mm.";
      }

      double d2r = Math.PI / 180.0;
      System.Collections.Generic.List<string> outputString = new List<string>();
      Console.WriteLine("Converting Tool Positions to MRS: ");

      Electroimpact.LinearAlgebra.c6dof PartToUaxisInverse = new Electroimpact.LinearAlgebra.c6dof(myBarrelFunction.PartToUaxis.Inverse());
      Electroimpact.LinearAlgebra.c6dof VaxisToFRCInverse = new Electroimpact.LinearAlgebra.c6dof(myBarrelFunction.VaxisToFRC.Inverse());
      Electroimpact.LinearAlgebra.RxMatrix u = new Electroimpact.LinearAlgebra.RxMatrix();
      double ErrorEuler = 0;
      double MaxRadial = 0;
      double TotalRadial = 0;

      int total_points_in_transform = 0;

      for (int ii = 0; ii < myPoints.Count; ii++)
      {
        double[] point = new double[4];
        double[] PointOutput;

        double xerr;
        double yerr;
        double zerr;


        if (whichSpinAxis == PrimarySpinAxis.X)
          PointOutput = myBarrelFunction.BarrelToFRC(myPoints[ii]);
        else
          PointOutput = myBarrelFunction.BarrelToFRC_AboutY(myPoints[ii]);


        xerr = PointOutput[0] - myPoints[ii].x_meas;
        yerr = PointOutput[1] - myPoints[ii].y_meas;
        zerr = PointOutput[2] - myPoints[ii].z_meas;
        ErrorEuler += (xerr * xerr + yerr * yerr + zerr * zerr);

        if (myPoints[ii].bUseInXform)
        {
          MaxRadial = Math.Sqrt((xerr * xerr + yerr * yerr + zerr * zerr)) > MaxRadial ? Math.Sqrt((xerr * xerr + yerr * yerr + zerr * zerr)) : MaxRadial;
          TotalRadial += Math.Sqrt((xerr * xerr + yerr * yerr + zerr * zerr));
          total_points_in_transform++;
        }

        myPoints[ii].errInXform = Math.Sqrt(xerr * xerr + yerr * yerr + zerr * zerr);
        myPoints[ii].point_number = ii + 1;
        checked_points_list.Items.Add(myPoints[ii], myPoints[ii].bUseInXform);

        errorList.Add(myPoints[ii].errInXform);

        outputString.Add("Data for Point: " + (ii + 1).ToString());
        if (myPoints[ii].bUseInXform)
          outputString.Add("This point is used in the transform");
        else
          outputString.Add("This point is NOT used in the transform");

        outputString.Add("Regular 6DOF method: ");
        outputString.Add("X nominal:" + myPoints[ii].x_nom.ToString("F3").PadLeft(9) + " -->" + PointOutput[0].ToString("F3").PadLeft(9) + " Measured: " + myPoints[ii].x_meas.ToString("F3").PadLeft(9) + " error: " + xerr.ToString("F3").PadLeft(6));
        outputString.Add("Y nominal:" + myPoints[ii].y_nom.ToString("F3").PadLeft(9) + " -->" + PointOutput[1].ToString("F3").PadLeft(9) + " Measured: " + myPoints[ii].y_meas.ToString("F3").PadLeft(9) + " error: " + yerr.ToString("F3").PadLeft(6));
        outputString.Add("Z nominal:" + myPoints[ii].z_nom.ToString("F3").PadLeft(9) + " -->" + PointOutput[2].ToString("F3").PadLeft(9) + " Measured: " + myPoints[ii].z_meas.ToString("F3").PadLeft(9) + " error: " + zerr.ToString("F3").PadLeft(6));

        cPoint p = new cPoint(0, 0, 0, PointOutput[0], PointOutput[1], PointOutput[2], myPoints[ii].upos, myPoints[ii].vpos);
        double[] BackToFwd;// = myBarrelFunction.FRCtoBarrel_AboutY(p);
        if (whichSpinAxis == PrimarySpinAxis.X)
          BackToFwd = myBarrelFunction.FRCtoBarrel(p);
        else
          BackToFwd = myBarrelFunction.FRCtoBarrel_AboutY(p);

        outputString.Add(PointOutput[0].ToString("F3").PadLeft(9) + " --> " + BackToFwd[0].ToString("F3").PadLeft(9));
        outputString.Add(PointOutput[1].ToString("F3").PadLeft(9) + " --> " + BackToFwd[1].ToString("F3").PadLeft(9));
        outputString.Add(PointOutput[2].ToString("F3").PadLeft(9) + " --> " + BackToFwd[2].ToString("F3").PadLeft(9));

        outputString.Add("End of Point: " + ii.ToString());
        outputString.Add("");
      }
      ErrorEuler = Math.Sqrt(ErrorEuler);
      outputString.Insert(0, "");

      string sz_unit = " mm ";
      string sz_alternate = " inch ";
      double mult = 25.4;

      if (sender != null)
      {
        sz_unit = formstate.uses_mm_data ? " mm " : " inch ";
        sz_alternate = formstate.uses_mm_data ? " inch " : " mm ";
        mult = formstate.uses_mm_data ? 1 / 25.4 : 25.4;
      }

      ErrAve = (TotalRadial / (double)total_points_in_transform);

      outputString.Insert(1, "*** units are in" + sz_unit + "unless noted otherwise ***");
      outputString.Insert(2, "");
      outputString.Insert(3, "Euler Fit (sqrt(sumsq()): " + ErrorEuler.ToString("F3"));
      outputString.Insert(4, "Euler Fit (Average): " + ErrAve.ToString("F3"));
      outputString.Insert(5, "Euler Fit (Max Radial): " + MaxRadial.ToString("F3"));
      outputString.Insert(6, "");
      outputString.Insert(7, "*** the next three items' units are " + sz_alternate + " ***");
      outputString.Insert(8, "");
      outputString.Insert(9, "Euler Fit (sqrt(sumsq()): " + (ErrorEuler * mult).ToString("F3"));
      outputString.Insert(10, "Euler Fit (Average): " + (TotalRadial * mult / (double)total_points_in_transform).ToString("F3"));
      outputString.Insert(11, "Euler Fit (Max Radial): " + (MaxRadial * mult).ToString("F3"));
      outputString.Insert(12, "\n\n");
      outputString.Add("");
      outputString.Add("End of Check");
      outputString.Add("");
      outputString.Add("");
      outputString.Add("");

      #region Check some rotations

      //From Tool to FRC

      //Practice Transform
      //Electroimpact.LinearAlgebra.c6dof Practice = new Electroimpact.LinearAlgebra.c6dof(10, 10, 10, 179.824*d2r, -.184*d2r, .135*d2r);
      Electroimpact.LinearAlgebra.cMatrix fwd = new Electroimpact.LinearAlgebra.cMatrix(myBarrelFunction.PartToUaxis.GetMatrix());
      Electroimpact.LinearAlgebra.cMatrix back = new Electroimpact.LinearAlgebra.cMatrix(fwd.Inverse());
      Electroimpact.LinearAlgebra.c6dof Rot = new Electroimpact.LinearAlgebra.c6dof(0, 0, 0, 0 * d2r, 0 * d2r, 0);
      Electroimpact.LinearAlgebra.cMatrix ret = new Electroimpact.LinearAlgebra.cMatrix(fwd.DotMe(Rot.GetMatrix()));
      outputString.Add("Rx: " + (Rot.rX / d2r).ToString("F3").PadLeft(9) + " --> " + ret.rXrYrZ[0, 0].ToString("F3").PadLeft(9));
      outputString.Add("Ry: " + (Rot.rY / d2r).ToString("F3").PadLeft(9) + " --> " + ret.rXrYrZ[1, 0].ToString("F3").PadLeft(9));
      outputString.Add("Rz: " + (Rot.rZ / d2r).ToString("F3").PadLeft(9) + " --> " + ret.rXrYrZ[2, 0].ToString("F3").PadLeft(9));

      //FRC to Tool
      Rot = new Electroimpact.LinearAlgebra.c6dof(0, 0, 0, ret.rXrYrZ[0, 0] * d2r, ret.rXrYrZ[1, 0] * d2r, ret.rXrYrZ[2, 0] * d2r);
      ret = new Electroimpact.LinearAlgebra.cMatrix(back.DotMe(Rot.GetMatrix()));
      outputString.Add("Rx: " + (Rot.rX.RadiansToDegrees()).ToString("F3").PadLeft(9) + " --> " + ret.rXrYrZ[0, 0].ToString("F3").PadLeft(9));
      outputString.Add("Ry: " + (Rot.rY.RadiansToDegrees()).ToString("F3").PadLeft(9) + " --> " + ret.rXrYrZ[1, 0].ToString("F3").PadLeft(9));
      outputString.Add("Rz: " + (Rot.rZ.RadiansToDegrees()).ToString("F3").PadLeft(9) + " --> " + ret.rXrYrZ[2, 0].ToString("F3").PadLeft(9));

      #endregion

      string[] lines = new string[outputString.Count];
      for (int ii = 0; ii < outputString.Count; ii++)
      {
        lines[ii] = outputString[ii];
      }
      txtEulerCheck.Lines = lines;
      EulerCheckLines = lines;

      string sPass = "SUCCESS";

      
      if (double.IsNaN(ErrAve) || ErrAve * mult > formstate.AveToleranceMM || MaxRadial * mult > formstate.MaxToleranceMM || !successfulSolve)
      {
        txtBxSimpleResults.BackColor = Color.Red;
        formstate.bGoodTransformFit = false;
        sPass = "BAD TRANSFORM";

      }
      else
      {
        txtBxSimpleResults.BackColor = Color.Green;
        formstate.bGoodTransformFit = true;
        sPass = "SUCCESS";

      }

      string[] SimpleLines = new string[11];

      SimpleLines[0] = "Transformed at: " + DateTime.Now.ToString();
      SimpleLines[1] = "";
      SimpleLines[2] = sPass;
      SimpleLines[3] = "";
      SimpleLines[4] = "*** Error Units of: " + sz_unit + " ***";
      SimpleLines[5] = "Average Error: " + ErrAve.ToString("F3");
      SimpleLines[6] = "Worst Point: " + MaxRadial.ToString("F3");
      SimpleLines[7] = "";
      SimpleLines[8] = "*** Error Units of: " + sz_alternate + " ***";
      SimpleLines[9] = "Average Error: " + (TotalRadial * mult / (double)total_points_in_transform).ToString("F3");
      SimpleLines[10] = "Worst Point: " + (MaxRadial * mult).ToString("F3");
      txtBxSimpleResults.Lines = SimpleLines;



      return MaxRadial;
    }


    //send to CNC
    private void btnNullComp_Click(object sender, EventArgs e)
    {
      if (formstate.uses_barrel_transform)
      {
        #region Barrel Identy


        if (!SetIPandConnect())
          return;

        {
          {
            Int32[] dong = new Int32[18];
            if (CNC.Connected)
            {

              dong[0] = 0;//0
              dong[1] = 0;//4
              dong[2] = 0;//8
              dong[3] = 0;//12
              dong[4] = 0;//16
              dong[5] = 0;//20

              //Identity Matrix
              dong[6] = (Int32)(1 * 1e6);
              dong[7] = 0;
              dong[8] = 0;
              dong[9] = 0;

              dong[10] = 0;
              dong[11] = (Int32)(1 * 1e6);
              dong[12] = 0;
              dong[13] = 0;

              dong[14] = 0;
              dong[15] = 0;
              dong[16] = (Int32)(1 * 1e6);
              dong[17] = 0;

              string Address = "D7300*4*18";
              CNC.WritePMCData(Address, dong, out err);

              Address = "D7396*4*18";
              CNC.WritePMCData(Address, dong, out err);

              Address = "D7492*4";
              dong[0] = 0;
              CNC.WritePMCData(Address, dong, out err);
            }
          }
        }
        #endregion
      }
      else
      {
        #region Regular Identy


        if (!SetIPandConnect())
          return;

        int result;

        List<string> TransName = TransformerInfo.getListOfTransforms();

        DocuTrackProSE.ListBoxDialog2 lb = new DocuTrackProSE.ListBoxDialog2();

        lb.FormPrompt = "Select which transform";
        lb.FormCaption = "Current Transforms";
        lb.Old = CNC.ReadPMCData("K2.2") == 0;
        lb.PassedInTool = passedInTool;
        int i = 0;
        foreach (string Name in TransName)
        {
          lb.T[i] = (i + 1).ToString() + " - " + Name;
          i++;
        }

        lb.ShowDialog();

        if (lb.DialogResult == DialogResult.Cancel || lb.InputResponse == "kejbnasdjkfnsdj")
        {
          CNC.Dispose();
          return;
        }
        string transform = lb.InputResponse.Substring(0, 1);
        result = Convert.ToInt16(transform);
        string comment = lb.InputResponse.Substring(4);

        TransName[result - 1] = comment;
        TransformerInfo.saveTransformNames(TransName);

        if (CNC.ReadPMCData("K2.2") == 0)  //if K2.2 is ON then new transform format, holds 18 transforms
        {
          //old transform format, holds 6 transforms

          if (result > 0 && result < 6)
          {
            Int32[] dong = new Int32[18];
            if (CNC.Connected)
            {

              dong[0] = 0;
              dong[1] = 0;
              dong[2] = 0;
              dong[3] = 0;
              dong[4] = 0;
              dong[5] = 0;

              //Identity Matrix
              dong[6] = (Int32)(1 * 1e6);
              dong[7] = 0;
              dong[8] = 0;
              dong[9] = 0;

              dong[10] = 0;
              dong[11] = (Int32)(1 * 1e6);
              dong[12] = 0;
              dong[13] = 0;

              dong[14] = 0;
              dong[15] = 0;
              dong[16] = (Int32)(1 * 1e6);
              dong[17] = 0;


              int offset = (result - 1) * 96;
              string Address = "D" + (3300 + offset).ToString() + "*4*18";
              //"D4400*4*24"

              CNC.WritePMCData(Address, dong, out err);
              CNC.Dispose();
            }
          }
        }
        else
        {
          //new transform format, holds 18 transforms
          if (result > 0 && result <= 18)
          {
            Int32[] dong = new Int32[6];
            if (CNC.Connected)
            {

              dong[0] = 0;
              dong[1] = 0;
              dong[2] = 0;
              dong[3] = 0;
              dong[4] = 0;
              dong[5] = 0;

              int offset = (result - 1) * 24;
              string Address = "D" + (3300 + offset).ToString() + "*4*6";
              //"D4400*4*24"

              CNC.WritePMCData(Address, dong, out err);
            }
          }
          CNC.Dispose();
        }
        MessageBox.Show("Identity Accepted");
        #endregion
      }
    }

    public void SetDataUnits(bool useMillimeters)
    {

      menustrip_mm_data.Checked = useMillimeters;
      formstate.uses_mm_data = menustrip_mm_data.Checked;
      update_optionlist();
    }



    private void btnCopyEuler_Click(object sender, EventArgs e)
    {
      string output = GetEuler(myBarrelFunction.PartToUaxis);
      System.Windows.Forms.Clipboard.SetText(output);
    }

    private void btnCopyVaxisToFRC_Click(object sender, EventArgs e)
    {
      string output = GetEuler(myBarrelFunction.VaxisToFRC);
      System.Windows.Forms.Clipboard.SetText(output);
    }

    public double[] GetEuler()
    {

      double mult = formstate.uses_mm_data ? 1.0 : 1.0 / 25.4;

      double[] values = new double[6];
      values[0] = myBarrelFunction.VaxisToFRC.X * mult;
      values[1] = myBarrelFunction.VaxisToFRC.Y * mult;
      values[2] = myBarrelFunction.VaxisToFRC.Z * mult;
      values[3] = myBarrelFunction.VaxisToFRC.rX.RadiansToDegrees();
      values[4] = myBarrelFunction.VaxisToFRC.rY.RadiansToDegrees();
      values[5] = myBarrelFunction.VaxisToFRC.rZ.RadiansToDegrees();

      return values;
    }

    private string GetEuler(Electroimpact.LinearAlgebra.c6dof Transform)
    {
      double d2r = Math.PI / 180.0;
      string output = "";
      output += Transform.X.ToString("F10") + ",";
      output += Transform.Y.ToString("F10") + ",";
      output += Transform.Z.ToString("F10") + ",";
      output += (Transform.rX.RadiansToDegrees()).ToString("F10") + ",";
      output += (Transform.rY.RadiansToDegrees()).ToString("F10") + ",";
      output += (Transform.rZ.RadiansToDegrees()).ToString("F10") + ",";
      Electroimpact.LinearAlgebra.cMatrix Minv = new Electroimpact.LinearAlgebra.cMatrix(Transform.GetMatrix());
      Minv.InvertMe();
      double[,] EulerInverse = Minv.GetMatrix;
      for (int row = 0; row < 3; row++)
      {
        for (int col = 0; col < 4; col++)
        {
          output += EulerInverse[row, col].ToString("F10") + ",";
        }
      }
      output = output.Remove(output.Length - 1);
      return output;
    }

    private void btnPasteInPointsToTransform_Click(object sender, EventArgs e)
    {
      if (PasteInPoints())
        btnTransform_Click(sender, e);
    }

    public bool PasteInPoints()
    {
      //b_FirstTimeThru = true;
      string textIN = System.Windows.Forms.Clipboard.GetText();
      string[] lines = textIN.Split('\n');
      using (Electroimpact.FileIO.cFileWriter fw = new FileIO.cFileWriter())
      {
        string filename = Electroimpact.FileIO.cFileOther.GetExecutionDirectoryMethod() + "points.csv";
        //label9.Text = filename;
        if (formstate.uses_barrel_transform)
        {
          if (formstate.uses_uv_table_transform)
            fw.WriteLine(filename, "Xnom,Ynom,Znom,Upos,Vpos,Xmeas,Ymeas,Zmeas", false);
          else
            fw.WriteLine(filename, "Xnom,Ynom,Znom,Upos,Xmeas,Ymeas,Zmeas", false);
        }
        else
        {
          fw.WriteLine(filename, "Xnom,Ynom,Znom,Xmeas,Ymeas,Zmeas", false);
        }

        int nArgs = formstate.uses_uv_table_transform ? 8 : 7;

        if (!formstate.uses_barrel_transform) // we are doing a regular rigid body transform
          nArgs = 6;

        for (int ii = 0; ii < lines.Length - 1; ii++)
        {
          string[] items = lines[ii].Split('\t');
          if (items.Length == nArgs)
          {
            double[] values = new double[nArgs];
            string line = "";
            for (int jj = 0; jj < nArgs; jj++)
            {
              values[jj] = double.Parse(items[jj]);
              line += values[jj].ToString("F6") + ",";
            }
            fw.WriteLine(filename, line, true);
          }
          else
          {
            if (formstate.uses_barrel_transform)
            {
              if (formstate.uses_uv_table_transform)
              {
                MessageBox.Show(lines[ii] + " wrong number of arguments. \n expected \"Xnom,Ynom,Znom,Upos,Vpos,Xmeas,Ymeas,Zmeas\"\nMake sure you intend to use v_table transform.");
                return false;
              }
              else
              {
                MessageBox.Show(lines[ii] + " wrong number of arguments. \n expected \"Xnom,Ynom,Znom,Upos,Xmeas,Ymeas,Zmeas\"\nDid you intend to use v_table transform?");
                return false;
              }
            }
            else
            {
              MessageBox.Show(lines[ii] + " wrong number of arguments. \n expected \"Xnom,Ynom,Znom,Xmeas,Ymeas,Zmeas\"\nDid you intend to use a barrel transform?");
              return false;
            }
          }
        }
      }
      Console.WriteLine(textIN);




      myPoints.Clear();
      ReadInPoints(Electroimpact.FileIO.cFileOther.GetExecutionDirectoryMethod() + "points.csv");//"points.csv");
      //label10.Text = Electroimpact.FileIO.cFileOther.GetExecutionDirectoryMethod() + "points.csv";////Directory.//Directory(filename);



      return true;
    }

    private void btnCopyEulerMatrix_Click(object sender, EventArgs e)
    {
      double[,] y_measEuler = myBarrelFunction.PartToUaxis.GetMatrix();
      string ret = "";

      for (int row = 0; row < 4; row++)
      {
        for (int col = 0; col < 4; col++)
        {
          ret += y_measEuler[row, col].ToString("F6") + ",";
        }
        ret += "\n";
      }
      System.Windows.Forms.Clipboard.SetText(ret);
    }

    private void btnPastePoints_Click(object sender, EventArgs e)
    {
      Transform(false);
    }

    private void Transform(bool reverse)
    {
      string textIN = System.Windows.Forms.Clipboard.GetText();
      string[] lines = textIN.Split('\n');
      System.Collections.Generic.List<Electroimpact.LinearAlgebra.c6dof> myVectors = new List<Electroimpact.LinearAlgebra.c6dof>();
      bool _3space = true;
      int n_args_3space = formstate.uses_uv_table_transform ? 5 : 4;
      int n_args_6space = formstate.uses_uv_table_transform ? 8 : 7;

      if (!formstate.uses_barrel_transform)
      {
        n_args_3space = 3;
        n_args_6space = 6;
      }

      for (int ii = 0; ii < lines.Length; ii++)
      {
        string[] items = lines[ii].Split('\t');
        if (items.Length == n_args_3space)
        {
          double[] values = new double[n_args_3space];
          for (int jj = 0; jj < n_args_3space; jj++)
          {
            values[jj] = double.Parse(items[jj]);
          }
          if (reverse)
          {
            Electroimpact.LinearAlgebra.c6dof pt = new Electroimpact.LinearAlgebra.c6dof(values[0], values[1], values[2], 0, 0, 0);
            if (formstate.uses_barrel_transform)
            {
              if (formstate.uses_uv_table_transform)
                myVectors.Add(new Electroimpact.LinearAlgebra.c6dof(myBarrelFunction.FRCtoBarrel(pt.GetMatrix(), values[n_args_3space - 2], values[n_args_3space - 1])));
              else
              {
                if (whichSpinAxis == PrimarySpinAxis.X)
                  myVectors.Add(new Electroimpact.LinearAlgebra.c6dof(myBarrelFunction.FRCtoBarrel(pt.GetMatrix(), values[n_args_3space - 1])));
                else
                  myVectors.Add(new Electroimpact.LinearAlgebra.c6dof(myBarrelFunction.FRCtoBarrel_AboutY(pt.GetMatrix(), values[n_args_3space - 1])));
              }
            }
            else
              myVectors.Add(new Electroimpact.LinearAlgebra.c6dof(myBarrelFunction.FRCtoBarrel(pt.GetMatrix(), 0.0)));
          }
          else
          {
            Electroimpact.LinearAlgebra.c6dof pt = new Electroimpact.LinearAlgebra.c6dof(values[0], values[1], values[2], 0, 0, 0);
            if (formstate.uses_barrel_transform)
            {
              if (formstate.uses_uv_table_transform)
                myVectors.Add(myBarrelFunction.BarrelToFRC(pt.GetMatrix(), values[n_args_3space - 2], values[n_args_3space - 1]));
              else
              {
                if (whichSpinAxis == PrimarySpinAxis.X)
                  myVectors.Add(myBarrelFunction.BarrelToFRC(pt.GetMatrix(), values[n_args_3space - 1]));
                else
                  myVectors.Add(myBarrelFunction.BarrelToFRC_AboutY(pt.GetMatrix(), values[n_args_3space - 1]));
              }
            }
            else
              myVectors.Add(myBarrelFunction.BarrelToFRC(pt.GetMatrix(), 0.0));
          }
        }
        if (items.Length == n_args_6space)
        {
          _3space = false;
          double[] values = new double[n_args_6space];
          for (int jj = 0; jj < n_args_6space; jj++)
          {
            values[jj] = double.Parse(items[jj]);
          }
          Electroimpact.LinearAlgebra.c6dof pt = new Electroimpact.LinearAlgebra.c6dof(values[0], values[1], values[2], values[3].DegreesToRadians(), values[4].DegreesToRadians(), values[5].DegreesToRadians());
          if (reverse)
          {
            if (formstate.uses_barrel_transform)
            {
              if (formstate.uses_uv_table_transform)
                myVectors.Add(new Electroimpact.LinearAlgebra.c6dof(myBarrelFunction.FRCtoBarrel(pt.GetMatrix(), values[n_args_6space - 2], values[n_args_6space - 1])));
              else
              {
                if (whichSpinAxis == PrimarySpinAxis.X)
                  myVectors.Add(new Electroimpact.LinearAlgebra.c6dof(myBarrelFunction.FRCtoBarrel(pt.GetMatrix(), values[n_args_6space - 1])));
                else
                  myVectors.Add(new Electroimpact.LinearAlgebra.c6dof(myBarrelFunction.FRCtoBarrel_AboutY(pt.GetMatrix(), values[n_args_6space - 1])));
              }
            }
            else
              myVectors.Add(new Electroimpact.LinearAlgebra.c6dof(myBarrelFunction.FRCtoBarrel(pt.GetMatrix(), 0.0)));
          }
          else
          {
            if (formstate.uses_barrel_transform)
            {
              if (formstate.uses_uv_table_transform)
                myVectors.Add(myBarrelFunction.BarrelToFRC(pt.GetMatrix(), values[n_args_6space - 2], values[n_args_6space - 1]));
              else
              {
                if (whichSpinAxis == PrimarySpinAxis.X)
                  myVectors.Add(myBarrelFunction.BarrelToFRC(pt.GetMatrix(), values[n_args_6space - 1]));
                else
                  myVectors.Add(myBarrelFunction.BarrelToFRC_AboutY(pt.GetMatrix(), values[n_args_6space - 1]));

              }
            }
            else
              myVectors.Add(myBarrelFunction.BarrelToFRC(pt.GetMatrix(), 0.0));
          }
        }
      }
      string[] linesout = new string[myVectors.Count + 1];
      linesout[0] = "X    ".PadLeft(12) + "Y    ".PadLeft(12) + "Z    ".PadLeft(12);
      for (int ii = 0; ii < myVectors.Count; ii++)
      {
        Electroimpact.LinearAlgebra.c6dof pt = myVectors[ii];
        if (_3space)
          linesout[ii + 1] = pt.X.ToString("F4").PadLeft(12) +
                             pt.Y.ToString("F4").PadLeft(12) +
                             pt.Z.ToString("F4").PadLeft(12);
        else
          linesout[ii + 1] = pt.X.ToString("F4").PadLeft(12) +
                             pt.Y.ToString("F4").PadLeft(12) +
                             pt.Z.ToString("F4").PadLeft(12) +
                             pt.rX.RadiansToDegrees().ToString("F4").PadLeft(12) +
                             pt.rY.RadiansToDegrees().ToString("F4").PadLeft(12) +
                             pt.rZ.RadiansToDegrees().ToString("F4").PadLeft(12);
      }
      txtPointsTransformed.Lines = linesout;
    }

    private void btnGetaCSVCopy_Click(object sender, EventArgs e)
    {
      string outtext = "";
      Electroimpact.csString eis = new Electroimpact.csString();
      for (int ii = 1; ii < txtPointsTransformed.Lines.Length; ii++)
      {
        string line = txtPointsTransformed.Lines[ii];
        csString s = new csString();
        s.String = line;
        line = "";
        bool killing_white = true;
        while (s.GetLeftNoAdv(1) != "")
        {
          string t = s.GetLeft(1);
          if (t == " ")
          {
            if (!killing_white)
            {
              line += ",";
              killing_white = true;
            }
          }
          else
          {
            line += t;
            killing_white = false;
          }

        }
        string[] test2 = line.Split(',');
        int jj;
        for (jj = 0; jj < test2.Length - 1; jj++)
          outtext += test2[jj] + "\t";
        outtext += test2[jj];
        if (ii < txtPointsTransformed.Lines.Length - 1)
          outtext += "\n";
      }
      System.Windows.Forms.Clipboard.Clear();
      System.Windows.Forms.Clipboard.SetText(outtext);
    }


    public bool SetIPandConnect()
    {

      bool WorkingIP = SetIPAddress();
      if (!WorkingIP)
        return false;

      bool bWorkingConnection = ConnectToCNC(formstate.cnc_ip);
      if (!bWorkingConnection)
        return false;

      return true;
    }

    public bool SetIPAddress()
    {

      DocuTrackProSE.InputBoxDialog ib = new DocuTrackProSE.InputBoxDialog();
      ib.FormPrompt = "Input CNC IP";
      ib.FormCaption = "CNC IP Dialog";


      ib.DefaultValue = formstate.cnc_ip;
      DialogResult dr = ib.ShowDialog();
      if (dr == DialogResult.OK)
      {
        if (!formstate.trySetCNC_IP(ib.InputResponse))
        {
          this.Enabled = true;
          return false;
        }
      }
      else
      {
        this.Enabled = true;
        return false;
      }
      return true;
    }


    public void SetOrCompareTransforms(bool SendToCNC)
    {
      if (formstate.bGoodTransformFit || !SendToCNC)
      {
        if (formstate.uses_barrel_transform && !manualTransform)
        {
          //barrel transform
          DataForBarrelTransform(SendToCNC);
        }
        else
        {
          //regular transform
          DataForRegularTransform(SendToCNC);
        }
      }
      else
      {
        MessageBox.Show("Error: Cannot Send Out of Tolerance Transform to the Machine.");

      }
      this.Enabled = true;
    }

    public void DataForBarrelTransform(bool bSendToCNC)
    {
      #region Barrel Transform
      this.Enabled = false;



      int barrelUnit = 0;
      List<Electroimpact.TransformerInfo.cBarrelTransform> BarrelTrans = TransformerInfo.getListOfBarrelT();

      if (formstate.uses_mult_mandrels)
      {

        if (!bSendToCNC)
        {
          MessageBox.Show("Verify is NOT implemented for multiple mandrels.");
          return;
        }

        DocuTrackProSE.ListBoxDialog lb = new DocuTrackProSE.ListBoxDialog();
        lb.FormPrompt = "Select the Mandrel Unit being transformed.";
        lb.FormCaption = "Select Mandrel Unit";

        lb.ShowDialog();

        if (lb.DialogResult == DialogResult.Cancel || lb.DialogResult == DialogResult.Yes)
        {
          this.Enabled = true;
          return;
        }

        barrelUnit = lb.InputResponse;
      }


      if (!SetIPandConnect())
        return;



      Int32[] dong = new Int32[18];
      if (CNC.Connected)
      {
        double mult = formstate.uses_mm_data ? 1.0 : 25.4;
        Int32[] values = new Int32[18];
        Int32[] valuesback = new Int32[18];

        values[0] = (Int32)(Math.Round(myBarrelFunction.PartToUaxis.X * mult, OffsetPower) * OffsetPrecision);
        values[1] = (Int32)(Math.Round(myBarrelFunction.PartToUaxis.Y * mult, OffsetPower) * OffsetPrecision);
        values[2] = (Int32)(Math.Round(myBarrelFunction.PartToUaxis.Z * mult, OffsetPower) * OffsetPrecision);
        values[3] = (Int32)(Math.Round(myBarrelFunction.PartToUaxis.rX.RadiansToDegrees(), AnlgePower) * AnglePrecision);
        values[4] = (Int32)(Math.Round(myBarrelFunction.PartToUaxis.rY.RadiansToDegrees(), AnlgePower) * AnglePrecision);
        values[5] = (Int32)(Math.Round(myBarrelFunction.PartToUaxis.rZ.RadiansToDegrees(), AnlgePower) * AnglePrecision);

        //Saving transform for PartToUaxis
        BarrelTrans[barrelUnit].MandrelToSpin.X = values[0];
        BarrelTrans[barrelUnit].MandrelToSpin.Y = values[1];
        BarrelTrans[barrelUnit].MandrelToSpin.Z = values[2];
        BarrelTrans[barrelUnit].MandrelToSpin.rX = values[3];
        BarrelTrans[barrelUnit].MandrelToSpin.rY = values[4];
        BarrelTrans[barrelUnit].MandrelToSpin.rZ = values[5];

        Electroimpact.LinearAlgebra.cMatrix EulerInverse = new Electroimpact.LinearAlgebra.cMatrix(myBarrelFunction.PartToUaxis.Inverse());
        double[,] data = EulerInverse.GetMatrix;
        values[6] = (Int32)(Math.Round(data[0, 0], MatrixPower) * MatrixPrecision);
        values[7] = (Int32)(Math.Round(data[0, 1], MatrixPower) * MatrixPrecision);
        values[8] = (Int32)(Math.Round(data[0, 2], MatrixPower) * MatrixPrecision);
        values[9] = (Int32)(Math.Round(data[0, 3] * mult, OffsetPower) * OffsetPrecision);
        values[10] = (Int32)(Math.Round(data[1, 0], MatrixPower) * MatrixPrecision);
        values[11] = (Int32)(Math.Round(data[1, 1], MatrixPower) * MatrixPrecision);
        values[12] = (Int32)(Math.Round(data[1, 2], MatrixPower) * MatrixPrecision);
        values[13] = (Int32)(Math.Round(data[1, 3] * mult, OffsetPower) * OffsetPrecision);
        values[14] = (Int32)(Math.Round(data[2, 0], MatrixPower) * MatrixPrecision);
        values[15] = (Int32)(Math.Round(data[2, 1], MatrixPower) * MatrixPrecision);
        values[16] = (Int32)(Math.Round(data[2, 2], MatrixPower) * MatrixPrecision);
        values[17] = (Int32)(Math.Round(data[2, 3] * mult, OffsetPower) * OffsetPrecision);





        string Address = "D7300*4*18"; //mandrelToSpin


        if (bSendToCNC)
        {
          CNC.WritePMCData(Address, values, out err);
          if (err != Electroimpact.FANUC.Err_Code.EW_OK)
          {
            MessageBox.Show("Error communicating with CNC");
            this.Enabled = true;
            return;
          }
        }

        valuesback = CNC.ReadPMCRange(Address, out err);
        if (err != Electroimpact.FANUC.Err_Code.EW_OK)
        {
          MessageBox.Show("Error communicating with CNC");
          this.Enabled = true;
          return;
        }
        if (valuesback.Length == values.Length)
        {
          for (int ii = 0; ii < values.Length; ii++)
          {
            if (!Equal2(valuesback[ii], values[ii], 1))
            {
              MessageBox.Show("Bad Download");
              this.Enabled = true;
              return;
            }
          }
        }
        else
        {
          MessageBox.Show("Bad Download");
          this.Enabled = true;
          return;
        }



        values[0] = (Int32)(Math.Round(myBarrelFunction.VaxisToFRC.X * mult, OffsetPower) * OffsetPrecision);
        values[1] = (Int32)(Math.Round(myBarrelFunction.VaxisToFRC.Y * mult, OffsetPower) * OffsetPrecision);
        values[2] = (Int32)(Math.Round(myBarrelFunction.VaxisToFRC.Z * mult, OffsetPower) * OffsetPrecision);
        values[3] = (Int32)(Math.Round(myBarrelFunction.VaxisToFRC.rX.RadiansToDegrees(), AnlgePower) * AnglePrecision);
        values[4] = (Int32)(Math.Round(myBarrelFunction.VaxisToFRC.rY.RadiansToDegrees(), AnlgePower) * AnglePrecision);
        values[5] = (Int32)(Math.Round(myBarrelFunction.VaxisToFRC.rZ.RadiansToDegrees(), AnlgePower) * AnglePrecision);

        //Saving transform for VaxisToFRC
        BarrelTrans[barrelUnit].SpinToFRC.X = values[0];
        BarrelTrans[barrelUnit].SpinToFRC.Y = values[1];
        BarrelTrans[barrelUnit].SpinToFRC.Z = values[2];
        BarrelTrans[barrelUnit].SpinToFRC.rX = values[3];
        BarrelTrans[barrelUnit].SpinToFRC.rY = values[4];
        BarrelTrans[barrelUnit].SpinToFRC.rZ = values[5];

        EulerInverse = new Electroimpact.LinearAlgebra.cMatrix(myBarrelFunction.VaxisToFRC.Inverse());
        data = EulerInverse.GetMatrix;
        values[6] = (Int32)(Math.Round(data[0, 0], MatrixPower) * MatrixPrecision);
        values[7] = (Int32)(Math.Round(data[0, 1], MatrixPower) * MatrixPrecision);
        values[8] = (Int32)(Math.Round(data[0, 2], MatrixPower) * MatrixPrecision);
        values[9] = (Int32)(Math.Round(data[0, 3] * mult, OffsetPower) * OffsetPrecision);
        values[10] = (Int32)(Math.Round(data[1, 0], MatrixPower) * MatrixPrecision);
        values[11] = (Int32)(Math.Round(data[1, 1], MatrixPower) * MatrixPrecision);
        values[12] = (Int32)(Math.Round(data[1, 2], MatrixPower) * MatrixPrecision);
        values[13] = (Int32)(Math.Round(data[1, 3] * mult, OffsetPower) * OffsetPrecision);
        values[14] = (Int32)(Math.Round(data[2, 0], MatrixPower) * MatrixPrecision);
        values[15] = (Int32)(Math.Round(data[2, 1], MatrixPower) * MatrixPrecision);
        values[16] = (Int32)(Math.Round(data[2, 2], MatrixPower) * MatrixPrecision);
        values[17] = (Int32)(Math.Round(data[2, 3] * mult, OffsetPower) * OffsetPrecision);





        Address = "D7396*4*18";//spinToFRC



        if (bSendToCNC)
        {
          CNC.WritePMCData(Address, values, out err);
          if (err != Electroimpact.FANUC.Err_Code.EW_OK)
          {
            MessageBox.Show("Error communicating with CNC");
            this.Enabled = true;
            return;
          }
        }

        valuesback = CNC.ReadPMCRange(Address, out err);

        if (err != Electroimpact.FANUC.Err_Code.EW_OK)
        {
          MessageBox.Show("Error communicating with CNC");
          this.Enabled = true;
          return;
        }

        if (valuesback.Length == values.Length)
        {
          for (int ii = 0; ii < values.Length; ii++)
          {
            if (!Equal2(valuesback[ii], values[ii], 1))
            {
              MessageBox.Show("Bad Download");
              this.Enabled = true;
              return;
            }
          }
        }
        else
        {
          MessageBox.Show("Bad Download");
          this.Enabled = true;
          return;
        }


        Address = "D7492*4"; //the A_x offset

        if (bSendToCNC)
        {
          CNC.WritePMCData(Address, (Int32)(Math.Round(myBarrelFunction.A_x * mult, OffsetPower) * OffsetPrecision));
          if (err != Electroimpact.FANUC.Err_Code.EW_OK)
          {
            MessageBox.Show("Error communicating with CNC");
            this.Enabled = true;
            return;
          }
        }

        int down = (Int32)(Math.Round(myBarrelFunction.A_x * mult, OffsetPower) * OffsetPrecision);
        int back = CNC.ReadPMCData(Address, out err);
        if (err != Electroimpact.FANUC.Err_Code.EW_OK)
        {
          MessageBox.Show("Error communicating with CNC");
          this.Enabled = true;
          return;
        }

        BarrelTrans[barrelUnit].Ax = down;
        TransformerInfo.SaveListOfBarrelT(BarrelTrans);

        if (!Equal2(down, back, 1))
        {
          MessageBox.Show("Bad Download");
          this.Enabled = true;
          return;
        }

        if (bSendToCNC)
          MessageBox.Show("Transform Accepted");
        else
          MessageBox.Show("Tranforms Match");
      }




      this.Enabled = true;
      #endregion

    }

    public void DataForRegularTransform(bool bSendToCNC)
    {
      #region RegularTransform



      if (!SetIPandConnect())
        return;

      int result;

      List<string> TransName = TransformerInfo.getListOfTransforms();

      DocuTrackProSE.ListBoxDialog2 lb = new DocuTrackProSE.ListBoxDialog2();

      lb.FormPrompt = "Select which transform";
      lb.FormCaption = "Current Transforms";
      lb.Old = CNC.ReadPMCData("K2.2") == 0;
      lb.PassedInTool = passedInTool;
      int i = 0;
      foreach (string Name in TransName)
      {
        lb.T[i] = (i + 1).ToString() + " - " + Name;
        i++;
      }

      lb.ShowDialog();

      if (lb.DialogResult == DialogResult.Cancel || lb.InputResponse == "kejbnasdjkfnsdj")
      {
        CNC.Dispose();
        return;
      }

      int index = lb.InputResponse.IndexOf(" ");
      string transform = lb.InputResponse.Substring(0, index);
      result = Convert.ToInt16(transform);
      string comment = lb.InputResponse.Substring(3 + index);
      double mult = formstate.uses_mm_data ? 1.0 : 25.4;

      TransName[result - 1] = comment;
      TransformerInfo.saveTransformNames(TransName);

      Int32[] ValuesFromTransformer = new Int32[6];

      if (CNC.ReadPMCData("K2.2") == 0) //if K2.2 is ON then new transform format, holds 18 transforms
      {
        //Old Transform format, holds 6 transforms

        if (result > 0 && result < 6)
        {
          Int32[] dong = new Int32[18];
          if (CNC.Connected)
          {
            //double d2r = Math.PI / 180.0;
            Int32[] values = new Int32[18];

            if (manualTransform)
            {
              values[0] = (Int32)(Math.Round(Convert.ToDouble(txtBx_Manual_X.Text) * mult, OffsetPower) * OffsetPrecision);
              values[1] = (Int32)(Math.Round(Convert.ToDouble(txtBx_Manual_Y.Text) * mult, OffsetPower) * OffsetPrecision);
              values[2] = (Int32)(Math.Round(Convert.ToDouble(txtBx_Manual_Z.Text) * mult, OffsetPower) * OffsetPrecision);
              values[3] = (Int32)(Math.Round(Convert.ToDouble(txtBx_Manual_rX.Text), AnlgePower) * AnglePrecision);
              values[4] = (Int32)(Math.Round(Convert.ToDouble(txtBx_Manual_rY.Text), AnlgePower) * AnglePrecision);
              values[5] = (Int32)(Math.Round(Convert.ToDouble(txtBx_Manual_rZ.Text), AnlgePower) * AnglePrecision);
            }
            else
            {
              values[0] = (Int32)(Math.Round(myBarrelFunction.VaxisToFRC.X * mult, OffsetPower) * OffsetPrecision);
              values[1] = (Int32)(Math.Round(myBarrelFunction.VaxisToFRC.Y * mult, OffsetPower) * OffsetPrecision);
              values[2] = (Int32)(Math.Round(myBarrelFunction.VaxisToFRC.Z * mult, OffsetPower) * OffsetPrecision);
              values[3] = (Int32)(Math.Round(myBarrelFunction.VaxisToFRC.rX.RadiansToDegrees(), AnlgePower) * AnglePrecision);
              values[4] = (Int32)(Math.Round(myBarrelFunction.VaxisToFRC.rY.RadiansToDegrees(), AnlgePower) * AnglePrecision);
              values[5] = (Int32)(Math.Round(myBarrelFunction.VaxisToFRC.rZ.RadiansToDegrees(), AnlgePower) * AnglePrecision);

              Electroimpact.LinearAlgebra.cMatrix EulerInverse = new Electroimpact.LinearAlgebra.cMatrix(myBarrelFunction.PartToUaxis.Inverse());
              double[,] data = EulerInverse.GetMatrix;
              values[6] = (Int32)(Math.Round(data[0, 0], MatrixPower) * MatrixPrecision);
              values[7] = (Int32)(Math.Round(data[0, 1], MatrixPower) * MatrixPrecision);
              values[8] = (Int32)(Math.Round(data[0, 2], MatrixPower) * MatrixPrecision);
              values[9] = (Int32)(Math.Round(data[0, 3] * mult, OffsetPower) * OffsetPrecision);
              values[10] = (Int32)(Math.Round(data[1, 0], MatrixPower) * MatrixPrecision);
              values[11] = (Int32)(Math.Round(data[1, 1], MatrixPower) * MatrixPrecision);
              values[12] = (Int32)(Math.Round(data[1, 2], MatrixPower) * MatrixPrecision);
              values[13] = (Int32)(Math.Round(data[1, 3] * mult, OffsetPower) * OffsetPrecision);
              values[14] = (Int32)(Math.Round(data[2, 0], MatrixPower) * MatrixPrecision);
              values[15] = (Int32)(Math.Round(data[2, 1], MatrixPower) * MatrixPrecision);
              values[16] = (Int32)(Math.Round(data[2, 2], MatrixPower) * MatrixPrecision);
              values[17] = (Int32)(Math.Round(data[2, 3] * mult, OffsetPower) * OffsetPrecision);
            }

            //copy values to a variable used for comparrison
            for (int j = 0; j < 6; j++)
            {
              ValuesFromTransformer[j] = values[j];
            }

            //determine if sending to CNC or comparing to CNC
            if (bSendToCNC)
            {
              for (int ii = 0; ii < values.Length; ii++)
              {
                if (ii == 12)
                  Console.WriteLine();
                Console.WriteLine(values[ii]);
              }
              int offset = (result - 1) * 96;
              string Address = "D" + (3300 + offset).ToString() + "*4*18";

              CNC.WritePMCData(Address, values, out err);

              if (err != Electroimpact.FANUC.Err_Code.EW_OK)
              {
                MessageBox.Show("Error communicating with CNC");
                this.Enabled = true;
                return;
              }


            }//end if(SendToCNC)


          }
        }
      }
      else
      {

        //new transform format, holds 18 transforms 


        if (result > 0 && result <= 18)
        {
          Int32[] dong = new Int32[18];
          if (CNC.Connected)
          {
            //double d2r = Math.PI / 180.0;
            Int32[] values = new Int32[6];

            if (manualTransform)
            {
              values[0] = (Int32)(Math.Round(Convert.ToDouble(txtBx_Manual_X.Text) * mult, OffsetPower) * OffsetPrecision);
              values[1] = (Int32)(Math.Round(Convert.ToDouble(txtBx_Manual_Y.Text) * mult, OffsetPower) * OffsetPrecision);
              values[2] = (Int32)(Math.Round(Convert.ToDouble(txtBx_Manual_Z.Text) * mult, OffsetPower) * OffsetPrecision);
              values[3] = (Int32)(Math.Round(Convert.ToDouble(txtBx_Manual_rX.Text), AnlgePower) * AnglePrecision);
              values[4] = (Int32)(Math.Round(Convert.ToDouble(txtBx_Manual_rY.Text), AnlgePower) * AnglePrecision);
              values[5] = (Int32)(Math.Round(Convert.ToDouble(txtBx_Manual_rZ.Text), AnlgePower) * AnglePrecision);
            }
            else
            {
              values[0] = (Int32)(Math.Round(myBarrelFunction.VaxisToFRC.X * mult, OffsetPower) * OffsetPrecision);
              values[1] = (Int32)(Math.Round(myBarrelFunction.VaxisToFRC.Y * mult, OffsetPower) * OffsetPrecision);
              values[2] = (Int32)(Math.Round(myBarrelFunction.VaxisToFRC.Z * mult, OffsetPower) * OffsetPrecision);
              values[3] = (Int32)(Math.Round(myBarrelFunction.VaxisToFRC.rX.RadiansToDegrees(), AnlgePower) * AnglePrecision);
              values[4] = (Int32)(Math.Round(myBarrelFunction.VaxisToFRC.rY.RadiansToDegrees(), AnlgePower) * AnglePrecision);
              values[5] = (Int32)(Math.Round(myBarrelFunction.VaxisToFRC.rZ.RadiansToDegrees(), AnlgePower) * AnglePrecision);
            }

            //copy values to a variable used for comparrison
            for (int j = 0; j < 6; j++)
            {
              ValuesFromTransformer[j] = values[j];
            }


            //determine if sending to CNC or comparing to CNC
            if (bSendToCNC)
            {
              for (int ii = 0; ii < values.Length; ii++)
              {
                Console.WriteLine(values[ii]);
              }
              int offset = (result - 1) * 24;
              string Address = "D" + (3300 + offset).ToString() + "*4*6";

              CNC.WritePMCData(Address, values, out err);

              if (err != Electroimpact.FANUC.Err_Code.EW_OK)
              {
                MessageBox.Show("Error communicating with CNC");
                this.Enabled = true;
                return;
              }
            }
          }
        }
      }

      if (!bSendToCNC)
      {
        //then compare to existing


        List<double[]> TransformsOnCNC = GetTransformsFromCNC(false);
        if (TransformsOnCNC == null)
        {
          //error reported inside of GetTransformsFromCNC
          return;
        }

        if (result > 0 && result <= 18)
        {
          double[] TheOneToCompareTo = TransformsOnCNC[result - 1];

          for (int j = 0; j < 6; j++) //only 6 values get checked
          {
            if (!Equal2(ValuesFromTransformer[j], TheOneToCompareTo[j], 10))
            {
              MessageBox.Show("Bad Download");
              this.Enabled = true;
              return;
            }
          }
          MessageBox.Show("Tranforms Match.");
          this.Enabled = true;
        }
      }
      else
      {
        //then setting the transform:
        manualTransform = false;
        MessageBox.Show("Transform Accepted");
      }

      CNC.Dispose();


      #endregion
    }


    public void btnCompToCNC_Click(object sender, EventArgs e)
    {
      bool bSendToCNC = true;
      SetOrCompareTransforms(bSendToCNC);


    }

    private bool LoadFromTransforms()
    {

      if (!Electroimpact.FileIO.cFileOther.FileExistsMethod("Transforms.csv"))
      {
        MessageBox.Show("Transforms.csv does not exist!");
        return false;
      }
      string lastline = "";
      using (StreamReader sr = new StreamReader("Transforms.csv"))
      {
        while (!sr.EndOfStream)
        {
          lastline = sr.ReadLine();
        }
        string[] transform = lastline.Split(',');
        double[] values = new double[transform.Length];
        if (transform.Length < 13)
        {
          MessageBox.Show("Bad last transform");
          return false;
        }
        for (int ii = 0; ii < 13; ii++)
        {
          double test;
          if (double.TryParse(transform[ii], out test)) values[ii] = test;
          else
          {
            MessageBox.Show("Last Transform is Invalid");
            return false;
          }
        }
        double d2r = Math.PI / 180.0;
        myBarrelFunction.PartToUaxis.X = values[0];
        myBarrelFunction.PartToUaxis.Y = values[1];
        myBarrelFunction.PartToUaxis.Z = values[2];
        myBarrelFunction.PartToUaxis.rX = values[3] * d2r;
        myBarrelFunction.PartToUaxis.rY = values[4] * d2r;
        myBarrelFunction.PartToUaxis.rZ = values[5] * d2r;
        myBarrelFunction.A_x = values[6];
        myBarrelFunction.VaxisToFRC.X = values[7];
        myBarrelFunction.VaxisToFRC.Y = values[8];
        myBarrelFunction.VaxisToFRC.Z = values[9];
        myBarrelFunction.VaxisToFRC.rX = values[10] * d2r;
        myBarrelFunction.VaxisToFRC.rY = values[11] * d2r;
        myBarrelFunction.VaxisToFRC.rZ = values[12] * d2r;

        //fw.WriteLine("Transforms.csv",
        //                              y_measBarrelFunction.PartToUaxis.X.ToString("F3") + "," +
        //                              y_measBarrelFunction.PartToUaxis.Y.ToString("F3") + "," +
        //                              y_measBarrelFunction.PartToUaxis.Z.ToString("F3") + "," +
        //                              (y_measBarrelFunction.PartToUaxis.rX.RadiansToDegrees()).ToString("F6") + "," +
        //                              (y_measBarrelFunction.PartToUaxis.rY.RadiansToDegrees()).ToString("F6") + "," +
        //                              (y_measBarrelFunction.PartToUaxis.rZ.RadiansToDegrees()).ToString("F6") + "," +
        //                              y_measBarrelFunction.A_x.ToString("F6") + "," +
        //                              y_measBarrelFunction.VaxisToFRC.X.ToString("F3") + "," +
        //                              y_measBarrelFunction.VaxisToFRC.Y.ToString("F3") + "," +
        //                              y_measBarrelFunction.VaxisToFRC.Z.ToString("F3") + "," +
        //                              (y_measBarrelFunction.VaxisToFRC.rX.RadiansToDegrees()).ToString("F6") + "," +
        //                              (y_measBarrelFunction.VaxisToFRC.rY.RadiansToDegrees()).ToString("F6") + "," +
        //                              (y_measBarrelFunction.VaxisToFRC.rZ.RadiansToDegrees()).ToString("F6") + ","
        //                              , true);
      }
      return true;
    }

    private void btnVerifyCompOnCNC_Click(object sender, EventArgs e)
    {

      bool bSendToCNC = false;
      SetOrCompareTransforms(bSendToCNC);
    }

    private bool Equal2(double left, double right, double epsilon)
    {
      return Math.Abs(left - right) <= epsilon;
    }

    private void btnPasteToReverse_Click(object sender, EventArgs e)
    {
      Transform(true);
    }

    private void chkShowAdvanced_CheckedChanged(object sender, EventArgs e)
    {
      if (chkShowAdvanced.Checked)
      {
        //this.Size.Width = 1233;//"1233, 893";
        //this.Size.Height = 893;
        this.Size = new Size(1447, 869);
      }
      else
      {
        //453, 895
        //this.Size.Width = 453;//"1233, 893";
        //this.Size.Height = 895;
        this.Size = new Size(425, 400);
      }

    }

    private void checked_points_list_MouseUp(object sender, MouseEventArgs e)
    {
      for (int ii = 0; ii < myPoints.Count; ii++)
        myPoints[ii].bUseInXform = false;
      for (int ii = 0; ii < checked_points_list.CheckedIndices.Count; ii++)
      {
        int jj = checked_points_list.CheckedIndices[ii];
        myPoints[checked_points_list.CheckedIndices[ii]].bUseInXform = true;
      }
    }

    private void frmBarrelTransformer_LocationChanged(object sender, EventArgs e)
    {
      if (this.Visible)
      {
        formstate.fsp = this.Location;
        cFormState.Save(formstate);
      }
    }

    private void menustrip_v_argument_option_Click(object sender, EventArgs e)
    {
      menustrip_v_argument_option.Checked = !menustrip_v_argument_option.Checked;
      formstate.uses_uv_table_transform = menustrip_v_argument_option.Checked;
      update_optionlist();
    }

    public void useXuFunctionAxToolStripMenuItem_Click(object sender, EventArgs e)
    {

      menustrip_useXuFunction_Ax.Checked = !menustrip_useXuFunction_Ax.Checked;
      formstate.uses_uofx_function = menustrip_useXuFunction_Ax.Checked;
    }

    private void menustrip_Barrel_Transform_Click(object sender, EventArgs e)
    {
      menustrip_Barrel_Transform.Checked = !menustrip_Barrel_Transform.Checked;
      formstate.uses_barrel_transform = menustrip_Barrel_Transform.Checked;
      grpBx_ManualTrans.Enabled = !menustrip_Barrel_Transform.Checked;
      update_optionlist();
    }

    private void menustrip_mm_data_Click(object sender, EventArgs e)
    {
      menustrip_mm_data.Checked = !menustrip_mm_data.Checked;
      formstate.uses_mm_data = menustrip_mm_data.Checked;

      lblTolUnitsAve.Text = lblTolUnitsMax.Text = lblEulerBoxUnits.Text = lblManualUnits.Text = formstate.uses_mm_data ? "mm" : "inches";

      if (formstate.uses_mm_data)
      {

        lblAveErrMax.Text = formstate.AveToleranceMM.ToString("F3");
        lblMaxErrMax.Text = formstate.MaxToleranceMM.ToString("F3");

      }
      else
      {
        lblAveErrMax.Text = (formstate.AveToleranceMM / 25.4).ToString("F3");
        lblMaxErrMax.Text = (formstate.MaxToleranceMM / 25.4).ToString("F3");
      }

      update_optionlist();
    }

    private void update_optionlist()
    {
      menustrip_v_argument_option.Enabled = menustrip_Barrel_Transform.Checked;
      menustrip_useXuFunction_Ax.Enabled = menustrip_Barrel_Transform.Checked;
      menustrip_multiple_mandrel_option.Enabled = menustrip_Barrel_Transform.Checked;
      formstate.SaveMe();
      setuptooltips();
    }

    private void setuptooltips()
    {
      string sz_units = formstate.uses_mm_data ? "mm " : "INCHES ";

      btnPasteInPointsToTransform.Text = formstate.uses_mm_data ? "Paste in points (mm data)" : "Paste in points (inch data)";
      btnSelectFile.Text = formstate.uses_mm_data ? "Select File (mm data)" : "Select File (inch data)";


      if (formstate.uses_barrel_transform)
      {
        if (formstate.uses_uv_table_transform)
        {
          toolTip1.SetToolTip(btnPasteInPointsToTransform, sz_units + "In order \"Xnom Ynom Znom Upos Vpos Xmeas  Ymeas Zmeas\".  Any number of sets is possible, copy them out of Excel and tap this button to paste.  Input must be " + sz_units);
          toolTip1.SetToolTip(btnPastePoints, "Any number of sets is possible, copy them out of Excel and tap this button to paste. In order \n\"X,Y,Z,Upos,Vpos\" or \n\"X,Y,Z,A,B,C,Upos,Vpos\"");
          toolTip1.SetToolTip(btnPasteToReverse, "Any number of sets is possible, copy them out of Excel and tap this button to paste. In order \n\"X,Y,Z,Upos,Vpos\" or \n\"X,Y,Z,A,B,C,Upos,Vpos\"");
        }
        else
        {
          toolTip1.SetToolTip(btnPasteInPointsToTransform, sz_units + "In order \"Xnom Ynom Znom Upos Xmeas  Ymeas Zmeas\".  Any number of sets is possible, copy them out of Excel and tap this button to paste.  Input must be " + sz_units);
          toolTip1.SetToolTip(btnPastePoints, "Any number of sets is possible, copy them out of Excel and tap this button to paste. In order \n\"X,Y,Z,Upos\" or \n\"X,Y,Z,A,B,C,Upos\"");
          toolTip1.SetToolTip(btnPasteToReverse, "Any number of sets is possible, copy them out of Excel and tap this button to paste. In order \n\"X,Y,Z,Upos\" or \n\"X,Y,Z,A,B,C,Upos\"");
        }
      }
      else
      {
        toolTip1.SetToolTip(btnPasteInPointsToTransform, sz_units + "In order \"Xnom Ynom Znom Xmeas Ymeas Zmeas\".  Any number of sets is possible, copy them out of Excel and tap this button to paste.  Input must be " + sz_units);
        toolTip1.SetToolTip(btnPastePoints, "Any number of sets is possible, copy them out of Excel and tap this button to paste. In order \n\"X,Y,Z\" or \n\"X,Y,Z,A,B,C\"");
        toolTip1.SetToolTip(btnPasteToReverse, "Any number of sets is possible, copy them out of Excel and tap this button to paste. In order \n\"X,Y,Z\" or \n\"X,Y,Z,A,B,C\"");
      }
    }

    #endregion

    private void menustrip_multiple_mandrel_option_Click(object sender, EventArgs e)
    {
      menustrip_multiple_mandrel_option.Checked = !menustrip_multiple_mandrel_option.Checked;
      formstate.uses_mult_mandrels = menustrip_multiple_mandrel_option.Checked;
      update_optionlist();
    }

    private void btn_Send_Manual_Trans_Click(object sender, EventArgs e)
    {
      manualTransform = true;
      btnCompToCNC.PerformClick();
    }

    private void btnSetManualTform_Click(object sender, EventArgs e)
    {
      formstate.bGoodTransformFit = true; //set this so can send to CNC and not care about last calculated fix
      string sSetXform = txtBx_Manual_X.Text + Environment.NewLine + txtBx_Manual_Y.Text + Environment.NewLine + txtBx_Manual_Z.Text + Environment.NewLine + txtBx_Manual_rX.Text + Environment.NewLine + txtBx_Manual_rY.Text + Environment.NewLine + txtBx_Manual_rZ.Text;
      this.InputTransform(sSetXform);
      txtbox_transform_args_update();

      //private void btnSetNow_Click(object sender, EventArgs e)
      //{
      //  string sSetXform = tbX.Text + Environment.NewLine + tbY.Text + Environment.NewLine + tbZ.Text + Environment.NewLine + tb_rX.Text + Environment.NewLine + tb_rY.Text + Environment.NewLine + tb_rZ.Text;
      //  my_frmTransformer.InputTransform(sSetXform);
      //  my_frmTransformer.DisplayXformValues();
      //  this.Close();
      //}
    }

    /// <summary>
    /// six arguments separated by line breaks.
    /// </summary>
    /// <param name="doggy"></param>
    public void InputTransform(string doggy)
    {
      System.IO.StringReader sr = new System.IO.StringReader(doggy);

      double x, y, z, rx, ry, rz = 0;

      x = double.Parse(sr.ReadLine());
      y = double.Parse(sr.ReadLine());
      z = double.Parse(sr.ReadLine());
      rx = double.Parse(sr.ReadLine());
      ry = double.Parse(sr.ReadLine());
      rz = double.Parse(sr.ReadLine());
      double d2r = Math.PI / 180.0;

      myBarrelFunction.VaxisToFRC.X = x;
      myBarrelFunction.VaxisToFRC.Y = y;
      myBarrelFunction.VaxisToFRC.Z = z;
      myBarrelFunction.VaxisToFRC.rX = rx * d2r;
      myBarrelFunction.VaxisToFRC.rY = ry * d2r;
      myBarrelFunction.VaxisToFRC.rZ = rz * d2r;


      myBarrelFunction.PartToUaxis.X = 0.0;
      myBarrelFunction.PartToUaxis.Y = 0.0;
      myBarrelFunction.PartToUaxis.Z = 0.0;
      myBarrelFunction.PartToUaxis.rX = 0.0;
      myBarrelFunction.PartToUaxis.rY = 0.0;
      myBarrelFunction.PartToUaxis.rZ = 0.0;


    }

    private void frmBarrelTransformer_FormClosed(object sender, FormClosedEventArgs e)
    {
      //~_CNC();
      return;

      //frmBarrelTransformer.
    }

    private List<double[]> GetBarrelTransformFromCNC(bool bConnectToCNC)
    {
      Electroimpact.FANUC.Err_Code err;
      List<double[]> transformList = new List<double[]>();
      //List<Electroimpact.TransformerInfo.cBarrelTransform> BarrelTrans = TransformerInfo.getListOfBarrelT();

      string Address = "";
      double[] currTransformConvert;


      if (bConnectToCNC) //if no connection passed in then connect here
      {

        if (!SetIPandConnect())
          return null;
      }




      #region Read Barrel


      //Int32[] dong = new Int32[6];
      //int barrelUnit = 0;
      if (CNC.Connected)
      {
        Int32[] Mand2Spin = new Int32[6];
        Int32[] Spin2FRC = new Int32[6];

        //mandrel to spin
        Address = "D7300*4*6";
        Mand2Spin = CNC.ReadPMCRange(Address, out err);

        if (err != Electroimpact.FANUC.Err_Code.EW_OK)
        {
          MessageBox.Show("Error communicating with CNC");
          return null;
        }

        currTransformConvert = new double[6];
        currTransformConvert[0] = (double)Mand2Spin[0];
        currTransformConvert[1] = (double)Mand2Spin[1];
        currTransformConvert[2] = (double)Mand2Spin[2];
        currTransformConvert[3] = (double)Mand2Spin[3];
        currTransformConvert[4] = (double)Mand2Spin[4];
        currTransformConvert[5] = (double)Mand2Spin[5];
        transformList.Add(currTransformConvert);

        //spin 2 frc
        Address = "D7396*4*18";
        Spin2FRC = CNC.ReadPMCRange(Address, out err);

        if (err != Electroimpact.FANUC.Err_Code.EW_OK)
        {
          MessageBox.Show("Error communicating with CNC");
          return null;
        }
        currTransformConvert = new double[6];
        currTransformConvert[0] = (double)Spin2FRC[0];
        currTransformConvert[1] = (double)Spin2FRC[1];
        currTransformConvert[2] = (double)Spin2FRC[2];
        currTransformConvert[3] = (double)Spin2FRC[3];
        currTransformConvert[4] = (double)Spin2FRC[4];
        currTransformConvert[5] = (double)Spin2FRC[5];
        transformList.Add(currTransformConvert);

        Address = "D7492*4";
        int A_x = CNC.ReadPMCData(Address, out err);
        if (err != Electroimpact.FANUC.Err_Code.EW_OK)
        {
          MessageBox.Show("Error communicating with CNC");
          return null;
        }

        currTransformConvert = new double[1];
        currTransformConvert[0] = (double)A_x;
        transformList.Add(currTransformConvert);

      }

      #endregion


      return transformList;
    }

    private List<double[]> GetTransformsFromCNC(bool bConnectToCNC)
    {
      List<double[]> transformList = new List<double[]>();

      Electroimpact.FANUC.Err_Code err;



      string Address = "";
      double[] currTransformConvert;
      //double mult = 25.4;


      if (bConnectToCNC) //if no connection passed in then connect here
      {

        if (!SetIPandConnect())
          return null;
      }



      #region Read Transforms

      Int32[] currTransform = new Int32[18];


      int maxI = 6; //for 5 transforms
      if (CNC.ReadPMCData("K2.2") == 0)  //if K2.2 is ON then new transform format, holds 18 transforms
      {
        for (int i = 0; i < maxI; i++)
        {
          currTransformConvert = new double[6];
          Address = "D" + (3300 + 96 * i).ToString() + "*4*18";
          currTransform = CNC.ReadPMCRange(Address, out err);
          currTransformConvert[0] = currTransform[0];// / 25.4 / OffsetPrecision;
          currTransformConvert[1] = currTransform[1];// / 25.4 / OffsetPrecision;
          currTransformConvert[2] = currTransform[2];// / 25.4 / OffsetPrecision;
          currTransformConvert[3] = currTransform[3];// / AnglePrecision;
          currTransformConvert[4] = currTransform[4];// / AnglePrecision;
          currTransformConvert[5] = currTransform[5];// / AnglePrecision;
          transformList.Add(currTransformConvert);
        }
      }
      else
      {
        maxI = 18;

        for (int i = 0; i < maxI; i++)
        {
          currTransformConvert = new double[6];
          Address = "D" + (3300 + 24 * i).ToString() + "*4*6";
          currTransform = CNC.ReadPMCRange(Address, out err);
          currTransformConvert[0] = currTransform[0];// / 25.4 / OffsetPrecision;
          currTransformConvert[1] = currTransform[1];// / 25.4 / OffsetPrecision;
          currTransformConvert[2] = currTransform[2];// / 25.4 / OffsetPrecision;
          currTransformConvert[3] = currTransform[3];// / AnglePrecision;
          currTransformConvert[4] = currTransform[4];// / AnglePrecision;
          currTransformConvert[5] = currTransform[5];// / AnglePrecision;
          transformList.Add(currTransformConvert);
        }
      }
      #endregion

      return transformList;
    }

    private void readTransformsOnCNCToolStripMenuItem_Click(object sender, EventArgs e)
    {

      List<double[]> BtransformList = GetBarrelTransformFromCNC(true);
      if (BtransformList == null)
      {
        //error reported inside of GetTransformsFromCNC
        return;
      }

      List<double[]> transformList = GetTransformsFromCNC(false); //raw data from CNC, comes out in mm units multiplied by OffsetPrecision or AnglePrecision
      if (transformList == null)
      {
        //error reported inside of GetTransformsFromCNC
        return;
      }
      List<string> TransName = TransformerInfo.getListOfTransforms();


      #region Output transforms to CSV
      string directory = "C:\\CellControl\\Transforms\\";
      string filename = "C:\\CellControl\\Transforms\\Transforms_" + DateTime.Now.ToString("yyyy_dd_M-HH-mm") + ".csv";

      if (!Directory.Exists(directory))
        Directory.CreateDirectory(directory);


      double unitMult = formstate.uses_mm_data ? 1.0 : 25.4;
      string units = formstate.uses_mm_data ? "mm" : "inches";

      try
      {
        using (StreamWriter sw = new StreamWriter(filename, false))
        {



          sw.WriteLine("MandrelToSpin");
          sw.WriteLine("X (" + units + ")," + BtransformList[0][0] / unitMult / OffsetPrecision);
          sw.WriteLine("Y (" + units + ")," + BtransformList[0][1] / unitMult / OffsetPrecision);
          sw.WriteLine("Z (" + units + ")," + BtransformList[0][2] / unitMult / OffsetPrecision);
          sw.WriteLine("rX (deg)," + BtransformList[0][3] / AnglePrecision);
          sw.WriteLine("rY (deg)," + BtransformList[0][4] / AnglePrecision);
          sw.WriteLine("rZ (deg)," + BtransformList[0][5] / AnglePrecision);
          //sw.WriteLine();

          sw.WriteLine("SpinToFRC");
          sw.WriteLine("X (" + units + ")," + BtransformList[1][0] / unitMult / OffsetPrecision);
          sw.WriteLine("Y (" + units + ")," + BtransformList[1][1] / unitMult / OffsetPrecision);
          sw.WriteLine("Z (" + units + ")," + BtransformList[1][2] / unitMult / OffsetPrecision);
          sw.WriteLine("rX (deg)," + BtransformList[1][3] / AnglePrecision);
          sw.WriteLine("rY (deg)," + BtransformList[1][4] / AnglePrecision);
          sw.WriteLine("rZ (deg)," + BtransformList[1][5] / AnglePrecision);
          //sw.WriteLine();
          sw.WriteLine("A_x");
          sw.WriteLine("(" + units + ")," + BtransformList[2][0] / unitMult / OffsetPrecision);
          sw.WriteLine();

          //int count = 0;
          //foreach (string n in TransName)
          for (int i = 0; i < transformList.Count; i++)
          {
            string n = TransName[i];

            sw.WriteLine((i + 1).ToString("F0") + " " + n);

            sw.WriteLine("X (" + units + ")," + transformList[i][0] / unitMult / OffsetPrecision);
            sw.WriteLine("Y (" + units + ")," + transformList[i][1] / unitMult / OffsetPrecision);
            sw.WriteLine("Z (" + units + ")," + transformList[i][2] / unitMult / OffsetPrecision);
            sw.WriteLine("rX (deg)," + transformList[i][3] / AnglePrecision);
            sw.WriteLine("rY (deg)," + transformList[i][4] / AnglePrecision);
            sw.WriteLine("rZ (deg)," + transformList[i][5] / AnglePrecision);
            sw.WriteLine();
            //count++;
          }
        }
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.Message);
      }

      #endregion

      if (System.Windows.Forms.MessageBox.Show("Open transform folder?", "Open Folder", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
      {
        System.Diagnostics.Process.Start("explorer.exe", "/select," + filename);
      }

    }//end function




    private void btnSelectFile_Click(object sender, EventArgs e)
    {
      if (ReadInPointsFromFile())
        btnTransform_Click(sender, e);
    }

    private bool ReadInPointsFromFile()
    {

      OpenFileDialog ofd = new OpenFileDialog();
      OpenFileDialog openFileDialog2 = new OpenFileDialog();
      //openFileDialog2.InitialDirectory = Settings.mainSettings.FileLocations.LoadFromFolder;// machine.configCell.getAttributeByNodeName("/configuration/LoadFromFolder", "value");


      openFileDialog2.Filter = "csv files|*.csv";
      DialogResult dr = openFileDialog2.ShowDialog();
      string filename = openFileDialog2.FileName;

      //label9.Text = filename;////Directory.//Directory(filename);


      List<string> InputLines = new List<string>();

      //read the lines of the data file
      Electroimpact.FileIO.cFileReader fr = new Electroimpact.FileIO.cFileReader();
      if (filename == "")
        return false;

      fr.OpenFile(filename);
      while (fr.Peek())
      {
        string lineIn = fr.ReadLine();
        InputLines.Add(lineIn);
      }
      fr.CloseFile();



      int nArgs = formstate.uses_uv_table_transform ? 8 : 7;

      if (!formstate.uses_barrel_transform) // we are doing a regular rigid body transform
        nArgs = 6;

      //bool success = false;
      string[] lines2 = new string[InputLines.Count - 1];

      //skip the 1st line because it is a header
      for (int ii = 1; ii < InputLines.Count; ii++)
      {
        string[] items = InputLines[ii].Split(','); //Split('\t');
        int nItems = items.Length;
        if (items[items.Length - 1] == "")
          nItems -= 1;
        if (nItems == nArgs)
        {
          lines2[ii - 1] = InputLines[ii];
        }
        else
        {
          string text = " wrong number of arguments. \n expected \"Xnom,Ynom,Znom,Upos,Xmeas,Ymeas,Zmeas\"\n";
          if (nArgs == 8)
            text = " wrong number of arguments. \n expected \"Xnom,Ynom,Znom,Upos,Vpos,Xmeas,Ymeas,Zmeas\"\n";
          if (nArgs == 6)
            text = " wrong number of arguments. \n expected \"Xnom,Ynom,Znom,Xmeas,Ymeas,Zmeas\"\n";

          MessageBox.Show(InputLines[ii] + text);
          return false;
        }

      }

      txtEulerCheck.Clear();
      this.Refresh();
      System.Threading.Thread.Sleep(50);


      myPoints.Clear();
      foreach (string line in lines2)
      {
        if (line != null)
        {
          cPoint temp;
          if (nArgs == 7)
            temp = new cPoint(line, false);
          else if (nArgs == 8)
            temp = new cPoint(line, true);
          else
            temp = new cPoint(line);
          if (temp != null)
            myPoints.Add(temp);
        }
      }


      return true;
    }

    private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
    {
      MessageBox.Show("Electroimpact Barrel Transformer Version: " + System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString());
    }

  }



  public enum PrimarySpinAxis
  {
    X,
    Y,
    Z
  }

  public class SingleSiemensXform
  {
    int my_size;
    public double[] Linear;
    public double[] Rotational;
    public double[] RotationAxisVector;
    public double OffsetAboutSpinAxis = 0;


    public SingleSiemensXform(int size)
    {
      my_size = size;

      Linear = new double[size];
      Rotational = new double[3];
      RotationAxisVector = new double[3];
    }

    public int[] toIntArray()
    {
      int[] rv = new int[6];
      for (int ii = 0; ii < rv.Length / 2; ii++)
      {
        rv[ii] = Linear.Length > ii ? (int)(Linear[ii] * 1000) : int.MinValue;
        int jj = ii + 3;
        rv[jj] = Rotational.Length > ii ? (int)(Rotational[ii] * 1000) : int.MinValue;
      }
      return rv;
    }
  }


  public class rotationPoint
  {
    public double x_nom = 0;
    public double y_nom = 0;
    public double z_nom = 0;
    public List<double> x_meas;
    public List<double> y_meas;
    public List<double> z_meas;
    public List<double> u_meas;
    public List<double> v_meas;

    public rotationPoint()
    {
      x_meas = new List<double>();
      y_meas = new List<double>();
      z_meas = new List<double>();
      u_meas = new List<double>();
      v_meas = new List<double>();
    }

  }

}

#region input box control
namespace DocuTrackProSE
{
  /// <summary>
  /// Summary description for InputBox.
  /// 
  public class InputBoxDialog : System.Windows.Forms.Form
  {

    #region Windows Contols and Constructor

    private System.Windows.Forms.Label lblPrompt;
    private System.Windows.Forms.Button btnOK;
    private System.Windows.Forms.Button button1;
    private System.Windows.Forms.TextBox txtInput;
    /// <summary>
    /// Required designer variable.
    /// 
    private System.ComponentModel.Container components = null;

    public InputBoxDialog()
    {
      //
      // Required for Windows Form Designer support
      //
      InitializeComponent();

      //
      // TODO: Add ay_nom constructor code after InitializeComponent call
      //
    }

    #endregion

    #region Dispose

    /// <summary>
    /// Clean up ay_nom resources being used.
    /// 
    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        if (components != null)
        {
          components.Dispose();
        }
      }
      base.Dispose(disposing);
    }

    #endregion

    #region Windows Form Designer generated code
    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// 
    private void InitializeComponent()
    {
      this.lblPrompt = new System.Windows.Forms.Label();
      this.btnOK = new System.Windows.Forms.Button();
      this.button1 = new System.Windows.Forms.Button();
      this.txtInput = new System.Windows.Forms.TextBox();
      this.SuspendLayout();
      // 
      // lblPrompt
      // 
      this.lblPrompt.Anchor =
        ((System.Windows.Forms.AnchorStyles)
        ((((System.Windows.Forms.AnchorStyles.Top |
        System.Windows.Forms.AnchorStyles.Bottom)
        | System.Windows.Forms.AnchorStyles.Left)
        | System.Windows.Forms.AnchorStyles.Right)));
      this.lblPrompt.BackColor = System.Drawing.SystemColors.Control;
      this.lblPrompt.Font =
        new System.Drawing.Font("Microsoft Sans Serif", 9.75F,
        System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point,
        ((System.Byte)(0)));
      this.lblPrompt.Location = new System.Drawing.Point(12, 9);
      this.lblPrompt.Name = "lblPrompt";
      this.lblPrompt.Size = new System.Drawing.Size(302, 82);
      this.lblPrompt.TabIndex = 3;
      // 
      // btnOK
      // 
      this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
      this.btnOK.Location = new System.Drawing.Point(326, 24);
      this.btnOK.Name = "btnOK";
      this.btnOK.Size = new System.Drawing.Size(64, 24);
      this.btnOK.TabIndex = 1;
      this.btnOK.Text = "&OK";
      this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
      // 
      // button1
      // 
      this.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
      this.button1.Location = new System.Drawing.Point(326, 56);
      this.button1.Name = "button1";
      this.button1.Size = new System.Drawing.Size(64, 24);
      this.button1.TabIndex = 2;
      this.button1.Text = "&Cancel";
      this.button1.Click += new System.EventHandler(this.button1_Click);
      // 
      // txtInput
      // 
      this.txtInput.Location = new System.Drawing.Point(8, 100);
      this.txtInput.Name = "txtInput";
      this.txtInput.Size = new System.Drawing.Size(379, 20);
      this.txtInput.TabIndex = 0;
      this.txtInput.Text = "";
      // 
      // InputBoxDialog
      // 
      this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
      this.ClientSize = new System.Drawing.Size(398, 128);
      this.Controls.Add(this.txtInput);
      this.Controls.Add(this.button1);
      this.Controls.Add(this.btnOK);
      this.Controls.Add(this.lblPrompt);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "InputBoxDialog";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "InputBox";
      this.Load += new System.EventHandler(this.InputBox_Load);
      this.ResumeLayout(false);

    }
    #endregion

    #region Private Variables
    string formCaption = string.Empty;
    string formPrompt = string.Empty;
    string inputResponse = string.Empty;
    string defaultValue = string.Empty;
    #endregion

    #region Public Properties
    public string FormCaption
    {
      get { return formCaption; }
      set { formCaption = value; }
    } // property FormCaption
    public string FormPrompt
    {
      get { return formPrompt; }
      set { formPrompt = value; }
    } // property FormPrompt
    public string InputResponse
    {
      get { return inputResponse; }
      set { inputResponse = value; }
    } // property InputResponse
    public string DefaultValue
    {
      get { return defaultValue; }
      set { defaultValue = value; }
    } // property DefaultValue

    #endregion

    #region Form and Control Events
    private void InputBox_Load(object sender, System.EventArgs e)
    {
      this.txtInput.Text = defaultValue;
      this.lblPrompt.Text = formPrompt;
      this.Text = formCaption;
      this.txtInput.SelectionStart = 0;
      this.txtInput.SelectionLength = this.txtInput.Text.Length;
      this.txtInput.Focus();
    }


    private void btnOK_Click(object sender, System.EventArgs e)
    {
      InputResponse = this.txtInput.Text;
      this.Close();
    }

    private void button1_Click(object sender, System.EventArgs e)
    {
      this.Close();
    }
    #endregion

  }

  public class ListBoxDialog : System.Windows.Forms.Form
  {
    #region Windows Contols and Constructor

    private System.Windows.Forms.Label lblPrompt;
    private System.Windows.Forms.Button btn2OK;
    private System.Windows.Forms.Button btn2Cnl;
    private System.Windows.Forms.Button btn2Add;
    private System.Windows.Forms.Button btn2Delete;
    private System.Windows.Forms.ListBox listInput;
    /// <summary>
    /// Required designer variable.
    /// 
    private System.ComponentModel.Container components = null;

    public ListBoxDialog()
    {
      InitializeComponent();
      this.AcceptButton = btn2OK;
      this.CancelButton = btn2Cnl;
      this.AcceptButton = btn2Add;
      this.AcceptButton = btn2Delete;
    }

    #region Dispose

    /// <summary>
    /// Clean up ay_nom resources being used.
    /// 
    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        if (components != null)
        {
          components.Dispose();
        }
      }
      base.Dispose(disposing);
    }

    #endregion

    #region Windows Form Designer generated code
    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// 
    private void InitializeComponent()
    {
      this.lblPrompt = new System.Windows.Forms.Label();
      this.btn2OK = new System.Windows.Forms.Button();
      this.btn2Cnl = new System.Windows.Forms.Button();
      this.btn2Add = new System.Windows.Forms.Button();
      this.btn2Delete = new System.Windows.Forms.Button();
      this.listInput = new System.Windows.Forms.ListBox();
      this.SuspendLayout();
      // 
      // lblPrompt
      // 
      this.lblPrompt.Anchor =
        ((System.Windows.Forms.AnchorStyles)
        ((((System.Windows.Forms.AnchorStyles.Top |
        System.Windows.Forms.AnchorStyles.Bottom)
        | System.Windows.Forms.AnchorStyles.Left)
        | System.Windows.Forms.AnchorStyles.Right)));
      this.lblPrompt.BackColor = System.Drawing.SystemColors.Control;
      this.lblPrompt.Font =
        new System.Drawing.Font("Microsoft Sans Serif", 9.75F,
        System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point,
        ((System.Byte)(0)));
      this.lblPrompt.Location = new System.Drawing.Point(12, 9);
      this.lblPrompt.Name = "lblPrompt2";
      this.lblPrompt.Size = new System.Drawing.Size(302, 82);
      this.lblPrompt.TabIndex = 4;
      // 
      // btnOK
      // 
      this.btn2OK.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.btn2OK.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
      this.btn2OK.Location = new System.Drawing.Point(326, 24);
      this.btn2OK.Name = "btnOK2";
      this.btn2OK.Size = new System.Drawing.Size(64, 24);
      this.btn2OK.TabIndex = 1;
      this.btn2OK.Text = "&OK";
      this.btn2OK.Click += new System.EventHandler(this.btn2OK_Click);
      // 
      // btnCnl
      // 
      this.btn2Cnl.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.btn2Cnl.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
      this.btn2Cnl.Location = new System.Drawing.Point(326, 56);
      this.btn2Cnl.Name = "button2";
      this.btn2Cnl.Size = new System.Drawing.Size(64, 24);
      this.btn2Cnl.TabIndex = 2;
      this.btn2Cnl.Text = "&Cancel";
      this.btn2Cnl.Click += new System.EventHandler(this.btn2Cnl_Click);
      // 
      // btnAdd
      // 
      this.btn2Add.DialogResult = System.Windows.Forms.DialogResult.Yes;
      this.btn2Add.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
      this.btn2Add.Location = new System.Drawing.Point(400, 24);
      this.btn2Add.Name = "btnAdd";
      this.btn2Add.Size = new System.Drawing.Size(64, 24);
      this.btn2Add.TabIndex = 3;
      this.btn2Add.Text = "&Add";
      this.btn2Add.Click += new System.EventHandler(this.btn2Add_Click);
      // 
      // btnDelete
      // 
      this.btn2Delete.DialogResult = System.Windows.Forms.DialogResult.Yes;
      this.btn2Delete.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
      this.btn2Delete.Location = new System.Drawing.Point(400, 56);
      this.btn2Delete.Name = "btnDelete";
      this.btn2Delete.Size = new System.Drawing.Size(64, 24);
      this.btn2Delete.TabIndex = 4;
      this.btn2Delete.Text = "&Delete";
      this.btn2Delete.Click += new System.EventHandler(this.btn2Delete_Click);
      // 
      // listInput
      // 
      this.listInput.Location = new System.Drawing.Point(8, 100);
      this.listInput.Name = "txtInput2";
      this.listInput.Size = new System.Drawing.Size(379, 70);
      this.listInput.TabIndex = 0;
      this.listInput.Text = "";
      // 
      // ListBoxDialog
      // 
      this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
      this.ClientSize = new System.Drawing.Size(475, 175);
      this.Controls.Add(this.listInput);
      this.Controls.Add(this.btn2Cnl);
      this.Controls.Add(this.btn2OK);
      this.Controls.Add(this.btn2Add);
      this.Controls.Add(this.btn2Delete);
      this.Controls.Add(this.lblPrompt);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "ListBoxDialog";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "InputBox";
      this.Load += new EventHandler(ListBoxDialog_Load);
      this.ResumeLayout(false);

    }
    #endregion

    #region Private Variables
    string formCaption = string.Empty;
    string formPrompt = string.Empty;
    int inputResponse = 0;
    string defaultValue = string.Empty;
    string[] t = new string[18];
    bool old;
    Electroimpact.TransformerInfo.cTool passedInTool;
    #endregion

    #region Public Properties

    public string FormCaption
    {
      get { return formCaption; }
      set { formCaption = value; }
    } // property FormCaption
    public string FormPrompt
    {
      get { return formPrompt; }
      set { formPrompt = value; }
    } // property FormPrompt
    public int InputResponse
    {
      get { return inputResponse; }
      set { inputResponse = value; }
    } // property InputResponse
    public string DefaultValue
    {
      get { return defaultValue; }
      set { defaultValue = value; }
    } // property DefaultValue
    public Electroimpact.TransformerInfo.cTool PassedInTool
    {
      get { return passedInTool; }
      set { passedInTool = value; }
    } // property PassedInTool

    #endregion

    #region Form and Control Events
    public void ListBoxDialog_Load(object sender, System.EventArgs e)
    {
      this.lblPrompt.Text = formPrompt;
      this.Text = formCaption;
      this.listInput.Focus();
      List<TransformerInfo.cBarrelTransform> BarrelTrans = TransformerInfo.getListOfBarrelT();
      int max = BarrelTrans.Count;
      for (int i = 0; i < max; i++)
      {
        this.listInput.Items.Add("Mandrel Unit " + (i + 1).ToString());
        continue;
      }
      int S = passedInTool == null ? 1 : passedInTool.M38S;
      this.listInput.SetSelected(S - 1, true);
    }


    private void btn2OK_Click(object sender, System.EventArgs e)
    {
      InputResponse = this.listInput.SelectedIndex;

      this.Close();
    }

    private void btn2Cnl_Click(object sender, System.EventArgs e)
    {
      this.Close();
    }

    private void btn2Add_Click(object sender, System.EventArgs e)
    {
      TransformerInfo.cBarrelTransform Barrelnew = new TransformerInfo.cBarrelTransform();
      List<TransformerInfo.cBarrelTransform> BarrelTrans = TransformerInfo.getListOfBarrelT();
      BarrelTrans.Add(Barrelnew);
      TransformerInfo.SaveListOfBarrelT(BarrelTrans);
      this.Close();
    }

    private void btn2Delete_Click(object sender, System.EventArgs e)
    {
      List<TransformerInfo.cBarrelTransform> BarrelTrans = TransformerInfo.getListOfBarrelT();
      int number = BarrelTrans.Count;
      BarrelTrans.Remove(BarrelTrans[number - 1]);
      TransformerInfo.SaveListOfBarrelT(BarrelTrans);
      this.Close();
    }

    #endregion

    #endregion
  }

  public class ListBoxDialog2 : System.Windows.Forms.Form
  {
    #region Windows Contols and Constructor

    private System.Windows.Forms.Label lblPrompt;
    private System.Windows.Forms.Button btn2OK;
    private System.Windows.Forms.Button btn2Cnl;
    private System.Windows.Forms.ListBox listInput;
    /// <summary>
    /// Required designer variable.
    /// 
    private System.ComponentModel.Container components = null;

    public ListBoxDialog2()
    {
      InitializeComponent();
      this.AcceptButton = btn2OK;
      this.CancelButton = btn2Cnl;
    }

    #region Dispose

    /// <summary>
    /// Clean up any resources being used.
    /// 
    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        if (components != null)
        {
          components.Dispose();
        }
      }
      base.Dispose(disposing);
    }

    #endregion

    #region Windows Form Designer generated code
    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// 
    private void InitializeComponent()
    {
      this.lblPrompt = new System.Windows.Forms.Label();
      this.btn2OK = new System.Windows.Forms.Button();
      this.btn2Cnl = new System.Windows.Forms.Button();
      this.listInput = new System.Windows.Forms.ListBox();
      this.SuspendLayout();
      // 
      // lblPrompt
      // 
      this.lblPrompt.Anchor =
        ((System.Windows.Forms.AnchorStyles)
        ((((System.Windows.Forms.AnchorStyles.Top |
        System.Windows.Forms.AnchorStyles.Bottom)
        | System.Windows.Forms.AnchorStyles.Left)
        | System.Windows.Forms.AnchorStyles.Right)));
      this.lblPrompt.BackColor = System.Drawing.SystemColors.Control;
      this.lblPrompt.Font =
        new System.Drawing.Font("Microsoft Sans Serif", 9.75F,
        System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point,
        ((System.Byte)(0)));
      this.lblPrompt.Location = new System.Drawing.Point(12, 9);
      this.lblPrompt.Name = "lblPrompt2";
      this.lblPrompt.Size = new System.Drawing.Size(302, 82);
      this.lblPrompt.TabIndex = 4;
      // 
      // btnOK
      // 
      this.btn2OK.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.btn2OK.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
      this.btn2OK.Location = new System.Drawing.Point(326, 24);
      this.btn2OK.Name = "btnOK2";
      this.btn2OK.Size = new System.Drawing.Size(64, 24);
      this.btn2OK.TabIndex = 1;
      this.btn2OK.Text = "&OK";
      this.btn2OK.Click += new System.EventHandler(this.btn2OK_Click);
      // 
      // btnCnl
      // 
      this.btn2Cnl.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.btn2Cnl.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
      this.btn2Cnl.Location = new System.Drawing.Point(326, 56);
      this.btn2Cnl.Name = "button2";
      this.btn2Cnl.Size = new System.Drawing.Size(64, 24);
      this.btn2Cnl.TabIndex = 2;
      this.btn2Cnl.Text = "&Cancel";
      this.btn2Cnl.Click += new System.EventHandler(this.btn2Cnl_Click);
      // 
      // listInput
      // 
      this.listInput.Location = new System.Drawing.Point(8, 100);
      this.listInput.Name = "txtInput2";
      this.listInput.Size = new System.Drawing.Size(379, 70);
      this.listInput.TabIndex = 0;
      this.listInput.Text = "";
      // 
      // ListBoxDialog
      // 
      this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
      this.ClientSize = new System.Drawing.Size(398, 175);
      this.Controls.Add(this.listInput);
      this.Controls.Add(this.btn2Cnl);
      this.Controls.Add(this.btn2OK);
      this.Controls.Add(this.lblPrompt);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "ListBoxDialog";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "InputBox";
      this.Load += new EventHandler(ListBoxDialog_Load);
      this.ResumeLayout(false);

    }
    #endregion

    #region Private Variables
    string formCaption = string.Empty;
    string formPrompt = string.Empty;
    string inputResponse = string.Empty;
    string defaultValue = string.Empty;
    string[] t = new string[18];
    bool old;
    Electroimpact.TransformerInfo.cTool passedInTool;
    #endregion

    #region Public Properties

    public string FormCaption
    {
      get { return formCaption; }
      set { formCaption = value; }
    } // property FormCaption
    public string FormPrompt
    {
      get { return formPrompt; }
      set { formPrompt = value; }
    } // property FormPrompt
    public string InputResponse
    {
      get { return inputResponse; }
      set { inputResponse = value; }
    } // property InputResponse
    public string DefaultValue
    {
      get { return defaultValue; }
      set { defaultValue = value; }
    } // property DefaultValue
    public string[] T
    {
      get { return t; }
      set { t = value; }
    } // property DefaultValue
    public Electroimpact.TransformerInfo.cTool PassedInTool
    {
      get { return passedInTool; }
      set { passedInTool = value; }
    } // property PassedInTool
    public bool Old
    {
      get { return old; }
      set { old = value; }
    } // property DefaultValue

    #endregion

    #region Form and Control Events
    public void ListBoxDialog_Load(object sender, System.EventArgs e)
    {
      this.lblPrompt.Text = formPrompt;
      this.Text = formCaption;
      this.listInput.Focus();
      int max = old == true ? 5 : 18;
      for (int i = 0; i < max; i++)
      {
        this.listInput.Items.Add(t[i]);
        continue;
      }
      int S = passedInTool == null ? 1 : passedInTool.M38S;
      this.listInput.SetSelected(S - 1, true);
    }


    private void btn2OK_Click(object sender, System.EventArgs e)
    {
      DocuTrackProSE.InputBoxDialog ib = new DocuTrackProSE.InputBoxDialog();
      ib.FormPrompt = "What is this comment being used for.";
      ib.FormCaption = "Edit Comment";
      int index = this.listInput.Text.IndexOf(" ");
      string transform = this.listInput.Text.Substring(0, index);
      string comment = this.listInput.Text.Substring(3 + index);
      ib.DefaultValue = comment;
      ib.ShowDialog();

      if (ib.DialogResult == DialogResult.Cancel)
      {
        InputResponse = "kejbnasdjkfnsdj";
        return;
      }

      this.listInput.Items.Add(transform + " - " + ib.InputResponse);
      int count = this.listInput.Items.Count;
      this.listInput.SetSelected(count - 1, true);
      InputResponse = this.listInput.Text;

      this.Close();
    }

    private void btn2Cnl_Click(object sender, System.EventArgs e)
    {
      this.Close();
    }

    #endregion

    #endregion
  }

}
#endregion

namespace NelderMeade
{

  class CRotation : OptFunctions
  {
    // Tell the optimizer what function to minimize 
    // 

    public double[] xB = null;  // Keep track of best
    int np = 0;  // Number of points
    int nVars = 3; //number of variables to solve: rX,rY,rZ,...
    public double jB = Double.MaxValue;
    public int nEval = 0;
    double tol = 0.01;  // tolerance to determine when to terminate search
    double ConvergenceTol;// tolerance for convergence of solution, if all guesses are within this amount then quit solving
    //Electroimpact.Transformer.frmBarrelTransformer myBrlTfrmr;
    double[] xmeas = null;
    double[] ymeas = null;
    double[] zmeas = null;
    double[] r2 = null;

    public CRotation(double tol, double[] xm, double[] ym, double[] zm)
    {
      this.tol = tol;
      this.ConvergenceTol = tol / 10.0;
      xB = new double[nVars];

      xmeas = xm;
      ymeas = ym;
      zmeas = zm;

      r2 = new double[xmeas.Length];
    }

    public double performanceFunc(double[] x)
    {
      //solve for center of rotation for a single point at multiple rotations
      nEval++;
      double err = 0.0;

      //calc r^2 for each
      for (int i = 0; i < xmeas.Length; i++)
      {
        r2[i] = (xmeas[i] - x[0]) * (xmeas[i] - x[0]) + (ymeas[i] - x[1]) * (ymeas[i] - x[1]) + (zmeas[i] - x[2]) * (zmeas[i] - x[2]);
      }

      //cycle through each meas
      for (int i = 0; i < r2.Length; i++)
      {
        //compare against each meas
        for (int j = 0; j < r2.Length; j++)
        {
          err += Math.Abs(r2[j] - r2[i]);
        }
      }

      return err;
    }

    public double GetTol()
    {
      return tol;
    }

    public double GetConvergenceTol()
    {
      return ConvergenceTol;
    }

    public bool optProgress(double jBest, double jNextBest,
            double[] xBest, double[] xNextBest)
    {
      // Return value is true if search should continue
      // Copy current best
      for (int i = 0; i < nVars; i++) xB[i] = xBest[i];
      jB = jBest;

      if (tol <= 0.0) return true;  // Don't bother testing!

      if (jBest < tol)
      {
        return false;  // Converged, don't continue search
      }
      else return true;
    }
  }

  class BTformer : OptFunctions
  {


    // Tell the optimizer what function to minimize 
    // 

    public double[] xB = null;  // Keep track of best
    int np = 0;  // Number of points
    int nVars = 10; //number of variables to solve: rX,rY,rZ,...
    public double jB = Double.MaxValue;
    public int nEval = 0;
    double tol = 0.01;  // tolerance to determine when to terminate search
    double ConvergenceTol;// tolerance for convergence of solution, if all guesses are within this amount then quit solving
    Electroimpact.Transformer.frmBarrelTransformer myBrlTfrmr;


    public BTformer(double tol, Electroimpact.Transformer.frmBarrelTransformer BrlTfrmerIn)//System.Collections.Generic.List<cPoint> lPoints)
    {
      this.tol = tol;
      this.ConvergenceTol = tol / 10.0;
      myBrlTfrmr = BrlTfrmerIn;
      xB = new double[nVars]; //use 10 variables for barrel transform
    }

    public double performanceFunc(double[] x)
    {
      nEval++;  // Count the number of function evaluations
      double err = myBrlTfrmr.CalculateError(x);
      return err;
    }

    public double GetTol()
    {
      return tol;
    }

    public double GetConvergenceTol()
    {
      return ConvergenceTol;
    }

    public bool optProgress(double jBest, double jNextBest,
            double[] xBest, double[] xNextBest)
    {
      // Return value is true if search should continue
      // Copy current best
      for (int i = 0; i < nVars; i++) xB[i] = xBest[i];
      jB = jBest;

      if (tol <= 0.0) return true;  // Don't bother testing!

      if (jBest < tol)
      {
        return false;  // Converged, don't continue search
      }
      else return true;
    }

  }


}
