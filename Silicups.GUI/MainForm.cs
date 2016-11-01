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

        private static readonly string RegistryPath = @"SOFTWARE\HinataSoft\Silicups\MainForm";

        private List<Project> Solution = new List<Project>();
        private Project CurrentProject = null;
        private IDataSeries CurrentDataSeries = null;
        private SeriesTypeEnum CurrentDataSeriesType = SeriesTypeEnum.Timed;
        private IDataSetMetadata SelectedMetadata = null;
        private bool IsInitializing = false;

        private double? OriginalP = null;
        private double? OriginalOffset = null;

        public MainForm()
        {
            InitializeComponent();

            try
            {
                var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(RegistryPath);
                if (key != null)
                {
                    checkBoxStyle.Checked = (key.GetValue("GliderStyle").ToString() == "1");
                }
            }
            catch
            { }

            radioButtonTimeseries.CheckedChanged += new EventHandler(radioButtonTimeseries_CheckedChanged);
            radioButtonPhased.CheckedChanged += new EventHandler(radioButtonPhased_CheckedChanged);
            radioButtonCompressed.CheckedChanged += new EventHandler(radioButtonCompressed_CheckedChanged);
            listBoxSolution.SelectedIndexChanged += new EventHandler(listBoxSolution_SelectedIndexChanged);
            listBoxSolution.KeyDown += new KeyEventHandler(listBoxSolution_KeyDown);
            listBoxObs.ItemCheck += new ItemCheckEventHandler(listBoxObs_ItemCheck);
            listBoxObs.SelectedIndexChanged += new EventHandler(listBoxObs_SelectedIndexChanged);
            listBoxObs.KeyDown += new KeyEventHandler(listBoxObs_KeyDown);
            textBoxOffset.TextChanged += new EventHandler(textBoxOffset_TextChanged);
            textBoxPPM.TextChanged += new EventHandler(textBoxPPM_TextChanged);
            textBoxOffsetPM.TextChanged += new EventHandler(textBoxOffsetPM_TextChanged);
            gliderP.GliderValueChanged += new Glider.GliderEventHandler(gliderP_GliderValueChanged);
            gliderP.GliderValueConfirmed += new Glider.GliderEventHandler(gliderP_GliderValueConfirmed);
            gliderOffset.GliderValueChanged += new Glider.GliderEventHandler(gliderOffset_GliderValueChanged);
            gliderOffset.GliderValueConfirmed += new Glider.GliderEventHandler(gliderOffset_GliderValueConfirmed);
            trackBarP.ValueChanged += new EventHandler(trackBarP_ValueChanged);
            trackBarOffset.ValueChanged += new EventHandler(trackBarOffset_ValueChanged);
            buttonSetOffset.Click += new EventHandler(buttonSetOffset_Click);
            buttonSetP.Click += new EventHandler(buttonSetP_Click);
            buttonZeroP.Click += new EventHandler(buttonZeroP_Click);
            buttonZeroOffset.Click += new EventHandler(buttonZeroOffset_Click);

            this.FormClosing += new FormClosingEventHandler(MainForm_FormClosing);

            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(MainForm_KeyDown);

            RefreshGliderStyle();
            checkBoxStyle.CheckedChanged += new EventHandler(checkBoxStyle_CheckedChanged);

#if DEBUG
            if (System.IO.File.Exists("autoload.xml"))
            { LoadSolution("autoload.xml"); }
#endif
        }

        void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                gliderP.ConfirmPosition();
                gliderOffset.ConfirmPosition();
            }
        }

        void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            var key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(RegistryPath);
            if (key != null)
            {
                key.SetValue("GliderStyle", checkBoxStyle.Checked ? 1 : 0, Microsoft.Win32.RegistryValueKind.DWord);
            }
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
            if (IsInitializing || (CurrentProject == null))
            { return; }

            CurrentProject.SetM0AndPString(textBoxM0.Text, textBoxP.Text);
            if (radioButtonPhased.Checked)
            { SetDataSource(SeriesTypeEnum.Phased, true); }
        }

        void textBoxPPM_TextChanged(object sender, EventArgs e)
        {
            if (IsInitializing || (CurrentProject == null))
            { return; }

            try
            { CurrentProject.PAmplitude = MathEx.ParseDouble(textBoxPPM.Text); }
            catch
            { }
        }

        void gliderP_GliderValueChanged(object sender, Glider.GliderEventArgs e)
        {
            RefineGliderTarget(ref OriginalP, e.GliderValue, textBoxP, textBoxPPM);
        }

        void gliderP_GliderValueConfirmed(object sender, Glider.GliderEventArgs e)
        {
            SetGliderTarget(ref OriginalP, e.GliderValue, textBoxP, textBoxPPM);
        }

        bool trackBarP_SuppressEvent = false;
        void trackBarP_ValueChanged(object sender, EventArgs e)
        {
            if (trackBarP_SuppressEvent) { return; }
            RefineGliderTarget(ref OriginalP, trackBarP.NormalizedValue(), textBoxP, textBoxPPM);
        }

        void buttonSetP_Click(object sender, EventArgs e)
        {
            try
            {
                trackBarP_SuppressEvent = false;
                SetGliderTarget(ref OriginalP, trackBarP.NormalizedValue(), textBoxP, textBoxPPM);
                trackBarP.Value = 0;
            }
            finally
            { trackBarP_SuppressEvent = false; }
        }

        void buttonZeroP_Click(object sender, EventArgs e)
        {
            trackBarP.Value = 0;
        }

        private static void RefineGliderTarget(ref double? target, double gliderValue, TextBox targetBox, TextBox amplitudeBox)
        {
            if (!target.HasValue)
            { return; }
            try
            { targetBox.Text = MathEx.FormatDouble(target.Value + gliderValue * MathEx.ParseDouble(amplitudeBox.Text)); }
            catch { }
        }

        private static void SetGliderTarget(ref double? target, double gliderValue, TextBox targetBox, TextBox amplitudeBox)
        {
            if (!target.HasValue)
            { return; }
            try
            {
                target += gliderValue * MathEx.ParseDouble(amplitudeBox.Text);
                targetBox.Text = MathEx.FormatDouble(target.Value);
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

        void textBoxOffsetPM_TextChanged(object sender, EventArgs e)
        {
            if (IsInitializing || (CurrentProject == null))
            { return; }

            try
            { CurrentProject.OffsetAmplitude = MathEx.ParseDouble(textBoxOffsetPM.Text); }
            catch
            { }
        }

        void gliderOffset_GliderValueChanged(object sender, Glider.GliderEventArgs e)
        {
            RefineGliderTarget(ref OriginalOffset, e.GliderValue, textBoxOffset, textBoxOffsetPM);
        }

        void gliderOffset_GliderValueConfirmed(object sender, Glider.GliderEventArgs e)
        {
            SetGliderTarget(ref OriginalOffset, e.GliderValue, textBoxOffset, textBoxOffsetPM);
        }

        bool trackBarOffset_SuppressEvent = false;
        void trackBarOffset_ValueChanged(object sender, EventArgs e)
        {
            if (trackBarOffset_SuppressEvent) { return; }
            RefineGliderTarget(ref OriginalOffset, trackBarOffset.NormalizedValue(), textBoxOffset, textBoxOffsetPM);
        }

        void buttonSetOffset_Click(object sender, EventArgs e)
        {
            try
            {
                trackBarOffset_SuppressEvent = true;
                SetGliderTarget(ref OriginalOffset, trackBarOffset.NormalizedValue(), textBoxOffset, textBoxOffsetPM);
                trackBarOffset.Value = 0;
            }
            finally
            { trackBarOffset_SuppressEvent = false; }
        }

        void buttonZeroOffset_Click(object sender, EventArgs e)
        {
            trackBarOffset.Value = 0;
        }

        // solution list box

        void listBoxSolution_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsInitializing)
            { return; }

            try
            {
                IsInitializing = true;
                CurrentProject = (Project)listBoxSolution.SelectedItem;
                trackBarP.Value = 0;
                trackBarOffset.Value = 0;
                RefreshCurrentProject();
            }
            finally
            {
                IsInitializing = false;
            }
        }

        void listBoxSolution_KeyDown(object sender, KeyEventArgs e)
        {
            if (IsInitializing || (CurrentProject == null))
            { return; }

            if (e.KeyCode == Keys.F2)
            {
                using (var form = new InputBoxForm("Object name:", CurrentProject.Caption))
                {
                    if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        CurrentProject.Caption = form.PromptValue;
                        listBoxSolution.RefreshItems();
                    }
                }
            }

            if (e.KeyCode == Keys.Delete)
            {
                DialogResult result = MessageBox.Show("Really delete the object from the solution?", "Confirmation", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if(result == DialogResult.OK)
                { RemoveProjectFromSolution(CurrentProject); }
            }
        }

        // observation list box

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

            try
            {
                IsInitializing = true;
                SelectedMetadata = null;
                IDataSetMetadata selectedMetadata = null;
                int selectedIndex = listBoxObs.SelectedIndex;
                for (int i = 0; i < listBoxObs.Items.Count; i++)
                {
                    var metadata = (IDataSetMetadata)listBoxObs.Items[i];
                    if (i == selectedIndex)
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
                trackBarOffset.Value = 0;
                SelectedMetadata = selectedMetadata;
                UpdateDataSource(true);
            }
            finally
            {
                IsInitializing = false;
            }
        }

        void listBoxObs_KeyDown(object sender, KeyEventArgs e)
        {
            if (IsInitializing || (SelectedMetadata == null))
            { return; }

            if (e.KeyCode == Keys.F2)
            {
                using (var form = new InputBoxForm("Set name:", SelectedMetadata.Caption))
                {
                    if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        SelectedMetadata.Caption = form.PromptValue;
                        listBoxObs.Refresh();
                    }
                }
            }

            if (e.KeyCode == Keys.Delete)
            {
                DialogResult result = MessageBox.Show("Really delete the set from the project?", "Confirmation", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (result == DialogResult.OK)
                { RemoveSetFromProject(SelectedMetadata); }
            }
        }

        // glider style toggle

        private void RefreshGliderStyle()
        {
            if (checkBoxStyle.Checked)
            {
                gliderP.Visible = false;
                gliderOffset.Visible = false;
                trackBarP.Visible = true;
                trackBarOffset.Visible = true;
                buttonSetP.Visible = true;
                buttonSetOffset.Visible = true;
                buttonZeroP.Visible = true;
                buttonZeroOffset.Visible = true;
            }
            else
            {
                gliderP.Visible = true;
                gliderOffset.Visible = true;
                trackBarP.Visible = false;
                trackBarOffset.Visible = false;
                buttonSetP.Visible = false;
                buttonSetOffset.Visible = false;
                buttonZeroP.Visible = false;
                buttonZeroOffset.Visible = false;
            }
        }

        void checkBoxStyle_CheckedChanged(object sender, EventArgs e)
        {
            RefreshGliderStyle();
        }

        // data sources

        private void SetDataSource(SeriesTypeEnum dataSeriesType, bool doUpdate = false)
        {
            IDataSeries dataSeries = null;
            if (CurrentProject != null)
            {
                switch (dataSeriesType)
                {
                    case SeriesTypeEnum.Timed: dataSeries = CurrentProject.TimeSeries; break;
                    case SeriesTypeEnum.Compressed: dataSeries = CurrentProject.CompressedSeries; break;
                    case SeriesTypeEnum.Phased: dataSeries = CurrentProject.PhasedSeries; break;
                }
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
            CurrentProject.Refresh();
            SetDataSource(CurrentDataSeriesType, doUpdate);
        }

        // files

        private void NewSolution()
        {
            try
            {
                IsInitializing = true;
                RenewSolution(new Project[] { new Project() } );
            }
            finally
            {
                IsInitializing = false;
            }
        }

        private void AddNewProjectToSolution()
        {
            try
            {
                IsInitializing = true;
                AddProjectToSolution(new Project());
            }
            finally
            {
                IsInitializing = false;
            }
        }

        private void RemoveProjectFromSolution(Project project)
        {
            try
            {
                IsInitializing = true;
                int currentlySelected = listBoxSolution.SelectedIndex;
                Solution.Remove(project);
                listBoxSolution.Items.Remove(project);
                if (listBoxSolution.Items.Count == 0)
                { CurrentProject = null; }
                else
                {
                    if (currentlySelected >= listBoxSolution.Items.Count)
                    { currentlySelected = listBoxSolution.Items.Count - 1; }
                    listBoxSolution.SelectedIndex = currentlySelected;
                    CurrentProject = (Project)listBoxSolution.SelectedItem;
                }
                RefreshCurrentProject();
            }
            finally
            {
                IsInitializing = false;
            }
        }

        private void LoadSolution(string path)
        {
            try
            {
                IsInitializing = true;
                var solution = new List<Project>();
                var doc = XmlHelper.LoadXml(path);
                XmlNode rootNode = doc["Silicups"];

                var projects = new Dictionary<string, Project>();
                var projectsUsed = new HashSet<string>();
                foreach (XmlNode projectNode in rootNode.FindNodes("Projects").FindNodes("Project"))
                {
                    string id = projectNode.GetAttribute("id").AsString();
                    Project project = new Core.Project();
                    project.LoadFromXml(projectNode);
                    projects.Add(id, project);
                }

                foreach (XmlNode projectNode in rootNode.FindNodes("Solution").FindNodes("Project"))
                {
                    string id = projectNode.GetAttribute("id").AsString();
                    solution.Add(projects[id]);
                    projectsUsed.Add(id);
                }

                int projectsNotUsed = 0;
                foreach (string id in projects.Keys)
                {
                    if (!projectsUsed.Contains(id))
                    { projectsNotUsed++; }
                }
                if (projectsNotUsed > 0)
                { MessageBox.Show(String.Format("{0} project(s) in the solution file has not been included in the solution", projectsNotUsed), "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning); }

                if (solution.Count == 0)
                {
                    MessageBox.Show("No project found in the solution file", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                RenewSolution(solution);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Exception when loading the solution", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                IsInitializing = false;
            }
        }

        private void SaveSolution(string path)
        {
            var doc = new XmlDocument();
            XmlNode rootNode = doc.AppendXmlElement("Silicups");
            XmlNode solutionNode = rootNode.AppendXmlElement("Solution");
            XmlNode projectsNode = rootNode.AppendXmlElement("Projects");
            foreach (Project project in Solution)
            {
                solutionNode.AppendXmlElement("Project").AppendXmlAttribute("id", project.Id);
                XmlNode projectNode = projectsNode.AppendXmlElement("Project");
                project.SaveToXml(projectNode);
            }
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
                if (CurrentProject == null)
                { return; }
                CurrentProject.AddDataFiles(filenames);
                RefreshCurrentProject();
            }
            finally
            {
                IsInitializing = false;
            }
        }

        private void LoadFiles(string baseDirectory, string pattern, string filter)
        {
            try
            {
                IsInitializing = true;
                if (CurrentProject == null)
                { return; }
                CurrentProject.AddDataFiles(baseDirectory, pattern, filter);
                RefreshCurrentProject();
            }
            finally
            {
                IsInitializing = false;
            }
        }

        private void SaveToTxt(string filename)
        {
            if (CurrentProject == null)
            { return; }

            using (var writer = new System.IO.StreamWriter(filename))
            {
                //writer.WriteLine("Data for project {0} (Silicups)", CurrentProject.Caption ?? CurrentProject.Id);
                //writer.WriteLine("JD MAG ERR");
                foreach (IDataSet set in CurrentProject.TimeSeries.Series)
                {
                    if (!set.Metadata.Enabled)
                    { continue; }
                    foreach (DataPoint point in set.Set)
                    {
                        writer.WriteLine("{0} {1} {2}",
                            MathEx.FormatDouble(point.X),
                            MathEx.FormatDouble(point.Y),
                            MathEx.FormatDouble(point.Yerr)
                        );
                    }
                }
            }
        }

        private void RenewSolution(IEnumerable<Project> projects)
        {
            Solution.Clear();
            listBoxSolution.Items.Clear();
            CurrentProject = null;

            Solution.AddRange(projects);
            if (Solution.Count > 0)
            {
                foreach (Project project in Solution)
                { listBoxSolution.Items.Add(project); }
                listBoxSolution.SelectedIndex = 0;
                CurrentProject = (Project)listBoxSolution.SelectedItem;
            }

            radioButtonTimeseries.Checked = true;
            SetDataSource(SeriesTypeEnum.Timed);

            RefreshCurrentProject();
        }

        private void AddProjectToSolution(Project project)
        {
            Solution.Add(project);
            listBoxSolution.Items.Add(project);
            listBoxSolution.SelectedIndex = listBoxSolution.Items.Count - 1;
            CurrentProject = (Project)listBoxSolution.SelectedItem;
            radioButtonTimeseries.Checked = true;
            SetDataSource(SeriesTypeEnum.Timed);
            RefreshCurrentProject();
        }

        private void ToggleControls(IEnumerable<Control> controls, bool enabled)
        {
            foreach (Control control in controls)
            {
                control.Enabled = enabled;
                if (!enabled && (control is TextBox))
                { control.Text = null; }
            }
        }

        private void RefreshCurrentProject()
        {
            var controls = new Control[] {
                radioButtonTimeseries,
                radioButtonCompressed,
                radioButtonPhased,
                textBoxM0,
                textBoxP,
                textBoxPPM,
                textBoxOffsetPM,
                gliderP,
                gliderOffset
            };

            SelectedMetadata = null;
            listBoxObs.Items.Clear();

            if (CurrentProject == null)
            {
                OriginalP = null;
                ToggleControls(controls, false);
                loadFileToolStripMenuItem.Enabled = false;
                loadFilesToolStripMenuItem.Enabled = false;
                graph.SetDataSource(null);
                return;
            }
            ToggleControls(controls, true);
            loadFileToolStripMenuItem.Enabled = true;
            loadFilesToolStripMenuItem.Enabled = true;

            OriginalP = CurrentProject.P;
            textBoxM0.Text = MathEx.FormatDouble(CurrentProject.M0);
            textBoxP.Text = MathEx.FormatDouble(CurrentProject.P);
            textBoxPPM.Text = MathEx.FormatDouble(CurrentProject.PAmplitude);
            textBoxOffsetPM.Text = MathEx.FormatDouble(CurrentProject.OffsetAmplitude);

            gliderP.Enabled = CurrentProject.P.HasValue && CurrentProject.PAmplitude.HasValue;
            gliderOffset.Enabled = false;

            foreach (IDataSetMetadata metadata in CurrentProject.GetMetadata())
            { listBoxObs.Items.Add(metadata, metadata.Enabled); }

            UpdateDataSource();
        }

        private void RemoveSetFromProject(IDataSetMetadata metadata)
        {
            if (CurrentProject == null)
            { return; }

            if (!CurrentProject.RemoveSet(metadata))
            { return; }

            RefreshCurrentProject();
        }

        // menu

        private void newProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewSolution();
        }

        private void loadSolutionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using(var fd = new OpenFileDialog())
            {
                fd.Filter = "XML Files (.xml)|*.xml|All Files (*.*)|*.*";
                DialogResult dialogResult = fd.ShowDialog();
                if (dialogResult == DialogResult.OK)
                { LoadSolution(fd.FileName); }
            }
        }

        private void saveSolutionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var fd = new SaveFileDialog())
            {
                fd.Filter = "XML Files (.xml)|*.xml|All Files (*.*)|*.*";
                DialogResult dialogResult = fd.ShowDialog();
                if (dialogResult == DialogResult.OK)
                { SaveSolution(fd.FileName); }
            }
        }

        private void addNewProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddNewProjectToSolution();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void loadFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var fd = new OpenFileDialog())
            {
                fd.Filter = "Text Files (.txt)|*.txt|All Files (*.*)|*.*";
                fd.Multiselect = true;
                DialogResult dialogResult = fd.ShowDialog();
                if (dialogResult == DialogResult.OK)
                { LoadFiles(fd.FileNames); }
            }
        }

        private void loadFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var fd = new FilesSelectForm())
            {
                DialogResult dialogResult = fd.ShowDialog();
                if (dialogResult == DialogResult.OK)
                {
                    try
                    { LoadFiles(fd.SelectedDirectory, fd.SelectedPattern, fd.SelectedFilter); }
                    catch (Exception ex)
                    { MessageBox.Show(ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                }
            }
        }

        private void exportToTxtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var fd = new SaveFileDialog())
            {
                fd.Filter = "Text Files (.txt)|*.txt|All Files (*.*)|*.*";
                DialogResult dialogResult = fd.ShowDialog();
                if (dialogResult == DialogResult.OK)
                { SaveToTxt(fd.FileName); }
            }
        }
    }

    public class MyListBox : ListBox
    {
        public new void RefreshItems()
        {
            base.RefreshItems();
        }
    }

    public static class TrackBarExtensions
    {
        public static double NormalizedValue(this TrackBar trackBar)
        {
            return trackBar.Value * 1.0 / trackBar.Maximum;
        }
    }
}
