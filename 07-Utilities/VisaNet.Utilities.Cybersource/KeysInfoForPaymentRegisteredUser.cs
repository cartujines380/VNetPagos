using System;

namespace VisaNet.Utilities.Cybersource
{
    public class KeysInfoForPaymentRegisteredUser : KeysInfoForPayment
    {
        public KeysInfoForPaymentRegisteredUser()
        {
            TemporaryTransactionIdentifier = Guid.NewGuid().ToString();
        }
    }
}