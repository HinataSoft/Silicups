using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Silicups.GUI
{
    public partial class Glider : Panel
    {
        public class GliderEventArgs : EventArgs
        {
            public double GliderValue { get; private set; }

            public GliderEventArgs(double gliderValue)
            {
                this.GliderValue = gliderValue;
            }
        }

        public delegate void GliderEventHandler(object sender, GliderEventArgs e);

        private Pen MainCrossPen = Pens.Black;
        private Pen DiagonalCrossPen = Pens.DarkGray;
        private Pen DisabledPen = Pens.LightGray;
        private Point MiddlePoint = Point.Empty;

        public double GliderValue { get; private set; }
        public event GliderEventHandler GliderValueChanged;
        public event GliderEventHandler GliderValueConfirmed;

        public Glider()
        {
            this.SetStyle(ControlStyles.DoubleBuffer |
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.ResizeRedraw,
                true);

            this.UpdateStyles();

            InitializeComponent();

            this.SizeChanged += new EventHandler(Glider_SizeChanged);
            this.MouseLeave += new EventHandler(Glider_MouseLeave);
            this.MouseMove += new MouseEventHandler(Glider_MouseMove);
            this.MouseClick += new MouseEventHandler(Glider_MouseClick);
            this.Paint += new PaintEventHandler(Glider_Paint);
        }

        void Glider_SizeChanged(object sender, EventArgs e)
        {
            MiddlePoint = new Point(Size.Width / 2, Size.Height / 2);
        }

        void Glider_MouseLeave(object sender, EventArgs e)
        {
            OnGliderValueChanged(0);
        }

        void Glider_MouseMove(object sender, MouseEventArgs e)
        {
            OnGliderValueChanged(GetGliderValue(e.Location));
        }

        void Glider_MouseClick(object sender, MouseEventArgs e)
        {
            OnGliderValueConfirmed(GetGliderValue(e.Location));
            Cursor.Position = PointToScreen(MiddlePoint);
        }

        void Glider_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            int width = this.Size.Width - 1;
            int height = this.Size.Height - 1;
            int middleWidth = MiddlePoint.X;
            int middleHeight = MiddlePoint.Y;

            Pen mainCrossPen = this.Enabled ? MainCrossPen : DisabledPen;
            Pen diagonalCrossPen = this.Enabled ? DiagonalCrossPen : DisabledPen;

            g.DrawLine(diagonalCrossPen, 0, 0, width, height);
            g.DrawLine(diagonalCrossPen, 0, height, width, 0);
            g.DrawLine(mainCrossPen, 0, middleHeight, width, middleHeight);
            g.DrawLine(mainCrossPen, middleWidth, 0, middleWidth, height);
            g.DrawRectangle(mainCrossPen, 0, 0, width, height);
        }

        private double GetGliderValue(Point point)
        {
            double dX = point.X - MiddlePoint.X;
            double dY = MiddlePoint.Y - point.Y;
            double dWidth = Size.Width;
            double dHeight = Size.Height;
            return (dX / dWidth) + (2.0 / Size.Width) * (dY / dHeight);
        }

        private void OnGliderValueChanged(double gliderValue)
        {
            this.GliderValue = gliderValue;
            GliderEventHandler gliderValueChanged = GliderValueChanged;
            if (gliderValueChanged != null)
            { gliderValueChanged(this, new GliderEventArgs(gliderValue)); }
        }

        private void OnGliderValueConfirmed(double gliderValue)
        {
            this.GliderValue = 0;
            GliderEventHandler gliderValueConfirmed = GliderValueConfirmed;
            if (gliderValueConfirmed != null)
            { gliderValueConfirmed(this, new GliderEventArgs(gliderValue)); }
        }
    }
}
