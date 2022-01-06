using System.Web.Http.Filters;
using VisaNet.Common.Logging.NLog;

namespace VisaNet.Services.LifApi.ExceptionFilters
{
    public class LogExceptionFilterAttribute : ExceptionFilterAttribute 
    {
        public override void OnException(HttpActionExecutedContext context)
        {
            NLogLogger.LogEvent(context.Exception);
        }
    }
}