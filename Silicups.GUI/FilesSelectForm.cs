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
        private static readonly string RegistryPath = @"SOFTWARE\HinataSoft\Silicups\FileSelectDialog";

        public string SelectedDirectory { get; private set; }
        public string SelectedPattern { get; private set; }
        public string SelectedFilter { get; private set; }

        public FilesSelectForm()
        {
            InitializeComponent();

            try
            {
                var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(RegistryPath);
                if (key != null)
                {
                    textBoxDirectory.Text = key.GetValue("Directory").ToString();
                    textBoxPattern.Text = key.GetValue("Pattern").ToString();
                    textBoxFilter.Text = key.GetValue("Filter").ToString();
                }
            }
            catch
            { }
        }

        private void buttonChoose_Click(object sender, EventArgs e)
        {
            using (var fd = new OpenFileDialog())
            {
                fd.Filter = "Text Files (.txt)|*.txt|All Files (*.*)|*.*";
                DialogResult dialogResult = fd.ShowDialog();
                if (dialogResult == DialogResult.OK)
                {
                    textBoxDirectory.Text = System.IO.Path.GetDirectoryName(fd.FileName);
                    textBoxPattern.Text = System.IO.Path.GetFileName(fd.FileName);
                }
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            this.SelectedDirectory = textBoxDirectory.Text;
            this.SelectedPattern = textBoxPattern.Text;
            this.SelectedFilter = textBoxFilter.Text;

            try
            {
                var key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(RegistryPath);
                if (key != null)
                {
                    key.SetValue("Directory", textBoxDirectory.Text);
                    key.SetValue("Pattern", textBoxPattern.Text);
                    key.SetValue("Filter", textBoxFilter.Text);
                }
            }
            catch
            { }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        class FilterItem
        {
            public bool Include;
            public string Contains;
        }
    }
}
