using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Common.Security;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration;
using VisaNet.Presentation.Core.WebApiHttpCustomRequest;

namespace VisaNet.Presentation.Core.WebApi.Implementations.Implementations.Administration
{
    public class GatewayClientService : WebApiClientService, IGatewayClientService
    {
        public GatewayClientService(ITransactionContext transactionContext)
            : base("Gateway", transactionContext)
        {

        }

        public Task<ICollection<GatewayDto>> FindAll()
        {
            return WebApiClient.CallApiServiceAsync<ICollection<GatewayDto>>(new WebApiHttpRequestGet(BaseUri, TransactionContext));
        }

        
    }
}
