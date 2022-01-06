using System;

namespace VisaNet.Utilities.Cybersource
{
    public abstract class KeysInfoForPayment : KeysInfoBasic
    {
        public double CybersourceAmount { get; set; }
        
        public int Quotas { get; set; }

        public BillForToken Bill { get; set; }

        //MDD30 - 
        public Guid ServiceId { get; set; }
        //MDD31 - 
        public Guid UserId { get; set; }
        //MDD32 - 
        public Guid CardId { get; set; }
        //MDD33 -
        public Guid GatewayId { get; set; }
        //MDD34 -
        public Guid DiscountObjId { get; set; }

        // no se estan usando en cybersource access

        //MDD45 -
        //public string DescriptionService { get; set; }

        //MDD53 -
        public string Field48 { get; set; }
    }
}