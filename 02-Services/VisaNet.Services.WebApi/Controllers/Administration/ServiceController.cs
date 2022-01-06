using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using VisaNet.Application.Interfaces;
using VisaNet.Common.Security.Entities.Enums;
using VisaNet.Common.Security.Filters.WebApiSecurity.AuthorizationFilters;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Domain.EntitiesDtos.TableFilters;

namespace VisaNet.Services.WebApi.Controllers.Administration
{
    public class ServiceController : ApiController
    {
        private readonly IServiceService _serviceService;

        public ServiceController(IServiceService serviceService)
        {
            _serviceService = serviceService;
        }

        [HttpGet]
        [WebApiAuthentication(Actions.ServiceList)]
        public HttpResponseMessage Get([FromUri] ServiceFilterDto filterDto)
        {
            var services = _serviceService.GetDataForTable(filterDto);
            return Request.CreateResponse(HttpStatusCode.OK, services);
        }

        [HttpGet]
        [WebApiAuthentication(Actions.ServiceList)]
        public HttpResponseMessage GetDataForList(Guid serviceId, bool container)
        {
            var services = _serviceService.GetDataForList(serviceId, container);
            return Request.CreateResponse(HttpStatusCode.OK, services);
        }

        [HttpGet]
        [WebApiAuthentication(Actions.ServiceList)]
        public HttpResponseMessage Get(Guid id)
        {
            var serv = _serviceService.All(null, s => s.Id == id,
                s => s.ServiceCategory, s => s.ServiceGateways, s => s.ServiceContainer, s => s.ServiceGateways.Select(g => g.Gateway),
                s => s.HighwayEnableEmails, s => s.BinGroups)
                .FirstOrDefault();
            return Request.CreateResponse(HttpStatusCode.OK, serv);
        }

        [HttpPut]
        [WebApiAuthentication(Actions.ServiceCreate)]
        public HttpResponseMessage Put([FromBody] ServiceDto entity)
        {
            var dto = _serviceService.Create(entity);
            return Request.CreateResponse(HttpStatusCode.OK, "bien");
        }

        [HttpPost]
        [WebApiAuthentication(Actions.ServiceEdit)]
        public HttpResponseMessage Post(Guid id, [FromBody] ServiceDto dto)
        {
            dto.Id = id;
            _serviceService.Edit(dto);
            return Request.CreateResponse(HttpStatusCode.OK, "bien");
        }

        [HttpDelete]
        [WebApiAuthentication(Actions.ServiceDelete)]
        public HttpResponseMessage Delete(Guid id)
        {
            _serviceService.Delete(id);

            return Request.CreateResponse(HttpStatusCode.OK, "bien");
        }

        [HttpDelete]
        [WebApiAuthentication(Actions.ServiceEdit)]
        public HttpResponseMessage ChangeStatus(Guid id)
        {
            _serviceService.ChangeStatus(id);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpGet]
        [WebApiAuthentication(Actions.ServiceList)]
        public HttpResponseMessage GetServicesLigthWithoutChildens(Guid? containerId = null, GatewayEnumDto? gatewayEnumDto = null)
        {
            var services = _serviceService.GetServicesLigthWithoutChildens(containerId, gatewayEnumDto);
            return Request.CreateResponse(HttpStatusCode.OK, services);
        }

        [HttpGet]
        [WebApiAuthentication(Actions.ServiceList)]
        public HttpResponseMessage GetServicesFromContainer(Guid containerId)
        {
            var services = _serviceService.GetServicesFromContainer(containerId);
            return Request.CreateResponse(HttpStatusCode.OK, services);
        }

        [HttpGet]
        [WebApiAuthentication(Actions.ReportsUsersVonList)]
        public HttpResponseMessage GetDataForReportsUsersVon([FromUri] ReportsUserVonFilterDto filter)
        {
            var services = _serviceService.GetDataForReportsUsersVon(filter);
            return Request.CreateResponse(HttpStatusCode.OK, services);
        }

        [HttpGet]
        [WebApiAuthentication(Actions.ReportsUsersVonList)]
        public HttpResponseMessage GetDataForReportsUsersVonCount([FromUri] ReportsUserVonFilterDto filter)
        {
            var count = _serviceService.GetDataForReportsUsersVonCount(filter);
            return Request.CreateResponse(HttpStatusCode.OK, count);
        }

        [HttpGet]
        [WebApiAuthentication(Actions.ReportsUsersVonList)]
        public HttpResponseMessage GetVonUsersCards(Guid userId, Guid serviceId)
        {
            var cards = _serviceService.GetVonUsersCards(userId, serviceId);
            return Request.CreateResponse(HttpStatusCode.OK, cards);
        }
    }
}