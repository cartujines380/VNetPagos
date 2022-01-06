using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Common.Security;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Web;
using VisaNet.Presentation.Core.WebApiHttpCustomRequest;

namespace VisaNet.Presentation.Core.WebApi.Implementations.Implementations.Web
{
    public class WebGatewayClientService : WebApiClientService, IWebGatewayClientService
    {
        public WebGatewayClientService(ITransactionContext transactionContext)
            : base("WebGateway", transactionContext)
        {

        }

        public Task<ICollection<GatewayDto>> FindAll()
        {
            return WebApiClient.CallApiServiceAsync<ICollection<GatewayDto>>(new WebApiHttpRequestGet(BaseUri, TransactionContext));
        }

        
    }
}
