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
    }
}
