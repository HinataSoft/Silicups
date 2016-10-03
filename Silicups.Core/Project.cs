using System;
using System.Collections.Generic;
using System.Text;

namespace Silicups.Core
{
    public class Project
    {
        internal DataPointSeries DataSeries { get; private set; }
        internal TimeSeries TimeSeries { get; private set; }
        internal CompressedSeries CompressedSeries { get; private set; }
        internal PhasedSeries PhasedSeries { get; private set; }

        internal double? M0 { get; set; }
        internal double? P { get; set; }

        internal Dictionary<int, DataPointSet> SetDict { get; private set; }

        private Project()
        {
            this.DataSeries = new DataPointSeries();
            this.TimeSeries = new TimeSeries(this.DataSeries);
            this.CompressedSeries = new CompressedSeries(this.DataSeries);
            this.PhasedSeries = new PhasedSeries(this.DataSeries);
            this.M0 = null;
            this.P = null;
            this.SetDict = new Dictionary<int, DataPointSet>();
        }

        private void FinishInit()
        {
            Refresh();
        }

        public void Refresh()
        {
            TimeSeries.Refresh();
            CompressedSeries.Refresh();
            RefreshM0AndP();
        }

        internal void RefreshM0AndP()
        {
            if ((M0 != null) && (P != null))
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

        public static Project CreateFromDataFile(string file)
        {
            return CreateFromDataFiles(new string[] { file });
        }

        public static Project CreateFromDataFiles(IEnumerable<string> files)
        {
            var project = new Project();
            int id = 0;
            foreach (string file in files)
            {
                var set = new DataPointSet(id, file);
                AppendMagFile(project, set, file);
                project.DataSeries.AddSet(set);
                project.SetDict.Add(id, set);
                id++;
            }
            project.FinishInit();
            return project;
        }

        private readonly static string MagFilePhasedTag = "Phased with elements ";
        private static void AppendMagFile(Project project, DataPointSet set, string filename)
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
                            project.M0 = MathEx.ParseDouble(parts[0]);
                            project.P = MathEx.ParseDouble(parts[2]);
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
            foreach (DataPointSet set in project.SetDict.Values)
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
    }
}
