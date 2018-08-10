using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Silicups.Util;

namespace Silicups.GUI
{
    public class FormEx : Form
    {
        protected void InitializeFormEx(string registryPath)
        {
            this.Load += (sender, e) => {

                int? x = null, y = null, w = null, h = null;

                try
                {
                    RegistryHelper.TryGetFromRegistry(registryPath,
                        new RegistryHelper.GetRegistryStringAction("FormEx_Position_X", (s) => { x = Int32.Parse(s); }),
                        new RegistryHelper.GetRegistryStringAction("FormEx_Position_Y", (s) => { y = Int32.Parse(s); }),
                        new RegistryHelper.GetRegistryStringAction("FormEx_Position_W", (s) => { w = Int32.Parse(s); }),
                        new RegistryHelper.GetRegistryStringAction("FormEx_Position_H", (s) => { h = Int32.Parse(s); })
                    );

                    if (x.HasValue && y.HasValue)
                    { this.Location = new System.Drawing.Point(x.Value, y.Value); }
                    if (w.HasValue && h.HasValue)
                    { this.Size = new System.Drawing.Size(w.Value, h.Value); }
                }
                catch
                {
                }
            };

            this.FormClosing += (sender, e) => {
                RegistryHelper.TrySetToRegistry(registryPath,
                    new RegistryHelper.SetRegistryAction("FormEx_Position_X", this.Location.X.ToString()),
                    new RegistryHelper.SetRegistryAction("FormEx_Position_Y", this.Location.Y.ToString()),
                    new RegistryHelper.SetRegistryAction("FormEx_Position_W", this.Size.Width.ToString()),
                    new RegistryHelper.SetRegistryAction("FormEx_Position_H", this.Size.Height.ToString())
                 );
            };
        }
    }
}
