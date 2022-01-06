using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;
using VisaNet.Common.Exceptions;
using VisaNet.Common.Logging.NLog;

namespace VisaNet.Services.LifApi.ExceptionFilters
{
    public class ProviderBusinessExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext.Exception is ProviderBusinessException)
            {
                var exception = (ProviderBusinessException)actionExecutedContext.Exception;
                NLogLogger.LogEvent(exception);
                actionExecutedContext.Response = new HttpResponseMessage(HttpStatusCode.BadGateway)
                {
                    Content = new StringContent(exception.Message),
                };
            }
        }

    }
}