using System;
using System.Net.Http;
using System.Threading.Tasks;
using VisaNet.Common.Security;

namespace VisaNet.Presentation.Core.WebApiHttpCustomRequest
{
    public class WebApiHttpRequestPut : WebApiHttpRequest
    {
        private readonly object _data;
        public WebApiHttpRequestPut(string uri, ITransactionContext transactionContext) : base(uri, transactionContext) { }
        public WebApiHttpRequestPut(string uri, ITransactionContext transactionContext, object data) : base(uri, transactionContext) { _data = data; }

        public async override Task<HttpResponseMessage> Action()
        {
            var uriBuilder = new UriBuilder(GetUri);
            
            return await GetHttpClient().PutAsJsonAsync(uriBuilder.Uri.ToString(), _data).ConfigureAwait(false);
        }
    }
}
