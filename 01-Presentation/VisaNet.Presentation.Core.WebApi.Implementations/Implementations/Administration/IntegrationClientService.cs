using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Common.Security.WebService;
using VisaNet.Domain.EntitiesDtos.ExternalRequest;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration;
using VisaNet.Presentation.Core.WebApiHttpCustomRequest;

namespace VisaNet.Presentation.Core.WebApi.Implementations.Implementations.Administration
{
    public class IntegrationClientService : WebApiClientService, IIntegrationClientService
    {
        public IntegrationClientService(IWebServiceTransactionContext transactionContext)
            : base("Integration", transactionContext)
        {
        }

        public Task<ICollection<WsBillQueryDto>> GetBillQueriesForTable(ReportsIntegrationFilterDto filterDto)
        {
            return WebApiClient.CallApiServiceAsync<ICollection<WsBillQueryDto>>(new WebApiHttpRequestGet(
                BaseUri + "/GetBillQueriesForTable", TransactionContext, filterDto.GetFilterDictionary()));
        }

        public Task<int> GetBillQueriesForTableCount(ReportsIntegrationFilterDto filterDto)
        {
            return WebApiClient.CallApiServiceAsync<int>(new WebApiHttpRequestGet(
                BaseUri + "/GetBillQueriesForTableCount", TransactionContext, filterDto.GetFilterDictionary()));
        }

        public Task<ICollection<WsBillPaymentOnlineDto>> GetBillPaymentsOnlineForTable(ReportsIntegrationFilterDto filterDto)
        {
            return WebApiClient.CallApiServiceAsync<ICollection<WsBillPaymentOnlineDto>>(new WebApiHttpRequestGet(
                BaseUri + "/GetBillPaymentsOnlineForTable", TransactionContext, filterDto.GetFilterDictionary()));
        }

        public Task<int> GetBillPaymentsOnlineForTableCount(ReportsIntegrationFilterDto filterDto)
        {
            return WebApiClient.CallApiServiceAsync<int>(new WebApiHttpRequestGet(
                BaseUri + "/GetBillPaymentsOnlineForTableCount", TransactionContext, filterDto.GetFilterDictionary()));
        }

        public Task<ICollection<WsCommerceQueryDto>> GetCommerceQueriesForTable(ReportsIntegrationFilterDto filterDto)
        {
            return WebApiClient.CallApiServiceAsync<ICollection<WsCommerceQueryDto>>(new WebApiHttpRequestGet(
                BaseUri + "/GetCommerceQueriesForTable", TransactionContext, filterDto.GetFilterDictionary()));
        }

        public Task<int> GetCommerceQueriesForTableCount(ReportsIntegrationFilterDto filterDto)
        {
            return WebApiClient.CallApiServiceAsync<int>(new WebApiHttpRequestGet(
                BaseUri + "/GetCommerceQueriesForTableCount", TransactionContext, filterDto.GetFilterDictionary()));
        }

        public Task<ICollection<WsPaymentCancellationDto>> GetPaymentCancellationsForTable(ReportsIntegrationFilterDto filterDto)
        {
            return WebApiClient.CallApiServiceAsync<ICollection<WsPaymentCancellationDto>>(new WebApiHttpRequestGet(
                BaseUri + "/GetPaymentCancellationsForTable", TransactionContext, filterDto.GetFilterDictionary()));
        }

        public Task<int> GetPaymentCancellationsForTableCount(ReportsIntegrationFilterDto filterDto)
        {
            return WebApiClient.CallApiServiceAsync<int>(new WebApiHttpRequestGet(
                BaseUri + "/GetPaymentCancellationsForTableCount", TransactionContext, filterDto.GetFilterDictionary()));
        }

        public Task<ICollection<WsCardRemoveDto>> GetCardRemovesForTable(ReportsIntegrationFilterDto filterDto)
        {
            return WebApiClient.CallApiServiceAsync<ICollection<WsCardRemoveDto>>(new WebApiHttpRequestGet(
                BaseUri + "/GetCardRemovesForTable", TransactionContext, filterDto.GetFilterDictionary()));
        }

        public Task<int> GetCardRemovesForTableCount(ReportsIntegrationFilterDto filterDto)
        {
            return WebApiClient.CallApiServiceAsync<int>(new WebApiHttpRequestGet(
                BaseUri + "/GetCardRemovesForTableCount", TransactionContext, filterDto.GetFilterDictionary()));
        }

        public Task<WsBillPaymentOnlineDto> GetWsBillPaymentOnline(Guid id)
        {
            return WebApiClient.CallApiServiceAsync<WsBillPaymentOnlineDto>(new WebApiHttpRequestGet(
                BaseUri + "/GetWsBillPaymentOnline", TransactionContext, new Dictionary<string, string> { { "id", id.ToString() } }));
        }

    }
}