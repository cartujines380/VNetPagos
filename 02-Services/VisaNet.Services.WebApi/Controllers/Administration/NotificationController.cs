using System.Net;
using System.Net.Http;
using System.Web.Http;
using VisaNet.Application.Interfaces;
using VisaNet.Domain.EntitiesDtos.TableFilters;

namespace VisaNet.Services.WebApi.Controllers.Administration
{

    public class NotificationController : ApiController
    {
        private readonly IServiceNotification _serviceNotification;

        public NotificationController(IServiceNotification serviceNotification)
        {
            _serviceNotification = serviceNotification;
        }

        [HttpGet]
        public HttpResponseMessage Get()
        {
            var notifications = _serviceNotification.GetDataForTable();
            return Request.CreateResponse(HttpStatusCode.OK, notifications);
        }

        [HttpPost]
        public HttpResponseMessage GetDashboardData([FromBody] ReportsDashboardFilterDto filters)
        {
            var notifications = _serviceNotification.GetDashboardData(filters);
            return Request.CreateResponse(HttpStatusCode.OK, notifications);
        }

        //nuevo
        [HttpPost]
        public HttpResponseMessage GetDashboardDataCount([FromBody] ReportsDashboardFilterDto filters)
        {
            var notifications = _serviceNotification.GetDashboardDataCount(filters);
            return Request.CreateResponse(HttpStatusCode.OK, notifications);
        }
    }
}
