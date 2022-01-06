using System;

namespace VisaNet.Utilities.Cybersource
{
    public class KeysInfoForPaymentAnonymousUser : KeysInfoForPayment
    {
        public KeysInfoForPaymentAnonymousUser()
        {
            TemporaryTransactionIdentifier = Guid.NewGuid().ToString();
        }
    }
}