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
    public class BinsController : ApiController
    {
        private readonly IServiceBin _serviceBin;
        private readonly IServiceBank _serviceBank;

        public BinsController(IServiceBin serviceBin, IServiceBank serviceBank)
        {
            _serviceBin = serviceBin;
            _serviceBank = serviceBank;
        }

        [HttpGet]
        [WebApiAuthentication(Actions.BinsList)]
        public HttpResponseMessage Get([FromUri] BinFilterDto filterDto)
        {
            var bins = _serviceBin.GetDataForTable(filterDto);
            return Request.CreateResponse(HttpStatusCode.OK, bins);
        }

        [HttpGet]
        [WebApiAuthentication(Actions.BinsList)]
        public HttpResponseMessage GetDataForTableCount([FromUri] BinFilterDto filterDto)
        {
            var count = _serviceBin.GetDataForTableCount(filterDto);
            return Request.CreateResponse(HttpStatusCode.OK, count);
        }

        [HttpGet]
        [WebApiAuthentication(Actions.BinsDetails)]
        public HttpResponseMessage Get(Guid id)
        {
            var con = _serviceBin.GetById(id, b => b.Gateway, b => b.BinGroups, b => b.BinAuthorizationAmountTypeList, b => b.AffiliationCard);
            return Request.CreateResponse(HttpStatusCode.OK, con);
        }

        [HttpPut]
        [WebApiAuthentication(Actions.BinsCreate)]
        public HttpResponseMessage Put([FromBody] BinDto entity)
        {
            var dto = _serviceBin.Create(entity);
            return Request.CreateResponse(HttpStatusCode.OK, "bien");
        }

        [HttpPost]
        [WebApiAuthentication(Actions.BinsEdit)]
        public HttpResponseMessage Post(Guid id, [FromBody] BinDto dto)
        {
            dto.Id = id;
            _serviceBin.Edit(dto);

            return Request.CreateResponse(HttpStatusCode.OK, "bien");
        }

        [HttpDelete]
        [WebApiAuthentication(Actions.BinsDelete)]
        public HttpResponseMessage Delete(Guid id)
        {
            _serviceBin.Delete(id);
            return Request.CreateResponse(HttpStatusCode.OK, "bien");
        }
        [HttpGet]
        public HttpResponseMessage GetBanks()
        {
            var banks = _serviceBank.GetDataForTable();
            return Request.CreateResponse(HttpStatusCode.OK, banks);
        }

        [HttpDelete]
        [WebApiAuthentication(Actions.BinsEdit)]
        public HttpResponseMessage ChangeStatus(Guid id)
        {
            _serviceBin.ChangeStatus(id);
            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}
