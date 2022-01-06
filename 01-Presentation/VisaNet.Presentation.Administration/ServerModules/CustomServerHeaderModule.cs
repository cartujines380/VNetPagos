using System;
using System.Web;

namespace VisaNet.Presentation.Administration.ServerModules
{
    public class CustomServerHeaderModule : IHttpModule
    {
        public void Init(HttpApplication context)
        {
            context.PreSendRequestHeaders += (sender, e) => ((HttpApplication)sender).Response.Headers.Remove("Server");
        }

        public void Dispose() { }
    }
}