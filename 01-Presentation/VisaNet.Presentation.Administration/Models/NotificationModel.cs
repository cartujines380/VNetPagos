using System;
using System.ComponentModel.DataAnnotations;
using VisaNet.Domain.EntitiesDtos.CustomAttributes;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Presentation.Administration.Models
{
    public class NotificationModel
    {
        public Guid Id { get; set; }

        [CustomDisplay("Notifications_Level")]
        public FixedNotificationLevelDto Level { get; set; }

        [CustomDisplay("Notifications_Category")]
        public FixedNotificationCategoryDto Category { get; set; }

        [CustomDisplay("Notifications_DateTime")]
        public DateTime DateTime { get; set; }

        [CustomDisplay("Notifications_Description")]
        public string Description { get; set; }

        [CustomDisplay("Notifications_Detail")]
        public string Detail { get; set; }

        [CustomDisplay("Notifications_Resolved")]
        public bool Resolved { get; set; }

        [CustomDisplay("Notifications_Comments")]
        [Required(AllowEmptyStrings = false)]
        public string Comments { get; set; }
    }
}