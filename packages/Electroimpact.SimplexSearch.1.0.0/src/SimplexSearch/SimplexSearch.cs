using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;


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

  public interface OptFunctions //to use the simplex optimizer a class must implement this interface
  {
    double performanceFunc(double[] x); //calculate the error
    Boolean optProgress(double jBest, double jNextBest, double[] xBest, double[] xNextBest); //quit if solution is good enough
    double GetTol(); //get the tolerance for a successful solution
    double GetConvergenceTol(); //get the tolerance for solution conversion
  }

  public class SimplexSearch
  {

    private OptFunctions funcs = null; //reference to a class that implements OptFunctions

    private int maxIt; //max number of iterations
    private int itCount = 0;  //Iteration counter
    double sign = 1.0; //Use 1.0 to minimize, -1.0 to maximize
    double[][] simplex = null;
    double[] jSimplex = null; //performance values
    int isBest = -1;
    int isNextBest = -1;
    int isWorst = -1;
    int isNextWorst = -1;
    int nParam = 0;

    public SimplexSearch(OptFunctions funcs, int maxIt)
    {
      this.funcs = funcs;
      this.maxIt = maxIt;
    }

    public int getBest()
    {
      return isBest;
    }

    public double[] getSimplex(int index)
    {
      return simplex[index];
    }

    public Boolean minimize(double[] xMin, double[] xMax)
    {
      bool useLimits = false;
      bool success = false;
      itCount = 0;
      nParam = xMin.Length; //Number of parameters
      jSimplex = new double[nParam +1]; //Performance index values

      //need this to be a jagged multidimensional array so can entire rows and columns
      simplex = new double[nParam + 1][];//nParam]; //each row is a parameter vector
      for (int i = 0; i < nParam + 1; i++)
        simplex[i] = new double[nParam];



      double[] x = new double[nParam]; //parameter vector
      double[] xBest = new double[nParam]; // Best parameter set thus far
      double[] xNextBest = new double[nParam];

      // Constuct the initial simplex
      // Use the center of the search space to start the simplex
      double[] xCenter = new double[nParam];
      double[] dxs = new double[nParam];  // Offset for simplex construction

      for (int i = 0; i < nParam; i++)
      {
        xCenter[i] = (xMin[i] + xMax[i]) / 2.0;
        dxs[i] = (xMax[i] - xMin[i]) * 0.2;
        simplex[0][i] = xCenter[i];  // First simplex point is at
        // the center of the parameter space
      }
      jSimplex[0] = sign * funcs.performanceFunc(simplex[0]);


      for (int i = 1; i < (nParam + 1); i++)
      {
        for (int j = 0; j < nParam; j++)
        {
          // i is the simplex number; j is the parameter number
          if ((i - 1) == j)
            simplex[i][j] = xCenter[j] + dxs[j];
          else
            simplex[i][j] = xCenter[j];
        }
        jSimplex[i] = sign * funcs.performanceFunc(simplex[i]);
        itCount++;
      }

      while (itCount < maxIt)
      {
        sortSimplex();  // Make sure the simplex is sorted
        //***printSimplex();
        double[] xTry = new double[nParam];
        double[] xTry2 = new double[nParam];
        double jTry2 = Double.MaxValue;//Double.MAX_VALUE;
        reflectFromWorst(1.0, xTry);

        if (useLimits)
        {
          for (int i = 0; i < nParam; i++)
          {
            if (xTry[i] < xMin[i])
              xTry[i] = xMin[i];
            if (xTry[i] > xMax[i])
              xTry[i] = xMax[i];
          }
        }

        double jTry = sign * funcs.performanceFunc(xTry);
        itCount++;
        //***System.out.println("itCount, jTry: " + itCount + " " + jTry);
        if (jTry < jSimplex[isBest])
        {
          // This try was better than the best
          // Try again with a larger step
          reflectFromWorst(2.0, xTry2);

          if (useLimits)
          {
            for (int i = 0; i < nParam; i++)
            {
              if (xTry2[i] < xMin[i])
                xTry2[i] = xMin[i];
              if (xTry2[i] > xMax[i])
                xTry2[i] = xMax[i];
            }
          }

          jTry2 = sign * funcs.performanceFunc(xTry2);
          itCount++;
          //***System.out.println("jTry2: " + jTry2);
          if (jTry2 < jTry)
          {
            // Still better, replace worst with this and re-sort
            for (int i = 0; i < nParam; i++)
            {
              simplex[isWorst][i] = xTry2[i];
              jSimplex[isWorst] = jTry2;
            }
          }
          else
          {
            // Not better, replace worst with xTry and re-sort
            for (int i = 0; i < nParam; i++)
            {
              simplex[isWorst][i] = xTry[i];
              jSimplex[isWorst] = jTry;
            }
          }
        }
        else if ((jTry < jSimplex[isWorst]) ||
                (jTry2 < jSimplex[isWorst]))
        {
          // We can replace the current worst point with either
          // try or try2
          if (jTry < jTry2)
          {
            // jTry is better than the worst point, replace the worst with
            // this one and re-sort for another trial
            for (int i = 0; i < nParam; i++)
            {
              simplex[isWorst][i] = xTry[i];
              jSimplex[isWorst] = jTry;
            }
          }
          else
          {
            // jTry2 is better than the worst point, replace the worst with
            // this one and re-sort for another trial
            for (int i = 0; i < nParam; i++)
            {
              simplex[isWorst][i] = xTry2[i];
              jSimplex[isWorst] = jTry2;
            }
          }
        }
        else
        {
          // No good news yet! Try a reduced reflection
          //  Re-use jTry, xTry -- they are known bad at this
          // point.
          reflectFromWorst(0.5, xTry);

          if (useLimits)
          {
            for (int i = 0; i < nParam; i++)
            {
              if (xTry[i] < xMin[i])
                xTry[i] = xMin[i];
              if (xTry[i] > xMax[i])
                xTry[i] = xMax[i];
            }
          }

          jTry = sign * funcs.performanceFunc(xTry);
          if (jTry < jSimplex[isWorst])
          {
            for (int i = 0; i < nParam; i++)
            {
              simplex[isWorst][i] = xTry[i];
              jSimplex[isWorst] = jTry;
            }
          }
          else
          {
            // Nothing worked! Contract about the best point
            // and try again
            contractOnBest(0.5);
            // Evaluate this new simplex
            for (int i = 0; i < (nParam + 1); i++)
            {
              jSimplex[i] = sign * funcs.performanceFunc(simplex[i]);
              itCount++;
            }
          }
        }

        // Call optProgress to see whether to continue searching
        sortSimplex();  // Make sure the simplex is sorted
        if (!funcs.optProgress(sign * jSimplex[isBest],
          sign * jSimplex[isNextBest],
              simplex[isBest], simplex[isNextBest]))
        {
          success = true;
          break;
        }

        //return if all guesses have converged
        if (Math.Abs(sign * jSimplex[isBest] - sign * jSimplex[isWorst]) < funcs.GetConvergenceTol())
        {
          success = false;
          return success;
        }

      } // End of main iteration loop

      return success;
    }

    public bool maximize(double[] xMin, double[] xMax)
    {
      sign = -1.0;
      return minimize(xMin, xMax);
    }


    public void sortSimplex()
    {
      // Get best, next best, and worst indices
      double jBest = jSimplex[0];
      isBest = 0;
      double jWorst = jSimplex[0];
      isWorst = 0;

      for (int i = 1; i < (nParam + 1); i++)
      {
        if (jSimplex[i] < jBest)
        {
          jBest = jSimplex[i];
          isBest = i;
        }
        if (jSimplex[i] > jWorst)
        {
          jWorst = jSimplex[i];
          isWorst = i;
        }
      }
      // Get next best and next worst by searching over all but the best
      double jNextBest = jSimplex[isWorst];
      isNextBest = isWorst;
      double jNextWorst = jSimplex[isBest];
      isNextWorst = isBest;
      for (int i = 0; i < (nParam + 1); i++)
      {
        // Don't consider the best or worst vertices
        if ((i == isBest) || (i == isWorst)) continue;

        if (jSimplex[i] < jNextBest)
        {
          jNextBest = jSimplex[i];
          isNextBest = i;
        }
        if (jSimplex[i] > jNextWorst)
        {
          jNextWorst = jSimplex[i];
          isNextWorst = i;
        }
      }
    }

    public void reflectFromWorst(double factor, double[] xTry)
    {
      // Reflect from the worst point through the centroid
      // of all points except the worst
      // factor = 1.0 reflects at the same size
      double[] xCentroid = new double[nParam];  // Centroid for reflection
      for (int i = 0; i < (nParam + 1); i++)
      {
        if (i == isWorst) continue;  // Don't include worst point
        for (int j = 0; j < nParam; j++)
          xCentroid[j] += simplex[i][j];
      }
      for (int j = 0; j < nParam; j++)
        xCentroid[j] /= nParam;  // Average the values
      //printParameterVector("Centroid",xCentroid);

      //double [] xDif = new double[nParam];
      for (int i = 0; i < nParam; i++)
      {
        double xDif = xCentroid[i] - simplex[isWorst][i];
        xTry[i] = xCentroid[i] + factor * xDif;
      }
      //printParameterVector("<reflect> xTry", xTry);
    }

    public void contractOnBest(double factor)
    {
      // Contract keeping only the best point
      for (int i = 0; i < (nParam + 1); i++)
      {
        if (i == isBest) continue;  // Don't touch the best point
        for (int j = 0; j < nParam; j++)
        {
          double xNew = simplex[isBest][j] + factor *
          (simplex[i][j] - simplex[isBest][j]);
          simplex[i][j] = xNew;
        }
        //jSimplex[i] = sign * funcs.performanceFunc(simplex[i]);
        //itCount++;
      }
    }
  }
}
