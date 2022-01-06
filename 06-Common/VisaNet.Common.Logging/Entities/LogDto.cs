using System;
using VisaNet.Domain.Entities.Enums;

namespace VisaNet.Common.Logging.Entities
{
    public class LogDto
    {
        public LogDto()
        {
            DateTime = DateTime.Now;
            TransactionDateTime = DateTime.Now;
        }

        public Guid Id { get; set; }

        public string UserName { get; set; }
        public string IP { get; set; }
        public Guid TransactionIdentifier { get; set; }
        public DateTime TransactionDateTime { get; set; }
        public string SessionId { get; set; }
        public string RequestUri { get; set; }

        public Guid? SystemUserId { get; set; }
        public Guid? ApplicationUserId { get; set; }
        public Guid? AnonymousUserId { get; set; }

        public DateTime DateTime { get; set; }
        public LogType LogType { get; set; }
        public LogOperationType LogOperationType { get; set; }
        public LogUserType LogUserType { get; set; }
        public LogCommunicationType LogCommunicationType { get; set; }
        public string Message { get; set; }

        public bool IncludeCallCenterMessage { get; set; }
        public string CallCenterMessage { get; set; }

        public Exception Exception { get; set; }

        public string ExceptionMessage { get; set; }
        public string InnerException { get; set; }

        public LogPaymentCyberSourceDto LogPaymentCyberSource { get; set; }
        public TransactionType TransactionType { get; set; }
        public Guid TemporaryId { get; set; }
    }
}
