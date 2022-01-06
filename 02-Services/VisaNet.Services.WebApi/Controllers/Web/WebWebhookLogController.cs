using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using VisaNet.Application.Interfaces;
using VisaNet.Domain.EntitiesDtos.ExternalRequest;

namespace VisaNet.Services.WebApi.Controllers.Web
{
    public class WebWebhookLogController : ApiController
    {
        private readonly IServiceWebhookDown _serviceWebhookDown;
        private readonly IServiceWebhookRegistration _serviceWebhookRegistration;
        private readonly IServiceWebhookNewAssociation _serviceWebhookNewAssociation;

        public WebWebhookLogController(IServiceWebhookDown serviceWebhookDown, IServiceWebhookRegistration serviceWebhookRegistration, IServiceWebhookNewAssociation serviceWebhookNewAssociation)
        {
            _serviceWebhookDown = serviceWebhookDown;
            _serviceWebhookRegistration = serviceWebhookRegistration;
            _serviceWebhookNewAssociation = serviceWebhookNewAssociation;
        }

        [HttpGet]
        public HttpResponseMessage GetWebhookDowns()
        {
            var dtos = _serviceWebhookDown.GetDataForTable();
            return Request.CreateResponse(HttpStatusCode.OK, dtos);
        }

        [HttpPut]
        public HttpResponseMessage PutWebhookDown([FromBody] WebhookDownDto entity)
        {
            var dto = _serviceWebhookDown.Create(entity);
            return Request.CreateResponse(HttpStatusCode.OK, dto);
        }

        [HttpGet]
        public HttpResponseMessage GetWebhookRegistrations()
        {
            var dtos = _serviceWebhookRegistration.GetDataForTable();
            return Request.CreateResponse(HttpStatusCode.OK, dtos);
        }

        [HttpPut]
        public HttpResponseMessage PutWebhookRegistration([FromBody] WebhookRegistrationDto entity)
        {
            var dto = _serviceWebhookRegistration.Create(entity, true);
            if (dto != null && dto.PaymentDto != null)
            {
                WebControllerHelper.LoadServiceReferenceParams(dto.PaymentDto.ServiceDto);
            }
            return Request.CreateResponse(HttpStatusCode.OK, dto);
        }

        [HttpGet]
        public HttpResponseMessage GetWebhookNewAssociations()
        {
            var dtos = _serviceWebhookNewAssociation.GetDataForTable();
            return Request.CreateResponse(HttpStatusCode.OK, dtos);
        }

        [HttpPut]
        public HttpResponseMessage PutWebhookNewAssociation([FromBody] WebhookNewAssociationDto entity)
        {
            var dto = _serviceWebhookNewAssociation.Create(entity);
            return Request.CreateResponse(HttpStatusCode.OK, dto);
        }

        [HttpGet]
        public HttpResponseMessage GetByIdOperation(string idOperation, string idapp)
        {
            var dto = _serviceWebhookRegistration.GetByIdOperation(idOperation, idapp);
            if (dto != null && dto.PaymentDto != null)
            {
                WebControllerHelper.LoadServiceReferenceParams(dto.PaymentDto.ServiceDto);
            }
            return Request.CreateResponse(HttpStatusCode.OK, dto);
        }

        [HttpGet]
        public HttpResponseMessage GetwebHookRegistrationsByIdOperation(string idOperation, Guid serviceId)
        {
            var dto = _serviceWebhookRegistration.GetByIdOperation(idOperation, serviceId);
            if (dto != null && dto.PaymentDto != null)
            {
                WebControllerHelper.LoadServiceReferenceParams(dto.PaymentDto.ServiceDto);
            }
            return Request.CreateResponse(HttpStatusCode.OK, dto);
        }

        [HttpGet]
        public HttpResponseMessage WebhookRegistrationIsIdOperationRepited(string idOperation, string idApp)
        {
            var dto = _serviceWebhookRegistration.IsOperationIdRepited(idOperation, idApp);
            return Request.CreateResponse(HttpStatusCode.OK, dto);
        }

    }
}