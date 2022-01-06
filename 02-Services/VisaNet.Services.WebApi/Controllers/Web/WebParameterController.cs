using System.Net;
using System.Net.Http;
using System.Web.Http;
using VisaNet.Application.Interfaces;

namespace VisaNet.Services.WebApi.Controllers.Web
{
    public class WebParameterController : ApiController
    {
        private readonly IServiceParameters _serviceParameters;

        public WebParameterController(IServiceParameters serviceParameters)
        {
            _serviceParameters = serviceParameters;
        }

        [HttpGet]
        public HttpResponseMessage Get()
        {
            var parameters = _serviceParameters.GetParametersForCard();
            return Request.CreateResponse(HttpStatusCode.OK, parameters);
        }
        
    }
}
