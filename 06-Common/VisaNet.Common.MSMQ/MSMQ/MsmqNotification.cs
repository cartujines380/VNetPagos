using System;

namespace VisaNet.Common.MSMQ.MSMQ
{

    public enum MsmqNotificationType
    {
        ProcessAll = 0,
        Mail = 1,
        SMS = 2,
    }

    [Serializable]
    public class MsmqNotification
    {
        public MsmqNotification()
        {
            MsmqNotificationType = MsmqNotificationType.ProcessAll;
        }

        public MsmqNotificationType MsmqNotificationType { get; set; }
        public Guid? NotificationId { get; set; }

    }
}
