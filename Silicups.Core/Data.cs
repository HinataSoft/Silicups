using System;
using System.Collections.Generic;
using System.Text;

namespace Silicups.Core
{
    public interface IDataSet
    {
        BoundingBox BoundingBox { get; }
        IEnumerable<DataPoint> Set { get; }
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

    public class DataSetDescription
    {
        public int Id { get; internal set; }
        public string Path { get; internal set; }
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

    public class DataPointSet : IDataSet
    {
        public BoundingBox BoundingBox { get; private set; }

        public bool Enabled { get; set; }
        public double OffsetY { get; set; }
        public DataSetDescription Description { get; set; }

        private List<DataPoint> list = new List<DataPoint>();

        public DataPointSet()
        {
            this.BoundingBox = BoundingBox.CloneEmpty();
            this.Enabled = true;
            this.OffsetY = 0;
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

        public IEnumerable<DataPoint> Set
        {
            get { return list; }
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

        public void AddSet(IEnumerable<DataPoint> set)
        {
            var pointSet = new DataPointSet();
            foreach (DataPoint p in set)
            { pointSet.Add(p); }

            list.Add(pointSet);
            BoundingBox.Union(pointSet.BoundingBox);
        }

        public void AddSet(DataPointSet pointSet)
        {
            list.Add(pointSet);
            BoundingBox.Union(pointSet.BoundingBox);
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

        public bool IsEmpty { get { return list.Count == 0; } }
    }

    public class DerivedSeries : IDataSeries
    {
        protected DataPointSeries DataSeries { get; private set; }
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

    public class TimeSeries : DerivedSeries, IRefreshableDataSeries
    {
        public TimeSeries(DataPointSeries dataSeries)
            : base(dataSeries)
        {
        }

        public void Refresh()
        {
            Clean();
            foreach (DataPointSet originalSet in DataSeries.DataSetSeries)
            {
                if (!originalSet.Enabled)
                { continue; }
                var set = new DataPointSet();
                foreach (DataPoint p in originalSet.Set)
                { set.Add(p.X, p.Y - originalSet.OffsetY, p.Yerr); }
                Add(set);
            }
        }
    }

    public class CompressedSeries : DerivedSeries, IRefreshableDataSeries
    {
        public CompressedSeries(DataPointSeries dataSeries)
            : base(dataSeries)
        {
        }

        public void Refresh()
        {
            Clean();
            double x = 0;
            foreach (DataPointSet originalSet in DataSeries.DataSetSeries)
            {
                if (!originalSet.Enabled)
                { continue; }

                var set = new DataPointSet();
                double xOffset = x - originalSet.BoundingBox.Left;

                foreach (DataPoint p in originalSet.Set)
                { set.Add(p.X + xOffset, p.Y - originalSet.OffsetY, p.Yerr); }

                Add(set);
                x += originalSet.BoundingBox.Width * 1.1;
            }
        }
    }

    public class PhasedSeries : DerivedSeries, IRefreshableDataSeries
    {
        public double M0 { get; set; }
        public double P { get; set; }

        public PhasedSeries(DataPointSeries dataSeries)
            : base(dataSeries)
        {
            this.M0 = 0;
            this.P = 0;
        }

        public void Refresh()
        {
            Clean();
            if (P == 0)
            {
                BoundingBox = new BoundingBox(-1, 0, 1, 1);
                return;
            }

            foreach (DataPointSet originalSet in DataSeries.DataSetSeries)
            {
                if (!originalSet.Enabled)
                { continue; }

                var set = new DataPointSet();
                foreach (DataPoint p in originalSet.Set)
                {
                    double phased = (p.X - M0) / P;
                    double phase = phased - Math.Floor(phased);
                    set.Add(phase, p.Y - originalSet.OffsetY, p.Yerr);
                    set.Add(phase - 1, p.Y - originalSet.OffsetY, p.Yerr);
                }
                Add(set);
                BoundingBox.Union(set.BoundingBox);
            }

            BoundingBox.Left = -1;
            BoundingBox.Right = 1;
        }
    }
}
