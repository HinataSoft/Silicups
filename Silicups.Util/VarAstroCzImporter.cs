using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using Silicups.Core;

namespace Silicups.Util
{
    public static class VarAstroCzImporter
    {
        private static Regex nameRegex = new Regex(@"<td><b class='big'>(.*?)</b></td>", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
        private static Regex m0Regex = new Regex(@"<td>M0 =.*value='([0-9.]*)'>", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
        private static Regex pRegex = new Regex(@"<td>PER =.*value='([0-9.]*)'>", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
        private static Regex rowRegex = new Regex(@"<tr\svalign='top'><td>[0-9].*?tr>", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Singleline);
        private static Regex fileRegex = new Regex("'obslog/([^']*?.txt)'", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
        private static Regex minimaRegex = new Regex("Tmin([GH])JD[ ]*=[ ]*([0-9]+[,.][0-9]+)", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
        private static Regex offsetRegex = new Regex("name='shift_[0-9]*' value='([0-9.-]*)'", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

        public static Project Import(Action<string> log, string url, string directory)
        {
            var webClient = new System.Net.WebClient();
            Uri rootUri = new Uri(url);

            // URL download
            log("Downloading HTTP(S) document ...");
            string source = webClient.DownloadString(url);
            log("... done");

            Project project = new Project();
            HelCor helCor = null;

            // name
            string name = null;
            foreach (Match match in nameRegex.Matches(source))
            { name = match.Groups[1].Value; log("Name: " + name); }

            // M0
            string m0 = null;
            foreach (Match match in m0Regex.Matches(source))
            { m0 = match.Groups[1].Value; log("M0: " + m0); }

            // PER
            string per = null;
            foreach (Match match in pRegex.Matches(source))
            { per = match.Groups[1].Value; log("PER: " + per); }

            // sets
            foreach (Match setMatch in rowRegex.Matches(source))
            {
                try
                {
                    var setSource = setMatch.Value;
                    //log("SET source: " + setSource);

                    string file = null;
                    foreach (Match match in fileRegex.Matches(setSource))
                    { file = match.Groups[1].Value; log("Source file: " + file); }

                    if (String.IsNullOrEmpty(file))
                    { continue; }

                    Uri fileUri = new Uri(rootUri, "obslog/" + file);
                    string filename = System.IO.Path.Combine(directory, file);
                    log("Target filename: " + filename);

                    log("Downloading " + fileUri.ToString());
                    webClient.DownloadFile(fileUri, filename);

                    string offsetString = null;
                    foreach (Match match in offsetRegex.Matches(setSource))
                    { offsetString = match.Groups[1].Value; log("Offset: " + offsetString); }

                    DataPointSet set = project.AddDataFile(filename);
                    double offset;
                    if (!String.IsNullOrEmpty(offsetString) && Double.TryParse(offsetString, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out offset))
                    { set.Metadata.OffsetY = -offset; }

                    // minima
                    {
                        var matches = minimaRegex.Matches(setSource);
                        for (int i = 0; i < matches.Count; i++)
                        {
                            var match = matches[i];

                            bool gjd = (match.Groups[1].Value == "G");

                            double time = Double.Parse(match.Groups[2].Value, System.Globalization.CultureInfo.InvariantCulture);

                            Project.XMarkTypeEnum pointType = Project.XMarkTypeEnum.AnyMinimum;

                            log("Got minimum " + match.Value);

                            if (gjd && (helCor == null))
                            {
                                log(match.Value + " ignored (no RA/DEC given)");
                                continue;
                            }

                            if (gjd && (helCor != null))
                            { time = helCor.ConvertGJDtoHJD(time); }

                            bool timeFound = false;
                            double diff = 0;
                            foreach (IDataSet otherSet in project.DataSeries.Series)
                            {
                                foreach (DataMark m in otherSet.XMarks)
                                {
                                    diff = Math.Abs(m.N - time);
                                    if (diff < 0.01)
                                    { timeFound = true; break; }
                                }
                            }

                            if (timeFound)
                            {
                                log(match.Value + " ignored (got it already, diff = " + ((int)(diff * 86400)) + " s)");
                                continue;
                            }

                            set.AddXMark((int)pointType, time);
                        }
                    }
                }
                catch (Exception e)
                {
                    log("Set exception: " + e.ToString());
                }

            }

            if (!String.IsNullOrEmpty(name))
            { project.Caption = name; }
            project.Refresh();
            project.SetM0AndPString(m0, per);
            return project;
        }
    }
}
