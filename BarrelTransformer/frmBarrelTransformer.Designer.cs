namespace Electroimpact.Transformer
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmBarrelTransformer));
      this.btnTransform = new System.Windows.Forms.Button();
      this.txtEulerCheck = new System.Windows.Forms.TextBox();
      this.btnNullComp = new System.Windows.Forms.Button();
      this.btnPasteInPointsToTransform = new System.Windows.Forms.Button();
      this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
      this.btnPastePoints = new System.Windows.Forms.Button();
      this.btnPasteToReverse = new System.Windows.Forms.Button();
      this.btnVerifyCompOnCNC = new System.Windows.Forms.Button();
      this.btnSelectFile = new System.Windows.Forms.Button();
      this.groupBox3 = new System.Windows.Forms.GroupBox();
      this.btnGetaCSVCopy = new System.Windows.Forms.Button();
      this.txtPointsTransformed = new System.Windows.Forms.TextBox();
      this.btnCompToCNC = new System.Windows.Forms.Button();
      this.chkShowAdvanced = new System.Windows.Forms.CheckBox();
      this.chkStupid = new System.Windows.Forms.CheckBox();
      this.checked_points_list = new System.Windows.Forms.CheckedListBox();
      this.lblPointsInXform = new System.Windows.Forms.Label();
      this.menuStrip1 = new System.Windows.Forms.MenuStrip();
      this.menustrip_options = new System.Windows.Forms.ToolStripMenuItem();
      this.menustrip_Barrel_Transform = new System.Windows.Forms.ToolStripMenuItem();
      this.menustrip_useXuFunction_Ax = new System.Windows.Forms.ToolStripMenuItem();
      this.menustrip_v_argument_option = new System.Windows.Forms.ToolStripMenuItem();
      this.menustrip_multiple_mandrel_option = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
      this.menustrip_mm_data = new System.Windows.Forms.ToolStripMenuItem();
      this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.readTransformsOnCNCToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.groupBox2 = new System.Windows.Forms.GroupBox();
      this.lblEulerBoxUnits = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.txtbox_transform_args = new System.Windows.Forms.TextBox();
      this.grpBx_ManualTrans = new System.Windows.Forms.GroupBox();
      this.lblManualUnits = new System.Windows.Forms.Label();
      this.btnSetManualTform = new System.Windows.Forms.Button();
      this.btn_Send_Manual_Trans = new System.Windows.Forms.Button();
      this.label7 = new System.Windows.Forms.Label();
      this.txtBx_Manual_rZ = new System.Windows.Forms.TextBox();
      this.label6 = new System.Windows.Forms.Label();
      this.txtBx_Manual_rY = new System.Windows.Forms.TextBox();
      this.label5 = new System.Windows.Forms.Label();
      this.txtBx_Manual_rX = new System.Windows.Forms.TextBox();
      this.label4 = new System.Windows.Forms.Label();
      this.txtBx_Manual_Z = new System.Windows.Forms.TextBox();
      this.label3 = new System.Windows.Forms.Label();
      this.txtBx_Manual_Y = new System.Windows.Forms.TextBox();
      this.label1 = new System.Windows.Forms.Label();
      this.txtBx_Manual_X = new System.Windows.Forms.TextBox();
      this.label8 = new System.Windows.Forms.Label();
      this.lblDetails = new System.Windows.Forms.Label();
      this.txtBxSimpleResults = new System.Windows.Forms.TextBox();
      this.lblTolAveErr = new System.Windows.Forms.Label();
      this.lblTolMaxErr = new System.Windows.Forms.Label();
      this.lblTolUnitsAve = new System.Windows.Forms.Label();
      this.lblTolUnitsMax = new System.Windows.Forms.Label();
      this.lblAveErrMax = new System.Windows.Forms.Label();
      this.lblMaxErrMax = new System.Windows.Forms.Label();
      this.cb_RemoveErrPts = new System.Windows.Forms.CheckBox();
      this.txtFBOT = new System.Windows.Forms.TextBox();
      this.label9 = new System.Windows.Forms.Label();
      this.groupBox3.SuspendLayout();
      this.menuStrip1.SuspendLayout();
      this.groupBox2.SuspendLayout();
      this.grpBx_ManualTrans.SuspendLayout();
      this.SuspendLayout();
      // 
      // btnTransform
      // 
      this.btnTransform.Location = new System.Drawing.Point(799, 486);
      this.btnTransform.Name = "btnTransform";
      this.btnTransform.Size = new System.Drawing.Size(203, 25);
      this.btnTransform.TabIndex = 0;
      this.btnTransform.Text = "Transform";
      this.btnTransform.UseVisualStyleBackColor = true;
      this.btnTransform.Click += new System.EventHandler(this.btnTransform_Click);
      // 
      // txtEulerCheck
      // 
      this.txtEulerCheck.Location = new System.Drawing.Point(421, 89);
      this.txtEulerCheck.Multiline = true;
      this.txtEulerCheck.Name = "txtEulerCheck";
      this.txtEulerCheck.ReadOnly = true;
      this.txtEulerCheck.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      this.txtEulerCheck.Size = new System.Drawing.Size(369, 719);
      this.txtEulerCheck.TabIndex = 7;
      // 
      // btnNullComp
      // 
      this.btnNullComp.Location = new System.Drawing.Point(799, 514);
      this.btnNullComp.Name = "btnNullComp";
      this.btnNullComp.Size = new System.Drawing.Size(203, 23);
      this.btnNullComp.TabIndex = 14;
      this.btnNullComp.Text = "Null CNC Comp";
      this.btnNullComp.UseVisualStyleBackColor = true;
      this.btnNullComp.Click += new System.EventHandler(this.btnNullComp_Click);
      // 
      // btnPasteInPointsToTransform
      // 
      this.btnPasteInPointsToTransform.Location = new System.Drawing.Point(7, 29);
      this.btnPasteInPointsToTransform.Name = "btnPasteInPointsToTransform";
      this.btnPasteInPointsToTransform.Size = new System.Drawing.Size(185, 42);
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
      this.btnVerifyCompOnCNC.Location = new System.Drawing.Point(221, 69);
      this.btnVerifyCompOnCNC.Name = "btnVerifyCompOnCNC";
      this.btnVerifyCompOnCNC.Size = new System.Drawing.Size(168, 29);
      this.btnVerifyCompOnCNC.TabIndex = 25;
      this.btnVerifyCompOnCNC.Text = "Verify Transform On CNC";
      this.toolTip1.SetToolTip(this.btnVerifyCompOnCNC, "Verifies that last transform calculated is on the CNC");
      this.btnVerifyCompOnCNC.UseVisualStyleBackColor = true;
      this.btnVerifyCompOnCNC.Click += new System.EventHandler(this.btnVerifyCompOnCNC_Click);
      // 
      // btnSelectFile
      // 
      this.btnSelectFile.Location = new System.Drawing.Point(7, 93);
      this.btnSelectFile.Name = "btnSelectFile";
      this.btnSelectFile.Size = new System.Drawing.Size(185, 42);
      this.btnSelectFile.TabIndex = 34;
      this.btnSelectFile.Text = "Select File (INCHES)";
      this.toolTip1.SetToolTip(this.btnSelectFile, "INCHES In order \"Xnom Ynom Znom Upos Xmeas  Ymeas Zmeas\".  Any number of sets is " +
        "possible, copy them out of Excel and tap this button to paste.  Input must be IN" +
        "CHES");
      this.btnSelectFile.UseVisualStyleBackColor = true;
      this.btnSelectFile.Click += new System.EventHandler(this.btnSelectFile_Click);
      // 
      // groupBox3
      // 
      this.groupBox3.Controls.Add(this.btnPasteToReverse);
      this.groupBox3.Controls.Add(this.btnGetaCSVCopy);
      this.groupBox3.Controls.Add(this.txtPointsTransformed);
      this.groupBox3.Controls.Add(this.btnPastePoints);
      this.groupBox3.Location = new System.Drawing.Point(796, 27);
      this.groupBox3.Name = "groupBox3";
      this.groupBox3.Size = new System.Drawing.Size(619, 453);
      this.groupBox3.TabIndex = 18;
      this.groupBox3.TabStop = false;
      this.groupBox3.Text = "Transform Points";
      // 
      // btnGetaCSVCopy
      // 
      this.btnGetaCSVCopy.Location = new System.Drawing.Point(158, 420);
      this.btnGetaCSVCopy.Name = "btnGetaCSVCopy";
      this.btnGetaCSVCopy.Size = new System.Drawing.Size(276, 31);
      this.btnGetaCSVCopy.TabIndex = 20;
      this.btnGetaCSVCopy.Text = "Get a tab delimited copy";
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
      this.txtPointsTransformed.Size = new System.Drawing.Size(607, 352);
      this.txtPointsTransformed.TabIndex = 19;
      // 
      // btnCompToCNC
      // 
      this.btnCompToCNC.Location = new System.Drawing.Point(221, 30);
      this.btnCompToCNC.Name = "btnCompToCNC";
      this.btnCompToCNC.Size = new System.Drawing.Size(168, 31);
      this.btnCompToCNC.TabIndex = 23;
      this.btnCompToCNC.Text = "Transform To CNC";
      this.btnCompToCNC.UseVisualStyleBackColor = true;
      this.btnCompToCNC.Click += new System.EventHandler(this.btnCompToCNC_Click);
      // 
      // chkShowAdvanced
      // 
      this.chkShowAdvanced.AutoSize = true;
      this.chkShowAdvanced.Location = new System.Drawing.Point(221, 7);
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
      this.chkStupid.Location = new System.Drawing.Point(799, 543);
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
      this.checked_points_list.Location = new System.Drawing.Point(799, 579);
      this.checked_points_list.Name = "checked_points_list";
      this.checked_points_list.Size = new System.Drawing.Size(224, 229);
      this.checked_points_list.TabIndex = 27;
      this.checked_points_list.MouseUp += new System.Windows.Forms.MouseEventHandler(this.checked_points_list_MouseUp);
      // 
      // lblPointsInXform
      // 
      this.lblPointsInXform.AutoSize = true;
      this.lblPointsInXform.Location = new System.Drawing.Point(796, 563);
      this.lblPointsInXform.Name = "lblPointsInXform";
      this.lblPointsInXform.Size = new System.Drawing.Size(98, 13);
      this.lblPointsInXform.TabIndex = 28;
      this.lblPointsInXform.Text = "Points In Transform";
      // 
      // menuStrip1
      // 
      this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menustrip_options,
            this.toolsToolStripMenuItem});
      this.menuStrip1.Location = new System.Drawing.Point(0, 0);
      this.menuStrip1.Name = "menuStrip1";
      this.menuStrip1.Size = new System.Drawing.Size(1676, 24);
      this.menuStrip1.TabIndex = 29;
      this.menuStrip1.Text = "menuStrip1";
      // 
      // menustrip_options
      // 
      this.menustrip_options.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menustrip_Barrel_Transform,
            this.menustrip_useXuFunction_Ax,
            this.menustrip_v_argument_option,
            this.menustrip_multiple_mandrel_option,
            this.toolStripSeparator1,
            this.menustrip_mm_data});
      this.menustrip_options.Name = "menustrip_options";
      this.menustrip_options.Size = new System.Drawing.Size(61, 20);
      this.menustrip_options.Text = "Options";
      // 
      // menustrip_Barrel_Transform
      // 
      this.menustrip_Barrel_Transform.Name = "menustrip_Barrel_Transform";
      this.menustrip_Barrel_Transform.Size = new System.Drawing.Size(197, 22);
      this.menustrip_Barrel_Transform.Text = "Use Barrel Transform";
      this.menustrip_Barrel_Transform.Click += new System.EventHandler(this.menustrip_Barrel_Transform_Click);
      // 
      // menustrip_useXuFunction_Ax
      // 
      this.menustrip_useXuFunction_Ax.Name = "menustrip_useXuFunction_Ax";
      this.menustrip_useXuFunction_Ax.Size = new System.Drawing.Size(197, 22);
      this.menustrip_useXuFunction_Ax.Text = "Use X(u) function (A_x)";
      this.menustrip_useXuFunction_Ax.Click += new System.EventHandler(this.useXuFunctionAxToolStripMenuItem_Click);
      // 
      // menustrip_v_argument_option
      // 
      this.menustrip_v_argument_option.Name = "menustrip_v_argument_option";
      this.menustrip_v_argument_option.Size = new System.Drawing.Size(197, 22);
      this.menustrip_v_argument_option.Text = "Use U and V arguments";
      this.menustrip_v_argument_option.Click += new System.EventHandler(this.menustrip_v_argument_option_Click);
      // 
      // menustrip_multiple_mandrel_option
      // 
      this.menustrip_multiple_mandrel_option.Name = "menustrip_multiple_mandrel_option";
      this.menustrip_multiple_mandrel_option.Size = new System.Drawing.Size(197, 22);
      this.menustrip_multiple_mandrel_option.Text = "Multiple Mandrels";
      this.menustrip_multiple_mandrel_option.Click += new System.EventHandler(this.menustrip_multiple_mandrel_option_Click);
      // 
      // toolStripSeparator1
      // 
      this.toolStripSeparator1.Name = "toolStripSeparator1";
      this.toolStripSeparator1.Size = new System.Drawing.Size(194, 6);
      // 
      // menustrip_mm_data
      // 
      this.menustrip_mm_data.Name = "menustrip_mm_data";
      this.menustrip_mm_data.Size = new System.Drawing.Size(197, 22);
      this.menustrip_mm_data.Text = "Use mm data";
      this.menustrip_mm_data.Click += new System.EventHandler(this.menustrip_mm_data_Click);
      // 
      // toolsToolStripMenuItem
      // 
      this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.readTransformsOnCNCToolStripMenuItem,
            this.aboutToolStripMenuItem});
      this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
      this.toolsToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
      this.toolsToolStripMenuItem.Text = "Tools";
      // 
      // readTransformsOnCNCToolStripMenuItem
      // 
      this.readTransformsOnCNCToolStripMenuItem.Name = "readTransformsOnCNCToolStripMenuItem";
      this.readTransformsOnCNCToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
      this.readTransformsOnCNCToolStripMenuItem.Text = "Export Transforms to File";
      this.readTransformsOnCNCToolStripMenuItem.Click += new System.EventHandler(this.readTransformsOnCNCToolStripMenuItem_Click);
      // 
      // aboutToolStripMenuItem
      // 
      this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
      this.aboutToolStripMenuItem.Size = new System.Drawing.Size(204, 22);
      this.aboutToolStripMenuItem.Text = "About";
      this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
      // 
      // groupBox2
      // 
      this.groupBox2.Controls.Add(this.lblEulerBoxUnits);
      this.groupBox2.Controls.Add(this.label2);
      this.groupBox2.Controls.Add(this.txtbox_transform_args);
      this.groupBox2.Location = new System.Drawing.Point(1059, 506);
      this.groupBox2.Name = "groupBox2";
      this.groupBox2.Size = new System.Drawing.Size(174, 301);
      this.groupBox2.TabIndex = 30;
      this.groupBox2.TabStop = false;
      this.groupBox2.Text = "Euler";
      // 
      // lblEulerBoxUnits
      // 
      this.lblEulerBoxUnits.AutoSize = true;
      this.lblEulerBoxUnits.Location = new System.Drawing.Point(6, 49);
      this.lblEulerBoxUnits.Name = "lblEulerBoxUnits";
      this.lblEulerBoxUnits.Size = new System.Drawing.Size(38, 13);
      this.lblEulerBoxUnits.TabIndex = 43;
      this.lblEulerBoxUnits.Text = "inches";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(6, 18);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(68, 26);
      this.label2.TabIndex = 6;
      this.label2.Text = "Part To Spin\r\nSpin To FRC\r\n";
      // 
      // txtbox_transform_args
      // 
      this.txtbox_transform_args.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.txtbox_transform_args.Location = new System.Drawing.Point(6, 65);
      this.txtbox_transform_args.Multiline = true;
      this.txtbox_transform_args.Name = "txtbox_transform_args";
      this.txtbox_transform_args.Size = new System.Drawing.Size(150, 198);
      this.txtbox_transform_args.TabIndex = 5;
      // 
      // grpBx_ManualTrans
      // 
      this.grpBx_ManualTrans.Controls.Add(this.lblManualUnits);
      this.grpBx_ManualTrans.Controls.Add(this.btnSetManualTform);
      this.grpBx_ManualTrans.Controls.Add(this.btn_Send_Manual_Trans);
      this.grpBx_ManualTrans.Controls.Add(this.label7);
      this.grpBx_ManualTrans.Controls.Add(this.txtBx_Manual_rZ);
      this.grpBx_ManualTrans.Controls.Add(this.label6);
      this.grpBx_ManualTrans.Controls.Add(this.txtBx_Manual_rY);
      this.grpBx_ManualTrans.Controls.Add(this.label5);
      this.grpBx_ManualTrans.Controls.Add(this.txtBx_Manual_rX);
      this.grpBx_ManualTrans.Controls.Add(this.label4);
      this.grpBx_ManualTrans.Controls.Add(this.txtBx_Manual_Z);
      this.grpBx_ManualTrans.Controls.Add(this.label3);
      this.grpBx_ManualTrans.Controls.Add(this.txtBx_Manual_Y);
      this.grpBx_ManualTrans.Controls.Add(this.label1);
      this.grpBx_ManualTrans.Controls.Add(this.txtBx_Manual_X);
      this.grpBx_ManualTrans.Location = new System.Drawing.Point(1239, 506);
      this.grpBx_ManualTrans.Name = "grpBx_ManualTrans";
      this.grpBx_ManualTrans.Size = new System.Drawing.Size(173, 301);
      this.grpBx_ManualTrans.TabIndex = 31;
      this.grpBx_ManualTrans.TabStop = false;
      this.grpBx_ManualTrans.Text = "Manual Transform (non barrel)";
      // 
      // lblManualUnits
      // 
      this.lblManualUnits.AutoSize = true;
      this.lblManualUnits.Location = new System.Drawing.Point(6, 18);
      this.lblManualUnits.Name = "lblManualUnits";
      this.lblManualUnits.Size = new System.Drawing.Size(38, 13);
      this.lblManualUnits.TabIndex = 47;
      this.lblManualUnits.Text = "inches";
      // 
      // btnSetManualTform
      // 
      this.btnSetManualTform.Location = new System.Drawing.Point(6, 221);
      this.btnSetManualTform.Name = "btnSetManualTform";
      this.btnSetManualTform.Size = new System.Drawing.Size(150, 34);
      this.btnSetManualTform.TabIndex = 46;
      this.btnSetManualTform.Text = "Set Manual Transform";
      this.btnSetManualTform.UseVisualStyleBackColor = true;
      this.btnSetManualTform.Click += new System.EventHandler(this.btnSetManualTform_Click);
      // 
      // btn_Send_Manual_Trans
      // 
      this.btn_Send_Manual_Trans.Location = new System.Drawing.Point(6, 261);
      this.btn_Send_Manual_Trans.Name = "btn_Send_Manual_Trans";
      this.btn_Send_Manual_Trans.Size = new System.Drawing.Size(150, 34);
      this.btn_Send_Manual_Trans.TabIndex = 45;
      this.btn_Send_Manual_Trans.Text = "Send Manual Transform To CNC";
      this.btn_Send_Manual_Trans.UseVisualStyleBackColor = true;
      this.btn_Send_Manual_Trans.Click += new System.EventHandler(this.btn_Send_Manual_Trans_Click);
      // 
      // label7
      // 
      this.label7.AutoSize = true;
      this.label7.Location = new System.Drawing.Point(112, 190);
      this.label7.Name = "label7";
      this.label7.Size = new System.Drawing.Size(17, 13);
      this.label7.TabIndex = 43;
      this.label7.Text = "rZ";
      // 
      // txtBx_Manual_rZ
      // 
      this.txtBx_Manual_rZ.Location = new System.Drawing.Point(6, 187);
      this.txtBx_Manual_rZ.Name = "txtBx_Manual_rZ";
      this.txtBx_Manual_rZ.Size = new System.Drawing.Size(100, 20);
      this.txtBx_Manual_rZ.TabIndex = 42;
      // 
      // label6
      // 
      this.label6.AutoSize = true;
      this.label6.Location = new System.Drawing.Point(112, 164);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(17, 13);
      this.label6.TabIndex = 41;
      this.label6.Text = "rY";
      // 
      // txtBx_Manual_rY
      // 
      this.txtBx_Manual_rY.Location = new System.Drawing.Point(6, 161);
      this.txtBx_Manual_rY.Name = "txtBx_Manual_rY";
      this.txtBx_Manual_rY.Size = new System.Drawing.Size(100, 20);
      this.txtBx_Manual_rY.TabIndex = 40;
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Location = new System.Drawing.Point(112, 138);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(17, 13);
      this.label5.TabIndex = 39;
      this.label5.Text = "rX";
      // 
      // txtBx_Manual_rX
      // 
      this.txtBx_Manual_rX.Location = new System.Drawing.Point(6, 135);
      this.txtBx_Manual_rX.Name = "txtBx_Manual_rX";
      this.txtBx_Manual_rX.Size = new System.Drawing.Size(100, 20);
      this.txtBx_Manual_rX.TabIndex = 38;
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(112, 109);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(14, 13);
      this.label4.TabIndex = 37;
      this.label4.Text = "Z";
      // 
      // txtBx_Manual_Z
      // 
      this.txtBx_Manual_Z.Location = new System.Drawing.Point(6, 109);
      this.txtBx_Manual_Z.Name = "txtBx_Manual_Z";
      this.txtBx_Manual_Z.Size = new System.Drawing.Size(100, 20);
      this.txtBx_Manual_Z.TabIndex = 36;
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(112, 86);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(14, 13);
      this.label3.TabIndex = 35;
      this.label3.Text = "Y";
      // 
      // txtBx_Manual_Y
      // 
      this.txtBx_Manual_Y.Location = new System.Drawing.Point(6, 83);
      this.txtBx_Manual_Y.Name = "txtBx_Manual_Y";
      this.txtBx_Manual_Y.Size = new System.Drawing.Size(100, 20);
      this.txtBx_Manual_Y.TabIndex = 34;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(112, 60);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(14, 13);
      this.label1.TabIndex = 33;
      this.label1.Text = "X";
      // 
      // txtBx_Manual_X
      // 
      this.txtBx_Manual_X.Location = new System.Drawing.Point(6, 57);
      this.txtBx_Manual_X.Name = "txtBx_Manual_X";
      this.txtBx_Manual_X.Size = new System.Drawing.Size(100, 20);
      this.txtBx_Manual_X.TabIndex = 32;
      // 
      // label8
      // 
      this.label8.AutoSize = true;
      this.label8.Location = new System.Drawing.Point(83, 77);
      this.label8.Name = "label8";
      this.label8.Size = new System.Drawing.Size(23, 13);
      this.label8.TabIndex = 35;
      this.label8.Text = "OR";
      // 
      // lblDetails
      // 
      this.lblDetails.AutoSize = true;
      this.lblDetails.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblDetails.Location = new System.Drawing.Point(418, 54);
      this.lblDetails.Name = "lblDetails";
      this.lblDetails.Size = new System.Drawing.Size(80, 20);
      this.lblDetails.TabIndex = 36;
      this.lblDetails.Text = "Fit Details";
      // 
      // txtBxSimpleResults
      // 
      this.txtBxSimpleResults.Location = new System.Drawing.Point(7, 219);
      this.txtBxSimpleResults.Multiline = true;
      this.txtBxSimpleResults.Name = "txtBxSimpleResults";
      this.txtBxSimpleResults.ReadOnly = true;
      this.txtBxSimpleResults.Size = new System.Drawing.Size(382, 169);
      this.txtBxSimpleResults.TabIndex = 37;
      // 
      // lblTolAveErr
      // 
      this.lblTolAveErr.AutoSize = true;
      this.lblTolAveErr.Location = new System.Drawing.Point(7, 145);
      this.lblTolAveErr.Name = "lblTolAveErr";
      this.lblTolAveErr.Size = new System.Drawing.Size(147, 13);
      this.lblTolAveErr.TabIndex = 38;
      this.lblTolAveErr.Text = "Tolerance For Average Error: ";
      // 
      // lblTolMaxErr
      // 
      this.lblTolMaxErr.AutoSize = true;
      this.lblTolMaxErr.Location = new System.Drawing.Point(7, 171);
      this.lblTolMaxErr.Name = "lblTolMaxErr";
      this.lblTolMaxErr.Size = new System.Drawing.Size(135, 13);
      this.lblTolMaxErr.TabIndex = 39;
      this.lblTolMaxErr.Text = "Tolerance For Worst Error: ";
      // 
      // lblTolUnitsAve
      // 
      this.lblTolUnitsAve.AutoSize = true;
      this.lblTolUnitsAve.Location = new System.Drawing.Point(218, 148);
      this.lblTolUnitsAve.Name = "lblTolUnitsAve";
      this.lblTolUnitsAve.Size = new System.Drawing.Size(38, 13);
      this.lblTolUnitsAve.TabIndex = 42;
      this.lblTolUnitsAve.Text = "inches";
      // 
      // lblTolUnitsMax
      // 
      this.lblTolUnitsMax.AutoSize = true;
      this.lblTolUnitsMax.Location = new System.Drawing.Point(218, 171);
      this.lblTolUnitsMax.Name = "lblTolUnitsMax";
      this.lblTolUnitsMax.Size = new System.Drawing.Size(38, 13);
      this.lblTolUnitsMax.TabIndex = 43;
      this.lblTolUnitsMax.Text = "inches";
      // 
      // lblAveErrMax
      // 
      this.lblAveErrMax.AutoSize = true;
      this.lblAveErrMax.Location = new System.Drawing.Point(160, 148);
      this.lblAveErrMax.Name = "lblAveErrMax";
      this.lblAveErrMax.Size = new System.Drawing.Size(34, 13);
      this.lblAveErrMax.TabIndex = 44;
      this.lblAveErrMax.Text = "0.060";
      // 
      // lblMaxErrMax
      // 
      this.lblMaxErrMax.AutoSize = true;
      this.lblMaxErrMax.Location = new System.Drawing.Point(160, 171);
      this.lblMaxErrMax.Name = "lblMaxErrMax";
      this.lblMaxErrMax.Size = new System.Drawing.Size(34, 13);
      this.lblMaxErrMax.TabIndex = 45;
      this.lblMaxErrMax.Text = "0.100";
      // 
      // cb_RemoveErrPts
      // 
      this.cb_RemoveErrPts.AutoSize = true;
      this.cb_RemoveErrPts.Location = new System.Drawing.Point(10, 196);
      this.cb_RemoveErrPts.Name = "cb_RemoveErrPts";
      this.cb_RemoveErrPts.Size = new System.Drawing.Size(246, 17);
      this.cb_RemoveErrPts.TabIndex = 46;
      this.cb_RemoveErrPts.Text = "Automatically Remove Points Out of Tolerance";
      this.cb_RemoveErrPts.UseVisualStyleBackColor = true;
      // 
      // txtFBOT
      // 
      this.txtFBOT.Location = new System.Drawing.Point(1418, 537);
      this.txtFBOT.Multiline = true;
      this.txtFBOT.Name = "txtFBOT";
      this.txtFBOT.Size = new System.Drawing.Size(100, 172);
      this.txtFBOT.TabIndex = 47;
      // 
      // label9
      // 
      this.label9.AutoSize = true;
      this.label9.Location = new System.Drawing.Point(1418, 521);
      this.label9.Name = "label9";
      this.label9.Size = new System.Drawing.Size(75, 13);
      this.label9.TabIndex = 48;
      this.label9.Text = "FANUC Robot";
      // 
      // frmBarrelTransformer
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(1676, 827);
      this.Controls.Add(this.label9);
      this.Controls.Add(this.txtFBOT);
      this.Controls.Add(this.cb_RemoveErrPts);
      this.Controls.Add(this.lblMaxErrMax);
      this.Controls.Add(this.lblAveErrMax);
      this.Controls.Add(this.lblTolUnitsMax);
      this.Controls.Add(this.lblTolUnitsAve);
      this.Controls.Add(this.lblTolMaxErr);
      this.Controls.Add(this.lblTolAveErr);
      this.Controls.Add(this.txtBxSimpleResults);
      this.Controls.Add(this.lblDetails);
      this.Controls.Add(this.label8);
      this.Controls.Add(this.btnSelectFile);
      this.Controls.Add(this.grpBx_ManualTrans);
      this.Controls.Add(this.groupBox2);
      this.Controls.Add(this.lblPointsInXform);
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
      this.Controls.Add(this.menuStrip1);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.MainMenuStrip = this.menuStrip1;
      this.Name = "frmBarrelTransformer";
      this.Text = "Electroimpact Barrel Transformer";
      this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmBarrelTransformer_FormClosed);
      this.LocationChanged += new System.EventHandler(this.frmBarrelTransformer_LocationChanged);
      this.groupBox3.ResumeLayout(false);
      this.groupBox3.PerformLayout();
      this.menuStrip1.ResumeLayout(false);
      this.menuStrip1.PerformLayout();
      this.groupBox2.ResumeLayout(false);
      this.groupBox2.PerformLayout();
      this.grpBx_ManualTrans.ResumeLayout(false);
      this.grpBx_ManualTrans.PerformLayout();
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
    private System.Windows.Forms.Button btnVerifyCompOnCNC;
    private System.Windows.Forms.CheckBox chkStupid;
    public System.Windows.Forms.CheckedListBox checked_points_list;
    private System.Windows.Forms.Label lblPointsInXform;
    private System.Windows.Forms.MenuStrip menuStrip1;
    private System.Windows.Forms.GroupBox groupBox2;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.TextBox txtbox_transform_args;
    private System.Windows.Forms.ToolStripMenuItem menustrip_options;
    private System.Windows.Forms.ToolStripMenuItem menustrip_v_argument_option;
    private System.Windows.Forms.ToolStripMenuItem menustrip_useXuFunction_Ax;
    private System.Windows.Forms.ToolStripMenuItem menustrip_Barrel_Transform;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
    private System.Windows.Forms.ToolStripMenuItem menustrip_mm_data;
    private System.Windows.Forms.ToolStripMenuItem menustrip_multiple_mandrel_option;
    private System.Windows.Forms.GroupBox grpBx_ManualTrans;
    private System.Windows.Forms.Button btn_Send_Manual_Trans;
    private System.Windows.Forms.Label label7;
    private System.Windows.Forms.TextBox txtBx_Manual_rZ;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.TextBox txtBx_Manual_rY;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.TextBox txtBx_Manual_rX;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.TextBox txtBx_Manual_Z;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.TextBox txtBx_Manual_Y;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.TextBox txtBx_Manual_X;
    private System.Windows.Forms.Button btnSetManualTform;
    
    private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem readTransformsOnCNCToolStripMenuItem;
    private System.Windows.Forms.Button btnSelectFile;
    private System.Windows.Forms.Label label8;
    private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
    private System.Windows.Forms.Label lblDetails;
    private System.Windows.Forms.TextBox txtBxSimpleResults;
    private System.Windows.Forms.Label lblTolAveErr;
    private System.Windows.Forms.Label lblTolMaxErr;
    private System.Windows.Forms.Label lblTolUnitsAve;
    private System.Windows.Forms.Label lblTolUnitsMax;
    private System.Windows.Forms.Label lblAveErrMax;
    private System.Windows.Forms.Label lblMaxErrMax;
    private System.Windows.Forms.Label lblEulerBoxUnits;
    private System.Windows.Forms.Label lblManualUnits;
		private System.Windows.Forms.CheckBox cb_RemoveErrPts;
    private System.Windows.Forms.TextBox txtFBOT;
    private System.Windows.Forms.Label label9;
  }
}

