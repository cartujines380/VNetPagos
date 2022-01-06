using System;
using VisaNet.Common.Logging.Entities;

namespace VisaNet.Domain.EntitiesDtos
{
    public class AuditLogDto
    {
        public Guid Id { get; set; }

        public string IP { get; set; }
        public Guid TransactionIdentifier { get; set; }

        public Guid? SystemUserId { get; set; }
        public Guid? AnonymousUserId { get; set; }
        public Guid? ApplicationUserId { get; set; }

        public DateTime DateTime { get; set; }
        public LogType LogType { get; set; }
        public LogOperationType LogOperationType { get; set; }
        public LogUserType LogUserType { get; set; }
        public LogCommunicationType LogCommunicationType { get; set; }
        public string Message { get; set; }

        public string ExceptionMessage { get; set; }
        public string InnerException { get; set; }

        public SystemUserDto SystemUser { get; set; }
        public AnonymousUserDto AnonymousUser { get; set; }
        public ApplicationUserDto ApplicationUser { get; set; }


    }
}
