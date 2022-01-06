using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Common.Security;
using VisaNet.CustomerSite.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters.Base;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration;
using VisaNet.Presentation.Core.WebApiHttpCustomRequest;

namespace VisaNet.Presentation.Core.WebApi.Implementations.Implementations.Administration
{
    public class CustomerSiteCommerceClientService : WebApiClientService, ICustomerSiteCommerceClientService
    {
        public CustomerSiteCommerceClientService(ITransactionContext transactionContext)
            : base("CustomerSiteCommerce", transactionContext)
        {

        }

        public Task<ICollection<CustomerSiteCommerceDto>> GetCommercesLigth()
        {
            return WebApiClient.CallApiServiceAsync<ICollection<CustomerSiteCommerceDto>>(new WebApiHttpRequestGet(BaseUri + "/GetCommercesLigth", TransactionContext));
        }

        public Task<ICollection<CustomerSiteCommerceDto>> GetDataForCommerceTable(BaseFilter filtersDto)
        {
            return WebApiClient.CallApiServiceAsync<ICollection<CustomerSiteCommerceDto>>(new WebApiHttpRequestGet(BaseUri + "/GetDataForCommerceTable", 
                TransactionContext, filtersDto.GetFilterDictionary()));
        }
        public Task<int> GetDataForCommerceTableCount(BaseFilter filtersDto)
        {
            return WebApiClient.CallApiServiceAsync<int>(new WebApiHttpRequestGet(BaseUri + "/GetDataForCommerceTableCount", 
                TransactionContext, filtersDto.GetFilterDictionary()));
        }

        public Task<CustomerSiteCommerceDto> Find(Guid id)
        {
            return
                WebApiClient.CallApiServiceAsync<CustomerSiteCommerceDto>(new WebApiHttpRequestGet(
                    BaseUri, TransactionContext, new Dictionary<string, string> { { "id", id.ToString() } }));
        }
        public Task Create(CustomerSiteCommerceDto bin)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestPut(BaseUri, TransactionContext, bin));
        }
        public Task Edit(CustomerSiteCommerceDto bin)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestPost(BaseUri, TransactionContext, bin.Id, bin));
        }
        public Task Delete(Guid id)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestDelete(BaseUri, TransactionContext, id));
        }
        
        public Task ChangeState(Guid entityId)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestPost(BaseUri + "/ChangeState", TransactionContext, entityId));
        }

        //DEBITO
        public Task<ICollection<CustomerSiteCommerceDto>> GetCommercesDebit(BaseFilter filterDto)
        {
            return WebApiClient.CallApiServiceAsync<ICollection<CustomerSiteCommerceDto>>(new WebApiHttpRequestGet(BaseUri + "/GetCommercesDebit",
                TransactionContext, filterDto.GetFilterDictionary()));
        }
        public Task<int> GetCommercesDebitCount(BaseFilter filterDto)
        {
            return WebApiClient.CallApiServiceAsync<int>(new WebApiHttpRequestGet(BaseUri + "/GetDebitCommercesCount",
                TransactionContext, filterDto.GetFilterDictionary()));
        }
        
        public Task EditDebitCommerceServiceId(CustomerSiteCommerceDto dto)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestPost(BaseUri + "/EditDebitCommerceServiceId",
                TransactionContext, dto));
        }
    }
}
