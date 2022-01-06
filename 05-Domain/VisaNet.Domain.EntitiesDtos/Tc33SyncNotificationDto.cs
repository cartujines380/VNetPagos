using System;
using VisaNet.Utilities.Notifications;

namespace VisaNet.Domain.EntitiesDtos
{
    public class Tc33SyncNotificationDto
    {
        public NotificationType Type { get; set; }

        public string UserName { get; set; }

        public string Message { get; set; }

        public Exception ExpectionError { get; set; }
    }
}
