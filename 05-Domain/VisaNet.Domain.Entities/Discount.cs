using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VisaNet.Domain.Entities.Base.Audit;
using VisaNet.Domain.Entities.Base.Base;
using VisaNet.Domain.Entities.Enums;

namespace VisaNet.Domain.Entities
{
    [Table("Discounts")]
    [TrackChanges]
    public class Discount : EntityBase, IAuditable
    {
        [Key]
        [TrackChangesAditionalInfo(Index = 0)]
        public override Guid Id { get; set; }
        public CardType CardType { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public int Fixed { get; set; }
        public int Additional { get; set; }
        public int MaximumAmount { get; set; }

        [MaxLength(50)]
        public string CreationUser { get; set; }
        [MaxLength(50)]
        public string LastModificationUser { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastModificationDate { get; set; }

        public DiscountType DiscountType { get; set; }

        public DiscountLabelType DiscountLabel { get; set; }
    }
}