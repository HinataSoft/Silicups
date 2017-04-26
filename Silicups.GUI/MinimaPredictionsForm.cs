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
    public partial class MinimaPredictionsForm : Form
    {
        private double M0;
        private double P;
        private DateTime PredictionsSince;
        bool DisableUpdate = false;

        public MinimaPredictionsForm(Project project)
        {
            InitializeComponent();
            this.M0 = project.M0.Value;
            this.P = project.P.Value;
            this.PredictionsSince = DateTime.UtcNow;
            FillPredictions();
        }

        private int GetLineCount()
        {
            return (predictionsListBox.Height - predictionsListBox.Margin.Top - predictionsListBox.Margin.Bottom) / predictionsListBox.ItemHeight;
        }

        private void FillPredictions()
        {
            predictionsListBox.Items.Clear();
            int lineCount = GetLineCount();
            double periods = Math.Ceiling((JD.DateTimeToJD(PredictionsSince) - M0) / P);
            double minimum = M0 + P * periods;
            for (int i = 0; i < lineCount; i++)
            {
                DateTime minimumTime = JD.JDToDateTime(minimum);
                predictionsListBox.Items.Add(String.Format("{0} - {1} UTC - {2}", minimum.ToString("F6", System.Globalization.CultureInfo.InvariantCulture), minimumTime.ToString(), minimumTime.ToLocalTime().ToString()));
                minimum += P;
            }
        }

        private void buttonPrevious_Click(object sender, EventArgs e)
        {
            PredictionsSince = PredictionsSince.AddDays(GetLineCount() * P * -1);
            FillPredictions();
        }

        private void buttonNext_Click(object sender, EventArgs e)
        {
            PredictionsSince = PredictionsSince.AddDays(GetLineCount() * P);
            FillPredictions();
        }

        private void predictionsListBox_Resize(object sender, EventArgs e)
        {
            if (DisableUpdate)
            { return; }
            DisableUpdate = true;
            FillPredictions();
            DisableUpdate = false;
        }
    }
}
