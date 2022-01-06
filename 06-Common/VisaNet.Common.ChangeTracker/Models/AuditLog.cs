using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VisaNet.Common.ChangeTracker.Models
{
    /// <summary>
    /// This model class is used to store the changes made in datbase values
    /// For the audit purpose. Only selected tables can be tracked with the help of TrackChanges Attribute present in the common library.
    /// </summary>
    public class AuditLog
    {
        public AuditLog()
        {
            LogDetails = new List<AuditLogDetail>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AuditLogId { get; set; }

        public Guid TransactionIdentifier { get; set; }

        public string IP { get; set; }

        public string UserName { get; set; }

        [Required]
        public DateTime EventDate { get; set; }

        [Required]
        public EventType EventType { get; set; }

        [Required]
        [MaxLength(256)]
        public string TableName { get; set; }

        [Required]
        [MaxLength(256)]
        public string RecordId { get; set; }

        public virtual ICollection<AuditLogDetail> LogDetails { get; set; }
        public string AditionalInfo { get; set; }
    }
}
