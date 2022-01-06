using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Common.Security;
using VisaNet.CustomerSite.EntitiesDtos;
using VisaNet.CustomerSite.EntitiesDtos.TableFilters;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters.Base;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Web;
using VisaNet.Presentation.Core.WebApiHttpCustomRequest;

namespace VisaNet.Presentation.Core.WebApi.Implementations.Implementations.Web
{
    public class WebDebitClientService : WebApiClientService, IWebDebitClientService
    {
        public WebDebitClientService(ITransactionContext transactionContext)
            : base("WebDebit", transactionContext)
        {
        }

        public Task<ICollection<CustomerSiteCommerceDto>> GetCommercesDebit(BaseFilter customerSiteCommerceFilterDto = null)
        {
            return WebApiClient.CallApiServiceAsync<ICollection<CustomerSiteCommerceDto>>(new WebApiHttpRequestGet(BaseUri + "/GetCommercesDebit", TransactionContext,
                customerSiteCommerceFilterDto != null ? customerSiteCommerceFilterDto.GetFilterDictionary() : null));
        }

        public Task<CustomerSiteCommerceDto> FindCommerceDebit(Guid id)
        {
            return
                WebApiClient.CallApiServiceAsync<CustomerSiteCommerceDto>(new WebApiHttpRequestGet(
                    BaseUri + "/FindCommerceDebit", TransactionContext, new Dictionary<string, string> { { "id", id.ToString() } }));
        }

        public Task<IEnumerable<DebitRequestDto>> GetDebitRequestByUserId(Guid userId)
        {
            return WebApiClient.CallApiServiceAsync<IEnumerable<DebitRequestDto>>(new WebApiHttpRequestGet(BaseUri + "/GetDebitRequestByUserId", TransactionContext, new Dictionary<string, string> { { "userId", userId.ToString() } }));
        }

        public Task<ICollection<DebitRequestTableDto>> GetDataForFromList(BaseFilter filtersDto)
        {
            return WebApiClient.CallApiServiceAsync<ICollection<DebitRequestTableDto>>(new WebApiHttpRequestPost(BaseUri + "/GetDataForFromList", TransactionContext, filtersDto));
        }

        public Task<bool> ValidateCardType(int binValue)
        {
            return
                WebApiClient.CallApiServiceAsync<bool>(new WebApiHttpRequestGet(
                    BaseUri + "/ValidateCardType", TransactionContext, new Dictionary<string, string> { { "binValue", binValue.ToString() } }));
        }

        public Task<DebitRequestDto> Create(DebitRequestDto dto)
        {
            return WebApiClient.CallApiServiceAsync<DebitRequestDto>(new WebApiHttpRequestPut(BaseUri + "/Create", TransactionContext, dto));
        }

        public Task<bool> CancelDebitRequest(Guid id)
        {
            return WebApiClient.CallApiServiceAsync<bool>(new WebApiHttpRequestPost(BaseUri + "/CancelDebitRequest", TransactionContext, id));
        }

        public Task<CybersourceCreateDebitWithNewCardDto> ProccesDataFromCybersource(IDictionary<string, string> csDictionary)
        {
            return WebApiClient.CallApiServiceAsync<CybersourceCreateDebitWithNewCardDto>(new WebApiHttpRequestPut(BaseUri + "/ProccesDataFromCybersource", TransactionContext, csDictionary));
        }

    }
}