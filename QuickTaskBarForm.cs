using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PluginsLibrary
{
    public partial class QuickTaskBarForm : Form
    {
        public QuickTaskBarForm()
        {
            InitializeComponent();
            this.MinimumSize = this.Size;
            // fixed height:
            this.MaximumSize = new Size(5000, this.Size.Height);
        }

        private void taskTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (OnTextEntered != null)
                {
                    if (OnTextEntered(taskTextBox.Text))
                    {
                        // done
                        taskTextBox.Text = string.Empty;
                        parsingErrorProvider.SetError(taskTextBox, null);
                    }
                    else
                    {
                        parsingErrorProvider.SetError(taskTextBox, "Failed to parse string");
                    }
                }
            }
        }

        private void helpLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // show rules
            if (OnHelp != null) OnHelp();
        }

        private void QuickTaskBarForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                // don't close, just hide
                Hide();
                e.Cancel = true;
            }
        }

        public Func<string, bool> OnTextEntered { get; set; }
        public Action OnHelp { get; set; }
    }
}
