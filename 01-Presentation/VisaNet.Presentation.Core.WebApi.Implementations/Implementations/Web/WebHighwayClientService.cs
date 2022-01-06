using System.Threading.Tasks;
using VisaNet.Common.Security.Mailgun;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Web;
using VisaNet.Presentation.Core.WebApiHttpCustomRequest;

namespace VisaNet.Presentation.Core.WebApi.Implementations.Implementations.Web
{
    public class WebHighwayClientService : WebApiClientService, IWebHighwayClientService
    {
        public WebHighwayClientService(IMailgunTransactionContext transactionContext)
            : base("WebHighway", transactionContext)
        {
        }

        public Task ProccessEmail(HighwayEmailDto email)
        {
            return WebApiClient.CallApiServiceAsync(new WebApiHttpRequestPut(BaseUri + "/ProccessEmail", TransactionContext, email));
        }

    }
}