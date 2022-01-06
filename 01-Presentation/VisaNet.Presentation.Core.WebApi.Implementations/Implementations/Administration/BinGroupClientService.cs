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
    public class BinGroupClientService : WebApiClientService, IBinGroupClientService
    {
        public BinGroupClientService(ITransactionContext transactionContext)
            : base("BinGroup", transactionContext)
        {

        }

        public Task<ICollection<BinGroupDto>> FindAll(BaseFilter filtersDto)
        {
            return WebApiClient.CallApiServiceAsync<ICollection<BinGroupDto>>(new WebApiHttpRequestGet(BaseUri, TransactionContext, filtersDto.GetFilterDictionary()));
        }

        public Task<ICollection<BinGroupDto>> FindAll()
        {
            return WebApiClient.CallApiServiceAsync<ICollection<BinGroupDto>>(new WebApiHttpRequestGet(BaseUri, TransactionContext));
        }

        public Task<BinGroupDto> Find(Guid id)
        {
            return WebApiClient.CallApiServiceAsync<BinGroupDto>(new WebApiHttpRequestGet(BaseUri, TransactionContext, new Dictionary<string, string> { { "id", id.ToString() } }));
        }

        public Task Create(BinGroupDto bin)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestPost(BaseUri, TransactionContext, bin));
        }

        public Task Edit(BinGroupDto bin)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestPut(BaseUri, TransactionContext, bin));
        }

        public Task Delete(Guid id)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestDelete(BaseUri, TransactionContext, id));
        }
    }
}
