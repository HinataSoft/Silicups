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
    public partial class AddMinimaForm : FormEx
    {
        private class DataSetMetadata : IDataSetMetadata
        {
            public string AbsolutePath { get { return String.Empty; } }
            public string RelativePath { get { return String.Empty; } }
            public string Caption { get; set; }
            public string Filter { get; set; }
            public bool Enabled { get; set; }
            public bool Hightlighted { get; set; }
            public double OffsetY { get; set; }
        }

        private class DataSet : IDataSet
        {
            public BoundingBox BoundingBox { get; private set; }
            public DataSetMetadata Metadata { get; private set; }
            public List<DataPoint> Set { get; private set; }
            public List<DataMark> XMarks { get; private set; }

            IDataSetMetadata IDataSet.Metadata { get { return this.Metadata; } }
            IEnumerable<DataPoint> IDataSet.Set { get { return this.Set; } }
            IEnumerable<DataMark> IDataSet.XMarks { get { return this.XMarks; } }

            public DataSet(IDataSet dataSet, double offsetX, bool reversed, Nullable<DataMark> xMark = null)
            {
                this.BoundingBox = BoundingBox.CloneEmpty();
                this.Metadata = new DataSetMetadata();
                this.Set = new List<DataPoint>();
                this.XMarks = new List<DataMark>();

                Metadata.OffsetY = dataSet.Metadata.OffsetY;
                Metadata.Enabled = true;
                Metadata.Hightlighted = false;
                Metadata.Filter = reversed ? "Red" : "Black";

                //foreach (DataPoint p in dataSet.Set)
                //{
                //    var normalX = (p.X - dataSet.BoundingBox.Left) / dataSet.BoundingBox.Width;
                //    normalX += offsetX;
                //    if (reversed)
                //    { normalX = 1 - normalX; }
                //    Set.Add(new DataPoint(normalX, p.Y, p.Yerr));
                //    BoundingBox.Union(normalX, p.Y);
                //}

                foreach (DataPoint p in dataSet.Set)
                {
                    double x = p.X;
                    if (reversed)
                    { x = dataSet.BoundingBox.Left - x + dataSet.BoundingBox.Right - offsetX * dataSet.BoundingBox.Width; }
                    Set.Add(new DataPoint(x, p.Y, p.Yerr));
                    BoundingBox.Union(x, p.Y);
                }
                if (xMark.HasValue)
                { XMarks.Add(xMark.Value); }
            }
        }

        private class DataSeries : IDataSeries
        {
            public BoundingBox BoundingBox { get; private set; }
            public List<IDataSet> Series { get; private set; }
            public bool IsEmpty { get { return false; } }

            IEnumerable<IDataSet> IDataSeries.Series { get { return this.Series; } }

            public DataSeries()
            {
                this.BoundingBox = BoundingBox.CloneEmpty();
                this.Series = new List<IDataSet>();
            }

            public void AddSet(IDataSet sourceDataSet, double offsetX, bool reversed, Nullable<DataMark> xMark = null)
            {
                var set = new DataSet(sourceDataSet, offsetX, reversed, xMark);
                Series.Add(set);
                BoundingBox.Union(set.BoundingBox);
            }
        }

        private static readonly string RegistryPath = Util.RegistryHelper.RegistryPath + @"\AddMinimaForm";

        public DataMark XMark { get; private set; }
        private IDataSet SourceDataSet;

        public AddMinimaForm(IDataSet sourceDataSet)
        {
            InitializeComponent();
            InitializeFormEx(RegistryPath);
            this.SourceDataSet = sourceDataSet;
            this.comboBoxMinType.SelectedIndex = 0;
            UpdateWorkingDataSeries();
        }

        private void UpdateWorkingDataSeries()
        {
            double offsetX = trackBar.Value / 1000f;
            double errX = trackBarErr.Value / 1000f;
            var xMark = SourceDataSet.BoundingBox.Left + SourceDataSet.BoundingBox.Width / 2 - offsetX / 2 * SourceDataSet.BoundingBox.Width;
            var xMarkErr = errX * SourceDataSet.BoundingBox.Width;
            XMark = new DataMark(comboBoxMinType.SelectedIndex, xMark, xMarkErr);

            var workingDataSeries = new DataSeries();
            workingDataSeries.AddSet(SourceDataSet, offsetX, false, XMark);
            workingDataSeries.AddSet(SourceDataSet, offsetX, true);
            this.graph.SetDataSource(workingDataSeries);

            label.Text = String.Format("TminHJD = {0} +- {1}", xMark, xMarkErr);
        }

        private void trackBar_ValueChanged(object sender, EventArgs e)
        {
            UpdateWorkingDataSeries();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void comboBoxMinType_SelectedValueChanged(object sender, EventArgs e)
        {
            UpdateWorkingDataSeries();
        }

        private void trackBarErr_ValueChanged(object sender, EventArgs e)
        {
            UpdateWorkingDataSeries();
        }
    }
}
