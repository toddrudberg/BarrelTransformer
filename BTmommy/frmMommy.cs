using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BTmommy
{
  public partial class frmMommy : Form
  {
    public frmMommy()
    {
      InitializeComponent();
    }
    

    private void frmMommy_Load(object sender, EventArgs e)
    {
      Electroimpact.FileIO.cFileReader fr = new Electroimpact.FileIO.cFileReader();
      fr.OpenFile(@"C:\Users\toddr\Dropbox\3535-KAL\2013-08-05_12-40-45_Points.csv");
      List<string> lines = new List<string>();

      int nRows = 0;
      while (fr.Peek())
      {
        nRows++;
        lines.Add(fr.ReadLine());
      }
      fr.CloseFile();

      double[,] points = new double[nRows, 7];

      for (int row = 0; row < nRows; row++)
      {
        string[] numbers = lines[row].Split(',');
        for (int col = 0; col < 7; col++)
        {
          points[row, col] = double.Parse(numbers[col]);
        }
      }

      Electroimpact.Transformer.frmBarrelTransformer myxfmr = new Electroimpact.Transformer.frmBarrelTransformer();
      //myxfmr.loadSolver();
      myxfmr.ShowDialog();
    }
  }
}
