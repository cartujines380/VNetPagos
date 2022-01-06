using System;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Domain.EntitiesDtos
{
    public class EmailMessageDto : NotificationMessageDto
    {
        public string To { get; set; }

        public MailgunStatusDto Status { get; set; }

        public EmailTypeDto EmailType { get; set; }

        public string DataByType { get; set; }

        public string MailgunDescription { get; set; }

        public virtual EmailMessageDto Parent { get; set; }

        public Guid? ParentId { get; set; }

        public string Body { get; set; }

        public string MailgunErrorDescription { get; set; }
    }
}