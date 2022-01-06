using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using VisaNet.Application.Interfaces;
using VisaNet.Domain.EntitiesDtos.TableFilters;

namespace VisaNet.Services.WebApi.Controllers.Administration
{
    public class WebhookLogController : ApiController
    {
        private readonly IServiceWebhookDown _serviceWebhookDown;
        private readonly IServiceWebhookNewAssociation _serviceWebhookNewAssociation;
        private readonly IServiceWebhookRegistration _serviceWebhookRegistration;

        public WebhookLogController(IServiceWebhookDown serviceWebhookDown, IServiceWebhookNewAssociation serviceWebhookNewAssociation, IServiceWebhookRegistration serviceWebhookRegistration)
        {
            _serviceWebhookDown = serviceWebhookDown;
            _serviceWebhookNewAssociation = serviceWebhookNewAssociation;
            _serviceWebhookRegistration = serviceWebhookRegistration;
        }

        [HttpGet]
        public HttpResponseMessage GetWebhookDowns()
        {
            var users = _serviceWebhookDown.GetDataForTable();
            return Request.CreateResponse(HttpStatusCode.OK, users);
        }

        [HttpGet]
        public HttpResponseMessage GetWebhookRegistrations()
        {
            var users = _serviceWebhookRegistration.GetDataForTable();
            return Request.CreateResponse(HttpStatusCode.OK, users);
        }

        [HttpGet]
        public HttpResponseMessage GetWebhookNewAssociations()
        {
            var users = _serviceWebhookNewAssociation.GetDataForTable();
            return Request.CreateResponse(HttpStatusCode.OK, users);
        }

        [HttpGet]
        public HttpResponseMessage GetWebhookRegistrationsForTable([FromUri] ReportsIntegrationFilterDto filterDto)
        {
            var dtos = _serviceWebhookRegistration.GetWebhookRegistrationsForTable(filterDto);
            return Request.CreateResponse(HttpStatusCode.OK, dtos);
        }

        [HttpGet]
        public HttpResponseMessage GetWebhookRegistrationsForTableCount([FromUri] ReportsIntegrationFilterDto filterDto)
        {
            var count = _serviceWebhookRegistration.GetWebhookRegistrationsForTableCount(filterDto);
            return Request.CreateResponse(HttpStatusCode.OK, count);
        }

        [HttpGet]
        public HttpResponseMessage GetWebhookDownsForTable([FromUri] ReportsIntegrationFilterDto filterDto)
        {
            var dtos = _serviceWebhookDown.GetWebhookDownsForTable(filterDto);
            return Request.CreateResponse(HttpStatusCode.OK, dtos);
        }

        [HttpGet]
        public HttpResponseMessage GetWebhookDownsForTableCount([FromUri] ReportsIntegrationFilterDto filterDto)
        {
            var count = _serviceWebhookDown.GetWebhookDownsForTableCount(filterDto);
            return Request.CreateResponse(HttpStatusCode.OK, count);
        }

        [HttpGet]
        public HttpResponseMessage GetWebhookNewAssociationsForTable([FromUri] ReportsIntegrationFilterDto filterDto)
        {
            var dtos = _serviceWebhookNewAssociation.GetWebhookNewAssociationsForTable(filterDto);
            return Request.CreateResponse(HttpStatusCode.OK, dtos);
        }

        [HttpGet]
        public HttpResponseMessage GetWebhookNewAssociationsForTableCount([FromUri] ReportsIntegrationFilterDto filterDto)
        {
            var count = _serviceWebhookNewAssociation.GetWebhookNewAssociationsForTableCount(filterDto);
            return Request.CreateResponse(HttpStatusCode.OK, count);
        }

        [HttpGet]
        public HttpResponseMessage GetWebhookNewAssociation([FromUri] Guid id)
        {
            var dto = _serviceWebhookNewAssociation.GetById(id);
            return Request.CreateResponse(HttpStatusCode.OK, dto);
        }

        [HttpGet]
        public HttpResponseMessage GetWebhookRegistration([FromUri] Guid id)
        {
            var dto = _serviceWebhookRegistration.GetById(id, x => x.Payment);
            return Request.CreateResponse(HttpStatusCode.OK, dto);
        }

    }
}