using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VisaNet.Domain.Entities.Base.Audit;
using VisaNet.Domain.Entities.Enums;

namespace VisaNet.Domain.Entities
{
    [Table("Emails")]
    public class EmailMessage : NotificationMessage, IAuditable
    {
        public string To { get; set; }

        public MailgunStatus Status { get; set; }

        public EmailType EmailType { get; set; }

        public string DataByType { get; set; }

        public string MailgunDescription { get; set; }

        public virtual EmailMessage Parent { get; set; }

        public Guid? ParentId { get; set; }

        public string Body { get; set; }

        [MaxLength(254)]
        [Index("ixkh_emails_01", 1, IsUnique = true)]
        public string MailgunId { get; set; }

        [SkipTracking]
        [MaxLength(50)]
        public string CreationUser { get; set; }
        [SkipTracking]
        [MaxLength(50)]
        public string LastModificationUser { get; set; }
        [SkipTracking]
        public DateTime CreationDate { get; set; }
        [SkipTracking]
        public DateTime LastModificationDate { get; set; }

        public string MailgunErrorDescription { get; set; }
    }
}