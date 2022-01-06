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
    public class BinsClientService : WebApiClientService, IBinsClientService
    {
        public BinsClientService(ITransactionContext transactionContext)
            : base("Bin", transactionContext)
        {

        }

        public Task<ICollection<BinDto>> GetDataForTable(BaseFilter filtersDto)
        {
            return WebApiClient.CallApiServiceAsync<ICollection<BinDto>>(new WebApiHttpRequestGet(BaseUri, TransactionContext, filtersDto.GetFilterDictionary()));
        }


        public Task<int> GetDataForTableCount(BaseFilter filtersDto)
        {
            return WebApiClient.CallApiServiceAsync<int>(new WebApiHttpRequestGet(BaseUri + "/GetDataForTableCount", TransactionContext, filtersDto.GetFilterDictionary()));
        }

        public Task<ICollection<BinDto>> FindAll()
        {
            return WebApiClient.CallApiServiceAsync<ICollection<BinDto>>(new WebApiHttpRequestGet(BaseUri, TransactionContext));
        }

        public Task<BinDto> Find(Guid id)
        {
            return
                WebApiClient.CallApiServiceAsync<BinDto>(new WebApiHttpRequestGet(
                    BaseUri, TransactionContext, new Dictionary<string, string> { { "id", id.ToString() } }));
        }

        public Task Create(BinDto bin)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestPut(BaseUri, TransactionContext, bin));
        }

        public Task Edit(BinDto bin)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestPost(BaseUri, TransactionContext, bin.Id, bin));
        }

        public Task Delete(Guid id)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestDelete(BaseUri, TransactionContext, id));
        }

        public Task<ICollection<BankDto>> GetBanks()
        {
            return WebApiClient.CallApiServiceAsync<ICollection<BankDto>>(new WebApiHttpRequestGet(BaseUri + "/GetBanks", TransactionContext));
        }

        public Task ChangeStatus(Guid id)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestDelete(BaseUri + "/ChangeStatus", TransactionContext, id));
        }
    }
}
