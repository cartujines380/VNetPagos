using System.Net;
using System.Net.Http;
using System.Web.Http;
using VisaNet.LIF.Interfaces;
using VisaNet.Services.LifApi.Models;

namespace VisaNet.Services.LifApi.Controllers
{
    public class CardController : ApiController
    {
        private readonly ICardService _cardService;

        public CardController(ICardService cardService)
        {
            _cardService = cardService;
        }

        [HttpPost]
        public HttpResponseMessage GetCardInfo(GetCardInfoModel model)
        {
            return Request.CreateResponse(HttpStatusCode.OK, _cardService.GetCardInfo(model.Bin,  model.IncludeIssuingCompany));
        }
        
        [HttpPost]
        public HttpResponseMessage GetNationalBins(GetNationalBinsModel model)
        {
            return Request.CreateResponse(HttpStatusCode.OK, _cardService.GetNationalBins(model.IncludeIssuingCompany));
        }
    }
}