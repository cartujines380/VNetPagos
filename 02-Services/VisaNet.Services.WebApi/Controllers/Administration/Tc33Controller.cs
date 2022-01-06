using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using VisaNet.Application.Interfaces;
using VisaNet.Common.Logging.Entities;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters;

namespace VisaNet.Services.WebApi.Controllers.Administration
{
    public class Tc33Controller : ApiController
    {
        private readonly IServiceTc33 _serviceTc33;


        public Tc33Controller(IServiceTc33 serviceReports)
        {
            _serviceTc33 = serviceReports;
        }

        [HttpGet]
        public HttpResponseMessage GetDataForTable([FromUri] ReportsTc33FilterDto filtersDto)
        {
            var data = _serviceTc33.GetDataForTable(filtersDto);
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        [HttpGet]
        public HttpResponseMessage GetDataForTableCount([FromUri] ReportsTc33FilterDto filtersDto)
        {
            var data = _serviceTc33.GetDataForTableCount(filtersDto);
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        [HttpPut]
        public HttpResponseMessage CreateProcess([FromBody] Tc33Dto dto)
        {
            var result = _serviceTc33.Create(dto, true);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [HttpPost]
        public HttpResponseMessage Edit([FromBody] Tc33Dto dto)
        {
            _serviceTc33.Edit(dto);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpPost]
        public HttpResponseMessage GetTC33([FromBody] Guid id)
        {
            var file = _serviceTc33.GetById(id);

            return Request.CreateResponse(HttpStatusCode.OK, file);
        }

        [HttpPost]
        public HttpResponseMessage GetTC33Transactions([FromBody] Guid id)
        {
            var trasactions = _serviceTc33.GetTC33Transactions(id);

            return Request.CreateResponse(HttpStatusCode.OK, trasactions);
        }

        [HttpPost]
        public HttpResponseMessage DownloadDetails([FromBody] Guid id)
        {
            byte[] arrBytes;
            string mimeType;

            _serviceTc33.DownloadDetails(id, out arrBytes, out mimeType);

            return Request.CreateResponse(HttpStatusCode.OK, arrBytes);
        }

        [HttpGet]
        public bool WasAlreadyProccessed(string requestId)
        {
            return _serviceTc33.WasAlreadyProccessed(requestId);
        }

        [HttpGet]
        public LogDto GetLogFromDb(string requestId)
        {
            return _serviceTc33.GetLogFromDb(requestId, 1);
        }

    }
}