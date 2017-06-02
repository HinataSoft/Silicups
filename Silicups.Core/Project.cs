﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Text.RegularExpressions;

namespace Silicups.Core
{
    public class Project : IPeriodDataProvider
    {
        public enum XMarkTypeEnum
        {
            AnyMinimum = 0,
            PrimaryMinimum = 1,
            SecondaryMinimum = 2,
            FlexPoint1 = 3,
            FlexPoint2 = 4,
            CalculatedPrimaryMinimum = 10,
            CalculatedSecondaryMinimum = 11,
        }

        public static Dictionary<int, string> XMarkTypeColors = new Dictionary<int, string>()
        {
            { (int)XMarkTypeEnum.AnyMinimum, "Black" },
            { (int)XMarkTypeEnum.PrimaryMinimum, "Blue" },
            { (int)XMarkTypeEnum.SecondaryMinimum, "Red" },
            { (int)XMarkTypeEnum.FlexPoint1, "Green" },
            { (int)XMarkTypeEnum.FlexPoint2, "Green" },
            { (int)XMarkTypeEnum.CalculatedPrimaryMinimum, "Blue" },
            { (int)XMarkTypeEnum.CalculatedSecondaryMinimum, "Red" },
        };

        public DataPointSeries DataSeries { get; private set; }
        public TimeSeries TimeSeries { get; private set; }
        public CompressedSeries CompressedSeries { get; private set; }
        private PhasedSeries PhasedSeries { get; set; }
        public PhasedSeries PhasedSeriesOrDefault { get { return CanProvidePeriodData ? PhasedSeries : null; } }

        public double? M0 { get; internal set; }
        public double? P { get; internal set; }

        public double? OffsetAmplitude { get; set; }
        public double? PAmplitude { get; set; }

        public string Id { get; internal set; }
        public string Caption { get; set; }

        public string AbsoluteBasePath { get; internal set; }
        public string FilePattern { get; internal set; }
        public string FileFilter { get; internal set; }

        public Project()
        {
            this.DataSeries = new DataPointSeries();
            this.TimeSeries = new TimeSeries(this.DataSeries, this);
            this.CompressedSeries = new CompressedSeries(this.DataSeries, this);
            this.PhasedSeries = new PhasedSeries(this.DataSeries, this);
            this.M0 = null;
            this.P = null;
            this.OffsetAmplitude = null;
            this.PAmplitude = null;
            this.Id = Guid.NewGuid().ToString();
            this.Caption = null;
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

        public Project(string workingDir, XmlNode root)
            : this()
        {
            LoadFromXml(workingDir, root, null);
        }

        public override string ToString()
        {
            return Caption ?? Id;
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
            TimeSeries.Refresh();
            CompressedSeries.Refresh();
            PhasedSeries.Refresh();
        }

        public void SortByDate()
        {
            DataSeries.SortByDate();
        }

        public bool CanProvidePeriodData
        {
            get { return P.HasValue && M0.HasValue && (P.Value != 0); }
        }

        public double GetPhased(double time)
        {
            double phased = (time - M0.Value) / P.Value;
            return phased - Math.Floor(phased);
        }

        public double GetFrequency(double timespan)
        {
            return timespan / P.Value;
        }

        public IEnumerable<double> GetFullPhasesBetween(double t1, double t2)
        {
            yield break;
        }

        public DataPointSet AddDataFile(string file)
        {
            string absolutePath = new System.IO.FileInfo(file).FullName;
            string relativePath = System.IO.Path.GetFileName(absolutePath);
            var set = new DataPointSet(absolutePath, relativePath);
            AppendMagFile(set, file);
            DataSeries.AddSet(set);
            return set;
        }

        public void RemoveDataFile(string file)
        {
            DataPointSet setToRemove = null;
            foreach (DataPointSet set in DataSeries.Series)
            {
                if (set.Metadata.AbsolutePath == file)
                { setToRemove = set; break; }
            }
            if (setToRemove != null)
            { DataSeries.RemoveSet(setToRemove); }
        }

        public void RefreshDataFile(string file)
        {
            foreach (DataPointSet set in DataSeries.Series)
            {
                if (set.Metadata.AbsolutePath == file)
                {
                    set.Clear();
                    AppendMagFile(set, file);
                }
            }
        }

        public void AddDataFiles(IEnumerable<string> files)
        {
            var existingFiles = MakePathHashSet();
            foreach (string file in files)
            {
                string absolutePath = new System.IO.FileInfo(file).FullName;
                string relativePath = System.IO.Path.GetFileName(absolutePath);
                if (existingFiles.Contains(absolutePath) || existingFiles.Contains(relativePath))
                { continue; }
                AddDataFile(file);
            }
            Refresh();
        }

        public void SetFileSource(string baseDirectory, string pattern, string filter)
        {
            this.AbsoluteBasePath = new System.IO.DirectoryInfo(baseDirectory).FullName + System.IO.Path.DirectorySeparatorChar;
            this.FilePattern = pattern;
            this.FileFilter = filter;
        }

        public void LoadFromXml(string workingDir, XmlNode root, List<Exception> exceptions)
        {
            workingDir = PathEx.GetFullAbsolutePath(workingDir);

            Id = root.FindAttribute("id").AsString();
            M0 = root.FindAttribute("m0").AsNullableDouble();
            P = root.FindAttribute("p").AsNullableDouble();

            foreach (XmlNode setNode in root.FindNodes("Set"))
            {
                try
                {
                    string absolutePath = setNode.GetOneNode("AbsolutePath").AsString();
                    string relativePath = setNode.GetOneNode("RelativePath").AsString();
                    absolutePath = PathEx.GetBestAbsolutePath(workingDir, absolutePath, relativePath);

                    var set = new DataPointSet(absolutePath, relativePath);
                    foreach (XmlNode xmarkNode in setNode.FindNodes("XMark"))
                    {
                        set.AddXMark(
                            FormatEx.ParseEnumToInt<XMarkTypeEnum>(xmarkNode.FindAttribute("type").AsString("AnyMinimum")),
                            xmarkNode.AsDouble(),
                            xmarkNode.FindAttribute("error").AsDouble(0)
                        );
                    }
                    set.Metadata.OffsetY = setNode.FindAttribute("offsetY").AsDouble(0);
                    set.Metadata.Enabled = setNode.FindAttribute("enabled").AsBoolean(true);
                    set.Metadata.Caption = setNode.FindAttribute("caption").AsString(null);
                    set.Metadata.Filter = setNode.FindAttribute("filter").AsString(null);
                    AppendMagFile(set, absolutePath);
                    DataSeries.AddSet(set);
                }
                catch (Exception e)
                {
                    if (exceptions != null)
                    { exceptions.Add(e); }
                }
            }

            {
                XmlNode settingsNode = root.FindOneNode("Settings");
                if (settingsNode != null)
                {
                    Caption = settingsNode.FindAttribute("caption").AsString(null);
                    PAmplitude = settingsNode.FindAttribute("pAmplitude").AsNullableDouble();
                    OffsetAmplitude = settingsNode.FindAttribute("offsetAmplitude").AsNullableDouble();
                }
            }

            {
                XmlNode sourceSettingsNode = root.FindOneNode("SourceSettings");
                if (sourceSettingsNode != null)
                {
                    AbsoluteBasePath = sourceSettingsNode.FindOneNode("AbsolutePath").AsString("");
                    string relativeBasePath = sourceSettingsNode.FindOneNode("RelativePath").AsString("");
                    // fixing absolute path using relative path if absolute does not exist
                    AbsoluteBasePath = PathEx.GetBestAbsolutePath(workingDir, AbsoluteBasePath, relativeBasePath);

                    FileFilter = sourceSettingsNode.FindOneNode("Filter").AsString("");
                    FilePattern = sourceSettingsNode.FindOneNode("Pattern").AsString("");

                    if (String.IsNullOrWhiteSpace(FileFilter))
                    { FileFilter = sourceSettingsNode.FindAttribute("filter").AsString(""); }
                    if (String.IsNullOrWhiteSpace(FilePattern))
                    { FilePattern = sourceSettingsNode.FindAttribute("pattern").AsString(""); }
                }
            }

            Refresh();
        }

        private HashSet<string> MakePathHashSet()
        {
            var existingFiles = new HashSet<string>();
            foreach (IDataSet set in DataSeries.Series)
            {
                existingFiles.Add(set.Metadata.AbsolutePath);
                existingFiles.Add(set.Metadata.RelativePath);
            }
            return existingFiles;
        }

        public void SaveToXml(string workingDir, XmlNode root)
        {
            workingDir = PathEx.GetFullAbsolutePath(workingDir);
            root.AppendXmlAttribute("id", Id);
            root.AppendXmlAttribute("m0", M0);
            root.AppendXmlAttribute("p", P);
            foreach (IDataSet set in DataSeries.Series)
            {
                string relativePath;
                try
                { relativePath = PathEx.MakeRelativePathFromOtherPath(set.Metadata.AbsolutePath, workingDir); }
                catch
                { relativePath = ""; }

                XmlNode setNode = root.AppendXmlElement("Set");
                setNode.AppendXmlElement("AbsolutePath", set.Metadata.AbsolutePath);
                setNode.AppendXmlElement("RelativePath", relativePath);
                setNode.AppendXmlAttribute("source", "file");
                setNode.AppendXmlAttribute("offsetY", set.Metadata.OffsetY);
                if (!String.IsNullOrEmpty(set.Metadata.Filter))
                { setNode.AppendXmlAttribute("filter", set.Metadata.Filter); }
                setNode.AppendXmlAttribute("enabled", set.Metadata.Enabled);
                if (!String.IsNullOrEmpty(set.Metadata.Caption))
                { setNode.AppendXmlAttribute("caption", set.Metadata.Caption); }
                foreach (DataMark m in set.XMarks)
                {
                    XmlNode xmarkNode = setNode.AppendXmlElement("XMark", FormatEx.FormatDouble(m.N));
                    xmarkNode.AppendXmlAttribute("type", ((XMarkTypeEnum)m.Type).ToString());
                    xmarkNode.AppendXmlAttribute("error", FormatEx.FormatDouble(m.Nerr));
                }
            }

            {
                XmlNode settingsNode = root.AppendXmlElement("Settings");
                if (!String.IsNullOrEmpty(Caption))
                { settingsNode.AppendXmlAttribute("caption", Caption); }
                settingsNode.AppendXmlAttribute("pAmplitude", PAmplitude);
                settingsNode.AppendXmlAttribute("offsetAmplitude", OffsetAmplitude);
            }

            if (!String.IsNullOrEmpty(AbsoluteBasePath))
            {
                string relativePath;
                try
                { relativePath = PathEx.MakeRelativePathFromOtherPath(AbsoluteBasePath, workingDir); }
                catch
                { relativePath = ""; }

                XmlNode sourceSettingsNode = root.AppendXmlElement("SourceSettings");
                sourceSettingsNode.AppendXmlAttribute("source", "file");
                sourceSettingsNode.AppendXmlElement("AbsolutePath", AbsoluteBasePath);
                sourceSettingsNode.AppendXmlElement("RelativePath", relativePath);
                sourceSettingsNode.AppendXmlElement("Filter", FileFilter);
                sourceSettingsNode.AppendXmlElement("Pattern", FilePattern);
            }
        }

        private const string MagFilePhasedTag = "Phased with elements ";
        private readonly static Regex FilterRegex = new Regex("Filter: ([_a-zA-Z][_a-zA-Z0-9]*)");

        private void AppendMagFile(DataPointSet set, string filename)
        {
            foreach (string s in System.IO.File.ReadAllLines(filename))
            {
                try
                {
                    if (s.StartsWith("24"))
                    {
                        string[] parts = s.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                        var x = FormatEx.ParseDouble(parts[0]);
                        var y = FormatEx.ParseDouble(parts[1]);
                        var yerr = FormatEx.ParseDouble(parts[2]);
                        if (y > 50)
                        { continue; }

                        set.Add(x, y, yerr);
                    }
                    else
                    {
                        if (!P.HasValue && !M0.HasValue)
                        {
                            int i = s.IndexOf(MagFilePhasedTag);
                            if (i > 0)
                            {
                                string[] parts = s.Substring(i + MagFilePhasedTag.Length).Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                                M0 = FormatEx.ParseDouble(parts[0]);
                                P = FormatEx.ParseDouble(parts[2]);
                            }
                        }
                        var match = FilterRegex.Match(s);
                        if (match.Success)
                        { set.Metadata.Filter = match.Groups[1].Value; }
                    }
                }
                catch
                { }
            }
            if (String.IsNullOrEmpty(set.Metadata.Caption) && !set.BoundingBox.IsEmpty)
            {
                DateTime date = JD.JDToDateTime(set.BoundingBox.Left);
                set.Metadata.Caption = date.ToString("yyyy'-'MM'-'dd");
            }
        }

        public bool RemoveSet(IDataSetMetadata metadata)
        {
            IDataSet setToRemove = null;
            foreach(IDataSet set in DataSeries.Series)
            {
                if (set.Metadata == metadata)
                { setToRemove = set; break; }
            }
            if (setToRemove != null)
            {
                DataSeries.RemoveSet((DataPointSet)setToRemove);
                Refresh();
                return true;
            }
            return false;
        }
    }

    public static class ProjectExtensions
    {
        public static IEnumerable<IDataSetMetadata> GetMetadata(this Project project)
        {
            foreach (IDataSet set in project.DataSeries.Series)
            { yield return set.Metadata; }
        }

        public static void SetM0AndPString(this Project project, string m0, string p)
        {
            try
            {
                project.M0 = FormatEx.ParseDouble(m0);
                project.P = FormatEx.ParseDouble(p);
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
            project.P = p;
            project.RefreshM0AndP();
        }
    }
}
