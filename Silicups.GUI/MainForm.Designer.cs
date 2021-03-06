﻿namespace Silicups.GUI
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.splitContainerMain = new System.Windows.Forms.SplitContainer();
            this.splitContainerLeft = new System.Windows.Forms.SplitContainer();
            this.listBoxSolution = new Silicups.GUI.MyListBox();
            this.listBoxObs = new System.Windows.Forms.CheckedListBox();
            this.graph = new Silicups.GUI.Graph(this.components);
            this.panelTools = new System.Windows.Forms.Panel();
            this.radioButtonOC = new System.Windows.Forms.RadioButton();
            this.buttonAddMinimum = new System.Windows.Forms.Button();
            this.checkBoxBinning = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.textBoxBinning = new Silicups.GUI.MyTextBox();
            this.buttonMinima = new System.Windows.Forms.Button();
            this.buttonZeroOffset = new System.Windows.Forms.Button();
            this.buttonZeroP = new System.Windows.Forms.Button();
            this.buttonSetOffset = new System.Windows.Forms.Button();
            this.buttonSetP = new System.Windows.Forms.Button();
            this.checkBoxStyle = new System.Windows.Forms.CheckBox();
            this.trackBarOffset = new System.Windows.Forms.TrackBar();
            this.trackBarP = new System.Windows.Forms.TrackBar();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxOffsetPM = new System.Windows.Forms.TextBox();
            this.gliderOffset = new Silicups.GUI.Glider();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxPPM = new System.Windows.Forms.TextBox();
            this.gliderP = new Silicups.GUI.Glider();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxOffset = new Silicups.GUI.MyTextBox();
            this.radioButtonCompressed = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxP = new Silicups.GUI.MyTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxM0 = new System.Windows.Forms.TextBox();
            this.radioButtonPhased = new System.Windows.Forms.RadioButton();
            this.radioButtonTimeseries = new System.Windows.Forms.RadioButton();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newSolutionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadSolutionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveSolutionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveSolutionAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportSolutionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportSolutionToCSVToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.usingTemplateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectTemplateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.projectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addNewProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renameObjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sortObservationsByDateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importFromVarastroczToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToTxtToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportGraphToPNGToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.minimaPredictionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.observationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setCaptionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setFilterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addMinimumToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.exportPhasedSeriesToTXTToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportPhasedTimeSeriesToTXTToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).BeginInit();
            this.splitContainerMain.Panel1.SuspendLayout();
            this.splitContainerMain.Panel2.SuspendLayout();
            this.splitContainerMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerLeft)).BeginInit();
            this.splitContainerLeft.Panel1.SuspendLayout();
            this.splitContainerLeft.Panel2.SuspendLayout();
            this.splitContainerLeft.SuspendLayout();
            this.panelTools.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarOffset)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarP)).BeginInit();
            this.menuStrip.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainerMain
            // 
            this.splitContainerMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainerMain.Location = new System.Drawing.Point(0, 27);
            this.splitContainerMain.Name = "splitContainerMain";
            // 
            // splitContainerMain.Panel1
            // 
            this.splitContainerMain.Panel1.Controls.Add(this.splitContainerLeft);
            // 
            // splitContainerMain.Panel2
            // 
            this.splitContainerMain.Panel2.Controls.Add(this.graph);
            this.splitContainerMain.Panel2.Controls.Add(this.panelTools);
            this.splitContainerMain.Size = new System.Drawing.Size(1166, 474);
            this.splitContainerMain.SplitterDistance = 385;
            this.splitContainerMain.TabIndex = 2;
            // 
            // splitContainerLeft
            // 
            this.splitContainerLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerLeft.Location = new System.Drawing.Point(0, 0);
            this.splitContainerLeft.Name = "splitContainerLeft";
            // 
            // splitContainerLeft.Panel1
            // 
            this.splitContainerLeft.Panel1.Controls.Add(this.listBoxSolution);
            // 
            // splitContainerLeft.Panel2
            // 
            this.splitContainerLeft.Panel2.Controls.Add(this.listBoxObs);
            this.splitContainerLeft.Size = new System.Drawing.Size(385, 474);
            this.splitContainerLeft.SplitterDistance = 144;
            this.splitContainerLeft.TabIndex = 1;
            // 
            // listBoxSolution
            // 
            this.listBoxSolution.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxSolution.FormattingEnabled = true;
            this.listBoxSolution.IntegralHeight = false;
            this.listBoxSolution.Location = new System.Drawing.Point(0, 0);
            this.listBoxSolution.Name = "listBoxSolution";
            this.listBoxSolution.Size = new System.Drawing.Size(144, 474);
            this.listBoxSolution.TabIndex = 0;
            // 
            // listBoxObs
            // 
            this.listBoxObs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxObs.FormattingEnabled = true;
            this.listBoxObs.IntegralHeight = false;
            this.listBoxObs.Location = new System.Drawing.Point(0, 0);
            this.listBoxObs.Name = "listBoxObs";
            this.listBoxObs.Size = new System.Drawing.Size(237, 474);
            this.listBoxObs.TabIndex = 0;
            // 
            // graph
            // 
            this.graph.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.graph.BackColor = System.Drawing.Color.White;
            this.graph.Location = new System.Drawing.Point(2, 0);
            this.graph.Name = "graph";
            this.graph.Size = new System.Drawing.Size(772, 355);
            this.graph.TabIndex = 1;
            // 
            // panelTools
            // 
            this.panelTools.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelTools.Controls.Add(this.radioButtonOC);
            this.panelTools.Controls.Add(this.buttonAddMinimum);
            this.panelTools.Controls.Add(this.checkBoxBinning);
            this.panelTools.Controls.Add(this.label6);
            this.panelTools.Controls.Add(this.textBoxBinning);
            this.panelTools.Controls.Add(this.buttonMinima);
            this.panelTools.Controls.Add(this.buttonZeroOffset);
            this.panelTools.Controls.Add(this.buttonZeroP);
            this.panelTools.Controls.Add(this.buttonSetOffset);
            this.panelTools.Controls.Add(this.buttonSetP);
            this.panelTools.Controls.Add(this.checkBoxStyle);
            this.panelTools.Controls.Add(this.trackBarOffset);
            this.panelTools.Controls.Add(this.trackBarP);
            this.panelTools.Controls.Add(this.label5);
            this.panelTools.Controls.Add(this.textBoxOffsetPM);
            this.panelTools.Controls.Add(this.gliderOffset);
            this.panelTools.Controls.Add(this.label4);
            this.panelTools.Controls.Add(this.textBoxPPM);
            this.panelTools.Controls.Add(this.gliderP);
            this.panelTools.Controls.Add(this.label3);
            this.panelTools.Controls.Add(this.textBoxOffset);
            this.panelTools.Controls.Add(this.radioButtonCompressed);
            this.panelTools.Controls.Add(this.label2);
            this.panelTools.Controls.Add(this.textBoxP);
            this.panelTools.Controls.Add(this.label1);
            this.panelTools.Controls.Add(this.textBoxM0);
            this.panelTools.Controls.Add(this.radioButtonPhased);
            this.panelTools.Controls.Add(this.radioButtonTimeseries);
            this.panelTools.Location = new System.Drawing.Point(2, 361);
            this.panelTools.Name = "panelTools";
            this.panelTools.Size = new System.Drawing.Size(772, 110);
            this.panelTools.TabIndex = 0;
            // 
            // radioButtonOC
            // 
            this.radioButtonOC.AutoSize = true;
            this.radioButtonOC.Location = new System.Drawing.Point(246, 4);
            this.radioButtonOC.Margin = new System.Windows.Forms.Padding(4);
            this.radioButtonOC.Name = "radioButtonOC";
            this.radioButtonOC.Size = new System.Drawing.Size(57, 24);
            this.radioButtonOC.TabIndex = 27;
            this.radioButtonOC.Text = "OC";
            this.radioButtonOC.UseVisualStyleBackColor = true;
            // 
            // buttonAddMinimum
            // 
            this.buttonAddMinimum.Location = new System.Drawing.Point(6, 49);
            this.buttonAddMinimum.Name = "buttonAddMinimum";
            this.buttonAddMinimum.Size = new System.Drawing.Size(34, 23);
            this.buttonAddMinimum.TabIndex = 26;
            this.buttonAddMinimum.Text = "Add";
            this.buttonAddMinimum.UseVisualStyleBackColor = true;
            this.buttonAddMinimum.Click += new System.EventHandler(this.buttonAddMinimum_Click);
            // 
            // checkBoxBinning
            // 
            this.checkBoxBinning.AutoSize = true;
            this.checkBoxBinning.Checked = true;
            this.checkBoxBinning.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxBinning.Location = new System.Drawing.Point(172, 81);
            this.checkBoxBinning.Name = "checkBoxBinning";
            this.checkBoxBinning.Size = new System.Drawing.Size(15, 14);
            this.checkBoxBinning.TabIndex = 25;
            this.checkBoxBinning.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(59, 81);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(107, 13);
            this.label6.TabIndex = 24;
            this.label6.Text = "Phase Graph Binning";
            // 
            // textBoxBinning
            // 
            this.textBoxBinning.Location = new System.Drawing.Point(193, 78);
            this.textBoxBinning.Name = "textBoxBinning";
            this.textBoxBinning.Size = new System.Drawing.Size(79, 20);
            this.textBoxBinning.TabIndex = 23;
            // 
            // buttonMinima
            // 
            this.buttonMinima.Location = new System.Drawing.Point(43, 49);
            this.buttonMinima.Name = "buttonMinima";
            this.buttonMinima.Size = new System.Drawing.Size(51, 23);
            this.buttonMinima.TabIndex = 22;
            this.buttonMinima.Text = "Minima";
            this.buttonMinima.UseVisualStyleBackColor = true;
            this.buttonMinima.Click += new System.EventHandler(this.buttonMinima_Click);
            // 
            // buttonZeroOffset
            // 
            this.buttonZeroOffset.Location = new System.Drawing.Point(326, 72);
            this.buttonZeroOffset.Name = "buttonZeroOffset";
            this.buttonZeroOffset.Size = new System.Drawing.Size(19, 23);
            this.buttonZeroOffset.TabIndex = 21;
            this.buttonZeroOffset.Text = "0";
            this.buttonZeroOffset.UseVisualStyleBackColor = true;
            // 
            // buttonZeroP
            // 
            this.buttonZeroP.Location = new System.Drawing.Point(326, 3);
            this.buttonZeroP.Name = "buttonZeroP";
            this.buttonZeroP.Size = new System.Drawing.Size(19, 23);
            this.buttonZeroP.TabIndex = 20;
            this.buttonZeroP.Text = "0";
            this.buttonZeroP.UseVisualStyleBackColor = true;
            // 
            // buttonSetOffset
            // 
            this.buttonSetOffset.Location = new System.Drawing.Point(346, 72);
            this.buttonSetOffset.Name = "buttonSetOffset";
            this.buttonSetOffset.Size = new System.Drawing.Size(19, 23);
            this.buttonSetOffset.TabIndex = 19;
            this.buttonSetOffset.Text = "<";
            this.buttonSetOffset.UseVisualStyleBackColor = true;
            // 
            // buttonSetP
            // 
            this.buttonSetP.Location = new System.Drawing.Point(346, 3);
            this.buttonSetP.Name = "buttonSetP";
            this.buttonSetP.Size = new System.Drawing.Size(19, 23);
            this.buttonSetP.TabIndex = 18;
            this.buttonSetP.Text = "<";
            this.buttonSetP.UseVisualStyleBackColor = true;
            // 
            // checkBoxStyle
            // 
            this.checkBoxStyle.AutoSize = true;
            this.checkBoxStyle.Checked = true;
            this.checkBoxStyle.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxStyle.Location = new System.Drawing.Point(305, 5);
            this.checkBoxStyle.Name = "checkBoxStyle";
            this.checkBoxStyle.Size = new System.Drawing.Size(15, 14);
            this.checkBoxStyle.TabIndex = 17;
            this.checkBoxStyle.UseVisualStyleBackColor = true;
            // 
            // trackBarOffset
            // 
            this.trackBarOffset.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.trackBarOffset.Location = new System.Drawing.Point(371, 49);
            this.trackBarOffset.Maximum = 2048;
            this.trackBarOffset.Minimum = -2048;
            this.trackBarOffset.Name = "trackBarOffset";
            this.trackBarOffset.Size = new System.Drawing.Size(392, 45);
            this.trackBarOffset.TabIndex = 16;
            this.trackBarOffset.TickFrequency = 64;
            this.trackBarOffset.TickStyle = System.Windows.Forms.TickStyle.Both;
            // 
            // trackBarP
            // 
            this.trackBarP.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.trackBarP.Location = new System.Drawing.Point(371, 3);
            this.trackBarP.Maximum = 2048;
            this.trackBarP.Minimum = -2048;
            this.trackBarP.Name = "trackBarP";
            this.trackBarP.Size = new System.Drawing.Size(392, 45);
            this.trackBarP.TabIndex = 15;
            this.trackBarP.TickFrequency = 64;
            this.trackBarP.TickStyle = System.Windows.Forms.TickStyle.Both;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(278, 55);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(21, 13);
            this.label5.TabIndex = 14;
            this.label5.Text = "+/-";
            // 
            // textBoxOffsetPM
            // 
            this.textBoxOffsetPM.Location = new System.Drawing.Point(305, 52);
            this.textBoxOffsetPM.Name = "textBoxOffsetPM";
            this.textBoxOffsetPM.Size = new System.Drawing.Size(60, 20);
            this.textBoxOffsetPM.TabIndex = 13;
            // 
            // gliderOffset
            // 
            this.gliderOffset.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gliderOffset.Cursor = System.Windows.Forms.Cursors.Cross;
            this.gliderOffset.Enabled = false;
            this.gliderOffset.Location = new System.Drawing.Point(371, 52);
            this.gliderOffset.Name = "gliderOffset";
            this.gliderOffset.Size = new System.Drawing.Size(392, 20);
            this.gliderOffset.TabIndex = 12;
            this.gliderOffset.Visible = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(278, 29);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(21, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "+/-";
            // 
            // textBoxPPM
            // 
            this.textBoxPPM.Location = new System.Drawing.Point(305, 26);
            this.textBoxPPM.Name = "textBoxPPM";
            this.textBoxPPM.Size = new System.Drawing.Size(60, 20);
            this.textBoxPPM.TabIndex = 10;
            // 
            // gliderP
            // 
            this.gliderP.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gliderP.Cursor = System.Windows.Forms.Cursors.Cross;
            this.gliderP.Enabled = false;
            this.gliderP.Location = new System.Drawing.Point(371, 26);
            this.gliderP.Name = "gliderP";
            this.gliderP.Size = new System.Drawing.Size(392, 20);
            this.gliderP.TabIndex = 9;
            this.gliderP.Visible = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(93, 55);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(73, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Vertical Offset";
            // 
            // textBoxOffset
            // 
            this.textBoxOffset.Enabled = false;
            this.textBoxOffset.Location = new System.Drawing.Point(172, 52);
            this.textBoxOffset.Name = "textBoxOffset";
            this.textBoxOffset.Size = new System.Drawing.Size(100, 20);
            this.textBoxOffset.TabIndex = 7;
            this.textBoxOffset.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textBoxOffset_KeyUp);
            // 
            // radioButtonCompressed
            // 
            this.radioButtonCompressed.AutoSize = true;
            this.radioButtonCompressed.Location = new System.Drawing.Point(90, 3);
            this.radioButtonCompressed.Name = "radioButtonCompressed";
            this.radioButtonCompressed.Size = new System.Drawing.Size(83, 17);
            this.radioButtonCompressed.TabIndex = 6;
            this.radioButtonCompressed.TabStop = true;
            this.radioButtonCompressed.Text = "Compressed";
            this.radioButtonCompressed.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(152, 29);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(14, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "P";
            // 
            // textBoxP
            // 
            this.textBoxP.Location = new System.Drawing.Point(172, 26);
            this.textBoxP.Name = "textBoxP";
            this.textBoxP.Size = new System.Drawing.Size(100, 20);
            this.textBoxP.TabIndex = 4;
            this.textBoxP.TextChanged += new System.EventHandler(this.textBoxP_TextChanged);
            this.textBoxP.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textBoxP_KeyUp);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(22, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "M0";
            // 
            // textBoxM0
            // 
            this.textBoxM0.Location = new System.Drawing.Point(40, 26);
            this.textBoxM0.Name = "textBoxM0";
            this.textBoxM0.Size = new System.Drawing.Size(100, 20);
            this.textBoxM0.TabIndex = 2;
            this.textBoxM0.TextChanged += new System.EventHandler(this.textBoxP_TextChanged);
            // 
            // radioButtonPhased
            // 
            this.radioButtonPhased.AutoSize = true;
            this.radioButtonPhased.Location = new System.Drawing.Point(179, 3);
            this.radioButtonPhased.Name = "radioButtonPhased";
            this.radioButtonPhased.Size = new System.Drawing.Size(61, 17);
            this.radioButtonPhased.TabIndex = 1;
            this.radioButtonPhased.Text = "Phased";
            this.radioButtonPhased.UseVisualStyleBackColor = true;
            // 
            // radioButtonTimeseries
            // 
            this.radioButtonTimeseries.AutoSize = true;
            this.radioButtonTimeseries.Checked = true;
            this.radioButtonTimeseries.Location = new System.Drawing.Point(6, 3);
            this.radioButtonTimeseries.Name = "radioButtonTimeseries";
            this.radioButtonTimeseries.Size = new System.Drawing.Size(78, 17);
            this.radioButtonTimeseries.TabIndex = 0;
            this.radioButtonTimeseries.TabStop = true;
            this.radioButtonTimeseries.Text = "Time series";
            this.radioButtonTimeseries.UseVisualStyleBackColor = true;
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.projectToolStripMenuItem,
            this.observationToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(1166, 24);
            this.menuStrip.TabIndex = 3;
            this.menuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newSolutionToolStripMenuItem,
            this.loadSolutionToolStripMenuItem,
            this.saveSolutionToolStripMenuItem,
            this.saveSolutionAsToolStripMenuItem,
            this.exportSolutionToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newSolutionToolStripMenuItem
            // 
            this.newSolutionToolStripMenuItem.Name = "newSolutionToolStripMenuItem";
            this.newSolutionToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.newSolutionToolStripMenuItem.Text = "New Solution";
            this.newSolutionToolStripMenuItem.Click += new System.EventHandler(this.newProjectToolStripMenuItem_Click);
            // 
            // loadSolutionToolStripMenuItem
            // 
            this.loadSolutionToolStripMenuItem.Name = "loadSolutionToolStripMenuItem";
            this.loadSolutionToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.loadSolutionToolStripMenuItem.Text = "Load Solution...";
            this.loadSolutionToolStripMenuItem.Click += new System.EventHandler(this.loadSolutionToolStripMenuItem_Click);
            // 
            // saveSolutionToolStripMenuItem
            // 
            this.saveSolutionToolStripMenuItem.Name = "saveSolutionToolStripMenuItem";
            this.saveSolutionToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveSolutionToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.saveSolutionToolStripMenuItem.Text = "Save Solution";
            this.saveSolutionToolStripMenuItem.Click += new System.EventHandler(this.saveSolutionToolStripMenuItem_Click);
            // 
            // saveSolutionAsToolStripMenuItem
            // 
            this.saveSolutionAsToolStripMenuItem.Name = "saveSolutionAsToolStripMenuItem";
            this.saveSolutionAsToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.saveSolutionAsToolStripMenuItem.Text = "Save Solution As...";
            this.saveSolutionAsToolStripMenuItem.Click += new System.EventHandler(this.saveSolutionAsToolStripMenuItem_Click);
            // 
            // exportSolutionToolStripMenuItem
            // 
            this.exportSolutionToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportSolutionToCSVToolStripMenuItem,
            this.usingTemplateToolStripMenuItem,
            this.selectTemplateToolStripMenuItem});
            this.exportSolutionToolStripMenuItem.Name = "exportSolutionToolStripMenuItem";
            this.exportSolutionToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.exportSolutionToolStripMenuItem.Text = "Export Solution To";
            // 
            // exportSolutionToCSVToolStripMenuItem
            // 
            this.exportSolutionToCSVToolStripMenuItem.Name = "exportSolutionToCSVToolStripMenuItem";
            this.exportSolutionToCSVToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.exportSolutionToCSVToolStripMenuItem.Text = "CSV...";
            this.exportSolutionToCSVToolStripMenuItem.Click += new System.EventHandler(this.exportSolutionToCSVToolStripMenuItem_Click);
            // 
            // usingTemplateToolStripMenuItem
            // 
            this.usingTemplateToolStripMenuItem.Name = "usingTemplateToolStripMenuItem";
            this.usingTemplateToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.usingTemplateToolStripMenuItem.Text = "Using Template...";
            this.usingTemplateToolStripMenuItem.Click += new System.EventHandler(this.usingTemplateToolStripMenuItem_Click);
            // 
            // selectTemplateToolStripMenuItem
            // 
            this.selectTemplateToolStripMenuItem.Name = "selectTemplateToolStripMenuItem";
            this.selectTemplateToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.selectTemplateToolStripMenuItem.Text = "Select Template...";
            this.selectTemplateToolStripMenuItem.Click += new System.EventHandler(this.selectTemplateToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // projectToolStripMenuItem
            // 
            this.projectToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addNewProjectToolStripMenuItem,
            this.renameObjectToolStripMenuItem,
            this.loadFileToolStripMenuItem,
            this.loadFilesToolStripMenuItem,
            this.sortObservationsByDateToolStripMenuItem,
            this.importFromVarastroczToolStripMenuItem,
            this.exportToTxtToolStripMenuItem,
            this.exportPhasedSeriesToTXTToolStripMenuItem,
            this.exportPhasedTimeSeriesToTXTToolStripMenuItem,
            this.exportGraphToPNGToolStripMenuItem,
            this.minimaPredictionsToolStripMenuItem});
            this.projectToolStripMenuItem.Name = "projectToolStripMenuItem";
            this.projectToolStripMenuItem.Size = new System.Drawing.Size(54, 20);
            this.projectToolStripMenuItem.Text = "Object";
            // 
            // addNewProjectToolStripMenuItem
            // 
            this.addNewProjectToolStripMenuItem.Name = "addNewProjectToolStripMenuItem";
            this.addNewProjectToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.I)));
            this.addNewProjectToolStripMenuItem.Size = new System.Drawing.Size(261, 22);
            this.addNewProjectToolStripMenuItem.Text = "Add New Object To Solution";
            this.addNewProjectToolStripMenuItem.Click += new System.EventHandler(this.addNewProjectToolStripMenuItem_Click);
            // 
            // renameObjectToolStripMenuItem
            // 
            this.renameObjectToolStripMenuItem.Name = "renameObjectToolStripMenuItem";
            this.renameObjectToolStripMenuItem.ShortcutKeyDisplayString = "F2";
            this.renameObjectToolStripMenuItem.Size = new System.Drawing.Size(261, 22);
            this.renameObjectToolStripMenuItem.Text = "Rename Object...";
            this.renameObjectToolStripMenuItem.Click += new System.EventHandler(this.renameObjectToolStripMenuItem_Click);
            // 
            // loadFileToolStripMenuItem
            // 
            this.loadFileToolStripMenuItem.Name = "loadFileToolStripMenuItem";
            this.loadFileToolStripMenuItem.Size = new System.Drawing.Size(261, 22);
            this.loadFileToolStripMenuItem.Text = "Add File[s]...";
            this.loadFileToolStripMenuItem.Click += new System.EventHandler(this.loadFileToolStripMenuItem_Click);
            // 
            // loadFilesToolStripMenuItem
            // 
            this.loadFilesToolStripMenuItem.Name = "loadFilesToolStripMenuItem";
            this.loadFilesToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
            this.loadFilesToolStripMenuItem.Size = new System.Drawing.Size(261, 22);
            this.loadFilesToolStripMenuItem.Text = "(Re)Scan File Series...";
            this.loadFilesToolStripMenuItem.Click += new System.EventHandler(this.loadFilesToolStripMenuItem_Click);
            // 
            // sortObservationsByDateToolStripMenuItem
            // 
            this.sortObservationsByDateToolStripMenuItem.Name = "sortObservationsByDateToolStripMenuItem";
            this.sortObservationsByDateToolStripMenuItem.Size = new System.Drawing.Size(261, 22);
            this.sortObservationsByDateToolStripMenuItem.Text = "Sort Observations By Date";
            this.sortObservationsByDateToolStripMenuItem.Click += new System.EventHandler(this.sortObservationsByDateToolStripMenuItem_Click);
            // 
            // importFromVarastroczToolStripMenuItem
            // 
            this.importFromVarastroczToolStripMenuItem.Name = "importFromVarastroczToolStripMenuItem";
            this.importFromVarastroczToolStripMenuItem.Size = new System.Drawing.Size(261, 22);
            this.importFromVarastroczToolStripMenuItem.Text = "Import from var.astro.cz...";
            this.importFromVarastroczToolStripMenuItem.Click += new System.EventHandler(this.importFromVarastroczToolStripMenuItem_Click);
            // 
            // exportToTxtToolStripMenuItem
            // 
            this.exportToTxtToolStripMenuItem.Name = "exportToTxtToolStripMenuItem";
            this.exportToTxtToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.E)));
            this.exportToTxtToolStripMenuItem.Size = new System.Drawing.Size(261, 22);
            this.exportToTxtToolStripMenuItem.Text = "Export Time Series To TXT...";
            this.exportToTxtToolStripMenuItem.Click += new System.EventHandler(this.exportToTxtToolStripMenuItem_Click);
            // 
            // exportGraphToPNGToolStripMenuItem
            // 
            this.exportGraphToPNGToolStripMenuItem.Name = "exportGraphToPNGToolStripMenuItem";
            this.exportGraphToPNGToolStripMenuItem.Size = new System.Drawing.Size(261, 22);
            this.exportGraphToPNGToolStripMenuItem.Text = "Export Graph to PNG...";
            this.exportGraphToPNGToolStripMenuItem.Click += new System.EventHandler(this.exportGraphToPNGToolStripMenuItem_Click);
            // 
            // minimaPredictionsToolStripMenuItem
            // 
            this.minimaPredictionsToolStripMenuItem.Name = "minimaPredictionsToolStripMenuItem";
            this.minimaPredictionsToolStripMenuItem.Size = new System.Drawing.Size(261, 22);
            this.minimaPredictionsToolStripMenuItem.Text = "Minima Predictions...";
            this.minimaPredictionsToolStripMenuItem.Click += new System.EventHandler(this.minimaPredictionsToolStripMenuItem_Click);
            // 
            // observationToolStripMenuItem
            // 
            this.observationToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.setCaptionToolStripMenuItem,
            this.setFilterToolStripMenuItem,
            this.addMinimumToolStripMenuItem});
            this.observationToolStripMenuItem.Name = "observationToolStripMenuItem";
            this.observationToolStripMenuItem.Size = new System.Drawing.Size(83, 20);
            this.observationToolStripMenuItem.Text = "Observation";
            // 
            // setCaptionToolStripMenuItem
            // 
            this.setCaptionToolStripMenuItem.Name = "setCaptionToolStripMenuItem";
            this.setCaptionToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.setCaptionToolStripMenuItem.Text = "Set Caption...";
            this.setCaptionToolStripMenuItem.Click += new System.EventHandler(this.setCaptionToolStripMenuItem_Click);
            // 
            // setFilterToolStripMenuItem
            // 
            this.setFilterToolStripMenuItem.Name = "setFilterToolStripMenuItem";
            this.setFilterToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.setFilterToolStripMenuItem.Text = "Set Filter...";
            this.setFilterToolStripMenuItem.Click += new System.EventHandler(this.setFilterToolStripMenuItem_Click);
            // 
            // addMinimumToolStripMenuItem
            // 
            this.addMinimumToolStripMenuItem.Name = "addMinimumToolStripMenuItem";
            this.addMinimumToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.addMinimumToolStripMenuItem.Text = "Add Minimum...";
            this.addMinimumToolStripMenuItem.Click += new System.EventHandler(this.addMinimumToolStripMenuItem_Click);
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip.Location = new System.Drawing.Point(0, 504);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(1166, 22);
            this.statusStrip.TabIndex = 4;
            this.statusStrip.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(78, 17);
            this.toolStripStatusLabel1.Text = "SILICUPS v0.9";
            // 
            // exportPhasedSeriesToTXTToolStripMenuItem
            // 
            this.exportPhasedSeriesToTXTToolStripMenuItem.Name = "exportPhasedSeriesToTXTToolStripMenuItem";
            this.exportPhasedSeriesToTXTToolStripMenuItem.Size = new System.Drawing.Size(261, 22);
            this.exportPhasedSeriesToTXTToolStripMenuItem.Text = "Export Phased Series To TXT...";
            this.exportPhasedSeriesToTXTToolStripMenuItem.Click += new System.EventHandler(this.exportPhasedSeriesToTXTToolStripMenuItem_Click);
            // 
            // exportPhasedTimeSeriesToTXTToolStripMenuItem
            // 
            this.exportPhasedTimeSeriesToTXTToolStripMenuItem.Name = "exportPhasedTimeSeriesToTXTToolStripMenuItem";
            this.exportPhasedTimeSeriesToTXTToolStripMenuItem.Size = new System.Drawing.Size(261, 22);
            this.exportPhasedTimeSeriesToTXTToolStripMenuItem.Text = "Export Phased Time Series To TXT...";
            this.exportPhasedTimeSeriesToTXTToolStripMenuItem.Click += new System.EventHandler(this.exportPhasedTimeSeriesToTXTToolStripMenuItem_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1166, 526);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.splitContainerMain);
            this.Controls.Add(this.menuStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip;
            this.Name = "MainForm";
            this.Text = "SILICUPS";
            this.splitContainerMain.Panel1.ResumeLayout(false);
            this.splitContainerMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).EndInit();
            this.splitContainerMain.ResumeLayout(false);
            this.splitContainerLeft.Panel1.ResumeLayout(false);
            this.splitContainerLeft.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerLeft)).EndInit();
            this.splitContainerLeft.ResumeLayout(false);
            this.panelTools.ResumeLayout(false);
            this.panelTools.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarOffset)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarP)).EndInit();
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainerMain;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.StatusStrip statusStrip;
        private Graph graph;
        private System.Windows.Forms.Panel panelTools;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.RadioButton radioButtonPhased;
        private System.Windows.Forms.RadioButton radioButtonTimeseries;
        private System.Windows.Forms.Label label2;
        private MyTextBox textBoxP;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxM0;
        private System.Windows.Forms.ToolStripMenuItem loadFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadFilesToolStripMenuItem;
        private System.Windows.Forms.CheckedListBox listBoxObs;
        private System.Windows.Forms.RadioButton radioButtonCompressed;
        private System.Windows.Forms.Label label3;
        private MyTextBox textBoxOffset;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxOffsetPM;
        private Glider gliderOffset;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxPPM;
        private Glider gliderP;
        private System.Windows.Forms.ToolStripMenuItem newSolutionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadSolutionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveSolutionAsToolStripMenuItem;
        private System.Windows.Forms.SplitContainer splitContainerLeft;
        private MyListBox listBoxSolution;
        private System.Windows.Forms.ToolStripMenuItem projectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addNewProjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportToTxtToolStripMenuItem;
        private System.Windows.Forms.TrackBar trackBarP;
        private System.Windows.Forms.TrackBar trackBarOffset;
        private System.Windows.Forms.Button buttonSetOffset;
        private System.Windows.Forms.Button buttonSetP;
        private System.Windows.Forms.CheckBox checkBoxStyle;
        private System.Windows.Forms.Button buttonZeroOffset;
        private System.Windows.Forms.Button buttonZeroP;
        private System.Windows.Forms.Button buttonMinima;
        private System.Windows.Forms.ToolStripMenuItem importFromVarastroczToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveSolutionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem renameObjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem minimaPredictionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sortObservationsByDateToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripMenuItem observationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setCaptionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setFilterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportSolutionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportSolutionToCSVToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem usingTemplateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectTemplateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addMinimumToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportGraphToPNGToolStripMenuItem;
        private System.Windows.Forms.CheckBox checkBoxBinning;
        private System.Windows.Forms.Label label6;
        private MyTextBox textBoxBinning;
        private System.Windows.Forms.Button buttonAddMinimum;
        private System.Windows.Forms.ToolStripMenuItem exportPhasedSeriesToTXTToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportPhasedTimeSeriesToTXTToolStripMenuItem;
        private System.Windows.Forms.RadioButton radioButtonOC;

    }
}

