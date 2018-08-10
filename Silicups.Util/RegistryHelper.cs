using System;
using System.Collections.Generic;
using System.Text;

namespace Silicups.Util
{
    public static class RegistryHelper
    {
        public static readonly string RegistryPath = @"SOFTWARE\HinataSoft\Silicups";

        public class GetRegistryAction
        {
            public string Key { get; private set; }
            public Action<object> Action { get; private set; }

            public GetRegistryAction(string key, Action<object> action)
            {
                this.Key = key;
                this.Action = action;
            }
        }

        public class GetRegistryStringAction : GetRegistryAction
        {
            public Action<string> StringAction { get; private set; }

            public GetRegistryStringAction(string key, Action<string> stringAction)
                : base(key, (o) => stringAction(o.ToString()))
            {
                this.StringAction = stringAction;
            }
        }

        public static void TryGetFromRegistry(string path, params GetRegistryAction[] actions)
        {
            try
            {
                var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(path);
                if (key != null)
                {
                    foreach (GetRegistryAction action in actions)
                    {
#if DEBUG
                        System.Diagnostics.Trace.WriteLine(String.Format("Getting {0}: {1}", action.Key, key.GetValue(action.Key) ));
#endif
                        action.Action(key.GetValue(action.Key));
                    }
                }
            }
            catch
            { }
        }

        public class SetRegistryAction
        {
            public string Key { get; private set; }
            public object Value { get; private set; }

            public SetRegistryAction(string key, object value)
            {
                this.Key = key;
                this.Value = value;
            }
        }

        public static void TrySetToRegistry(string path, params SetRegistryAction[] actions)
        {
            try
            {
                var key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(path);
                if (key != null)
                {
                    foreach (SetRegistryAction action in actions)
                    {
#if DEBUG
                        System.Diagnostics.Trace.WriteLine(String.Format("Setting {0}: {1}", action.Key, action.Value));
#endif
                        key.SetValue(action.Key, action.Value);
                    }
                }
            }
            catch
            { }
        }
    }
}
