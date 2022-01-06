using System;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;


namespace VisaNet.Common.Security.Filters.WebApiSecurity.ActionFilters
{
    public class CustomHttpsAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (!String.Equals(actionContext.Request.RequestUri.Scheme, "https", StringComparison.OrdinalIgnoreCase))
            {
                actionContext.Response = new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("HTTPS Required")
                };
                return;
            }
        }

    }
}
