using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Silicups.GUI
{
    public partial class FilesSelectForm : Form
    {
        public string SelectedDirectory { get; set; }
        public string SelectedPattern { get; set; }

        public FilesSelectForm()
        {
            InitializeComponent();
        }

        public string[] FileNames
        {
            get { return System.IO.Directory.GetFiles(SelectedDirectory, SelectedPattern, System.IO.SearchOption.AllDirectories); }
        }

        private void buttonChoose_Click(object sender, EventArgs e)
        {
            var fd = new OpenFileDialog();
            fd.Filter = "Text Files (.txt)|*.txt|All Files (*.*)|*.*";
            DialogResult dialogResult = fd.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                textBoxDirectory.Text = System.IO.Path.GetDirectoryName(fd.FileName);
                textBoxPattern.Text = System.IO.Path.GetFileName(fd.FileName);
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            this.SelectedDirectory = textBoxDirectory.Text;
            this.SelectedPattern = textBoxPattern.Text;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
