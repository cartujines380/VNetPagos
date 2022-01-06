using System;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using VisaNet.Common.Security.Filters.WebApiSecurity.ActionFilters;
using VisaNet.Services.WebApi.Filters.ExceptionFilters;

namespace VisaNet.Services.WebApi
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            //Security Configurations
            var config = GlobalConfiguration.Configuration;
            config.Filters.Add(new TokenValidationAttribute());
            //config.Filters.Add(new CustomHttpsAttribute());
            config.Filters.Add(new IPHostValidationAttribute());

            config.Filters.Add(new GenericExceptionFilterAttribute());
            config.Filters.Add(new BusinessExceptionFilterAttribute());
            config.Filters.Add(new FatalExceptionFilterAttribute());
            config.Filters.Add(new BillExceptionFilterAttribute());

            GlobalConfiguration.Configuration.Formatters.XmlFormatter.SupportedMediaTypes.Clear();
            GlobalConfiguration.Configuration.Formatters.XmlFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/json"));

            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var exception = Server.GetLastError();
            Response.Clear();
        }

    }
}