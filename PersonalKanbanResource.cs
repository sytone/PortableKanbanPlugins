using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Grapevine;
using Grapevine.Server;

namespace RestServicePlugin
{
    
    public sealed class PersonalKanbanResource : RESTResource
    {
        [RESTRoute(Method = HttpMethod.GET, PathInfo = @"^\/columns")]
        public void HandleColumnsRequests(HttpListenerContext context)
        {
            this.SendJsonResponse(context, RestServicePlugin.PersonalKanban.Columns);
        }

        [RESTRoute(Method = HttpMethod.GET, PathInfo = @"^\/people")]
        public void HandlePeopleRequests(HttpListenerContext context)
        {
            this.SendJsonResponse(context, RestServicePlugin.PersonalKanban.Persons);
        }

        [RESTRoute(Method = HttpMethod.GET, PathInfo = @"^\/tags")]
        public void HandleTagsRequests(HttpListenerContext context)
        {
            this.SendJsonResponse(context, RestServicePlugin.PersonalKanban.Tags);
        }

        [RESTRoute(Method = HttpMethod.GET, PathInfo = @"^\/tasks$")]
        public void HandleTasksRequests(HttpListenerContext context)
        {
            this.SendJsonResponse(context, RestServicePlugin.PersonalKanban.Tasks);
        }

        [RESTRoute(Method = HttpMethod.GET, PathInfo = @"^\/tasks\/\b[A-F0-9]{8}(?:-[A-F0-9]{4}){3}-[A-F0-9]{12}\b$")]
        public void HandleTaskRequests(HttpListenerContext context)
        {
            Guid taskGuid = Guid.Parse(context.Request.Url.AbsolutePath.Replace("/tasks/", ""));
            this.SendJsonResponse(context, RestServicePlugin.PersonalKanban.GetTask(taskGuid));
        }

        [RESTRoute(Method = HttpMethod.GET, PathInfo = @"^\/topics")]
        public void HandleTopicsRequests(HttpListenerContext context)
        {
            this.SendJsonResponse(context, RestServicePlugin.PersonalKanban.Topics);
        }

        //[RESTRoute]
        public void HandleAllGetRequests(HttpListenerContext context)
        {
            this.SendTextResponse(context, "GET is a success!");
        }

        [RESTRoute(Method = HttpMethod.DELETE, PathInfo = @"^/shutdown$")]
        public void HandleShutDownRequests(HttpListenerContext context)
        {
            this.SendTextResponse(context, "shutting down");
            this.Server.Stop();
        }
    }
}
