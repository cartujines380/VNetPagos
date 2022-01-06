using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;
using VisaNet.Common.Exceptions;
using VisaNet.Common.Logging.NLog;

namespace VisaNet.Services.LifApi.ExceptionFilters
{
    public class GenericExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext.Exception != null &&
                (
                    (!(actionExecutedContext.Exception is BillException)) &&
                    (!(actionExecutedContext.Exception is BusinessException)) &&
                    (!(actionExecutedContext.Exception is FatalException))
                )
            )
            {
                NLogLogger.LogEvent(actionExecutedContext.Exception);
                actionExecutedContext.Response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(ExceptionMessages.ResourceManager.GetString(CodeExceptions.GENERAL_ERROR)),
                };
            }
        }

    }
}