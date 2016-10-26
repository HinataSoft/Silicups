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
    public partial class InputBoxForm : Form
    {
        public string PromptValue = null;

        public InputBoxForm(string caption, string defaultText = "")
        {
            InitializeComponent();

            label.Text = caption;
            textBox.Text = defaultText;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            PromptValue = textBox.Text;
            if (String.IsNullOrWhiteSpace(PromptValue))
            { PromptValue = null; }
        }
    }
}
