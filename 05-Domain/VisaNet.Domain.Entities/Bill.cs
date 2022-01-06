using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VisaNet.Domain.Entities.Base.Audit;
using VisaNet.Domain.Entities.Base.Base;

namespace VisaNet.Domain.Entities
{
    [Table("Bills")]
    public class Bill : EntityBase, IAuditable
    {
        [Key]
        public override Guid Id { get; set; }
        public DateTime ExpirationDate { get; set; }

        [MaxLength(250)]
        public string Description { get; set; }        
        public double Amount { get; set; }
        [MaxLength(5)]
        public string Currency { get; set; }
        
        [MaxLength(100)]
        public string BillExternalId { get; set; }
        //[MaxLength(100)]
        //public string GatewayTransactionId { get; set; }
        [MaxLength(100)]
        public string GatewayTransactionId { get; set; }

        
        public Guid? PaymentId { get; set; }
        public virtual Payment Payment { get; set; }

        public bool FinalConsumer { get; set; }
        public double TaxedAmount { get; set; }
        public int Discount { get; set; }
        public double DiscountAmount { get; set; }
        
        //usado para realizar un reverso
        public string SucivePreBillNumber { get; set; }
        
        [MaxLength(50)]
        public string CreationUser { get; set; }
        [MaxLength(50)]
        public string LastModificationUser { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastModificationDate { get; set; }
    }
}
