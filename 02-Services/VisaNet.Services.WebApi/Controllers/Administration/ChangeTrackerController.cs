using System.Net;
using System.Net.Http;
using System.Web.Http;
using VisaNet.Application.Interfaces;
using VisaNet.Domain.EntitiesDtos.TableFilters;

namespace VisaNet.Services.WebApi.Controllers.Administration
{
    public class ChangeTrackerController : ApiController
    {
        private readonly IServiceChangeTracker _changeTrackerService;

        public ChangeTrackerController(IServiceChangeTracker changeTrackerService)
        {
            _changeTrackerService = changeTrackerService;
        }

        [HttpPost]
        //[WebApiAuthentication(Actions.BinsList)]
        public HttpResponseMessage GetDataForTable([FromBody] ChangeTrackerFilterDto filterDto)
        {
            var bins = _changeTrackerService.GetDataForTable(filterDto);
            return Request.CreateResponse(HttpStatusCode.OK, bins);
        }

        [HttpPost]
        //[WebApiAuthentication(Actions.BinsDetails)]
        public HttpResponseMessage GetChangesDetails([FromBody] int id)
        {
            var con = _changeTrackerService.GetById(id);
            return Request.CreateResponse(HttpStatusCode.OK, con);
        }

        [HttpPost]
        //[WebApiAuthentication(Actions.BinsDetails)]
        public HttpResponseMessage Count([FromBody] ChangeTrackerFilterDto filterDto)
        {
            var con = _changeTrackerService.Count(filterDto);
            return Request.CreateResponse(HttpStatusCode.OK, con);
        }
        
        [HttpGet]
        //[WebApiAuthentication(Actions.BinsDetails)]
        public HttpResponseMessage GetEntities()
        {
            var con = _changeTrackerService.GetEntities();
            return Request.CreateResponse(HttpStatusCode.OK, con);
        }
    }
}