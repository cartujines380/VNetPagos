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
    public class LifApiBillController : ApiController
    {
        private readonly IServiceLifApiBill _serviceLifApiBill;

        public LifApiBillController(IServiceLifApiBill serviceLifApiBill)
        {
            _serviceLifApiBill = serviceLifApiBill;
        }

        [HttpGet]
        [WebApiAuthentication(Actions.ReportsLifApiBillList)]
        public HttpResponseMessage Get([FromUri] LifApiBillFilterDto filterDto)
        {
            var bins = _serviceLifApiBill.GetDataForTable(filterDto);
            return Request.CreateResponse(HttpStatusCode.OK, bins);
        }

        [HttpGet]
        [WebApiAuthentication(Actions.ReportsLifApiBillDetails)]
        public HttpResponseMessage Get(Guid id)
        {
            var con = _serviceLifApiBill.GetById(id);
            return Request.CreateResponse(HttpStatusCode.OK, con);
        }

        [HttpPost]
        [WebApiAuthentication(Actions.ReportsLifApiBillList)]
        public HttpResponseMessage GetDataForLifApiBillCount([FromBody] LifApiBillFilterDto filterDto)
        {
            var users = _serviceLifApiBill.GetDataForLifApiBillCount(filterDto);
            return Request.CreateResponse(HttpStatusCode.OK, users);
        }
    }
}
