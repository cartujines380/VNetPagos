using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using VisaNet.Application.Interfaces;
using VisaNet.Common.Exceptions;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Utilities.Cybersource;

namespace VisaNet.Services.WebApi.Controllers.Web
{
    public class WebPaymentController : ApiController
    {
        private readonly IServicePayment _servicePayment;

        public WebPaymentController(IServicePayment servicePayment)
        {
            _servicePayment = servicePayment;
        }

        [HttpPost]
        public HttpResponseMessage GetPayments([FromBody] PaymentFilterDto filterDto)
        {
            var payments = _servicePayment.GetDataForFromList(filterDto);
            if (payments != null)
            {
                foreach (var paymentDto in payments)
                {
                    WebControllerHelper.LoadServiceReferenceParams(paymentDto.ServiceDto);
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, payments);
        }

        [HttpPut]
        public HttpResponseMessage Put([FromBody] PaymentDto entity)
        {
            var dto = _servicePayment.Create(entity, true);
            if (dto != null)
            {
                WebControllerHelper.LoadServiceReferenceParams(dto.ServiceDto);
            }
            return Request.CreateResponse(HttpStatusCode.OK, dto);
        }

        [HttpPut]
        public HttpResponseMessage NotifyPayment([FromBody] IDictionary<string, string> entity)
        {
            var dto = _servicePayment.NotifyGateways(entity);
            if (dto != null && dto.NewPaymentDto != null)
            {
                WebControllerHelper.LoadServiceReferenceParams(dto.NewPaymentDto.ServiceDto);
            }
            return Request.CreateResponse(HttpStatusCode.OK, dto);
        }

        [HttpPut]
        public HttpResponseMessage CancelPaymentCybersource([FromBody] CancelPayment entity)
        {
            _servicePayment.CancelPaymentCybersource(entity);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpGet]
        public HttpResponseMessage DownloadTicket([FromUri] Guid id, [FromUri] string transactionNumber, [FromUri] Guid userId)
        {
            byte[] arrBytes;
            string mimeType;

            //CONTROL A ESTE NIVEL PORQUE EN EL SERVICIO SI PUEDE SER VACIO. LLAMADAS INTERNAS
            if (userId == Guid.Empty || userId == null)
                throw new FatalException(CodeExceptions.USER_NOT_EXIST);

            _servicePayment.GeneratePaymentTicket(id, transactionNumber, userId, out arrBytes, out mimeType);
            return Request.CreateResponse(HttpStatusCode.OK, arrBytes);
        }

        [HttpGet]
        public HttpResponseMessage SendTicketByEmail([FromUri] Guid id, [FromUri] string transactionNumber, [FromUri] Guid userId)
        {
            //CONTROL A ESTE NIVEL PORQUE EN EL SERVICIO SI PUEDE SER VACIO. LLAMADAS INTERNAS
            if (userId == Guid.Empty || userId == null)
                throw new FatalException(CodeExceptions.USER_NOT_EXIST);

            _servicePayment.SendPaymentTicketByEmail(id, transactionNumber, userId);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpGet]
        public HttpResponseMessage IsPaymentDoneWithServiceAssosiated(Guid serviceAssosiatedId)
        {
            var result = _servicePayment.IsPaymentDoneWithServiceAssosiated(serviceAssosiatedId);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }
        [HttpGet]
        public HttpResponseMessage CountPaymentsDone(Guid registredUserId, Guid anonymousUserId, Guid serviceId)
        {
            var result = _servicePayment.CountPaymentsDone(registredUserId, anonymousUserId, serviceId);
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [HttpPut]
        public HttpResponseMessage ReversePayment([FromBody] RefundPayment entity)
        {
            _servicePayment.ReversePayment(entity);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpGet]
        public HttpResponseMessage NotifyError([FromUri] string data)
        {
            _servicePayment.NotifyError(data);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpPost]
        public HttpResponseMessage GetDataForFromList([FromBody] PaymentFilterDto filterDto)
        {
            var payments = _servicePayment.GetDataForFromList(filterDto);
            if (payments != null)
            {
                foreach (var paymentDto in payments)
                {
                    WebControllerHelper.LoadServiceReferenceParams(paymentDto.ServiceDto);
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, payments);
        }

        [HttpPost]
        public HttpResponseMessage CancelPayment([FromBody] string transactionNumber)
        {
            var paymentCanceled = _servicePayment.CancelPaymentDone(Guid.Empty, transactionNumber, false);
            return Request.CreateResponse(HttpStatusCode.OK, paymentCanceled);
        }

        [HttpPost]
        public HttpResponseMessage DeleteCardInCybersource([FromBody] DeleteTrnsDto entity)
        {
            var cardDeleted = _servicePayment.DeleteCardInCybersource(entity.UserId, entity.CardId, entity.TransactionNumber);
            return Request.CreateResponse(HttpStatusCode.OK, cardDeleted);
        }

        [HttpPost]
        public HttpResponseMessage NotifyExternalSourceNewPayment([FromBody] PaymentDto paymentDto)
        {
            var ok = _servicePayment.NotifyExternalSourceNewPayment(paymentDto);
            return Request.CreateResponse(HttpStatusCode.OK, ok);
        }

    }
}