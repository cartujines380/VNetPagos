using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VisaNet.Common.Security.WebService;
using VisaNet.Domain.EntitiesDtos.ExternalRequest;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.WebService;
using VisaNet.Presentation.Core.WebApiHttpCustomRequest;

namespace VisaNet.Presentation.Core.WebApi.Implementations.Implementations.WebService
{
    public class WsExternalClientService : WebApiClientService, IWsExternalClientService
    {
        public WsExternalClientService(IWebServiceTransactionContext transactionContext)
            : base("WebServiceExternal", transactionContext)
        {

        }

        public Task<ICollection<WsBillQueryDto>> GetBillQueries()
        {
            return WebApiClient.CallApiServiceAsync<ICollection<WsBillQueryDto>>(new WebApiHttpRequestGet(BaseUri + "/GetBillQueries", TransactionContext));
        }

        public Task<WsBillQueryDto> CreateBillQuery(WsBillQueryDto dto)
        {
            return WebApiClient.CallApiServiceAsync<WsBillQueryDto>(new WebApiHttpRequestPut(BaseUri + "/CreateBillQuery", TransactionContext, dto));
        }

        public Task EditBillQuery(WsBillQueryDto dto)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestPost(BaseUri + "/EditBillQuery", TransactionContext, dto));
        }

        public Task<ICollection<WsBillPaymentOnlineDto>> GetBillPaymentsOnline()
        {
            return WebApiClient.CallApiServiceAsync<ICollection<WsBillPaymentOnlineDto>>(new WebApiHttpRequestGet(BaseUri + "/GetBillPaymentsOnline", TransactionContext));
        }

        public Task<WsBillPaymentOnlineDto> CreateBillPaymentOnline(WsBillPaymentOnlineDto dto)
        {
            return WebApiClient.CallApiServiceAsync<WsBillPaymentOnlineDto>(new WebApiHttpRequestPut(BaseUri + "/CreateBillPaymentOnline", TransactionContext, dto));
        }

        public Task EditBillPaymentOnline(WsBillPaymentOnlineDto dto)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestPost(BaseUri + "/EditBillPaymentOnline", TransactionContext, dto));
        }

        public Task<bool> BillPaymentOnlineByOperationIdUsed(string idOperation, string idApp)
        {
            return WebApiClient.CallApiServiceAsync<bool>(new WebApiHttpRequestGet(BaseUri + "/BillPaymentOnlineByOperationIdUsed", TransactionContext, new Dictionary<string, string>() { { "idOperation", idOperation }, { "idApp", idApp } }));
        }
        public Task<bool> BillPaymentOnlineByOperationIdUsed(string idOperation)
        {
            return WebApiClient.CallApiServiceAsync<bool>(new WebApiHttpRequestGet(BaseUri + "/BillPaymentOnlineByOperationIdUsed", TransactionContext, new Dictionary<string, string>() { { "idOperation", idOperation }}));
        }

        public Task<ICollection<WsCommerceQueryDto>> GetCommerceQueries()
        {
            return WebApiClient.CallApiServiceAsync<ICollection<WsCommerceQueryDto>>(new WebApiHttpRequestGet(BaseUri + "/GetCommerceQueries", TransactionContext));
        }

        public Task<WsCommerceQueryDto> CreateCommerceQuery(WsCommerceQueryDto dto)
        {
            return WebApiClient.CallApiServiceAsync<WsCommerceQueryDto>(new WebApiHttpRequestPut(BaseUri + "/CreateCommerceQuery", TransactionContext, dto));
        }

        public Task EditCommerceQuery(WsCommerceQueryDto dto)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestPost(BaseUri + "/EditCommerceQuery", TransactionContext, dto));
        }

        public Task<ICollection<WsPaymentCancellationDto>> GetPaymentCancellations()
        {
            return WebApiClient.CallApiServiceAsync<ICollection<WsPaymentCancellationDto>>(new WebApiHttpRequestGet(BaseUri + "/GetPaymentCancellations", TransactionContext));
        }

        public Task<WsPaymentCancellationDto> CreatePaymentCancellation(WsPaymentCancellationDto dto)
        {
            return WebApiClient.CallApiServiceAsync<WsPaymentCancellationDto>(new WebApiHttpRequestPut(BaseUri + "/CreatePaymentCancellation", TransactionContext, dto));
        }

        public Task EditPaymentCancellation(WsPaymentCancellationDto dto)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestPost(BaseUri + "/EditPaymentCancellation", TransactionContext, dto));
        }

        public Task<bool> PaymentCancellationsByOperationIdUsed(string idOperation)
        {
            return WebApiClient.CallApiServiceAsync<bool>(new WebApiHttpRequestGet(BaseUri + "/PaymentCancellationsByOperationIdUsed", TransactionContext, new Dictionary<string, string>() { { "idOperation", idOperation } }));
        }

        public Task<bool> PaymentCancellationsIsOperationIdUsed(string idOperation, string idApp)
        {
            return WebApiClient.CallApiServiceAsync<bool>(new WebApiHttpRequestGet(BaseUri + "/PaymentCancellationsIsOperationIdUsed", TransactionContext, new Dictionary<string, string>() { { "idOperation", idOperation }, { "idApp", idApp } }));
        }

        public Task<bool> BillPaymentOnlineIsOperationIdUsed(string idOperation, string idApp)
        {
            return WebApiClient.CallApiServiceAsync<bool>(new WebApiHttpRequestGet(BaseUri + "/BillPaymentOnlineIsOperationIdUsed", TransactionContext, new Dictionary<string, string>() { { "idOperation", idOperation }, { "idApp", idApp } }));
        }

        public Task<bool> CardRemoveIsOperationIdUsed(string idOperation, string idApp)
        {
            return WebApiClient.CallApiServiceAsync<bool>(new WebApiHttpRequestGet(BaseUri + "/CardRemoveIsOperationIdUsed", TransactionContext, new Dictionary<string, string>() { { "idOperation", idOperation }, { "idApp", idApp } }));
        }

        public Task<WsCardRemoveDto> CreateCardRemove(WsCardRemoveDto dto)
        {
            return WebApiClient.CallApiServiceAsync<WsCardRemoveDto>(new WebApiHttpRequestPut(BaseUri + "/CreateCardRemove", TransactionContext, dto));
        }

        public Task EditCardRemove(WsCardRemoveDto dto)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestPost(BaseUri + "/EditCardRemove", TransactionContext, dto));
        }

    }
}