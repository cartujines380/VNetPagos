using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using VisaNet.Application.Interfaces;
using VisaNet.Common.DependencyInjection;
using VisaNet.Common.Logging.Entities;
using VisaNet.Common.Logging.Services;
using VisaNet.Common.Security.Entities.Enums;
using VisaNet.Utilities.Cryptography;


namespace VisaNet.Common.Security.Filters.WebApiSecurity.AuthorizationFilters
{
    public class WebApiAuthenticationAttribute : ActionFilterAttribute
    {
        private readonly List<int> _actions;

        public WebApiAuthenticationAttribute() { }

        public WebApiAuthenticationAttribute(params Actions[] actions)
        {
            _actions = new List<int>();
            actions.ToList().ForEach(a => _actions.Add((int)a));
        }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var decryptedTokenValues = new string[3];
            try
            {
                var token = actionContext.Request.Headers.GetValues("Authorization-Token").First();
                var decryptedToken = AESSecurity.Decrypt(token);

                decryptedTokenValues = decryptedToken.Split('|');
                var serviceSystemUser = NinjectRegister.Get<IServiceSystemUser>();

                if (!_actions.Any(action => serviceSystemUser.ValidateUserAction(decryptedTokenValues[1], (Actions)action)))
                    throw new Exception();
            }
            catch (Exception)
            {
                var requestUri = actionContext.Request.RequestUri;

                var loggerService = NinjectRegister.Get<ILoggerService>();

                loggerService.Create(new LogDto
                {
                    LogType = LogType.Info,
                    IP = decryptedTokenValues[0],
                    DateTime = DateTime.Now,
                    TransactionIdentifier = Guid.Parse(decryptedTokenValues[2]),
                    UserName = decryptedTokenValues[1],
                    Message = "Usuario no autorizado.",
                    RequestUri = requestUri.ToString(),
                });


                actionContext.Response = new HttpResponseMessage(System.Net.HttpStatusCode.Forbidden)
                {
                    Content = new StringContent("Unauthorized User")
                };
            }
        }
    }
}
