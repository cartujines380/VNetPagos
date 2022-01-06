using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;
using VisaNet.Common.Exceptions;
using VisaNet.Common.Logging.NLog;

namespace VisaNet.Services.LifApi.ExceptionFilters
{
    public class BusinessExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext.Exception is BusinessException)
            {
                var exception = (BusinessException)actionExecutedContext.Exception;
                NLogLogger.LogEvent(exception);
                actionExecutedContext.Response = new HttpResponseMessage(HttpStatusCode.Forbidden)
                {
                    Content = new StringContent(exception.Message),
                };
            }
        }

    }
}