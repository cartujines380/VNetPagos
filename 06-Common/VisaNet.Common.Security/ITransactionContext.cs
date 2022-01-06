using System;

namespace VisaNet.Common.Security
{
    public interface ITransactionContext
    {
        string UserName { get; }
        Guid TransactionIdentifier { get; }
        DateTime TransactionDateTime { get; set; }
        string IP { get; }
        string RequestUri { get; }
        string SystemUserId { get; }
        string ApplicationUserId { get; }
        string AnonymousUserId { get; }
        string SessionId { get; }
        Guid TraceId { get; }
    }
}
