using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using VisaNet.Application.Interfaces;
using VisaNet.Domain.EntitiesDtos.TableFilters;

namespace VisaNet.Services.WebApi.Controllers.Web
{
    public class WebReportController : ApiController
    {
        private readonly IServiceReports _serviceReports;

        public WebReportController(IServiceReports serviceReports)
        {
            _serviceReports = serviceReports;
        }

        [HttpPost]
        public HttpResponseMessage PieChart([FromBody] ReportFilterDto filterDto)
        {
            return Request.CreateResponse(HttpStatusCode.OK, _serviceReports.GetDashboardPieChartData(filterDto));
        }

        [HttpPost]
        public HttpResponseMessage LineChart([FromBody] ReportFilterDto filterDto)
        {
            return Request.CreateResponse(HttpStatusCode.OK, _serviceReports.GetDashboardLineChartData(filterDto));
        }

        [HttpGet]
        public HttpResponseMessage ServicesCategories(Guid userId)
        {
            return Request.CreateResponse(HttpStatusCode.OK, _serviceReports.ServicesCategories(userId));
        }

        [HttpGet]
        public HttpResponseMessage ServicesWithPayments(Guid userId)
        {
            return Request.CreateResponse(HttpStatusCode.OK, _serviceReports.ServicesWithPayments(userId));
        }

    }
}