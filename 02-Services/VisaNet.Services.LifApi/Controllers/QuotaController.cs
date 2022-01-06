using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using VisaNet.LIF.Interfaces;
using VisaNet.Services.LifApi.Models;

namespace VisaNet.Services.LifApi.Controllers
{
    public class QuotaController : ApiController
    {
        private readonly IQuotaService _quotaService;

        public QuotaController(IQuotaService quotaService)
        {
            _quotaService = quotaService;
        }

        [HttpPost]
        public HttpResponseMessage GetQuotasForBin([FromBody] int cardBin)
        {
            return Request.CreateResponse(HttpStatusCode.OK, _quotaService.GetQuotasForBin(cardBin));
        }

        [HttpPost]
        public HttpResponseMessage GetQuotasForBinAndService([FromBody] GetQuotasForBinAndServiceModel model )
        {
            return Request.CreateResponse(HttpStatusCode.OK, _quotaService.GetQuotasForBinAndService(model.CardBin, model.ServiceId));
        }

    }
}