using System.Net;
using System.Net.Http;
using System.Web.Http;
using VisaNet.Application.Interfaces;

namespace VisaNet.Services.WebApi.Controllers.Web
{
    public class WebQuotationController : ApiController
    {
        private readonly IServiceQuotation _serviceQuotation;

        public WebQuotationController(IServiceQuotation serviceQuotation)
        {
            _serviceQuotation = serviceQuotation;
        }

        [HttpGet]
        public HttpResponseMessage Get()
        {
            var quotations = _serviceQuotation.GetDataForTable();
            return Request.CreateResponse(HttpStatusCode.OK, quotations);
        }

    }
}