using System.Net;
using System.Net.Http;
using System.Web.Http;
using VisaNet.LIF.Interfaces;
using VisaNet.Services.LifApi.Models;

namespace VisaNet.Services.LifApi.Controllers
{
    public class DiscountController : ApiController
    {
        private readonly IDiscountService _discountService;

        public DiscountController(IDiscountService discountService)
        {
            _discountService = discountService;
        }

        [HttpPost]
        public HttpResponseMessage CalculateDiscount(CalculateDiscountModel model)
        {

            return Request.CreateResponse(HttpStatusCode.OK, _discountService.CalculateDiscount(model.Bill, model.Bin));
        }
        
        [HttpPost]
        public HttpResponseMessage CalculateDiscountForService(CalculateDiscountModel model)
        {

            return Request.CreateResponse(HttpStatusCode.OK, _discountService.CalculateDiscount(model.Bill, model.Bin, model.ServiceId));
        }
    }
}