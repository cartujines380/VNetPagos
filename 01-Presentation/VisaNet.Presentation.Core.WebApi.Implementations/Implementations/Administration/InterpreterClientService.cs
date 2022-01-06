using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Common.Security;
using VisaNet.Domain.Entities;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters.Base;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration;
using VisaNet.Presentation.Core.WebApiHttpCustomRequest;

namespace VisaNet.Presentation.Core.WebApi.Implementations.Implementations.Administration
{
    public class InterpreterClientService : WebApiClientService, IInterpreterClientService
    {
        public InterpreterClientService(ITransactionContext transactionContext)
            : base("Interpreter", transactionContext)
        {

        }

        public Task<ICollection<InterpreterDto>> FindAll(BaseFilter filtersDto)
        {
            return WebApiClient.CallApiServiceAsync<ICollection<InterpreterDto>>(new WebApiHttpRequestGet(BaseUri, TransactionContext, filtersDto.GetFilterDictionary()));
        }

        public Task<ICollection<InterpreterDto>> FindAll()
        {
            return WebApiClient.CallApiServiceAsync<ICollection<InterpreterDto>>(new WebApiHttpRequestGet(BaseUri, TransactionContext));
        }

        public Task<InterpreterDto> Find(Guid id)
        {
            return
                WebApiClient.CallApiServiceAsync<InterpreterDto>(new WebApiHttpRequestGet(
                    BaseUri, TransactionContext, new Dictionary<string, string> { { "id", id.ToString() } }));
        }

        public Task<InterpreterDto> Create(InterpreterDto bin)
        {
            return WebApiClient.CallApiServiceAsync<InterpreterDto>(new WebApiHttpRequestPut(BaseUri, TransactionContext, bin));
        }

        public Task Edit(InterpreterDto bin)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestPost(BaseUri, TransactionContext, bin.Id, bin));
        }

        public Task Delete(Guid id)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestDelete(BaseUri, TransactionContext, id));
        }

        public Task<int> GetDataForInterpreterCount(BaseFilter filterDto)
        {
            return WebApiClient.CallApiServiceAsync<int>(new WebApiHttpRequestPost(BaseUri + "/GetDataForInterpreterCount", TransactionContext, filterDto));
        }
    }
}
