using System;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using VisaNet.Common.Logging.NLog;
using VisaNet.Common.Security.Helpers;
using VisaNet.Utilities.Cryptography;

namespace VisaNet.LIF.WebApi
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

        }

        protected void Application_PostMapRequestHandler()
        {
            var authorizationTokenUnEncrypted = string.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}",
                                                                  Request.GetUserIP(),
                                                                  Request.Form["AppId"],
                /*"TransactionIdentifier",*/Guid.NewGuid(),
                /*"TransactionDateTime",*/DateTime.Now,
                /*"RequestUri",*/Request.Url.AbsoluteUri,
                /*"SystemUserId",*/"",
                /*"ApplicationUserId"*/"",
                /*"AnonymousUserId"*/"",
                /*"SessionId"*/"",
                /*"TraceId"*/"");

            Request.Headers.Add("Authorization-Token", AESSecurity.Encrypt(authorizationTokenUnEncrypted));
        }
        
        protected void Application_Error(object sender, EventArgs e)
        {
            var ex = Server.GetLastError().GetBaseException();
            NLogLogger.LogEvent(NLogType.Error, ex.ToString());
        }
    }
}
