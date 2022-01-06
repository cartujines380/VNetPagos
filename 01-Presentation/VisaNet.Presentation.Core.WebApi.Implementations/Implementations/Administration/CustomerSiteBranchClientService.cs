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
    public class CustomerSiteBranchClientService : WebApiClientService, ICustomerSiteBranchClientService
    {
        public CustomerSiteBranchClientService(ITransactionContext transactionContext)
            : base("CustomerSiteBranch", transactionContext)
        {

        }

        public Task<ICollection<CustomerSiteBranchDto>> GetBranchesLigth(BaseFilter filtersDto)
        {
            return WebApiClient.CallApiServiceAsync<ICollection<CustomerSiteBranchDto>>(new WebApiHttpRequestGet(BaseUri + "/GetBranchesLigth", TransactionContext, filtersDto.GetFilterDictionary()));
        }
        
        public Task<ICollection<CustomerSiteBranchDto>> GetDataForBranchTable(BaseFilter filtersDto)
        {
            return WebApiClient.CallApiServiceAsync<ICollection<CustomerSiteBranchDto>>(new WebApiHttpRequestGet(BaseUri + "/GetDataForBranchTable",
                TransactionContext, filtersDto.GetFilterDictionary()));
        }
        public Task<int> GetDataForBranchTableCount(BaseFilter filtersDto)
        {
            return WebApiClient.CallApiServiceAsync<int>(new WebApiHttpRequestGet(BaseUri + "/GetDataForBranchTableCount",
                TransactionContext, filtersDto.GetFilterDictionary()));
        }

        public Task<CustomerSiteBranchDto> Find(Guid id)
        {
            return
                WebApiClient.CallApiServiceAsync<CustomerSiteBranchDto>(new WebApiHttpRequestGet(
                    BaseUri + "/GetBranch", TransactionContext, new Dictionary<string, string> { { "id", id.ToString() } }));
        }
        public Task Create(CustomerSiteBranchDto bin)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestPut(BaseUri + "/CreateBranch", TransactionContext, bin));
        }
        public Task Edit(CustomerSiteBranchDto bin)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestPost(BaseUri + "/EditBranch", TransactionContext, bin.Id, bin));
        }
        public Task Delete(Guid id)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestDelete(BaseUri + "/DeleteBranch", TransactionContext, id));
        }

        public Task ChangeState(Guid entityId)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestPost(BaseUri + "/ChangeBranchState", TransactionContext, entityId));
        }
        
    }
}
