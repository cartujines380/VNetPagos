using System;
using VisaNet.Domain.EntitiesDtos;

namespace VisaNet.Utilities.Cybersource
{
    public class KeysInfoForTokenRecurrentUser : KeysInfoForToken
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string MobileNumber { get; set; }
        public string IdentityNumber { get; set; }
        public string Password { get; set; }

        public NotificationConfigDto NotificationsConfig { get; set; }

        public KeysInfoForTokenRecurrentUser()
        {
            TemporaryTransactionIdentifier = Guid.NewGuid().ToString();
        }
    }
}