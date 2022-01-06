using System.Threading.Tasks;
using VisaNet.Common.Security;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Web;
using VisaNet.Presentation.Core.WebApiHttpCustomRequest;

namespace VisaNet.Presentation.Core.WebApi.Implementations.Implementations.Web
{
    public class WebParameterClientService : WebApiClientService, IWebParameterClientService
    {
        public WebParameterClientService(ITransactionContext transactionContext)
            : base("WebParameter", transactionContext)
        {

        }


        public Task<ParametersDto> Get()
        {
            return WebApiClient.CallApiServiceAsync<ParametersDto>(new WebApiHttpRequestGet(BaseUri, TransactionContext));
        }
    }
}
