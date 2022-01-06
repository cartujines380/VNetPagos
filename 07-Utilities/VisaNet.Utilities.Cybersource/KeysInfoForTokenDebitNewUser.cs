using System;

namespace VisaNet.Utilities.Cybersource
{
    public class KeysInfoForTokenDebitNewUser : KeysInfoForTokenDebit
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string MobileNumber { get; set; }
        public string IdentityNumber { get; set; }
        public string Password { get; set; }

        public KeysInfoForTokenDebitNewUser()
        {
            TemporaryTransactionIdentifier = Guid.NewGuid().ToString();
        }
    }
}