using System;
using System.ComponentModel.DataAnnotations;

namespace VisaNet.Common.Logging.Entities
{
    public class Log
    {
        [Key]
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

        public string ExceptionMessage { get; set; }
        public string InnerException { get; set; }

        public virtual LogPaymentCyberSource LogPaymentCyberSource { get; set; }
        public Guid? TemporaryId { get; set; }

        /// <summary>
        /// Check if this entity is transient, ie, without identity at this moment.
        /// </summary>
        /// <returns>True if entity is transient, else false.</returns>
        public bool IsTransient()
        {
            return Id == Guid.Empty;
        }

        /// <summary>
        /// Generate identity for this entity
        /// </summary>
        public void GenerateNewIdentity()
        {
            if (IsTransient()) Id = Guid.NewGuid();
        }

    }
}
