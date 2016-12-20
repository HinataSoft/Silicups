using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Silicups.Core
{
    public static class FormatEx
    {
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

        public static T ParseEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        public static int ParseEnumToInt<T>(string value)
        {
            return (int)Enum.Parse(typeof(T), value, true);
        }
    }
}
