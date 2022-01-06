using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Common.Security;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Web;
using VisaNet.Presentation.Core.WebApiHttpCustomRequest;

namespace VisaNet.Presentation.Core.WebApi.Implementations.Implementations.Web
{
    public class WebVisaNetOnIntegrationClientService : WebApiClientService, IWebVisaNetOnIntegrationClientService
    {
        public WebVisaNetOnIntegrationClientService(ITransactionContext transactionContext)
            : base("WebVisaNetOnIntegration", transactionContext)
        {
        }

        public Task<ResultDto> ProcessOperation(ProcessOperationDto processOperationDto)
        {
            return WebApiClient.CallApiServiceAsync<ResultDto>(new WebApiHttpRequestPost(
                BaseUri + "/ProcessOperation", TransactionContext, processOperationDto));
        }

        public Task<VonDataDto> FindVonData(string idApp, string idUserExternal, string idCardExternal)
        {
            return WebApiClient.CallApiServiceAsync<VonDataDto>(new WebApiHttpRequestGet(BaseUri + "/FindVonData", TransactionContext,
                new Dictionary<string, string>
                {
                    { "idApp", idApp },
                    { "idUserExternal", idUserExternal },
                    { "idCardExternal", idCardExternal }
                }));
        }

        public Task<VonDataDto> FindVonData(string idApp, Guid anonymousUserId, string idCardExternal)
        {
            return WebApiClient.CallApiServiceAsync<VonDataDto>(new WebApiHttpRequestGet(BaseUri + "/FindVonData", TransactionContext,
                new Dictionary<string, string>
                {
                    { "idApp", idApp },
                    { "anonymousUserId", anonymousUserId.ToString() },
                    { "idCardExternal", idCardExternal }
                }));
        }

        public Task<ICollection<VonDataDto>> FindVonData(string idApp, string idUserExternal)
        {
            return WebApiClient.CallApiServiceAsync<ICollection<VonDataDto>>(new WebApiHttpRequestGet(BaseUri + "/FindVonData", TransactionContext,
                new Dictionary<string, string>
                {
                    { "idApp", idApp },
                    { "idUserExternal", idUserExternal }
                }));
        }

        public Task<ICollection<VonDataDto>> FindVonData(string idApp, Guid anonymousUserId)
        {
            return WebApiClient.CallApiServiceAsync<ICollection<VonDataDto>>(new WebApiHttpRequestGet(BaseUri + "/FindVonData", TransactionContext,
                new Dictionary<string, string>
                {
                    { "idApp", idApp },
                    { "anonymousUserId", anonymousUserId.ToString() }
                }));
        }

        public Task<byte[]> DownloadTicket(string transactionNumber, Guid userId)
        {
            return
                WebApiClient.CallApiServiceAsync<byte[]>(new WebApiHttpRequestGet(BaseUri + "/DownloadTicket",
                    TransactionContext,
                    new Dictionary<string, string> { { "transactionNumber", transactionNumber }, { "userId", userId.ToString() } }));
        }

        public Task SendPaymentTicketByEmail(string transactionNumber, Guid userId)
        {
            return
                WebApiClient.CallApiServiceAsync(new WebApiHttpRequestGet(BaseUri + "/SendPaymentTicketByEmail",
                    TransactionContext,
                    new Dictionary<string, string> { { "transactionNumber", transactionNumber }, { "userId", userId.ToString() } }));
        }

        public Task<PaymentDto> GetPaymentDto(string transactionNumber, string idApp)
        {
            return
                WebApiClient.CallApiServiceAsync<PaymentDto>(new WebApiHttpRequestGet(BaseUri + "/GetPaymentDto",
                    TransactionContext,
                    new Dictionary<string, string> { { "transactionNumber", transactionNumber }, { "idApp", idApp } }));
        }

        public Task<bool> CancelPayment(string transactionNumber)
        {
            return
                WebApiClient.CallApiServiceAsync<bool>(new WebApiHttpRequestPost(BaseUri + "/CancelPayment",
                    TransactionContext, transactionNumber));
        }
    }
}