namespace Silicups.GUI
{
    partial class MinimaForm
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
            this.textBoxMinima = new System.Windows.Forms.TextBox();
            this.buttonAdd = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBoxMinima
            // 
            this.textBoxMinima.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxMinima.Location = new System.Drawing.Point(12, 12);
            this.textBoxMinima.Multiline = true;
            this.textBoxMinima.Name = "textBoxMinima";
            this.textBoxMinima.Size = new System.Drawing.Size(260, 212);
            this.textBoxMinima.TabIndex = 0;
            // 
            // buttonAdd
            // 
            this.buttonAdd.Location = new System.Drawing.Point(12, 230);
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.Size = new System.Drawing.Size(85, 23);
            this.buttonAdd.TabIndex = 1;
            this.buttonAdd.Text = "Add (mirroring)";
            this.buttonAdd.UseVisualStyleBackColor = true;
            // 
            // MinimaForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 265);
            this.Controls.Add(this.buttonAdd);
            this.Controls.Add(this.textBoxMinima);
            this.Name = "MinimaForm";
            this.Text = "MinimaForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxMinima;
        private System.Windows.Forms.Button buttonAdd;
    }
}