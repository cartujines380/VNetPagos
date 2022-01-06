using System;

namespace VisaNet.Common.Logging.Entities
{
    public class AuditTransactionLogDto
    {
        public Int64 RowNum { get; set; }
        public Guid TransactionIdentifier { get; set; }
        public Guid? SystemUserId { get; set; }
        public Guid? ApplicationUserId { get; set; }
        public Guid? AnonymousUserId { get; set; }
        public string IP { get; set; }
        public DateTime TransactionDateTime { get; set; }
        public string LDAPUserName { get; set; }
        public string ApplicationUserEmail { get; set; }
        public string AnonymousUserEmail { get; set; }
        public LogOperationType LogOperationType { get; set; }
        public LogUserType LogUserType { get; set; }
        public Int32 TotalRows { get; set; }
        public LogType LogType { get; set; }
        public LogCommunicationType LogCommunicationType { get; set; }
        public string Message { get; set; }
    }
}
