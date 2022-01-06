using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using VisaNet.Application.Interfaces;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Services.WebApi.Controllers.Administration
{
    public class ServiceValidatorController : ApiController
    {
        private readonly IServiceServiceValidator _serviceServiceValidator;

        public ServiceValidatorController(IServiceServiceValidator serviceServiceValidator)
        {
            _serviceServiceValidator = serviceServiceValidator;
        }

        [HttpGet]
        public HttpResponseMessage ValidateLinkService(Guid serviceId)
        {
            var ok = _serviceServiceValidator.ValidateLinkService(serviceId);
            return Request.CreateResponse(HttpStatusCode.OK, ok);
        }

        [HttpPost]
        public HttpResponseMessage ValidateLinkService([FromBody] ServiceDto service)
        {
            var ok = _serviceServiceValidator.ValidateLinkService(service);
            return Request.CreateResponse(HttpStatusCode.OK, ok);
        }

        [HttpGet]
        public HttpResponseMessage ValidateVisaNetOnService(Guid serviceId)
        {
            var ok = _serviceServiceValidator.ValidateVisaNetOnService(serviceId);
            return Request.CreateResponse(HttpStatusCode.OK, ok);
        }

        [HttpPost]
        public HttpResponseMessage ValidateVisaNetOnService([FromBody] ServiceDto service)
        {
            var ok = _serviceServiceValidator.ValidateVisaNetOnService(service);
            return Request.CreateResponse(HttpStatusCode.OK, ok);
        }

        [HttpGet]
        public HttpResponseMessage ValidateDebitService(Guid serviceId)
        {
            var ok = _serviceServiceValidator.ValidateDebitService(serviceId);
            return Request.CreateResponse(HttpStatusCode.OK, ok);
        }

        [HttpPost]
        public HttpResponseMessage ValidateDebitService([FromBody] ServiceDto service)
        {
            var ok = _serviceServiceValidator.ValidateDebitService(service);
            return Request.CreateResponse(HttpStatusCode.OK, ok);
        }

        [HttpGet]
        public HttpResponseMessage ValidateServiceGatewayActive(Guid serviceId, GatewayEnumDto gatewayEnum)
        {
            var ok = _serviceServiceValidator.ValidateServiceGatewayActive(serviceId, gatewayEnum);
            return Request.CreateResponse(HttpStatusCode.OK, ok);
        }

    }
}