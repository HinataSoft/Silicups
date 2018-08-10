using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Silicups.Core;

namespace Silicups.GUI
{
    public partial class MinimaForm : FormEx
    {
        private static readonly string RegistryPath = Util.RegistryHelper.RegistryPath + @"\MinimaForm";

        Project CurrentProject;

        public MinimaForm(Project project)
        {
            InitializeComponent();
            InitializeFormEx(RegistryPath);
            this.CurrentProject = project;
            LoadMinimaFromProject(CurrentProject);
        }

        public void LoadMinimaFromProject(Project project)
        {
            var sb = new StringBuilder();
            foreach (IDataSet set in project.DataSeries.Series)
            {
                foreach (DataMark mark in set.XMarks)
                {
                    sb.Append(FormatEx.FormatDouble(mark.N));
                    if (mark.Nerr > 0)
                    { sb.Append("+-").Append(FormatEx.FormatDouble(mark.Nerr)); }
                    switch (mark.Type)
                    {
                        case (int)Project.XMarkTypeEnum.PrimaryMinimum: sb.Append(" p"); break;
                        case (int)Project.XMarkTypeEnum.SecondaryMinimum: sb.Append(" s"); break;
                    }
                    sb.AppendLine();
                }
            }
            textBoxMinima.Text = sb.ToString();
        }

        public void SetMinimaToProject()
        {
            SetMinimaToProject(CurrentProject);
        }

        public void SetMinimaToProject(Project project)
        {
            var newMinimaList = new List<Tuple<DataPointSet, DataMark>>();
            var exceptionSB = new StringBuilder();

            string[] lines = textBoxMinima.Text.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                try
                {
                    string minString = line.Trim();
                    Project.XMarkTypeEnum type = Project.XMarkTypeEnum.AnyMinimum;
                    if (minString.EndsWith("p"))
                    {
                        type = Project.XMarkTypeEnum.PrimaryMinimum;
                        minString = minString.Substring(0, minString.Length - 1);
                    }
                    if (minString.EndsWith("s"))
                    {
                        type = Project.XMarkTypeEnum.SecondaryMinimum;
                        minString = minString.Substring(0, minString.Length - 1);
                    }
                    if (minString.EndsWith("(p)"))
                    {
                        type = Project.XMarkTypeEnum.PrimaryMinimum;
                        minString = minString.Substring(0, minString.Length - 3);
                    }
                    if (minString.EndsWith("(s)"))
                    {
                        type = Project.XMarkTypeEnum.SecondaryMinimum;
                        minString = minString.Substring(0, minString.Length - 3);
                    }
                    if (minString.StartsWith("TminHJD="))
                    {
                        minString = minString.Substring("TminHJD=".Length);
                    }

                    double minErr = 0;
                    int minErrPos = minString.IndexOf("+-");
                    if (minErrPos > 0)
                    {
                        string minErrString = minString.Substring(minErrPos + 2);
                        minErr = FormatEx.ParseDouble(minErrString);
                        minString = minString.Substring(0, minErrPos);
                    }

                    double min = FormatEx.ParseDouble(minString.Trim());

                    IDataSet bestSet = null;
                    double bestDistance = Double.PositiveInfinity;

                    foreach (IDataSet set in project.DataSeries.Series)
                    {
                        {
                            double distance = Math.Abs(min - set.BoundingBox.Left);
                            if (distance < bestDistance)
                            {
                                bestSet = set;
                                bestDistance = distance;
                            }
                        }
                        {
                            double distance = Math.Abs(min - set.BoundingBox.Right);
                            if (distance < bestDistance)
                            {
                                bestSet = set;
                                bestDistance = distance;
                            }
                        }
                    }

                    newMinimaList.Add(new Tuple<DataPointSet, DataMark>((DataPointSet)bestSet, new DataMark((int)type, min, minErr)));
                }
                catch (Exception e)
                {
                    exceptionSB.Append(line).Append(": ").AppendLine(e.Message);
                }
            }

            if (exceptionSB.Length > 0)
            {
                throw new Exception(exceptionSB.ToString());
            }

            foreach (IDataSet set in project.DataSeries.Series)
            { ((DataPointSet)set).ClearXMarks(); }

            foreach (Tuple<DataPointSet, DataMark> tuple in newMinimaList)
            { tuple.Item1.AddXMark(tuple.Item2); }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            try
            {
                SetMinimaToProject();
                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception when processing minima list -- try to correct the list or cancel the dialog for no changes");
            }
        }

        private void buttonEstimator_Click(object sender, EventArgs e)
        {
            SetMinimaToProject(CurrentProject);
            var sb = new StringBuilder();
            foreach (IDataSet set in CurrentProject.DataSeries.Series)
            {
                foreach (DataMark mark in set.XMarks)
                {
                    sb.Append("TminHJD=");
                    sb.Append(FormatEx.FormatDouble(mark.N));
                    switch (mark.Type)
                    {
                        case (int)Project.XMarkTypeEnum.PrimaryMinimum: sb.Append(" (p)"); break;
                        case (int)Project.XMarkTypeEnum.SecondaryMinimum: sb.Append(" (s)"); break;
                    }
                    sb.AppendLine();
                }
            }
            textBoxMinima.Text = sb.ToString();
        }
    }
}
