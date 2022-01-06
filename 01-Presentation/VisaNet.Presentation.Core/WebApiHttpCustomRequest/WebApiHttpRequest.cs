using System;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using VisaNet.Common.Security;
using VisaNet.Utilities.Cryptography;

namespace VisaNet.Presentation.Core.WebApiHttpCustomRequest
{
    public abstract class WebApiHttpRequest
    {
        protected WebApiHttpRequest(string uri, ITransactionContext transactionContext)
        {
            GetUri = new Uri(uri);
            TransactionContext = transactionContext;
        }

        protected Uri GetUri { get; private set; }
        protected ITransactionContext TransactionContext { get; private set; }

        public HttpClient GetHttpClient()
        {
            var httpClient = new HttpClient();
            var authorizationTokenUnEncrypted = string.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}",
                TransactionContext.IP,
                TransactionContext.UserName,
                TransactionContext.TransactionIdentifier,
                DateTime.Now.ToString(new CultureInfo("es-UY")),
                //TransactionContext.TransactionDateTime.ToString(new CultureInfo("es-UY")), //SE CAMBIA POR DateTime.Now PARA PROBAR SOLUCIONAR PROBLEMA DE PARSEO DE FECHA
                TransactionContext.RequestUri,
                TransactionContext.SystemUserId,
                TransactionContext.ApplicationUserId,
                TransactionContext.AnonymousUserId,
                TransactionContext.SessionId,
                TransactionContext.TraceId);

            httpClient.DefaultRequestHeaders.Add("Authorization-Token", AESSecurity.Encrypt(authorizationTokenUnEncrypted));
            return httpClient;
        }

        public abstract Task<HttpResponseMessage> Action();
    }
}
