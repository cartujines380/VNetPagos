using System;
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
    public class ConciliationVNPController : ApiController
    {
        private readonly IServiceConciliationBanred _serviceConciliationBanred;
        private readonly IServiceConciliationSistarbanc _serviceConciliationSistarbanc;
        private readonly IServiceConciliationSucive _serviceConciliationSucive;
        private readonly IServiceConciliationCybersource _serviceConciliationCybersource;
        private readonly IServiceConciliationVisanetCallback _serviceConciliationVisanetCallback;
        private readonly IServiceConciliationRun _serviceConciliationRun;

        public ConciliationVNPController(IServiceConciliationBanred serviceConciliationBanred, IServiceConciliationSistarbanc serviceConciliationSistarbanc,
            IServiceConciliationCybersource serviceConciliationCybersource, IServiceConciliationSucive serviceConciliationSucive,
            IServiceConciliationVisanetCallback serviceConciliationVisanetCallback, IServiceConciliationRun serviceConciliationRun)
        {
            _serviceConciliationBanred = serviceConciliationBanred;
            _serviceConciliationSistarbanc = serviceConciliationSistarbanc;
            _serviceConciliationCybersource = serviceConciliationCybersource;
            _serviceConciliationSucive = serviceConciliationSucive;
            _serviceConciliationVisanetCallback = serviceConciliationVisanetCallback;
            _serviceConciliationRun = serviceConciliationRun;
        }

        [HttpPost]
        [WebApiAuthentication(Actions.ConciliationVNP)]
        public HttpResponseMessage RunConciliation(RunConciliationDto dto)
        {
            switch (dto.App)
            {
                case ConciliationAppDto.Banred:
                    _serviceConciliationBanred.SingleFileConciliation(dto.FileName);
                    break;

                case ConciliationAppDto.Batch:
                    _serviceConciliationVisanetCallback.SingleFileConciliation(dto.FileName);
                    break;

                case ConciliationAppDto.CyberSource:
                    _serviceConciliationCybersource.GetConciliation(new ReportsConciliationFilterDto { From = dto.Date, To = dto.DateTo });
                    break;

                case ConciliationAppDto.Sistarbanc:
                    _serviceConciliationSistarbanc.SingleFileConciliation(dto.FileName);
                    break;

                case ConciliationAppDto.Sucive:
                    _serviceConciliationSucive.GetConciliation(new ReportsConciliationFilterDto { From = dto.Date, To = dto.DateTo });
                    break;
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpGet]
        [WebApiAuthentication(Actions.ConciliationVNP)]
        public HttpResponseMessage GetConciliationRunReport([FromUri] ReportsConciliationRunFilterDto filter)
        {
            var report = _serviceConciliationRun.GetConciliationRunReport(filter);
            return Request.CreateResponse(HttpStatusCode.OK, report);
        }

        [HttpGet]
        [WebApiAuthentication(Actions.ConciliationVNP)]
        public HttpResponseMessage GetConciliationRunReportCount([FromUri] ReportsConciliationRunFilterDto filter)
        {
            var count = _serviceConciliationRun.GetConciliationRunReportCount(filter);
            return Request.CreateResponse(HttpStatusCode.OK, count);
        }

        [HttpGet]
        [WebApiAuthentication(Actions.ConciliationVNP)]
        public HttpResponseMessage GetConciliationRun([FromUri] Guid id)
        {
            var obj = _serviceConciliationRun.GetById(id);
            return Request.CreateResponse(HttpStatusCode.OK, obj);
        }

    }
}