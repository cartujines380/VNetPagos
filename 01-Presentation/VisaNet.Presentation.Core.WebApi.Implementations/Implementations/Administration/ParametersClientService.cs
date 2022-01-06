using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Common.Security;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration;
using VisaNet.Presentation.Core.WebApiHttpCustomRequest;

namespace VisaNet.Presentation.Core.WebApi.Implementations.Implementations.Administration
{
    public class ParametersClientService : WebApiClientService, IParametersClientService
    {
        public ParametersClientService(ITransactionContext transactionContext)
            : base("Parameters", transactionContext)
        {

        }

        public Task<ICollection<ParametersDto>> FindAll()
        {
            return WebApiClient.CallApiServiceAsync<ICollection<ParametersDto>>(new WebApiHttpRequestGet(BaseUri, TransactionContext));
        }

        public Task<ParametersDto> Find(Guid id)
        {
            return
                WebApiClient.CallApiServiceAsync<ParametersDto>(new WebApiHttpRequestGet(
                    BaseUri, TransactionContext, new Dictionary<string, string> { { "id", id.ToString() } }));
        }

        public Task Edit(ParametersDto parameters)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestPost(BaseUri, TransactionContext, parameters.Id, parameters));
        }
    }
}
