using System;
using System.ComponentModel.DataAnnotations;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Domain.EntitiesDtos
{
    public class NotificationDto
    {
        public Guid Id { get; set; }

        public Guid? ServiceId { get; set; }
        public virtual ServiceDto Service { get; set; }

        public DateTime Date { get; set; }
        
        [StringLength(500, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string Message { get; set; }

        public NotificationPruposeDto NotificationPrupose { get; set; }

        public Guid RegisteredUserId { get; set; }
        public virtual ApplicationUserDto RegisteredUser { get; set; }
    }
}
