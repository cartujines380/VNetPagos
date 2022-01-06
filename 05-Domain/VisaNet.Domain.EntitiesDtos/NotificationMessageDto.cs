using System;
using VisaNet.Domain.Entities;
using VisaNet.Domain.Entities.Enums;

namespace VisaNet.Domain.EntitiesDtos
{
    public class NotificationMessageDto
    {
        protected NotificationMessageDto()
        {
            CreationDateTime = DateTime.Now;
        }


        public Guid Id { get; set; }

        public DateTime CreationDateTime { get; set; }
        public int SendIntents { get; set; }
        public DateTime? SendDateTime { get; set; }
        public DateTime? LastSendIntentDateTime { get; set; }

        public Guid? ApplicationUserId { get; set; }

        public ApplicationUser ApplicationUser { get; set; }
    }
}