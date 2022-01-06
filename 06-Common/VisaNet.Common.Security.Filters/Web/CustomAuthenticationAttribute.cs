using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;
using VisaNet.Common.Security.Entities;
using VisaNet.Common.Security.Entities.Enums;

namespace VisaNet.Common.Security.Filters.Web
{
    public class CustomAuthenticationAttribute : ActionFilterAttribute, IAuthenticationFilter
    {
        private readonly List<int> _actions;

        public CustomAuthenticationAttribute(params Actions[] actions)
        {
            _actions = new List<int>();
            actions.ToList().ForEach(a => _actions.Add((int)a));
        }

        public void OnAuthentication(AuthenticationContext filterContext)
        {
            var httpContextUser = filterContext.HttpContext.User;
            var authorizedActions = HttpContext.Current.Session["SESSION_CURRENT_USER_ENABLED_ACTIONS"] as List<Action>;

            if (authorizedActions == null || !httpContextUser.Identity.IsAuthenticated)
            {
                filterContext.Result = new RedirectResult("~/Account/LogOff");
                return;
            }


            if (httpContextUser.Identity.IsAuthenticated)
            {
                if (authorizedActions.Any(a => _actions.Contains(a.Id))) return;
            }

            filterContext.Result = new RedirectResult("~/Account/NotAllowed");
        }

        public void OnAuthenticationChallenge(AuthenticationChallengeContext filterContext) { }
    }

}