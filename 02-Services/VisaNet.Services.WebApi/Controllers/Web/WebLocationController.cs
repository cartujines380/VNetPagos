using System.Net;
using System.Net.Http;
using System.Web.Http;
using VisaNet.Application.Interfaces;
using VisaNet.Domain.EntitiesDtos.TableFilters;

namespace VisaNet.Services.WebApi.Controllers.Web
{
    public class WebLocationController : ApiController
    {
        private readonly IServiceLocation _serviceLocation;

        public WebLocationController(IServiceLocation serviceLocation)
        {
            _serviceLocation = serviceLocation;
        }

        [HttpGet]
        public HttpResponseMessage Get()
        {
            var locations = _serviceLocation.GetDataForTable();
            return Request.CreateResponse(HttpStatusCode.OK, locations);
        }
        
        [HttpGet]
        public HttpResponseMessage GetLocationForList([FromUri] LocationFilterDto filter)
        {
            var locations = _serviceLocation.GetDataForList(filter);
            return Request.CreateResponse(HttpStatusCode.OK, locations);
        }
    }
}
