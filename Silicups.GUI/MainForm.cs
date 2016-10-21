using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;

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

        private double? OriginalP = null;
        private double? OriginalOffset = null;

        public MainForm()
        {
            InitializeComponent();

            radioButtonTimeseries.CheckedChanged += new EventHandler(radioButtonTimeseries_CheckedChanged);
            radioButtonPhased.CheckedChanged += new EventHandler(radioButtonPhased_CheckedChanged);
            radioButtonCompressed.CheckedChanged += new EventHandler(radioButtonCompressed_CheckedChanged);
            listBoxObs.ItemCheck += new ItemCheckEventHandler(listBoxObs_ItemCheck);
            listBoxObs.SelectedIndexChanged += new EventHandler(listBoxObs_SelectedIndexChanged);
            textBoxOffset.TextChanged += new EventHandler(textBoxOffset_TextChanged);
            gliderP.GliderValueChanged += new Glider.GliderEventHandler(gliderP_GliderValueChanged);
            gliderP.GliderValueConfirmed += new Glider.GliderEventHandler(gliderP_GliderValueConfirmed);
            gliderOffset.GliderValueChanged += new Glider.GliderEventHandler(gliderOffset_GliderValueChanged);
            gliderOffset.GliderValueConfirmed += new Glider.GliderEventHandler(gliderOffset_GliderValueConfirmed);

#if DEBUG
            if (System.IO.File.Exists("autoload.xml"))
            { LoadProject("autoload.xml"); }
            else if (System.IO.File.Exists("autoload.txt"))
            { LoadFile("autoload.txt"); }
#endif
        }

        // radio buttons

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

        // M0 + P

        private void textBoxP_TextChanged(object sender, EventArgs e)
        {
            if (IsInitializing)
            { return; }

            Project.SetM0AndPString(textBoxM0.Text, textBoxP.Text);
            if (radioButtonPhased.Checked)
            { SetDataSource(SeriesTypeEnum.Phased, true); }
        }

        void gliderP_GliderValueChanged(object sender, Glider.GliderEventArgs e)
        {
            if (OriginalP == null)
            { return; }
            try
            { textBoxP.Text = MathEx.FormatDouble(OriginalP.Value + e.GliderValue * MathEx.ParseDouble(textBoxPPM.Text));  }
            catch { }
        }

        void gliderP_GliderValueConfirmed(object sender, Glider.GliderEventArgs e)
        {
            if (OriginalP == null)
            { return; }
            try
            {
                OriginalP += e.GliderValue * MathEx.ParseDouble(textBoxPPM.Text);
                textBoxP.Text = MathEx.FormatDouble(OriginalP.Value);
            }
            catch { }
        }

        // offset

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

        void gliderOffset_GliderValueChanged(object sender, Glider.GliderEventArgs e)
        {
            if (OriginalOffset == null)
            { return; }
            try
            { textBoxOffset.Text = MathEx.FormatDouble(OriginalOffset.Value + e.GliderValue * MathEx.ParseDouble(textBoxOffsetPM.Text)); }
            catch { }
        }

        void gliderOffset_GliderValueConfirmed(object sender, Glider.GliderEventArgs e)
        {
            if (OriginalOffset == null)
            { return; }
            try
            {
                OriginalOffset += e.GliderValue * MathEx.ParseDouble(textBoxOffsetPM.Text);
                textBoxOffset.Text = MathEx.FormatDouble(OriginalOffset.Value);
            }
            catch { }
        }

        // list box

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
                OriginalOffset = selectedMetadata.OffsetY;
                textBoxOffset.Text = MathEx.FormatDouble(selectedMetadata.OffsetY);
                textBoxOffset.Enabled = true;
                gliderOffset.Enabled = true;
            }
            else
            {
                OriginalOffset = null;
                textBoxOffset.Text = "";
                textBoxOffset.Enabled = false;
                gliderOffset.Enabled = false;
            }
            SelectedMetadata = selectedMetadata;
            UpdateDataSource(true);
        }

        // data sources

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

        // files

        private void NewProject()
        {
            try
            {
                IsInitializing = true;
                Project = new Core.Project();
                RefreshProject();
            }
            finally
            {
                IsInitializing = false;
            }
        }

        private void LoadProject(string path)
        {
            try
            {
                IsInitializing = true;
                var doc = XmlHelper.LoadXml(path);
                XmlNode rootNode = doc["Silicups"];

                XmlNode projectNode = rootNode.FindOneNode("Project");
                Project = new Core.Project();
                if (projectNode != null)
                { Project.AddFromXml(projectNode); }

                RefreshProject();

                XmlNode guiNode = rootNode.FindOneNode("GUI");
                if (guiNode != null)
                {
                    textBoxPPM.Text = guiNode.FindAttribute("pAmplitude").AsString(textBoxPPM.Text);
                    textBoxOffsetPM.Text = guiNode.FindAttribute("offsetAmplitude").AsString(textBoxOffsetPM.Text);
                }
            }
            finally
            {
                IsInitializing = false;
            }
        }

        private void SaveProject(string path)
        {
            var doc = new XmlDocument();
            XmlNode rootNode = doc.AppendXmlElement("Silicups");
            XmlNode projectsNode = rootNode.AppendXmlElement("Projects");
            XmlNode projectNode = projectsNode.AppendXmlElement("Project");
            if (Project != null)
            { Project.SaveToXml(projectNode); }
            XmlNode guiNode = projectNode.AppendXmlElement("GUI");
            guiNode.AppendXmlAttribute("pAmplitude", textBoxPPM.Text);
            guiNode.AppendXmlAttribute("offsetAmplitude", textBoxOffsetPM.Text);
            doc.Save(path);
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
                if (Project == null)
                { Project = new Core.Project(); }
                Project.AddDataFiles(filenames);
                RefreshProject();
            }
            finally
            {
                IsInitializing = false;
            }
        }

        private void RefreshProject()
        {
            OriginalP = Project.P;
            textBoxM0.Text = Project.GetM0String();
            textBoxP.Text = Project.GetPString();

            try
            {
                double pBase = MathEx.GetLower125Base(Project.P.Value);
                textBoxPPM.Text = MathEx.FormatDouble(pBase);
                gliderP.Enabled = true;
            }
            catch
            {
                textBoxPPM.Text = "";
                gliderP.Enabled = false;
            }

            SelectedMetadata = null;
            listBoxObs.Items.Clear();
            if(Project != null)
            {
                foreach (IDataSetMetadata metadata in Project.GetMetadata())
                { listBoxObs.Items.Add(metadata, metadata.Enabled); }
            }

            radioButtonTimeseries.Checked = true;
            SetDataSource(SeriesTypeEnum.Timed);

            textBoxOffsetPM.Text = (CurrentDataSeries != null)
                ? MathEx.FormatDouble(MathEx.GetLower125Base(CurrentDataSeries.BoundingBox.Height))
                : null;
            OriginalOffset = null;
            gliderOffset.Enabled = false;
        }

        // menu


        private void newProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewProject();
        }

        private void loadProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var fd = new OpenFileDialog();
            fd.Filter = "XML Files (.xml)|*.xml|All Files (*.*)|*.*";
            DialogResult dialogResult = fd.ShowDialog();
            if (dialogResult == DialogResult.OK)
            { LoadProject(fd.FileName); }
        }

        private void saveProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var fd = new SaveFileDialog();
            fd.Filter = "XML Files (.xml)|*.xml|All Files (*.*)|*.*";
            DialogResult dialogResult = fd.ShowDialog();
            if (dialogResult == DialogResult.OK)
            { SaveProject(fd.FileName); }
        }

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
