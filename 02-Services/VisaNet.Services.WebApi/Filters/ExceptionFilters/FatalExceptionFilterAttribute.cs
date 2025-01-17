﻿using System;
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;
using VisaNet.Common.Exceptions;
using VisaNet.Common.Logging.NLog;

namespace VisaNet.Services.WebApi.Filters.ExceptionFilters
{
    public class FatalExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext.Exception is FatalException)
            {
                var exception = (FatalException)actionExecutedContext.Exception;

                Elmah.ErrorSignal.FromCurrentContext().Raise(exception);
                Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception(exception.InternalMessage));
                NLogLogger.LogEvent(exception);
                actionExecutedContext.Response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(exception.Message),
                };
            }

        }
    }
}