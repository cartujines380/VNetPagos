using System.Net;
using System.Net.Http;
using System.Web.Http;
using VisaNet.Application.Interfaces;

namespace VisaNet.Services.WebApi.Controllers.Web
{
    public class WebPromotionController : ApiController
    {
        private readonly IServicePromotion _servicePromotion;

        public WebPromotionController(IServicePromotion servicePromotion)
        {
            _servicePromotion = servicePromotion;
        }

        [HttpGet]
        public HttpResponseMessage Get()
        {
            var promotion = _servicePromotion.GetLastActive();
            return Request.CreateResponse(HttpStatusCode.OK, promotion);
        }

    }
}