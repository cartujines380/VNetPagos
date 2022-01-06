using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Common.Logging.NLog;
using VisaNet.Common.Security.WebService;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.ExternalRequest;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.WebService;
using VisaNet.Presentation.Core.WebApiHttpCustomRequest;

namespace VisaNet.Presentation.Core.WebApi.Implementations.Implementations.WebService
{
    public class WsExternalAppClientService : WebApiClientService, IWsExternalAppClientService
    {
        public WsExternalAppClientService(IWebServiceTransactionContext transactionContext)
            : base("WebServiceExternalApp", transactionContext)
        {
            
        }

        public Task<ICollection<HighwayBillDto>> AddNewBills(WebServiceBillsInputDto dto)
        {
            return WebApiClient.CallApiServiceAsync<ICollection<HighwayBillDto>>(new WebApiHttpRequestPut(BaseUri + "/AddNewBills", TransactionContext, dto));
        }

        public Task<int> DeleteBills(WebServiceBillsDeleteDto dto)
        {
            return WebApiClient.CallApiServiceAsync<int>(new WebApiHttpRequestPut(BaseUri + "/DeleteBills", TransactionContext, dto));
        }

        public Task<List<WebServiceApplicationClientDto>> AssosiatedServiceClientUpdate(WebServiceClientInputDto dto)
        {
            return WebApiClient.CallApiServiceAsync<List<WebServiceApplicationClientDto>>(new WebApiHttpRequestPut(BaseUri + "/AssosiatedServiceClientUpdate", TransactionContext, dto));
        }

        public Task<WebServiceBillStatusDto> BillsStatus(WsBillQueryDto dto)
        {
            return WebApiClient.CallApiServiceAsync<WebServiceBillStatusDto>(new WebApiHttpRequestPut(BaseUri + "/BillsStatus", TransactionContext, dto));
        }

        public Task<string> GetCertificateThumbprint(WebServiceClientInputDto dto)
        {
            return WebApiClient.CallApiServiceAsync<string>(new WebApiHttpRequestPut(BaseUri + "/GetCertificateThumbprint", TransactionContext, dto));
        }

        #region Apps
        public Task<string> GetCertificateThumbprintIdApp(WebServiceClientInputDto dto)
        {
            return WebApiClient.CallApiServiceAsync<string>(new WebApiHttpRequestPut(BaseUri + "/GetCertificateThumbprintIdApp", TransactionContext, dto));
        }
        public Task<TransactionResult> MakePayment(WsBillPaymentOnlineDto dto)
        {
            return WebApiClient.CallApiServiceAsync<TransactionResult>(new WebApiHttpRequestPut(BaseUri + "/MakePayment", TransactionContext, dto));
        }
        public Task<TransactionResult> CancelPayment(WsPaymentCancellationDto dto)
        {
            return WebApiClient.CallApiServiceAsync<TransactionResult>(new WebApiHttpRequestPut(BaseUri + "/CancelPayment", TransactionContext, dto));
        }
        public Task<WebServiceTransactionHistoryDto> TransactionsHistory(WsBillQueryDto dto)
        {
            return WebApiClient.CallApiServiceAsync<WebServiceTransactionHistoryDto>(new WebApiHttpRequestPut(BaseUri + "/TransactionsHistory", TransactionContext, dto));
        }
        public Task<TransactionCommerceResult> GetServices(WsCommerceQueryDto dto)
        {
            return WebApiClient.CallApiServiceAsync<TransactionCommerceResult>(new WebApiHttpRequestPut(BaseUri + "/GetServices", TransactionContext, dto));
        }
        public Task<TransactionResult> RemoveCard(WsCardRemoveDto dto)
        {
            return WebApiClient.CallApiServiceAsync<TransactionResult>(new WebApiHttpRequestPut(BaseUri + "/RemoveCard", TransactionContext, dto));
        }

        public Task<IList<WebhookNewAssociationDto>> GetUrlTransacctionPosts(WsUrlTransactionQueryDto dto)
        {
            return WebApiClient.CallApiServiceAsync<IList<WebhookNewAssociationDto>>(new WebApiHttpRequestPut(BaseUri + "/GetUrlTransacctionPosts", TransactionContext, dto));
        }

        #endregion

    }
}