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
    public partial class MainForm : Form
    {
        public List<DataPoint> DataList = null;
        public List<DataPoint> PhaseList = null;

        public MainForm()
        {
            InitializeComponent();

            radioButtonPhased.CheckedChanged += new EventHandler(radioButtonPhased_CheckedChanged);

            if (System.IO.File.Exists("autoload.txt"))
            { LoadFile("autoload.txt"); }
        }

        void radioButtonPhased_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonPhased.Checked)
            {
                if ((DataList == null) || String.IsNullOrEmpty(textBoxM0.Text) || String.IsNullOrEmpty(textBoxP.Text))
                { radioButtonTimeseries.Checked = true; }
                else
                {
                    try
                    { MakePhaseList(false); }
                    catch
                    { radioButtonTimeseries.Checked = true; }
                }
            }
            else
            {
                SetDataSource(DataList);
            }
        }

        private void SetDataSource(List<DataPoint> list)
        {
            if (list == null)
            { graph.DataSource = null; }
            else
            { graph.DataSource = () => list; }
        }

        private void LoadFile(string filename)
        {
            StartLoadFile();
            AppendFile(filename);
            FinishLoadFile();
        }

        private void StartLoadFile()
        {
            listBoxObs.Items.Clear();
            DataList = new List<DataPoint>();
            PhaseList = null;
            radioButtonTimeseries.Checked = true;
            textBoxM0.Text = "";
            textBoxP.Text = "";
        }

        private void FinishLoadFile()
        {
            SetDataSource(DataList);
        }

        private readonly static string PhasedTag = "Phased with elements ";
        private void AppendFile(string filename)
        {
            foreach (string s in System.IO.File.ReadAllLines(filename))
            {
                try
                {
                    if (s.StartsWith("24"))
                    {
                        string[] parts = s.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        var x = ParseDouble(parts[0]);
                        var y = ParseDouble(parts[1]);
                        var yerr = ParseDouble(parts[2]);
                        if (y > 50)
                        { continue; }

                        DataList.Add(new DataPoint() { X = x, Y = y, Yerr = yerr });
                    }
                    else
                    {
                        int i = s.IndexOf(PhasedTag);
                        if (i > 0)
                        {
                            string[] parts = s.Substring(i + PhasedTag.Length).Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            textBoxM0.Text = parts[0];
                            textBoxP.Text = parts[2];
                        }
                    }
                }
                catch
                { }
            }
            listBoxObs.Items.Add(filename, true);
        }

        private void MakePhaseList(bool updateOnly)
        {
            double m0 = ParseDouble(textBoxM0.Text);
            double per = ParseDouble(textBoxP.Text);

            bool doUpdate = (PhaseList != null) && updateOnly;
            if (doUpdate)
            { PhaseList.Clear(); }
            else
            { PhaseList = new List<DataPoint>(); }

            foreach (DataPoint p in DataList)
            {
                double phased = (p.X - m0) / per;
                double phase = phased - Math.Floor(phased);
                PhaseList.Add(new DataPoint() { X = phase, Y = p.Y, Yerr = p.Yerr });
                PhaseList.Add(new DataPoint() { X = phase - 1, Y = p.Y, Yerr = p.Yerr });
            }

            if (doUpdate)
            { graph.Invalidate(); }
            else
            { SetDataSource(PhaseList); }
        }

        private void textBoxP_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (radioButtonPhased.Checked)
                { MakePhaseList(true); }
            }
            catch
            { }
        }

        private double ParseDouble(string s)
        {
            return Double.Parse(s.Replace(',', '.'), System.Globalization.CultureInfo.InvariantCulture);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void loadFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var fd = new OpenFileDialog();
            fd.Filter = "Text Files (.txt)|*.txt|All Files (*.*)|*.*";
            fd.Multiselect = true;
            DialogResult dialogResult = fd.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                StartLoadFile();
                foreach (string filename in fd.FileNames)
                { AppendFile(filename); }
                FinishLoadFile();
            }
        }

        private void loadFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var fd = new FilesSelectForm();
            DialogResult dialogResult = fd.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                try
                {
                    StartLoadFile();
                    foreach (string filename in fd.FileNames)
                    { AppendFile(filename); }
                    FinishLoadFile();                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
