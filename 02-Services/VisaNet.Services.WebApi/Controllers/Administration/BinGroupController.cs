using System;
using System.Linq;
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
    public class BinGroupController : ApiController
    {

        private readonly IServiceBinGroup _binGroupService;

        public BinGroupController(IServiceBinGroup binGroupService)
        {
            _binGroupService = binGroupService;
        }

        [HttpGet]
        [WebApiAuthentication(Actions.BinsList)]
        public HttpResponseMessage Get([FromUri] BinGroupFilterDto filterDto)
        {
            var bins = _binGroupService.GetDataForTable(filterDto);
            return Request.CreateResponse(HttpStatusCode.OK, bins);
        }

        [HttpGet]
        [WebApiAuthentication(Actions.BinsDetails)]
        public HttpResponseMessage Get(Guid id)
        {
            var binGroup = _binGroupService.GetById(id);//, b => b.Bins.Select(x => x.Bank));
            return Request.CreateResponse(HttpStatusCode.OK, binGroup);
        }

        [HttpPut]
        [WebApiAuthentication(Actions.BinsEdit)]
        public HttpResponseMessage Put([FromBody] BinGroupDto entity)
        {
            _binGroupService.Edit(entity);
            
            return Request.CreateResponse(HttpStatusCode.OK, "bien");
        }

        [HttpPost]
        [WebApiAuthentication(Actions.BinsCreate)]
        public HttpResponseMessage Post([FromBody] BinGroupDto entity)
        {
            _binGroupService.Create(entity);
            
            return Request.CreateResponse(HttpStatusCode.OK, "bien");
        }

        [HttpDelete]
        [WebApiAuthentication(Actions.BinsDelete)]
        public HttpResponseMessage Delete(Guid id)
        {
            _binGroupService.Delete(id);
            return Request.CreateResponse(HttpStatusCode.OK, "bien");
        }

    }
}
