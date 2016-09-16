using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Silicups.Core;

namespace Silicups.GUI
{
    public partial class MainForm : Form
    {
        private Project Project = null;
        private bool IsInitializing = false;

        public MainForm()
        {
            InitializeComponent();

            radioButtonTimeseries.CheckedChanged += new EventHandler(radioButtonTimeseries_CheckedChanged);
            radioButtonPhased.CheckedChanged += new EventHandler(radioButtonPhased_CheckedChanged);
            radioButtonCompressed.CheckedChanged += new EventHandler(radioButtonCompressed_CheckedChanged);

            if (System.IO.File.Exists("autoload.txt"))
            { LoadFile("autoload.txt"); }
        }

        void radioButtonTimeseries_CheckedChanged(object sender, EventArgs e)
        {
            if (!IsInitializing && radioButtonTimeseries.Checked)
            { SetDataSource(Project.GetTimeSeries()); }
        }

        void radioButtonCompressed_CheckedChanged(object sender, EventArgs e)
        {
            if (!IsInitializing && radioButtonCompressed.Checked)
            { SetDataSource(Project.GetCompressedSeries()); }
        }

        void radioButtonPhased_CheckedChanged(object sender, EventArgs e)
        {
            if (!IsInitializing && radioButtonPhased.Checked)
            { SetDataSource(Project.GetPhasedSeries()); }
        }

        private void textBoxP_TextChanged(object sender, EventArgs e)
        {
            if (IsInitializing)
            { return; }

            Project.SetM0AndPString(textBoxM0.Text, textBoxP.Text);
            if (radioButtonPhased.Checked)
            { SetDataSource(Project.GetPhasedSeries(), true); }
        }

        private void SetDataSource(IDataSeries dataSeries, bool doUpdate = false)
        {
            if (doUpdate && (dataSeries != null))
            { graph.UpdateDataSource(dataSeries); }
            else
            { graph.SetDataSource(dataSeries); }
        }

        private void LoadFile(string filename)
        {
            LoadFiles(new string[] { filename });
        }

        private void LoadFiles(IEnumerable<string> filenames)
        {
            try
            {
                IsInitializing = true;
                var project = Silicups.Core.Project.CreateFromDataFiles(filenames);
                this.Project = project;
                FinishLoadFile();
            }
            finally
            {
                IsInitializing = false;
            }
        }

        private void FinishLoadFile()
        {
            textBoxM0.Text = Project.GetM0String();
            textBoxP.Text = Project.GetPString();

            listBoxObs.Items.Clear();
            if(Project != null)
            {
                foreach (DataSetDescription description in Project.GetDescriptions())
                { listBoxObs.Items.Add(description.Path, true); }
            }

            radioButtonTimeseries.Checked = true;
            SetDataSource(Project.GetTimeSeries());
        }

        // menu

        private void loadFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var fd = new OpenFileDialog();
            fd.Filter = "Text Files (.txt)|*.txt|All Files (*.*)|*.*";
            fd.Multiselect = true;
            DialogResult dialogResult = fd.ShowDialog();
            if (dialogResult == DialogResult.OK)
            { LoadFiles(fd.FileNames); }
        }

        private void loadFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var fd = new FilesSelectForm();
            DialogResult dialogResult = fd.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                try
                {
                    LoadFiles(fd.FileNames);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
