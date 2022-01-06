using System.Threading.Tasks;
using VisaNet.Common.Security;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Web;
using VisaNet.Presentation.Core.WebApiHttpCustomRequest;

namespace VisaNet.Presentation.Core.WebApi.Implementations.Implementations.Web
{
    public class WebContactClientService : WebApiClientService, IWebContactClientService
    {
        public WebContactClientService(ITransactionContext transactionContext)
            : base("WebContact", transactionContext)
        {

        }

        public Task Create(ContactDto contact)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestPut(BaseUri, TransactionContext, contact));
        }

        public Task GetContactTypes()
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestGet(BaseUri, TransactionContext));
        }
    }
}
