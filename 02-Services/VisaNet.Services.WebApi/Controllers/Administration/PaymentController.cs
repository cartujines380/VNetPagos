using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using VisaNet.Application.Interfaces;
using VisaNet.Common.Logging.NLog;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Utilities.Cybersource;

namespace VisaNet.Services.WebApi.Controllers.Administration
{
    public class PaymentController : ApiController
    {
        private readonly IServicePayment _servicePayment;

        public PaymentController(IServicePayment servicePayment)
        {
            _servicePayment = servicePayment;
        }

        [HttpGet]
        public HttpResponseMessage GetByTransactionNumber(string transactionNumber)
        {
            var payment = _servicePayment
                .AllNoTracking(null, p => p.TransactionNumber == transactionNumber, p => p.PaymentIdentifier, p => p.Bills, p => p.AnonymousUser, p => p.RegisteredUser, p => p.DiscountObj)
                .FirstOrDefault();
            return Request.CreateResponse(HttpStatusCode.OK, payment);
        }

        [HttpGet]
        public HttpResponseMessage GetFullPaymentByTransactionNumber(string transactionNumber)
        {
            var payment = _servicePayment
                .AllNoTracking(null, p => p.TransactionNumber == transactionNumber,
                p => p.PaymentIdentifier, p => p.Bills, p => p.AnonymousUser, p => p.RegisteredUser, p => p.DiscountObj,
                p => p.Gateway, p => p.Service, p => p.Service.ServiceGateways, p => p.Service.ServiceGateways.Select(y => y.Gateway))
                .FirstOrDefault();
            return Request.CreateResponse(HttpStatusCode.OK, payment);
        }

        [HttpGet]
        public HttpResponseMessage Get()
        {
            var payments = _servicePayment.GetDataForTable();
            return Request.CreateResponse(HttpStatusCode.OK, payments);
        }

        [HttpGet]
        public HttpResponseMessage ReportsTransactionsDataFromDbView([FromUri] ReportsTransactionsFilterDto filterDto)
        {
            var payments = _servicePayment.ReportsTransactionsDataFromDbView(filterDto);
            return Request.CreateResponse(HttpStatusCode.OK, payments);
        }

        [HttpGet]
        public HttpResponseMessage ReportsTransactionsDataCount([FromUri] ReportsTransactionsFilterDto filterDto)
        {
            var payments = _servicePayment.ReportsTransactionsDataCount(filterDto);
            return Request.CreateResponse(HttpStatusCode.OK, payments);
        }

        [HttpPost]
        public HttpResponseMessage GetDashboardData([FromBody] ReportsDashboardFilterDto filterDto)
        {
            var payments = _servicePayment.GetDashboardData(filterDto);
            NLogLogger.LogEvent(NLogType.Info, "API - Cantidad de payments " + payments.Count());
            return Request.CreateResponse(HttpStatusCode.OK, payments);
        }

        [HttpGet]
        public HttpResponseMessage DownloadTicket([FromUri] Guid id, [FromUri] string transactionNumber, [FromUri] Guid userId)
        {
            byte[] arrBytes;
            string mimeType;
            _servicePayment.GeneratePaymentTicket(id, transactionNumber, userId, out arrBytes, out mimeType);

            return Request.CreateResponse(HttpStatusCode.OK, arrBytes);
        }

        [HttpGet]
        public HttpResponseMessage TestCyberSourcePayment([FromUri] Guid serviceId)
        {
            var resp = _servicePayment.TestCyberSourcePayment(serviceId);
            return Request.CreateResponse(HttpStatusCode.OK, resp);
        }

        [HttpPost]
        public HttpResponseMessage TestCyberSourceCancelPayment(Guid id, [FromBody] CyberSourceOperationData cSourceOperationData)
        {
            var resp = _servicePayment.TestCyberSourceCancelPayment(id, cSourceOperationData);
            return Request.CreateResponse(HttpStatusCode.OK, resp);
        }

        [HttpGet]
        public HttpResponseMessage TestCyberSourceReports([FromUri] Guid serviceId)
        {
            var resp = _servicePayment.TestCyberSourceReports(serviceId);
            return Request.CreateResponse(HttpStatusCode.OK, resp);
        }

        [HttpGet]
        public HttpResponseMessage TestCyberSourceReversal([FromUri] Guid serviceId)
        {
            var resp = _servicePayment.TestCyberSourceReversal(serviceId);
            return Request.CreateResponse(HttpStatusCode.OK, resp);
        }

        [HttpPut]
        public HttpResponseMessage ReversePayment([FromBody] RefundPayment entity)
        {
            _servicePayment.ReversePayment(entity);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpPost]
        public HttpResponseMessage CancelPayment([FromBody] CancelTrnsDto entity)
        {
            var result = _servicePayment.CancelPaymentDone(entity.PaymentId, entity.TransactionNumber, entity.Notify);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [HttpPost]
        public HttpResponseMessage DeleteCardInCybersource([FromBody] DeleteTrnsDto entity)
        {
            var cardDeleted = _servicePayment.DeleteCardInCybersource(entity.UserId, entity.CardId, entity.TransactionNumber);
            return Request.CreateResponse(HttpStatusCode.OK, cardDeleted);
        }

    }
}