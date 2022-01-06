using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using VisaNet.Application.Interfaces;
using VisaNet.Common.Security.Entities.Enums;
using VisaNet.Common.Security.Filters.WebApiSecurity.AuthorizationFilters;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters;

namespace VisaNet.Services.WebApi.Controllers.Administration
{
    public class ContactController : ApiController
    {
        private readonly IServiceContact _serviceContact;

        public ContactController(IServiceContact serviceContact)
        {
            _serviceContact = serviceContact;
        }

        [HttpGet]
        [WebApiAuthentication(Actions.ContactList)]
        public HttpResponseMessage Get([FromUri] ContactFilterDto filterDto)
        {
            var contacts = _serviceContact.GetDataForTable(filterDto);
            return Request.CreateResponse(HttpStatusCode.OK, contacts);
        }

        [HttpGet]
        [WebApiAuthentication(Actions.ContactDetails)]
        public HttpResponseMessage Get(Guid id)
        {
            var con = _serviceContact.GetById(id, contact => contact.UserTook);
            return Request.CreateResponse(HttpStatusCode.OK, con);
        }

        [HttpPut]
        [WebApiAuthentication(Actions.ContactCreate)]
        public HttpResponseMessage Put([FromBody] ContactDto entity)
        {

            var dto = _serviceContact.Create(new ContactDto { Name = entity.Name, Surname = entity.Surname, Email = entity.Email, QueryType = entity.QueryType, Subject = entity.Subject, Message = entity.Message });


            return Request.CreateResponse(HttpStatusCode.OK, "bien");
        }

        [HttpPost]
        [WebApiAuthentication(Actions.ContactEdit)]
        public HttpResponseMessage Post(Guid id, [FromBody] ContactDto dto)
        {
            dto.Id = id;
            _serviceContact.Edit(dto);

            return Request.CreateResponse(HttpStatusCode.OK, "bien");
        }

        [HttpDelete]
        [WebApiAuthentication(Actions.ContactDelete)]
        public HttpResponseMessage Delete(Guid id)
        {
            _serviceContact.Delete(id);

            return Request.CreateResponse(HttpStatusCode.OK, "bien");
        }

        [HttpPost]
        [WebApiAuthentication(Actions.ContactList)]
        public HttpResponseMessage GetDashboardData([FromBody] ReportsDashboardFilterDto filterDto)
        {
            var contacts = _serviceContact.GetDashboardData(filterDto);
            return Request.CreateResponse(HttpStatusCode.OK, contacts);
        }

        //nuevo
        [HttpPost]
        [WebApiAuthentication(Actions.ContactList)]
        public HttpResponseMessage GetDashboardDataCount([FromBody] ReportsDashboardFilterDto filterDto)
        {
            var contacts = _serviceContact.GetDashboardDataCount(filterDto);
            return Request.CreateResponse(HttpStatusCode.OK, contacts);
        }
    }
}
