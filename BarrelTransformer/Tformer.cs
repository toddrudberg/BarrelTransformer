using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
//using Electroimpact.Transformer;
using Electroimpact;

namespace NelderMeade
{
  class Tformer: OptFunctions
  {

    
    // Tell the optimizer what function to minimize 
    // 

    public double[] xB = null;  // Keep track of best
    //double[][] Noms;  // nominals
    //double[][] Meas;  // measured
    int np = 0;  // Number of points
    int nVars = 10; //number of variables to solve: rX,rY,rZ
    public double jB = Double.MaxValue;
    public int nEval = 0;
    double tol = 0.01;  // tolerance to determine when to terminate search
    //double[] Xcalc;
    //double[] Ycalc;
    //double[] Zcalc;

    Transformer.frmBarrelTransformer myBrlTfrmr;
    
    //System.Collections.Generic.List<cPoint> PointsList;



    public Tformer(double tol, Transformer.frmBarrelTransformer BrlTfrmerIn)//System.Collections.Generic.List<cPoint> lPoints)
    {
      this.tol = tol;

      myBrlTfrmr = BrlTfrmerIn;

      xB = new double[nVars]; //use 10 variables for barrel transform

      //PointsList = lPoints;
      
      
      //np = lPoints.Count;// Check length of array
     

      
      //Xcalc = new double[np];
      //Ycalc = new double[np];
      //Zcalc = new double[np];

      

    }

    public double performanceFunc(double[] x)
    {
      nEval++;  // Count the number of function evaluations
      double sum = 0.0;
      //double ave = 0.0;
      double[] SumSq = new double[np];// = 0.0;

      double d2r = Math.PI / 180.0;

      //Evaluator_Evaluate
      //CalculateError()
      return myBrlTfrmr.CalculateError(x);
      //return Transformer.frmBarrelTransformer.CalculateError();
        

        //case 9:
        //  myBarrelFunction.SpinToFRC.X = myBarrelFunction.SpinToFRC.Y = myBarrelFunction.SpinToFRC.Z = 0;
        //  myBarrelFunction.SpinToFRC.rX = ev.Problem.VarDecision.Value[0] * d2r;
        //  myBarrelFunction.SpinToFRC.rY = ev.Problem.VarDecision.Value[1] * d2r;
        //  myBarrelFunction.SpinToFRC.rZ = ev.Problem.VarDecision.Value[2] * d2r;
        //  myBarrelFunction.A_x = ev.Problem.VarDecision.Value[3];
        //  myBarrelFunction.MandrelToSpin.X = ev.Problem.VarDecision.Value[4];
        //  myBarrelFunction.MandrelToSpin.Y = ev.Problem.VarDecision.Value[5];
        //  myBarrelFunction.MandrelToSpin.Z = ev.Problem.VarDecision.Value[6];
        //  myBarrelFunction.MandrelToSpin.rX = ev.Problem.VarDecision.Value[7] * d2r;
        //  myBarrelFunction.MandrelToSpin.rY = ev.Problem.VarDecision.Value[8] * d2r;
        //  myBarrelFunction.MandrelToSpin.rZ = ev.Problem.VarDecision.Value[9] * d2r;
        //  if (chkStupid.Checked)
        //  {
        //    myBarrelFunction.A_x = 0;
        //    myBarrelFunction.MandrelToSpin.X = 0;
        //    myBarrelFunction.MandrelToSpin.Y = 0;
        //    myBarrelFunction.MandrelToSpin.Z = 0;
        //    myBarrelFunction.MandrelToSpin.rX = 0;
        //    myBarrelFunction.MandrelToSpin.rY = 0;
        //    myBarrelFunction.MandrelToSpin.rZ = 0;
        //  }


      //double rX = d2r * x[0];
      //double rY = d2r * x[1];
      //double rZ = d2r * x[2];

      //double sA = Math.Sin(rX);
      //double cA = Math.Cos(rX);
      //double sB = Math.Sin(rY);
      //double cB = Math.Cos(rY);
      //double sC = Math.Sin(rZ);
      //double cC = Math.Cos(rZ);

      //double M11 = cB * cC;
      //double M12 = -cB * sC;
      //double M13 = sB;
      //double M21 = cA * sC + sA * sB * cC;
      //double M22 = cA * cC - sA * sB * sC;
      //double M23 = -sA * cB;
      //double M31 = sA * sC - cA * sB * cC;
      //double M32 = sA * cC + cA * sB * sC;
      //double M33 = cA * cB;



      //for (int i = 0; i < np; i++)
      //{
      //  if (!PointsList[i].bUseInXform)
      //    continue;


      //  Xcalc[i] = M11 * PointsList[i].nx + M12 * PointsList[i].ny + M13 * PointsList[i].nz;// +x[3];
      //  Ycalc[i] = M21 * PointsList[i].nx + M22 * PointsList[i].ny + M23 * PointsList[i].nz;// +x[7];
      //  Zcalc[i] = M31 * PointsList[i].nx + M32 * PointsList[i].ny + M33 * PointsList[i].nz;// +x[11];


      //  //sum of squares
      //  //do NOT use square root of sum of squares, that just gets the best average distance
      //  SumSq[i] = ((PointsList[i].mx - Xcalc[i]) * (PointsList[i].mx - Xcalc[i]) +
      //            (PointsList[i].my - Ycalc[i]) * (PointsList[i].my - Ycalc[i]) +
      //            (PointsList[i].mz - Zcalc[i]) * (PointsList[i].mz - Zcalc[i]));

      //  sum += SumSq[i];
      //}

      return sum;
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
