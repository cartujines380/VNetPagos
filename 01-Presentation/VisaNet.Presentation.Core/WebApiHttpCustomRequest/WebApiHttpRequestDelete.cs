using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using VisaNet.Common.Security;

namespace VisaNet.Presentation.Core.WebApiHttpCustomRequest
{
    public class WebApiHttpRequestDelete : WebApiHttpRequest
    {
        private readonly Guid? _id;
        public WebApiHttpRequestDelete(string uri, ITransactionContext transactionContext) : base(uri, transactionContext) { }
        public WebApiHttpRequestDelete(string uri, ITransactionContext transactionContext, Guid id)
            : base(uri, transactionContext)
        {
            _id = id;
        }

        
        public async override Task<HttpResponseMessage> Action()
        {
            var uriBuilder = new UriBuilder(GetUri);

            if (_id != null)
            {
                var query = HttpUtility.ParseQueryString(uriBuilder.Query);
                query.Add("id", _id.ToString());
                uriBuilder.Query = query.ToString();
            }

            return await GetHttpClient().DeleteAsync(uriBuilder.Uri.ToString()).ConfigureAwait(false);
        }
    }
}
