using System;
using System.Web.Http;
using System.Web.Mvc;
using VisaNet.Common.Logging.NLog;
using VisaNet.Services.LifApi.ExceptionFilters;

namespace VisaNet.Services.LifApi
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            var config = GlobalConfiguration.Configuration;

            config.Filters.Add(new LogExceptionFilterAttribute());
            config.Filters.Add(new GenericExceptionFilterAttribute());
            config.Filters.Add(new BusinessExceptionFilterAttribute());
            config.Filters.Add(new FatalExceptionFilterAttribute());
            config.Filters.Add(new BillExceptionFilterAttribute());

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var exception = Server.GetLastError();
            NLogLogger.LogEvent(exception);
            Response.Clear();
        }

    }
}
