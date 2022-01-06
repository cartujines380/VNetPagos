using System;
using System.Linq;
using System.Web;
using VisaNet.Utilities.Cryptography;

namespace VisaNet.Common.Security
{
    public class WebApiTransactionContext : IWebApiTransactionContext
    {
        public WebApiTransactionContext(bool loadDataFromHttpContext = true)
        {
            if (loadDataFromHttpContext && HttpContext.Current != null)
            {
                var token = HttpContext.Current.Request.Headers.GetValues("Authorization-Token").First();
                var decryptedToken = AESSecurity.Decrypt(token);

                var decryptedTokenValues = decryptedToken.Split('|');

                IP = decryptedTokenValues[0];
                UserName = decryptedTokenValues[1];
                TransactionIdentifier = Guid.Parse(decryptedTokenValues[2]);
                TransactionDateTime = DateTime.Parse(decryptedTokenValues[3]);
                RequestUri = decryptedTokenValues[4];
                SystemUserId = decryptedTokenValues[5];
                ApplicationUserId = decryptedTokenValues[6];
                AnonymousUserId = decryptedTokenValues[7];
                SessionId = decryptedTokenValues[8];
                TraceId = Guid.Parse(decryptedTokenValues[9]);
            }
            else
            {
                UserName = string.Empty;
                TransactionIdentifier = Guid.Empty;
                IP = string.Empty;
            }
        }

        public string UserName { get; private set; }
        public Guid TransactionIdentifier { get; set; }
        public DateTime TransactionDateTime { get; set; }
        public string IP { get; private set; }
        public string RequestUri { get; private set; }
        public string SystemUserId { get; private set; }
        public string ApplicationUserId { get; set; }
        public string AnonymousUserId { get; set; }
        public string SessionId { get; private set; }
        public Guid TraceId { get; private set; }
    }
}