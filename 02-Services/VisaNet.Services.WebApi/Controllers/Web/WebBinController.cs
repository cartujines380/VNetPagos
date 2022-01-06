using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using VisaNet.Application.Interfaces;

namespace VisaNet.Services.WebApi.Controllers.Web
{
    public class WebBinController : ApiController
    {
        private readonly IServiceBin _serviceBin;

        public WebBinController(IServiceBin serviceBin)
        {
            _serviceBin = serviceBin;
        }

        [HttpGet]
        public HttpResponseMessage Get(int value)
        {
            var bin = _serviceBin.Find(value);
            return Request.CreateResponse(HttpStatusCode.OK, bin);
        }

        [HttpPost]
        public HttpResponseMessage GetBinsFromMask(IList<int> mask)
        {
            var bin = _serviceBin.GetBinsFromMask(mask);
            return Request.CreateResponse(HttpStatusCode.OK, bin);
        }

        [HttpGet]
        public HttpResponseMessage FindByGuid(Guid cardId)
        {
            var bin = _serviceBin.FindByGuid(cardId);
            return Request.CreateResponse(HttpStatusCode.OK, bin);
        }
    }
}
