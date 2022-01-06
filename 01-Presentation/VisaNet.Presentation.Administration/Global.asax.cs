
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Elmah;
using VisaNet.Common.DependencyInjection;
using VisaNet.Common.Logging.NLog;
using VisaNet.Presentation.Administration.Constants;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration;

namespace VisaNet.Presentation.Administration
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected async void Application_Start()
        {
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            MvcHandler.DisableMvcResponseHeader = true;

            System.Web.Optimization.PreApplicationStartCode.Start();

            var currentApplicationContext = HttpContext.Current.Application;
            if (currentApplicationContext[ApplicationConstants.SYSTEM_ACTIONS] == null)
                currentApplicationContext[ApplicationConstants.SYSTEM_ACTIONS] = await NinjectRegister.Get<IRoleClientService>().GetActions();
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var exception = Server.GetLastError();
            Response.Clear();

            var httpException = exception as HttpException;

            if (httpException != null)
            {
                NLogLogger.LogEvent(httpException);

                ErrorSignal.FromCurrentContext().Raise(httpException);
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
        }

    }
}
