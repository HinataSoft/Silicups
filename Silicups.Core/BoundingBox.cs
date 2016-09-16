using System;
using System.Collections.Generic;
using System.Text;

namespace Silicups.Core
{
    public class BoundingBox
    {
        private static readonly BoundingBox Empty = new BoundingBox(Double.PositiveInfinity, Double.PositiveInfinity, Double.NegativeInfinity, Double.NegativeInfinity);
        private static readonly BoundingBox Unary = new BoundingBox(0, 0, 1, 1);

        private double left;
        private double right;
        private double top;
        private double bottom;

        public double Left { get { return left; } set { left = value; } }
        public double Right { get { return right; } set { right = value; } }
        public double Top { get { return top; } set { top = value; } }
        public double Bottom { get { return bottom; } set { bottom = value; } }

        public double Width { get { return Math.Abs(Left - Right); } }
        public double Height { get { return Math.Abs(Top - Bottom); } }

        public bool IsEmpty
        {
            get
            {
                return (Left == Double.PositiveInfinity)
                    && (Top == Double.PositiveInfinity)
                    && (Right == Double.NegativeInfinity)
                    && (Bottom == Double.NegativeInfinity);
            }
        }

        public BoundingBox(double left, double top, double right, double bottom)
        {
            this.left = left;
            this.top = top;
            this.right = right;
            this.bottom = bottom;
        }

        public BoundingBox Clone()
        {
            return new BoundingBox(Left, Top, Right, Bottom);
        }

        public static BoundingBox CloneEmpty()
        {
            return Empty.Clone();
        }

        public static BoundingBox CloneUnary()
        {
            return Unary.Clone();
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

        public void Union(double x, double y)
        {
            if (Left > x) { Left = x; }
            if (Right < x) { Right = x; }
            if (Top > y) { Top = y; }
            if (Bottom < y) { Bottom = y; }
        }

        public void Union(BoundingBox other)
        {
            if (Left > other.Left) { Left = other.Left; }
            if (Right < other.Right) { Right = other.Right; }
            if (Top > other.Top) { Top = other.Top; }
            if (Bottom < other.Bottom) { Bottom = other.Bottom; }
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

        public void AddScale(double factor, double ratio = 0.5)
        {
            AddScaleX(factor, ratio);
            AddScaleY(factor, ratio);
        }

        public void AddScaleX(double factor, double ratio = 0.5)
        {
            ratio = MathEx.MinMax(0, ratio, 1);
            double addition = Width * factor;
            Left -= addition * ratio;
            Right += addition * (1 - ratio);
        }

        public void AddScaleY(double factor, double ratio = 0.5)
        {
            ratio = MathEx.MinMax(0, ratio, 1);
            double addition = Height * factor;
            Top -= addition * ratio;
            Bottom += addition * (1 - ratio);
        }
    }
}
