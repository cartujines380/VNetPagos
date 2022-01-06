using System;
using System.Collections.Generic;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Presentation.Web.Areas.Private.Models
{
    public class BillModel
    {
        public  Guid Id { get; set; }

        public DateTime ExpirationDate { get; set; }
        public DateTime? GeneratedDate { get; set; }
        public double Amount { get; set; }
        public string Currency { get; set; }
        public string Description { get; set; }
        public string BillExternalId { get; set; }
        public bool Pay { get; set; }

        public bool FinalConsumer { get; set; }
        public int Discount { get; set; }
        public double TaxedAmount { get; set; }
        public string GatewayTransactionId { get; set; }

        public bool Payable { get; set; }

        public GatewayEnumDto Gateway { get; set; }

        //dato para sucive
        public string Line { get; set; }
        public bool HasAnnualPatent { get; set; }

        //dato para sistarbanc
        public List<BillModel> Bills { get; set; }
        
    }
}
