using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using VisaNet.Common.Exceptions;
using VisaNet.Common.Resource.LIF;
using VisaNet.LIF.WebApi.Models;

namespace VisaNet.LIF.WebApi.Controllers
{
    public abstract class BaseApiController : ApiController
    {
        protected HttpResponseMessage CreateHttpResponseMessage(HttpStatusCode code, object content)
        {
            return new HttpResponseMessage
            {
                StatusCode = code,
                Content = new StringContent(System.Web.Helpers.Json.Encode(new OutModel
                {
                    Data = content
                }), Encoding.UTF8, "application/json")
            };
        }

        protected HttpResponseMessage CreateHttpResponseMessage(HttpStatusCode code, object content, string exceptionCode)
        {
            return new HttpResponseMessage
            {
                StatusCode = code,
                Content = new StringContent(System.Web.Helpers.Json.Encode(new OutModel
                {
                    Data = content,
                    Code = GetLIFCode(exceptionCode)
                }), Encoding.UTF8, "application/json")
            };
        }

        private int GetLIFCode(string exceptionCode)
        {
            if (exceptionCode == CodeExceptions.BIN_VALUE_NOT_RECOGNIZED)
                return 3;

            if (exceptionCode == CodeExceptions.BIN_NOTACTIVE)
                return 4;

            if (exceptionCode == CodeExceptions.BIN_NOTVALID_FOR_SERVICE)
                return 5;

            if (exceptionCode == CodeExceptions.BIN_NOTACTIVE2)
                return 6;

            if (exceptionCode == CodeExceptions.GENERAL_ERROR)
                return 7;

            return 100;
        }

        protected HttpResponseMessage CreateHttpResponseMessage(HttpStatusCode code, ModelStateDictionary content)
        {
            //Si entra en este caso es porque el model es null
            if (!content.Values.Any())
            {
                return new HttpResponseMessage
                {
                    StatusCode = code,
                    Content = new StringContent(System.Web.Helpers.Json.Encode(new OutModel
                    {
                        Code = 2,
                        Data = LIFStrings.EmptyModel
                    }), Encoding.UTF8, "application/json")

                };
            }

            return new HttpResponseMessage
            {
                StatusCode = code,
                Content = new StringContent(System.Web.Helpers.Json.Encode(new OutModel
                {
                    Code = 1,
                    Data = content.Values.SelectMany(x => x.Errors).Select(z => z.ErrorMessage)
                }), Encoding.UTF8, "application/json")

            };
        }

        protected IDictionary<string, string> ParseContent(string content)
        {
            var values = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(content))
            {
                values = content.Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries)
               .Select(part => part.Split('='))
               .ToDictionary(split => split[0].ToLower(), split => Uri.UnescapeDataString(split[1]));
            }
            return values;
        }

    }
}