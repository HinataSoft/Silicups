using System;
using System.Collections.Generic;
using System.Text;

namespace Silicups.Util
{
    public class HelCor
    {
        private static class physics_sfasu_edu
        {
            // http://www.physics.sfasu.edu/astro/javascript/hjd.html

            // Math wrapper
            static readonly double pi = Math.PI;
            static readonly double Rads = 0.017453292519943295769236907684886; // pi / 180.0
            static double sin(double a) { return Math.Sin(a); }
            static double cos(double a) { return Math.Cos(a); }
            static double asin(double a) { return Math.Asin(a); }
            static double atan(double a) { return Math.Atan(a); }
            static double abs(double a) { return Math.Abs(a); }
            static double tan(double a) { return Math.Tan(a); }
            static double floor(double a) { return Math.Floor(a); }
            static double pow(double a, double e) { return Math.Pow(a, e); }


            static void GetPosition(double param_jd, out double ra, out double dec)
            {
                double D = param_jd - 2451545;

                double pe = (102.94719 + 0.00000911309 * D) * Rads;
                double ae = 1.00000011 - 1.36893E-12 * D;
                double ee = 0.01671022 - 0.00000000104148 * D;
                double le = (Rads * (100.46435 + 0.985609101 * D));

                // Get Earth's position using Kepler's equation
                double Me1 = (le - pe);
                double B = Me1 / (2 * pi);
                Me1 = 2 * pi * (B - floor(abs(B)));
                if (B < 0)
                { Me1 = 2 * pi * (B + floor(abs(B))); }
                if (Me1 < 0)
                { Me1 = 2 * pi + Me1; }

                double e = Me1;
                double delta = 0.05;
                while (abs(delta) >= pow(10, -12))
                {
                    delta = e - ee * sin(e) - Me1;
                    e = e - delta / (1 - ee * cos(e));
                }
                double ve = 2 * atan(pow(((1 + ee) / (1 - ee)), 0.5) * tan(0.5 * e));
                if (ve < 0)
                { ve = ve + 2 * pi; }
                double re = ae * (1 - ee * ee) / (1 + ee * cos(ve));
                double xe = re * cos(ve + pe);
                double ye = re * sin(ve + pe);
                double ze = 0;

                // Sun Coodinates
                double ecl = 23.439292 * Rads;
                double xeq = xe;
                double yeq = ye * cos(ecl) - ze * sin(ecl);
                double zeq = ye * sin(ecl) + ze * cos(ecl);

                ra = 12 + atan(yeq / xeq) * 12 / pi;
                if (xe < 0)
                { ra += 12; }
                if ((ye < 0) && (xe > 0))
                { ra += 24; }

                dec = -180 * atan(zeq / pow((xeq * xeq + yeq * yeq), 0.5)) / pi;
            }

            static public double get_helcor(double param_jd, double param_ra, double param_dec)
            {
                // Sun
                double ra, dec;
                GetPosition(param_jd, out ra, out dec);

                //double rah = floor(ra);
                //double ram = floor((ra - floor(ra)) * 60);
                //double decd = floor(abs(dec));
                //if (dec < 0)
                //{ decd = -1 * decd; }
                //double decm = floor((abs(dec) - floor(abs(dec))) * 60);
                //Console.WriteLine("Sun -- RA: " + rah + "h " + ram + "m   DEC: " + decd + "o " + decm + "'");

                // Earth
                dec = -1 * dec;
                ra = ra + 12;
                if (ra > 24)
                { ra = ra - 24; }

                //rah = floor(ra);
                //ram = floor((ra - floor(ra)) * 60);
                //decd = floor(abs(dec));
                //if (dec < 0) { decd = -1 * decd; }
                //decm = floor((abs(dec) - floor(abs(dec))) * 60);
                //Console.WriteLine("Earth -- RA: " + rah + "h " + ram + "m   DEC: " + decd + "o " + decm + "'");

                //Object    
                double ORA = param_ra * 15;
                double ODEC = param_dec;

                //Earth XYZ
                double cel = cos(dec * pi / 180);
                double earthx = cos(ra * pi / 12) * cel;
                double earthy = sin(ra * pi / 12) * cel;
                double earthz = sin(dec * pi / 180);
                //Console.WriteLine("Earth -- X,Y,Z: " + earthx + "," + earthy + "," + earthz);

                //Object XYZ
                cel = cos(ODEC * pi / 180);
                double objectx = cos(ORA * pi / 180) * cel;
                double objecty = sin(ORA * pi / 180) * cel;
                double objectz = sin(ODEC * pi / 180);
                //Console.WriteLine("Object -- X,Y,Z: " + objectx + "," + objecty + "," + objectz);
                //Console.WriteLine("Julian Date: " + param_jd);

                //Light Time (Minutes per AU)
                double ausec = 8.3168775;
                return ausec * (earthx * objectx + earthy * objecty + earthz * objectz) / (24 * 60);
            }
        }

        private static class keele_ac_uk
        {
            // http://www.astro.keele.ac.uk/oldusers/rno/Astronomy/hjd-0.1.c

            // Math wrapper
            static readonly double M_PI = Math.PI;
            static double sin(double a) { return Math.Sin(a); }
            static double cos(double a) { return Math.Cos(a); }
            static double asin(double a) { return Math.Asin(a); }
            static double atan(double a) { return Math.Atan(a); }
            static double abs(double a) { return Math.Abs(a); }
            static double tan(double a) { return Math.Tan(a); }
            static double floor(double a) { return Math.Floor(a); }

            const double ausec = 499.01265;   /* Time it takes light to travel 1 AU */
            const double degtorad = 0.01745329251; /* Conversion factor from degrees to radians */

            struct coordinates
            {
                public double ra;   /* The right ascension in degrees */
                public double dec;  /* The declination in degrees */
                public double x;    /* The X position in AU */
                public double y;    /* The Y position in AU */
                public double z;    /* The Z position in AU */
            }

            static public double get_helcor(double param_jd, double param_ra, double param_dec)
            {
                coordinates earth = new coordinates();  /* Earth co-ordinates */
                coordinates source = new coordinates(); /* Source co-ordinates */

                double correction_secs; /* Correction factor in seconds from MJD to HMJD */
                double cel;   /* intermediate calculation from spherical to cartesian co-ordinates */

                /* Defaults */

                earth.ra = earth.dec = 0.0;

                /* Enter the co-ordinates of the source */
                /* Calculate the RA and declination in decimal degree notation */

                source.ra = param_ra;
                source.dec = param_dec;

                /* Attempt to find the RA and Dec of the earth using the astronomical
                   calculator book. */

                heliocentric_ra_dec(param_jd, out earth.ra, out earth.dec);

                //double rah = floor(earth.ra);
                //double ram = floor((earth.ra - floor(earth.ra)) * 60);
                //double decd = floor(abs(earth.dec));
                //if (earth.dec < 0) { decd = -1 * decd; }
                //double decm = floor((abs(earth.dec) - floor(abs(earth.dec))) * 60);
                //Console.WriteLine("Earth -- RA: " + rah + "h " + ram + "m   DEC: " + decd + "o " + decm + "'");

                /* Calculate the heliocentric co-ordinates as X, Y and Z terms */
                cel = cos(earth.dec * degtorad);
                earth.x = cos(earth.ra * 15 * degtorad) * cel;
                earth.y = sin(earth.ra * 15 * degtorad) * cel;
                earth.z = sin(earth.dec * degtorad);

                //Console.WriteLine("Earth -- X,Y,Z: " + earth.x + "," + earth.y + "," + earth.z);

                /* Calculate the X,Y,Z co-ordinates of your source */
                cel = cos(source.dec * degtorad);
                source.x = cos(source.ra * 15 * degtorad) * cel;
                source.y = sin(source.ra * 15 * degtorad) * cel;
                source.z = sin(source.dec * degtorad);

                //Console.WriteLine("Object -- X,Y,Z: " + source.x + "," + source.y + "," + source.z);

                /* Calculate the correction in seconds for the light travel time
                   between the source and the earth vectors in a heliocentric
                   reference frame. */

                correction_secs = ausec *
                  (earth.x * source.x + earth.y * source.y + earth.z * source.z);

                return (correction_secs / (24.0 * 3600));
            }

            /*
             * heliocentric_ra_dec
             *           - procedure to calculate heliocentric right ascension and
             *             declination of the earth at a given date.
             *
             * Inputs
             *   mjd  - the modified Julian date
             *
             * Returns
             *   *ra  - the right ascension of the earth
             *   *dec - the declination of the earth
             *
             */

            const double eccentricity = 0.016718;    /* Eccentricity of the Earth's orbit*/
            const double ecliptic_long = 278.833540; /* The longitude of the ecliptic at 1 Jan 1980  0:00 UT */
            const double perigee_long = 282.596403;  /* The longitude of perigee at 1 Jan 1980 00:00 UT */
            const double deg_to_rad = 0.017453292519943295769236907684886; // M_PI / 180.0;  /* A degrees to radians converion */
            const double tropical_year = 365.24219572; /* The length of the tropical year in days */
            const double obliquity = 23.441884; /* The obliquity of the orbit */
            //const double mjd_1980 = 44238.0;   /* The MJD on 1 Jan 1980 00:00 UT */
            const double jd_1980 = 44238.0 + 2400000.5;   /* The JD on 1 Jan 1980 00:00 UT */

            static void heliocentric_ra_dec(double param_jd, out double ra, out double dec)
            {
                double mean_anomoly;  /* The mean anomoly of the sun */
                double days_from_1980; /* The number of days from 1 Jan 1980 */
                double solar_longitude; /* The longitude of the sun */
                double number_of_deg;   /* The number of degrees in longitude the sun has travelled */
                double equation_of_centres; /* The value for the equation of centres */
                double x;  /* An X position */
                double y;  /* A  Y position */
                double beta; /* The ecliptic longitude */
                double number_of_rotations; /* An integer number of solar orbits */

                /* Calculate the number of days from 1 Jan 1980 */

                //days_from_1980 = (mjd - mjd_1980);
                days_from_1980 = (param_jd - jd_1980);

                /* Calculate the number of degrees around in the orbit travelled in this time */

                number_of_deg = (360.0 / tropical_year) * days_from_1980;

                /* Adjust so the number of degrees is between 0 and 360 */

                if ((number_of_deg < 0.0) || (number_of_deg > 360.0))
                {
                    number_of_rotations = number_of_deg / 360.0;
                    number_of_rotations = floor(number_of_rotations);
                    number_of_deg -= number_of_rotations * 360.0;
                }

                /* Calculate the mean anomoly */

                mean_anomoly = number_of_deg - perigee_long + ecliptic_long;

                /* Since the orbit is elliptical and not circular, calculate the equation of centres */

                equation_of_centres = (360.0 / M_PI) * eccentricity *
                  sin(mean_anomoly * deg_to_rad);

                /* Calculate the solar longitude */

                solar_longitude = number_of_deg + equation_of_centres + ecliptic_long;
                if (solar_longitude > 360.0)
                    solar_longitude -= 360.0;

                /* The ecliptic latitude is zero for the Sun. */

                beta = 0.0;

                /* Calculate the RA and Dec of the sun */

                dec = asin((sin(beta * deg_to_rad) * cos(obliquity * deg_to_rad)) +
                        (cos(beta * deg_to_rad) * sin(obliquity * deg_to_rad) *
                         sin(solar_longitude * deg_to_rad)));

                dec /= deg_to_rad;
                x = cos(solar_longitude * deg_to_rad);
                y = (sin(solar_longitude * deg_to_rad) *
                     cos(obliquity * deg_to_rad)) -
                  (tan(beta * deg_to_rad) * sin(obliquity * deg_to_rad));
                ra = atan(y / x);
                ra /= deg_to_rad;

                if (ra < 0.0)
                    ra += 360.0;

                ra /= 15.0;

                /* Convert from geocentric to heliocentric co-ordinates for the
                   Earth*/

                dec *= -1.0;
                ra -= 12.0;
                if (ra < 0.0)
                {
                    ra += 24.0;
                }

            }
        }

        private static class MuniPack
        {
            /**************************************************************

            hcor.c (C-Munipack project)
            Heliocentric correction
            Copyright (C) 2003 David Motl, dmotl@volny.cz

            Originates from original Munipack by Filip Hroch.

             trajd.f -- translated by f2c (version 19950110).

            This program is free software; you can redistribute it and/or
            modify it under the terms of the GNU General Public License
            as published by the Free Software Foundation; either version 2
            of the License, or (at your option) any later version.

            This program is distributed in the hope that it will be useful,
            but WITHOUT ANY WARRANTY; without even the implied warranty of
            MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
            GNU General Public License for more details.

            You should have received a copy of the GNU General Public License
            along with this program; if not, write to the Free Software
            Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.

            **************************************************************/

            /* Inacuracy in singularity tests */
            static readonly double TOL = 0.000001;

            /* Angle of earth's axis */
            static readonly double EPS = ((23 + 26 / 60.0) / 180 * M_PI);

            // Math wrapper
            static readonly double M_PI = Math.PI;
            static double sin(double a) { return Math.Sin(a); }
            static double cos(double a) { return Math.Cos(a); }
            static double asin(double a) { return Math.Asin(a); }
            static double atan(double a) { return Math.Atan(a); }
            static double fabs(double a) { return Math.Abs(a); }

            /**********************   HELPER FUNCTIONS   ********************************/

            /* Pocita heliocentrickou korekci */
            static double cmpack_hcor2(double la, double be, double ls, double rs)
            {
                return -0.00577552 * rs * cos(be) * cos(la - ls);
            }

            /* Prevede souradnice ra,de (rovnikove) na la, be (ekliptikalni) */
            static void cmpack_rdtolb(double ra, double de, out double la, out double be)
            {
                /* Prevod na radiany */
                ra = ra / 12.0 * M_PI;
                de = de / 180.0 * M_PI;

                /* Do intervalu 0..2pi */
                while (ra < 0.0) ra += 2 * M_PI;
                while (ra >= 2 * M_PI) ra -= 2 * M_PI;

                /* Vylouceni singularit */
                if (de > M_PI / 2 - TOL)
                {
                    la = M_PI / 2;
                    be = M_PI / 2 - EPS;
                }
                else if (de < -M_PI / 2 + TOL)
                {
                    la = 3 * M_PI / 2;
                    be = EPS - M_PI / 2;
                }
                else if (fabs(ra - M_PI / 2) < TOL)
                {
                    la = M_PI / 2;
                    be = de - EPS;
                }
                else if (fabs(ra - 3 * M_PI / 2) < TOL)
                {
                    la = 3 * M_PI / 2;
                    be = de + EPS;
                }
                else
                {
                    /* Normalni vypocet */
                    be = asin(cos(EPS) * sin(de) - sin(EPS) * cos(de) * sin(ra));
                    la = atan((sin(EPS) * sin(de) + cos(EPS) * cos(de) * sin(ra)) / (cos(de) * cos(ra)));
                    /* Do spravneho kvadrantu */
                    if (cos(ra) < 0.0) la += M_PI;
                }
            }

            /* Vypocet ekl. delky slunce a vzdalenosti zeme - slunce */
            static void cmpack_sun(double jd, out double sun_ls, out double sun_rs)
            {
                double t, vt, ls, ms, m5, m4, m2, lm;

                t = jd - 2451545.0;
                vt = 1 + (t / 36525.0);
                ls = (0.779072 + 0.00273790931 * t) * 2 * M_PI;
                ms = (0.993126 + 0.00273777850 * t) * 2 * M_PI;
                m5 = (0.056531 + 0.00023080893 * t) * 2 * M_PI;
                m4 = (0.053856 + 0.00145561327 * t) * 2 * M_PI;
                m2 = (0.140023 + 0.00445036173 * t) * 2 * M_PI;
                lm = (0.606434 + 0.03660110129 * t) * 2 * M_PI;
                ls = ls * 3600 * 180 / M_PI + 6910 * sin(ms)
                      + 72 * sin(2 * ms)
                      - 17 * vt * sin(ms)
                      - 7 * cos(ms - m5)
                      + 6 * sin(lm - ls)
                      + 5 * sin(4 * ms + 8 * m4 + 3 * m5)
                      - 5 * cos(2 * ms - 2 * m2)
                      - 4 * sin(ms - m2)
                      + 4 * cos(4 * ms - 8 * m4 + 3 * m5)
                      + 3 * sin(2 * ms - 2 * m2)
                      - 3 * sin(m5)
                      - 3 * sin(2 * ms - 2 * m5);

                /* Ekl. delka slunce v radianech */
                sun_ls = ls / 3600 / 180 * M_PI;

                /* vzdalenost slunce v AU */
                sun_rs = 1.00014 - 0.01675 * cos(ms) - 0.00014 * cos(2 * ms);
            }

            /**********************   PUBLIC FUNCTIONS   ********************************/

            /* Pocita heliocentrickou korekci */
            static public double cmpack_helcorr(double jd, double ra, double dec)
            {
                double ls, rs, la, be;

                cmpack_sun(jd, out ls, out rs);
                cmpack_rdtolb(ra, dec, out la, out be);
                return cmpack_hcor2(la, be, ls, rs);
            }

            /****************************************************************************/
        }

        private double ra;
        private double dec;

        public HelCor(string ra, string dec)
        {
            int raH = Int32.Parse(ra.Substring(0, 2));
            int raM = Int32.Parse(ra.Substring(2, 2));
            double raS = Double.Parse(ra.Substring(4), System.Globalization.CultureInfo.InvariantCulture);

            int start = (dec.StartsWith("+") || dec.StartsWith("-")) ? 1 : 0;
            int decD = Int32.Parse(dec.Substring(0, start + 2));
            int decM = Int32.Parse(dec.Substring(start + 2, 2));
            double decS = Double.Parse(dec.Substring(start + 4), System.Globalization.CultureInfo.InvariantCulture);

            this.ra = hhmmssToDecimal(raH, raM, raS);
            this.dec = hhmmssToDecimal(decD, decM, decS);
        }

        public HelCor(double ra, double dec)
        {
            this.ra = ra;
            this.dec = dec;
        }

        public HelCor(int raH, int raM, double raS, int decD, int decM, double decS)
            : this(hhmmssToDecimal(raH, raM, raS), hhmmssToDecimal(decD, decM, decS))
        {
        }

        private static double hhmmssToDecimal(double hh, double mm, double ss)
        {
            return ((ss / 60.0) + mm) / 60.0 + hh;
        }

        public double ConvertGJDtoHJD(double jd)
        {
            //var helCor = HelCor.MuniPack.cmpack_helcorr(jd, ra, dec);
            //var helCor = HelCor.keele_ac_uk.get_helcor(jd, ra, dec);
            var helCor = HelCor.physics_sfasu_edu.get_helcor(jd, ra, dec);
            //Console.WriteLine("helcor = " + (helCor * 24 * 60).ToString("F4"));
            return jd + helCor;
        }
    }
}
