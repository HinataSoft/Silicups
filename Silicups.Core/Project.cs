using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Silicups.Core
{
    public class Project
    {
        internal DataPointSeries DataSeries { get; private set; }
        internal TimeSeries TimeSeries { get; private set; }
        internal CompressedSeries CompressedSeries { get; private set; }
        internal PhasedSeries PhasedSeries { get; private set; }

        public double? M0 { get; internal set; }
        public double? P { get; internal set; }

        public double? OffsetAmplitude { get; internal set; }
        public double? PAmplitude { get; internal set; }

        public Project()
        {
            this.DataSeries = new DataPointSeries();
            this.TimeSeries = new TimeSeries(this.DataSeries);
            this.CompressedSeries = new CompressedSeries(this.DataSeries);
            this.PhasedSeries = new PhasedSeries(this.DataSeries);
            this.M0 = null;
            this.P = null;
            this.OffsetAmplitude = null;
            this.PAmplitude = null;
        }

        public Project(string file)
            : this(new string[] { file })
        {
        }

        public Project(IEnumerable<string> files)
            : this()
        {
            AddDataFiles(files);
        }

        public Project(XmlNode root)
            : this()
        {
            AddFromXml(root);
        }

        public void Refresh()
        {
            TimeSeries.Refresh();
            CompressedSeries.Refresh();

            if (P.HasValue && !PAmplitude.HasValue)
            { PAmplitude = MathEx.GetLower125Base(P.Value); }

            if(!Double.IsInfinity(DataSeries.BoundingBox.Height) && !OffsetAmplitude.HasValue)
            { OffsetAmplitude = MathEx.GetLower125Base(DataSeries.BoundingBox.Height); }

            RefreshM0AndP();
        }

        internal void RefreshM0AndP()
        {
            if (M0.HasValue && P.HasValue)
            {
                PhasedSeries.M0 = M0.Value;
                PhasedSeries.P = P.Value;
            }
            else
            {
                PhasedSeries.M0 = 0;
                PhasedSeries.P = 0;
            }
            PhasedSeries.Refresh();
        }

        public void AddDataFiles(IEnumerable<string> files)
        {
            var existingFiles = new HashSet<string>();
            foreach (IDataSet set in DataSeries.Series)
            { existingFiles.Add(set.Metadata.Path); }
            foreach (string file in files)
            {
                if (existingFiles.Contains(file))
                { continue; }
                var set = new DataPointSet(file);
                AppendMagFile(set, file);
                DataSeries.AddSet(set);
            }
            Refresh();
        }

        public void AddFromXml(XmlNode root)
        {
            var existingFiles = new HashSet<string>();
            foreach (IDataSet set in DataSeries.Series)
            { existingFiles.Add(set.Metadata.Path); }
            foreach (XmlNode setNode in root.FindNodes("Set"))
            {
                string file = setNode.AsString();
                if (existingFiles.Contains(file))
                { continue; }
                var set = new DataPointSet(file);
                AppendMagFile(set, file);
                DataSeries.AddSet(set);
                set.Metadata.OffsetY = setNode.FindAttribute("offsetY").AsDouble(0);
                set.Metadata.Enabled = setNode.FindAttribute("enabled").AsBoolean(true);
            }
            M0 = root.FindAttribute("m0").AsNullableDouble();
            P = root.FindAttribute("p").AsNullableDouble();
            PAmplitude = root.FindAttribute("pAmplitude").AsNullableDouble();
            OffsetAmplitude = root.FindAttribute("offsetAmplitude").AsNullableDouble();
            Refresh();
        }

        public void SaveToXml(XmlNode root)
        {
            root.AppendXmlAttribute("m0", M0.Value);
            root.AppendXmlAttribute("p", P.Value);
            root.AppendXmlAttribute("pAmplitude", PAmplitude.Value);
            root.AppendXmlAttribute("offsetAmplitude", OffsetAmplitude.Value);
            foreach (IDataSet set in DataSeries.Series)
            {
                XmlNode setNode = root.AppendXmlElement("Set");
                setNode.InnerText = set.Metadata.Path;
                setNode.AppendXmlAttribute("offsetY", set.Metadata.OffsetY);
                setNode.AppendXmlAttribute("enabled", set.Metadata.Enabled);
            }
        }

        private readonly static string MagFilePhasedTag = "Phased with elements ";
        private void AppendMagFile(DataPointSet set, string filename)
        {
            foreach (string s in System.IO.File.ReadAllLines(filename))
            {
                try
                {
                    if (s.StartsWith("24"))
                    {
                        string[] parts = s.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        var x = MathEx.ParseDouble(parts[0]);
                        var y = MathEx.ParseDouble(parts[1]);
                        var yerr = MathEx.ParseDouble(parts[2]);
                        if (y > 50)
                        { continue; }

                        set.Add(x, y, yerr);
                    }
                    else
                    {
                        int i = s.IndexOf(MagFilePhasedTag);
                        if (i > 0)
                        {
                            string[] parts = s.Substring(i + MagFilePhasedTag.Length).Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            M0 = MathEx.ParseDouble(parts[0]);
                            P = MathEx.ParseDouble(parts[2]);
                        }
                    }
                }
                catch
                { }
            }
        }
    }

    public static class ProjectExtensions
    {
        public static IDataSeries GetTimeSeries(this Project project)
        {
            return (project != null) && !project.TimeSeries.IsEmpty ? project.TimeSeries : null;
        }

        public static IDataSeries GetCompressedSeries(this Project project)
        {
            return (project != null) && !project.CompressedSeries.IsEmpty ? project.CompressedSeries : null;
        }

        public static IDataSeries GetPhasedSeries(this Project project)
        {
            return (project != null) && !project.PhasedSeries.IsEmpty ? project.PhasedSeries : null;
        }

        public static IEnumerable<IDataSetMetadata> GetMetadata(this Project project)
        {
            if (project == null)
            { yield break; }
            foreach (IDataSet set in project.DataSeries.Series)
            { yield return set.Metadata; }
        }

        public static string GetM0String(this Project project)
        {
            return (project != null) && (project.M0 != null) ? MathEx.FormatDouble(project.M0.Value) : null;
        }

        public static string GetPString(this Project project)
        {
            return (project != null) && (project.P != null) ? MathEx.FormatDouble(project.P.Value) : null;
        }

        public static void SetM0AndPString(this Project project, string m0, string p)
        {
            if (project == null)
            { return; }
            try
            {
                project.M0 = MathEx.ParseDouble(m0);
                project.P = MathEx.ParseDouble(p);
            }
            catch
            {
                project.M0 = null;
                project.P = null;
            }
            project.RefreshM0AndP();
        }

        public static void SetP(this Project project, double? p)
        {
            if (project == null)
            { return; }
            project.P = p;
            project.RefreshM0AndP();
        }
    }
}
