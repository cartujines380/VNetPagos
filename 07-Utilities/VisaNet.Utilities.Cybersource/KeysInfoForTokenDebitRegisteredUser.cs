using System;

namespace VisaNet.Utilities.Cybersource
{
    public class KeysInfoForTokenDebitRegisteredUser : KeysInfoForTokenDebit
    {
        public KeysInfoForTokenDebitRegisteredUser()
        {
            TemporaryTransactionIdentifier = Guid.NewGuid().ToString();
        }
    }
}