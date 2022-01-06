using System;
using VisaNet.Domain.EntitiesDtos;

namespace VisaNet.Utilities.Cybersource
{
    public class KeysInfoForTokenRegisteredUser : KeysInfoForToken
    {
        public NotificationConfigDto NotificationsConfig { get; set; }

        public KeysInfoForTokenRegisteredUser()
        {
            TemporaryTransactionIdentifier = Guid.NewGuid().ToString();
        }
    }
}