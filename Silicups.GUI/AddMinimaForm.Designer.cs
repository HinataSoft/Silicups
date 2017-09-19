namespace Silicups.GUI
{
    partial class AddMinimaForm
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
            this.graph = new Silicups.GUI.Graph(this.components);
            this.trackBar = new System.Windows.Forms.TrackBar();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.trackBarErr = new System.Windows.Forms.TrackBar();
            this.comboBoxMinType = new System.Windows.Forms.ComboBox();
            this.label = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarErr)).BeginInit();
            this.SuspendLayout();
            // 
            // graph
            // 
            this.graph.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.graph.BackColor = System.Drawing.Color.White;
            this.graph.Location = new System.Drawing.Point(12, 12);
            this.graph.Name = "graph";
            this.graph.Size = new System.Drawing.Size(1087, 473);
            this.graph.TabIndex = 0;
            // 
            // trackBar
            // 
            this.trackBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.trackBar.LargeChange = 20;
            this.trackBar.Location = new System.Drawing.Point(12, 549);
            this.trackBar.Maximum = 2000;
            this.trackBar.Minimum = -2000;
            this.trackBar.Name = "trackBar";
            this.trackBar.Size = new System.Drawing.Size(1087, 45);
            this.trackBar.TabIndex = 1;
            this.trackBar.TickFrequency = 50;
            this.trackBar.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.trackBar.ValueChanged += new System.EventHandler(this.trackBar_ValueChanged);
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOK.Location = new System.Drawing.Point(1024, 491);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 2;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(1024, 520);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 3;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // trackBarErr
            // 
            this.trackBarErr.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.trackBarErr.LargeChange = 20;
            this.trackBarErr.Location = new System.Drawing.Point(606, 498);
            this.trackBarErr.Maximum = 1000;
            this.trackBarErr.Name = "trackBarErr";
            this.trackBarErr.Size = new System.Drawing.Size(412, 45);
            this.trackBarErr.TabIndex = 4;
            this.trackBarErr.TickFrequency = 50;
            this.trackBarErr.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.trackBarErr.ValueChanged += new System.EventHandler(this.trackBarErr_ValueChanged);
            // 
            // comboBoxMinType
            // 
            this.comboBoxMinType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxMinType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxMinType.FormattingEnabled = true;
            this.comboBoxMinType.Items.AddRange(new object[] {
            "Unknown",
            "Primary",
            "Secondary"});
            this.comboBoxMinType.Location = new System.Drawing.Point(479, 510);
            this.comboBoxMinType.Name = "comboBoxMinType";
            this.comboBoxMinType.Size = new System.Drawing.Size(121, 21);
            this.comboBoxMinType.TabIndex = 5;
            this.comboBoxMinType.SelectedValueChanged += new System.EventHandler(this.comboBoxMinType_SelectedValueChanged);
            // 
            // label
            // 
            this.label.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label.AutoSize = true;
            this.label.Location = new System.Drawing.Point(12, 513);
            this.label.Name = "label";
            this.label.Size = new System.Drawing.Size(35, 13);
            this.label.TabIndex = 6;
            this.label.Text = "label1";
            // 
            // AddMinimaForm
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(1111, 604);
            this.Controls.Add(this.label);
            this.Controls.Add(this.comboBoxMinType);
            this.Controls.Add(this.trackBarErr);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.trackBar);
            this.Controls.Add(this.graph);
            this.Name = "AddMinimaForm";
            this.Text = "AddMinimaForm";
            ((System.ComponentModel.ISupportInitialize)(this.trackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarErr)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Graph graph;
        private System.Windows.Forms.TrackBar trackBar;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.TrackBar trackBarErr;
        private System.Windows.Forms.ComboBox comboBoxMinType;
        private System.Windows.Forms.Label label;
    }
}