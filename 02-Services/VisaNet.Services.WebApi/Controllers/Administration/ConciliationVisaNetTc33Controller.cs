using System.Net;
using System.Net.Http;
using System.Web.Http;
using VisaNet.Application.Interfaces;
using VisaNet.Common.Security.Entities.Enums;
using VisaNet.Common.Security.Filters.WebApiSecurity.AuthorizationFilters;
using VisaNet.Domain.EntitiesDtos;

namespace VisaNet.Services.WebApi.Controllers.Administration
{
    public class ConciliationVisaNetTc33Controller : ApiController
    {
        private readonly IServiceConciliationVisanet _serviceConciliationVisanet;

        public ConciliationVisaNetTc33Controller(IServiceConciliationVisanet serviceConciliationVisanet)
        {
            _serviceConciliationVisanet = serviceConciliationVisanet;
        }

        [HttpPut]
        [WebApiAuthentication(Actions.ReportsTc33)]
        public HttpResponseMessage Create(ConciliationVisanetDto dto)
        {
            var created = _serviceConciliationVisanet.Create(dto, true);
            return Request.CreateResponse(HttpStatusCode.OK, created);
        }

        [HttpPost]
        [WebApiAuthentication(Actions.ReportsTc33)]
        public HttpResponseMessage Edit(ConciliationVisanetDto dto)
        {
            _serviceConciliationVisanet.Edit(dto);
            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}