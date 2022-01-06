using System;
using System.Linq;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using VisaNet.Application.Interfaces;

namespace VisaNet.Common.Security.Filters.WebApiSecurity.ActionFilters
{
    public class TokenValidationAttribute : ActionFilterAttribute
    {
        private readonly IServiceSystemUser _serviceSystemUser;


        public TokenValidationAttribute(IServiceSystemUser serviceSystemUser)
        {
            _serviceSystemUser = serviceSystemUser;
        }

        public TokenValidationAttribute()
        {

        }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            try
            {
                actionContext.Request.Headers.GetValues("Authorization-Token").First();
            }
            catch (Exception)
            {
                actionContext.Response = new HttpResponseMessage(System.Net.HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("Missing Authorization-Token")
                };
                return;
            }
        }

    }
}
