using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;
using Grapevine.Server;
using PortableKanban.Data;

namespace RestServicePlugin
{
    public class RestServicePlugin : PortableKanban.Data.IKanbanPlugin
    {
        private static BackgroundWorker restServer;
        private static RESTServer server;
        internal static Kanban PersonalKanban;
        public void Run(Guid currentUserId, Guid currentTaskId, Guid currentViewId, Kanban kanban, IWin32Window owner)
        {
            if (PersonalKanban == null)
            {
                // is kanba thread safe... Time will tell! 
                PersonalKanban = kanban;
            }

            if (restServer == null && server == null)
            {
                server = new RESTServer();
                restServer = new BackgroundWorker();
                restServer.WorkerSupportsCancellation = true;
                restServer.DoWork += (sender, args) =>
                {
                    BackgroundWorker worker = sender as BackgroundWorker;

                    // Server Code. 

                    AutoClosingMessageBox.Show("Server Started...", "Personal Kanban REST Service", MessageBoxButtons.OK, MessageBoxIcon.Information, 1000);
                    server.Start();

                    while (server.IsListening && !worker.CancellationPending)
                    {
                        Thread.Sleep(300);
                    }

                    server.Stop();

                };

                restServer.RunWorkerCompleted += (sender, args) =>
                {
                    if (args.Cancelled)
                    {
                        // Nice Cleanup.
                        AutoClosingMessageBox.Show("Server Shutdown...", "Personal Kanban REST Service", MessageBoxButtons.OK, MessageBoxIcon.Information, 1000);
                    }
                    else if (args.Error != null)
                    {
                        // We broke :(
                        AutoClosingMessageBox.Show("Server Crashed...", "Personal Kanban REST Service", MessageBoxButtons.OK, MessageBoxIcon.Error, 1000);
                    }
                    else
                    {
                        // Who knows!
                        AutoClosingMessageBox.Show("Server Shutdown...", "Personal Kanban REST Service", MessageBoxButtons.OK, MessageBoxIcon.Question, 1000);
                    }
                };
            }

            if (restServer.IsBusy)
            {
                restServer.CancelAsync();
            }
            else
            {
                restServer.RunWorkerAsync(kanban);
            }

        }

        public Guid Id { get { return new Guid("{DEA0E5FC-E545-4A88-9E93-9ACFA8A50FFF}"); } }
        public string Name { get { return "Personal Kanban REST Service"; } }   
        public Dictionary<string, string> Settings { get; set; }
        public Action OnUpdateView { get; set; }
    }
}
