using System.Net;
using System.Net.Http;
using System.Web.Http;
using VisaNet.Application.Interfaces;
using VisaNet.Domain.EntitiesDtos;

namespace VisaNet.Services.WebApi.Controllers.Web
{
    public class WebHighwayController : ApiController
    {
        private readonly IServiceHighway _serviceHighway;

        public WebHighwayController(IServiceHighway serviceHighway)
        {
            _serviceHighway = serviceHighway;
        }

        [HttpPut]
        public HttpResponseMessage ProccessEmail([FromBody] HighwayEmailDto entity)
        {
            _serviceHighway.ProccessEmail(entity);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

    }
}