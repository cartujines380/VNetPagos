using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using VisaNet.Application.Interfaces;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters;

namespace VisaNet.Services.WebApi.Controllers.Web
{
    public class WebServiceAssosiateController : ApiController
    {
        private readonly IServiceServiceAssosiate _serviceServiceAssosiate;

        public WebServiceAssosiateController(IServiceServiceAssosiate serviceServiceAssosiate)
        {
            _serviceServiceAssosiate = serviceServiceAssosiate;
        }

        [HttpGet]
        public HttpResponseMessage Get([FromUri] ServiceFilterAssosiateDto filterDto)
        {
            //Obtengo solo los servicios asociados habilitados
            var services = _serviceServiceAssosiate.GetDataForTable(filterDto).Where(s => s.Enabled);
            foreach (var s in services)
            {
                WebControllerHelper.LoadServiceReferenceParams(s.ServiceDto);
            }
            return Request.CreateResponse(HttpStatusCode.OK, services);
        }

        [HttpGet]
        public HttpResponseMessage GetDataForFrontList([FromUri] ServiceFilterAssosiateDto filterDto)
        {
            //Obtengo solo los servicios asociados habilitados
            var services = _serviceServiceAssosiate.GetDataForFrontList(filterDto).Where(s => s.Enabled);
            foreach (var s in services)
            {
                WebControllerHelper.LoadServiceReferenceParams(s.ServiceDto);
            }
            return Request.CreateResponse(HttpStatusCode.OK, services);
        }

        [HttpGet]
        public HttpResponseMessage Get(Guid id)
        {
            var cat = _serviceServiceAssosiate.GetById(id, s => s.Service, s => s.RegisteredUser, s => s.NotificationConfig, s => s.AutomaticPayment, s => s.DefaultCard);
            if (cat != null)
            {
                WebControllerHelper.LoadServiceReferenceParams(cat.ServiceDto);
            }
            return Request.CreateResponse(HttpStatusCode.OK, cat);
        }

        [HttpPut]
        public HttpResponseMessage Put([FromBody] ServiceAssociatedDto entity)
        {
            var dto = _serviceServiceAssosiate.Create(entity, true);
            if (dto != null)
            {
                WebControllerHelper.LoadServiceReferenceParams(dto.ServiceDto);
            }
            return Request.CreateResponse(HttpStatusCode.OK, dto);
        }

        [HttpPost]
        public HttpResponseMessage Post(Guid id, [FromBody] ServiceAssociatedDto dto)
        {
            dto.Id = id;
            _serviceServiceAssosiate.Edit(dto);
            return Request.CreateResponse(HttpStatusCode.OK, "bien");
        }

        [HttpPost]
        public HttpResponseMessage AddPayment([FromBody] AutomaticPaymentDto dto)
        {
            _serviceServiceAssosiate.AddAutomaticPayment(dto);
            return Request.CreateResponse(HttpStatusCode.OK, "bien");
        }

        [HttpPost]
        public HttpResponseMessage DeleteService([FromBody] Guid serviceAssociatedId)
        {
            _serviceServiceAssosiate.DeleteService(serviceAssociatedId);
            return Request.CreateResponse(HttpStatusCode.OK, "bien");
        }

        [HttpGet]
        public HttpResponseMessage GetServicesWithAutomaticPayment([FromUri] ServiceFilterAssosiateDto filterDto)
        {
            //Obtengo solo los servicios asociados habilitados
            var services = _serviceServiceAssosiate.GetServicesForAutomaticPayment(filterDto).Where(s => s.Enabled);
            foreach (var s in services)
            {
                WebControllerHelper.LoadServiceReferenceParams(s.ServiceDto);
            }
            return Request.CreateResponse(HttpStatusCode.OK, services);
        }

        [HttpGet]
        public HttpResponseMessage GetServicesActiveAutomaticPayment([FromUri] ServiceFilterAssosiateDto filterDto)
        {
            //Obtengo solo los servicios asociados habilitados
            var services = _serviceServiceAssosiate.GetServicesActiveAutomaticPayment(filterDto).Where(s => s.Enabled);
            foreach (var s in services)
            {
                WebControllerHelper.LoadServiceReferenceParams(s.ServiceDto);
            }
            return Request.CreateResponse(HttpStatusCode.OK, services);
        }

        [HttpPost]
        public HttpResponseMessage DeleteAutomaticPayment([FromBody] Guid serviceAssosiatedId)
        {
            _serviceServiceAssosiate.DeleteAutomaticPayment(serviceAssosiatedId);
            return Request.CreateResponse(HttpStatusCode.OK, "bien");
        }

        [HttpGet]
        public HttpResponseMessage IsServiceAssosiatedToUser(Guid userId, Guid serviceId, string ref1, string ref2, string ref3, string ref4, string ref5, string ref6)
        {
            var result = _serviceServiceAssosiate.IsServiceAssosiatedToUser(userId, serviceId, ref1, ref2, ref3, ref4, ref5, ref6);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [HttpGet]
        public HttpResponseMessage ServiceAssosiatedToUser(Guid userId, Guid serviceId, string ref1, string ref2, string ref3, string ref4, string ref5, string ref6)
        {
            var result = _serviceServiceAssosiate.ServiceAssosiatedToUser(userId, serviceId, ref1, ref2, ref3, ref4, ref5, ref6);
            if (result != null)
            {
                WebControllerHelper.LoadServiceReferenceParams(result.ServiceDto);
            }
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [HttpGet]
        public HttpResponseMessage GetServicesForBills(Guid userId)
        {
            var services = _serviceServiceAssosiate.GetServicesForBills(userId).Where(s => s.Active && s.Enabled);
            foreach (var s in services)
            {
                WebControllerHelper.LoadServiceReferenceParams(s.ServiceDto);
            }
            return Request.CreateResponse(HttpStatusCode.OK, services);
        }

        [HttpPost]
        public HttpResponseMessage EditDescription(Guid id, [FromBody] ServiceAssociatedDto dto)
        {
            dto.Id = id;
            _serviceServiceAssosiate.EditDescription(dto);
            return Request.CreateResponse(HttpStatusCode.OK, "bien");
        }

        [HttpGet]
        public HttpResponseMessage HasAutomaticPaymentCreated(Guid userId)
        {
            var has = _serviceServiceAssosiate.HasAutomaticPaymentCreated(userId);
            return Request.CreateResponse(HttpStatusCode.OK, has);
        }

        [HttpGet]
        public HttpResponseMessage HasAsosiatedService(Guid userId)
        {
            var has = _serviceServiceAssosiate.HasAsosiatedService(userId);
            return Request.CreateResponse(HttpStatusCode.OK, has);
        }

        [HttpPost]
        public HttpResponseMessage CreateOrUpdateDeleted([FromBody] ServiceAssociatedDto entityDto)
        {
            var result = _serviceServiceAssosiate.CreateOrUpdateDeleted(entityDto);
            if (result != null)
            {
                WebControllerHelper.LoadServiceReferenceParams(result.ServiceDto);
            }
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [HttpPost]
        public HttpResponseMessage DeleteCardFromService([FromBody] CardServiceDataDto dto)
        {
            var result = _serviceServiceAssosiate.DeleteCardFromService(dto.ServiceId, dto.CardId, dto.UserId);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [HttpPost]
        public HttpResponseMessage AddCardToService([FromBody] CardServiceDataDto dto)
        {
            var result = _serviceServiceAssosiate.AddCardToService(dto.ServiceId, dto.CardId, Guid.Empty, dto.UserId, dto.OperationId);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [HttpPost]
        public HttpResponseMessage ProccesDataFromCybersource([FromBody] IDictionary<string, string> cybersourceData)
        {
            var result = _serviceServiceAssosiate.ProccesDataFromCybersource(cybersourceData);
            if (result != null && result.ServiceAssociatedDto != null)
            {
                WebControllerHelper.LoadServiceReferenceParams(result.ServiceAssociatedDto.ServiceDto);
            }
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [HttpPost]
        public HttpResponseMessage ProccesDataFromCybersourceForApps([FromBody] IDictionary<string, string> cybersourceData)
        {
            var result = _serviceServiceAssosiate.ProccesDataFromCybersourceForApps(cybersourceData);
            if (result != null && result.CybersourceCreateServiceAssociatedDto != null && result.CybersourceCreateServiceAssociatedDto.ServiceAssociatedDto != null)
            {
                WebControllerHelper.LoadServiceReferenceParams(result.CybersourceCreateServiceAssociatedDto.ServiceAssociatedDto.ServiceDto);
            }
            if (result != null && result.WebhookRegistrationDto != null && result.WebhookRegistrationDto.PaymentDto != null)
            {
                WebControllerHelper.LoadServiceReferenceParams(result.WebhookRegistrationDto.PaymentDto.ServiceDto);
            }
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [HttpPost]
        public HttpResponseMessage AssociateServiceToUserFromCardCreated([FromBody] ServiceAssociatedDto dto)
        {
            var result = _serviceServiceAssosiate.AssociateServiceToUserFromCardCreated(dto);
            if (result != null)
            {
                WebControllerHelper.LoadServiceReferenceParams(result.ServiceDto);
            }
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [HttpGet]
        public HttpResponseMessage GetServiceAssociatedDtoFromIdUserExternal(string idUserExternal, string idApp)
        {
            var result = _serviceServiceAssosiate.GetServiceAssociatedDtoFromIdUserExternal(idUserExternal, idApp);
            if (result != null)
            {
                WebControllerHelper.LoadServiceReferenceParams(result.ServiceDto);
            }
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

    }
}