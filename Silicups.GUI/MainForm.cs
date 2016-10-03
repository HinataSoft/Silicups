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
        enum SeriesTypeEnum
        {
            Timed,
            Compressed,
            Phased,
        }

        private Project Project = null;
        private IDataSeries CurrentDataSeries = null;
        private SeriesTypeEnum CurrentDataSeriesType = SeriesTypeEnum.Timed;
        private IDataSetMetadata SelectedMetadata = null;
        private bool IsInitializing = false;

        public MainForm()
        {
            InitializeComponent();

            radioButtonTimeseries.CheckedChanged += new EventHandler(radioButtonTimeseries_CheckedChanged);
            radioButtonPhased.CheckedChanged += new EventHandler(radioButtonPhased_CheckedChanged);
            radioButtonCompressed.CheckedChanged += new EventHandler(radioButtonCompressed_CheckedChanged);
            listBoxObs.ItemCheck += new ItemCheckEventHandler(listBoxObs_ItemCheck);
            listBoxObs.SelectedIndexChanged += new EventHandler(listBoxObs_SelectedIndexChanged);
            textBoxOffset.TextChanged += new EventHandler(textBoxOffset_TextChanged);

            if (System.IO.File.Exists("autoload.txt"))
            { LoadFile("autoload.txt"); }
        }

        void radioButtonTimeseries_CheckedChanged(object sender, EventArgs e)
        {
            if (!IsInitializing && radioButtonTimeseries.Checked)
            { SetDataSource(SeriesTypeEnum.Timed); }
        }

        void radioButtonCompressed_CheckedChanged(object sender, EventArgs e)
        {
            if (!IsInitializing && radioButtonCompressed.Checked)
            { SetDataSource(SeriesTypeEnum.Compressed); }
        }

        void radioButtonPhased_CheckedChanged(object sender, EventArgs e)
        {
            if (!IsInitializing && radioButtonPhased.Checked)
            { SetDataSource(SeriesTypeEnum.Phased); }
        }

        private void textBoxP_TextChanged(object sender, EventArgs e)
        {
            if (IsInitializing)
            { return; }

            Project.SetM0AndPString(textBoxM0.Text, textBoxP.Text);
            if (radioButtonPhased.Checked)
            { SetDataSource(SeriesTypeEnum.Phased, true); }
        }

        void listBoxObs_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (IsInitializing)
            { return; }

            var metadata = (IDataSetMetadata)listBoxObs.Items[e.Index];
            metadata.Enabled = (e.NewValue == CheckState.Checked);
            RefreshDataSource();
        }

        void listBoxObs_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsInitializing)
            { return; }

            SelectedMetadata = null;
            IDataSetMetadata selectedMetadata = null;
            int selectedIndex = listBoxObs.SelectedIndex;
            for (int i = 0; i < listBoxObs.Items.Count; i++)
            {
                var metadata = (IDataSetMetadata)listBoxObs.Items[i];
                if(i == selectedIndex)
                {
                    metadata.Hightlighted = true;
                    selectedMetadata = metadata;
                }
                else
                {
                    metadata.Hightlighted = false;
                }
            }
            if (selectedMetadata != null)
            {
                textBoxOffset.Text = MathEx.FormatDouble(selectedMetadata.OffsetY);
                textBoxOffset.Enabled = true;
            }
            else
            {
                textBoxOffset.Text = "";
                textBoxOffset.Enabled = false;
            }
            SelectedMetadata = selectedMetadata;
            UpdateDataSource(true);
        }

        void textBoxOffset_TextChanged(object sender, EventArgs e)
        {
            if (IsInitializing || (SelectedMetadata == null))
            { return; }

            try
            {
                SelectedMetadata.OffsetY = MathEx.ParseDouble(textBoxOffset.Text);
                RefreshDataSource();
            }
            catch
            { }
        }

        private void SetDataSource(SeriesTypeEnum dataSeriesType, bool doUpdate = false)
        {
            IDataSeries dataSeries = null;
            switch (dataSeriesType)
            {
                case SeriesTypeEnum.Timed: dataSeries = Project.GetTimeSeries(); break;
                case SeriesTypeEnum.Compressed: dataSeries = Project.GetCompressedSeries(); break;
                case SeriesTypeEnum.Phased: dataSeries = Project.GetPhasedSeries(); break;
            }
            CurrentDataSeriesType = dataSeriesType;

            if (CurrentDataSeries != dataSeries)
            { doUpdate = false; }
            CurrentDataSeries = dataSeries;

            if (doUpdate && (CurrentDataSeries != null))
            { graph.UpdateDataSource(CurrentDataSeries); }
            else
            { graph.SetDataSource(CurrentDataSeries); }
        }

        private void UpdateDataSource(bool doUpdate = false)
        {
            SetDataSource(CurrentDataSeriesType, doUpdate);
        }

        private void RefreshDataSource(bool doUpdate = false)
        {
            Project.Refresh();
            SetDataSource(CurrentDataSeriesType, doUpdate);
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

            SelectedMetadata = null;
            listBoxObs.Items.Clear();
            if(Project != null)
            {
                foreach (IDataSetMetadata metadata in Project.GetMetadata())
                { listBoxObs.Items.Add(metadata, metadata.Enabled); }
            }

            radioButtonTimeseries.Checked = true;
            SetDataSource(SeriesTypeEnum.Timed);
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
