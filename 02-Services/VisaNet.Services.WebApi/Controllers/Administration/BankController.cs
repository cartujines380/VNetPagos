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
    public class BankController : ApiController
    {
        private readonly IServiceBank _serviceBank;

        public BankController(IServiceBank serviceBank)
        {
            _serviceBank = serviceBank;
        }

        [HttpGet]
        [WebApiAuthentication(Actions.BankList)]
        public HttpResponseMessage Get([FromUri] BankFilterDto filterDto)
        {
            var bins = _serviceBank.GetDataForTable(filterDto);
            return Request.CreateResponse(HttpStatusCode.OK, bins);
        }

        [HttpGet]
        [WebApiAuthentication(Actions.BankDetails)]
        public HttpResponseMessage Get(Guid id)
        {
            var con = _serviceBank.GetById(id);
            return Request.CreateResponse(HttpStatusCode.OK, con);
        }

        [HttpPut]
        [WebApiAuthentication(Actions.BankCreate)]
        public HttpResponseMessage Put([FromBody] BankDto entity)
        {
            var dto = _serviceBank.Create(entity);
            return Request.CreateResponse(HttpStatusCode.OK, "bien");
        }

        [HttpPost]
        [WebApiAuthentication(Actions.BankEdit)]
        public HttpResponseMessage Post(Guid id, [FromBody] BankDto dto)
        {
            dto.Id = id;
            _serviceBank.Edit(dto);

            return Request.CreateResponse(HttpStatusCode.OK, "bien");
        }

        [HttpDelete]
        [WebApiAuthentication(Actions.BankDelete)]
        public HttpResponseMessage Delete(Guid id)
        {
            _serviceBank.Delete(id);
            return Request.CreateResponse(HttpStatusCode.OK, "bien");
        }

        [HttpPost]
        public HttpResponseMessage GetDataForBankCount([FromBody] BankFilterDto filterDto)
        {
            var users = _serviceBank.GetDataForBankCount(filterDto);
            return Request.CreateResponse(HttpStatusCode.OK, users);
        }
    }
}
