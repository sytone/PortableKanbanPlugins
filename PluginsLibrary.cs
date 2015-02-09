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
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;

// Every set of plugins should use own namespace
// to avoid confusion
// Add PortableKanban.Data.dll to the project
namespace PluginsLibrary
{
    /// <summary>
    /// Every plugin should be implemented as a class
    /// implementing IKanbanPlugin interface
    /// </summary>
    public class InfoPlugin : PortableKanban.Data.IKanbanPlugin
    {        
        /// <summary>
        /// Unique plugin id
        /// </summary>
        public Guid Id { get { return new Guid("{A422B074-0D43-4DBE-8E16-7358944B2552}"); } }                    
        
        /// <summary>
        /// Unique plugin name
        /// </summary>
        public string Name { get { return "• Info •"; } }      
  
        /// <summary>
        /// Plugin stores all its' settings in a dictionary
        /// setting name - setting value (as a string)
        /// </summary>
        public Dictionary<string, string> Settings { get; set; }

        /// <summary>
        /// Main plugin function
        /// </summary>
        /// <param name="currentUserId">current user id</param>
        /// <param name="currentTaskId">currently selected task id</param>
        /// <param name="currentViewId">currently selected view id</param>
        /// <param name="kanban">kanban</param>
        /// <param name="owner">main window</param>
        public void Run(
            Guid currentUserId,
            Guid currentTaskId,
            Guid currentViewId,
            PortableKanban.Data.Kanban kanban,
            IWin32Window owner)
        {
            MessageBox.Show(owner,                
                "At this moment your kanban contains:\n\n" +
                kanban.Columns.Count.ToString() + " column(s)\n" +
                kanban.Tasks.Count.ToString() + " task(s)\n" +
                kanban.Topics.Count.ToString() + " topic(s)\n" +
                kanban.Persons.Count.ToString() + " person(s)\n" +
                kanban.Tags.Count.ToString() + " tag(s)\n" +
                kanban.Views.Count.ToString() + " view(s)\n" +
                kanban.Users.Count.ToString() + " user(s)" +
                "\n\nDo you need a specific plugin? Contact developer to discuss your needs.\n",
                Name,
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        /// <summary>
        /// Update main window within plugin methods
        /// </summary>
        /// <returns></returns>
        public Action OnUpdateView { get; set; }
    }

    /// <summary>
    /// Every plugin should be implemented as a class
    /// implementing IKanbanPlugin interface
    /// </summary>
    public class MoveTasksPlugin : PortableKanban.Data.IKanbanPlugin
    {
        /// <summary>
        /// Unique plugin id
        /// </summary>
        public Guid Id { get { return new Guid("{8787B24D-0E5C-465A-9AE8-B51AB2D38283}"); } }
        
        /// <summary>
        /// Unique plugin name
        /// </summary>
        public string Name { get { return "Move Tasks"; } }

        /// <summary>
        /// Plugin stores all its' settings in a dictionary
        /// setting name - setting value (as a string)
        /// </summary>
        public Dictionary<string, string> Settings { get; set; }

        /// <summary>
        /// Main plugin function
        /// </summary>
        /// <param name="currentUserId">current user id</param>
        /// <param name="currentTaskId">currently selected task id</param>
        /// <param name="currentViewId">currently selected view id</param>
        /// <param name="kanban">kanban</param>
        /// <param name="owner">main window</param>
        public void Run(
            Guid currentUserId,
            Guid currentTaskId,
            Guid currentViewId,
            PortableKanban.Data.Kanban kanban,
            IWin32Window owner)
        {            
            MoveTasksForm dlg = new MoveTasksForm(kanban);
            dlg.Text = Name;            

            // reuse settings
            if (Settings != null)
            {
                if (Settings.ContainsKey("From")) { dlg.From = Guid.Parse(Settings["From"]); }
                if (Settings.ContainsKey("To")) { dlg.To = Guid.Parse(Settings["To"]); }
                if (Settings.ContainsKey("Age")) { dlg.Age = int.Parse(Settings["Age"]); }
            }
            
            if (dlg.ShowDialog(owner) != DialogResult.OK) return;

            // select tasks
            DateTime now = DateTime.Now;
            var tasks = kanban.Tasks
                .Where(x => (x.ColumnId == dlg.From && x.Done && (now - x.Completed).TotalDays >= dlg.Age))
                .ToList();
            // update tasks
            foreach (PortableKanban.Data.Task t in tasks)
            { t.ColumnId = dlg.To; }
            // store tasks
            kanban.StoreTasks(tasks);            

            // update settings, will be stored in main app config file
            if (Settings == null) Settings = new Dictionary<string, string>();
            Settings["From"] = dlg.From.ToString();
            Settings["To"] = dlg.To.ToString();
            Settings["Age"] = dlg.Age.ToString();

            // update main window 
            if (OnUpdateView != null) OnUpdateView();
        }

        /// <summary>
        /// Update main window within plugin methods
        /// </summary>
        /// <returns></returns>
        public Action OnUpdateView { get; set; }
    }

    /// <summary>
    /// Every plugin should be implemented as a class
    /// implementing IKanbanPlugin interface
    /// </summary>
    public class TimeTrackingReportPlugin : PortableKanban.Data.IKanbanPlugin
    {
        /// <summary>
        /// Unique plugin id
        /// </summary>
        public Guid Id { get { return new Guid("{8213FB2A-9043-4085-8BFC-7D54FE95178C}"); } }
        
        /// <summary>
        /// Unique plugin name
        /// </summary>
        public string Name { get { return "Time Tracking Report"; } }

        /// <summary>
        /// Plugin stores all its' settings in a dictionary
        /// setting name - setting value (as a string)
        /// </summary>
        public Dictionary<string, string> Settings { get; set; }

        /// <summary>
        /// Main plugin function
        /// </summary>
        /// <param name="currentUserId">current user id</param>
        /// <param name="currentTaskId">currently selected task id</param>
        /// <param name="currentViewId">currently selected view id</param>
        /// <param name="kanban">kanban</param>
        /// <param name="owner">main window</param>
        public void Run(
            Guid currentUserId,
            Guid currentTaskId,
            Guid currentViewId,
            PortableKanban.Data.Kanban kanban,
            IWin32Window owner)
        {
            TimeTrackingForm dlg = new TimeTrackingForm();
            dlg.Text = Name;

            // reuse settings
            if (Settings != null)
            {
                if (Settings.ContainsKey("From")) { dlg.From = DateTime.Parse(Settings["From"]); }
                if (Settings.ContainsKey("To")) { dlg.To = DateTime.Parse(Settings["To"]); }
                if (Settings.ContainsKey("IncludeTopics")) { dlg.IncludeTopics = Boolean.Parse(Settings["IncludeTopics"]); }
                if (Settings.ContainsKey("IncludeTags")) { dlg.IncludeTags = Boolean.Parse(Settings["IncludeTags"]); }
                if (Settings.ContainsKey("IncludeComments")) { dlg.IncludeComments = Boolean.Parse(Settings["IncludeComments"]); }
                if (Settings.ContainsKey("ReportType"))
                {
                    // a bit cumbersome cause ReportType is property
                    PortableKanban.Data.HtmlReport.ReportType rt;
                    if (Enum.TryParse<PortableKanban.Data.HtmlReport.ReportType>(Settings["ReportType"], out rt))
                    {
                        dlg.ReportType = rt;
                    }
                }
            }            

            if (dlg.ShowDialog(owner) != DialogResult.OK) return;           

            // create report...
            string reportTitle = "TimeTrackingReport_" + dlg.From.ToString("yyyyMMdd") + 
                (dlg.From != dlg.To ? ("_" + dlg.To.ToString("yyyyMMdd")) : string.Empty);
            string bodyColor = ColorTranslator.ToHtml(Color.White);
            string foreColor = string.Empty;
            string backColor = string.Empty;
            string foreColor1 = ColorTranslator.ToHtml(Color.Black);
            string foreColor2 = ColorTranslator.ToHtml(Color.Black);
            string foreColorH = ColorTranslator.ToHtml(Color.WhiteSmoke);
            string backColor1 = ColorTranslator.ToHtml(Color.WhiteSmoke);
            string backColor2 = ColorTranslator.ToHtml(Color.LightGray);
            string backColorH = ColorTranslator.ToHtml(Color.DimGray);

            PortableKanban.Data.HtmlReport r = new PortableKanban.Data.HtmlReport(reportTitle, bodyColor);

            // header: from, to... dates are rounded - don't know why
            // but it doesn't look Ok in Excel
            r.AppendTitle(3,
                dlg.From != dlg.To ? (dlg.From.ToString("d") + " - " + dlg.To.ToString("d")) :
                dlg.From.ToString("d"),
                StringAlignment.Center, false, false);
            r.AppendSeparator();

            // for every user...
            foreach (PortableKanban.Data.User user in kanban.Users)
            {
                // select time tracks
                var timeTracks = kanban.TimeTracks
                    .Where(x => x.UserId == user.Id)
                    // from 'from' 00:00 till the end of 'to' day,
                    // To & From contain only dates
                    .Where(x => x.Start >= dlg.From && x.Stop < dlg.To.AddDays(1.0))
                    .OrderBy(x => x.Start);
                if (timeTracks.Count() == 0) continue;

                double totalSpent = 0.0;
                bool toggle = false;
                int taskCount = 0;

                // start table
                r.AppendTitle(3, user.Name, StringAlignment.Near, false, false);
                r.AppendTableStart(100, StringAlignment.Center); // table width in percents
                r.AppendRowStart(backColorH, StringAlignment.Center);
                r.AppendHeader("#", foreColorH, StringAlignment.Center, false, false);
                r.AppendHeader("Task", foreColorH, StringAlignment.Center, false, false);
                if (dlg.IncludeTopics)
                {
                    r.AppendHeader("Topic", foreColorH, StringAlignment.Center, false, false);
                }
                if (dlg.IncludeTags)
                {
                    r.AppendHeader("Tags", foreColorH, StringAlignment.Center, false, false);
                }
                r.AppendHeader("Date", foreColorH, StringAlignment.Center, false, false);
                r.AppendHeader("Start", foreColorH, StringAlignment.Center, false, false);
                r.AppendHeader("Stop", foreColorH, StringAlignment.Center, false, false);
                if (dlg.IncludeComments)
                {
                    r.AppendHeader("Comment", foreColorH, StringAlignment.Center, false, false);
                }
                r.AppendHeader("Spent, hrs", foreColorH, StringAlignment.Center, false, false);
                r.AppendRowEnd();

                foreach (var tt in timeTracks)
                {
                    // toggle colors
                    if (toggle)
                    {
                        backColor = backColor1;
                        foreColor = foreColor1;
                    }
                    else
                    {
                        backColor = backColor2;
                        foreColor = foreColor2;
                    }
                    toggle = !toggle;

                    r.AppendRowStart(backColor, StringAlignment.Center);
                    r.AppendCell((++taskCount).ToString(), foreColor, StringAlignment.Center, false, false);
                    PortableKanban.Data.Task t = kanban.Tasks.FirstOrDefault(x => x.Id == tt.TaskId);
                    // just to be sure
                    r.AppendCell(t != null ? t.Text : "Unknown", foreColor, StringAlignment.Near, false, false);
                    // topic
                    if (dlg.IncludeTopics)
                    {
                        PortableKanban.Data.Topic topic = kanban.GetTopic(t.TopicId);
                        r.AppendCell(topic != null ? topic.Name : string.Empty, foreColor, StringAlignment.Center, false, false);
                    }
                    // tags
                    if (dlg.IncludeTags)
                    {
                        if (t.Tags.Count > 0)
                        {
                            StringBuilder sb = new StringBuilder();
                            for (int i = 0; i < t.Tags.Count; i++)
                            {
                                PortableKanban.Data.Tag tag = kanban.GetTag(t.Tags[i]);
                                if (tag != null)
                                {
                                    sb.Append(tag.Name);
                                    if (i < t.Tags.Count - 1) sb.Append(", ");
                                }
                            }
                            sb.AppendLine();
                            r.AppendCell(sb.ToString(), foreColor, StringAlignment.Center, false, false);
                        }
                        else
                        {
                            r.AppendCell(string.Empty, foreColor, StringAlignment.Center, false, false);
                        }
                    }
                    // start
                    r.AppendCell(tt.Start.ToShortDateString(), foreColor, StringAlignment.Center, false, false);
                    r.AppendCell(tt.Start.ToShortTimeString(), foreColor, StringAlignment.Center, false, false);
                    // stop
                    r.AppendCell(tt.Stop.ToShortTimeString(), foreColor, StringAlignment.Center, false, false);
                    // comment
                    if (dlg.IncludeComments)
                    {
                        r.AppendCell(tt.Comment , foreColor, StringAlignment.Near, false, false);
                    }
                    // spent
                    double spent = (tt.Stop - tt.Start).TotalHours;
                    r.AppendCell(spent.ToString("F2"), foreColor, StringAlignment.Center, false, false);
                    totalSpent += spent;
                }

                // add summary; toggle colors
                if (toggle)
                {
                    backColor = backColor1;
                    foreColor = foreColor1;
                }
                else
                {
                    backColor = backColor2;
                    foreColor = foreColor2;
                }
                r.AppendRowStart(backColor, StringAlignment.Center);
                r.AppendCell(string.Empty, foreColor, StringAlignment.Center, false, false); //#
                r.AppendCell("Total spent, hrs:", foreColor, StringAlignment.Near, false, false);
                if (dlg.IncludeTopics) r.AppendCell(string.Empty, foreColor, StringAlignment.Near, false, false);
                if (dlg.IncludeTags) r.AppendCell(string.Empty, foreColor, StringAlignment.Near, false, false);
                if (dlg.IncludeComments) r.AppendCell(string.Empty, foreColor, StringAlignment.Near, false, false);
                r.AppendCell(string.Empty, foreColor, StringAlignment.Center, false, false); // date
                r.AppendCell(string.Empty, foreColor, StringAlignment.Center, false, false); // start
                r.AppendCell(string.Empty, foreColor, StringAlignment.Center, false, false); // stop
                r.AppendCell(totalSpent.ToString("F2"), foreColor, StringAlignment.Center, false, false);
                r.AppendRowEnd();
                r.AppendTableEnd();
            }                       

            // save to file & open it
            r.Flush(dlg.ReportType, true);            

            // update settings, will be stored within main app config file
            if (Settings == null) Settings = new Dictionary<string, string>();
            Settings["From"] = dlg.From.ToString("d"); // short date
            Settings["To"] = dlg.To.ToString("d");
            Settings["IncludeTopics"] = dlg.IncludeTopics.ToString();
            Settings["IncludeTags"] = dlg.IncludeTags.ToString();
            Settings["IncludeComments"] = dlg.IncludeComments.ToString();
            Settings["ReportType"] = dlg.ReportType.ToString();
        }

        /// <summary>
        /// Update main window within plugin methods
        /// </summary>
        /// <returns></returns>
        public Action OnUpdateView { get; set; }
    }

    /// <summary>
    /// Every plugin should be implemented as a class
    /// implementing IKanbanPlugin interface
    /// </summary>
    public class QuickTaskBarPlugin : PortableKanban.Data.IKanbanPlugin
    {
        /// <summary>
        /// Unique plugin id
        /// </summary>
        public Guid Id { get { return new Guid("{794548FB-9729-4E96-9249-D39F8843898D}"); } }
        
        /// <summary>
        /// Unique plugin name
        /// </summary>
        public string Name { get { return "Quick Task Bar"; } }

        /// <summary>
        /// Plugin stores all its' settings in a dictionary
        /// setting name - setting value (as a string)
        /// </summary>
        public Dictionary<string, string> Settings { get; set; }

        private static QuickTaskBarForm _dlg = new QuickTaskBarForm();
        private PortableKanban.Data.Kanban _kanban;

        private const string _initialsPrefix = "@";
        private const string _topicPrefix = "%";
        private const string _columnPrefix = "$";
        private const string _deadlinePrefix = "~";
        private const string _tagPrefix = "#";
        private const string _mediumPrty = "!!";
        private const string _highPrty = "!!!";
        private const string _helpFormat =
            "Add new tasks using the following syntax:\n\n" +
            "[Priority] [" + _initialsPrefix + "Initials] [" + _topicPrefix + "Topic] [" + 
            _columnPrefix + "Column] [" + _deadlinePrefix + "Deadline] [" +_tagPrefix + "Tag] Task\n\n" +
            "where:\n" +
            "Priority:\tnone - Low, " + _mediumPrty + " - Medium, " + _highPrty + " - High;\n" +
            "Initials:\tperson initials, skip for default one;\n" +
            "Topic:\ttopic, skip for default one;\n" +
            "Column:\tcolumn, skip for the 1st column;\n" +
            "Deadline:\tdeadline date in the short form;\n" +
            "Tag:\ttag, repeat if more than one tag to be assigned;\n" +
            "Task:\ttask text.\n\n" +
            "Samples:\n\n" +
            _mediumPrty + " " + _initialsPrefix + "ME " +  _topicPrefix + "Leisure " + _columnPrefix + "Year " + 
            _deadlinePrefix + "{0} " + _tagPrefix + "Trips Go to vacation\n" +
            _topicPrefix + "work " + _columnPrefix + "\"this week\" Finish report " + _tagPrefix + "urgent " + _highPrty;

        /// <summary>
        /// Main plugin function
        /// </summary>
        /// <param name="currentUserId">current user id</param>
        /// <param name="currentTaskId">currently selected task id</param>
        /// <param name="currentViewId">currently selected view id</param>
        /// <param name="kanban">kanban</param>
        /// <param name="owner">main window</param>
        public void Run(
            Guid currentUserId,
            Guid currentTaskId,
            Guid currentViewId,
            PortableKanban.Data.Kanban kanban,
            IWin32Window owner)
        {
            // will be used
            _kanban = kanban;
            // non-modal
            if (!_dlg.Visible)
            {
                _dlg.Text = Name;
                _dlg.OnTextEntered = OnTextEntered;
                _dlg.OnHelp = OnHelp;
                _dlg.Show(owner);
            }
        }

        /// <summary>
        /// Parse text
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private bool OnTextEntered(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return false;
            List<PortableKanban.Data.Person> persons = _kanban.Persons;
            List<PortableKanban.Data.Topic> topics = _kanban.Topics;
            List<PortableKanban.Data.Column> columns = _kanban.Columns;
            List<PortableKanban.Data.Tag> tags = _kanban.Tags;

            // won't work with quoted strings
            // List<string> tokens = new List<string>(text.Trim().Split(' '));
            // another way is to use regex, but it's too cumbersome
            List<string> tokens = Split(text);
            
            // create new task
            PortableKanban.Data.Task task = new PortableKanban.Data.Task();

            // parse text:
            // priority
            int index = tokens.FindIndex(x => x.StartsWith(_highPrty));
            if (index != -1 && tokens[index].Length == _highPrty.Length)
            {
                task.Priority = PortableKanban.Data.TaskPriority.High;
                tokens.RemoveAt(0);
            }
            index = tokens.FindIndex(x => x.StartsWith(_mediumPrty));
            if (index != -1 && tokens[index].Length == _mediumPrty.Length)
            {
                task.Priority = PortableKanban.Data.TaskPriority.High;
                tokens.RemoveAt(0);
            }
          
            // person          
            index = tokens.FindIndex(x => x.StartsWith(_initialsPrefix));
            if (index != -1 && tokens[index].Length > 1)
            {
                string temp = tokens[index].ToLower().Substring(1).Trim('"'); //!
                int i = persons.FindIndex(x => x.Initials.ToLower() == temp);
                if (i != -1)
                {
                    // found
                    task.PersonId = persons[i].Id;
                    tokens.RemoveAt(index);
                }
                else
                {
                    // default person (if possible)
                    i = persons.FindIndex(x => x.Default == true);
                    if (i != -1) task.PersonId = persons[i].Id;
                }
            }

            // topic
            index = tokens.FindIndex(x => x.StartsWith(_topicPrefix));
            if (index != -1 && tokens[index].Length > 1)
            {
                string temp = tokens[index].ToLower().Substring(1).Trim('"'); //!
                int i = topics.FindIndex(x => x.Name.ToLower() == temp);
                if (i != -1)
                {
                    // found
                    task.TopicId = topics[i].Id;
                    tokens.RemoveAt(index);
                }
                else
                {
                    // default topic (if possible)
                    i = topics.FindIndex(x => x.Default == true);
                    if (i != -1) task.TopicId = topics[i].Id;
                }
            }

            // column
            index = tokens.FindIndex(x => x.StartsWith(_columnPrefix));
            if (index != -1 && tokens[index].Length > 1)
            {
                string temp = tokens[index].ToLower().Substring(1).Trim('"'); //!
                int i = columns.FindIndex(x => x.Name.ToLower() == temp);
                if (i != -1)
                {
                    // found
                    task.ColumnId = columns[i].Id;
                    tokens.RemoveAt(index);
                }
                else
                {
                    // default column is 0 (backlog)                 
                    if (columns.Count > 0) task.ColumnId = columns[0].Id;
                }
            }

            // deadline
            index = tokens.FindIndex(x => x.StartsWith(_deadlinePrefix));
            if (index != -1 && tokens[index].Length > 1)
            {
                string temp = tokens[index].Substring(1).Trim('"'); //!;
                DateTime day;
                if (DateTime.TryParse(temp, out day))
                {
                    task.HasDeadline = true;
                    task.Deadline = day;
                    tokens.RemoveAt(index);
                }
            }

            // tags, may be more than one
            List<string> tagNames = tokens.FindAll(x => x.StartsWith(_tagPrefix));
            foreach (string t in tagNames)
            {
                string temp = t.ToLower().Substring(1).Trim('"'); //!
                int i = tags.FindIndex(x => x.Name.ToLower() == temp);
                if (i != -1)
                {
                    // found
                    task.Tags.Add(tags[i].Id);
                    tokens.RemoveAt(tokens.IndexOf(t));
                }                
            }
            // default tag (if any)
            if (task.Tags.Count == 0)
            {
                int i = tags.FindIndex(x => x.Default == true);
                if (i != -1) task.Tags.Add(tags[i].Id);
            }

            // task text: everything else
            task.Text = string.Join(" ", tokens).Trim();

            // assign column - it has to be
            if (task.ColumnId == Guid.Empty)
            {
                if (columns.Count > 0) task.ColumnId = columns[0].Id;
            }

            // validate task & save   
            if (!string.IsNullOrWhiteSpace(task.Text) && task.ColumnId != Guid.Empty)
            {
                _kanban.StoreTask(task);
                //MessageBox.Show(
                    //_dlg, _kanban.DumpTask(task), _name,
                    //MessageBoxButtons.OK, MessageBoxIcon.Information);
                if (OnUpdateView != null) OnUpdateView();
                return true;
            }
                        
            return false;
        }

        /// <summary>
        /// Show help
        /// </summary>
        private void OnHelp()
        {
            string help = string.Format(_helpFormat, DateTime.Now.ToShortDateString());
            MessageBox.Show(_dlg, help, Name, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Update main window within plugin methods
        /// </summary>
        /// <returns></returns>
        public Action OnUpdateView { get; set; }

        /// <summary>
        /// Dead simple string splitter with quoted string support
        /// </summary>
        /// <param name="textToSplit"></param>
        /// <returns>list of splitted strings</returns>
        private List<string> Split(string textToSplit)
        {
            List<string> tokens = new List<string>();
            StringBuilder sb = new StringBuilder();
            bool quoted = false;
            string text = textToSplit.Trim();

            foreach (char c in text)
            {
                if (c == '"')
                {
                    quoted = !quoted;
                }

                if (!quoted && c == ' ')
                {
                    tokens.Add(sb.ToString());
                    sb = new StringBuilder();
                }
                else // if (c != '"') // don't skip quotes, mauybe a part of task text
                {
                    sb.Append(c);
                }
            }

            if (sb.Length > 0) tokens.Add(sb.ToString());
            return tokens;
        }
    }
}
