using System;
using VisaNet.Domain.EntitiesDtos;

namespace VisaNet.Utilities.Cybersource
{
    public abstract class KeysInfoForToken : KeysInfoBasic
    {
        //MDD31 - 
        public Guid UserId { get; set; }
        //MDD32 - 
        public Guid CardId { get; set; }
        //MDD30 - 
        public Guid ServiceId { get; set; }

    }
}