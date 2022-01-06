using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VisaNet.Domain.Entities.Base.Audit;
using VisaNet.Domain.Entities.Base.Base;

namespace VisaNet.Domain.Entities
{
    [Table("LifApiBills")]
    [TrackChanges]
    public class LifApiBill : EntityBase, IAuditable
    {
        [Key]
        [TrackChangesAditionalInfo]
        public override Guid Id { get; set; }

        public string AppId { get; set; }
        public string OperationId { get; set; }

        public string Currency { get; set; }
        public double Amount { get; set; }
        public double TaxedAmount { get; set; }
        public bool IsFinalConsumer { get; set; }
        public int LawId { get; set; }

        public string BinValue { get; set; }
        [MaxLength(1)]
        public string CardType { get; set; }
        public string IssuingCompany { get; set; }
        public double DiscountAmount { get; set; }
        public double AmountToCyberSource { get; set; }

        public string Error { get; set; }

        public string CreationUser { get; set; }
        public string LastModificationUser { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastModificationDate { get; set; }
    }
}