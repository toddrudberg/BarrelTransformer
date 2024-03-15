using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Electroimpact.LinearAlgebra;

namespace Electroimpact
{

  public class cPoint
  {
    const double d2r = Math.PI / 180.0;
    public double x_nom;
    public double y_nom;
    public double z_nom;
    public double x_meas;
    public double y_meas;
    public double z_meas;
    public double upos;
    public double vpos;
    public bool bUseInXform = true;
    public bool readError = false;
    public double errInXform = 0.0;
    public int point_number = 0;

    public cPoint()
    {
      x_nom = 0;
      y_nom = 0;
      z_nom = 0;
      x_meas = 0;
      y_meas = 0;
      z_meas = 0;
      upos = 0;
      vpos = 0;
    }
    /// <summary>
    /// for barrel transform situations.  multi-6dof solutions.
    /// </summary>
    /// <param name="line_in"></param>
    /// <param name="uses_vpos"></param>
    public cPoint(string line_in, bool uses_vpos)
    {
      string[] args = line_in.Split(',');
      int nargs = uses_vpos ? 8 : 7;

      if (args.Length >= nargs)
      {
        try
        {
          int argnum = 0;
          x_nom = double.Parse(args[argnum]); argnum++;
          y_nom = double.Parse(args[argnum]); argnum++;
          z_nom = double.Parse(args[argnum]); argnum++;
          upos = double.Parse(args[argnum]); argnum++;
          if (uses_vpos)
          {
            vpos = double.Parse(args[argnum]); argnum++;
          }
          else
            vpos = 0.0;
          x_meas = double.Parse(args[argnum]); argnum++;
          y_meas = double.Parse(args[argnum]); argnum++;
          z_meas = double.Parse(args[argnum]); argnum++;

        }
        catch
        {
          readError = true;
        }
      }
    }

    /// <summary>
    /// For single rigid body solutions only.
    /// </summary>
    /// <param name="line_in"></param>
    public cPoint(string line_in)
    {
      string[] args = line_in.Split(',');
      int nargs = 6;

      if (args.Length >= nargs)
      {
        try
        {
          int argnum = 0;
          x_nom = double.Parse(args[argnum]); argnum++;
          y_nom = double.Parse(args[argnum]); argnum++;
          z_nom = double.Parse(args[argnum]); argnum++;
          x_meas = double.Parse(args[argnum]); argnum++;
          y_meas = double.Parse(args[argnum]); argnum++;
          z_meas = double.Parse(args[argnum]); argnum++;
          upos = 0.0;
          vpos = 0.0;
        }
        catch
        {
          readError = true;
        }
      }
    }

    public cPoint(double nX, double nY, double nZ, double mX, double mY, double mZ)
    {
      x_nom = nX;
      y_nom = nY;
      z_nom = nZ;
      x_meas = mX;
      y_meas = mY;
      z_meas = mZ;
      upos = 0.0;
      vpos = 0.0;
    }
    public cPoint(double nX, double nY, double nZ, double mX, double mY, double mZ, double Upos)
    {
      x_nom = nX;
      y_nom = nY;
      z_nom = nZ;
      x_meas = mX;
      y_meas = mY;
      z_meas = mZ;
      upos = Upos;
      vpos = 0.0;
    }
    public cPoint(double nX, double nY, double nZ, double mX, double mY, double mZ, double Upos, double Vpos)
    {
      x_nom = nX;
      y_nom = nY;
      z_nom = nZ;
      x_meas = mX;
      y_meas = mY;
      z_meas = mZ;
      upos = Upos;
      vpos = Vpos;
    }
    /// <summary>
    /// measured points to a 4x1 array
    /// </summary>
    /// <returns></returns>
    public double[] toArray()
    {
      double[] dog = new double[4];
      dog[0] = x_meas;
      dog[1] = y_meas;
      dog[2] = z_meas;
      dog[3] = 1.0;
      return dog;
    }

    /// <summary>
    /// nominal tooling points to a 4x1 array
    /// </summary>
    /// <returns></returns>
    public double[] toArrayNominal()//double Ax, double X, double Y, double Z, double rx, double ry, double rz)
    {
      double[] dog = new double[4];
      dog[0] = x_nom;
      dog[1] = y_nom;
      dog[2] = z_nom;
      dog[3] = 1.0;
      return dog;
    }

    public override string ToString()
    {
      string output;
      output = "Point " + point_number.ToString() + ".  Radial Fit: " + errInXform.ToString("F3");
      return output;
    }
  }

  public class cBarrelFunctions
  {
    public Electroimpact.LinearAlgebra.c6dof PartToUaxis;
    public Electroimpact.LinearAlgebra.c6dof VaxisToFRC;
    public Electroimpact.LinearAlgebra.c6dof airplane_to_barrel = new Electroimpact.LinearAlgebra.c6dof();
    public double[] spinVector;
    public double A_x;

    public class cSetupBarrelTransformer
    {
      public double PartToUaxis_X = 0.0;
      public double PartToUaxis_Y = 0.0;
      public double PartToUaxis_Z = 0.0;
      public double PartToUaxis_rX = 0.0;
      public double PartToUaxis_rY = 0.0;
      public double PartToUaxis_rZ = 0.0;

      public double VtoFRC_X = 0.0;
      public double VtoFRC_Y = 0.0;
      public double VtoFRC_Z = 0.0;
      public double VtoFRC_rX = 0.0;
      public double VtoFRC_rY = 0.0;
      public double VtoFRC_rZ = 0.0;

      public double A_x = 0.0;

      public cSetupBarrelTransformer()
      {}

      public void setArgs(cBarrelFunctions bf)
      {
        PartToUaxis_X = bf.PartToUaxis.X;
        PartToUaxis_Y = bf.PartToUaxis.Y;
        PartToUaxis_Z = bf.PartToUaxis.Z;
        PartToUaxis_rX = bf.PartToUaxis.rX.RadiansToDegrees();
        PartToUaxis_rY = bf.PartToUaxis.rY.RadiansToDegrees();
        PartToUaxis_rZ = bf.PartToUaxis.rZ.RadiansToDegrees();
        VtoFRC_X = bf.VaxisToFRC.X;
        VtoFRC_Y = bf.VaxisToFRC.Y;
        VtoFRC_Z = bf.VaxisToFRC.Z;
        VtoFRC_rX = bf.VaxisToFRC.rX.RadiansToDegrees();
        VtoFRC_rY = bf.VaxisToFRC.rY.RadiansToDegrees();
        VtoFRC_rZ = bf.VaxisToFRC.rZ.RadiansToDegrees();

        A_x = bf.A_x;
      }
    }

    public cBarrelFunctions()
    {
      PartToUaxis = new Electroimpact.LinearAlgebra.c6dof();
      VaxisToFRC = new Electroimpact.LinearAlgebra.c6dof();
      A_x = 0;
    }

    /// <summary>
    /// Use this constructor when building from a setup string
    /// </summary>
    /// <param name="Setup"> a .csv sting where we have X,Y,Z,rX,rY,rZ,A_x,X,Y,Z,rX,rY,rZ.  rotaries are in degrees</param>
    public cBarrelFunctions(string Setup)
    {
      PartToUaxis = new Electroimpact.LinearAlgebra.c6dof();
      VaxisToFRC = new Electroimpact.LinearAlgebra.c6dof();

      string[] Lines = Setup.Split(',');
      if (Lines.Length >= 13) //used >= incase of extra string in a csv split.
      {
        double dog = 0.0;
        PartToUaxis.X = double.TryParse(Lines[0], out dog) ? dog : 0.0;
        PartToUaxis.Y = double.TryParse(Lines[1], out dog) ? dog : 0.0;
        PartToUaxis.Z = double.TryParse(Lines[2], out dog) ? dog : 0.0;
        PartToUaxis.rX = double.TryParse(Lines[3], out dog) ? dog.DegreesToRadians() : 0.0;
        PartToUaxis.rY = double.TryParse(Lines[4], out dog) ? dog.DegreesToRadians() : 0.0;
        PartToUaxis.rZ = double.TryParse(Lines[5], out dog) ? dog.DegreesToRadians() : 0.0;
        A_x = double.TryParse(Lines[6], out A_x) ? A_x : 0.0;
        VaxisToFRC.X = double.TryParse(Lines[7], out dog) ? dog : 0.0;
        VaxisToFRC.Y = double.TryParse(Lines[8], out dog) ? dog : 0.0;
        VaxisToFRC.Z = double.TryParse(Lines[9], out dog) ? dog : 0.0;
        VaxisToFRC.rX = double.TryParse(Lines[10], out dog) ? dog.DegreesToRadians() : 0.0;
        VaxisToFRC.rY = double.TryParse(Lines[11], out dog) ? dog.DegreesToRadians() : 0.0;
        VaxisToFRC.rZ = double.TryParse(Lines[12], out dog) ? dog.DegreesToRadians() : 0.0;
      }
    }
    /// <summary>
    /// Use this constructor when building from cnc mamory and you only one a single rigid body transform.
    /// </summary>
    /// <param name="Setup"> an arrary where we have X,Y,Z,rX,rY,rZ,A_x,X,Y,Z,rX,rY,rZ.  rotaries are in degrees</param>
    public cBarrelFunctions(int[] arguments)
    {
      //arguments inside the cnc are in mm, so need to convert to inch.
      PartToUaxis = new Electroimpact.LinearAlgebra.c6dof();
      VaxisToFRC = new Electroimpact.LinearAlgebra.c6dof();
      double mult_offset = 1.0e-3 / 25.4;
      double mult_angular = (1.0e-6).DegreesToRadians();
      if (arguments.Length >= 6)
      {
        PartToUaxis.X = (double)arguments[0] * mult_offset;
        PartToUaxis.Y = (double)arguments[1] * mult_offset;
        PartToUaxis.Z = (double)arguments[2] * mult_offset;
        PartToUaxis.rX = (double)arguments[3] * mult_angular;
        PartToUaxis.rY = (double)arguments[4] * mult_angular;
        PartToUaxis.rZ = (double)arguments[5] * mult_angular;
        A_x = 0.0;
      }      
    }



    /// <summary>
    /// Use this constructor when building from cnc mamory and you want a full barrel transform.
    /// </summary>
    /// <param name="Setup"> an arrary where we have X,Y,Z,rX,rY,rZ,A_x,X,Y,Z,rX,rY,rZ.  rotaries are in degrees</param>
    public cBarrelFunctions(int[] man_to_spin, int[] spin_to_frc, int a_x)
    {
      PartToUaxis = new Electroimpact.LinearAlgebra.c6dof();
      VaxisToFRC = new Electroimpact.LinearAlgebra.c6dof();
      double mult_offset = 1.0e-3 / 25.4;
      double mult_angular = (1.0e-6).DegreesToRadians(); //Siemens needs to be 1.0e-3

      if (man_to_spin.Length >= 6)
      {
        PartToUaxis.MakeMatrix_rZrYrX((double)man_to_spin[0] * mult_offset, (double)man_to_spin[1] * mult_offset, (double)man_to_spin[2] * mult_offset,
          (double)man_to_spin[3] * mult_angular, (double)man_to_spin[4] * mult_angular, (double)man_to_spin[5] * mult_angular);
      }
      if (spin_to_frc.Length >= 6)
      {
        VaxisToFRC.MakeMatrix_rZrYrX((double)spin_to_frc[0] * mult_offset, (double)spin_to_frc[1] * mult_offset, (double)spin_to_frc[2] * mult_offset,
        (double)spin_to_frc[3] * mult_angular, (double)spin_to_frc[4] * mult_angular, (double)spin_to_frc[5] * mult_angular);
      }
      A_x = a_x * mult_offset;
    }

    /// <summary>
    /// Use this constructor when building from cnc mamory and you want a full barrel transform.
    /// </summary>
    /// <param name="Setup"> an arrary where we have X,Y,Z,rX,rY,rZ,A_x,X,Y,Z,rX,rY,rZ.  rotaries are in degrees</param>
    public cBarrelFunctions(int[] man_to_spin, int[] spin_to_frc, int a_x, double[] vector)
    {
      spinVector = vector;
      PartToUaxis = new Electroimpact.LinearAlgebra.c6dof();
      VaxisToFRC = new Electroimpact.LinearAlgebra.c6dof();
      double mult_offset = 1.0e-3 / 25.4;
      double mult_angular = (1.0e-6).DegreesToRadians(); //Siemens needs to be 1.0e-3

      if (man_to_spin.Length >= 6)
      {
        PartToUaxis.MakeMatrix_rZrYrX((double)man_to_spin[0] * mult_offset, (double)man_to_spin[1] * mult_offset, (double)man_to_spin[2] * mult_offset,
          (double)man_to_spin[3] * mult_angular, (double)man_to_spin[4] * mult_angular, (double)man_to_spin[5] * mult_angular);
      }
      if (spin_to_frc.Length >= 6)
      {
        VaxisToFRC.MakeMatrix_rZrYrX((double)spin_to_frc[0] * mult_offset, (double)spin_to_frc[1] * mult_offset, (double)spin_to_frc[2] * mult_offset,
        (double)spin_to_frc[3] * mult_angular, (double)spin_to_frc[4] * mult_angular, (double)spin_to_frc[5] * mult_angular);
      }
      A_x = a_x * mult_offset;
    }

    /// <summary>
    /// Use this constructor when building from cnc mamory and you want a full barrel transform plus a tool to mandrel transform.
    /// </summary>
    /// <param name="Setup"> an arrary where we have X,Y,Z,rX,rY,rZ,A_x,X,Y,Z,rX,rY,rZ.  rotaries are in degrees</param>
    public cBarrelFunctions(int[] tool_to_mandrel, int[] man_to_spin, int[] spin_to_frc, int a_x)
    {
      PartToUaxis = new Electroimpact.LinearAlgebra.c6dof();
      VaxisToFRC = new Electroimpact.LinearAlgebra.c6dof();
      Electroimpact.LinearAlgebra.c6dof ToolToMandrel = new LinearAlgebra.c6dof();
      double mult_offset = 1.0e-3 / 25.4;
      double mult_angular = (1.0e-6).DegreesToRadians();

      if (tool_to_mandrel.Length >= 6)
      {
        ToolToMandrel.X = (double)tool_to_mandrel[0] * mult_offset;
        ToolToMandrel.Y = (double)tool_to_mandrel[1] * mult_offset;
        ToolToMandrel.Z = (double)tool_to_mandrel[2] * mult_offset;
        ToolToMandrel.rX = (double)tool_to_mandrel[3] * mult_angular;
        ToolToMandrel.rY = (double)tool_to_mandrel[4] * mult_angular;
        ToolToMandrel.rZ = (double)tool_to_mandrel[5] * mult_angular;
      }
      if (man_to_spin.Length >= 6)
      {
        PartToUaxis.X = (double)man_to_spin[0] * mult_offset;
        PartToUaxis.Y = (double)man_to_spin[1] * mult_offset;
        PartToUaxis.Z = (double)man_to_spin[2] * mult_offset;
        PartToUaxis.rX = (double)man_to_spin[3] * mult_angular;
        PartToUaxis.rY = (double)man_to_spin[4] * mult_angular;
        PartToUaxis.rZ = (double)man_to_spin[5] * mult_angular;
      }
      //MandrelToSpin = new LinearAlgebra.c6dof(ToolToMandrel.DotMe(MandrelToSpin.GetMatrix()));
      PartToUaxis = new LinearAlgebra.c6dof(PartToUaxis.DotMe(ToolToMandrel.GetMatrix()));

      if (spin_to_frc.Length >= 6)
      {
        VaxisToFRC.X = (double)spin_to_frc[0] * mult_offset;
        VaxisToFRC.Y = (double)spin_to_frc[1] * mult_offset;
        VaxisToFRC.Z = (double)spin_to_frc[2] * mult_offset;
        VaxisToFRC.rX = (double)spin_to_frc[3] * mult_angular;
        VaxisToFRC.rY = (double)spin_to_frc[4] * mult_angular;
        VaxisToFRC.rZ = (double)spin_to_frc[5] * mult_angular;
      }
      A_x = a_x * mult_offset;
    }

    public cBarrelFunctions(Electroimpact.LinearAlgebra.c6dof mandreltospin, double a_x, Electroimpact.LinearAlgebra.c6dof spintofrc)
    {
      PartToUaxis = mandreltospin;
      VaxisToFRC = spintofrc;
      A_x = a_x;
    }

    public cBarrelFunctions(cSetupBarrelTransformer SetupBarrelTransformer)
    {
      VaxisToFRC = new c6dof();
      PartToUaxis = new c6dof();
      VaxisToFRC.X = SetupBarrelTransformer.VtoFRC_X;
      VaxisToFRC.Y = SetupBarrelTransformer.VtoFRC_Y;
      VaxisToFRC.Z = SetupBarrelTransformer.VtoFRC_Z;
      VaxisToFRC.rX = SetupBarrelTransformer.VtoFRC_rX.DegreesToRadians();
      VaxisToFRC.rY = SetupBarrelTransformer.VtoFRC_rY.DegreesToRadians();
      VaxisToFRC.rZ = SetupBarrelTransformer.VtoFRC_rZ.DegreesToRadians();

      PartToUaxis.X = SetupBarrelTransformer.PartToUaxis_X;
      PartToUaxis.Y = SetupBarrelTransformer.PartToUaxis_Y;
      PartToUaxis.Z = SetupBarrelTransformer.PartToUaxis_Z;
      PartToUaxis.rX = SetupBarrelTransformer.PartToUaxis_rX.DegreesToRadians();
      PartToUaxis.rY = SetupBarrelTransformer.PartToUaxis_rY.DegreesToRadians();
      PartToUaxis.rZ = SetupBarrelTransformer.PartToUaxis_rZ.DegreesToRadians();

      A_x = SetupBarrelTransformer.A_x;

    }
    /// <summary>
    /// Paremeters are degrees
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <param name="rX"></param>
    /// <param name="rY"></param>
    /// <param name="rZ"></param>
    public void SetAirplaneToBarrel(double x, double y, double z, double rX, double rY, double rZ)
    {
      Electroimpact.LinearAlgebra.RrMatrix T1 = new Electroimpact.LinearAlgebra.RrMatrix(x, y, z);
      Electroimpact.LinearAlgebra.c6dof rotations = new Electroimpact.LinearAlgebra.c6dof(0, 0, 0, -rX.DegreesToRadians(), rY.DegreesToRadians(), rZ.DegreesToRadians());
      airplane_to_barrel = new Electroimpact.LinearAlgebra.c6dof(rotations.DotMe(T1.GetMatrix));
    }

    public double[] AirplaneToBarrel(double x, double y, double z)
    {
      Electroimpact.LinearAlgebra.RrMatrix rr = new Electroimpact.LinearAlgebra.RrMatrix(x, y, z);
      double[,] ret = airplane_to_barrel.DotMe(rr.GetMatrix);
      double[] dang = { ret[0, 3], ret[1, 3], ret[2, 3], 1 };
      return dang;
    }

    /// <summary>
    /// returns the coordinate in mandrel or "barrel" coordinates.
    /// angular arguments are in degrees
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
      Electroimpact.LinearAlgebra.c6dof m = new LinearAlgebra.c6dof(x, y, z, rx.DegreesToRadians(), ry.DegreesToRadians(), rz.DegreesToRadians());
      m = new LinearAlgebra.c6dof(airplane_to_barrel.DotMe(m.GetMatrix()));
      return m;
    }

    public double[] BarrelToAirplane(double x, double y, double z)
    {
      Electroimpact.LinearAlgebra.c6dof BtoA = new Electroimpact.LinearAlgebra.c6dof(airplane_to_barrel.Inverse());
      Electroimpact.LinearAlgebra.RrMatrix rr = new Electroimpact.LinearAlgebra.RrMatrix(x, y, z);
      double[,] ret = BtoA.DotMe(rr.GetMatrix);
      double[] dang = {ret[0,3],ret[1,3],ret[2,3], 1};
      return dang;
    }

    public void ClearTransform()
    {
      PartToUaxis.GetMatrix(0, 0, 0, 0, 0, 0);
      VaxisToFRC.GetMatrix(0, 0, 0, 0, 0, 0);
      SetAirplaneToBarrel(0, 0, 0, 0, 0, 0);
      A_x = 0;
    }

    /// <summary>
    /// Part Coordinates to FRC.  U and V in degrees.  V is optional, use only if you have the UV rotator.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <param name="u"></param>
    /// <param name="v"></param>
    /// <returns></returns>
    public double[] BarrelToFRC(double x, double y, double z, double u, double v)
    {
      return BarrelToFRC(new cPoint(x, y, z, 0, 0, 0, u, v));
    }

    public double[] BarrelToFRC(double x, double y, double z, double u)
    {
      return BarrelToFRC(new cPoint(x, y, z, 0, 0, 0, u, 0.0));
    }

    public double[] BarrelToFRC(cPoint Point)
    {
      double[] ret;

      c6dof u = new c6dof(0, 0, 0, Point.upos.DegreesToRadians(), 0, 0);
      c6dof v = new c6dof(0, 0, 0, 0, Point.vpos.DegreesToRadians(), 0);
      ret = Point.toArrayNominal();
      ret = PartToUaxis.DotMe(ret); // where is the point relative to the spin axis?
      ret[0] += A_x * Math.Sin(Point.upos.DegreesToRadians() / 2.0);  // stupid simple X function
      ret = u.DotMe(ret);                                // spin the part
      ret = v.DotMe(ret); 
      ret = VaxisToFRC.DotMe(ret);                        // Where is the spun part relative to the FRC.
      return ret;
    }

    public double[] BarrelToFRC_AboutY(cPoint Point)
    {
      double[] ret;

      //c6dof u = new c6dof(0, 0, 0, 0, Point.upos.DegreesToRadians(), 0);
      c6dof v = new c6dof(0, 0, 0, 0, Point.upos.DegreesToRadians(), 0);
      ret = Point.toArrayNominal();
      ret = PartToUaxis.DotMe(ret); // where is the point relative to the spin axis?
      ret[0] += A_x * Math.Sin(Point.upos.DegreesToRadians() / 2.0);  // stupid simple X function
     //. ret = u.DotMe(ret);                                // spin the part
      ret = v.DotMe(ret);
      ret = VaxisToFRC.DotMe(ret);                        // Where is the spun part relative to the FRC.
      return ret;
    }


    /// <summary>
    /// Part coortinate to FRC, vpos in degrees.
    /// </summary>
    /// <param name="Point"></param>
    /// <param name="vpos"></param>
    /// <returns></returns>
    public Electroimpact.LinearAlgebra.c6dof BarrelToFRC_AboutY(double[,] point, double vpos)
    {
      //no Vpos, call U a rotation about Y for dumbasses who like to rotate about the part Y axis

      c6dof Point = new c6dof(point);
      //RxMatrix u = new RxMatrix(upos.DegreesToRadians());
      RyMatrix v = new RyMatrix(vpos.DegreesToRadians());

      // where is the point relative to the spin axis
      Electroimpact.LinearAlgebra.c6dof ret = new Electroimpact.LinearAlgebra.c6dof(PartToUaxis.DotMe(Point.GetMatrix()));
      //ret.X += A_x * Math.Sin(upos.DegreesToRadians() / 2.0);                                              // stupid simple X function
      //ret = new LinearAlgebra.c6dof(u.DotMe(ret.GetMatrix()));                               // spin the part
      ret = new c6dof(v.DotMe(ret.GetMatrix()));
      ret = new LinearAlgebra.c6dof(VaxisToFRC.DotMe(ret.GetMatrix()));                        // Where is the spun part relative to the FRC.
      return ret;
    }

    /// <summary>
    /// Part coortinate to FRC, upos and vpos in degrees.
    /// </summary>
    /// <param name="Point"></param>
    /// <param name="upos"></param>
    /// <param name="vpos"></param>
    /// <returns></returns>
    public Electroimpact.LinearAlgebra.c6dof BarrelToFRC(double[,] point, double upos, double vpos)
    {
      c6dof Point = new c6dof(point);
      RxMatrix u = new RxMatrix(upos.DegreesToRadians());
      RyMatrix v = new RyMatrix(vpos.DegreesToRadians());

      // where is the point relative to the spin axis?
      Electroimpact.LinearAlgebra.c6dof ret = new Electroimpact.LinearAlgebra.c6dof(PartToUaxis.DotMe(Point.GetMatrix()));
      ret.X += A_x * Math.Sin(upos.DegreesToRadians() / 2.0);                                              // stupid simple X function
      ret = new LinearAlgebra.c6dof(u.DotMe(ret.GetMatrix()));                               // spin the part
      ret = new LinearAlgebra.c6dof(v.DotMe(ret.GetMatrix()));
      ret = new LinearAlgebra.c6dof(VaxisToFRC.DotMe(ret.GetMatrix()));                        // Where is the spun part relative to the FRC.
      return ret;
    }

    public Electroimpact.LinearAlgebra.c6dof BarrelToFRC(double[,] Point, double upos)
    {
      return BarrelToFRC(Point, upos, 0.0);
    }

    public bool spinRX()
    {
        if (this.spinVector != null)
        {
            if (spinVector[0] > spinVector[1] && spinVector[0] > spinVector[2])
                return true;
        }
        return false;
    }
    public bool spinRY()
    {
        if (this.spinVector != null)
        {
            if (spinVector[1] > spinVector[0] && spinVector[1] > spinVector[2])
                return true;
        }
        return false;
    }

    /// <summary>
    /// returns a value in degrees representing the correct A
    /// </summary>
    /// <param name="point"></param>
    /// <param name="distanceFromU0toA0"></param>
    /// <returns></returns>
    public double ConvertAtoPurerX(Electroimpact.LinearAlgebra.c6dof point, double distanceFromU0toA0 = -90.0)
    {
      double thetaTodd = Theta(point.X, point.Y, point.Z, distanceFromU0toA0);
      return RolloverAxisDistance(point.rX.RadiansToDegrees() + thetaTodd);
    }

    public double ConvertAtoPurerX(Electroimpact.LinearAlgebra.c6dof point, double distanceFromU0toA0, List<cStation> stns)
    {
      double thetaTodd = Theta(point.X, point.Y, point.Z, distanceFromU0toA0, stns);
      return RolloverAxisDistance(point.rX.RadiansToDegrees() + thetaTodd);
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
        alpha = Math.Asin(z / R).RadiansToDegrees();
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

    public double[,] FRCtoBarrel(double[,] point, double Upos, double Vpos)
    {
      Electroimpact.LinearAlgebra.c6dof iMandrelToSpin = new Electroimpact.LinearAlgebra.c6dof(PartToUaxis.Inverse());
      Electroimpact.LinearAlgebra.c6dof iSpinToFRC = new Electroimpact.LinearAlgebra.c6dof(VaxisToFRC.Inverse());
      Electroimpact.LinearAlgebra.RxMatrix u = new Electroimpact.LinearAlgebra.RxMatrix(-Upos.DegreesToRadians());
      RyMatrix v = new RyMatrix(-Vpos.DegreesToRadians());
      double[,] result = iSpinToFRC.DotMe(point);
      result = v.DotMe(result);
      result = u.DotMe(result);
      result[0,3] -= A_x * Math.Sin(Upos.DegreesToRadians() / 2.0);
      result = iMandrelToSpin.DotMe(result);
      return result;
    }
    public double[,] FRCtoBarrel(double[,] point, double Upos)
    {
      return FRCtoBarrel(point, Upos, 0.0);
    }

    public double[] FRCtoBarrel(double x, double y, double z, double u, double v)
    {
      return FRCtoBarrel(new cPoint(0, 0, 0, x, y, z, u, v));
    }

    public double[] FRCtoBarrel(double x, double y, double z, double u)
    {
      return FRCtoBarrel(x, y, z, u, 0.0);
    }

    public double[] FRCtoBarrel(cPoint Point)
    {
      double[] ret;

      Electroimpact.LinearAlgebra.c6dof iMandrelToSpin = new Electroimpact.LinearAlgebra.c6dof(PartToUaxis.Inverse());
      Electroimpact.LinearAlgebra.c6dof iSpinToFRC = new Electroimpact.LinearAlgebra.c6dof(VaxisToFRC.Inverse());
      Electroimpact.LinearAlgebra.RxMatrix u = new Electroimpact.LinearAlgebra.RxMatrix(-Point.upos.DegreesToRadians());
      Electroimpact.LinearAlgebra.RyMatrix v = new Electroimpact.LinearAlgebra.RyMatrix(-Point.vpos.DegreesToRadians());
      ret = Point.toArray();
      ret = iSpinToFRC.DotMe(Point.toArray());
      ret = v.DotMe(ret);
      ret = u.DotMe(ret);
      ret[0] -= A_x * Math.Sin(Point.upos.DegreesToRadians() / 2.0);
      ret = iMandrelToSpin.DotMe(ret);
      return ret;
    }

    public double[] FRCtoBarrel_AboutY(cPoint Point)
    {
      double[] ret;

      Electroimpact.LinearAlgebra.c6dof iMandrelToSpin = new Electroimpact.LinearAlgebra.c6dof(PartToUaxis.Inverse());
      Electroimpact.LinearAlgebra.c6dof iSpinToFRC = new Electroimpact.LinearAlgebra.c6dof(VaxisToFRC.Inverse());
      //Electroimpact.LinearAlgebra.RxMatrix u = new Electroimpact.LinearAlgebra.RxMatrix(-Point.upos.DegreesToRadians());
      Electroimpact.LinearAlgebra.RyMatrix v = new Electroimpact.LinearAlgebra.RyMatrix(-Point.upos.DegreesToRadians());
      ret = Point.toArray();
      ret = iSpinToFRC.DotMe(Point.toArray());
      ret = v.DotMe(ret);
     // ret = u.DotMe(ret);
      //ret[0] -= A_x * Math.Sin(Point.upos.DegreesToRadians() / 2.0);
      ret = iMandrelToSpin.DotMe(ret);
      return ret;
    }

    public double[,] FRCtoBarrel_AboutY(double[,] point, double Vpos)
    {
      Electroimpact.LinearAlgebra.c6dof iMandrelToSpin = new Electroimpact.LinearAlgebra.c6dof(PartToUaxis.Inverse());
      Electroimpact.LinearAlgebra.c6dof iSpinToFRC = new Electroimpact.LinearAlgebra.c6dof(VaxisToFRC.Inverse());
      Electroimpact.LinearAlgebra.RyMatrix v = new Electroimpact.LinearAlgebra.RyMatrix(-Vpos.DegreesToRadians());
      //RyMatrix v = new RyMatrix(-Vpos.DegreesToRadians());
      double[,] result = iSpinToFRC.DotMe(point);
      result = v.DotMe(result);
      //result = u.DotMe(result);
      //result[0, 3] -= A_x * Math.Sin(Upos.DegreesToRadians() / 2.0);
      result = iMandrelToSpin.DotMe(result);
      return result;
    }


  }
}
