using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Common.Security;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Web;
using VisaNet.Presentation.Core.WebApiHttpCustomRequest;

namespace VisaNet.Presentation.Core.WebApi.Implementations.Implementations.Web
{
    public class WebSubscriberClientService : WebApiClientService, IWebSubscriberClientService
    {
        public WebSubscriberClientService(ITransactionContext transactionContext)
            : base("WebSubscriber", transactionContext)
        {

        }

        public Task Create(SubscriberDto subscriber)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestPut(BaseUri, TransactionContext, subscriber));
        }

        public Task DeleteByEmail(string email)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestGet(BaseUri + "/DeleteByEmail", TransactionContext,
                    new Dictionary<string, string> { { "email", email } }));
        }

        public Task<bool> ExistsEmail(string email)
        {
            return WebApiClient.CallApiServiceAsync<bool>(new WebApiHttpRequestGet(BaseUri + "/ExistsEmail", TransactionContext,
                    new Dictionary<string, string> { { "email", email } }));
        }
    }
}
