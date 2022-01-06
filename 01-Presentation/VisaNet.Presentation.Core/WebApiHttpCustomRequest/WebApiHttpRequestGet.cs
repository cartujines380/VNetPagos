using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using VisaNet.Common.Security;

namespace VisaNet.Presentation.Core.WebApiHttpCustomRequest
{
    public class WebApiHttpRequestGet : WebApiHttpRequest
    {
        private readonly IDictionary<string, string> _parameters;

        public WebApiHttpRequestGet(string uri, ITransactionContext transactionContext) : base(uri, transactionContext) { }

        public WebApiHttpRequestGet(string uri, ITransactionContext transactionContext, IDictionary<string, string> parameters)
            : base(uri, transactionContext)
        {
            _parameters = parameters;
        }


        public async override Task<HttpResponseMessage> Action()
        {
            var uriBuilder = new UriBuilder(GetUri);
            if (_parameters != null)
            {
                var query = HttpUtility.ParseQueryString(uriBuilder.Query);
                foreach (var param in _parameters) { query.Add(param.Key, param.Value); }
                uriBuilder.Query = query.ToString();
            }

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = uriBuilder.Uri,
            };

            return await GetHttpClient().SendAsync(request).ConfigureAwait(false);
        }
    }
}
