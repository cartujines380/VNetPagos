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
    public class LifApiBillClientService : WebApiClientService, ILifApiBillClientService
    {
        public LifApiBillClientService(ITransactionContext transactionContext)
            : base("LifApiBill", transactionContext)
        {

        }

        public Task<ICollection<LifApiBillDto>> FindAll(BaseFilter filtersDto)
        {
            return WebApiClient.CallApiServiceAsync<ICollection<LifApiBillDto>>(new WebApiHttpRequestGet(BaseUri, TransactionContext, filtersDto.GetFilterDictionary()));
        }

        public Task<LifApiBillDto> Find(Guid id)
        {
            return
                WebApiClient.CallApiServiceAsync<LifApiBillDto>(new WebApiHttpRequestGet(
                    BaseUri, TransactionContext, new Dictionary<string, string> { { "id", id.ToString() } }));
        }
        
        public Task<int> GetDataForLifApiBillCount(BaseFilter filterDto)
        {
            return WebApiClient.CallApiServiceAsync<int>(new WebApiHttpRequestPost(BaseUri + "/GetDataForLifApiBillCount", TransactionContext, filterDto));
        }
    }
}
