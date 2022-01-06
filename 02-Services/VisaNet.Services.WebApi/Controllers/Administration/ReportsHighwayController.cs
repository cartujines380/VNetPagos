using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using VisaNet.Application.Interfaces;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters;

namespace VisaNet.Services.WebApi.Controllers.Administration
{
    public class ReportsHighwayController : ApiController
    {
        private readonly IServiceHighway _serviceHighway;

        public ReportsHighwayController(IServiceHighway serviceHighway)
        {
            _serviceHighway = serviceHighway;
        }

        [HttpPost]
        public HttpResponseMessage GetHighwayEmailsReports([FromBody] ReportsHighwayEmailFilterDto filtersDto)
        {
            var data = _serviceHighway.GetHighwayEmailsReports(filtersDto);
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        [HttpPost]
        public HttpResponseMessage GetHighwayEmailsReportsCount([FromBody] ReportsHighwayEmailFilterDto filtersDto)
        {
            var count = _serviceHighway.GetHighwayEmailsReportsCount(filtersDto);
            return Request.CreateResponse(HttpStatusCode.OK, count);
        }

        [HttpPost]
        public HttpResponseMessage GetHighwayBillReports([FromBody] ReportsHighwayBillFilterDto filtersDto)
        {
            var data = _serviceHighway.GetHighwayBillReports(filtersDto);
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        [HttpPost]
        public HttpResponseMessage GetHighwayBillReportsCount([FromBody] ReportsHighwayBillFilterDto filtersDto)
        {
            var data = _serviceHighway.GetHighwayBillReportsCount(filtersDto);
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        [HttpPut]
        public HttpResponseMessage ProccessEmailFile([FromBody] HighwayEmailDto entity)
        {
            var errorList = _serviceHighway.ProccessEmailFile(entity);
            return Request.CreateResponse(HttpStatusCode.OK, errorList);
        }

        [HttpPut]
        public HttpResponseMessage ProccessEmailFileExternalSoruce([FromBody] HighwayEmailDto entity)
        {
            var errorList = _serviceHighway.ProccessEmailFileExternalSoruce(entity);
            return Request.CreateResponse(HttpStatusCode.OK, errorList);
        }

        [HttpPost]
        public HttpResponseMessage GetHighwayEmail([FromBody] Guid id)
        {
            var data = _serviceHighway.GetHighwayEmail(id);
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        [HttpPost]
        public HttpResponseMessage SendPost()
        {
            var contentType = Request.Content.Headers.ContentType;
            var contentInString = Request.Content.ReadAsMultipartAsync().GetAwaiter().GetResult();


            //Request.Content = new StringContent(contentInString);
            //Request.Content.Headers.ContentType = contentType;



            return Request.CreateResponse(HttpStatusCode.OK, "OK");
        }

    }
}