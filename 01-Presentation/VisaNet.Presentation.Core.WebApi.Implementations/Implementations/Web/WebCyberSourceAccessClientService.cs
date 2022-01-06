using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using VisaNet.Common.Security;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Web;
using VisaNet.Presentation.Core.WebApiHttpCustomRequest;
using VisaNet.Utilities.Cybersource;

namespace VisaNet.Presentation.Core.WebApi.Implementations.Implementations.Web
{
    public class WebCyberSourceAccessClientService : WebApiClientService, IWebCyberSourceAccessClientService
    {
        public WebCyberSourceAccessClientService(ITransactionContext transactionContext)
            : base("WebCyberSourceAccess", transactionContext)
        {

        }

        public Task<IDictionary<string, string>> GenerateKeys(IGenerateToken item)
        {
            var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };
            return WebApiClient.CallApiServiceAsync<IDictionary<string, string>>(new WebApiHttpRequestPost(
                             BaseUri + "/GenerateKeys", TransactionContext, item, settings));
        }

    }
}
