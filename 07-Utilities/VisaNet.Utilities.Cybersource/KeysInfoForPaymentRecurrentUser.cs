using System;

namespace VisaNet.Utilities.Cybersource
{
    public class KeysInfoForPaymentRecurrentUser : KeysInfoForPayment
    {
        public KeysInfoForPaymentRecurrentUser()
        {
            TemporaryTransactionIdentifier = Guid.NewGuid().ToString();
        }
    }
}