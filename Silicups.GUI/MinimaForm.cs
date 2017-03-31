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
    public partial class MinimaForm : Form
    {
        Project CurrentProject;

        public MinimaForm(Project project)
        {
            InitializeComponent();
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
            foreach (IDataSet set in project.DataSeries.Series)
            {
                ((DataPointSet)set).ClearXMarks();
            }

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

                    ((DataPointSet)bestSet).AddXMark((int)type, min);
                }
                catch(Exception e)
                {
                    MessageBox.Show(e.ToString(), "Exception");
                }
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
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
