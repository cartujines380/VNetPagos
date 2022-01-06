using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Common.Security;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters.Base;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration;
using VisaNet.Presentation.Core.WebApiHttpCustomRequest;

namespace VisaNet.Presentation.Core.WebApi.Implementations.Implementations.Administration
{
    public class BankClientService : WebApiClientService, IBankClientService
    {
        public BankClientService(ITransactionContext transactionContext)
            : base("Bank", transactionContext)
        {

        }

        public Task<ICollection<BankDto>> FindAll(BaseFilter filtersDto)
        {
            return WebApiClient.CallApiServiceAsync<ICollection<BankDto>>(new WebApiHttpRequestGet(BaseUri, TransactionContext, filtersDto.GetFilterDictionary()));
        }
        public Task<ICollection<BankDto>> FindAll()
        {
            return WebApiClient.CallApiServiceAsync<ICollection<BankDto>>(new WebApiHttpRequestGet(BaseUri, TransactionContext));
        }
        public Task<BankDto> Find(Guid id)
        {
            return
                WebApiClient.CallApiServiceAsync<BankDto>(new WebApiHttpRequestGet(
                    BaseUri, TransactionContext, new Dictionary<string, string> { { "id", id.ToString() } }));
        }
        public Task Create(BankDto bin)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestPut(BaseUri, TransactionContext, bin));
        }
        public Task Edit(BankDto bin)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestPost(BaseUri, TransactionContext, bin.Id, bin));
        }
        public Task Delete(Guid id)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestDelete(BaseUri, TransactionContext, id));
        }

        public Task<int> GetDataForBankCount(BaseFilter filterDto)
        {
            return WebApiClient.CallApiServiceAsync<int>(new WebApiHttpRequestPost(BaseUri + "/GetDataForBankCount", TransactionContext, filterDto));
        }
    }
}
