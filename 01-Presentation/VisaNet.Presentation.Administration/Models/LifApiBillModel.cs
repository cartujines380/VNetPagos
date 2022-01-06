using System;
using VisaNet.Presentation.Administration.CustomAttributes;

namespace VisaNet.Presentation.Administration.Models
{
    public class LifApiBillModel 
    {
        public Guid Id { get; set; }

        [CustomDisplay("LifApiBill_AppId")]
        public string AppId { get; set; }
        [CustomDisplay("LifApiBill_OperationId")]
        public string OperationId { get; set; }

        [CustomDisplay("LifApiBill_Currency")]
        public string Currency { get; set; }
        [CustomDisplay("LifApiBill_Amount")]
        public double Amount { get; set; }
        [CustomDisplay("LifApiBill_TaxedAmount")]
        public double TaxedAmount { get; set; }
        [CustomDisplay("LifApiBill_IsFinalConsumer")]
        public bool IsFinalConsumer { get; set; }
        [CustomDisplay("LifApiBill_LawId")]
        public int LawId { get; set; }
        [CustomDisplay("LifApiBill_BinValue")]
        public string BinValue { get; set; }

        [CustomDisplay("LifApiBill_CardType")]
        public string CardType { get; set; }
        [CustomDisplay("LifApiBill_IssuingCompany")]
        public string IssuingCompany { get; set; }
        [CustomDisplay("LifApiBill_DiscountAmount")]
        public double DiscountAmount { get; set; }
        [CustomDisplay("LifApiBill_AmountToCyberSource")]
        public double AmountToCyberSource { get; set; }
        [CustomDisplay("LifApiBill_CreationDate")]
        public DateTime CreationDate { get; set; }
        [CustomDisplay("LifApiBill_Error")]
        public string Error { get; set; }

        
    }
}
