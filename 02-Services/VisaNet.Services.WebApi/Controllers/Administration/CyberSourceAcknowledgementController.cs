using System.Net;
using System.Net.Http;
using System.Web.Http;
using VisaNet.Application.Interfaces;
using VisaNet.Domain.EntitiesDtos;

namespace VisaNet.Services.WebApi.Controllers.Administration
{
    public class CyberSourceAcknowledgementController : ApiController
    {
        private readonly IServiceCyberSourceAcknowledgement _cyberSourceAcknowledgementService;

        public CyberSourceAcknowledgementController(IServiceCyberSourceAcknowledgement cyberSourceAcknowledgementService)
        {
            _cyberSourceAcknowledgementService = cyberSourceAcknowledgementService;
        }

        [HttpPost]
        public HttpResponseMessage Process([FromBody] CyberSourceAcknowledgementDto post)
        {
            _cyberSourceAcknowledgementService.Process(post);
            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}