using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Common.Security;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters.Base;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Web;
using VisaNet.Presentation.Core.WebApiHttpCustomRequest;
using VisaNet.Utilities.Cybersource;

namespace VisaNet.Presentation.Core.WebApi.Implementations.Implementations.Web
{
    public class WebPaymentClientService : WebApiClientService, IWebPaymentClientService
    {
        public WebPaymentClientService(ITransactionContext transactionContext)
            : base("WebPayment", transactionContext)
        {
        }

        public Task<ICollection<PaymentDto>> FindAll(BaseFilter filtersDto)
        {
            return WebApiClient.CallApiServiceAsync<ICollection<PaymentDto>>(new WebApiHttpRequestPost(BaseUri + "/GetPayments", TransactionContext, filtersDto));
        }

        public Task<PaymentDto> Create(PaymentDto payment)
        {
            return WebApiClient.CallApiServiceAsync<PaymentDto>(new WebApiHttpRequestPut(BaseUri, TransactionContext, payment));
        }

        public Task<byte[]> DownloadTicket(Guid id, string transactionNumber, Guid userId)
        {
            return
                WebApiClient.CallApiServiceAsync<byte[]>(new WebApiHttpRequestGet(BaseUri + "/DownloadTicket",
                    TransactionContext,
                    new Dictionary<string, string> { { "id", id.ToString() }, { "transactionNumber", transactionNumber }, { "userId", userId.ToString() } }));
        }

        public Task SendTicketByEmail(Guid id, string transactionNumber, Guid userId)
        {
            return
                WebApiClient.CallApiServiceAsync<byte[]>(new WebApiHttpRequestGet(BaseUri + "/SendTicketByEmail",
                    TransactionContext,
                    new Dictionary<string, string> { { "id", id.ToString() }, { "transactionNumber", transactionNumber }, { "userId", userId.ToString() } }));

        }

        public Task<CybersourceCreatePaymentDto> NotifyPayment(IDictionary<string, string> csDictionaryData)
        {
            return WebApiClient.CallApiServiceAsync<CybersourceCreatePaymentDto>(new WebApiHttpRequestPut(BaseUri + "/NotifyPayment", TransactionContext, csDictionaryData));
        }

        public Task CancelPaymentCybersource(CancelPayment payment)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestPut(BaseUri + "/CancelPaymentCybersource", TransactionContext, payment));
        }

        public Task<bool> IsPaymentDoneWithServiceAssosiated(Guid id)
        {
            return WebApiClient.CallApiServiceAsync<bool>(new WebApiHttpRequestGet(BaseUri + "/IsPaymentDoneWithServiceAssosiated", TransactionContext,
                    new Dictionary<string, string> { { "serviceAssosiatedId", id.ToString() } }));

        }

        public Task<int> CountPaymentsDone(Guid registredUserId, Guid anonymousUserId, Guid serviceId)
        {
            return
                WebApiClient.CallApiServiceAsync<int>(new WebApiHttpRequestGet(BaseUri + "/CountPaymentsDone",
                    TransactionContext,
                    new Dictionary<string, string> {
                    { "registredUserId", registredUserId.ToString() },
                    { "anonymousUserId", anonymousUserId.ToString() },
                    { "serviceId", serviceId.ToString() }
                    }));

        }

        public Task ReversePayment(RefundPayment reverse)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestPut(BaseUri + "/ReversePayment", TransactionContext, reverse));
        }

        public Task NotifyError(string data)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestGet(BaseUri + "/NotifyError", TransactionContext,
                    new Dictionary<string, string> { { "data", data } }));
        }

        public Task<ICollection<PaymentDto>> GetDataForFromList(BaseFilter filtersDto)
        {
            return WebApiClient.CallApiServiceAsync<ICollection<PaymentDto>>(new WebApiHttpRequestPost(BaseUri + "/GetDataForFromList", TransactionContext, filtersDto));
        }

        public Task<bool> NotifyExternalSourceNewPayment(PaymentDto paymentDto)
        {
            return WebApiClient.CallApiServiceAsync<bool>(new WebApiHttpRequestPost(
                BaseUri + "/NotifyExternalSourceNewPayment", TransactionContext, paymentDto));
        }
    }
}