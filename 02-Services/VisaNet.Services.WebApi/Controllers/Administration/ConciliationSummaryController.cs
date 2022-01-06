using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using VisaNet.Application.Interfaces;
using VisaNet.Common.Security.Entities.Enums;
using VisaNet.Common.Security.Filters.WebApiSecurity.AuthorizationFilters;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters;

namespace VisaNet.Services.WebApi.Controllers.Administration
{
    public class ConciliationSummaryController : ApiController
    {
        private readonly IServiceConciliationSummary _serviceConciliationSummary;
        private readonly IServiceConciliationBanred _serviceConciliationBanred;
        private readonly IServiceConciliationSistarbanc _serviceConciliationSistarbanc;
        private readonly IServiceConciliationSucive _serviceConciliationSucive;
        private readonly IServiceConciliationCybersource _serviceConciliationCybersource;
        private readonly IServiceConciliationVisanet _serviceConciliationVisanet;
        private readonly IServiceConciliationVisanetCallback _serviceConciliationVisanetCallback;

        public ConciliationSummaryController(IServiceConciliationSummary serviceConciliationSummary, IServiceConciliationBanred serviceConciliationBanred,
            IServiceConciliationSistarbanc serviceConciliationSistarbanc, IServiceConciliationCybersource serviceConciliationCybersource, IServiceConciliationSucive serviceConciliationSucive,
            IServiceConciliationVisanet serviceConciliationVisanet, IServiceConciliationVisanetCallback serviceConciliationVisanetCallback)
        {
            _serviceConciliationSummary = serviceConciliationSummary;
            _serviceConciliationBanred = serviceConciliationBanred;
            _serviceConciliationSistarbanc = serviceConciliationSistarbanc;
            _serviceConciliationCybersource = serviceConciliationCybersource;
            _serviceConciliationSucive = serviceConciliationSucive;
            _serviceConciliationVisanet = serviceConciliationVisanet;
            _serviceConciliationVisanetCallback = serviceConciliationVisanetCallback;
        }

        [HttpPost]
        [WebApiAuthentication(Actions.ReportsConciliationDetails)]
        public HttpResponseMessage Post(Guid id, [FromBody] ConciliationSummaryDto dto)
        {
            dto.Id = id;
            _serviceConciliationSummary.Edit(dto);

            return Request.CreateResponse(HttpStatusCode.OK, "bien");
        }

        [HttpGet]
        [WebApiAuthentication(Actions.ReportsConciliationDetails)]
        public HttpResponseMessage GetConciliationSummary(Guid id)
        {
            var conciliation = _serviceConciliationSummary.GetById(id);
            return Request.CreateResponse(HttpStatusCode.OK, conciliation);
        }

        [HttpGet]
        [WebApiAuthentication(Actions.ReportsConciliationDetails)]
        public HttpResponseMessage GetConciliationBanred(Guid id)
        {
            var conciliation = _serviceConciliationBanred.GetById(id);
            return Request.CreateResponse(HttpStatusCode.OK, conciliation);
        }

        [HttpGet]
        [WebApiAuthentication(Actions.ReportsConciliationDetails)]
        public HttpResponseMessage GetConciliationSistarbanc(Guid id)
        {
            var conciliation = _serviceConciliationSistarbanc.GetById(id);
            return Request.CreateResponse(HttpStatusCode.OK, conciliation);
        }
        [HttpGet]
        [WebApiAuthentication(Actions.ReportsConciliationDetails)]
        public HttpResponseMessage GetConciliationSucive(Guid id)
        {
            var conciliation = _serviceConciliationSucive.GetById(id);
            return Request.CreateResponse(HttpStatusCode.OK, conciliation);
        }
        [HttpGet]
        [WebApiAuthentication(Actions.ReportsConciliationDetails)]
        public HttpResponseMessage GetConciliationCybersource(Guid id)
        {
            var conciliation = _serviceConciliationCybersource.GetById(id);
            return Request.CreateResponse(HttpStatusCode.OK, conciliation);
        }

        [HttpGet]
        [WebApiAuthentication(Actions.ReportsConciliationDetails)]
        public HttpResponseMessage GetConciliationVisanet(Guid id)
        {
            var conciliation = _serviceConciliationVisanet.GetById(id);
            return Request.CreateResponse(HttpStatusCode.OK, conciliation);
        }

        [HttpGet]
        [WebApiAuthentication(Actions.ReportsConciliationDetails)]
        public HttpResponseMessage GetConciliationBatch(Guid id)
        {
            var conciliation = _serviceConciliationVisanetCallback.GetById(id);
            return Request.CreateResponse(HttpStatusCode.OK, conciliation);
        }

        [HttpPost]
        [WebApiAuthentication(Actions.ReportsConciliationDetails)]
        public HttpResponseMessage GetConciliationSummary([FromBody] ReportsConciliationFilterDto filtersDto)
        {
            var conciliation = _serviceConciliationSummary.GetDataForTable(filtersDto);
            return Request.CreateResponse(HttpStatusCode.OK, conciliation);
        }

    }
}