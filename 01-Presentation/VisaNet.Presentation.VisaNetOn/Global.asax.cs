using System;
using System.Configuration;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using VisaNet.Common.Logging.NLog;
using VisaNet.Presentation.VisaNetOn.Helpers.Html;

namespace VisaNet.Presentation.VisaNetOn
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var exception = Server.GetLastError();
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
            }
        }

        protected void Application_PreSendRequestHeaders()
        {
            try
            {
                if (Request.UrlReferrer != null)
                {
                    var dnsCalling = Request.UrlReferrer.Authority;

                    //TODO: borrar!
                    //var rawrequest = Request.ToRaw();
                    //NLogLogger.LogEvent(NLogType.Info, "-------------------- INICIO RAW REQUEST --------------------");
                    //NLogLogger.LogEvent(NLogType.Info, rawrequest);
                    //NLogLogger.LogEvent(NLogType.Info, "-------------------- FIN RAW REQUEST --------------------");

                    var domains = ConfigurationManager.AppSettings["EnableDomains"];
                    if (domains.Contains(dnsCalling))
                    {
                        Response.Headers.Remove("X-Frame-Options");
                        Response.AddHeader("X-Frame-Options", "AllowAll");
                    }
                }
            }
            catch (Exception e)
            {
                NLogLogger.LogEvent(NLogType.Info, "VisaNetOn - Global.asax - Application_PreSendRequestHeaders - Exception");
                NLogLogger.LogEvent(e);
            }
        }

    }
}