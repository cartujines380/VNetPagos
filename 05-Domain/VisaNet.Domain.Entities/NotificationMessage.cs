using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VisaNet.Domain.Entities.Base.Base;

namespace VisaNet.Domain.Entities
{
    [Table("NotificationMessages")]
    public abstract class NotificationMessage : EntityBase
    {
        protected NotificationMessage()
        {
            CreationDateTime = DateTime.Now;
        }

        [Key]
        public override Guid Id { get; set; }

        public DateTime CreationDateTime { get; set; }
        public int SendIntents { get; set; }
        public DateTime? SendDateTime { get; set; }
        public DateTime? LastSendIntentDateTime { get; set; }

        public Guid? ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        public ApplicationUser ApplicationUser { get; set; }
    }
}