using System.Net;
using System.Net.Http;
using System.Web.Http;
using VisaNet.Application.Interfaces;
using VisaNet.Domain.EntitiesDtos.TableFilters;

namespace VisaNet.Services.WebApi.Controllers.Administration
{
    public class ReportsController : ApiController
    {
        private readonly IServiceReports _serviceReports;

        public ReportsController(IServiceReports serviceReports)
        {
            _serviceReports = serviceReports;
        }

        [HttpPost]
        public HttpResponseMessage GetTransactionsAmountData([FromBody] ReportsTransactionsAmountFilterDto filtersDto)
        {
            var data = _serviceReports.GetTransactionsAmountData(filtersDto);
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        [HttpPost]
        public HttpResponseMessage GetCybersourceTransactionsData([FromBody] ReportsCybersourceTransactionsFilterDto filtersDto)
        {
            var data = _serviceReports.GetCybersourceTransactionsData(filtersDto);
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        [HttpPost]
        public HttpResponseMessage GetCybersourceTransactionsDetails([FromBody] ReportsCybersourceTransactionsFilterDto filtersDto)
        {
            var data = _serviceReports.GetCybersourceTransactionsDetails(filtersDto);
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        [HttpPost]
        public HttpResponseMessage GetDashboardSP([FromBody] ReportsDashboardFilterDto filtersDto)
        {
            var data = _serviceReports.GetDashboardSP(filtersDto);
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }
    }
}
