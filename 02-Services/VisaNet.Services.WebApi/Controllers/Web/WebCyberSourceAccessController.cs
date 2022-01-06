using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using VisaNet.Components.CyberSource.Interfaces;
using VisaNet.Services.WebApi.Models;
using VisaNet.Utilities.Cybersource;

namespace VisaNet.Services.WebApi.Controllers.Web
{
    public class WebCyberSourceAccessController : ApiController
    {
        private readonly ICybersourceAccessFacade _cyberSourceAccessFacade;

        public WebCyberSourceAccessController(ICybersourceAccessFacade cyberSourceAccessFacade)
        {
            _cyberSourceAccessFacade = cyberSourceAccessFacade;
        }

        [HttpPost]
        public HttpResponseMessage GenerateKeys([ModelBinder(typeof(CustomModelBinder))] IGenerateToken item)
        {
            var keys = _cyberSourceAccessFacade.GenerateKeys(item);
            return Request.CreateResponse(HttpStatusCode.OK, keys);
        }

    }
}