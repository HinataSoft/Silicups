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
        public string SelectedDirectory { get; private set; }
        public string SelectedPattern { get; private set; }
        public string SelectedFilter { get; private set; }

        public FilesSelectForm()
        {
            InitializeComponent();
        }

        public string[] FileNames
        {
            get { return FindFiles(); }
        }

        private string[] FindFiles()
        {
            var filters = new List<FilterItem>();
            if (!String.IsNullOrWhiteSpace(SelectedFilter))
            {
                foreach(string s in SelectedFilter.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    string part = s;
                    bool include = true;
                    if (part.StartsWith("+"))
                    {
                        part = part.Substring(1);
                    }
                    else if (part.StartsWith("-"))
                    {
                        include = false;
                        part = part.Substring(1);
                    }
                    if(!String.IsNullOrWhiteSpace(part))
                    { filters.Add(new FilterItem() { Include = include, Contains = part }); }
                }
            }

            var files = new List<string>();
            foreach(string filename in System.IO.Directory.GetFiles(SelectedDirectory, SelectedPattern, System.IO.SearchOption.AllDirectories))
            {
                bool filtered = false;
                foreach (FilterItem filter in filters)
                {
                    if (filter.Include && !filename.Contains(filter.Contains))
                    { filtered = true; break; }
                    if (!filter.Include && filename.Contains(filter.Contains))
                    { filtered = true; break; }
                }
                if (filtered)
                { continue; }

                files.Add(filename);
            }
            return files.ToArray();
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
            this.SelectedFilter = textBoxFilter.Text;
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
