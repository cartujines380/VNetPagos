using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using VisaNet.Common.Security;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration;
using VisaNet.Presentation.Core.WebApiHttpCustomRequest;
using VisaNet.Utilities.Cybersource;

namespace VisaNet.Presentation.Core.WebApi.Implementations.Implementations.Administration
{
    public class PaymentClientService : WebApiClientService, IPaymentClientService
    {
        public PaymentClientService(ITransactionContext transactionContext)
            : base("Payment", transactionContext)
        {

        }

        public Task<PaymentDto> GetByTransactionNumber(string transactionNumber)
        {
            return WebApiClient.CallApiServiceAsync<PaymentDto>(new WebApiHttpRequestGet(BaseUri + "/GetByTransactionNumber", TransactionContext, new Dictionary<string, string>
            {
                { "transactionNumber", transactionNumber }
            }));
        }

        public Task<PaymentDto> GetFullPaymentByTransactionNumber(string transactionNumber)
        {
            return WebApiClient.CallApiServiceAsync<PaymentDto>(new WebApiHttpRequestGet(BaseUri + "/GetFullPaymentByTransactionNumber", TransactionContext, new Dictionary<string, string>
            {
                { "transactionNumber", transactionNumber }
            }));
        }

        public Task<ICollection<PaymentDto>> FindAll()
        {
            return WebApiClient.CallApiServiceAsync<ICollection<PaymentDto>>(new WebApiHttpRequestGet(BaseUri, TransactionContext));
        }

        public Task<ICollection<PaymentBillDto>> ReportsTransactionsDataFromDbView(ReportsTransactionsFilterDto filtersDto)
        {
            return WebApiClient.CallApiServiceAsync<ICollection<PaymentBillDto>>(new WebApiHttpRequestGet(BaseUri + "/ReportsTransactionsDataFromDbView", TransactionContext, filtersDto.GetFilterDictionary()));
        }

        public Task<int> ReportsTransactionsDataCount(ReportsTransactionsFilterDto filtersDto)
        {
            return WebApiClient.CallApiServiceAsync<int>(new WebApiHttpRequestGet(BaseUri + "/ReportsTransactionsDataCount", TransactionContext, filtersDto.GetFilterDictionary()));
        }

        public Task<IEnumerable<PaymentDto>> GetDashboardData(ReportsDashboardFilterDto filtersDto)
        {
            return WebApiClient.CallApiServiceAsync<IEnumerable<PaymentDto>>(new WebApiHttpRequestPost(BaseUri + "/GetDashboardData", TransactionContext, filtersDto));
        }

        public Task<byte[]> DownloadTicket(Guid id, string transactionNumber, Guid userId)
        {
            return
                WebApiClient.CallApiServiceAsync<byte[]>(new WebApiHttpRequestGet(BaseUri + "/DownloadTicket",
                    TransactionContext,
                    new Dictionary<string, string> { { "id", id.ToString() }, { "transactionNumber", transactionNumber }, { "userId", userId.ToString() } }));
        }

        public Task<CyberSourceOperationData> TestCyberSourcePayment(Guid serviceId)
        {
            return
                WebApiClient.CallApiServiceAsync<CyberSourceOperationData>(new WebApiHttpRequestGet(BaseUri + "/TestCyberSourcePayment",
                    TransactionContext, new Dictionary<string, string> { { "serviceId", serviceId.ToString() } }));
        }

        public Task<CyberSourceOperationData> TestCyberSourceCancelPayment(Guid id, CyberSourceOperationData cSOperationData)
        {
            return
                WebApiClient.CallApiServiceAsync<CyberSourceOperationData>(new WebApiHttpRequestPost(BaseUri + "/TestCyberSourceCancelPayment", TransactionContext, id, cSOperationData));
        }

        public Task<CyberSourceOperationData> TestCyberSourceReversal(Guid serviceId)
        {
            return
                WebApiClient.CallApiServiceAsync<CyberSourceOperationData>(new WebApiHttpRequestGet(BaseUri + "/TestCyberSourceReversal",
                    TransactionContext, new Dictionary<string, string> { { "serviceId", serviceId.ToString() } }));
        }

        public Task<bool> TestCyberSourceReports(Guid serviceId)
        {
            return
                WebApiClient.CallApiServiceAsync<bool>(new WebApiHttpRequestGet(BaseUri + "/TestCyberSourceReports",
                    TransactionContext, new Dictionary<string, string> { { "serviceId", serviceId.ToString() } }));
        }

        public Task ReversePayment(RefundPayment reverse)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestPut(BaseUri + "/ReversePayment", TransactionContext, reverse));
        }

        public Task<CyberSourceOperationData> CancelPayment(CancelTrnsDto cancelPayment)
        {
            return WebApiClient.CallApiServiceAsync<CyberSourceOperationData>(new WebApiHttpRequestPost(BaseUri + "/CancelPayment", TransactionContext, cancelPayment));
        }
    }
}