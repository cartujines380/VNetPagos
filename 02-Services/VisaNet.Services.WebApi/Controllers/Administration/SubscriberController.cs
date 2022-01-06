using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using VisaNet.Application.Interfaces;
using VisaNet.Common.Exceptions;
using VisaNet.Common.Security.Entities.Enums;
using VisaNet.Common.Security.Filters.WebApiSecurity.AuthorizationFilters;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters;

namespace VisaNet.Services.WebApi.Controllers.Administration
{
    public class SubscriberController : ApiController
    {
        private readonly IServiceSubscriber _serviceSubscriber;

        public SubscriberController(IServiceSubscriber serviceSubscriber)
        {
            _serviceSubscriber = serviceSubscriber;
        }

        [HttpGet]
        [WebApiAuthentication(Actions.SubscriberList)]
        public HttpResponseMessage Get([FromUri] SubscriberFilterDto filterDto)
        {
            var subscribers = _serviceSubscriber.GetDataForTable(filterDto);
            return Request.CreateResponse(HttpStatusCode.OK, subscribers);
        }

        [HttpPut]
        [WebApiAuthentication(Actions.SubscriberCreate)]
        public HttpResponseMessage Put([FromBody] SubscriberDto entity)
        {
            try
            {
                var dto = _serviceSubscriber.Create(new SubscriberDto { Name = entity.Name, Surname = entity.Surname, Email = entity.Email });
            }
            catch (BusinessException e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e.Message);
            }
            catch (FatalException) { }


            return Request.CreateResponse(HttpStatusCode.OK, "bien");
        }

        [HttpPost]
        [WebApiAuthentication(Actions.SubscriberEdit)]
        public HttpResponseMessage Post(string id, SubscriberDto dto)
        {
            try
            {
                dto.Id = new Guid(id);
                _serviceSubscriber.Edit(dto);
            }
            catch (BusinessException e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e.Message);
            }
            catch (FatalException) { }


            return Request.CreateResponse(HttpStatusCode.OK, "bien");
        }

        [HttpDelete]
        [WebApiAuthentication(Actions.SubscriberDelete)]
        public HttpResponseMessage Delete(Guid id)
        {
            try
            {
                _serviceSubscriber.Delete(id);
            }
            catch (BusinessException e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e.Message);
            }
            catch (FatalException) { }


            return Request.CreateResponse(HttpStatusCode.OK, "bien");
        }

        [HttpPost]
        [WebApiAuthentication(Actions.SubscriberList)]
        public HttpResponseMessage GetDashboardData([FromBody] ReportsDashboardFilterDto filterDto)
        {
            var subscribers = _serviceSubscriber.GetDashboardData(filterDto);
            return Request.CreateResponse(HttpStatusCode.OK, subscribers);
        }

        //nuevo
        [HttpPost]
        [WebApiAuthentication(Actions.SubscriberList)]
        public HttpResponseMessage GetDashboardDataCount([FromBody] ReportsDashboardFilterDto filterDto)
        {
            var subscribers = _serviceSubscriber.GetDashboardDataCount(filterDto);
            return Request.CreateResponse(HttpStatusCode.OK, subscribers);
        }
    }
}
