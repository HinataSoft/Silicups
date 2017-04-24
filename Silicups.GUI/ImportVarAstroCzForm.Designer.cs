namespace Silicups.GUI
{
    partial class ImportVarAstroCzForm
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
            this.textBoxURL = new System.Windows.Forms.TextBox();
            this.textBoxDirectory = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonChooseDir = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonStart = new System.Windows.Forms.Button();
            this.checkBoxOverwrite = new System.Windows.Forms.CheckBox();
            this.buttonClose = new System.Windows.Forms.Button();
            this.textBoxLog = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // textBoxURL
            // 
            this.textBoxURL.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxURL.Location = new System.Drawing.Point(88, 12);
            this.textBoxURL.Name = "textBoxURL";
            this.textBoxURL.Size = new System.Drawing.Size(587, 20);
            this.textBoxURL.TabIndex = 0;
            // 
            // textBoxDirectory
            // 
            this.textBoxDirectory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxDirectory.Location = new System.Drawing.Point(88, 40);
            this.textBoxDirectory.Name = "textBoxDirectory";
            this.textBoxDirectory.Size = new System.Drawing.Size(504, 20);
            this.textBoxDirectory.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(53, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "URL";
            // 
            // buttonChooseDir
            // 
            this.buttonChooseDir.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonChooseDir.Location = new System.Drawing.Point(600, 38);
            this.buttonChooseDir.Name = "buttonChooseDir";
            this.buttonChooseDir.Size = new System.Drawing.Size(75, 23);
            this.buttonChooseDir.TabIndex = 2;
            this.buttonChooseDir.Text = "Choose";
            this.buttonChooseDir.UseVisualStyleBackColor = true;
            this.buttonChooseDir.Click += new System.EventHandler(this.buttonChooseDir_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(33, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(49, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "Directory";
            // 
            // buttonStart
            // 
            this.buttonStart.Location = new System.Drawing.Point(300, 66);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(75, 23);
            this.buttonStart.TabIndex = 4;
            this.buttonStart.Text = "Start";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // checkBoxOverwrite
            // 
            this.checkBoxOverwrite.AutoSize = true;
            this.checkBoxOverwrite.Checked = true;
            this.checkBoxOverwrite.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxOverwrite.Location = new System.Drawing.Point(88, 70);
            this.checkBoxOverwrite.Name = "checkBoxOverwrite";
            this.checkBoxOverwrite.Size = new System.Drawing.Size(109, 17);
            this.checkBoxOverwrite.TabIndex = 3;
            this.checkBoxOverwrite.Text = "Overwrite existing";
            this.checkBoxOverwrite.UseVisualStyleBackColor = true;
            // 
            // buttonClose
            // 
            this.buttonClose.Location = new System.Drawing.Point(381, 66);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(75, 23);
            this.buttonClose.TabIndex = 5;
            this.buttonClose.Text = "Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // textBoxLog
            // 
            this.textBoxLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxLog.Location = new System.Drawing.Point(12, 107);
            this.textBoxLog.Multiline = true;
            this.textBoxLog.Name = "textBoxLog";
            this.textBoxLog.Size = new System.Drawing.Size(663, 264);
            this.textBoxLog.TabIndex = 6;
            // 
            // ImportVarAstroCzForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(687, 383);
            this.Controls.Add(this.textBoxLog);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.checkBoxOverwrite);
            this.Controls.Add(this.buttonStart);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.buttonChooseDir);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxDirectory);
            this.Controls.Add(this.textBoxURL);
            this.Name = "ImportVarAstroCzForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Import from var.astro.cz";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxURL;
        private System.Windows.Forms.TextBox textBoxDirectory;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonChooseDir;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.CheckBox checkBoxOverwrite;
        private System.Windows.Forms.Button buttonClose;
        private System.Windows.Forms.TextBox textBoxLog;
    }
}