// Portable Kanban
// Copyright © 2012-2015  Dmitry Ivanov (http://dmitryivanov.net/)
// All rights reserved.

// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:

//   * Redistributions of source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//   * Redistributions in binary form must reproduce the above copyright
//     notice, this list of conditions and the following disclaimer in the
//     documentation and/or other materials provided with the distribution.
//   * Neither the name of Portable Kanban nor the names of its contributors may be used
//     to endorse or promote products derived from this software without
//     specific prior written permission.

// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
// ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE
// LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
// CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
// SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
// INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
// CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
// ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
// POSSIBILITY OF SUCH DAMAGE.

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
    public partial class TimeTrackingForm : Form
    {
        public DateTime From
        {
            // date only
            get { return fromPicker.Value.Date; }
            set { try { fromPicker.Value = value.Date; } catch { } }
        }

        public DateTime To
        {
            // date only
            get { return toPicker.Value.Date; }
            set { try { toPicker.Value = value.Date; } catch { } }
        }

        public bool IncludeTopics
        {
            get { return topicsCheckBox.Checked; }
            set { topicsCheckBox.Checked = value; }
        }

        public bool IncludeTags
        {
            get { return tagsCheckBox.Checked; }
            set { tagsCheckBox.Checked = value; }
        }

        public bool IncludeComments
        {
            get { return commentsCheckBox.Checked; }
            set { commentsCheckBox.Checked = value; }
        }

        public PortableKanban.Data.HtmlReport.ReportType ReportType { get; set; }

        public TimeTrackingForm()
        {
            InitializeComponent();
            // size
            this.MinimumSize = this.Size;      
            // default report type
            ReportType = PortableKanban.Data.HtmlReport.ReportType.Html;
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            if (radioButtonHTML.Checked) ReportType = PortableKanban.Data.HtmlReport.ReportType.Html;
            if (radioButtonDOC.Checked) ReportType = PortableKanban.Data.HtmlReport.ReportType.Doc;
            if (radioButtonXLS.Checked) ReportType = PortableKanban.Data.HtmlReport.ReportType.Xls;
        }

        private void TimeTrackingForm_Load(object sender, EventArgs e)
        {
            // report type
            switch (ReportType)
            {
                case PortableKanban.Data.HtmlReport.ReportType.Html:
                    radioButtonHTML.Checked = true;
                    break;
                case PortableKanban.Data.HtmlReport.ReportType.Doc:
                    radioButtonDOC.Checked = true;
                    break;
                case PortableKanban.Data.HtmlReport.ReportType.Xls:
                    radioButtonXLS.Checked = true;
                    break;
            }
        }
    }
}
