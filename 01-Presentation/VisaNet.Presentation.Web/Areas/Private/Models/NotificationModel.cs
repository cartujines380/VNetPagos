using System;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Presentation.Web.Areas.Private.Models
{
    public class NotificationModel
    {
        public Guid Id { get; set; }

        public Guid? ServiceId { get; set; }
        public string ServiceImageName { get; set; }
        public string ServiceName { get; set; }
        public string ServiceDesc { get; set; }

        public DateTime Date { get; set; }

        public string Message { get; set; }

        public NotificationPruposeDto NotificationPrupose { get; set; }

        public Guid RegisteredUserId { get; set; }
        public string RegisteredUserEmail { get; set; }
        public string RegisteredUserMobileNumber { get; set; }
    }
}
