using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using VisaNet.Application.Interfaces;
using VisaNet.Domain.EntitiesDtos;

namespace VisaNet.Services.WebApi.Controllers.Web
{
    public class WebDiscountController : ApiController
    {
        private readonly IServiceDiscountCalculator _serviceDiscountCalculator;

        public WebDiscountController(IServiceDiscountCalculator serviceDiscountCalculator)
        {
            _serviceDiscountCalculator = serviceDiscountCalculator;
        }

        //[HttpGet]
        //public HttpResponseMessage Get()
        //{
        //    var discounts = _serviceDiscount.GetDataForTable();
        //    return Request.CreateResponse(HttpStatusCode.OK, discounts);
        //}

        [HttpPost]
        public HttpResponseMessage Post([FromBody]DiscountQueryDto discountQuery)
        {
            var discount = _serviceDiscountCalculator.Calculate(discountQuery);
            return Request.CreateResponse(HttpStatusCode.OK, discount);
        }


        [HttpGet]
        public HttpResponseMessage Get(int binNumber, Guid serviceId)
        {
            var result = _serviceDiscountCalculator.ValidateBin(binNumber, serviceId);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }
    }
}
