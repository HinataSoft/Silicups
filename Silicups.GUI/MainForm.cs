using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;

using Silicups.Core;
using Silicups.Util;

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

        private Version Version;
        private List<Project> Solution = new List<Project>();
        private Project CurrentProject = null;
        private IDataSeries CurrentDataSeries = null;
        private SeriesTypeEnum CurrentDataSeriesType = SeriesTypeEnum.Timed;
        private IDataSetMetadata SelectedMetadata = null;
        private bool IsInitializing = false;
        private bool IsDirty = false;
        private string CurrentSolutionFile = null;

        private double? OriginalP = null;
        private double? OriginalOffset = null;

        public MainForm()
        {
            InitializeComponent();
            ResetTitle();

            this.Version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            DateTime buildDate = new DateTime(2000, 1, 1).AddDays(Version.Build).AddSeconds(Version.Revision * 2);
            toolStripStatusLabel1.Text = String.Format("Silicups {0} (built {1})", Version, buildDate);

            checkBoxBinning.Checked = false;
            textBoxBinning.Text = "0.01";
            RegistryHelper.TryGetFromRegistry(RegistryPath,
                new RegistryHelper.GetRegistryStringAction("GliderStyle", (s) => { checkBoxStyle.Checked = (s == "1"); } ),
                new RegistryHelper.GetRegistryStringAction("BinningEnabled", (s) => { checkBoxBinning.Checked = (s == "1"); } ),
                new RegistryHelper.GetRegistryStringAction("BinningValue", (s) => { textBoxBinning.Text = s; } )
            );

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
            textBoxP.AfterPaste += new Action(textBoxP_AfterPaste);
            textBoxOffset.AfterPaste += new Action(textBoxOffset_AfterPaste);
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
            checkBoxBinning.CheckedChanged += new EventHandler(checkBoxBinning_CheckedChanged);
            textBoxBinning.TextChanged += new EventHandler(textBoxBinning_TextChanged);

            this.FormClosing += new FormClosingEventHandler(MainForm_FormClosing);

            this.KeyPreview = true;
            this.KeyDown += new KeyEventHandler(MainForm_KeyDown);

            RefreshGliderStyle();
            checkBoxStyle.CheckedChanged += new EventHandler(checkBoxStyle_CheckedChanged);

            setCaptionToolStripMenuItem.Enabled = false;

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
                e.Handled = true;
                return;
            }
            if (!e.Control && ((e.KeyCode == Keys.A) || (e.KeyCode == Keys.Q)))
            {
                if ((CurrentProject != null) && (listBoxObs.Items.Count > 0))
                {
                    if (listBoxObs.SelectedIndex < 0)
                    {
                        listBoxObs.SelectedIndex = 0;
                        return;
                    }
                    if ((e.KeyCode == Keys.A) && (listBoxObs.SelectedIndex + 1 < listBoxObs.Items.Count))
                    { listBoxObs.SelectedIndex++; }
                    if ((e.KeyCode == Keys.Q) && (listBoxObs.SelectedIndex > 0))
                    { listBoxObs.SelectedIndex--; }
                    e.Handled = true;
                    return;
                }
            }
            if (e.Control && (e.KeyCode == Keys.B))
            {
                Clipboard.SetText(Version.ToString());
                e.Handled = true;
                return;
            }
        }

        void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!ClosingConfirmed())
            {
                e.Cancel = true;
                return;
            }

            RegistryHelper.TrySetToRegistry(RegistryPath,
                new RegistryHelper.SetRegistryAction("GliderStyle", checkBoxStyle.Checked ? 1 : 0),
                new RegistryHelper.SetRegistryAction("BinningEnabled", checkBoxBinning.Checked ? 1 : 0),
                new RegistryHelper.SetRegistryAction("BinningValue", textBoxBinning.Text)
             );
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
            SetDirty();
            if (radioButtonPhased.Checked)
            { SetDataSource(SeriesTypeEnum.Phased, true); }
        }

        private void textBoxP_KeyUp(object sender, KeyEventArgs e)
        {
            textBoxP_HandleDirectChange();
        }

        void textBoxP_AfterPaste()
        {
            textBoxP_HandleDirectChange();
        }

        void textBoxP_HandleDirectChange()
        {
            if (IsInitializing || trackBarP_SuppressEvent)
            { return; }

            try
            {
                OriginalP = FormatEx.ParseDouble(textBoxP.Text);
                trackBarP_SuppressEvent = true;
                trackBarP.Value = 0;
            }
            catch
            {
            }
            finally
            {
                trackBarP_SuppressEvent = false;
            }
        }

        void textBoxPPM_TextChanged(object sender, EventArgs e)
        {
            if (IsInitializing || (CurrentProject == null))
            { return; }

            try
            { CurrentProject.PAmplitude = FormatEx.ParseDouble(textBoxPPM.Text); }
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
            { targetBox.Text = FormatEx.FormatDouble(target.Value + gliderValue * FormatEx.ParseDouble(amplitudeBox.Text)); }
            catch { }
        }

        private void SetGliderTarget(ref double? target, double gliderValue, TextBox targetBox, TextBox amplitudeBox)
        {
            if (!target.HasValue)
            { return; }
            try
            {
                target += gliderValue * FormatEx.ParseDouble(amplitudeBox.Text);
                targetBox.Text = FormatEx.FormatDouble(target.Value);
                SetDirty();
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
                SelectedMetadata.OffsetY = FormatEx.ParseDouble(textBoxOffset.Text);
                RefreshDataSource();
                SetDirty();
            }
            catch
            { }
        }

        private void textBoxOffset_KeyUp(object sender, KeyEventArgs e)
        {
            textBoxOffset_HandleDirectChange();
        }

        void textBoxOffset_AfterPaste()
        {
            textBoxOffset_HandleDirectChange();
        }

        void textBoxOffset_HandleDirectChange()
        {
            if (IsInitializing || trackBarOffset_SuppressEvent)
            { return; }

            try
            {
                OriginalOffset = FormatEx.ParseDouble(textBoxOffset.Text);
                trackBarOffset_SuppressEvent = true;
                trackBarOffset.Value = 0;
            }
            catch
            {
            }
            finally
            {
                trackBarOffset_SuppressEvent = false;
            }
        }

        void textBoxOffsetPM_TextChanged(object sender, EventArgs e)
        {
            if (IsInitializing || (CurrentProject == null))
            { return; }

            try
            { CurrentProject.OffsetAmplitude = FormatEx.ParseDouble(textBoxOffsetPM.Text); }
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
                RenameProject();
            }

            if (e.KeyCode == Keys.Delete)
            {
                DialogResult result = MessageBox.Show("Really delete the object from the solution?", "Confirmation", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if(result == DialogResult.OK)
                { RemoveProjectFromSolution(CurrentProject); SetDirty(); }
            }

            if (e.Control && ((e.KeyCode == Keys.Up) || (e.KeyCode == Keys.Down)))
            {
                if ((listBoxSolution.Items.Count > 0) && (listBoxSolution.SelectedIndex >= 0))
                {
                    int index = listBoxSolution.SelectedIndex;
                    if ((e.KeyCode == Keys.Up) && (listBoxSolution.SelectedIndex > 0))
                    { IListExtensions.Swap(Solution, listBoxSolution.Items, index - 1, index); listBoxSolution.SelectedIndex--; }
                    if ((e.KeyCode == Keys.Down) && (listBoxSolution.SelectedIndex + 1 < listBoxSolution.Items.Count))
                    { IListExtensions.Swap(Solution, listBoxSolution.Items, index, index + 1); listBoxSolution.SelectedIndex++; }
                    SetDirty();
                    e.Handled = true;
                    return;
                }
            }
        }

        private void RenameProject()
        {
            if (IsInitializing || (CurrentProject == null))
            { return; }

            RenameProject(CurrentProject);
            listBoxSolution.RefreshItems();
            SetDirty();
        }

        private void RenameProject(Project project)
        {
            using (var form = new InputBoxForm("Object name:", project.Caption))
            {
                if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                { project.Caption = form.PromptValue; }
            }
        }

        private void RenameSet()
        {
            if (IsInitializing || (CurrentProject == null) || (SelectedMetadata == null))
            { return; }

            RenameSet(SelectedMetadata);
        }

        private void RenameSet(IDataSetMetadata metadata)
        {
            MessageBox.Show("Custom set renaming has been disabled", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //using (var form = new InputBoxForm("Set name:", SelectedMetadata.Caption))
            //{
            //    if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            //    {
            //        SelectedMetadata.Caption = form.PromptValue;
            //        listBoxObs.Refresh();
            //        SetDirty();
            //    }
            //}
        }

        private void SetFilter()
        {
            if (IsInitializing || (CurrentProject == null) || (SelectedMetadata == null))
            { return; }

            SetFilter(SelectedMetadata);
        }

        private void SetFilter(IDataSetMetadata metadata)
        {
            using (var form = new InputBoxForm("Set filter:", SelectedMetadata.Filter))
            {
                if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    SelectedMetadata.Filter = form.PromptValue;
                    listBoxObs.Refresh();
                    graph.Invalidate();
                    SetDirty();
                }
            }
        }

        private void AddMinimum()
        {
            if (IsInitializing || (CurrentProject == null) || (SelectedMetadata == null))
            { return; }

            AddMinimum(CurrentProject, SelectedMetadata);
        }

        private void AddMinimum(Project project, IDataSetMetadata metadata)
        {
            IDataSet dataSet = System.Linq.Enumerable.Single(System.Linq.Enumerable.Where(project.DataSeries.Series, (set) => set.Metadata == metadata));
            if (dataSet.BoundingBox.IsEmpty)
            { return; }

            using (var form = new AddMinimaForm(dataSet))
            {
                if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    ((DataPointSet)dataSet).AddXMark(form.XMark);
                    RefreshDataSource(true);
                    graph.Invalidate();
                    SetDirty();
                }
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
            SetDirty();
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
                    textBoxOffset.Text = FormatEx.FormatDouble(selectedMetadata.OffsetY);
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
                RenameSet();
            }

            if (e.KeyCode == Keys.Delete)
            {
                DialogResult result = MessageBox.Show("Really delete the set from the project?", "Confirmation", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (result == DialogResult.OK)
                { RemoveSetFromProject(SelectedMetadata); SetDirty(); }
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

        void checkBoxBinning_CheckedChanged(object sender, EventArgs e)
        {
            RefreshPhaseBinning();
        }

        void textBoxBinning_TextChanged(object sender, EventArgs e)
        {
            RefreshPhaseBinning();
        }

        void RefreshPhaseBinning()
        {
            if (IsInitializing || (CurrentProject == null))
            { return; }
            SetPhaseBinning();
            UpdateDataSource(true);
        }

        void SetPhaseBinning()
        {
            if (CurrentProject == null)
            { return; }
            CurrentProject.SetPhaseBinning(checkBoxBinning.Checked ? FormatEx.ParseDouble(textBoxBinning.Text) : 0);
        }

        // title

        private void RefreshTitle()
        {
            string dirtyFlag = IsDirty ? "*" : "";
            if (!String.IsNullOrEmpty(CurrentSolutionFile))
            { this.Text = String.Format("{1}{0} - Silicups", dirtyFlag, CurrentSolutionFile); }
            else
            { this.Text = String.Format("Silicups{0}", dirtyFlag); }
        }

        private void SetDirty()
        {
            IsDirty = true;
            RefreshTitle();
        }

        private void SetTitle(string currentSolutionFile, bool isDirty = false)
        {
            CurrentSolutionFile = currentSolutionFile;
            IsDirty = isDirty;
            RefreshTitle();
        }

        private void ResetTitle()
        {
            SetTitle(null);
        }

        private bool ClosingConfirmed()
        {
            if (!IsDirty)
            { return true; }

            switch(MessageBox.Show("Save changes to the solution file?", "Confirmation", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
            {
                case DialogResult.Yes:
                    return SaveSolutionWithDialogResult() == System.Windows.Forms.DialogResult.OK;
                case DialogResult.No:
                    return true;
                case DialogResult.Cancel:
                default:
                    return false;
            }
        }

        // files

        private void NewSolution()
        {
            try
            {
                IsInitializing = true;
                RenewSolution(new Project[] { new Project() } );
                ResetTitle();
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
                var newProject = new Project();
                RenameProject(newProject);
                AddProjectToSolution(newProject);
                SetDirty();
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
                SetDirty();
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
                var projectLoadExceptions = new List<Exception>();
                foreach (XmlNode projectNode in rootNode.FindNodes("Projects").FindNodes("Project"))
                {
                    string id = projectNode.GetAttribute("id").AsString();
                    Project project = new Core.Project();
                    project.LoadFromXml(path, projectNode, projectLoadExceptions);
                    projects.Add(id, project);
                }

                foreach (XmlNode projectNode in rootNode.FindNodes("Solution").FindNodes("Project"))
                {
                    string id = projectNode.GetAttribute("id").AsString();
                    solution.Add(projects[id]);
                    projectsUsed.Add(id);
                }

                bool isDirty = false;
                if (projectLoadExceptions.Count > 0)
                {
                    var sb = new StringBuilder();
                    sb.Append("The project has been loaded only partially").AppendLine();
                    if (projectLoadExceptions.Count > 1)
                    { sb.Append("There was an exception while loading the project:").AppendLine(); }
                    else
                    { sb.Append("There were ").Append(projectLoadExceptions.Count).Append(" exceptions while loading the project:").AppendLine(); }
                    foreach (Exception e in projectLoadExceptions)
                    { sb.Append(" - ").AppendLine(e.Message); }

                    MessageBox.Show(sb.ToString(), "The project has been loaded only partially", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    isDirty = true;
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
                SetTitle(path, isDirty);
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
            try
            {
                if (System.IO.File.Exists(path))
                {
                    string directory = System.IO.Path.GetDirectoryName(path);
                    string backupDirectory = System.IO.Path.Combine(directory, "backup");
                    if (System.IO.Directory.Exists(backupDirectory))
                    {
                        string file = System.IO.Path.GetFileNameWithoutExtension(path);
                        string timestamp = DateTime.Now.ToString("yyyy'-'MM'-'dd'_'HH'-'mm'-'ss'-'fffffff");
                        string extension = System.IO.Path.GetExtension(path);
                        string backupPath = System.IO.Path.Combine(backupDirectory, String.Format("{0}_{1}{2}", file, timestamp, extension));
                        System.IO.File.Copy(path, backupPath);
                    }
                }
            }
            catch (Exception e)
            {
                toolStripStatusLabel1.Text = "Exception when creating backup: " + e.Message;
            }

            var doc = new XmlDocument();
            XmlNode rootNode = doc.AppendXmlElement("Silicups");
            XmlNode solutionNode = rootNode.AppendXmlElement("Solution");
            XmlNode projectsNode = rootNode.AppendXmlElement("Projects");
            foreach (Project project in Solution)
            {
                solutionNode.AppendXmlElement("Project").AppendXmlAttribute("id", project.Id);
                XmlNode projectNode = projectsNode.AppendXmlElement("Project");
                project.SaveToXml(path, projectNode);
            }
            doc.Save(path);
            SetTitle(path);
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

                var files = new List<string>(PathEx.FindFiles(baseDirectory, pattern, filter));

                var fileListForm = new FileListForm(files, CurrentProject);
                if (fileListForm.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                { return; }

                foreach (FileListForm.FileAction action in fileListForm.GetChangeActions())
                {
                    switch (action.Action)
                    {
                        case FileListForm.ActionEnum.Add: CurrentProject.AddDataFile(action.Path); SetDirty(); break;
                        case FileListForm.ActionEnum.Keep: CurrentProject.RefreshDataFile(action.Path); break;
                        case FileListForm.ActionEnum.Remove: CurrentProject.RemoveDataFile(action.Path); SetDirty(); break;
                    }
                }

                CurrentProject.SetFileSource(baseDirectory, pattern, filter);
                RefreshCurrentProject();
                RefreshDataSource();
            }
            catch (Exception e)
            {
                MessageBox.Show("Exception when scanning for files: " + e.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                            FormatEx.FormatDouble(point.X),
                            FormatEx.FormatDouble(point.Y),
                            FormatEx.FormatDouble(point.Yerr)
                        );
                    }
                }
            }
        }

        private void RenewSolution(IEnumerable<Project> projects)
        {
            Solution.Clear();
            Solution.AddRange(projects);
            RenewSolution();
        }

        private void RenewSolution()
        {
            listBoxSolution.Items.Clear();
            CurrentProject = null;

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
                gliderOffset,
                checkBoxBinning,
                textBoxBinning
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
            textBoxM0.Text = FormatEx.FormatDouble(CurrentProject.M0);
            textBoxP.Text = FormatEx.FormatDouble(CurrentProject.P);
            textBoxPPM.Text = FormatEx.FormatDouble(CurrentProject.PAmplitude);
            textBoxOffsetPM.Text = FormatEx.FormatDouble(CurrentProject.OffsetAmplitude);

            gliderP.Enabled = CurrentProject.P.HasValue && CurrentProject.PAmplitude.HasValue;
            gliderOffset.Enabled = false;

            foreach (IDataSetMetadata metadata in CurrentProject.GetMetadata())
            { listBoxObs.Items.Add(metadata, metadata.Enabled); }

            SetPhaseBinning();
            UpdateDataSource(true);
        }

        private void RemoveSetFromProject(IDataSetMetadata metadata)
        {
            if (CurrentProject == null)
            { return; }

            if (!CurrentProject.RemoveSet(metadata))
            { return; }

            RefreshCurrentProject();
            SetDirty();
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
                RegistryHelper.TryGetFromRegistry(RegistryPath, new RegistryHelper.GetRegistryStringAction("LoadSolutionPath", (s) => { fd.InitialDirectory = s; } ));
                DialogResult dialogResult = fd.ShowDialog();
                if (dialogResult == DialogResult.OK)
                {
                    RegistryHelper.TrySetToRegistry(RegistryPath, new RegistryHelper.SetRegistryAction("LoadSolutionPath", System.IO.Path.GetDirectoryName(fd.FileName)));
                    LoadSolution(fd.FileName);
                }
            }
        }

        private DialogResult SaveSolutionWithDialogResult(string path)
        {
            try
            {
                SaveSolution(path);
                return DialogResult.OK;
            }
            catch (Exception e)
            {
                if (MessageBox.Show(e.Message, "Exception", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error) == DialogResult.Retry)
                { SaveSolutionWithDialogResult(path); }
                return DialogResult.Cancel;
            }
        }

        private DialogResult SaveSolutionWithDialogResult()
        {
            if (!String.IsNullOrEmpty(CurrentSolutionFile))
            { return SaveSolutionWithDialogResult(CurrentSolutionFile); }
            else
            { return SaveSolutionAsWithDialogResult(); }
        }

        private DialogResult SaveSolutionAsWithDialogResult()
        {
            using (var fd = new SaveFileDialog())
            {
                fd.Filter = "XML Files (.xml)|*.xml|All Files (*.*)|*.*";
                RegistryHelper.TryGetFromRegistry(RegistryPath, new RegistryHelper.GetRegistryStringAction("SaveSolutionPath", (s) => { fd.InitialDirectory = s; } ));
                DialogResult dialogResult = fd.ShowDialog();
                if (dialogResult == DialogResult.OK)
                {
                    RegistryHelper.TrySetToRegistry(RegistryPath, new RegistryHelper.SetRegistryAction("SaveSolutionPath", System.IO.Path.GetDirectoryName(fd.FileName))); 
                    return SaveSolutionWithDialogResult(fd.FileName);
                }
                return DialogResult.Cancel;
            }
        }

        private void saveSolutionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveSolutionWithDialogResult();
        }

        private void saveSolutionAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveSolutionAsWithDialogResult();
        }

        private void addNewProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddNewProjectToSolution();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ClosingConfirmed())
            { Application.Exit(); }
        }

        private void renameObjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RenameProject();
        }

        private void loadFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var fd = new OpenFileDialog())
            {
                fd.Filter = "Text Files (.txt)|*.txt|All Files (*.*)|*.*";
                fd.Multiselect = true;
                RegistryHelper.TryGetFromRegistry(RegistryPath, new RegistryHelper.GetRegistryStringAction("LoadFilePath", (s) => { fd.InitialDirectory = s; }));
                DialogResult dialogResult = fd.ShowDialog();
                if ((dialogResult == DialogResult.OK) && (fd.FileNames.Length > 0))
                {
                    RegistryHelper.TrySetToRegistry(RegistryPath, new RegistryHelper.SetRegistryAction("LoadFilePath", System.IO.Path.GetDirectoryName(fd.FileNames[0])));
                    LoadFiles(fd.FileNames);
                }
            }
        }

        private void loadFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var fd = FilesSelectForm.CreateFilesSelectForm(CurrentProject))
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
                RegistryHelper.TryGetFromRegistry(RegistryPath, new RegistryHelper.GetRegistryStringAction("ExportToTxtPath", (s) => { fd.InitialDirectory = s; }));
                DialogResult dialogResult = fd.ShowDialog();
                if (dialogResult == DialogResult.OK)
                {
                    RegistryHelper.TrySetToRegistry(RegistryPath, new RegistryHelper.SetRegistryAction("ExportToTxtPath", System.IO.Path.GetDirectoryName(fd.FileName)));
                    SaveToTxt(fd.FileName);
                }
            }
        }

        private void buttonMinima_Click(object sender, EventArgs e)
        {
            if (CurrentProject == null)
            { return; }

            var minimaForm = new MinimaForm(CurrentProject);
            if (minimaForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                SetDirty();
                RefreshDataSource(true);
            }
        }

        private void importFromVarastroczToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var importVarAstroCzForm = new ImportVarAstroCzForm();
            importVarAstroCzForm.ShowDialog();
            if (importVarAstroCzForm.Project != null)
            { Solution.Add(importVarAstroCzForm.Project); }
            RenewSolution();
        }

        private void minimaPredictionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentProject == null)
            { return; }

            if (!CurrentProject.M0.HasValue || !CurrentProject.P.HasValue || (CurrentProject.P.Value <= 0))
            {
                MessageBox.Show("Epoch (M0 and P) not given, cannot show predictions", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            new MinimaPredictionsForm(CurrentProject).ShowDialog();
        }

        private void sortObservationsByDateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CurrentProject == null)
            { return; }

            CurrentProject.SortByDate();
            RefreshCurrentProject();
            SetDirty();
        }

        private void setCaptionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RenameSet();
        }

        private void setFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetFilter();
        }

        private void addMinimumToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddMinimum();
        }

        private void exportSolutionToCSVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var fd = new SaveFileDialog())
            {
                fd.Title = "Export to CSV";
                fd.Filter = "CSV Files (.csv)|*.csv|All Files (*.*)|*.*";
                RegistryHelper.TryGetFromRegistry(RegistryPath, new RegistryHelper.GetRegistryStringAction("ExportToCSVPath", (s) => { fd.InitialDirectory = s; }));
                DialogResult dialogResult = fd.ShowDialog();
                if (dialogResult == DialogResult.OK)
                {
                    RegistryHelper.TrySetToRegistry(RegistryPath, new RegistryHelper.SetRegistryAction("ExportToCSVPath", System.IO.Path.GetDirectoryName(fd.FileName)));
                    SaveToCSV(fd.FileName);
                }
            }
        }

        private void SaveToCSV(string path)
        {
            try
            {
                using (var writer = new System.IO.StreamWriter(path))
                {
                    writer.WriteLine("OBJECT;M0;P");
                    foreach (Project project in Solution)
                    {
                        string objectName = project.Caption;
                        if (objectName.Contains("\"") || objectName.Contains(";"))
                        {
                            objectName = objectName.Replace("\"", "\"\"");
                            objectName = "\"" + objectName + "\"";
                        }
                        if (project.CanProvidePeriodData)
                        { writer.WriteLine("{0};{1};{2}", objectName, FormatEx.FormatDouble(project.M0), FormatEx.FormatDouble(project.P)); }
                        else
                        { writer.WriteLine(objectName); }
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Error when exporting to CSV", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void exportGraphToPNGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var fd = new SaveFileDialog())
            {
                fd.Title = "Export to PNG";
                fd.Filter = "PNG Files (.png)|*.png|All Files (*.*)|*.*";
                RegistryHelper.TryGetFromRegistry(RegistryPath, new RegistryHelper.GetRegistryStringAction("ExportToPNGPath", (s) => { fd.InitialDirectory = s; }));
                DialogResult dialogResult = fd.ShowDialog();
                if (dialogResult == DialogResult.OK)
                {
                    RegistryHelper.TrySetToRegistry(RegistryPath, new RegistryHelper.SetRegistryAction("ExportToPNGPath", System.IO.Path.GetDirectoryName(fd.FileName)));
                    SaveToPNG(fd.FileName);
                }
            }
        }

        private void SaveToPNG(string path)
        {
            try
            {
                using (Bitmap b = new Bitmap(graph.Width, graph.Height))
                {
                    graph.DrawToBitmap(b, new Rectangle(Point.Empty, b.Size));
                    b.Save(path);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Error when exporting to CSV", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void usingTemplateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string template = null;
            RegistryHelper.TryGetFromRegistry(RegistryPath, new RegistryHelper.GetRegistryStringAction("TemplatePath", (s) => { template = s; }));
            if (String.IsNullOrEmpty(template))
            {
                if (MessageBox.Show("No template file selected, do you want to select a template file?", "Export using a template", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == System.Windows.Forms.DialogResult.Yes)
                { selectTemplateToolStripMenuItem_Click(sender, e); }
                else
                { return; }
            }

            RegistryHelper.TryGetFromRegistry(RegistryPath, new RegistryHelper.GetRegistryStringAction("TemplatePath", (s) => { template = s; }));
            if (String.IsNullOrEmpty(template))
            { MessageBox.Show("No template found", "Error when exporting using a template", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            if (!System.IO.File.Exists(template))
            { MessageBox.Show("Template file not found found: " + template, "Error when exporting using a template", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            using (var fd = new SaveFileDialog())
            {
                fd.Title = "Export using a template";
                fd.Filter = "All Files (*.*)|*.*";
                RegistryHelper.TryGetFromRegistry(RegistryPath, new RegistryHelper.GetRegistryStringAction("ExportUsingTemplatePath", (s) => { fd.InitialDirectory = s; }));
                DialogResult dialogResult = fd.ShowDialog();
                if (dialogResult == DialogResult.OK)
                {
                    RegistryHelper.TrySetToRegistry(RegistryPath, new RegistryHelper.SetRegistryAction("ExportUsingTemplatePath", System.IO.Path.GetDirectoryName(fd.FileName)));
                    SaveUsingTemplate(template, fd.FileName);
                }
            }
        }

        private void SaveUsingTemplate(string template, string path)
        {
            try
            {
                using (var templateStream = System.IO.File.OpenRead(template))
                using (var outputStream = System.IO.File.Open(path, System.IO.FileMode.Create))
                {
                    var templateEngine = new SolutionTemplate(Solution);
                    templateEngine.GenerateOutputStream(templateStream, outputStream);
                }
            }
            catch (NoNullAllowedException e)
            {
                MessageBox.Show(e.ToString(), "Error when exporting using a template", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void selectTemplateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var fd = new OpenFileDialog())
            {
                fd.Title = "Select a template file";
                fd.Filter = "All Files (*.*)|*.*";
                fd.CheckFileExists = true;
                RegistryHelper.TryGetFromRegistry(RegistryPath, new RegistryHelper.GetRegistryStringAction("TemplatePath", (s) => { fd.InitialDirectory = System.IO.Path.GetDirectoryName(s); }));
                DialogResult dialogResult = fd.ShowDialog();
                if (dialogResult == DialogResult.OK)
                {
                    RegistryHelper.TrySetToRegistry(RegistryPath, new RegistryHelper.SetRegistryAction("TemplatePath", fd.FileName));
                }
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

    public class MyTextBox : TextBox
    {
        public event Action AfterPaste;

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            switch (m.Msg)
            {
                case 0x302: //WM_PASTE
                    Action afterPaste = AfterPaste;
                    if (afterPaste != null)
                    { afterPaste(); }
                    break;
            }
        }
    }

    public static class TrackBarExtensions
    {
        public static double NormalizedValue(this TrackBar trackBar)
        {
            return trackBar.Value * 1.0 / trackBar.Maximum;
        }
    }

    public static class IListExtensions
    {
        public static void Swap(this System.Collections.IList list, int index1, int index2)
        {
            object tmp = list[index1];
            list[index1] = list[index2];
            list[index2] = tmp;
        }

        public static void Swap<T>(this IList<T> list, int index1, int index2)
        {
            T tmp = list[index1];
            list[index1] = list[index2];
            list[index2] = tmp;
        }

        public static void Swap<T1, T2>(IList<T1> list1, IList<T2> list2, int index1, int index2)
        {
            Swap(list1, index1, index2);
            Swap(list2, index1, index2);
        }

        public static void Swap<T1>(IList<T1> list1, System.Collections.IList list2, int index1, int index2)
        {
            Swap(list1, index1, index2);
            Swap(list2, index1, index2);
        }
    }
}
