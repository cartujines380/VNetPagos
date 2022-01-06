using System.Threading.Tasks;
using VisaNet.Common.Security;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.LIF;
using VisaNet.Presentation.Core.WebApiHttpCustomRequest;

namespace VisaNet.Presentation.Core.WebApi.Implementations.Implementations.LIF
{
    public class LifApiBillClientService : WebApiClientService, ILifApiBillClientService
    {
        public LifApiBillClientService(ITransactionContext transactionContext)
            : base("LifApiBill", transactionContext)
        {
        }

        public Task Create(LifApiBillDto lifApiBill)
        {
            return WebApiClient.CallApiServiceAsync<BinDto>(new WebApiHttpRequestPost(BaseUri + "/Create", TransactionContext, new { lifApiBill }));
        }

    }
}