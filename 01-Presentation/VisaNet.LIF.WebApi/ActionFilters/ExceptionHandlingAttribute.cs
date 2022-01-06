using System.Web.Http.Filters;
using VisaNet.Common.Exceptions;
using VisaNet.Common.Logging.NLog;

namespace VisaNet.LIF.WebApi.ActionFilters
{
    public class ExceptionHandlingAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            if (context.Exception is BusinessException)
            {
                return;
            }

            //Log Critical errors
            NLogLogger.LogEvent(NLogType.Error, context.Exception.ToString());
        }
    }
}