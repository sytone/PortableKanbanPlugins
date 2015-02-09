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
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PluginsLibrary
{
    /// <summary>
    /// Main form of "Move Tasks" plugin
    /// </summary>
    public partial class MoveTasksForm : Form
    {
        /// <summary>
        /// Source column id
        /// </summary>
        public Guid From 
        { 
            get { return (Guid)comboBoxFrom.SelectedValue; }
            set { try { comboBoxFrom.SelectedValue = value; } catch { } }
        }

        /// <summary>
        /// Destination column id
        /// </summary>
        public Guid To 
        { 
            get { return (Guid)comboBoxTo.SelectedValue; }
            set { try { comboBoxTo.SelectedValue = value; } catch { } }
        }

        /// <summary>
        /// Age of tasks in days (now - completed)
        /// </summary>
        public int Age 
        { 
            get { return (int)ageUpDown.Value; }
            set { try { ageUpDown.Value = value; } catch { } }
        }

        /// <summary>
        /// C'tor requires kanban to fill in comboboxes
        /// </summary>
        /// <param name="kanban"></param>
        public MoveTasksForm(PortableKanban.Data.Kanban kanban)
        {
            InitializeComponent();
            // size
            this.MinimumSize = this.Size;

            comboBoxFrom.DataSource = kanban.Columns;
            comboBoxFrom.DisplayMember = "Name";
            comboBoxFrom.ValueMember = "Id";
            comboBoxTo.DataSource = kanban.Columns;
            comboBoxTo.DisplayMember = "Name";
            comboBoxTo.ValueMember = "Id";
        }
    }
}
