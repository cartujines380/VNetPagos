using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using VisaNet.Application.Interfaces;
using VisaNet.Domain.EntitiesDtos.TableFilters;

namespace VisaNet.Services.WebApi.Controllers.Administration
{
    public class IntegrationController : ApiController
    {
        private readonly IServiceWsBillPaymentOnline _serviceWsBillPaymentOnline;
        private readonly IServiceWsBillQuery _serviceWsBillQuery;
        private readonly IServiceWsCommerceQuery _serviceWsCommerceQuery;
        private readonly IServiceWsPaymentCancellation _serviceWsPaymentCancellation;
        private readonly IServiceWsCardRemove _serviceWsCardRemove;

        public IntegrationController(IServiceWsBillPaymentOnline serviceWsBillPaymentOnline, IServiceWsBillQuery serviceWsBillQuery, IServiceWsCommerceQuery serviceWsCommerceQuery, IServiceWsPaymentCancellation serviceWsPaymentCancellation, IServiceWsCardRemove serviceWsCardRemove)
        {
            _serviceWsBillPaymentOnline = serviceWsBillPaymentOnline;
            _serviceWsBillQuery = serviceWsBillQuery;
            _serviceWsCommerceQuery = serviceWsCommerceQuery;
            _serviceWsPaymentCancellation = serviceWsPaymentCancellation;
            _serviceWsCardRemove = serviceWsCardRemove;
        }

        [HttpGet]
        public HttpResponseMessage GetBillQueriesForTable([FromUri] ReportsIntegrationFilterDto filterDto)
        {
            var dtos = _serviceWsBillQuery.GetBillQueriesForTable(filterDto);
            return Request.CreateResponse(HttpStatusCode.OK, dtos);
        }

        [HttpGet]
        public HttpResponseMessage GetBillQueriesForTableCount([FromUri] ReportsIntegrationFilterDto filterDto)
        {
            var count = _serviceWsBillQuery.GetBillQueriesForTableCount(filterDto);
            return Request.CreateResponse(HttpStatusCode.OK, count);
        }

        [HttpGet]
        public HttpResponseMessage GetBillPaymentsOnlineForTable([FromUri] ReportsIntegrationFilterDto filterDto)
        {
            var dtos = _serviceWsBillPaymentOnline.GetBillPaymentsOnlineForTable(filterDto);
            return Request.CreateResponse(HttpStatusCode.OK, dtos);
        }

        [HttpGet]
        public HttpResponseMessage GetBillPaymentsOnlineForTableCount([FromUri] ReportsIntegrationFilterDto filterDto)
        {
            var count = _serviceWsBillPaymentOnline.GetBillPaymentsOnlineForTableCount(filterDto);
            return Request.CreateResponse(HttpStatusCode.OK, count);
        }

        [HttpGet]
        public HttpResponseMessage GetCommerceQueriesForTable([FromUri] ReportsIntegrationFilterDto filterDto)
        {
            var dtos = _serviceWsCommerceQuery.GetCommerceQueriesForTable(filterDto);
            return Request.CreateResponse(HttpStatusCode.OK, dtos);
        }

        [HttpGet]
        public HttpResponseMessage GetCommerceQueriesForTableCount([FromUri] ReportsIntegrationFilterDto filterDto)
        {
            var count = _serviceWsCommerceQuery.GetCommerceQueriesForTableCount(filterDto);
            return Request.CreateResponse(HttpStatusCode.OK, count);
        }

        [HttpGet]
        public HttpResponseMessage GetPaymentCancellationsForTable([FromUri] ReportsIntegrationFilterDto filterDto)
        {
            var dtos = _serviceWsPaymentCancellation.GetPaymentCancellationsForTable(filterDto);
            return Request.CreateResponse(HttpStatusCode.OK, dtos);
        }

        [HttpGet]
        public HttpResponseMessage GetPaymentCancellationsForTableCount([FromUri] ReportsIntegrationFilterDto filterDto)
        {
            var count = _serviceWsPaymentCancellation.GetPaymentCancellationsForTableCount(filterDto);
            return Request.CreateResponse(HttpStatusCode.OK, count);
        }

        [HttpGet]
        public HttpResponseMessage GetCardRemovesForTable([FromUri] ReportsIntegrationFilterDto filterDto)
        {
            var dtos = _serviceWsCardRemove.GetCardRemovesForTable(filterDto);
            return Request.CreateResponse(HttpStatusCode.OK, dtos);
        }

        [HttpGet]
        public HttpResponseMessage GetCardRemovesForTableCount([FromUri] ReportsIntegrationFilterDto filterDto)
        {
            var count = _serviceWsCardRemove.GetCardRemovesForTableCount(filterDto);
            return Request.CreateResponse(HttpStatusCode.OK, count);
        }

        [HttpGet]
        public HttpResponseMessage GetWsBillPaymentOnline([FromUri] Guid id)
        {
            var dto = _serviceWsBillPaymentOnline.GetById(id, x => x.Payment);
            return Request.CreateResponse(HttpStatusCode.OK, dto);
        }

    }
}