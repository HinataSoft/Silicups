using System;
using System.Collections.Generic;
using System.Text;

namespace Silicups.Core
{
    public static class JD
    {
        // http://quasar.as.utexas.edu/BillInfo/JulianDatesG.html
        public static DateTime JDToDateTime(double jd)
        {
            double q = jd + 0.5;
            int z = (int)Math.Floor(q);
            int w = (int)((z - 1867216.25) / 36524.25);
            int x = w / 4;
            int a = z + 1 + w - x;
            int b = a + 1524;
            int c = (int)((b - 122.1) / 365.25);
            int d = (int)(365.25 * c);
            int e = (int)((b - d) / 30.6001);
            int f = (int)(30.6001 * e);
            int day = (int)(b - d - f + (q - z));
            int month = e - 1; if (month > 12) { month -= 12; }
            int year = (month <= 2) ? c - 4715 : c - 4716;

            return new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Utc).AddTicks((long)((q - z) * TimeSpan.TicksPerDay));
        }

        // http://stackoverflow.com/a/26676794
        public static double DateTimeToJD(DateTime dateTime)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan diff = dateTime.ToUniversalTime() - origin;
            double unixTime = Math.Floor(diff.TotalSeconds);
            double julianDate = (unixTime / 86400) + 2440587.5;

            return julianDate;
        }
    }
}
