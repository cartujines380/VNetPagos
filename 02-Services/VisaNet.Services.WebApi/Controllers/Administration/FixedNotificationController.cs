using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using VisaNet.Application.Interfaces;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters;

namespace VisaNet.Services.WebApi.Controllers.Administration
{

    public class FixedNotificationController : ApiController
    {
        private readonly IServiceFixedNotification _serviceFixedNotificationService;

        public FixedNotificationController(IServiceFixedNotification serviceFixedNotificationService)
        {
            _serviceFixedNotificationService = serviceFixedNotificationService;
        }

        [HttpGet]
        public HttpResponseMessage Get()
        {
            var notifications = _serviceFixedNotificationService.GetDataForMenu();
            return Request.CreateResponse(HttpStatusCode.OK, notifications);
        }

        [HttpPost]
        public HttpResponseMessage FindAll([FromBody] FixedNotificationFilterDto filters)
        {
            var notifications = _serviceFixedNotificationService.GetDataForTable(filters);
            return Request.CreateResponse(HttpStatusCode.OK, notifications);
        }

        [HttpPost]
        public HttpResponseMessage Edit([FromBody] FixedNotificationDto model)
        {
            _serviceFixedNotificationService.Edit(model);
            return Request.CreateResponse(HttpStatusCode.OK);
        }


        [HttpGet]
        public HttpResponseMessage GetById(Guid id)
        {
            var notifications = _serviceFixedNotificationService.GetById(id);
            return Request.CreateResponse(HttpStatusCode.OK, notifications);
        }


        [HttpPost]
        public HttpResponseMessage ResolveAll([FromBody] ResolveAllFixedDto model)
        {
            _serviceFixedNotificationService.ResolveAll(model);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

    }
}
