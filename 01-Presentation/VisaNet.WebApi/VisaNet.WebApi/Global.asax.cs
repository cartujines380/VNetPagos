using System;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using VisaNet.Common.Logging.NLog;

namespace VisaNet.WebApi
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var exception = Server.GetLastError();
            NLogLogger.LogEvent(exception);
        }
    }
}
