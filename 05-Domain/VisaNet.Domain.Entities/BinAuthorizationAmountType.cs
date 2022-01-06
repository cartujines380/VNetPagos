using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VisaNet.Domain.Entities.Base.Base;
using VisaNet.Domain.Entities.Enums;

namespace VisaNet.Domain.Entities
{
    [Table("BinAuthorizationAmountType")]
    [TrackChanges]
    public class BinAuthorizationAmountType : EntityBase
    {
        [Key]
        public override Guid Id { get; set; }
        public AuthorizationAmountType AuthorizationAmountType { get; set; }
        public DiscountType Law { get; set; }

        public Guid BinId { get; set; }
        public virtual Bin Bin{ get; set; }

    }
}
