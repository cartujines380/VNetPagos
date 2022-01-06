using System.Net;
using System.Net.Http;
using System.Web.Http;
using VisaNet.Application.Interfaces;
using VisaNet.Domain.EntitiesDtos.TableFilters;

namespace VisaNet.Services.WebApi.Controllers.Web
{
    public class WebNotificationController : ApiController
    {
        private readonly IServiceNotification _serviceNotification;

        public WebNotificationController(IServiceNotification serviceNotification)
        {
            _serviceNotification = serviceNotification;
        }

        [HttpPost]
        public HttpResponseMessage Post([FromBody] NotificationFilterDto filterDto)
        {
            var notifications = _serviceNotification.GetDataForTable(filterDto);
            if (notifications != null)
            {
                foreach (var notificationDto in notifications)
                {
                    WebControllerHelper.LoadServiceReferenceParams(notificationDto.Service);
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, notifications);
        }

    }
}