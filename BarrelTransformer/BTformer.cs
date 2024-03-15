using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
//using Electroimpact.Transformer;
using Electroimpact;


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
//"AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
//LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
//FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE
//COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT,
//INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING,
//BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
//LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
//CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT,
//STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
//ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED
//OF THE POSSIBILITY OF SUCH DAMAGE.
//====================================

namespace NelderMeade
{
  class BTformer: OptFunctions
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



    public BTformer(double tol, Transformer.frmBarrelTransformer BrlTfrmerIn)//System.Collections.Generic.List<cPoint> lPoints)
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
      //double sum = 0.0;
      //double ave = 0.0;
      //double[] SumSq = new double[np];// = 0.0;

      //double d2r = Math.PI / 180.0;

      //Evaluator_Evaluate
      //CalculateError()
      double err = myBrlTfrmr.CalculateError(x);
      //if (nEval > 549)
      //  err = err;
      return err;

    }

    public double GetTol()
    {
      return tol;
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
