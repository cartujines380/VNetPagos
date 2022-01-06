using System;

namespace VisaNet.Common.Security
{
    public interface IWebApiTransactionContext
    {
        string UserName { get; }
        Guid TransactionIdentifier { get; set; }
        DateTime TransactionDateTime { get; set; }
        string IP { get; }
        string RequestUri { get; }
        string SystemUserId { get; }
        string ApplicationUserId { get; set; }
        string AnonymousUserId { get; }
        string SessionId { get; }
        Guid TraceId { get; }
    }
}
