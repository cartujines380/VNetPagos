using System;
using System.Collections.Generic;

namespace VisaNet.Presentation.VisaNetOn.Models
{
    public class PagePaymentModel : PageModel
    {
        public bool RememberUser { get; set; }
        public bool EnableRememberUser { get; set; }

        public int Quotas { get; set; }

        //Bill & Discount
        public BillData BillData { get; set; }
        public DiscountData DiscountData { get; set; }
    }

    public class BillData
    {
        public string ExternalId { get; set; }
        public double Amount { get; set; }
        public double TaxedAmount { get; set; }
        public string Currency { get; set; }
        public string CurrencySymbol { get; set; }
        public bool FinalConsumer { get; set; }
        public DateTime GenerationDate { get; set; }
        public bool AcceptQuotas { get; set; }
        public string Description { get; set; }
        public List<LineData> BillsDetails { get; set; }
    }

    public class DiscountData
    {
        public double CybersourceAmount { get; set; }
        public Guid DiscountObjId { get; set; }
        public double DiscountAmount { get; set; }
        public double Amount { get; set; }
        public double TaxedAmount { get; set; }
        public int DiscountType { get; set; }
        public int BillDiscount { get; set; }
    }

    public class LineData
    {
        public int Order { get; set; }
        public string Concept { get; set; }
        public double Amount { get; set; }
    }

}