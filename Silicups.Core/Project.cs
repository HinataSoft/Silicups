using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Silicups.Core
{
    public class Project
    {
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
        public string FilePattern { get; set; }
        public string FileFilter { get; set; }

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
            var existingFiles = MakePathHashSet();
            foreach (string file in files)
            {
                string absolutePath = new System.IO.FileInfo(file).FullName;
                string relativePath = PathEx.MakeRelativePath(absolutePath, System.IO.Directory.GetCurrentDirectory());
                if (existingFiles.Contains(absolutePath) || existingFiles.Contains(relativePath))
                { continue; }
                var set = new DataPointSet(absolutePath, relativePath);
                AppendMagFile(set, file);
                DataSeries.AddSet(set);
            }
            Refresh();
        }

        public void LoadFromXml(XmlNode root)
        {
            Id = root.FindAttribute("id").AsString();
            M0 = root.FindAttribute("m0").AsNullableDouble();
            P = root.FindAttribute("p").AsNullableDouble();

            foreach (XmlNode setNode in root.FindNodes("Set"))
            {
                string absolutePath = setNode.FindOneNode("AbsolutePath").AsString();
                string relativePath = setNode.FindOneNode("RelativePath").AsString();
                var set = new DataPointSet(absolutePath, relativePath);
                string path = System.IO.File.Exists(relativePath) ? relativePath : absolutePath;
                AppendMagFile(set, path);
                DataSeries.AddSet(set);
                set.Metadata.OffsetY = setNode.FindAttribute("offsetY").AsDouble(0);
                set.Metadata.Enabled = setNode.FindAttribute("enabled").AsBoolean(true);
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

        public void SaveToXml(XmlNode root)
        {
            root.AppendXmlAttribute("id", Id);
            root.AppendXmlAttribute("m0", M0);
            root.AppendXmlAttribute("p", P);
            foreach (IDataSet set in DataSeries.Series)
            {
                XmlNode setNode = root.AppendXmlElement("Set");
                XmlNode absolutePathNode = root.AppendXmlElement("AbsolutePath");
                XmlNode relativePathNode = root.AppendXmlElement("RelativePath");
                absolutePathNode.InnerText = set.Metadata.AbsolutePath;
                relativePathNode.InnerText = set.Metadata.RelativePath;
                setNode.AppendXmlAttribute("offsetY", set.Metadata.OffsetY);
                setNode.AppendXmlAttribute("enabled", set.Metadata.Enabled);
            }
            XmlNode settingsNode = root.AppendXmlElement("Settings");
            if (!String.IsNullOrEmpty(Caption))
            { settingsNode.AppendXmlAttribute("caption", Caption); }
            settingsNode.AppendXmlAttribute("pAmplitude", PAmplitude);
            settingsNode.AppendXmlAttribute("offsetAmplitude", OffsetAmplitude);
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

    internal static class PathEx
    {
        /// <summary>
        /// Creates a relative path from one file or folder to another.
        /// http://stackoverflow.com/a/340454
        /// </summary>
        /// <param name="fromPath">Contains the directory that defines the start of the relative path.</param>
        /// <param name="toPath">Contains the path that defines the endpoint of the relative path.</param>
        /// <returns>The relative path from the start directory to the end path or <c>toPath</c> if the paths are not related.</returns>
        public static String MakeRelativePath(String fromPath, String toPath)
        {
            if (String.IsNullOrEmpty(fromPath)) throw new ArgumentNullException("fromPath");
            if (String.IsNullOrEmpty(toPath)) throw new ArgumentNullException("toPath");

            Uri fromUri = new Uri(fromPath);
            Uri toUri = new Uri(toPath);

            if (fromUri.Scheme != toUri.Scheme) { return toPath; } // path can't be made relative.

            Uri relativeUri = fromUri.MakeRelativeUri(toUri);
            String relativePath = Uri.UnescapeDataString(relativeUri.ToString());

            if (toUri.Scheme.Equals("file", StringComparison.InvariantCultureIgnoreCase))
            {
                relativePath = relativePath.Replace(System.IO.Path.AltDirectorySeparatorChar, System.IO.Path.DirectorySeparatorChar);
            }

            return relativePath;
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
            project.P = p;
            project.RefreshM0AndP();
        }
    }
}
