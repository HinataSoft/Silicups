using System;
using System.Collections.Generic;
using System.Text;

namespace Silicups.Core
{
    public interface IDataSetMetadata
    {
        string AbsolutePath { get; }
        string RelativePath { get; }
        string Caption { get; set; }

        bool Enabled { get; set; }
        bool Hightlighted { get; set; }
        double OffsetY { get; set; }
    }

    public interface IDataSet
    {
        BoundingBox BoundingBox { get; }
        IDataSetMetadata Metadata { get; }
        IEnumerable<DataPoint> Set { get; }
        IEnumerable<DataMark> XMarks { get; }
    }

    public interface IDataSeries
    {
        BoundingBox BoundingBox { get; }
        IEnumerable<IDataSet> Series { get; }
        bool IsEmpty { get; }
    }

    public interface IRefreshableDataSeries : IDataSeries
    {
        void Refresh();
    }

    public class DataSetMetadata : IDataSetMetadata
    {
        public string AbsolutePath { get; internal set; }
        public string RelativePath { get; internal set; }
        public string Caption { get; set; }
        public bool Enabled { get; set; }
        public bool Hightlighted { get; set; }
        public double OffsetY { get; set; }

        public override string ToString()
        {
            return Caption ?? RelativePath ?? AbsolutePath ?? "DataSet";
        }
    }

    public struct DataPoint
    {
        public double X;
        public double Y;
        public double Yerr;

        public DataPoint(double x, double y, double yerr)
        {
            this.X = x;
            this.Y = y;
            this.Yerr = yerr;
        }
    }

    public struct DataMark
    {
        public int Type;
        public double N;

        public DataMark(DataMark m)
        {
            this.Type = m.Type;
            this.N = m.N;
        }

        public DataMark(int type, double n)
        {
            this.Type = type;
            this.N = n;
        }
    }

    public class DataPointSet : IDataSet
    {
        public BoundingBox BoundingBox { get; private set; }

        public IDataSetMetadata Metadata { get; internal set; }

        private List<DataPoint> list = new List<DataPoint>();
        private List<DataMark> xmarks = new List<DataMark>();

        public DataPointSet(IDataSet templateSet)
        {
            this.BoundingBox = BoundingBox.CloneEmpty();
            this.Metadata = templateSet.Metadata;
        }

        public DataPointSet(string absolutePath, string relativePath)
        {
            this.BoundingBox = BoundingBox.CloneEmpty();
            this.Metadata = new DataSetMetadata()
            {
                AbsolutePath = absolutePath,
                RelativePath = relativePath,
                Enabled = true,
                Hightlighted = false,
                OffsetY = 0,
            };
        }

        public void Add(DataPoint p)
        {
            list.Add(p);
            BoundingBox.Union(p.X, p.Y);
        }

        public void Add(double x, double y, double yerr)
        {
            Add(new DataPoint(x, y, yerr));
        }

        public void Clear()
        {
            list.Clear();
        }

        public void AddXMark(DataMark m)
        {
            xmarks.Add(m);
        }

        public void AddXMark(int type, double x)
        {
            xmarks.Add(new DataMark(type, x));
        }

        public void ClearXMarks()
        {
            xmarks.Clear();
        }

        public IEnumerable<DataPoint> Set
        {
            get { return list; }
        }

        public IEnumerable<DataMark> XMarks
        {
            get { return xmarks; }
        }
    }

    public class DataPointSeries : IDataSeries
    {
        private List<DataPointSet> list = new List<DataPointSet>();

        public BoundingBox BoundingBox { get; private set; }

        public DataPointSeries()
        {
            this.BoundingBox = BoundingBox.CloneEmpty();
        }

        public void AddSet(DataPointSet pointSet)
        {
            list.Add(pointSet);
            BoundingBox.Union(pointSet.BoundingBox);
        }

        public void RemoveSet(DataPointSet pointSet)
        {
            list.Remove(pointSet);
            BoundingBox = BoundingBox.CloneEmpty();
            foreach (DataPointSet set in list)
            { BoundingBox.Union(set.BoundingBox); }
        }

        public IEnumerable<IDataSet> Series
        {
            get
            {
                foreach (DataPointSet set in list)
                { yield return set; }
            }
        }

        public IEnumerable<IDataSet> DataSetSeries
        {
            get
            {
                return list;
            }
        }

        public void SortByDate()
        {
            list.Sort((a, b) => a.BoundingBox.Left.CompareTo(b.BoundingBox.Left));
        }

        public bool IsEmpty { get { return list.Count == 0; } }
    }

    public class DerivedSeries : IDataSeries
    {
        protected DataPointSeries DataSeries { get; set; }
        protected List<DataPointSet> List = new List<DataPointSet>();

        public BoundingBox BoundingBox { get; protected set; }

        public DerivedSeries(DataPointSeries dataSeries)
        {
            this.DataSeries = dataSeries;
            this.BoundingBox = BoundingBox.CloneEmpty();
        }

        public IEnumerable<IDataSet> Series
        {
            get
            {
                foreach (DataPointSet set in List)
                { yield return set; }
            }
        }

        protected void Clean()
        {
            List = new List<DataPointSet>();
            BoundingBox = BoundingBox.CloneEmpty();
        }

        protected void Add(DataPointSet set)
        {
            List.Add(set);
            BoundingBox.Union(set.BoundingBox);
        }

        public bool IsEmpty { get { return List.Count == 0; } }
    }

    public interface IPeriodDataProvider
    {
        bool CanProvidePeriodData { get; }
        double GetPhased(double time);
        IEnumerable<double> GetFullPhasesBetween(double t1, double t2);
    }

    public class DerivedSeriesWithPeriodProvider : DerivedSeries
    {
        protected IPeriodDataProvider PeriodDataProvider { get; private set; }

        public DerivedSeriesWithPeriodProvider(DataPointSeries dataSeries, IPeriodDataProvider periodDataProvider)
            : base(dataSeries)
        {
            this.PeriodDataProvider = periodDataProvider;
        }

        protected void InsertPhaseMarks()
        {
        }
    }

    public class TimeSeries : DerivedSeriesWithPeriodProvider, IRefreshableDataSeries
    {
        public TimeSeries(DataPointSeries dataSeries, IPeriodDataProvider periodDataProvider)
            : base(dataSeries, periodDataProvider)
        {
        }

        public void Refresh()
        {
            Clean();
            foreach (DataPointSet originalSet in DataSeries.DataSetSeries)
            {
                if (!originalSet.Metadata.Enabled)
                { continue; }

                var set = new DataPointSet(originalSet);

                foreach (DataPoint p in originalSet.Set)
                { set.Add(p.X, p.Y - originalSet.Metadata.OffsetY, p.Yerr); }

                foreach (DataMark m in originalSet.XMarks)
                { set.AddXMark(m); }

                InsertPhaseMarks();
                Add(set);
            }
        }
    }

    public class CompressedSeries : DerivedSeriesWithPeriodProvider, IRefreshableDataSeries
    {
        public CompressedSeries(DataPointSeries dataSeries, IPeriodDataProvider periodDataProvider)
            : base(dataSeries, periodDataProvider)
        {
        }

        public void Refresh()
        {
            Clean();
            double x = 0;
            foreach (DataPointSet originalSet in DataSeries.DataSetSeries)
            {
                if (!originalSet.Metadata.Enabled)
                { continue; }

                var set = new DataPointSet(originalSet);
                double xOffset = x - originalSet.BoundingBox.Left;

                foreach (DataPoint p in originalSet.Set)
                { set.Add(p.X + xOffset, p.Y - originalSet.Metadata.OffsetY, p.Yerr); }

                foreach (DataMark m in originalSet.XMarks)
                { set.AddXMark(m.Type, m.N + xOffset); }

                InsertPhaseMarks();
                Add(set);
                x += originalSet.BoundingBox.Width * 1.1;
            }
        }
    }

    public class PhasedSeries : DerivedSeriesWithPeriodProvider, IRefreshableDataSeries
    {
        public PhasedSeries(DataPointSeries dataSeries, IPeriodDataProvider periodDataProvider)
            : base(dataSeries, periodDataProvider)
        {
        }

        public void Refresh()
        {
            Clean();
            if (!PeriodDataProvider.CanProvidePeriodData)
            {
                BoundingBox = new BoundingBox(-1, 0, 1, 1);
                return;
            }

            foreach (DataPointSet originalSet in DataSeries.DataSetSeries)
            {
                if (!originalSet.Metadata.Enabled)
                { continue; }

                var set = new DataPointSet(originalSet);
                foreach (DataPoint p in originalSet.Set)
                {
                    double phase = PeriodDataProvider.GetPhased(p.X);
                    set.Add(phase, p.Y - originalSet.Metadata.OffsetY, p.Yerr);
                    set.Add(phase - 1, p.Y - originalSet.Metadata.OffsetY, p.Yerr);
                }

                foreach (DataMark m in originalSet.XMarks)
                {
                    double phase = PeriodDataProvider.GetPhased(m.N);
                    set.AddXMark(m.Type, phase);
                    set.AddXMark(m.Type, phase - 1);
                }

                InsertPhaseMarks();
                Add(set);
                BoundingBox.Union(set.BoundingBox);
            }

            BoundingBox.Left = -1;
            BoundingBox.Right = 1;
        }
    }
}
