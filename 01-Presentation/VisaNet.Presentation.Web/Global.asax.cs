using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using VisaNet.Common.Logging.NLog;
using VisaNet.Presentation.Web.Constants;
using VisaNet.Presentation.Web.Models;

namespace VisaNet.Presentation.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            MvcHandler.DisableMvcResponseHeader = true;

            //SETEO INICIAL DE COMCERCIOS DE DEBITO
            var currentApplicationContext = HttpContext.Current.Application;
            if (currentApplicationContext[ApplicationConstants.APPLICATION_SELECTED_COMMERCES] == null)
                currentApplicationContext[ApplicationConstants.APPLICATION_SELECTED_COMMERCES] = new List<CommerceModel>();
        }

        protected void Application_BeginRequest()
        {
            CultureInfo CI = new CultureInfo("es-UY");
            CI.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
            Thread.CurrentThread.CurrentUICulture = CI;
            Thread.CurrentThread.CurrentCulture = CI;

            //Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("es-UY");
            //Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("es-UY");
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
                Response.Redirect(string.Format("~/Error"));
            }
        }

        protected void Application_BeginRequest(Object sender, EventArgs e)
        {
            if (HttpContext.Current.Request.IsSecureConnection.Equals(false) && HttpContext.Current.Request.IsLocal.Equals(false))
            {
                Response.Redirect("https://" + Request.ServerVariables["HTTP_HOST"] + HttpContext.Current.Request.RawUrl);
            }

            if (HttpContext.Current.Request.RawUrl.ToLower().Contains("/callcenter"))
            {
                var userHostAddress = Request.UserHostAddress;
                var enableIPs = ConfigurationManager.AppSettings["IpForCallCenter"];

                if (!enableIPs.Contains(userHostAddress))
                {
                    NLogLogger.LogEvent(NLogType.Info, string.Format("LA IP {0} QUISO ACCEDER A CALLCENTER PERO NO ESTA HABILITADA", userHostAddress));
                    var host = Request.ServerVariables["HTTP_HOST"];
                    Response.Redirect("https://" + host);
                }
            }

        }

        //protected void Session_End(Object sender, EventArgs e)
        //{
        //    Session["TimeOut"] = true;
        //}

    }
}
