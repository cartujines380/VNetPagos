using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;
using VisaNet.Common.Exceptions;
using VisaNet.Common.Logging.NLog;

namespace VisaNet.Services.WebApi.Filters.ExceptionFilters
{
    public class ProviderFatalExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext.Exception is ProviderFatalException)
            {
                var exception = (ProviderFatalException)actionExecutedContext.Exception;
                NLogLogger.LogEvent(exception);
                actionExecutedContext.Response = new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)
                {
                    Content = new StringContent(exception.Message),
                };
            }

        }
    }
}