using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VisaNet.Domain.Entities.Base.Audit;
using VisaNet.Domain.Entities.Base.Base;
using VisaNet.Domain.Entities.Enums;

namespace VisaNet.Domain.Entities
{
    [Table("Bin")]
    [TrackChanges]
    public class Bin : EntityBase, IAuditable
    {
        [Key]
        public override Guid Id { get; set; }
        public Guid GatewayId { get; set; }
        public virtual Gateway Gateway { get; set; }
        [MaxLength(50)]
        [TrackChangesAditionalInfo(Index = 0)]
        public string Name { get; set; }
        [MaxLength(250)]
        public string Description { get; set; }
        [Index(IsUnique = true)]
        public int Value { get; set; }

        [MaxLength(250)]
        public string ImageName { get; set; }

        //TODO: Las imagenes como refereencias se van a eliminar en un proximo release
        public Guid? ImageId { get; set; }
        public virtual Image Image { get; set; }

        public CardType CardType { get; set; }
        //public AuthorizationAmountType AuthorizationAmountType { get; set; }

        public Guid? BankId { get; set; }
        public virtual Bank Bank { get; set; }

        public virtual ICollection<BinGroup> BinGroups { get; set; }

        [StringLength(2)]
        public string Country { get; set; }

        public bool Active { get; set; }

        public bool EditedFromBO { get; set; }

        [MaxLength(6)]
        public string IssuerBin { get; set; }

        [MaxLength(6)]
        public string ProcessorBin { get; set; }

        [SkipTracking]
        public string CreationUser { get; set; }
        [SkipTracking]
        public string LastModificationUser { get; set; }
        [SkipTracking]
        public DateTime CreationDate { get; set; }
        [SkipTracking]
        public DateTime LastModificationDate { get; set; }

        public virtual ICollection<BinAuthorizationAmountType> BinAuthorizationAmountTypeList { get; set; }

        public Guid? AffiliationCardId { get; set; }
        public virtual AffiliationCard AffiliationCard { get; set; }

        public override bool Equals(object obj)
        {
            var objBin = (Bin)obj;
            return this.Value.Equals(objBin.Value);
        }

    }
}
