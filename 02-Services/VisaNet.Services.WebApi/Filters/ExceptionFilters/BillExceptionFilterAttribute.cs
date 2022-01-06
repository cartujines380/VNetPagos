using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;
using VisaNet.Common.Exceptions;


namespace VisaNet.Services.WebApi.Filters.ExceptionFilters
{
    public class BillExceptionFilterAttribute:ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext.Exception is BillException)
            {
                var exception = (BillException)actionExecutedContext.Exception;
                
                actionExecutedContext.Response = new HttpResponseMessage(HttpStatusCode.Unused)
                {
                    Content = new StringContent(exception.Message),
                };
            }

        }
    }
}