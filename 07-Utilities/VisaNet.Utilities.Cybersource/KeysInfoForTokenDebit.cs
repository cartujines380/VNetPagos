using System;

namespace VisaNet.Utilities.Cybersource
{
    public abstract class KeysInfoForTokenDebit : KeysInfoForToken
    {
        public string CommerceAndProductName { get; set; }
        public Guid CommerceId { get; set; }
        public Guid ProductId { get; set; }
    }
}