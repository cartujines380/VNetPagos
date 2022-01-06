using System.Net;
using System.Net.Http;
using System.Web.Http;
using VisaNet.Application.Interfaces;

namespace VisaNet.Services.WebApi.Controllers.Web
{
    public class WebGatewayController : ApiController
    {
        private readonly IServiceService _serviceService;

        public WebGatewayController(IServiceService serviceService)
        {
            _serviceService = serviceService;
        }

        [HttpGet]
        public HttpResponseMessage Get()
        {
            return Request.CreateResponse(HttpStatusCode.OK, _serviceService.GetGateways());
        }

    }
}
