using System;
using System.ComponentModel.DataAnnotations.Schema;
using VisaNet.Domain.Entities.Base.Audit;
using VisaNet.Domain.Entities.Enums;

namespace VisaNet.Domain.Entities
{
    [Table("Sms")]
    public class SmsMessage : NotificationMessage, IAuditable
    {
        public SmsType SmsType { get; set; }
        public string Message { get; set; }
        public string MobileNumber { get; set; }
        public SmsStatus SmsStatus { get; set; }

        public string CreationUser { get; set; }
        public string LastModificationUser { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastModificationDate { get; set; }
    }
}
