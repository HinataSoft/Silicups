using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Silicups.GUI
{
    public partial class ImportVarAstroCzForm : Form
    {
        private static readonly string RegistryPath = @"SOFTWARE\HinataSoft\Silicups\ImportVarAstroCzForm";
        public Silicups.Core.Project Project = null; 

        public ImportVarAstroCzForm()
        {
            InitializeComponent();

            try
            {
                var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(RegistryPath);
                if (key != null)
                {
                    textBoxDirectory.Text = key.GetValue("Directory").ToString();
                    textBoxURL.Text = key.GetValue("URL").ToString();
                }
            }
            catch
            { }
        }

        private void buttonChooseDir_Click(object sender, EventArgs e)
        {
            using (var fd = new FolderBrowserDialog())
            {
                DialogResult dialogResult = fd.ShowDialog();
                if (dialogResult == DialogResult.OK)
                {
                    textBoxDirectory.Text = fd.SelectedPath;
                }
            }
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            try
            {
                var key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(RegistryPath);
                if (key != null)
                {
                    key.SetValue("Directory", textBoxDirectory.Text);
                    key.SetValue("URL", textBoxURL.Text);
                }
            }
            catch
            { }

            textBoxLog.Clear();
            try
            {
                Project = Silicups.Util.VarAstroCzImporter.Import(Log, textBoxURL.Text, textBoxDirectory.Text);
            }
            catch (Exception ex)
            {
                Project = null;
                Log("Exception: " + ex);
            }
        }

        private void Log(string s)
        {
            textBoxLog.AppendText(s + "\r\n");
        }
    }
}
