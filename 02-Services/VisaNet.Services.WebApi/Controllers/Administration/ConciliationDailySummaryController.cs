using System.Net;
using System.Net.Http;
using System.Web.Http;
using VisaNet.Application.Interfaces;
using VisaNet.Common.Security.Entities.Enums;
using VisaNet.Common.Security.Filters.WebApiSecurity.AuthorizationFilters;
using VisaNet.Domain.EntitiesDtos.TableFilters;

namespace VisaNet.Services.WebApi.Controllers.Administration
{
    public class ConciliationDailySummaryController : ApiController
    {
        private readonly IServiceConciliationDailySummary _conciliationDailyService;

        public ConciliationDailySummaryController(IServiceConciliationDailySummary conciliationDailyService)
        {
            _conciliationDailyService = conciliationDailyService;
        }

        [HttpGet]
        public HttpResponseMessage Get()
        {
            return Request.CreateResponse(HttpStatusCode.OK, "OK");
        }

        [HttpPost]
        [WebApiAuthentication(Actions.ConciliationDailyList)]
        public HttpResponseMessage Post([FromBody] DailyConciliationFilterDto filtersDto)
        {
            var data = _conciliationDailyService.GetConciliationDailySummary(filtersDto);
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }
    }
}