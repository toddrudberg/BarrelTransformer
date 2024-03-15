using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Electroimpact
{

  public class cPoint
  {
    const double d2r = Math.PI / 180.0;
    public double nx;
    public double ny;
    public double nz;
    public double mx;
    public double my;
    public double mz;
    public double upos;
    public bool bUseInXform = true;
    public double errInXform = 0.0;
    public int point_number = 0;

    public cPoint()
    {
      nx = 0;
      ny = 0;
      nz = 0;
      mx = 0;
      my = 0;
      mz = 0;
      upos = 0;
    }

    public cPoint(double nX, double nY, double nZ, double mX, double mY, double mZ, double Upos)
    {
      nx = nX;
      ny = nY;
      nz = nZ;
      mx = mX;
      my = mY;
      mz = mZ;
      upos = Upos;
    }

    public double[] toArray()
    {
      double[] dog = new double[4];
      dog[0] = mx;
      dog[1] = my;
      dog[2] = mz;
      dog[3] = 1.0;
      return dog;
    }
    public double[] toArrayNominal()//double Ax, double X, double Y, double Z, double rx, double ry, double rz)
    {
      double[] dog = new double[4];
      //Electroimpact.LinearAlgebra.RxMatrix u = new Electroimpact.LinearAlgebra.RxMatrix(upos * d2r);

      dog[0] = nx;
      dog[1] = ny;
      dog[2] = nz;
      dog[3] = 1.0;

      //Electroimpact.LinearAlgebra.c6dof m = new Electroimpact.LinearAlgebra.c6dof(X, Y, Z, rx * d2r, ry * d2r, rz * d2r);
      //dog = m.DotMe(dog);

      //dog[0] = nx + Ax * Math.Sin((upos - upos / 2) * d2r);

      //dog = u.DotMe(dog);

      return dog;
    }

    public override string ToString()
    {
      string output;
      output = "Point " + point_number.ToString() + ".  Radial Fit (inch): " + errInXform.ToString("F3");
      return output;
    }
  }

  public class cBarrelFunctions
  {
    public Electroimpact.LinearAlgebra.c6dof MandrelToSpin;
    public Electroimpact.LinearAlgebra.c6dof SpinToFRC;
    public Electroimpact.LinearAlgebra.c6dof AtoB = new Electroimpact.LinearAlgebra.c6dof();
    public double A_x;
    double d2r = Math.PI / 180.0;

    public cBarrelFunctions()
    {
      MandrelToSpin = new Electroimpact.LinearAlgebra.c6dof();
      SpinToFRC = new Electroimpact.LinearAlgebra.c6dof();
      A_x = 0;
    }

    /// <summary>
    /// Use this constructor when building from a setup string
    /// </summary>
    /// <param name="Setup"> a .csv sting where we have X,Y,Z,rX,rY,rZ,A_x,X,Y,Z,rX,rY,rZ.  rotaries are in degrees</param>
    public cBarrelFunctions(string Setup)
    {
      MandrelToSpin = new Electroimpact.LinearAlgebra.c6dof();
      SpinToFRC = new Electroimpact.LinearAlgebra.c6dof();

      string[] Lines = Setup.Split(',');
      if (Lines.Length >= 13) //used >= incase of extra string in a csv split.
      {
        double dog = 0.0;
        MandrelToSpin.X = double.TryParse(Lines[0], out dog) ? dog : 0.0;
        MandrelToSpin.Y = double.TryParse(Lines[1], out dog) ? dog : 0.0;
        MandrelToSpin.Z = double.TryParse(Lines[2], out dog) ? dog : 0.0;
        MandrelToSpin.rX = double.TryParse(Lines[3], out dog) ? dog * d2r : 0.0;
        MandrelToSpin.rY = double.TryParse(Lines[4], out dog) ? dog * d2r : 0.0;
        MandrelToSpin.rZ = double.TryParse(Lines[5], out dog) ? dog * d2r : 0.0;
        A_x = double.TryParse(Lines[6], out A_x) ? A_x : 0.0;
        SpinToFRC.X = double.TryParse(Lines[7], out dog) ? dog : 0.0;
        SpinToFRC.Y = double.TryParse(Lines[8], out dog) ? dog : 0.0;
        SpinToFRC.Z = double.TryParse(Lines[9], out dog) ? dog : 0.0;
        SpinToFRC.rX = double.TryParse(Lines[10], out dog) ? dog * d2r : 0.0;
        SpinToFRC.rY = double.TryParse(Lines[11], out dog) ? dog * d2r : 0.0;
        SpinToFRC.rZ = double.TryParse(Lines[12], out dog) ? dog * d2r : 0.0;
      }
    }
    /// <summary>
    /// Use this constructor when building from cnc mamory and you only one a single rigid body transform.
    /// </summary>
    /// <param name="Setup"> an arrary where we have X,Y,Z,rX,rY,rZ,A_x,X,Y,Z,rX,rY,rZ.  rotaries are in degrees</param>
    public cBarrelFunctions(int[] arguments)
    {
      //arguments inside the cnc are in mm, so need to convert to inch.
      MandrelToSpin = new Electroimpact.LinearAlgebra.c6dof();
      SpinToFRC = new Electroimpact.LinearAlgebra.c6dof();
      double mult_offset = 1.0e-3 / 25.4;
      double mult_angular = 1.0e-6 * d2r;
      if (arguments.Length >= 6)
      {
        MandrelToSpin.X = (double)arguments[0] * mult_offset;
        MandrelToSpin.Y = (double)arguments[1] * mult_offset;
        MandrelToSpin.Z = (double)arguments[2] * mult_offset;
        MandrelToSpin.rX = (double)arguments[3] * mult_angular;
        MandrelToSpin.rY = (double)arguments[4] * mult_angular;
        MandrelToSpin.rZ = (double)arguments[5] * mult_angular;
        A_x = 0.0;
      }      
    }

    /// <summary>
    /// Use this constructor when building from cnc mamory and you want a full barrel transform.
    /// </summary>
    /// <param name="Setup"> an arrary where we have X,Y,Z,rX,rY,rZ,A_x,X,Y,Z,rX,rY,rZ.  rotaries are in degrees</param>
    public cBarrelFunctions(int[] man_to_spin, int[] spin_to_frc, int a_x)
    {
      MandrelToSpin = new Electroimpact.LinearAlgebra.c6dof();
      SpinToFRC = new Electroimpact.LinearAlgebra.c6dof();
      double mult_offset = 1.0e-3 / 25.4;
      double mult_angular = 1.0e-6 * d2r;
      if (man_to_spin.Length >= 6)
      {
        MandrelToSpin.X = (double)man_to_spin[0] * mult_offset;
        MandrelToSpin.Y = (double)man_to_spin[1] * mult_offset;
        MandrelToSpin.Z = (double)man_to_spin[2] * mult_offset;
        MandrelToSpin.rX = (double)man_to_spin[3] * mult_angular;
        MandrelToSpin.rY = (double)man_to_spin[4] * mult_angular;
        MandrelToSpin.rZ = (double)man_to_spin[5] * mult_angular;
      }
      if (spin_to_frc.Length >= 6)
      {
        SpinToFRC.X = (double)spin_to_frc[0] * mult_offset;
        SpinToFRC.Y = (double)spin_to_frc[1] * mult_offset;
        SpinToFRC.Z = (double)spin_to_frc[2] * mult_offset;
        SpinToFRC.rX = (double)spin_to_frc[3] * mult_angular;
        SpinToFRC.rY = (double)spin_to_frc[4] * mult_angular;
        SpinToFRC.rZ = (double)spin_to_frc[5] * mult_angular;
      }
      A_x = a_x * mult_offset;
    }

    public cBarrelFunctions(Electroimpact.LinearAlgebra.c6dof mandreltospin, double a_x, Electroimpact.LinearAlgebra.c6dof spintofrc)
    {
      MandrelToSpin = mandreltospin;
      SpinToFRC = spintofrc;
      A_x = a_x;
    }

    public void SetAirplaneToBarrel(double x, double y, double z, double rX, double rY, double rZ)
    {
      Electroimpact.LinearAlgebra.RrMatrix T1 = new Electroimpact.LinearAlgebra.RrMatrix(x, y, z);
      Electroimpact.LinearAlgebra.c6dof rotations = new Electroimpact.LinearAlgebra.c6dof(0, 0, 0, -rX * d2r, rY * d2r, rZ * d2r);
      AtoB = new Electroimpact.LinearAlgebra.c6dof(rotations.DotMe(T1.GetMatrix));
    }

    public double[] AirplaneToBarrel(double x, double y, double z)
    {
      Electroimpact.LinearAlgebra.RrMatrix rr = new Electroimpact.LinearAlgebra.RrMatrix(x, y, z);
      double[,] ret = AtoB.DotMe(rr.GetMatrix);
      double[] dang = { ret[0, 3], ret[1, 3], ret[2, 3], 1 };
      return dang;
    }

    /// <summary>
    /// returns the coordinate in mandrel or "barrel" coordinates.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <param name="rx">IN DEGREES</param>
    /// <param name="ry">IN DEGREES</param>
    /// <param name="rz">IN DEGREES</param>
    /// <returns></returns>
    public Electroimpact.LinearAlgebra.c6dof AirplaneToBarrel(double x, double y, double z, double rx, double ry, double rz)
    {
      Electroimpact.LinearAlgebra.c6dof m = new LinearAlgebra.c6dof(x, y, z, rx * d2r, ry * d2r, rz * d2r);
      m = new LinearAlgebra.c6dof(AtoB.DotMe(m.GetMatrix()));
      return m;
    }

    public double[] BarrelToAirplane(double x, double y, double z)
    {
      Electroimpact.LinearAlgebra.c6dof BtoA = new Electroimpact.LinearAlgebra.c6dof(AtoB.Inverse());
      Electroimpact.LinearAlgebra.RrMatrix rr = new Electroimpact.LinearAlgebra.RrMatrix(x, y, z);
      double[,] ret = BtoA.DotMe(rr.GetMatrix);
      double[] dang = {ret[0,3],ret[1,3],ret[2,3], 1};
      return dang;
    }

    public void ClearTransform()
    {
      MandrelToSpin.GetMatrix(0, 0, 0, 0, 0, 0);
      SpinToFRC.GetMatrix(0, 0, 0, 0, 0, 0);
      SetAirplaneToBarrel(0, 0, 0, 0, 0, 0);
      A_x = 0;
    }

    public double[] BarrelToFRC(double x, double y, double z, double u)
    {
      return BarrelToFRC(new cPoint(x, y, z, 0, 0, 0, u));
    }

    public double[] BarrelToFRC(cPoint Point)
    {
      double[] ret;
      double d2r = Math.PI / 180.0;
      Electroimpact.LinearAlgebra.RxMatrix u = new Electroimpact.LinearAlgebra.RxMatrix(Point.upos * d2r);
      ret = Point.toArrayNominal();
      ret = MandrelToSpin.DotMe(Point.toArrayNominal()); // where is the point relative to the spin axis?
      ret[0] += A_x * Math.Sin(Point.upos / 2.0 * d2r);  // stupid simple X function
      ret = u.DotMe(ret);                                // spin the part
      ret = SpinToFRC.DotMe(ret);                        // Where is the spun part relative to the FRC.
      return ret;
    }

    public Electroimpact.LinearAlgebra.c6dof BarrelToFRC(Electroimpact.LinearAlgebra.c6dof Point, double upos)
    {

      double d2r = Math.PI / 180.0;
      Electroimpact.LinearAlgebra.RxMatrix u = new Electroimpact.LinearAlgebra.RxMatrix(upos * d2r);

      Electroimpact.LinearAlgebra.c6dof ret = new Electroimpact.LinearAlgebra.c6dof(MandrelToSpin.DotMe(Point.GetMatrix())); // where is the point relative to the spin axis?
      ret.X += A_x * Math.Sin(upos / 2.0 * d2r);                                              // stupid simple X function
      ret = new LinearAlgebra.c6dof(u.DotMe(ret.GetMatrix()));                               // spin the part
      ret = new LinearAlgebra.c6dof(SpinToFRC.DotMe(ret.GetMatrix()));                        // Where is the spun part relative to the FRC.
      return ret;
    }
    /// <summary>
    /// returns a value in degrees representing the correct A
    /// </summary>
    /// <param name="point"></param>
    /// <param name="distanceFromU0toA0"></param>
    /// <returns></returns>
    public double ConvertAtoPurerX(Electroimpact.LinearAlgebra.c6dof point, double distanceFromU0toA0)
    {
      double thetaTodd = Theta(point.X, point.Y, point.Z, distanceFromU0toA0);
      return RolloverAxisDistance(point.rX / d2r + thetaTodd);
    }

    public double ConvertAtoPurerX(Electroimpact.LinearAlgebra.c6dof point, double distanceFromU0toA0, List<cStation> stns)
    {
      double thetaTodd = Theta(point.X, point.Y, point.Z, distanceFromU0toA0, stns);
      return RolloverAxisDistance(point.rX / d2r + thetaTodd);
    }

    public double Theta(double x, double y, double z, double angleToUzero) //god how I had angleToUzero!  distance in degrees from the U zero pos to the A zero pos...i know compilcated
    {
      double theta;
      double R;
      double alpha = 0.0;
      //long dong[18];
      //tStation cs;

      //dong[0] = DoubleToLong( y * 1000.0 );  //7600
      //dong[1] = DoubleToLong( z * 1000.0 ); //7604

      //if (myCentroid.count > 0)
      //{
      //  Comp_CreateCompStation(myCentroid.myPoints, (long)myCentroid.count, x, &cs);
      //  y -= cs.Y; //as it was in mukilteo.  
      //  //Makes sense...work it out on paper.  But consider upper right hand corner:  
      //  //Say Y = 100".  If the virtual center is down 32" (argument = -32") then
      //  //Y' = Y - -32 = 132".  This will give you the correct angle.
      //  //Same logic for Z, but z argument is typically zero or very close to zero.
      //  z -= cs.Z;
      //}
      double d2r = Math.PI / 180.0;
      R = Math.Sqrt(z * z + y * y);
      if (R > 0)
      {
        alpha = Math.Asin(z / R) / d2r;
        if (y >= 0)
          theta = alpha;
        else
          theta = 180.0 - alpha;
        theta += angleToUzero;
        return RolloverAxisPosition(theta);
      }
      return 0.0;

    }

    public double Theta(double x, double y, double z, double Offset, List<cStation> stns)
    {
      double theta;
      double R;
      double alpha = 0.0;
      double DegToRad = Math.PI / 180.0;
      //long dong[18];
      //tStation cs;

      //dong[0] = DoubleToLong( y * 1000.0 );  //7600
      //dong[1] = DoubleToLong( z * 1000.0 ); //7604

      if (stns.Count > 0)
      {
        cStation cs;
        Comp_CreateCompStation(stns, x, out cs);
        y -= cs.Y; //as it was in mukilteo.  
        //Makes sense...work it out on paper.  But consider upper right hand corner:  
        //Say Y = 100".  If the virtual center is down 32" (argument = -32") then
        //Y' = Y - -32 = 132".  This will give you the correct angle.
        //Same logic for Z, but z argument is typically zero or very close to zero.
        z -= cs.Z;
      }

      R = Math.Sqrt(z * z + y * y);
      if (R > 0)
      {
        alpha = Math.Asin(z / R) / DegToRad;
        if (y >= 0)
          theta = alpha;
        else
          theta = 180.0 - alpha;
        theta += Offset;
        return RolloverAxisPosition(theta);
      }
      return 0;
    }

    public class cStation
    {
      public double loc = 0; //use as Tow Command for tow blocks.
      public double X = 0;
      public double Y = 0;
      public double Z = 0;
    }
    private void Comp_CreateCompStation(List<cStation> stations, double AxVal, out cStation csi)
    {
      cStation stnNext = new cStation();
      cStation stnPrev = new cStation();
      csi = new cStation();
      int test, lb, dist;
      test = stations.Count / 2;//Length / 2;
      lb = stations.Count;
      for (; ; )
      {
        stnNext = stations[test];
        if (AxVal <= (double)stnNext.loc)
        {
          //stnPrev = stnNext - 1;
          stnPrev = stations[test - 1];
          if (AxVal > (double)stnPrev.loc)
            break;
          else
          {
            lb = test;
            test /= 2;
            if (test == 0)
              break;
          }
        }
        else
        {
          if (test == stations.Count - 1)
          {
            //memcpy(csi, stations + Length - 1, sizeof(tStation));
            csi.loc = 0.0;
            csi.X = 0.0;
            csi.Y = stations[stations.Count - 1].Y;
            csi.Z = stations[stations.Count - 1].Z;
            return;
          }
          dist = (lb - test) / 2;
          dist = dist > 1 ? dist : 1;
          test += dist;
        }
      }
      if (test > 0)
      {
        stnNext = stations[test];
        stnPrev = stations[test - 1];
        csi.loc = stnNext.loc;
        csi.X = Comp_LinearInterpolate(AxVal, stnNext.loc, stnPrev.loc, stnNext.X, stnPrev.X);
        csi.Y = Comp_LinearInterpolate(AxVal, stnNext.loc, stnPrev.loc, stnNext.Y, stnPrev.Y);
        csi.Z = Comp_LinearInterpolate(AxVal, stnNext.loc, stnPrev.loc, stnNext.Z, stnPrev.Z);
      }
      else
      { //Off the table to the negative.
        csi.loc = 0.0;
        csi.X = 0.0;
        csi.Y = stations[0].Y;
        csi.Z = stations[0].Z;
      }
    }

    private double Comp_LinearInterpolate(double loc, double X1, double X2, double Y1, double Y2)
    {
      double m;
      double b;

      m = (Y2 - Y1) / (X2 - X1);
      b = Y1 - m * X1;
      return m * loc + b;
    }

    private double RolloverAxisDistance(double AxisPosition)
    {
      if (Math.Abs(AxisPosition) > 180.0)
      {
        if (AxisPosition > 0.0)
          AxisPosition -= 360.0;
        else
          AxisPosition += 360.0;
      }
      return AxisPosition;
    }

    double RolloverAxisPosition(double AxisPosition)
    {
      if (AxisPosition > 360.0)
        return AxisPosition - 360.0;
      else if (AxisPosition < 0.0)
        return AxisPosition + 360.0;
      return AxisPosition;
    }


    public double[] FRCtoBarrel(double x, double y, double z, double u)
    {
      return FRCtoBarrel(new cPoint(0, 0, 0, x, y, z, u));
    }

    public double[] FRCtoBarrel(cPoint Point)
    {
      double[] ret;
      double d2r = Math.PI / 180.0;
      Electroimpact.LinearAlgebra.c6dof iMandrelToSpin = new Electroimpact.LinearAlgebra.c6dof(MandrelToSpin.Inverse());
      Electroimpact.LinearAlgebra.c6dof iSpinToFRC = new Electroimpact.LinearAlgebra.c6dof(SpinToFRC.Inverse());
      Electroimpact.LinearAlgebra.RxMatrix u = new Electroimpact.LinearAlgebra.RxMatrix(-Point.upos * d2r);
      ret = Point.toArray();
      ret = iSpinToFRC.DotMe(Point.toArray());
      ret = u.DotMe(ret);
      ret[0] -= A_x * Math.Sin(Point.upos / 2.0 * d2r);
      ret = iMandrelToSpin.DotMe(ret);
      return ret;
    }
  }
}
