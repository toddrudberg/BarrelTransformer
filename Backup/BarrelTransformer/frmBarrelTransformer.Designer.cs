namespace Transformer
{
  partial class frmBarrelTransformer
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.components = new System.ComponentModel.Container();
      this.btnTransform = new System.Windows.Forms.Button();
      this.txtEulerCheck = new System.Windows.Forms.TextBox();
      this.btnNullComp = new System.Windows.Forms.Button();
      this.btnPasteInPointsToTransform = new System.Windows.Forms.Button();
      this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
      this.btnPastePoints = new System.Windows.Forms.Button();
      this.btnPasteToReverse = new System.Windows.Forms.Button();
      this.btnVerifyCompOnCNC = new System.Windows.Forms.Button();
      this.groupBox3 = new System.Windows.Forms.GroupBox();
      this.groupBox2 = new System.Windows.Forms.GroupBox();
      this.btnCopySpinToFRC = new System.Windows.Forms.Button();
      this.btnCopyEulerMatrix = new System.Windows.Forms.Button();
      this.btnCopyEuler = new System.Windows.Forms.Button();
      this.label4 = new System.Windows.Forms.Label();
      this.textBox5 = new System.Windows.Forms.TextBox();
      this.label2 = new System.Windows.Forms.Label();
      this.textBox2 = new System.Windows.Forms.TextBox();
      this.btnGetaCSVCopy = new System.Windows.Forms.Button();
      this.txtPointsTransformed = new System.Windows.Forms.TextBox();
      this.btnCompToCNC = new System.Windows.Forms.Button();
      this.chkShowAdvanced = new System.Windows.Forms.CheckBox();
      this.chkStupid = new System.Windows.Forms.CheckBox();
      this.checked_points_list = new System.Windows.Forms.CheckedListBox();
      this.label1 = new System.Windows.Forms.Label();
      this.groupBox3.SuspendLayout();
      this.groupBox2.SuspendLayout();
      this.SuspendLayout();
      // 
      // btnTransform
      // 
      this.btnTransform.Location = new System.Drawing.Point(582, 593);
      this.btnTransform.Name = "btnTransform";
      this.btnTransform.Size = new System.Drawing.Size(203, 25);
      this.btnTransform.TabIndex = 0;
      this.btnTransform.Text = "Transform";
      this.btnTransform.UseVisualStyleBackColor = true;
      this.btnTransform.Click += new System.EventHandler(this.btnTransform_Click);
      // 
      // txtEulerCheck
      // 
      this.txtEulerCheck.Location = new System.Drawing.Point(17, 118);
      this.txtEulerCheck.Multiline = true;
      this.txtEulerCheck.Name = "txtEulerCheck";
      this.txtEulerCheck.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      this.txtEulerCheck.Size = new System.Drawing.Size(407, 704);
      this.txtEulerCheck.TabIndex = 7;
      // 
      // btnNullComp
      // 
      this.btnNullComp.Location = new System.Drawing.Point(582, 624);
      this.btnNullComp.Name = "btnNullComp";
      this.btnNullComp.Size = new System.Drawing.Size(203, 23);
      this.btnNullComp.TabIndex = 14;
      this.btnNullComp.Text = "Null CNC Comp";
      this.btnNullComp.UseVisualStyleBackColor = true;
      this.btnNullComp.Click += new System.EventHandler(this.btnNullComp_Click);
      // 
      // btnPasteInPointsToTransform
      // 
      this.btnPasteInPointsToTransform.Location = new System.Drawing.Point(12, 12);
      this.btnPasteInPointsToTransform.Name = "btnPasteInPointsToTransform";
      this.btnPasteInPointsToTransform.Size = new System.Drawing.Size(198, 77);
      this.btnPasteInPointsToTransform.TabIndex = 15;
      this.btnPasteInPointsToTransform.Text = "Paste In Points (INCHES)";
      this.toolTip1.SetToolTip(this.btnPasteInPointsToTransform, "INCHES In order \"Xnom Ynom Znom Upos Xmeas  Ymeas Zmeas\".  Any number of sets is " +
              "possible, copy them out of Excel and tap this button to paste.  Input must be IN" +
              "CHES");
      this.btnPasteInPointsToTransform.UseVisualStyleBackColor = true;
      this.btnPasteInPointsToTransform.Click += new System.EventHandler(this.btnPasteInPointsToTransform_Click);
      // 
      // btnPastePoints
      // 
      this.btnPastePoints.Location = new System.Drawing.Point(6, 27);
      this.btnPastePoints.Name = "btnPastePoints";
      this.btnPastePoints.Size = new System.Drawing.Size(200, 29);
      this.btnPastePoints.TabIndex = 18;
      this.btnPastePoints.Text = "Paste In Points To Transform";
      this.toolTip1.SetToolTip(this.btnPastePoints, "Any number of sets is possible, copy them out of Excel and tap this button to pas" +
              "te. In order X,Y,Z,Upos");
      this.btnPastePoints.UseVisualStyleBackColor = true;
      this.btnPastePoints.Click += new System.EventHandler(this.btnPastePoints_Click);
      // 
      // btnPasteToReverse
      // 
      this.btnPasteToReverse.Location = new System.Drawing.Point(212, 27);
      this.btnPasteToReverse.Name = "btnPasteToReverse";
      this.btnPasteToReverse.Size = new System.Drawing.Size(200, 29);
      this.btnPasteToReverse.TabIndex = 21;
      this.btnPasteToReverse.Text = "Paste In Points To Reverse Transform";
      this.toolTip1.SetToolTip(this.btnPasteToReverse, "Any number of sets is possible, copy them out of Excel and tap this button to pas" +
              "te. In order X,Y,Z,Upos");
      this.btnPasteToReverse.UseVisualStyleBackColor = true;
      this.btnPasteToReverse.Click += new System.EventHandler(this.btnPasteToReverse_Click);
      // 
      // btnVerifyCompOnCNC
      // 
      this.btnVerifyCompOnCNC.Location = new System.Drawing.Point(226, 49);
      this.btnVerifyCompOnCNC.Name = "btnVerifyCompOnCNC";
      this.btnVerifyCompOnCNC.Size = new System.Drawing.Size(198, 29);
      this.btnVerifyCompOnCNC.TabIndex = 25;
      this.btnVerifyCompOnCNC.Text = "Verify Transform On CNC";
      this.toolTip1.SetToolTip(this.btnVerifyCompOnCNC, "Verifies that last transform calculated is on the CNC");
      this.btnVerifyCompOnCNC.UseVisualStyleBackColor = true;
      this.btnVerifyCompOnCNC.Click += new System.EventHandler(this.btnVerifyCompOnCNC_Click);
      // 
      // groupBox3
      // 
      this.groupBox3.Controls.Add(this.groupBox2);
      this.groupBox3.Controls.Add(this.btnPasteToReverse);
      this.groupBox3.Controls.Add(this.btnGetaCSVCopy);
      this.groupBox3.Controls.Add(this.txtPointsTransformed);
      this.groupBox3.Controls.Add(this.btnPastePoints);
      this.groupBox3.Location = new System.Drawing.Point(576, 12);
      this.groupBox3.Name = "groupBox3";
      this.groupBox3.Size = new System.Drawing.Size(619, 560);
      this.groupBox3.TabIndex = 18;
      this.groupBox3.TabStop = false;
      this.groupBox3.Text = "Transform Points";
      // 
      // groupBox2
      // 
      this.groupBox2.Controls.Add(this.btnCopySpinToFRC);
      this.groupBox2.Controls.Add(this.btnCopyEulerMatrix);
      this.groupBox2.Controls.Add(this.btnCopyEuler);
      this.groupBox2.Controls.Add(this.label4);
      this.groupBox2.Controls.Add(this.textBox5);
      this.groupBox2.Controls.Add(this.label2);
      this.groupBox2.Controls.Add(this.textBox2);
      this.groupBox2.Location = new System.Drawing.Point(325, 73);
      this.groupBox2.Name = "groupBox2";
      this.groupBox2.Size = new System.Drawing.Size(276, 317);
      this.groupBox2.TabIndex = 22;
      this.groupBox2.TabStop = false;
      this.groupBox2.Text = "Euler";
      // 
      // btnCopySpinToFRC
      // 
      this.btnCopySpinToFRC.Location = new System.Drawing.Point(129, 62);
      this.btnCopySpinToFRC.Name = "btnCopySpinToFRC";
      this.btnCopySpinToFRC.Size = new System.Drawing.Size(129, 23);
      this.btnCopySpinToFRC.TabIndex = 15;
      this.btnCopySpinToFRC.Text = "Copy Spin To FRC";
      this.btnCopySpinToFRC.UseVisualStyleBackColor = true;
      // 
      // btnCopyEulerMatrix
      // 
      this.btnCopyEulerMatrix.Enabled = false;
      this.btnCopyEulerMatrix.Location = new System.Drawing.Point(129, 114);
      this.btnCopyEulerMatrix.Name = "btnCopyEulerMatrix";
      this.btnCopyEulerMatrix.Size = new System.Drawing.Size(75, 36);
      this.btnCopyEulerMatrix.TabIndex = 14;
      this.btnCopyEulerMatrix.Text = "(INOP) Copy Euler Matrix";
      this.btnCopyEulerMatrix.UseVisualStyleBackColor = true;
      // 
      // btnCopyEuler
      // 
      this.btnCopyEuler.Location = new System.Drawing.Point(129, 35);
      this.btnCopyEuler.Name = "btnCopyEuler";
      this.btnCopyEuler.Size = new System.Drawing.Size(129, 23);
      this.btnCopyEuler.TabIndex = 13;
      this.btnCopyEuler.Text = "Copy Mandrel To Spin";
      this.btnCopyEuler.UseVisualStyleBackColor = true;
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(6, 227);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(139, 13);
      this.label4.TabIndex = 12;
      this.label4.Text = "Inverse of Orientation Matrix";
      // 
      // textBox5
      // 
      this.textBox5.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.textBox5.Location = new System.Drawing.Point(9, 243);
      this.textBox5.Multiline = true;
      this.textBox5.Name = "textBox5";
      this.textBox5.Size = new System.Drawing.Size(249, 56);
      this.textBox5.TabIndex = 11;
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(6, 18);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(128, 13);
      this.label2.TabIndex = 6;
      this.label2.Text = "Arguments CB uses these";
      // 
      // textBox2
      // 
      this.textBox2.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.textBox2.Location = new System.Drawing.Point(9, 34);
      this.textBox2.Multiline = true;
      this.textBox2.Name = "textBox2";
      this.textBox2.Size = new System.Drawing.Size(113, 190);
      this.textBox2.TabIndex = 5;
      // 
      // btnGetaCSVCopy
      // 
      this.btnGetaCSVCopy.Location = new System.Drawing.Point(6, 508);
      this.btnGetaCSVCopy.Name = "btnGetaCSVCopy";
      this.btnGetaCSVCopy.Size = new System.Drawing.Size(331, 31);
      this.btnGetaCSVCopy.TabIndex = 20;
      this.btnGetaCSVCopy.Text = "Get a csv copy";
      this.btnGetaCSVCopy.UseVisualStyleBackColor = true;
      this.btnGetaCSVCopy.Click += new System.EventHandler(this.btnGetaCSVCopy_Click);
      // 
      // txtPointsTransformed
      // 
      this.txtPointsTransformed.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.txtPointsTransformed.Location = new System.Drawing.Point(6, 62);
      this.txtPointsTransformed.Multiline = true;
      this.txtPointsTransformed.Name = "txtPointsTransformed";
      this.txtPointsTransformed.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      this.txtPointsTransformed.Size = new System.Drawing.Size(286, 440);
      this.txtPointsTransformed.TabIndex = 19;
      // 
      // btnCompToCNC
      // 
      this.btnCompToCNC.Location = new System.Drawing.Point(226, 12);
      this.btnCompToCNC.Name = "btnCompToCNC";
      this.btnCompToCNC.Size = new System.Drawing.Size(198, 31);
      this.btnCompToCNC.TabIndex = 23;
      this.btnCompToCNC.Text = "Transform To CNC";
      this.btnCompToCNC.UseVisualStyleBackColor = true;
      this.btnCompToCNC.Click += new System.EventHandler(this.btnCompToCNC_Click);
      // 
      // chkShowAdvanced
      // 
      this.chkShowAdvanced.AutoSize = true;
      this.chkShowAdvanced.Location = new System.Drawing.Point(17, 95);
      this.chkShowAdvanced.Name = "chkShowAdvanced";
      this.chkShowAdvanced.Size = new System.Drawing.Size(105, 17);
      this.chkShowAdvanced.TabIndex = 24;
      this.chkShowAdvanced.Text = "Show Advanced";
      this.chkShowAdvanced.UseVisualStyleBackColor = true;
      this.chkShowAdvanced.CheckedChanged += new System.EventHandler(this.chkShowAdvanced_CheckedChanged);
      // 
      // chkStupid
      // 
      this.chkStupid.AutoSize = true;
      this.chkStupid.Location = new System.Drawing.Point(582, 665);
      this.chkStupid.Name = "chkStupid";
      this.chkStupid.Size = new System.Drawing.Size(110, 17);
      this.chkStupid.TabIndex = 26;
      this.chkStupid.Text = "Single6DOFXform";
      this.chkStupid.UseVisualStyleBackColor = true;
      // 
      // checked_points_list
      // 
      this.checked_points_list.CheckOnClick = true;
      this.checked_points_list.FormattingEnabled = true;
      this.checked_points_list.Location = new System.Drawing.Point(799, 593);
      this.checked_points_list.Name = "checked_points_list";
      this.checked_points_list.Size = new System.Drawing.Size(286, 229);
      this.checked_points_list.TabIndex = 27;
      this.checked_points_list.MouseUp += new System.Windows.Forms.MouseEventHandler(this.checked_points_list_MouseUp);
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(796, 577);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(98, 13);
      this.label1.TabIndex = 28;
      this.label1.Text = "Points In Transform";
      // 
      // frmBarrelTransformer
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(1230, 857);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.checked_points_list);
      this.Controls.Add(this.chkStupid);
      this.Controls.Add(this.btnVerifyCompOnCNC);
      this.Controls.Add(this.chkShowAdvanced);
      this.Controls.Add(this.btnCompToCNC);
      this.Controls.Add(this.groupBox3);
      this.Controls.Add(this.btnPasteInPointsToTransform);
      this.Controls.Add(this.btnNullComp);
      this.Controls.Add(this.txtEulerCheck);
      this.Controls.Add(this.btnTransform);
      this.Name = "frmBarrelTransformer";
      this.Text = "Electroimpact Barrel Transformer";
      this.Load += new System.EventHandler(this.Form1_Load);
      this.groupBox3.ResumeLayout(false);
      this.groupBox3.PerformLayout();
      this.groupBox2.ResumeLayout(false);
      this.groupBox2.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button btnTransform;
    private System.Windows.Forms.TextBox txtEulerCheck;
    private System.Windows.Forms.Button btnNullComp;
    private System.Windows.Forms.Button btnPasteInPointsToTransform;
    private System.Windows.Forms.ToolTip toolTip1;
    private System.Windows.Forms.GroupBox groupBox3;
    private System.Windows.Forms.TextBox txtPointsTransformed;
    private System.Windows.Forms.Button btnPastePoints;
    private System.Windows.Forms.Button btnGetaCSVCopy;
    private System.Windows.Forms.Button btnCompToCNC;
    private System.Windows.Forms.Button btnPasteToReverse;
    private System.Windows.Forms.CheckBox chkShowAdvanced;
    private System.Windows.Forms.GroupBox groupBox2;
    private System.Windows.Forms.Button btnCopySpinToFRC;
    private System.Windows.Forms.Button btnCopyEulerMatrix;
    private System.Windows.Forms.Button btnCopyEuler;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.TextBox textBox5;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.TextBox textBox2;
    private System.Windows.Forms.Button btnVerifyCompOnCNC;
    private System.Windows.Forms.CheckBox chkStupid;
    private System.Windows.Forms.CheckedListBox checked_points_list;
    private System.Windows.Forms.Label label1;
  }
}

