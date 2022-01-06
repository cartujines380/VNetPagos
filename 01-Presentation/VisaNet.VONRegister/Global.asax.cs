using System;
using System.Configuration;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using VisaNet.Common.Logging.NLog;
using VisaNet.VONRegister.Constants;

namespace VisaNet.VONRegister
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var exception = Server.GetLastError();

            NLogLogger.LogEvent(exception);

            Response.Clear();

            var httpException = exception as HttpException;

            if (httpException != null)
            {
                if (!httpException.Message.Contains("favicon"))
                {
                    NLogLogger.LogEvent(httpException);
                }

                //ErrorSignal.FromCurrentContext().Raise(httpException);
                // clear error on server
                Server.ClearError();
                Response.Redirect(string.Format("~/Error"));
            }
        }

        //SI TIENE EL ALLOWALL SE LO PUSE EN Application_PostAcquireRequestState
        protected void Application_PreSendRequestHeaders()
        {
            var isAllow = Response.Headers.Get("X-Frame-Options");
            if (!string.IsNullOrEmpty(isAllow) && isAllow.Contains("AllowAll"))
            {
                Response.Headers.Remove("X-Frame-Options");
                Response.AddHeader("X-Frame-Options", "AllowAll");
            }
        }

        //YA GUARDE EN SESSION QUIEN ME QUIERE INVOCAR
        protected void Application_PostAcquireRequestState()
        {
            var context = HttpContext.Current;
            if (context != null && context.Session != null)
            {
                var dns = (string)Session[SessionConstants.HashedDomain];
                var domains = ConfigurationManager.AppSettings["EnableDomains"];
                if (!string.IsNullOrEmpty(dns))
                {
                    if (domains.Contains(dns))
                    {
                        Response.Headers.Remove("X-Frame-Options");
                        Response.AddHeader("X-Frame-Options", "AllowAll");
                    }
                }
            }
        }

    }
}