using System.Net;
using System.Net.Http;
using System.Web.Http;
using VisaNet.Application.Interfaces;

namespace VisaNet.Services.WebApi.Controllers.Administration
{
    public class GatewayController : ApiController
    {
        private readonly IServiceService _serviceService;

        public GatewayController(IServiceService serviceService)
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
