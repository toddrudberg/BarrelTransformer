using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using SolverPlatform;
using Electroimpact;

namespace Transformer
{
  public partial class frmBarrelTransformer : Form
  {
    const int AnlgePower = 6;
    const int OffsetPower = 3;
    const int MatrixPower = 6;
    double AnglePrecision = 1.0 * Math.Pow(10,(double)AnlgePower);
    double OffsetPrecision = 1.0 * Math.Pow(10, (double)OffsetPower);
    double MatrixPrecision = 1.0 * Math.Pow(10, (double)MatrixPower);
    double[,] EulerMatrix = new double[4, 4];

    System.Collections.Generic.List<Electroimpact.cPoint> myPoints = new List<cPoint>();
    int Count = 0;
    //double A_x = 0.0;
    //Electroimpact.LinearAlgebra.c6dof myBarrelFunction.MandrelToSpin = new Electroimpact.LinearAlgebra.c6dof();
    //Electroimpact.LinearAlgebra.c6dof myBarrelFunction.SpinToFRC = new Electroimpact.LinearAlgebra.c6dof();

    Electroimpact.cBarrelFunctions myBarrelFunction;

    private void ReadInPoints(string FileName)
    {
      Electroimpact.FileIO.cFileReader fr = new Electroimpact.FileIO.cFileReader();
      fr.OpenFile(FileName);
      fr.ReadLine();//throw out the first line.
      while (fr.Peek())
      {
        string line = fr.ReadLine();
        string[] data = line.Split(',');
        myPoints.Add(new cPoint(double.Parse(data[0]), double.Parse(data[1]), double.Parse(data[2]),
                        double.Parse(data[4]), double.Parse(data[5]), double.Parse(data[6]), double.Parse(data[3])));

      }
      fr.CloseFile();
    }

    public frmBarrelTransformer()
    {
      InitializeComponent();
      myBarrelFunction = new cBarrelFunctions();
    }
      //Transform
    bool b_FirstTimeThru = true;
    private void btnTransform_Click(object sender, EventArgs e)
    {
      double d2r = Math.PI / 180.0;

      txtEulerCheck.Clear();
      this.Refresh();
      System.Threading.Thread.Sleep(50);

      if (b_FirstTimeThru)
      {
        myPoints.Clear();
        ReadInPoints("points.csv");
      }
      b_FirstTimeThru = false;
      string[] lines = new string[6];

      myBarrelFunction.ClearTransform();

      //SolveMeOriginal();
      //SolveMe();
      SolveMeII();

      lines = new string[13];
      lines[0] = myBarrelFunction.MandrelToSpin.X.ToString("F6").PadLeft(14);
      lines[1] = myBarrelFunction.MandrelToSpin.Y.ToString("F6").PadLeft(14);
      lines[2] = myBarrelFunction.MandrelToSpin.Z.ToString("F6").PadLeft(14);
      lines[3] = (myBarrelFunction.MandrelToSpin.rX / d2r).ToString("F6").PadLeft(14);
      lines[4] = (myBarrelFunction.MandrelToSpin.rY / d2r).ToString("F6").PadLeft(14);
      lines[5] = (myBarrelFunction.MandrelToSpin.rZ / d2r).ToString("F6").PadLeft(14);
      lines[6] = "A_x: " + myBarrelFunction.A_x.ToString("F6").PadLeft(14);
      lines[7] = myBarrelFunction.SpinToFRC.X.ToString("F6").PadLeft(14);
      lines[8] = myBarrelFunction.SpinToFRC.Y.ToString("F6").PadLeft(14);
      lines[9] = myBarrelFunction.SpinToFRC.Z.ToString("F6").PadLeft(14);
      lines[10] = (myBarrelFunction.SpinToFRC.rX / d2r).ToString("F6").PadLeft(14);
      lines[11] = (myBarrelFunction.SpinToFRC.rY / d2r).ToString("F6").PadLeft(14);
      lines[12] = (myBarrelFunction.SpinToFRC.rZ / d2r).ToString("F6").PadLeft(14);
      textBox2.Lines = lines;

      double[,] data2 = myBarrelFunction.MandrelToSpin.Inverse();

      lines = new string[3];
      lines[0] = data2[0, 0].ToString("F6").PadLeft(9) + " | " + data2[0, 1].ToString("F6").PadLeft(9) + " | " + data2[0, 2].ToString("F6").PadLeft(9);
      lines[1] = data2[1, 0].ToString("F6").PadLeft(9) + " | " + data2[1, 1].ToString("F6").PadLeft(9) + " | " + data2[1, 2].ToString("F6").PadLeft(9);
      lines[2] = data2[2, 0].ToString("F6").PadLeft(9) + " | " + data2[2, 1].ToString("F6").PadLeft(9) + " | " + data2[2, 2].ToString("F6").PadLeft(9);
      textBox5.Lines = lines;
      #region Output The Results

      Electroimpact.FileIO.cFileWriter fw = new Electroimpact.FileIO.cFileWriter();

      if( !Electroimpact.FileIO.cFileOther.FileExistsMethod("Transforms.csv" ))
      {
        fw.WriteLine("Transforms.csv", "MtoS.X,MtoS.Y,MtoS.Z,MtoS.rX,MtoS.rY,MtoS.rZ,A_x,StoFRC.X,StoFRC.Y,StoFRC.Z,StoFRC.rX,StoFRC.rY,StoFRC.rZ", true);
      }
      fw.WriteLine("Transforms.csv",
                                    myBarrelFunction.MandrelToSpin.X.ToString("F3") + "," +
                                    myBarrelFunction.MandrelToSpin.Y.ToString("F3") + "," +
                                    myBarrelFunction.MandrelToSpin.Z.ToString("F3") + "," +
                                    (myBarrelFunction.MandrelToSpin.rX / d2r).ToString("F6") + "," +
                                    (myBarrelFunction.MandrelToSpin.rY / d2r).ToString("F6") + "," +
                                    (myBarrelFunction.MandrelToSpin.rZ / d2r).ToString("F6") + "," +
                                    myBarrelFunction.A_x.ToString("F6") + "," +
                                    myBarrelFunction.SpinToFRC.X.ToString("F3") + "," +
                                    myBarrelFunction.SpinToFRC.Y.ToString("F3") + "," +
                                    myBarrelFunction.SpinToFRC.Z.ToString("F3") + "," +
                                    (myBarrelFunction.SpinToFRC.rX / d2r).ToString("F6") + "," +
                                    (myBarrelFunction.SpinToFRC.rY / d2r).ToString("F6") + "," +
                                    (myBarrelFunction.SpinToFRC.rZ / d2r).ToString("F6") + ","
                                    , true);
      fw.CloseFile();
      #endregion
      btnCheckEuler_Click(sender, e);
    }

    private void SolveMe()
    {
      double d2r = Math.PI / 180.0;
      double stepsize = 1.0e-9;

      //Count = 3;
      //stepsize = 1.0e-9;
      //using (Problem p = new Problem(Solver_Type.Minimize, Count, 1))
      //{
      //  #region Problem Setup
      //  p.Evaluators[Eval_Type.Iteration].OnEvaluate += new EvaluateEventHandler(frmMain_OnEvaluate);
      //  p.Evaluators[Eval_Type.Function].OnEvaluate += new EvaluateEventHandler(Evaluator_Evaluate);

      //  p.VarDecision.InitialValue[0] = MandrelToSpin.rX / d2r;
      //  p.VarDecision.InitialValue[1] = MandrelToSpin.rY / d2r;
      //  p.VarDecision.InitialValue[2] = MandrelToSpin.rZ / d2r;

      //  p.Engine = p.Engines[Engine.GRGName];
      //  p.ProblemType = Problem_Type.OptLP;
      //  p.Engine.Params["MaxTime"].Value = 120;
      //  Console.WriteLine(p.Engine.Params["MaxTime"].Value.ToString("F10"));
      //  p.Engine.Params["SearchOption"].Value = 1;
      //  Console.WriteLine("SearchOption " + p.Engine.Params["SearchOption"].Value.ToString());
      //  p.Engine.Params["Derivatives"].Value = 1;
      //  Console.WriteLine("Derivatives " + p.Engine.Params["Derivatives"].Value.ToString());
      //  p.Engine.Params["Estimates"].Value = 1;
      //  Console.WriteLine("Estimates " + p.Engine.Params["Estimates"].Value.ToString());
      //  p.Engine.Params["stepsize"].Value = stepsize; // double.Parse(cmbStepSize.Text);
      //  Console.WriteLine("stepsize " + p.Engine.Params["stepsize"].Value.ToString("F10"));
      //  p.Engine.Params["Iterations"].Value = 100;
      //  Console.WriteLine("Iterations " + p.Engine.Params["Iterations"].Value.ToString());
      //  p.Engine.Params["Precision"].Value = 0.000001;
      //  Console.WriteLine("Precision " + p.Engine.Params["Precision"].Value.ToString("F10"));
      //  p.Engine.Params["Scaling"].Value = 0;
      //  Console.WriteLine(p.Engine.Params["Scaling"].Value.ToString());
      //  Console.WriteLine();
      //  Console.WriteLine("Solving this many points: " + myPoints.Count.ToString());
      //  //p.Engine.Params["convergance"].Value = .0001;// 10e-4 to 10e-9 affects how long it will chew on a problem.  Bigger the number, the less it will chew.
      //  //p.Engine.Params["multistart"].Value = 1;
      //  //p.Engine.Params["stepsize"].Value = .0001; //minimum guess size.
      //  //p.Engine.Params["Derivatives"].Value = 2;

      //  p.Solver.Optimize();
      //  p.Dispose();
      //  #endregion
      //}

      //System.Threading.Thread.Sleep(500);

      Count = 10;
      using (Problem p = new Problem(Solver_Type.Minimize, Count, 1))
      {
        #region Problem Setup
        p.Evaluators[Eval_Type.Iteration].OnEvaluate += new EvaluateEventHandler(frmMain_OnEvaluate);
        p.Evaluators[Eval_Type.Function].OnEvaluate += new EvaluateEventHandler(Evaluator_Evaluate);

        p.VarDecision.InitialValue[0] = myBarrelFunction.MandrelToSpin.rX / d2r;
        p.VarDecision.InitialValue[1] = myBarrelFunction.MandrelToSpin.rY / d2r;
        p.VarDecision.InitialValue[2] = myBarrelFunction.MandrelToSpin.rZ / d2r;
        p.VarDecision.InitialValue[3] = myBarrelFunction.A_x;
        p.VarDecision.InitialValue[4] = myBarrelFunction.SpinToFRC.X;
        p.VarDecision.InitialValue[5] = myBarrelFunction.SpinToFRC.Y;
        p.VarDecision.InitialValue[6] = myBarrelFunction.SpinToFRC.Z;
        p.VarDecision.InitialValue[7] = myBarrelFunction.SpinToFRC.rX / d2r;
        p.VarDecision.InitialValue[8] = myBarrelFunction.SpinToFRC.rY / d2r;
        p.VarDecision.InitialValue[9] = myBarrelFunction.SpinToFRC.rZ / d2r;

        p.Engine = p.Engines[Engine.GRGName];
        p.ProblemType = Problem_Type.OptLP;
        p.Engine.Params["MaxTime"].Value = 120;
        Console.WriteLine(p.Engine.Params["MaxTime"].Value.ToString("F10"));
        p.Engine.Params["SearchOption"].Value = 1;
        Console.WriteLine("SearchOption " + p.Engine.Params["SearchOption"].Value.ToString());
        p.Engine.Params["Derivatives"].Value = 1;
        Console.WriteLine("Derivatives " + p.Engine.Params["Derivatives"].Value.ToString());
        p.Engine.Params["Estimates"].Value = 1;
        Console.WriteLine("Estimates " + p.Engine.Params["Estimates"].Value.ToString());
        stepsize = 1.0e-7;
        p.Engine.Params["stepsize"].Value = stepsize; // double.Parse(cmbStepSize.Text);
        Console.WriteLine("stepsize " + p.Engine.Params["stepsize"].Value.ToString("F10"));
        p.Engine.Params["Iterations"].Value = 100;
        Console.WriteLine("Iterations " + p.Engine.Params["Iterations"].Value.ToString());
        p.Engine.Params["Precision"].Value = 0.000001;
        Console.WriteLine("Precision " + p.Engine.Params["Precision"].Value.ToString("F10"));
        p.Engine.Params["Scaling"].Value = 0;
        Console.WriteLine(p.Engine.Params["Scaling"].Value.ToString());
        Console.WriteLine();
        Console.WriteLine("Solving this many points: " + myPoints.Count.ToString());
        //p.Engine.Params["convergance"].Value = .0001;// 10e-4 to 10e-9 affects how long it will chew on a problem.  Bigger the number, the less it will chew.
        //p.Engine.Params["multistart"].Value = 1;
        //p.Engine.Params["stepsize"].Value = .0001; //minimum guess size.
        //p.Engine.Params["Derivatives"].Value = 2;

        p.Solver.Optimize();
        p.Dispose();
        #endregion
      }

      //Count = 9;
      //stepsize = 1.0e-9;
      //using (Problem p = new Problem(Solver_Type.Minimize, Count, 1))
      //{
      //  #region Problem Setup
      //  p.Evaluators[Eval_Type.Iteration].OnEvaluate += new EvaluateEventHandler(frmMain_OnEvaluate);
      //  p.Evaluators[Eval_Type.Function].OnEvaluate += new EvaluateEventHandler(Evaluator_Evaluate);

      //  p.VarDecision.InitialValue[0] = MandrelToSpin.rX / d2r;
      //  p.VarDecision.InitialValue[1] = MandrelToSpin.rY / d2r;
      //  p.VarDecision.InitialValue[2] = MandrelToSpin.rZ / d2r;
      //  p.VarDecision.InitialValue[3] = SpinToFRC.X;
      //  p.VarDecision.InitialValue[4] = SpinToFRC.Y;
      //  p.VarDecision.InitialValue[5] = SpinToFRC.Z;
      //  p.VarDecision.InitialValue[6] = SpinToFRC.rX;
      //  p.VarDecision.InitialValue[7] = SpinToFRC.rY;
      //  p.VarDecision.InitialValue[8] = SpinToFRC.rZ;
      //  p.Engine = p.Engines[Engine.GRGName];
      //  p.ProblemType = Problem_Type.OptLP;
      //  p.Engine.Params["MaxTime"].Value = 120;
      //  Console.WriteLine(p.Engine.Params["MaxTime"].Value.ToString("F10"));
      //  p.Engine.Params["SearchOption"].Value = 1;
      //  Console.WriteLine("SearchOption " + p.Engine.Params["SearchOption"].Value.ToString());
      //  p.Engine.Params["Derivatives"].Value = 1;
      //  Console.WriteLine("Derivatives " + p.Engine.Params["Derivatives"].Value.ToString());
      //  p.Engine.Params["Estimates"].Value = 1;
      //  Console.WriteLine("Estimates " + p.Engine.Params["Estimates"].Value.ToString());
      //  stepsize = 1.0e-7;
      //  p.Engine.Params["stepsize"].Value = stepsize; // double.Parse(cmbStepSize.Text);
      //  Console.WriteLine("stepsize " + p.Engine.Params["stepsize"].Value.ToString("F10"));
      //  p.Engine.Params["Iterations"].Value = 100;
      //  Console.WriteLine("Iterations " + p.Engine.Params["Iterations"].Value.ToString());
      //  p.Engine.Params["Precision"].Value = 0.000001;
      //  Console.WriteLine("Precision " + p.Engine.Params["Precision"].Value.ToString("F10"));
      //  p.Engine.Params["Scaling"].Value = 0;
      //  Console.WriteLine(p.Engine.Params["Scaling"].Value.ToString());
      //  Console.WriteLine();
      //  Console.WriteLine("Solving this many points: " + myPoints.Count.ToString());

      //  p.Solver.Optimize();
      //  p.Dispose();
      //  #endregion
      //}

      //Count = 1;
      //stepsize = 1.0e-9;
      //using (Problem p = new Problem(Solver_Type.Minimize, Count, 1))
      //{
      //  #region Problem Setup
      //  p.Evaluators[Eval_Type.Iteration].OnEvaluate += new EvaluateEventHandler(frmMain_OnEvaluate);
      //  p.Evaluators[Eval_Type.Function].OnEvaluate += new EvaluateEventHandler(Evaluator_Evaluate);

      //  p.VarDecision.InitialValue[0] = A_x;

      //  p.Engine = p.Engines[Engine.GRGName];
      //  p.ProblemType = Problem_Type.OptLP;
      //  p.Engine.Params["MaxTime"].Value = 120;
      //  Console.WriteLine(p.Engine.Params["MaxTime"].Value.ToString("F10"));
      //  p.Engine.Params["SearchOption"].Value = 1;
      //  Console.WriteLine("SearchOption " + p.Engine.Params["SearchOption"].Value.ToString());
      //  p.Engine.Params["Derivatives"].Value = 1;
      //  Console.WriteLine("Derivatives " + p.Engine.Params["Derivatives"].Value.ToString());
      //  p.Engine.Params["Estimates"].Value = 1;
      //  Console.WriteLine("Estimates " + p.Engine.Params["Estimates"].Value.ToString());
      //  p.Engine.Params["stepsize"].Value = stepsize; // double.Parse(cmbStepSize.Text);
      //  Console.WriteLine("stepsize " + p.Engine.Params["stepsize"].Value.ToString("F10"));
      //  p.Engine.Params["Iterations"].Value = 100;
      //  Console.WriteLine("Iterations " + p.Engine.Params["Iterations"].Value.ToString());
      //  p.Engine.Params["Precision"].Value = 0.000001;
      //  Console.WriteLine("Precision " + p.Engine.Params["Precision"].Value.ToString("F10"));
      //  p.Engine.Params["Scaling"].Value = 0;
      //  Console.WriteLine(p.Engine.Params["Scaling"].Value.ToString());
      //  Console.WriteLine();
      //  Console.WriteLine("Solving this many points: " + myPoints.Count.ToString());
      //  //p.Engine.Params["convergance"].Value = .0001;// 10e-4 to 10e-9 affects how long it will chew on a problem.  Bigger the number, the less it will chew.
      //  //p.Engine.Params["multistart"].Value = 1;
      //  //p.Engine.Params["stepsize"].Value = .0001; //minimum guess size.
      //  //p.Engine.Params["Derivatives"].Value = 2;

      //  p.Solver.Optimize();
      //  p.Dispose();
      //  #endregion
      //}



    }

    private void SolveMeII()
    {
      double d2r = Math.PI / 180.0;
      double stepsize = 1.0e-9;



      Count = 9;
      using (Problem p = new Problem(Solver_Type.Minimize, Count+1, 1))
      {
        #region Problem Setup
        p.Evaluators[Eval_Type.Iteration].OnEvaluate += new EvaluateEventHandler(frmMain_OnEvaluate);
        p.Evaluators[Eval_Type.Function].OnEvaluate += new EvaluateEventHandler(Evaluator_Evaluate);

        p.VarDecision.InitialValue[0] = myBarrelFunction.SpinToFRC.rX / d2r;
        p.VarDecision.InitialValue[1] = myBarrelFunction.SpinToFRC.rY / d2r;
        p.VarDecision.InitialValue[2] = myBarrelFunction.SpinToFRC.rZ / d2r;
        p.VarDecision.InitialValue[3] = myBarrelFunction.A_x;
        p.VarDecision.InitialValue[4] = myBarrelFunction.MandrelToSpin.X;
        p.VarDecision.InitialValue[5] = myBarrelFunction.MandrelToSpin.Y;
        p.VarDecision.InitialValue[6] = myBarrelFunction.MandrelToSpin.Z;
        p.VarDecision.InitialValue[7] = myBarrelFunction.MandrelToSpin.rX / d2r;
        p.VarDecision.InitialValue[8] = myBarrelFunction.MandrelToSpin.rY / d2r;
        p.VarDecision.InitialValue[9] = myBarrelFunction.MandrelToSpin.rZ / d2r;

        p.Engine = p.Engines[Engine.GRGName];
        p.ProblemType = Problem_Type.OptLP;
        p.Engine.Params["MaxTime"].Value = 120;
        Console.WriteLine(p.Engine.Params["MaxTime"].Value.ToString("F10"));
        p.Engine.Params["SearchOption"].Value = 1;
        Console.WriteLine("SearchOption " + p.Engine.Params["SearchOption"].Value.ToString());
        p.Engine.Params["Derivatives"].Value = 1;
        Console.WriteLine("Derivatives " + p.Engine.Params["Derivatives"].Value.ToString());
        p.Engine.Params["Estimates"].Value = 1;
        Console.WriteLine("Estimates " + p.Engine.Params["Estimates"].Value.ToString());
        stepsize = 1.0e-7;
        p.Engine.Params["stepsize"].Value = stepsize; // double.Parse(cmbStepSize.Text);
        Console.WriteLine("stepsize " + p.Engine.Params["stepsize"].Value.ToString("F10"));
        p.Engine.Params["Iterations"].Value = 100;
        Console.WriteLine("Iterations " + p.Engine.Params["Iterations"].Value.ToString());
        p.Engine.Params["Precision"].Value = 0.000001;
        Console.WriteLine("Precision " + p.Engine.Params["Precision"].Value.ToString("F10"));
        p.Engine.Params["Scaling"].Value = 0;
        Console.WriteLine(p.Engine.Params["Scaling"].Value.ToString());
        Console.WriteLine();
        Console.WriteLine("Solving this many points: " + myPoints.Count.ToString());
        //p.Engine.Params["convergance"].Value = .0001;// 10e-4 to 10e-9 affects how long it will chew on a problem.  Bigger the number, the less it will chew.
        //p.Engine.Params["multistart"].Value = 1;
        //p.Engine.Params["stepsize"].Value = .0001; //minimum guess size.
        //p.Engine.Params["Derivatives"].Value = 2;

        p.Solver.Optimize();
        p.Dispose();
        #endregion
      }
    }

    private void SolveMeOriginal()
    {
      double d2r = Math.PI / 180.0;
      double stepsize = 1.0e-6;

      Count = 3;
      stepsize = 1.0e-6;
      using (Problem p = new Problem(Solver_Type.Minimize, Count, 1))
      {
        #region Problem Setup
        p.Evaluators[Eval_Type.Iteration].OnEvaluate += new EvaluateEventHandler(frmMain_OnEvaluate);
        p.Evaluators[Eval_Type.Function].OnEvaluate += new EvaluateEventHandler(Evaluator_Evaluate);

        p.VarDecision.InitialValue[0] = myBarrelFunction.MandrelToSpin.rX / d2r;
        p.VarDecision.InitialValue[1] = myBarrelFunction.MandrelToSpin.rY / d2r;
        p.VarDecision.InitialValue[2] = myBarrelFunction.MandrelToSpin.rZ / d2r;

        p.Engine = p.Engines[Engine.GRGName];
        p.ProblemType = Problem_Type.OptLP;
        p.Engine.Params["MaxTime"].Value = 120;
        Console.WriteLine(p.Engine.Params["MaxTime"].Value.ToString("F10"));
        p.Engine.Params["SearchOption"].Value = 1;
        Console.WriteLine("SearchOption " + p.Engine.Params["SearchOption"].Value.ToString());
        p.Engine.Params["Derivatives"].Value = 1;
        Console.WriteLine("Derivatives " + p.Engine.Params["Derivatives"].Value.ToString());
        p.Engine.Params["Estimates"].Value = 1;
        Console.WriteLine("Estimates " + p.Engine.Params["Estimates"].Value.ToString());
        p.Engine.Params["stepsize"].Value = stepsize; // double.Parse(cmbStepSize.Text);
        Console.WriteLine("stepsize " + p.Engine.Params["stepsize"].Value.ToString("F10"));
        p.Engine.Params["Iterations"].Value = 100;
        Console.WriteLine("Iterations " + p.Engine.Params["Iterations"].Value.ToString());
        p.Engine.Params["Precision"].Value = 0.000001;
        Console.WriteLine("Precision " + p.Engine.Params["Precision"].Value.ToString("F10"));
        p.Engine.Params["Scaling"].Value = 0;
        Console.WriteLine(p.Engine.Params["Scaling"].Value.ToString());
        Console.WriteLine();
        Console.WriteLine("Solving this many points: " + myPoints.Count.ToString());
        //p.Engine.Params["convergance"].Value = .0001;// 10e-4 to 10e-9 affects how long it will chew on a problem.  Bigger the number, the less it will chew.
        //p.Engine.Params["multistart"].Value = 1;
        //p.Engine.Params["stepsize"].Value = .0001; //minimum guess size.
        //p.Engine.Params["Derivatives"].Value = 2;

        p.Solver.Optimize();
        p.Dispose();
        #endregion
      }


      Count = 3;
      stepsize = 1.0e-9;
      using (Problem p = new Problem(Solver_Type.Minimize, Count, 1))
      {
        #region Problem Setup
        p.Evaluators[Eval_Type.Iteration].OnEvaluate += new EvaluateEventHandler(frmMain_OnEvaluate);
        p.Evaluators[Eval_Type.Function].OnEvaluate += new EvaluateEventHandler(Evaluator_Evaluate);

        p.VarDecision.InitialValue[0] = myBarrelFunction.MandrelToSpin.rX / d2r;
        p.VarDecision.InitialValue[1] = myBarrelFunction.MandrelToSpin.rY / d2r;
        p.VarDecision.InitialValue[2] = myBarrelFunction.MandrelToSpin.rZ / d2r;

        p.Engine = p.Engines[Engine.GRGName];
        p.ProblemType = Problem_Type.OptLP;
        p.Engine.Params["MaxTime"].Value = 120;
        Console.WriteLine(p.Engine.Params["MaxTime"].Value.ToString("F10"));
        p.Engine.Params["SearchOption"].Value = 1;
        Console.WriteLine("SearchOption " + p.Engine.Params["SearchOption"].Value.ToString());
        p.Engine.Params["Derivatives"].Value = 1;
        Console.WriteLine("Derivatives " + p.Engine.Params["Derivatives"].Value.ToString());
        p.Engine.Params["Estimates"].Value = 1;
        Console.WriteLine("Estimates " + p.Engine.Params["Estimates"].Value.ToString());
        p.Engine.Params["stepsize"].Value = stepsize; // double.Parse(cmbStepSize.Text);
        Console.WriteLine("stepsize " + p.Engine.Params["stepsize"].Value.ToString("F10"));
        p.Engine.Params["Iterations"].Value = 100;
        Console.WriteLine("Iterations " + p.Engine.Params["Iterations"].Value.ToString());
        p.Engine.Params["Precision"].Value = 0.000001;
        Console.WriteLine("Precision " + p.Engine.Params["Precision"].Value.ToString("F10"));
        p.Engine.Params["Scaling"].Value = 0;
        Console.WriteLine(p.Engine.Params["Scaling"].Value.ToString());
        Console.WriteLine();
        Console.WriteLine("Solving this many points: " + myPoints.Count.ToString());
        //p.Engine.Params["convergance"].Value = .0001;// 10e-4 to 10e-9 affects how long it will chew on a problem.  Bigger the number, the less it will chew.
        //p.Engine.Params["multistart"].Value = 1;
        //p.Engine.Params["stepsize"].Value = .0001; //minimum guess size.
        //p.Engine.Params["Derivatives"].Value = 2;

        p.Solver.Optimize();
        p.Dispose();
        #endregion
      }


      Count = 3;
      stepsize = 1.0e-9;
      using (Problem p = new Problem(Solver_Type.Minimize, Count, 1))
      {
        #region Problem Setup
        p.Evaluators[Eval_Type.Iteration].OnEvaluate += new EvaluateEventHandler(frmMain_OnEvaluate);
        p.Evaluators[Eval_Type.Function].OnEvaluate += new EvaluateEventHandler(Evaluator_Evaluate);

        p.VarDecision.InitialValue[0] = myBarrelFunction.MandrelToSpin.rX / d2r;
        p.VarDecision.InitialValue[1] = myBarrelFunction.MandrelToSpin.rY / d2r;
        p.VarDecision.InitialValue[2] = myBarrelFunction.MandrelToSpin.rZ / d2r;

        p.Engine = p.Engines[Engine.GRGName];
        p.ProblemType = Problem_Type.OptLP;
        p.Engine.Params["MaxTime"].Value = 120;
        Console.WriteLine(p.Engine.Params["MaxTime"].Value.ToString("F10"));
        p.Engine.Params["SearchOption"].Value = 1;
        Console.WriteLine("SearchOption " + p.Engine.Params["SearchOption"].Value.ToString());
        p.Engine.Params["Derivatives"].Value = 1;
        Console.WriteLine("Derivatives " + p.Engine.Params["Derivatives"].Value.ToString());
        p.Engine.Params["Estimates"].Value = 1;
        Console.WriteLine("Estimates " + p.Engine.Params["Estimates"].Value.ToString());
        p.Engine.Params["stepsize"].Value = stepsize; // double.Parse(cmbStepSize.Text);
        Console.WriteLine("stepsize " + p.Engine.Params["stepsize"].Value.ToString("F10"));
        p.Engine.Params["Iterations"].Value = 100;
        Console.WriteLine("Iterations " + p.Engine.Params["Iterations"].Value.ToString());
        p.Engine.Params["Precision"].Value = 0.000001;
        Console.WriteLine("Precision " + p.Engine.Params["Precision"].Value.ToString("F10"));
        p.Engine.Params["Scaling"].Value = 0;
        Console.WriteLine(p.Engine.Params["Scaling"].Value.ToString());
        Console.WriteLine();
        Console.WriteLine("Solving this many points: " + myPoints.Count.ToString());
        //p.Engine.Params["convergance"].Value = .0001;// 10e-4 to 10e-9 affects how long it will chew on a problem.  Bigger the number, the less it will chew.
        //p.Engine.Params["multistart"].Value = 1;
        //p.Engine.Params["stepsize"].Value = .0001; //minimum guess size.
        //p.Engine.Params["Derivatives"].Value = 2;

        p.Solver.Optimize();
        p.Dispose();
        #endregion
      }


      Count = 4;
      using (Problem p = new Problem(Solver_Type.Minimize, Count, 1))
      {
        #region Problem Setup
        p.Evaluators[Eval_Type.Iteration].OnEvaluate += new EvaluateEventHandler(frmMain_OnEvaluate);
        p.Evaluators[Eval_Type.Function].OnEvaluate += new EvaluateEventHandler(Evaluator_Evaluate);

        p.VarDecision.InitialValue[0] = myBarrelFunction.MandrelToSpin.rX / d2r;
        p.VarDecision.InitialValue[1] = myBarrelFunction.MandrelToSpin.rY / d2r;
        p.VarDecision.InitialValue[2] = myBarrelFunction.MandrelToSpin.rZ / d2r;
        p.VarDecision.InitialValue[3] = myBarrelFunction.A_x;

        p.Engine = p.Engines[Engine.GRGName];
        p.ProblemType = Problem_Type.OptLP;
        p.Engine.Params["MaxTime"].Value = 120;
        Console.WriteLine(p.Engine.Params["MaxTime"].Value.ToString("F10"));
        p.Engine.Params["SearchOption"].Value = 1;
        Console.WriteLine("SearchOption " + p.Engine.Params["SearchOption"].Value.ToString());
        p.Engine.Params["Derivatives"].Value = 1;
        Console.WriteLine("Derivatives " + p.Engine.Params["Derivatives"].Value.ToString());
        p.Engine.Params["Estimates"].Value = 1;
        Console.WriteLine("Estimates " + p.Engine.Params["Estimates"].Value.ToString());
        stepsize = 1.0e-7;
        p.Engine.Params["stepsize"].Value = stepsize; // double.Parse(cmbStepSize.Text);
        Console.WriteLine("stepsize " + p.Engine.Params["stepsize"].Value.ToString("F10"));
        p.Engine.Params["Iterations"].Value = 100;
        Console.WriteLine("Iterations " + p.Engine.Params["Iterations"].Value.ToString());
        p.Engine.Params["Precision"].Value = 0.000001;
        Console.WriteLine("Precision " + p.Engine.Params["Precision"].Value.ToString("F10"));
        p.Engine.Params["Scaling"].Value = 0;
        Console.WriteLine(p.Engine.Params["Scaling"].Value.ToString());
        Console.WriteLine();
        Console.WriteLine("Solving this many points: " + myPoints.Count.ToString());
        //p.Engine.Params["convergance"].Value = .0001;// 10e-4 to 10e-9 affects how long it will chew on a problem.  Bigger the number, the less it will chew.
        //p.Engine.Params["multistart"].Value = 1;
        //p.Engine.Params["stepsize"].Value = .0001; //minimum guess size.
        //p.Engine.Params["Derivatives"].Value = 2;

        p.Solver.Optimize();
        p.Dispose();
        #endregion
      }


      Count = 10;
      using (Problem p = new Problem(Solver_Type.Minimize, Count, 1))
      {
        #region Problem Setup
        p.Evaluators[Eval_Type.Iteration].OnEvaluate += new EvaluateEventHandler(frmMain_OnEvaluate);
        p.Evaluators[Eval_Type.Function].OnEvaluate += new EvaluateEventHandler(Evaluator_Evaluate);

        p.VarDecision.InitialValue[0] = myBarrelFunction.MandrelToSpin.rX / d2r;
        p.VarDecision.InitialValue[1] = myBarrelFunction.MandrelToSpin.rY / d2r;
        p.VarDecision.InitialValue[2] = myBarrelFunction.MandrelToSpin.rZ / d2r;
        p.VarDecision.InitialValue[3] = myBarrelFunction.A_x;
        p.VarDecision.InitialValue[4] = myBarrelFunction.SpinToFRC.X;
        p.VarDecision.InitialValue[5] = myBarrelFunction.SpinToFRC.Y;
        p.VarDecision.InitialValue[6] = myBarrelFunction.SpinToFRC.Z;
        p.VarDecision.InitialValue[7] = myBarrelFunction.SpinToFRC.rX / d2r;
        p.VarDecision.InitialValue[8] = myBarrelFunction.SpinToFRC.rY / d2r;
        p.VarDecision.InitialValue[9] = myBarrelFunction.SpinToFRC.rZ / d2r;

        p.Engine = p.Engines[Engine.GRGName];
        p.ProblemType = Problem_Type.OptLP;
        p.Engine.Params["MaxTime"].Value = 120;
        Console.WriteLine(p.Engine.Params["MaxTime"].Value.ToString("F10"));
        p.Engine.Params["SearchOption"].Value = 1;
        Console.WriteLine("SearchOption " + p.Engine.Params["SearchOption"].Value.ToString());
        p.Engine.Params["Derivatives"].Value = 1;
        Console.WriteLine("Derivatives " + p.Engine.Params["Derivatives"].Value.ToString());
        p.Engine.Params["Estimates"].Value = 1;
        Console.WriteLine("Estimates " + p.Engine.Params["Estimates"].Value.ToString());
        stepsize = 1.0e-7;
        p.Engine.Params["stepsize"].Value = stepsize; // double.Parse(cmbStepSize.Text);
        Console.WriteLine("stepsize " + p.Engine.Params["stepsize"].Value.ToString("F10"));
        p.Engine.Params["Iterations"].Value = 100;
        Console.WriteLine("Iterations " + p.Engine.Params["Iterations"].Value.ToString());
        p.Engine.Params["Precision"].Value = 0.000001;
        Console.WriteLine("Precision " + p.Engine.Params["Precision"].Value.ToString("F10"));
        p.Engine.Params["Scaling"].Value = 0;
        Console.WriteLine(p.Engine.Params["Scaling"].Value.ToString());
        Console.WriteLine();
        Console.WriteLine("Solving this many points: " + myPoints.Count.ToString());
        //p.Engine.Params["convergance"].Value = .0001;// 10e-4 to 10e-9 affects how long it will chew on a problem.  Bigger the number, the less it will chew.
        //p.Engine.Params["multistart"].Value = 1;
        //p.Engine.Params["stepsize"].Value = .0001; //minimum guess size.
        //p.Engine.Params["Derivatives"].Value = 2;

        p.Solver.Optimize();
        p.Dispose();
        #endregion
      }
    }
    
    Engine_Action frmMain_OnEvaluate(Evaluator evaluator)
    {
      Evaluator_Evaluate(evaluator); //Try the try.
      Console.WriteLine(evaluator.Problem.FcnObjective.Value[0].ToString("0.000000"));


      return Engine_Action.Continue;
    }


    private Engine_Action Evaluator_Evaluate(Evaluator ev)
    {
      double d2r = Math.PI / 180.0;

      switch (Count)
      {
        case 1:
          myBarrelFunction.A_x = ev.Problem.VarDecision.Value[0];
          break;
        case 3:
          myBarrelFunction.MandrelToSpin.X = myBarrelFunction.MandrelToSpin.Y = myBarrelFunction.MandrelToSpin.Z = 0;
          myBarrelFunction.MandrelToSpin.rX = ev.Problem.VarDecision.Value[0] * d2r;
          myBarrelFunction.MandrelToSpin.rY = ev.Problem.VarDecision.Value[1] * d2r;
          myBarrelFunction.MandrelToSpin.rZ = ev.Problem.VarDecision.Value[2] * d2r;
          break;
        case 4:
          myBarrelFunction.A_x = ev.Problem.VarDecision.Value[3];
          myBarrelFunction.MandrelToSpin.X = myBarrelFunction.MandrelToSpin.Y = myBarrelFunction.MandrelToSpin.Z = 0;
          myBarrelFunction.MandrelToSpin.rX = ev.Problem.VarDecision.Value[0] * d2r;
          myBarrelFunction.MandrelToSpin.rY = ev.Problem.VarDecision.Value[1] * d2r;
          myBarrelFunction.MandrelToSpin.rZ = ev.Problem.VarDecision.Value[2] * d2r;
          break;
        case 9:
          myBarrelFunction.SpinToFRC.X = myBarrelFunction.SpinToFRC.Y = myBarrelFunction.SpinToFRC.Z = 0;
          myBarrelFunction.SpinToFRC.rX = ev.Problem.VarDecision.Value[0] * d2r;
          myBarrelFunction.SpinToFRC.rY = ev.Problem.VarDecision.Value[1] * d2r;
          myBarrelFunction.SpinToFRC.rZ = ev.Problem.VarDecision.Value[2] * d2r;
          myBarrelFunction.A_x = ev.Problem.VarDecision.Value[3];
          myBarrelFunction.MandrelToSpin.X = ev.Problem.VarDecision.Value[4];
          myBarrelFunction.MandrelToSpin.Y = ev.Problem.VarDecision.Value[5];
          myBarrelFunction.MandrelToSpin.Z = ev.Problem.VarDecision.Value[6];
          myBarrelFunction.MandrelToSpin.rX = ev.Problem.VarDecision.Value[7] * d2r;
          myBarrelFunction.MandrelToSpin.rY = ev.Problem.VarDecision.Value[8] * d2r;
          myBarrelFunction.MandrelToSpin.rZ = ev.Problem.VarDecision.Value[9] * d2r;
          if (chkStupid.Checked)
          {
            myBarrelFunction.A_x = 0;
            myBarrelFunction.MandrelToSpin.X = 0;
            myBarrelFunction.MandrelToSpin.Y = 0;
            myBarrelFunction.MandrelToSpin.Z = 0;
            myBarrelFunction.MandrelToSpin.rX = 0;
            myBarrelFunction.MandrelToSpin.rY = 0;
            myBarrelFunction.MandrelToSpin.rZ = 0;
          }

          break;
        case 10:
          myBarrelFunction.MandrelToSpin.X = myBarrelFunction.MandrelToSpin.Y = myBarrelFunction.MandrelToSpin.Z = 0;
          myBarrelFunction.MandrelToSpin.rX = ev.Problem.VarDecision.Value[0] * d2r;
          myBarrelFunction.MandrelToSpin.rY = ev.Problem.VarDecision.Value[1] * d2r;
          myBarrelFunction.MandrelToSpin.rZ = ev.Problem.VarDecision.Value[2] * d2r;
          myBarrelFunction.A_x = ev.Problem.VarDecision.Value[3];
          myBarrelFunction.SpinToFRC.X = ev.Problem.VarDecision.Value[4];
          myBarrelFunction.SpinToFRC.Y = ev.Problem.VarDecision.Value[5];
          myBarrelFunction.SpinToFRC.Z = ev.Problem.VarDecision.Value[6];
          myBarrelFunction.SpinToFRC.rX = ev.Problem.VarDecision.Value[7] * d2r;
          myBarrelFunction.SpinToFRC.rY = ev.Problem.VarDecision.Value[8] * d2r;
          myBarrelFunction.SpinToFRC.rZ = ev.Problem.VarDecision.Value[9] * d2r;
          break;
        default:
          break;
      }

      double error = 0.0;
      double Txerr = 0;
      double Tyerr = 0;
      double Tzerr = 0;
      int number_of_points_to_solve = 0;
      for (int ii = 0; ii < myPoints.Count; ii++)
      {
        double[] ret = myBarrelFunction.BarrelToFRC(myPoints[ii]);
        double xerr = (ret[0] - myPoints[ii].mx);
        double yerr = (ret[1] - myPoints[ii].my);
        double zerr = (ret[2] - myPoints[ii].mz);

        if (myPoints[ii].bUseInXform)
          number_of_points_to_solve++;
        else
          continue;

        Txerr += xerr;
        Tyerr += yerr;
        Tzerr += zerr;

        error += (xerr * xerr + yerr * yerr + zerr * zerr);
      }
      if( Count == 9 )
      {
        Txerr /= (double)number_of_points_to_solve;
        Tyerr /= (double)number_of_points_to_solve;
        Tzerr /= (double)number_of_points_to_solve;
        myBarrelFunction.SpinToFRC.X = -Txerr;
        myBarrelFunction.SpinToFRC.Y = -Tyerr;
        myBarrelFunction.SpinToFRC.Z = -Tzerr;
        error = 0.0;
        for (int ii = 0; ii < myPoints.Count; ii++)
        {
          if (!myPoints[ii].bUseInXform)
            continue;

          double[] ret = myBarrelFunction.BarrelToFRC(myPoints[ii]);
          double xerr = (ret[0] - myPoints[ii].mx);
          double yerr = (ret[1] - myPoints[ii].my);
          double zerr = (ret[2] - myPoints[ii].mz);
          error += (xerr * xerr + yerr * yerr + zerr * zerr);
        }
      }
      else
      {
        Txerr /= (double)number_of_points_to_solve;
        Tyerr /= (double)number_of_points_to_solve;
        Tzerr /= (double)number_of_points_to_solve;
        myBarrelFunction.MandrelToSpin.X = -Txerr;
        myBarrelFunction.MandrelToSpin.Y = -Tyerr;
        myBarrelFunction.MandrelToSpin.Z = -Tzerr;
        error = 0.0;
        for (int ii = 0; ii < myPoints.Count; ii++)
        {
          if (!myPoints[ii].bUseInXform)
            continue;

          double[] ret = myBarrelFunction.BarrelToFRC(myPoints[ii]);

          double xerr = (ret[0] - myPoints[ii].mx);
          double yerr = (ret[1] - myPoints[ii].my);
          double zerr = (ret[2] - myPoints[ii].mz);
          error += (xerr * xerr + yerr * yerr + zerr * zerr);
        }
      }
      
      ev.Problem.FcnObjective.Value[0] = error;
      return Engine_Action.Continue;
    }

      //check Euler Angle
    private void btnCheckEuler_Click(object sender, EventArgs e)
    {
      checked_points_list.Items.Clear();

      double d2r = Math.PI / 180.0;
      System.Collections.Generic.List<string> outputString = new List<string>();
      Console.WriteLine("Converting Tool Positions to MRS: ");

      Electroimpact.LinearAlgebra.c6dof MandrelToSpinInverse = new Electroimpact.LinearAlgebra.c6dof(myBarrelFunction.MandrelToSpin.Inverse());
      Electroimpact.LinearAlgebra.c6dof SpinToFRCInverse = new Electroimpact.LinearAlgebra.c6dof(myBarrelFunction.SpinToFRC.Inverse());
      Electroimpact.LinearAlgebra.RxMatrix u = new Electroimpact.LinearAlgebra.RxMatrix();
      double ErrorEuler = 0;
      double MaxRadial = 0;
      double TotalRadial = 0;

      outputString.Add("");
      outputString.Add("*** units are in inch unless otherwise noted ***");
      outputString.Add("");

      int total_points_in_transform = 0;
      
      for (int ii = 0; ii < myPoints.Count; ii++)
      {
        double[] point = new double[4];
        double[] PointOutput;

        double xerr;
        double yerr;
        double zerr;

        PointOutput = myBarrelFunction.BarrelToFRC(myPoints[ii]);

        xerr = PointOutput[0] - myPoints[ii].mx;
        yerr = PointOutput[1] - myPoints[ii].my;
        zerr = PointOutput[2] - myPoints[ii].mz;
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

        outputString.Add("Data for Point: " + (ii + 1).ToString());
        if (myPoints[ii].bUseInXform)
          outputString.Add("This point is used in the transform");
        else
          outputString.Add("This point is NOT used in the transform");

        outputString.Add("Regular 6DOF method: ");
        outputString.Add("X nominal:" + myPoints[ii].nx.ToString("F3").PadLeft(9) + " -->" + PointOutput[0].ToString("F3").PadLeft(9) + " Measured: " + myPoints[ii].mx.ToString("F3").PadLeft(9) + " error: " + xerr.ToString("F3").PadLeft(6));
        outputString.Add("Y nominal:" + myPoints[ii].ny.ToString("F3").PadLeft(9) + " -->" + PointOutput[1].ToString("F3").PadLeft(9) + " Measured: " + myPoints[ii].my.ToString("F3").PadLeft(9) + " error: " + yerr.ToString("F3").PadLeft(6));
        outputString.Add("Z nominal:" + myPoints[ii].nz.ToString("F3").PadLeft(9) + " -->" + PointOutput[2].ToString("F3").PadLeft(9) + " Measured: " + myPoints[ii].mz.ToString("F3").PadLeft(9) + " error: " + zerr.ToString("F3").PadLeft(6));

        cPoint p = new cPoint(0, 0, 0, PointOutput[0], PointOutput[1], PointOutput[2], myPoints[ii].upos);
        double[] BackToFwd = myBarrelFunction.FRCtoBarrel(p);

        outputString.Add(PointOutput[0].ToString("F3").PadLeft(9) + " --> " + BackToFwd[0].ToString("F3").PadLeft(9));
        outputString.Add(PointOutput[1].ToString("F3").PadLeft(9) + " --> " + BackToFwd[1].ToString("F3").PadLeft(9));
        outputString.Add(PointOutput[2].ToString("F3").PadLeft(9) + " --> " + BackToFwd[2].ToString("F3").PadLeft(9));

        outputString.Add("End of Point: " + ii.ToString());
        outputString.Add("");
      }
      ErrorEuler = Math.Sqrt(ErrorEuler);
      outputString.Add("");
      outputString.Add("Euler Fit (sqrt(sumsq()): " + ErrorEuler.ToString("F3"));
      outputString.Add("Euler Fit (Average): " + (TotalRadial / (double)total_points_in_transform).ToString("F3"));
      outputString.Add("Euler Fit (Max Radial): " + MaxRadial.ToString("F3"));
      outputString.Add("");
      outputString.Add("End of Check");
      outputString.Add("");
      outputString.Add("");
      outputString.Add("");

      #region Check some rotations

      //From Tool to FRC

      //Practice Transform
      //Electroimpact.LinearAlgebra.c6dof Practice = new Electroimpact.LinearAlgebra.c6dof(10, 10, 10, 179.824*d2r, -.184*d2r, .135*d2r);
      Electroimpact.LinearAlgebra.cMatrix fwd = new Electroimpact.LinearAlgebra.cMatrix(myBarrelFunction.MandrelToSpin.GetMatrix());
      Electroimpact.LinearAlgebra.cMatrix back = new Electroimpact.LinearAlgebra.cMatrix(fwd.Inverse());
      Electroimpact.LinearAlgebra.c6dof Rot = new Electroimpact.LinearAlgebra.c6dof(0, 0, 0, 0*d2r, 0*d2r, 0);
      Electroimpact.LinearAlgebra.cMatrix ret = new Electroimpact.LinearAlgebra.cMatrix(fwd.DotMe(Rot.GetMatrix()));
      outputString.Add("Rx: " + (Rot.rX/d2r).ToString("F3").PadLeft(9) + " --> " + ret.rXrYrZ[0, 0].ToString("F3").PadLeft(9));
      outputString.Add("Ry: " + (Rot.rY/d2r).ToString("F3").PadLeft(9) + " --> " + ret.rXrYrZ[1, 0].ToString("F3").PadLeft(9));
      outputString.Add("Rz: " + (Rot.rZ/d2r).ToString("F3").PadLeft(9) + " --> " + ret.rXrYrZ[2, 0].ToString("F3").PadLeft(9));

      //FRC to Tool
      Rot = new Electroimpact.LinearAlgebra.c6dof(0, 0, 0, ret.rXrYrZ[0, 0] * d2r, ret.rXrYrZ[1, 0] * d2r, ret.rXrYrZ[2, 0] * d2r);
      ret = new Electroimpact.LinearAlgebra.cMatrix(back.DotMe(Rot.GetMatrix()));
      outputString.Add("Rx: " + (Rot.rX / d2r).ToString("F3").PadLeft(9) + " --> " + ret.rXrYrZ[0, 0].ToString("F3").PadLeft(9));
      outputString.Add("Ry: " + (Rot.rY / d2r).ToString("F3").PadLeft(9) + " --> " + ret.rXrYrZ[1, 0].ToString("F3").PadLeft(9));
      outputString.Add("Rz: " + (Rot.rZ / d2r).ToString("F3").PadLeft(9) + " --> " + ret.rXrYrZ[2, 0].ToString("F3").PadLeft(9));

      #endregion
      string[] lines = new string[outputString.Count];
      for (int ii = 0; ii < outputString.Count; ii++)
      {
        lines[ii] = outputString[ii];
      }
      txtEulerCheck.Lines = lines;
    }

    //send to CNC
    private void btnNullComp_Click(object sender, EventArgs e)
    {
      Electroimpact.FANUC.Err_Code err;
      Electroimpact.FANUC.OpenCNC CNC;// = new Electroimpact.FANUC.OpenCNC();
      DocuTrackProSE.InputBoxDialog ib = new DocuTrackProSE.InputBoxDialog();
      ib.FormPrompt = "Input CNC IP";
      ib.FormCaption = "CNC IP Dialog";
      ib.DefaultValue = "192.168.168.";
      ib.ShowDialog();
      CNC = new Electroimpact.FANUC.OpenCNC(ib.InputResponse, out err);
      if( err != Electroimpact.FANUC.Err_Code.EW_OK)
      {
        MessageBox.Show("No Joy");
        return;
      }

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
    }

    private void btnCopyEuler_Click(object sender, EventArgs e)
    {
      string output = GetEuler(myBarrelFunction.MandrelToSpin);
      System.Windows.Forms.Clipboard.SetText(output);
    }

    private void btnCopySpinToFRC_Click(object sender, EventArgs e)
    {
      string output = GetEuler(myBarrelFunction.SpinToFRC);
      System.Windows.Forms.Clipboard.SetText(output);
    }

    private string GetEuler(Electroimpact.LinearAlgebra.c6dof Transform)
    {
      double d2r = Math.PI / 180.0;
      string output = "";
      output += Transform.X.ToString("F10") + ",";
      output += Transform.Y.ToString("F10") + ",";
      output += Transform.Z.ToString("F10") + ",";
      output += (Transform.rX / d2r).ToString("F10") + ",";
      output += (Transform.rY / d2r).ToString("F10") + ",";
      output += (Transform.rZ / d2r).ToString("F10") + ",";
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
      b_FirstTimeThru = true;
      string textIN = System.Windows.Forms.Clipboard.GetText();
      string[] lines = textIN.Split('\n');
      Electroimpact.FileIO.cFileWriter fw = new Electroimpact.FileIO.cFileWriter();
      string filename = Electroimpact.FileIO.cFileOther.GetExecutionDirectoryMethod() + "points.csv";
      fw.WriteLine(filename, "Xnom,Ynom,Znom,Upos,Xmeas,Ymeas,Zmeas", false);
      for (int ii = 0; ii < lines.Length; ii++)
      {
        string[] items = lines[ii].Split('\t');
        if (items.Length == 7)
        {
          double[] values = new double[7];
          string line = "";
          for (int jj = 0; jj < 7; jj++)
          {
            values[jj] = double.Parse(items[jj]);
            line += values[jj].ToString("F6") + ",";
          }
          fw.WriteLine(filename, line, true);
        }
      }
      Console.WriteLine(textIN);
      btnTransform_Click(sender, e);
    }

    private void btnCopyEulerMatrix_Click(object sender, EventArgs e)
    {
      double[,] myEuler = myBarrelFunction.MandrelToSpin.GetMatrix();
      string ret = "";

      for (int row = 0; row < 4; row++)
      {
        for (int col = 0; col < 4; col++)
        {
          ret += myEuler[row, col].ToString("F6") + ",";
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
      System.Collections.Generic.List<double[]> myVectors = new List<double[]>();
      for (int ii = 0; ii < lines.Length; ii++)
      {
        string[] items = lines[ii].Split('\t');
        if (items.Length == 4)
        {
          double[] values = new double[4];
          for (int jj = 0; jj < 4; jj++)
          {
            values[jj] = double.Parse(items[jj]);
          }
          if (reverse)
          {
            cPoint apoint = new cPoint(0, 0, 0, values[0], values[1], values[2], values[3]);
            myVectors.Add(myBarrelFunction.FRCtoBarrel(apoint));
          }
          else
          {
            cPoint apoint = new cPoint(values[0], values[1], values[2], 0, 0, 0, values[3]);
            myVectors.Add(myBarrelFunction.BarrelToFRC(apoint));
          }
        }
      }
      string[] linesout = new string[myVectors.Count + 1];
      linesout[0] = "X    ".PadLeft(12) + "Y    ".PadLeft(12) + "Z    ".PadLeft(12);
      for (int ii = 0; ii < myVectors.Count; ii++)
      {
        double[] vector = myVectors[ii];
        if (vector.Length == 4)
          linesout[ii + 1] = vector[0].ToString("F4").PadLeft(12) + vector[1].ToString("F4").PadLeft(12) + vector[2].ToString("F4").PadLeft(12);
        else if (vector.Length == 6)
          linesout[ii + 1] = vector[0].ToString("F4").PadLeft(12) + vector[1].ToString("F4").PadLeft(12) + vector[2].ToString("F4").PadLeft(12) + vector[3].ToString("F4").PadLeft(12) + vector[4].ToString("F4").PadLeft(12) + vector[5].ToString("F4").PadLeft(12);
      }
      txtPointsTransformed.Lines = linesout;
    }

    private void btnGetaCSVCopy_Click(object sender, EventArgs e)
    {
      string outtext = "";
      Electroimpact.csString eis = new Electroimpact.csString();
      for (int ii = 1; ii < txtPointsTransformed.Lines.Length; ii++)
      {
        string[] test = txtPointsTransformed.Lines[ii].Split(' ');
        string[] test2 = new string[3];
        int kk = 0;
        for (int jj = 0; jj < test.Length; jj++)
        {
          if (test[jj] != "")
            test2[kk++] = test[jj];
          if (kk > 2)
            break;
        }
        eis.String = txtPointsTransformed.Lines[ii];
        if (ii > 1)
          outtext += "\n";
        outtext += test2[0] + "," + test2[1] + "," + test2[2];
      }
      System.Windows.Forms.Clipboard.Clear();
      System.Windows.Forms.Clipboard.SetText(outtext);
    }

    private void Form1_Load(object sender, EventArgs e)
    {
      this.Size = new Size(453, 893);

#if(!DEBUG)
   SolverPlatform.Environment _spe = new
   SolverPlatform.Environment("r0028082");
#endif

    }

    private void btnCompToCNC_Click(object sender, EventArgs e)
    {
      this.Enabled = false;
      Electroimpact.FANUC.Err_Code err;
      Electroimpact.FANUC.OpenCNC CNC;// = new Electroimpact.FANUC.OpenCNC();
      DocuTrackProSE.InputBoxDialog ib = new DocuTrackProSE.InputBoxDialog();
      ib.FormPrompt = "Input CNC IP";
      ib.FormCaption = "CNC IP Dialog";
      string cncip = "192.168.1.";

      if (Electroimpact.FileIO.cFileOther.FileExistsMethod("cncip.txt"))
      {
        Electroimpact.FileIO.cFileReader fr = new Electroimpact.FileIO.cFileReader();
        fr.OpenFile("cncip.txt");
        if (fr.Peek())
          cncip = fr.ReadLine();
        fr.CloseFile();
      }
      ib.DefaultValue = cncip;
      DialogResult dr = ib.ShowDialog();
      if (dr == DialogResult.Cancel)
      {
        this.Enabled = true;
        return;
      }
      Electroimpact.FileIO.cFileWriter fw = new Electroimpact.FileIO.cFileWriter();
      fw.WriteLine("cncip.txt", ib.InputResponse, false);
      fw.CloseFile();
      CNC = new Electroimpact.FANUC.OpenCNC(ib.InputResponse, out err);
      if (err != Electroimpact.FANUC.Err_Code.EW_OK)
      {
        MessageBox.Show("No Joy");
        this.Enabled = true;
        return;
      }

      {
        {
          Int32[] dong = new Int32[18];
          if (CNC.Connected)
          {
            double mult = 25.4;
            double d2r = Math.PI / 180.0;
            Int32[] values = new Int32[18];
            Int32[] valuesback = new Int32[18];

            values[0] = (Int32)(Math.Round(myBarrelFunction.MandrelToSpin.X * mult, OffsetPower) * OffsetPrecision);
            values[1] = (Int32)(Math.Round(myBarrelFunction.MandrelToSpin.Y * mult, OffsetPower) * OffsetPrecision);
            values[2] = (Int32)(Math.Round(myBarrelFunction.MandrelToSpin.Z * mult, OffsetPower) * OffsetPrecision);
            values[3] = (Int32)(Math.Round(myBarrelFunction.MandrelToSpin.rX / d2r, AnlgePower) * AnglePrecision);
            values[4] = (Int32)(Math.Round(myBarrelFunction.MandrelToSpin.rY / d2r, AnlgePower) * AnglePrecision);
            values[5] = (Int32)(Math.Round(myBarrelFunction.MandrelToSpin.rZ / d2r, AnlgePower) * AnglePrecision);

            Electroimpact.LinearAlgebra.cMatrix EulerInverse = new Electroimpact.LinearAlgebra.cMatrix(myBarrelFunction.MandrelToSpin.Inverse());
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
            valuesback = CNC.ReadPMCRange(Address, out err);
            if (valuesback.Length == values.Length)
            {
              for (int ii = 0; ii < values.Length; ii++)
              {
                if( !Equal2(valuesback[ii],values[ii], 1) )
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

            values[0] = (Int32)(Math.Round(myBarrelFunction.SpinToFRC.X * mult, OffsetPower) * OffsetPrecision);
            values[1] = (Int32)(Math.Round(myBarrelFunction.SpinToFRC.Y * mult, OffsetPower) * OffsetPrecision);
            values[2] = (Int32)(Math.Round(myBarrelFunction.SpinToFRC.Z * mult, OffsetPower) * OffsetPrecision);
            values[3] = (Int32)(Math.Round(myBarrelFunction.SpinToFRC.rX / d2r, AnlgePower) * AnglePrecision);
            values[4] = (Int32)(Math.Round(myBarrelFunction.SpinToFRC.rY / d2r, AnlgePower) * AnglePrecision);
            values[5] = (Int32)(Math.Round(myBarrelFunction.SpinToFRC.rZ / d2r, AnlgePower) * AnglePrecision);

            EulerInverse = new Electroimpact.LinearAlgebra.cMatrix(myBarrelFunction.SpinToFRC.Inverse());
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
            valuesback = CNC.ReadPMCRange(Address, out err);
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

            Address = "D7492*4";
            CNC.WritePMCData(Address, (Int32)(Math.Round(myBarrelFunction.A_x * mult, OffsetPower) * OffsetPrecision));
            int down = (Int32)(Math.Round(myBarrelFunction.A_x * mult, OffsetPower) * OffsetPrecision);
            int back = CNC.ReadPMCData(Address, out err);
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
      }

    private bool LoadFromTransforms()
    {
      Electroimpact.FileIO.cFileReader fr = new Electroimpact.FileIO.cFileReader();

      if (!Electroimpact.FileIO.cFileOther.FileExistsMethod("Transforms.csv"))
      {
        MessageBox.Show("Transforms.csv does not exist!");
        return false;
      }
      string lastline = "";
      fr.OpenFile("Transforms.csv");
      while (fr.Peek())
      {
        lastline = fr.ReadLine();
      }
      string[] transform = lastline.Split(',');
      double[] values = new double[transform.Length];
      if( transform.Length < 13 )
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
      myBarrelFunction.MandrelToSpin.X = values[0];
      myBarrelFunction.MandrelToSpin.Y = values[1];
      myBarrelFunction.MandrelToSpin.Z = values[2];
      myBarrelFunction.MandrelToSpin.rX = values[3] * d2r;
      myBarrelFunction.MandrelToSpin.rY = values[4] * d2r;
      myBarrelFunction.MandrelToSpin.rZ = values[5] * d2r;
      myBarrelFunction.A_x = values[6];
      myBarrelFunction.SpinToFRC.X = values[7];
      myBarrelFunction.SpinToFRC.Y = values[8];
      myBarrelFunction.SpinToFRC.Z = values[9];
      myBarrelFunction.SpinToFRC.rX = values[10] * d2r;
      myBarrelFunction.SpinToFRC.rY = values[11] * d2r;
      myBarrelFunction.SpinToFRC.rZ = values[12] * d2r;

      //fw.WriteLine("Transforms.csv",
      //                              myBarrelFunction.MandrelToSpin.X.ToString("F3") + "," +
      //                              myBarrelFunction.MandrelToSpin.Y.ToString("F3") + "," +
      //                              myBarrelFunction.MandrelToSpin.Z.ToString("F3") + "," +
      //                              (myBarrelFunction.MandrelToSpin.rX / d2r).ToString("F6") + "," +
      //                              (myBarrelFunction.MandrelToSpin.rY / d2r).ToString("F6") + "," +
      //                              (myBarrelFunction.MandrelToSpin.rZ / d2r).ToString("F6") + "," +
      //                              myBarrelFunction.A_x.ToString("F6") + "," +
      //                              myBarrelFunction.SpinToFRC.X.ToString("F3") + "," +
      //                              myBarrelFunction.SpinToFRC.Y.ToString("F3") + "," +
      //                              myBarrelFunction.SpinToFRC.Z.ToString("F3") + "," +
      //                              (myBarrelFunction.SpinToFRC.rX / d2r).ToString("F6") + "," +
      //                              (myBarrelFunction.SpinToFRC.rY / d2r).ToString("F6") + "," +
      //                              (myBarrelFunction.SpinToFRC.rZ / d2r).ToString("F6") + ","
      //                              , true);
      fr.CloseFile();
      return true;
    }

    private void btnVerifyCompOnCNC_Click(object sender, EventArgs e)
    {
      this.Enabled = false;
      Electroimpact.FANUC.Err_Code err;
      Electroimpact.FANUC.OpenCNC CNC;// = new Electroimpact.FANUC.OpenCNC();
      DocuTrackProSE.InputBoxDialog ib = new DocuTrackProSE.InputBoxDialog();
      ib.FormPrompt = "Input CNC IP";
      ib.FormCaption = "CNC IP Dialog";
      string cncip = "192.168.1.";
      if (Electroimpact.FileIO.cFileOther.FileExistsMethod("cncip.txt"))
      {
        Electroimpact.FileIO.cFileReader fr = new Electroimpact.FileIO.cFileReader();
        fr.OpenFile("cncip.txt");
        if (fr.Peek())
          cncip = fr.ReadLine();
        fr.CloseFile();
      }
      ib.DefaultValue = cncip;
      ib.ShowDialog();

      if (ib.DialogResult == DialogResult.Cancel)
      {
        this.Enabled = true;
        return;
      }
      Electroimpact.FileIO.cFileWriter fw = new Electroimpact.FileIO.cFileWriter();
      fw.WriteLine("cncip.txt", ib.InputResponse, false);
      fw.CloseFile();

      CNC = new Electroimpact.FANUC.OpenCNC(ib.InputResponse, out err);
      if (err != Electroimpact.FANUC.Err_Code.EW_OK)
      {
        MessageBox.Show("No Joy");
        this.Enabled = true;
        return;
      }

      {
        {
          Int32[] dong = new Int32[18];
          if (!LoadFromTransforms())
          {
            this.Enabled = true;
            return;
          }
          if (CNC.Connected)
          {
            double mult = 25.4;
            double d2r = Math.PI / 180.0;
            Int32[] values = new Int32[18];
            Int32[] valuesback = new Int32[18];

            values[0] = (Int32)(Math.Round(myBarrelFunction.MandrelToSpin.X * mult, OffsetPower) * OffsetPrecision);
            values[1] = (Int32)(Math.Round(myBarrelFunction.MandrelToSpin.Y * mult, OffsetPower) * OffsetPrecision);
            values[2] = (Int32)(Math.Round(myBarrelFunction.MandrelToSpin.Z * mult, OffsetPower) * OffsetPrecision);
            values[3] = (Int32)(Math.Round(myBarrelFunction.MandrelToSpin.rX / d2r, AnlgePower) * AnglePrecision);
            values[4] = (Int32)(Math.Round(myBarrelFunction.MandrelToSpin.rY / d2r, AnlgePower) * AnglePrecision);
            values[5] = (Int32)(Math.Round(myBarrelFunction.MandrelToSpin.rZ / d2r, AnlgePower) * AnglePrecision);

            Electroimpact.LinearAlgebra.cMatrix EulerInverse = new Electroimpact.LinearAlgebra.cMatrix(myBarrelFunction.MandrelToSpin.Inverse());
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
            valuesback = CNC.ReadPMCRange(Address, out err);
            if (valuesback.Length == values.Length)
            {
              for (int ii = 0; ii < values.Length; ii++)
              {
                if (!Equal2(valuesback[ii], values[ii], 10))
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
          }
        }
      }
      MessageBox.Show("Yep...All is well");
      this.Enabled = true;
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
        this.Size = new Size(1233, 893);
      }
      else
      {
        //453, 895
        //this.Size.Width = 453;//"1233, 893";
        //this.Size.Height = 895;
        this.Size = new Size(453, 893);
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


  }
}
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
      // TODO: Add any constructor code after InitializeComponent call
      //
    }

    #endregion

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
}