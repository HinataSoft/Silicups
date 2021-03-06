﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Silicups.Core
{
    public interface IDataSetMetadata
    {
        string AbsolutePath { get; }
        string RelativePath { get; }
        string Caption { get; set; }
        string Filter { get; set; }

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
        private BoundingBox BoundingBox;

        public string AbsolutePath { get; internal set; }
        public string RelativePath { get; internal set; }
        public string Caption { get; set; }
        public string Filter { get; set; }
        public bool Enabled { get; set; }
        public bool Hightlighted { get; set; }
        public double OffsetY { get; set; }

        public DataSetMetadata(BoundingBox boundingBox)
        {
            this.BoundingBox = boundingBox;
        }

        public override string ToString()
        {
            //return Caption ?? RelativePath ?? AbsolutePath ?? "DataSet";
            if (!String.IsNullOrEmpty(Caption))
            { return Caption; }

            string dateCaption = JD.JDToDateTime(BoundingBox.Left).ToString("yyyy'-'MM'-'dd");
            if (!String.IsNullOrEmpty(Filter))
            { return String.Format("{0} [{1}]", dateCaption, Filter); }
            else
            { return dateCaption; }
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
        public double Nerr;

        public DataMark(DataMark m)
        {
            this.Type = m.Type;
            this.N = m.N;
            this.Nerr = m.Nerr;
        }

        public DataMark(int type, double n, double nerr)
        {
            this.Type = type;
            this.N = n;
            this.Nerr = nerr;
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
            this.Metadata = new DataSetMetadata(BoundingBox)
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

        public void Sort()
        {
            list.Sort((a, b) => a.X.CompareTo(b.X));
        }

        public void AddXMark(DataMark m)
        {
            xmarks.Add(m);
        }

        public void AddXMark(int type, double x, double xerr)
        {
            xmarks.Add(new DataMark(type, x, xerr));
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
        protected IDataSeries DataSeries { get; set; }
        protected List<IDataSet> List = new List<IDataSet>();

        public BoundingBox BoundingBox { get; protected set; }

        public DerivedSeries(IDataSeries dataSeries)
        {
            this.DataSeries = dataSeries;
            this.BoundingBox = BoundingBox.CloneEmpty();
        }

        public IEnumerable<IDataSet> Series
        {
            get
            {
                return List;
            }
        }

        protected void Clean()
        {
            List = new List<IDataSet>();
            BoundingBox = BoundingBox.CloneEmpty();
        }

        protected void Add(IDataSet set)
        {
            List.Add(set);
            BoundingBox.Union(set.BoundingBox);
        }

        public bool IsEmpty { get { return List.Count == 0; } }
    }

    public interface IPeriodDataProvider
    {
        bool CanProvidePeriodData { get; }
        double GetM0();
        double GetPeriod();
        double GetFrequency();
        double GetPhased(double time);
        double GetDephased(double phase);
        IEnumerable<double> GetFullPhasesBetween(double t1, double t2);
    }

    public class DerivedSeriesWithPeriodProvider : DerivedSeries
    {
        protected IPeriodDataProvider PeriodDataProvider { get; private set; }

        public DerivedSeriesWithPeriodProvider(IDataSeries dataSeries, IPeriodDataProvider periodDataProvider)
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
        public TimeSeries(IDataSeries dataSeries, IPeriodDataProvider periodDataProvider)
            : base(dataSeries, periodDataProvider)
        {
        }

        public void Refresh()
        {
            Clean();
            foreach (IDataSet originalSet in DataSeries.Series)
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
        public CompressedSeries(IDataSeries dataSeries, IPeriodDataProvider periodDataProvider)
            : base(dataSeries, periodDataProvider)
        {
        }

        public void Refresh()
        {
            Clean();
            double x = 0;
            foreach (IDataSet originalSet in DataSeries.Series)
            {
                if (!originalSet.Metadata.Enabled)
                { continue; }

                var set = new DataPointSet(originalSet);
                double xOffset = x - originalSet.BoundingBox.Left;

                foreach (DataPoint p in originalSet.Set)
                { set.Add(p.X + xOffset, p.Y - originalSet.Metadata.OffsetY, p.Yerr); }

                foreach (DataMark m in originalSet.XMarks)
                { set.AddXMark(m.Type, m.N + xOffset, m.Nerr); }

                InsertPhaseMarks();
                Add(set);
                x += originalSet.BoundingBox.Width * 1.1;
            }
        }
    }

    public class PhasedSeries : DerivedSeriesWithPeriodProvider, IRefreshableDataSeries
    {
        public double AbsIntervalSize { get; private set; }

        public PhasedSeries(IDataSeries dataSeries, IPeriodDataProvider periodDataProvider)
            : base(dataSeries, periodDataProvider)
        {
            AbsIntervalSize = 1;
        }

        public void SetAbsIntervalSize(double absIntervalSize)
        {
            AbsIntervalSize = absIntervalSize;
            Refresh();
        }

        public void Refresh()
        {
            Clean();
            if (!PeriodDataProvider.CanProvidePeriodData)
            {
                BoundingBox = new BoundingBox(-AbsIntervalSize, 0, AbsIntervalSize, 1);
                return;
            }

            double frequency = PeriodDataProvider.GetFrequency();
            foreach (IDataSet originalSet in DataSeries.Series)
            {
                if (!originalSet.Metadata.Enabled)
                { continue; }

                var set = new DataPointSet(originalSet);
                foreach (DataPoint p in originalSet.Set)
                {
                    double phase = PeriodDataProvider.GetPhased(p.X);
                    if (phase <= AbsIntervalSize)
                    { set.Add(phase, p.Y - originalSet.Metadata.OffsetY, p.Yerr); }
                    if (phase - 1 >= -AbsIntervalSize)
                    { set.Add(phase - 1, p.Y - originalSet.Metadata.OffsetY, p.Yerr); }
                }

                foreach (DataMark m in originalSet.XMarks)
                {
                    double phase = PeriodDataProvider.GetPhased(m.N);
                    if (phase <= AbsIntervalSize)
                    { set.AddXMark(m.Type, phase, m.Nerr * frequency); }
                    if (phase - 1 >= AbsIntervalSize)
                    { set.AddXMark(m.Type, phase - 1, m.Nerr * frequency); }
                }

                InsertPhaseMarks();
                Add(set);
                BoundingBox.Union(set.BoundingBox);
            }

            BoundingBox.Left = -AbsIntervalSize;
            BoundingBox.Right = AbsIntervalSize;
        }
    }

    public class BinnedSeries : DerivedSeriesWithPeriodProvider, IRefreshableDataSeries
    {
        public class Stat
        {
            public int Count = 0;
            public double Sum = 0;
            public double Err2Sum = 0;

            public double AvgSum { get { return Sum / Count; } }
            public double AvgErr { get { return Math.Sqrt(Err2Sum / Count); } }
        }

        public double BinDivision { get; set; }

        public BinnedSeries(IDataSeries dataSeries, IPeriodDataProvider periodDataProvider)
            : base(dataSeries, periodDataProvider)
        {
        }

        public void Refresh()
        {
            Clean();
            if (Double.IsNaN(BinDivision) || (BinDivision <= 0))
            {
                // no binning
                foreach (IDataSet originalSet in DataSeries.Series)
                { if (originalSet.Metadata.Enabled) { Add(originalSet); } }
                return;
            }

            // binning
            double invbin = 1.0 / BinDivision;
            var bins = new Dictionary<double, Stat>();
            var set = new DataPointSet(null, null);
            foreach (IDataSet originalSet in DataSeries.Series)
            {
                if (!originalSet.Metadata.Enabled)
                { continue; }
                foreach (DataPoint p in originalSet.Set)
                {
                    double binX = Math.Floor(p.X * invbin) / invbin + BinDivision / 2;
                    Stat stat;
                    if (!bins.TryGetValue(binX, out stat))
                    {
                        stat = new Stat();
                        bins.Add(binX, stat);
                    }
                    stat.Count++;
                    stat.Sum += p.Y;
                    stat.Err2Sum += p.Yerr * p.Yerr;
                }
                foreach (DataMark m in originalSet.XMarks)
                { set.AddXMark(m); }
            }

            foreach (var pair in bins)
            { set.Add(new DataPoint(pair.Key, pair.Value.AvgSum, pair.Value.AvgErr)); }
            set.Sort();
            this.Add(set);
        }
    }

    public class DephaseSeries : DerivedSeriesWithPeriodProvider, IRefreshableDataSeries
    {
        public DephaseSeries(IDataSeries dataSeries, IPeriodDataProvider periodDataProvider)
            : base(dataSeries, periodDataProvider)
        {
        }

        public void Refresh()
        {
            Clean();
            var set = new DataPointSet(null, null);
            if (PeriodDataProvider.CanProvidePeriodData)
            {
                foreach (IDataSet originalSet in DataSeries.Series)
                {
                    foreach (DataPoint p in originalSet.Set)
                    {
                        double phase = p.X;
                        if ((phase > -0.5) && (phase <= 0.5))
                        {
                            double x = PeriodDataProvider.GetDephased(phase);
                            set.Add(new DataPoint(x, p.Y, p.Yerr));
                        }
                    }
                    foreach (DataMark m in originalSet.XMarks)
                    { set.AddXMark(m); }
                }
                set.Sort();
            }
            this.Add(set);
        }
    }

    public class OCSeries : DerivedSeriesWithPeriodProvider, IRefreshableDataSeries
    {
        public OCSeries(IDataSeries dataSeries, IPeriodDataProvider periodDataProvider)
            : base(dataSeries, periodDataProvider)
        {
        }

        public void Refresh()
        {
            Clean();
            if (!PeriodDataProvider.CanProvidePeriodData)
            {
                BoundingBox = new BoundingBox(0, 0, 1, 1);
                return;
            }

            double period = PeriodDataProvider.GetPeriod();
            foreach (IDataSet originalSet in DataSeries.Series)
            {
                if (!originalSet.Metadata.Enabled)
                { continue; }

                var set = new DataPointSet(originalSet);

                foreach (DataMark m in originalSet.XMarks)
                {
                    double phase = PeriodDataProvider.GetPhased(m.N);
                    if (phase > 0.5)
                    { phase = phase - 1; }
                    set.Add(m.N, phase * period, m.Nerr);
                }

                InsertPhaseMarks();
                Add(set);
                BoundingBox.Union(set.BoundingBox);
            }
        }
    }
}
