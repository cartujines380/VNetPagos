using System;
using System.Collections.Generic;

namespace VisaNet.Domain.EntitiesDtos.ChangeTracker
{
    /// <summary>
    /// This model class is used to store the changes made in datbase values
    /// For the audit purpose. Only selected tables can be tracked with the help of TrackChanges Attribute present in the common library.
    /// </summary>
    public class AuditLogDto
    {
        public AuditLogDto()
        {
            LogDetails = new List<AuditLogDetailDto>();
        }

       
        public int AuditLogId { get; set; }

        public Guid TransactionIdentifier { get; set; }

        public string IP { get; set; }

        public string UserName { get; set; }

        public DateTime EventDate { get; set; }

        public EventTypeDto EventType { get; set; }

        public string TableName { get; set; }

        public string RecordId { get; set; }

        public ICollection<AuditLogDetailDto> LogDetails { get; set; }

        public string AditionalInfo { get; set; }

    }
}
