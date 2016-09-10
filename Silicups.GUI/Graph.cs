using System;
using System.Drawing;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;

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

        public delegate IEnumerable<DataPoint> DataSourceDelegate();

        public BoundingBox DataBB { get; set; }
        public BoundingBox ViewBB { get; set; }

        public int GraphBorderPaddingRatio = 20;
        public int MarkRatio = 100;
        public int MinorTickCount = 20;
        public int MajorTickEvery = 5;

        public Pen GraphBorder = Pens.Black;
        public Pen DataPointError = Pens.Gray;
        public Pen CoordMark = Pens.LightGray;

        public Brush DataPointBrush = Brushes.Black;
        public Brush AxisTextBrush = Brushes.Black;

        private Point MouseLastLocation = Point.Empty;
        private DataSourceDelegate dataSource = null;

        private void InitializeGraph()
        {
            this.SetStyle(ControlStyles.DoubleBuffer |
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.ResizeRedraw,
                true);

            this.UpdateStyles();

            DataSource = null;

            this.MouseDown += new MouseEventHandler(Graph_MouseDown);
            this.MouseMove += new MouseEventHandler(Graph_MouseMove);
            this.MouseUp += new MouseEventHandler(Graph_MouseUp);
            this.MouseWheel += new MouseEventHandler(Graph_MouseWheel);

            _t();
        }

        public DataSourceDelegate DataSource
        {
            get
            {
                return dataSource;
            }

            set
            {
                dataSource = value;
                DataBB = null;

                if(dataSource != null)
                {
                    DataBB = new BoundingBox() {
                        Left = Double.PositiveInfinity, Right = Double.NegativeInfinity,
                        Top = Double.PositiveInfinity, Bottom = Double.NegativeInfinity
                    };
                    foreach (DataPoint p in DataSource())
                    { DataBB.Union(p.X, p.Y); }
                    if ((DataBB.Left > DataBB.Right) || (DataBB.Bottom < DataBB.Top))
                    { DataBB = null; }
                }

                if (DataBB == null)
                {
                    DataBB = new BoundingBox() {
                        Left = 2457658.583, Right = 2457658.925,
                        Top = 13, Bottom = 14
                    };
                }
                else
                {
                    DataBB.ScaleX(0.1);
                    DataBB.ScaleY(0.1);
                }

                ViewBB = DataBB.Clone();
                Invalidate();
            }
        }

        public void UpdateDataSource(DataSourceDelegate updatedDataSource)
        {
            dataSource = updatedDataSource;
            Invalidate();
        }

        #region test

        private void _t()
        {
            //_t(0.056);
            //_t(0.046);
            //_t(210.056);
            //_t(190.056);
        }

        private void _t(double t)
        {
            System.Diagnostics.Trace.WriteLine(String.Format("N({0}) = {1}", t, NormalizeTick(t)));
        }

        #endregion

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
            ViewBB.ScaleX(0.001 * -e.Delta);

            if ((Control.ModifierKeys & Keys.Shift) != 0)
            { ViewBB.ScaleY(0.001 * -e.Delta); }
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
                double tick = NormalizeTick(ViewBB.Width / (MinorTickCount + 1));
                double start = NormalizeStart(ViewBB.Left, tick);
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
                double tick = NormalizeTick(ViewBB.Height / (MinorTickCount + 1));
                double start = NormalizeStart(ViewBB.Top, tick);

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
                foreach (DataPoint p in DataSource())
                {
                    float x = (float)(((p.X - ViewBB.Left) / ViewBB.Width) * graphWidth);
                    float y = (float)(((p.Y - ViewBB.Top) / ViewBB.Height) * graphHeight);
                    float yerr = (float)(p.Yerr / ViewBB.Height * graphHeight);
                    if ((x < markSize2) || (x > graphWidth - markSize2) || (y < markSize2) || (y > graphHeight - markSize2))
                    { continue; }

                    g.DrawLine(DataPointError, graphLeft + x, Math.Max(graphTop + y - yerr, graphTop), graphLeft + x, Math.Min(graphTop + y + yerr, graphBottom));
                    //g.FillEllipse(DataPointBrush, graphLeft + x - markSize2, graphTop + y - markSize2, markSize, markSize);
                }
                foreach (DataPoint p in DataSource())
                {
                    float x = (float)(((p.X - ViewBB.Left) / ViewBB.Width) * graphWidth);
                    float y = (float)(((p.Y - ViewBB.Top) / ViewBB.Height) * graphHeight);
                    float yerr = (float)(p.Yerr / ViewBB.Height * graphHeight);
                    if ((x < markSize2) || (x > graphWidth - markSize2) || (y < markSize2) || (y > graphHeight - markSize2))
                    { continue; }

                    //g.DrawLine(DataPointError, graphLeft + x, Math.Max(graphTop + y - yerr, graphTop), graphLeft + x, Math.Min(graphTop + y + yerr, graphBottom));
                    g.FillEllipse(DataPointBrush, graphLeft + x - markSize2, graphTop + y - markSize2, markSize, markSize);
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

        private double NormalizeTick(double tick)
        {
            double power = Math.Log10(tick);
            double powerfloor = Math.Floor(power);
            double tickbase1 = Math.Pow(10, powerfloor);
            double tickbase2 = tickbase1 * 2;
            double tickbase5 = tickbase1 * 5;
            double tickbase10 = tickbase1 * 10;

            if (tick <= tickbase2)
            { return tickbase2; }
            if (tick <= tickbase5)
            { return tickbase5; }
            return tickbase10;

            //if (tick >= tickbase5)
            //{ return tickbase5; }
            //if (tick >= tickbase2)
            //{ return tickbase2; }
            //return tickbase1;
        }

        private double NormalizeStart(double value, double tick)
        {
            return Math.Ceiling(value / tick) * tick;
        }
    }

    public class DataPoint
    {
        public double X;
        public double Y;
        public double Yerr;
    }

    public class TickPoint
    {
        public double value;
        public float coord;
        public int intCoord;
        public int index;
    }

    public class BoundingBox
    {
        public double Left { get; set; }
        public double Right { get; set; }
        public double Top { get; set; }
        public double Bottom { get; set; }

        public double Width { get { return Math.Abs(Left - Right); } }
        public double Height { get { return Math.Abs(Top - Bottom); } }

        public BoundingBox Clone()
        {
            return new BoundingBox() {
                Left = this.Left, Right = this.Right,
                Top = this.Top, Bottom = this.Bottom
            };
        }

        public void TruncateTo(BoundingBox other)
        {
            Left = Math.Max(Left, other.Left);
            Right = Math.Min(Right, other.Right);
            Top = Math.Max(Top, other.Top);
            Bottom = Math.Min(Bottom, other.Bottom);
        }

        public void ShiftTo(BoundingBox other)
        {
            if (other.Left > Left)
            { TranslateX(other.Left - Left); }
            else if (other.Right < Right)
            { TranslateX(other.Right - Right); }

            if (other.Top > Top)
            { TranslateY(other.Top - Top); }
            else if (other.Bottom < Bottom)
            { TranslateY(other.Bottom - Bottom); }
        }

        public void TranslateX(double dx)
        {
            Left += dx;
            Right += dx;
        }

        public void TranslateY(double dy)
        {
            Top += dy;
            Bottom += dy;
        }

        public void Union(double x, double y)
        {
            if (Left > x) { Left = x; }
            if (Right < x) { Right = x; }
            if (Top > y) { Top = y; }
            if (Bottom < y) { Bottom = y; }
        }

        public void ScaleX(double factor)
        {
            double addition2 = Width * factor / 2;
            Left -= addition2;
            Right += addition2;
        }

        public void ScaleY(double factor)
        {
            double addition2 = Height * factor / 2;
            Top -= addition2;
            Bottom += addition2;
        }
    }

    public static class MathEx
    {
        public static float MinMax(float min, float value, float max)
        {
            return Math.Min(Math.Max(min, value), max);
        }

        public static double MinMax(double min, double value, double max)
        {
            return Math.Min(Math.Max(min, value), max);
        }
    }
}
