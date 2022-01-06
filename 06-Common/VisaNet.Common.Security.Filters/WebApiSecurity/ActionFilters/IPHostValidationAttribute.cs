using System;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace VisaNet.Common.Security.Filters.WebApiSecurity.ActionFilters
{
    public class IPHostValidationAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var context = actionContext.Request.Properties["MS_HttpContext"] as System.Web.HttpContextBase;
            string userIP = context.Request.UserHostAddress;
            try
            {
                //TODO: Validate IP
                //AuthorizedIPRepository.GetAuthorizedIPs().First(x => x == userIP);
            }
            catch (Exception)
            {
                actionContext.Response =
                   new HttpResponseMessage(System.Net.HttpStatusCode.Forbidden)
                   {
                       Content = new StringContent("Unauthorized IP Address")
                   };
                return;
            }
        }
    }
}
