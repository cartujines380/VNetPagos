using System;
using System.Web.Mvc;

namespace VisaNet.Common.Security.Filters.Web
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class CustomAuthorize : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                filterContext.Result = new JsonResult
                {
                    Data = new
                    {
                        sessionExpired = true,
                        loginUrl = System.Web.Security.FormsAuthentication.LoginUrl
                    },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
            else
            {
                base.HandleUnauthorizedRequest(filterContext);
            }
        }
    }

}