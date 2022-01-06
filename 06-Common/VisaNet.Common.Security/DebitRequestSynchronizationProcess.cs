using System;

namespace VisaNet.Common.Security
{
    public class DebitRequestSynchronizationProcess : IWebApiTransactionContext
    {
        public DebitRequestSynchronizationProcess()
        {
            IP = "Localhost";
            UserName = "DebitRequestSynchronizationProcess";
            TransactionIdentifier = Guid.NewGuid();
            RequestUri = "-";
            TransactionDateTime = DateTime.Now;
            TraceId = Guid.NewGuid();
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
        public Guid TraceId { get; set; }
    }
}
