using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Input;
using Newtonsoft.Json;
using VisaNet.Common.Security;

namespace VisaNet.Presentation.Core.WebApiHttpCustomRequest
{
    public class WebApiHttpRequestPost : WebApiHttpRequest
    {
        private readonly Guid? _id;
        private readonly object _data;
        private readonly JsonSerializerSettings _settings;
        public WebApiHttpRequestPost(string uri, ITransactionContext transactionContext) : base(uri, transactionContext) { }
        public WebApiHttpRequestPost(string uri, ITransactionContext transactionContext, object data) : base(uri, transactionContext) { _data = data; }
        public WebApiHttpRequestPost(string uri, ITransactionContext transactionContext, Guid id, object data) : base(uri, transactionContext) { _id = id; _data = data; }
        public WebApiHttpRequestPost(string uri, ITransactionContext transactionContext, object data, JsonSerializerSettings settings) : base(uri, transactionContext) { _data = data; _settings = settings; }

        public async override Task<HttpResponseMessage> Action()
        {
            var uriBuilder = new UriBuilder(GetUri);

            if (_id != null)
            {
                var query = HttpUtility.ParseQueryString(uriBuilder.Query);
                query.Add("id", _id.ToString());
                uriBuilder.Query = query.ToString();
            }

            if (_settings == null)
            {
                return await GetHttpClient().PostAsJsonAsync(uriBuilder.Uri.ToString(), _data).ConfigureAwait(false);
            }

            //Custom serialization for interfaces
            var json = JsonConvert.SerializeObject(_data, typeof(ICommand), _settings);
            HttpContent jsonContent = new StringContent(json);
            return await GetHttpClient().PostAsync(uriBuilder.Uri, jsonContent).ConfigureAwait(false);
        }

    }
}
