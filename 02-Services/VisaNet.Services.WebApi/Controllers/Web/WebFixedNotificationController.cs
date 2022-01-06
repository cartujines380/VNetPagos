using System.Net;
using System.Net.Http;
using System.Web.Http;
using VisaNet.Application.Interfaces;
using VisaNet.Domain.EntitiesDtos;

namespace VisaNet.Services.WebApi.Controllers.Web
{

    public class WebFixedNotificationController : ApiController
    {
        private readonly IServiceFixedNotification _serviceFixedNotificationService;

        public WebFixedNotificationController(IServiceFixedNotification serviceFixedNotificationService)
        {
            _serviceFixedNotificationService = serviceFixedNotificationService;
        }
        
        [HttpPut]
        public HttpResponseMessage Put([FromBody] FixedNotificationDto entity)
        {
            var dto = _serviceFixedNotificationService.Create(entity);
            return Request.CreateResponse(HttpStatusCode.OK, "bien");
        }

    }
}
