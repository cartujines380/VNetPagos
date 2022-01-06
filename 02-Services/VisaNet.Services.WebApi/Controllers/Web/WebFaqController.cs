using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using VisaNet.Application.Interfaces;
using VisaNet.Domain.EntitiesDtos.TableFilters;

namespace VisaNet.Services.WebApi.Controllers.Web
{
    public class WebFaqController : ApiController
    {
        private readonly IServiceFaq _serviceFaq;

        public WebFaqController(IServiceFaq serviceFaq)
        {
            _serviceFaq = serviceFaq;
        }

        [HttpGet]
        public HttpResponseMessage Get([FromUri] FaqFilterDto filterDto)
        {
            var faqs = _serviceFaq.GetDataForTable(filterDto);
            return Request.CreateResponse(HttpStatusCode.OK, faqs);
        }

        [HttpGet]
        public HttpResponseMessage Get(string id)
        {
            var faq = _serviceFaq.GetById(Guid.Parse(id));
            return Request.CreateResponse(HttpStatusCode.OK, faq);
        }
    }
}
