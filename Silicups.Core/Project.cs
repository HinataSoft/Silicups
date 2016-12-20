using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

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
            { (int)XMarkTypeEnum.PrimaryMinimum, "Red" },
            { (int)XMarkTypeEnum.SecondaryMinimum, "Red" },
            { (int)XMarkTypeEnum.FlexPoint1, "Green" },
            { (int)XMarkTypeEnum.FlexPoint2, "Green" },
            { (int)XMarkTypeEnum.CalculatedPrimaryMinimum, "Blue" },
            { (int)XMarkTypeEnum.CalculatedSecondaryMinimum, "Blue" },
        };

        public DataPointSeries DataSeries { get; private set; }
        public TimeSeries TimeSeries { get; private set; }
        public CompressedSeries CompressedSeries { get; private set; }
        public PhasedSeries PhasedSeries { get; private set; }

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

        public Project(XmlNode root)
            : this()
        {
            LoadFromXml(root);
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

        public bool CanProvidePeriodData
        {
            get { return P.HasValue && M0.HasValue && (P.Value != 0); }
        }

        public double GetPhased(double time)
        {
            double phased = (time - M0.Value) / P.Value;
            return phased - Math.Floor(phased);
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

        public void AddDataFiles(string baseDirectory, string pattern, string filter)
        {
            this.AbsoluteBasePath = new System.IO.DirectoryInfo(baseDirectory).FullName + System.IO.Path.DirectorySeparatorChar;
            this.FilePattern = pattern;
            this.FileFilter = filter;
            AddDataFiles(PathEx.FindFiles(AbsoluteBasePath, pattern, filter));
        }

        public void LoadFromXml(XmlNode root)
        {
            Id = root.FindAttribute("id").AsString();
            M0 = root.FindAttribute("m0").AsNullableDouble();
            P = root.FindAttribute("p").AsNullableDouble();

            foreach (XmlNode setNode in root.FindNodes("Set"))
            {
                string absolutePath = null;
                string relativePath = null;
                if (setNode.FindOneNode("AbsolutePath") != null)
                {
                    absolutePath = setNode.GetOneNode("AbsolutePath").AsString();
                    relativePath = setNode.GetOneNode("RelativePath").AsString();
                }
                else
                {
                    // TOFIX: Obsolete
                    string pathComposite = setNode.AsString();
                    string[] pathParts = pathComposite.Split('|');
                    if (pathParts.Length == 0)
                    { continue; }
                    absolutePath = pathParts[0];
                    relativePath = (pathParts.Length > 1) ? pathParts[1] : null;
                }
                var set = new DataPointSet(absolutePath, relativePath);
                string path = !String.IsNullOrEmpty(relativePath) && System.IO.File.Exists(relativePath) ? relativePath : absolutePath;
                AppendMagFile(set, path);
                foreach (XmlNode xmarkNode in setNode.FindNodes("XMark"))
                { set.AddXMark(FormatEx.ParseEnumToInt<XMarkTypeEnum>(xmarkNode.FindAttribute("type").AsString("AnyMinimum")), xmarkNode.AsDouble()); }
                DataSeries.AddSet(set);
                set.Metadata.OffsetY = setNode.FindAttribute("offsetY").AsDouble(0);
                set.Metadata.Enabled = setNode.FindAttribute("enabled").AsBoolean(true);
                set.Metadata.Caption = setNode.FindAttribute("caption").AsString(null);
            }
            XmlNode settingsNode = root.FindOneNode("Settings");
            if (settingsNode != null)
            {
                Caption = settingsNode.FindAttribute("caption").AsString(null);
                PAmplitude = settingsNode.FindAttribute("pAmplitude").AsNullableDouble();
                OffsetAmplitude = settingsNode.FindAttribute("offsetAmplitude").AsNullableDouble();
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

        public void SaveToXml(string solutionPath, XmlNode root)
        {
            solutionPath = System.IO.Path.GetDirectoryName(solutionPath) + System.IO.Path.DirectorySeparatorChar;
            root.AppendXmlAttribute("id", Id);
            root.AppendXmlAttribute("m0", M0);
            root.AppendXmlAttribute("p", P);
            foreach (IDataSet set in DataSeries.Series)
            {
                string relativePath = PathEx.MakeRelativePathFromOtherPath(set.Metadata.AbsolutePath, solutionPath);

                XmlNode setNode = root.AppendXmlElement("Set");
                setNode.AppendXmlElement("AbsolutePath", set.Metadata.AbsolutePath);
                setNode.AppendXmlElement("RelativePath", relativePath);
                setNode.AppendXmlAttribute("source", "file");
                setNode.AppendXmlAttribute("offsetY", set.Metadata.OffsetY);
                setNode.AppendXmlAttribute("enabled", set.Metadata.Enabled);
                if (!String.IsNullOrEmpty(set.Metadata.Caption))
                { setNode.AppendXmlAttribute("caption", set.Metadata.Caption); }
                foreach (DataMark m in set.XMarks)
                {
                    XmlNode xmarkNode = setNode.AppendXmlElement("XMark", FormatEx.FormatDouble(m.N));
                    xmarkNode.AppendXmlAttribute("type", ((XMarkTypeEnum)m.Type).ToString());
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
                string relativeBasePath = PathEx.MakeRelativePathFromOtherPath(AbsoluteBasePath, solutionPath);

                XmlNode sourceSettingsNode = root.AppendXmlElement("SourceSettings");
                sourceSettingsNode.InnerText = String.Format("{0}|{1}", AbsoluteBasePath, relativeBasePath);
                sourceSettingsNode.AppendXmlAttribute("source", "file");
                sourceSettingsNode.AppendXmlAttribute("filter", FileFilter);
                sourceSettingsNode.AppendXmlAttribute("pattern", FilePattern);
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
                        var x = FormatEx.ParseDouble(parts[0]);
                        var y = FormatEx.ParseDouble(parts[1]);
                        var yerr = FormatEx.ParseDouble(parts[2]);
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
                            M0 = FormatEx.ParseDouble(parts[0]);
                            P = FormatEx.ParseDouble(parts[2]);
                        }
                    }
                }
                catch
                { }
            }
            if (String.IsNullOrEmpty(set.Metadata.Caption) && !Double.IsInfinity(set.BoundingBox.Left))
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
