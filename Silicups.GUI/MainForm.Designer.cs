namespace Silicups.GUI
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
            this.listBoxObs = new System.Windows.Forms.CheckedListBox();
            this.graph = new Silicups.GUI.Graph(this.components);
            this.panelTools = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxOffsetPM = new System.Windows.Forms.TextBox();
            this.gliderOffset = new Silicups.GUI.Glider();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxPPM = new System.Windows.Forms.TextBox();
            this.gliderP = new Silicups.GUI.Glider();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxOffset = new System.Windows.Forms.TextBox();
            this.radioButtonCompressed = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxP = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxM0 = new System.Windows.Forms.TextBox();
            this.radioButtonPhased = new System.Windows.Forms.RadioButton();
            this.radioButtonTimeseries = new System.Windows.Forms.RadioButton();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadFilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).BeginInit();
            this.splitContainerMain.Panel1.SuspendLayout();
            this.splitContainerMain.Panel2.SuspendLayout();
            this.splitContainerMain.SuspendLayout();
            this.panelTools.SuspendLayout();
            this.menuStrip.SuspendLayout();
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
            this.splitContainerMain.Panel1.Controls.Add(this.listBoxObs);
            // 
            // splitContainerMain.Panel2
            // 
            this.splitContainerMain.Panel2.Controls.Add(this.graph);
            this.splitContainerMain.Panel2.Controls.Add(this.panelTools);
            this.splitContainerMain.Size = new System.Drawing.Size(984, 527);
            this.splitContainerMain.SplitterDistance = 207;
            this.splitContainerMain.TabIndex = 2;
            // 
            // listBoxObs
            // 
            this.listBoxObs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBoxObs.FormattingEnabled = true;
            this.listBoxObs.Location = new System.Drawing.Point(0, 0);
            this.listBoxObs.Name = "listBoxObs";
            this.listBoxObs.Size = new System.Drawing.Size(207, 527);
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
            this.graph.Size = new System.Drawing.Size(768, 421);
            this.graph.TabIndex = 1;
            // 
            // panelTools
            // 
            this.panelTools.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
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
            this.panelTools.Location = new System.Drawing.Point(2, 427);
            this.panelTools.Name = "panelTools";
            this.panelTools.Size = new System.Drawing.Size(768, 97);
            this.panelTools.TabIndex = 0;
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
            this.textBoxOffsetPM.Location = new System.Drawing.Point(411, 52);
            this.textBoxOffsetPM.Name = "textBoxOffsetPM";
            this.textBoxOffsetPM.Size = new System.Drawing.Size(60, 20);
            this.textBoxOffsetPM.TabIndex = 13;
            // 
            // gliderOffset
            // 
            this.gliderOffset.Cursor = System.Windows.Forms.Cursors.Cross;
            this.gliderOffset.Enabled = false;
            this.gliderOffset.Location = new System.Drawing.Point(305, 52);
            this.gliderOffset.Name = "gliderOffset";
            this.gliderOffset.Size = new System.Drawing.Size(100, 20);
            this.gliderOffset.TabIndex = 12;
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
            this.textBoxPPM.Location = new System.Drawing.Point(411, 26);
            this.textBoxPPM.Name = "textBoxPPM";
            this.textBoxPPM.Size = new System.Drawing.Size(60, 20);
            this.textBoxPPM.TabIndex = 10;
            // 
            // gliderP
            // 
            this.gliderP.Cursor = System.Windows.Forms.Cursors.Cross;
            this.gliderP.Enabled = false;
            this.gliderP.Location = new System.Drawing.Point(305, 26);
            this.gliderP.Name = "gliderP";
            this.gliderP.Size = new System.Drawing.Size(100, 20);
            this.gliderP.TabIndex = 9;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(95, 55);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Vertical offset";
            // 
            // textBoxOffset
            // 
            this.textBoxOffset.Enabled = false;
            this.textBoxOffset.Location = new System.Drawing.Point(172, 52);
            this.textBoxOffset.Name = "textBoxOffset";
            this.textBoxOffset.Size = new System.Drawing.Size(100, 20);
            this.textBoxOffset.TabIndex = 7;
            // 
            // radioButtonCompressed
            // 
            this.radioButtonCompressed.AutoSize = true;
            this.radioButtonCompressed.Location = new System.Drawing.Point(124, 3);
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
            this.radioButtonPhased.Location = new System.Drawing.Point(213, 3);
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
            this.radioButtonTimeseries.Location = new System.Drawing.Point(40, 3);
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
            this.fileToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(984, 24);
            this.menuStrip.TabIndex = 3;
            this.menuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadFileToolStripMenuItem,
            this.loadFilesToolStripMenuItem,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // loadFileToolStripMenuItem
            // 
            this.loadFileToolStripMenuItem.Name = "loadFileToolStripMenuItem";
            this.loadFileToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.loadFileToolStripMenuItem.Text = "Load file[s]...";
            this.loadFileToolStripMenuItem.Click += new System.EventHandler(this.loadFileToolStripMenuItem_Click);
            // 
            // loadFilesToolStripMenuItem
            // 
            this.loadFilesToolStripMenuItem.Name = "loadFilesToolStripMenuItem";
            this.loadFilesToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.loadFilesToolStripMenuItem.Text = "Load file series...";
            this.loadFilesToolStripMenuItem.Click += new System.EventHandler(this.loadFilesToolStripMenuItem_Click);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // statusStrip
            // 
            this.statusStrip.Location = new System.Drawing.Point(0, 557);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(984, 22);
            this.statusStrip.TabIndex = 4;
            this.statusStrip.Text = "statusStrip1";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(984, 579);
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
            this.panelTools.ResumeLayout(false);
            this.panelTools.PerformLayout();
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
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
        private System.Windows.Forms.TextBox textBoxP;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxM0;
        private System.Windows.Forms.ToolStripMenuItem loadFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadFilesToolStripMenuItem;
        private System.Windows.Forms.CheckedListBox listBoxObs;
        private System.Windows.Forms.RadioButton radioButtonCompressed;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxOffset;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxOffsetPM;
        private Glider gliderOffset;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxPPM;
        private Glider gliderP;

    }
}

