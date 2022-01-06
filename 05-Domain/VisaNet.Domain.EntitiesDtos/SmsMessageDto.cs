using System;
using VisaNet.Domain.Entities;
using VisaNet.Domain.Entities.Enums;

namespace VisaNet.Domain.EntitiesDtos
{
    public class SmsMessageDto
    {
        public Guid Id { get; set; }

        public DateTime CreationDateTime { get; set; }
        public int SendIntents { get; set; }
        public DateTime? SendDateTime { get; set; }
        public DateTime? LastSendIntentDateTime { get; set; }

        public Guid? ApplicationUserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }

        public SmsType SmsType { get; set; }
        public string Message { get; set; }
        public string MobileNumber { get; set; }
        public SmsStatus SmsStatus { get; set; }
    }
}
