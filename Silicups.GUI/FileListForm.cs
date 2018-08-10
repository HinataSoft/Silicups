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
    public partial class FileListForm : FormEx
    {
        public enum ActionEnum
        {
            Add,
            Remove,
            Keep,
        }

        public class FileAction
        {
            public ActionEnum Action;
            public string Path;

            public override string ToString()
            {
                return String.Format("{0} {1}", Action, Path);
            }

            public static int Compare(FileAction a, FileAction b)
            {
                if (a.Action == b.Action)
                { return a.Path.CompareTo(b.Path); }
                return a.Action.CompareTo(b.Action);
            }
        }

        private static readonly string RegistryPath = Util.RegistryHelper.RegistryPath + @"\FileListForm";

        public FileListForm(List<string> paths, Project project)
        {
            InitializeComponent();
            InitializeFormEx(RegistryPath);

            List<FileAction> actions = MakeActions(paths, project);
            foreach (FileAction action in actions)
            {
                listBox.Items.Add(action, true);
            }
        }

        public List<FileAction> GetChangeActions()
        {
            var result = new List<FileAction>();
            for(int i = 0; i < listBox.Items.Count; i++)
            {
                FileAction action = (FileAction)listBox.Items[i];
                ActionEnum actionType = action.Action;
                if (!listBox.GetItemChecked(i))
                {
                    if (actionType == ActionEnum.Keep)
                    { actionType = ActionEnum.Remove; }
                    else
                    { continue; }
                }
                result.Add(new FileAction { Action = actionType, Path = action.Path });
            }
            return result;
        }

        private static List<FileAction> MakeActions(List<string> paths, Project project)
        {
            var result = new List<FileAction>();
            var existingPaths = new HashSet<string>();

            foreach (IDataSet set in project.DataSeries.Series)
            { existingPaths.Add(set.Metadata.AbsolutePath); }

            foreach (string path in paths)
            {
                ActionEnum action = existingPaths.Contains(path)
                    ? ActionEnum.Keep
                    : ActionEnum.Add;
                result.Add(new FileAction { Action = action, Path = path });
            }

            var pathsCovered = new HashSet<string>(paths);
            foreach (string path in existingPaths)
            {
                if (!pathsCovered.Contains(path))
                { result.Add(new FileAction { Action = ActionEnum.Remove, Path = path }); }
            }

            result.Sort(FileAction.Compare);
            return result;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }
    }
}
