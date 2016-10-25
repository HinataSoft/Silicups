using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Silicups.Core
{
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

        public static double ParseDouble(string s)
        {
            return Double.Parse(s.Replace(',', '.'), System.Globalization.CultureInfo.InvariantCulture);
        }

        public static string FormatDouble(double d)
        {
            return d.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }

        public static string FormatDouble(double? d)
        {
            return d.HasValue ? FormatDouble(d.Value) : null;
        }

        public static double GetLower125Base(double d)
        {
            var bases = new Bases125(d);
            if (d >= bases.base5)
            { return bases.base5; }
            if (d >= bases.base2)
            { return bases.base2; }
            return bases.base1;
        }

        public static double GetHigher125Base(double d)
        {
            var bases = new Bases125(d);
            if (d <= bases.base2)
            { return bases.base2; }
            if (d <= bases.base5)
            { return bases.base5; }
            return bases.base10;
        }

        public static double RoundToHigher(double value, double step)
        {
            return Math.Ceiling(value / step) * step;
        }

        public static double RoundToLower(double value, double step)
        {
            return Math.Floor(value / step) * step;
        }

        public struct Bases125
        {
            public double base1;
            public double base2;
            public double base5;
            public double base10;

            public Bases125(double d)
            {
                double power = Math.Log10(d);
                double powerfloor = Math.Floor(power);
                base1 = Math.Pow(10, powerfloor);
                base2 = base1 * 2;
                base5 = base1 * 5;
                base10 = base1 * 10;
            }
        }

    }
}
