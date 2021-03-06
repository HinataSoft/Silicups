﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Silicups.GUI
{
    public partial class FilesSelectForm : FormEx
    {
        private static readonly string RegistryPath = Util.RegistryHelper.RegistryPath + @"\FileSelectDialog";

        private bool InitedFromRegistry { get; set; }
        public string SelectedDirectory { get; private set; }
        public string SelectedPattern { get; private set; }
        public string SelectedFilter { get; private set; }

        public FilesSelectForm()
        {
            InitializeComponent();
            InitializeFormEx(RegistryPath);

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

            this.InitedFromRegistry = true;
        }

        public FilesSelectForm(string directory, string pattern, string filter)
        {
            InitializeComponent();

            textBoxDirectory.Text = directory;
            textBoxPattern.Text = pattern;
            textBoxFilter.Text = filter;

            this.InitedFromRegistry = false;
        }

        public static FilesSelectForm CreateFilesSelectForm(Silicups.Core.Project project)
        {
            if ((project != null) && !String.IsNullOrWhiteSpace(project.AbsoluteBasePath))
            { return new FilesSelectForm(project.AbsoluteBasePath, project.FilePattern, project.FileFilter); }
            else
            { return new FilesSelectForm(); }
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
            buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;

            this.SelectedDirectory = textBoxDirectory.Text;
            this.SelectedPattern = textBoxPattern.Text;
            this.SelectedFilter = textBoxFilter.Text;

            try
            {
                if (InitedFromRegistry)
                {
                    var key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(RegistryPath);
                    if (key != null)
                    {
                        key.SetValue("Directory", textBoxDirectory.Text);
                        key.SetValue("Pattern", textBoxPattern.Text);
                        key.SetValue("Filter", textBoxFilter.Text);
                    }
                }
            }
            catch
            { }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
