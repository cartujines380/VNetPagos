using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Common.Security;
using VisaNet.CustomerSite.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters.Base;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration;
using VisaNet.Presentation.Core.WebApiHttpCustomRequest;

namespace VisaNet.Presentation.Core.WebApi.Implementations.Implementations.Administration
{
    public class CustomerSiteSystemUserClientService : WebApiClientService, ICustomerSiteSystemUserClientService
    {
        public CustomerSiteSystemUserClientService(ITransactionContext transactionContext)
            : base("CustomerSiteSystemUser", transactionContext)
        {

        }

        public Task<ICollection<CustomerSiteSystemUserDto>> GetSystemUserLigth()
        {
            return WebApiClient.CallApiServiceAsync<ICollection<CustomerSiteSystemUserDto>>(new WebApiHttpRequestGet(BaseUri + "/GetSystemUserLigth", TransactionContext));
        }

        public Task<ICollection<CustomerSiteSystemUserDto>> GetDataForSystemUserTable(BaseFilter filtersDto)
        {
            return WebApiClient.CallApiServiceAsync<ICollection<CustomerSiteSystemUserDto>>(new WebApiHttpRequestGet(BaseUri + "/GetDataForSystemUserTable",
                TransactionContext, filtersDto.GetFilterDictionary()));
        }

        public Task<int> GetDataForSystemUserTableCount(BaseFilter filtersDto)
        {
            return WebApiClient.CallApiServiceAsync<int>(new WebApiHttpRequestGet(BaseUri + "/GetDataForSystemUserTableCount",
               TransactionContext, filtersDto.GetFilterDictionary()));
        }

        public Task<CustomerSiteSystemUserDto> Find(Guid id)
        {
            return
                WebApiClient.CallApiServiceAsync<CustomerSiteSystemUserDto>(new WebApiHttpRequestGet(
                    BaseUri, TransactionContext, new Dictionary<string, string> { { "id", id.ToString() } }));
        }

        public Task<CustomerSiteSystemUserDto> Create(CustomerSiteSystemUserDto bin)
        {
            return WebApiClient.CallApiServiceAsync<CustomerSiteSystemUserDto>(new WebApiHttpRequestPut(BaseUri, TransactionContext, bin));
        }

        public Task Edit(CustomerSiteSystemUserDto bin)
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
    }
}
