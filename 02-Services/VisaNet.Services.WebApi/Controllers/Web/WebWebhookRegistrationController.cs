using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using VisaNet.Application.Interfaces;
using VisaNet.Domain.Entities.ExternalRequest;
using VisaNet.Domain.EntitiesDtos.ExternalRequest;

namespace VisaNet.Services.WebApi.Controllers.Web
{
    public class WebWebhookRegistrationController : ApiController
    {
        private readonly IServiceWebhookRegistration _serviceWebhookRegistration;

        public WebWebhookRegistrationController(IServiceWebhookRegistration serviceWebhookRegistration)
        {
            _serviceWebhookRegistration = serviceWebhookRegistration;
        }

        [HttpGet]
        public HttpResponseMessage Get()
        {
            var users = _serviceWebhookRegistration.GetDataForTable();
            return Request.CreateResponse(HttpStatusCode.OK, users);
        }

        [HttpPut]
        public HttpResponseMessage Put([FromBody] WebhookRegistrationDto entity)
        {
            var dto = _serviceWebhookRegistration.Create(entity);
            if (dto != null && dto.PaymentDto != null)
            {
                WebControllerHelper.LoadServiceReferenceParams(dto.PaymentDto.ServiceDto);
            }
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
        public HttpResponseMessage GetByIdOperation(string idOperation, Guid serviceId)
        {
            var dto = _serviceWebhookRegistration.GetByIdOperation(idOperation, serviceId);
            if (dto != null && dto.PaymentDto != null)
            {
                WebControllerHelper.LoadServiceReferenceParams(dto.PaymentDto.ServiceDto);
            }
            return Request.CreateResponse(HttpStatusCode.OK, dto);
        }

        [HttpGet]
        public HttpResponseMessage IsOperationIdRepited(string idOperation, string idApp)
        {
            var dto = _serviceWebhookRegistration.IsOperationIdRepited(idOperation, idApp);
            return Request.CreateResponse(HttpStatusCode.OK, dto);
        }

        [HttpPost]
        public HttpResponseMessage GenerateAccessToken([FromBody] WebhookRegistrationDto entity)
        {
            var webhookAccessToken = _serviceWebhookRegistration.GenerateAccessToken(entity);
            return Request.CreateResponse(HttpStatusCode.OK, webhookAccessToken);
        }

        [HttpPost]
        public HttpResponseMessage GetByAccessToken([FromBody] WebhookAccessTokenDto dto)
        {
            var webhookRegistrationDto = _serviceWebhookRegistration.GetByAccessToken(dto);
            if (webhookRegistrationDto != null && webhookRegistrationDto.PaymentDto != null)
            {
                WebControllerHelper.LoadServiceReferenceParams(webhookRegistrationDto.PaymentDto.ServiceDto);
            }
            return Request.CreateResponse(HttpStatusCode.OK, webhookRegistrationDto);
        }

        [HttpPost]
        public HttpResponseMessage ValidateAccessToken([FromBody] WebhookAccessTokenDto dto)
        {
            var ok = _serviceWebhookRegistration.ValidateAccessToken(dto);
            return Request.CreateResponse(HttpStatusCode.OK, ok);
        }

        [HttpGet]
        public HttpResponseMessage FindById(Guid id)
        {
            var webhookRegistrationDto = _serviceWebhookRegistration.GetById(id, x => x.BillLines);
            if (webhookRegistrationDto != null && webhookRegistrationDto.PaymentDto != null)
            {
                WebControllerHelper.LoadServiceReferenceParams(webhookRegistrationDto.PaymentDto.ServiceDto);
            }
            return Request.CreateResponse(HttpStatusCode.OK, webhookRegistrationDto);
        }

        [HttpGet]
        public HttpResponseMessage IsTokenActive([FromUri] AccessTokenFilterDto dto)
        {
            var isIt = _serviceWebhookRegistration.IsTokenActive(dto);
            return Request.CreateResponse(HttpStatusCode.OK, isIt);
        }

        [HttpGet]
        public HttpResponseMessage SetAccessTokenAsPaid(Guid webhookRegistrationId)
        {
            var isIt = _serviceWebhookRegistration.UpdateStatusAccessToken(webhookRegistrationId, WebhookAccessState.Paid);
            return Request.CreateResponse(HttpStatusCode.OK, isIt);
        }

    }
}