using System.Web.Http;
using System.Web.Mvc;

namespace VisaNet.Presentation.FileSharing
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}
