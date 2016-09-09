using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Silicups.GUI
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            if (System.IO.File.Exists("autoload.txt"))
            {
                var data = new List<DataPoint>();
                foreach (string s in System.IO.File.ReadAllLines("autoload.txt"))
                {
                    try
                    {
                        if (s.StartsWith("24"))
                        {
                            string[] parts = s.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            var x = Double.Parse(parts[0].Replace(',', '.'), System.Globalization.CultureInfo.InvariantCulture);
                            var y = Double.Parse(parts[1].Replace(',', '.'), System.Globalization.CultureInfo.InvariantCulture);
                            var yerr = Double.Parse(parts[2].Replace(',', '.'), System.Globalization.CultureInfo.InvariantCulture);
                            if (y > 50)
                            { continue; }

                            data.Add(new DataPoint() { X = x, Y = y, Yerr = yerr });
                        }
                    }
                    catch
                    { }
                }
                if (data.Count > 0)
                { graph.DataSource = () => data; }
            }
        }
    }
}
