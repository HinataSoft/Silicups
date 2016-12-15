using System;
using System.Drawing;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;

using Silicups.Core;

namespace Silicups.GUI
{
    public partial class Graph : Panel
    {
        #region Default contructors

        public Graph()
        {
            InitializeComponent();
            InitializeGraph();
        }

        public Graph(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
            InitializeGraph();
        }

        #endregion

        private BoundingBox DataBB = BoundingBox.CloneUnary();
        private BoundingBox ViewBB = BoundingBox.CloneUnary();

        public int GraphBorderPaddingRatio = 20;
        public int MarkRatio = 100;
        public int MinorTickCount = 20;
        public int MajorTickEvery = 5;

        public Pen GraphBorder = Pens.Black;
        public Pen DataPointError = Pens.Gray;
        public Pen CoordMark = Pens.LightGray;
        public Pen XMark = Pens.Blue;

        public Brush DataPointBrush = Brushes.Black;
        public Brush DataPointHighlightedBrush = Brushes.Red;
        public Brush AxisTextBrush = Brushes.Black;

        private Point MouseLastLocation = Point.Empty;
        private IDataSeries DataSource = null;
        private bool DataSourceInited = false;

        private void InitializeGraph()
        {
            this.SetStyle(ControlStyles.DoubleBuffer |
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.ResizeRedraw,
                true);

            this.UpdateStyles();

            DataSource = null;
            DataSourceInited = false;

            this.MouseDown += new MouseEventHandler(Graph_MouseDown);
            this.MouseMove += new MouseEventHandler(Graph_MouseMove);
            this.MouseUp += new MouseEventHandler(Graph_MouseUp);
            this.MouseWheel += new MouseEventHandler(Graph_MouseWheel);
        }

        public void SetDataSource(IDataSeries dataSeries)
        {
            if (dataSeries == null)
            {
                DisableDataSource();
                return;
            }

            DataSource = dataSeries;
            DataSourceInited = true;
            DataBB = DataSource.BoundingBox.Clone();
            DataBB.AddScale(0.1);
            ViewBB = DataBB.Clone();
            Invalidate();
        }

        public void UpdateDataSource(IDataSeries dataSeries)
        {
            if (!DataSourceInited)
            {
                SetDataSource(dataSeries);
                return;
            }

            DataSource = dataSeries;
            Invalidate();
        }

        public void DisableDataSource()
        {
            DataSource = null;
            DataSourceInited = false;
            DataBB = BoundingBox.CloneUnary();
            DataBB.AddScale(0.1);
            ViewBB = DataBB.Clone();
            Invalidate();
        }

        private float GraphBorderPadding
        {
            get
            {
                int sideSize = Math.Min(this.Width, this.Height);
                return sideSize / GraphBorderPaddingRatio;
            }
        }

        void Graph_MouseDown(object sender, MouseEventArgs e)
        {
            if ((e.Button == System.Windows.Forms.MouseButtons.Left) && !e.Location.IsEmpty)
            { MouseLastLocation = e.Location; }
        }

        void Graph_MouseMove(object sender, MouseEventArgs e)
        {
            Graph_MouseLocationChanged(e.Location);
        }

        void Graph_MouseUp(object sender, MouseEventArgs e)
        {
            Graph_MouseLocationChanged(e.Location);
            MouseLastLocation = Point.Empty;
        }

        void Graph_MouseLocationChanged(Point newLocation)
        {
            if (MouseLastLocation.IsEmpty || newLocation.IsEmpty)
            { return; }

            ViewBB.TranslateX(-ViewBB.Width * (newLocation.X - MouseLastLocation.X) / this.Width);
            ViewBB.TranslateY(-ViewBB.Height * (newLocation.Y - MouseLastLocation.Y) / this.Height);
            ViewBB.ShiftTo(DataBB);

            MouseLastLocation = newLocation;
            Invalidate();
        }

        void Graph_MouseWheel(object sender, MouseEventArgs e)
        {
            float graphBorderPadding = GraphBorderPadding;
            ViewBB.AddScaleX(0.001 * -e.Delta, (e.Location.X - graphBorderPadding) / (this.Width - 2 * graphBorderPadding));

            if ((Control.ModifierKeys & Keys.Shift) != 0)
            { ViewBB.AddScaleY(0.001 * -e.Delta, (e.Location.Y - graphBorderPadding) / (this.Height - 2 * graphBorderPadding)); }
            ViewBB.TruncateTo(DataBB);
            Invalidate();
        }

        private void Graph_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(this.BackColor);

            if ((this.Width < 20) || (this.Height < 20))
            { return; }

            int sideSize = Math.Min(this.Width, this.Height);
            int graphBorderPadding = sideSize / GraphBorderPaddingRatio;
            float markSize = MathEx.MinMax(2, sideSize * 1f / MarkRatio, 5);
            float markSize2 = markSize / 2;
            float tickSize = graphBorderPadding / 3f;
            int tickLength = (int)Math.Round(tickSize);

            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

            var axisTextFont = new Font(FontFamily.GenericSansSerif, (float)(graphBorderPadding / 2.0));

            // graph border
            int graphWidth = this.Width - 2 * graphBorderPadding;
            int graphHeight = this.Height - 2 * graphBorderPadding;
            int graphLeft = graphBorderPadding;
            int graphTop = graphBorderPadding;
            int graphBottom = this.Height - graphBorderPadding;

            // X axis (prep)
            var xTicks = new List<TickPoint>();
            {
                double tick = MathEx.GetHigher125Base(ViewBB.Width / (MinorTickCount + 1));
                double start = MathEx.RoundToHigher(ViewBB.Left, tick);
                int tickIndex = 0;
                double value = start;
                while (value < ViewBB.Right)
                {
                    float coord = graphLeft + (float)(((value - ViewBB.Left) / ViewBB.Width) * graphWidth);
                    xTicks.Add(new TickPoint() {
                        value = value,
                        coord = coord,
                        intCoord = (int)Math.Round(coord),
                        index = tickIndex
                    });

                    tickIndex++;
                    value = start + tickIndex * tick;
                }
            }

            // Y axis
            var yTicks = new List<TickPoint>();
            {
                double tick = MathEx.GetHigher125Base(ViewBB.Height / (MinorTickCount + 1));
                double start = MathEx.RoundToHigher(ViewBB.Top, tick);

                int tickIndex = 0;
                double value = start;
                while (value < ViewBB.Bottom)
                {
                    float coord = graphTop + (float)(((value - ViewBB.Top) / ViewBB.Height) * graphHeight);
                    yTicks.Add(new TickPoint() {
                        value = value,
                        coord = coord,
                        intCoord = (int)Math.Round(coord),
                        index = tickIndex
                    });

                    tickIndex++;
                    value = start + tickIndex * tick;
                }
            }

            // crosses
            {
                int crossLength = (int)Math.Ceiling(tickSize / 2);
                foreach (TickPoint yTick in yTicks)
                {
                    foreach (TickPoint xTick in xTicks)
                    {
                        g.DrawLine(CoordMark, xTick.intCoord - crossLength, yTick.intCoord, xTick.intCoord + crossLength, yTick.intCoord);
                        g.DrawLine(CoordMark, xTick.intCoord, yTick.intCoord - crossLength, xTick.intCoord, yTick.intCoord + crossLength);
                    }
                }
            }

            // data
            if (DataSource != null)
            {
                foreach (IDataSet set in DataSource.Series)
                {
                    if (!set.Metadata.Enabled)
                    { continue; }
                    foreach (DataPoint p in set.Set)
                    {
                        float x = (float)(((p.X - ViewBB.Left) / ViewBB.Width) * graphWidth);
                        float y = (float)(((p.Y - ViewBB.Top) / ViewBB.Height) * graphHeight);
                        float yerr = (float)(p.Yerr / ViewBB.Height * graphHeight);
                        if ((x < markSize2) || (x > graphWidth - markSize2) || (y < markSize2) || (y > graphHeight - markSize2))
                        { continue; }

                        g.DrawLine(DataPointError, graphLeft + x, Math.Max(graphTop + y - yerr, graphTop), graphLeft + x, Math.Min(graphTop + y + yerr, graphBottom));
                    }
                }
                foreach (IDataSet set in DataSource.Series)
                {
                    if (!set.Metadata.Enabled)
                    { continue; }
                    foreach (DataMark m in set.XMarks)
                    {
                        float x = (float)(((m.N - ViewBB.Left) / ViewBB.Width) * graphWidth);
                        if ((x < markSize2) || (x > graphWidth - markSize2))
                        { continue; }
                        g.DrawLine(XMark, graphLeft + x, graphTop, graphLeft + x, graphHeight);
                    }
                } 
                foreach (IDataSet set in DataSource.Series)
                {
                    if (!set.Metadata.Enabled || set.Metadata.Hightlighted)
                    { continue; }
                    foreach (DataPoint p in set.Set)
                    {
                        float x = (float)(((p.X - ViewBB.Left) / ViewBB.Width) * graphWidth);
                        float y = (float)(((p.Y - ViewBB.Top) / ViewBB.Height) * graphHeight);
                        float yerr = (float)(p.Yerr / ViewBB.Height * graphHeight);
                        if ((x < markSize2) || (x > graphWidth - markSize2) || (y < markSize2) || (y > graphHeight - markSize2))
                        { continue; }

                        g.FillEllipse(DataPointBrush, graphLeft + x - markSize2, graphTop + y - markSize2, markSize, markSize);
                    }
                }
                foreach (IDataSet set in DataSource.Series)
                {
                    if (!set.Metadata.Enabled || !set.Metadata.Hightlighted)
                    { continue; }
                    foreach (DataPoint p in set.Set)
                    {
                        float x = (float)(((p.X - ViewBB.Left) / ViewBB.Width) * graphWidth);
                        float y = (float)(((p.Y - ViewBB.Top) / ViewBB.Height) * graphHeight);
                        float yerr = (float)(p.Yerr / ViewBB.Height * graphHeight);
                        if ((x < markSize2) || (x > graphWidth - markSize2) || (y < markSize2) || (y > graphHeight - markSize2))
                        { continue; }

                        g.FillEllipse(DataPointHighlightedBrush, graphLeft + x - markSize2, graphTop + y - markSize2, markSize, markSize);
                    }
                }
            }

            // border
            g.DrawRectangle(GraphBorder, graphLeft, graphTop, graphWidth, graphHeight);

            // X axis
            foreach (TickPoint tick in xTicks)
            {
                int length = tickLength;
                if ((tick.index % MajorTickEvery) == 0)
                {
                    length *= 2;
                    string tickMark = tick.value.ToString();
                    SizeF stringSize = g.MeasureString(tickMark, axisTextFont);
                    float stringCoord = MathEx.MinMax(0, tick.coord - stringSize.Width / 2, this.Width);
                    g.DrawString(tickMark, axisTextFont, AxisTextBrush, stringCoord, graphBottom);
                }
                g.DrawLine(GraphBorder, tick.intCoord, graphBottom - length, tick.intCoord, graphBottom);
            }

            // Y axis
            foreach (TickPoint tick in yTicks)
            {
                int length = tickLength;
                if ((tick.index % MajorTickEvery) == 0)
                {
                    length *= 2;
                    string tickMark = tick.value.ToString();
                    SizeF stringSize = g.MeasureString(tickMark, axisTextFont);
                    float stringCoord = MathEx.MinMax(graphTop, tick.coord - stringSize.Height / 2, graphBottom - stringSize.Height);
                    g.DrawString(tickMark, axisTextFont, AxisTextBrush, graphLeft, stringCoord);
                }
                g.DrawLine(GraphBorder, graphLeft - length, tick.intCoord, graphLeft, tick.intCoord);
            }
        }
    }

    public class TickPoint
    {
        public double value;
        public float coord;
        public int intCoord;
        public int index;
    }
}
