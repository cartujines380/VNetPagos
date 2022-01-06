using System.Threading.Tasks;
using VisaNet.Common.Security;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Web;
using VisaNet.Presentation.Core.WebApiHttpCustomRequest;

namespace VisaNet.Presentation.Core.WebApi.Implementations.Implementations.Web
{
    public class WebHomePageClientService : WebApiClientService, IWebHomePageClientService
    {
        public WebHomePageClientService(ITransactionContext transactionContext)
            : base("WebHomePage", transactionContext)
        {

        }

        public Task<HomePageDto> Get()
        {
            return WebApiClient.CallApiServiceAsync<HomePageDto>(new WebApiHttpRequestGet(BaseUri, TransactionContext));
        }
    }
}
