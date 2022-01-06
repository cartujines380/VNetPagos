using System;

namespace VisaNet.Utilities.Cybersource
{
    public class KeysInfoForPaymentNewUser : KeysInfoForPayment
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Address { get; set; }

        public KeysInfoForPaymentNewUser()
        {
            TemporaryTransactionIdentifier = Guid.NewGuid().ToString();
        }

    }
}