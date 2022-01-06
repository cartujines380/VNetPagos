using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VisaNet.Domain.Entities.Base.Audit;
using VisaNet.Domain.Entities.Base.Base;

namespace VisaNet.Domain.Entities
{
    [Table("Parameters")]
    [TrackChanges]
    public class Parameters: EntityBase, IAuditable
    {
        [Key]
        [TrackChangesAditionalInfo(Index = 0)]
        public override Guid Id { get; set; }

        public Email Contact { get; set; }

        public Email ErrorNotification { get; set; }

        public Email SendingEmail { get; set; }

        public BankCode Banred { get; set; }
        public BankCode Sistarbanc { get; set; }
        public BankCode SistarbancBrou { get; set; }
        public BankCode Cybersource { get; set; }
        public BankCode Sucive { get; set; }
        public BankCode Geocom { get; set; }

        [MaxLength(100)]
        public string MerchantId { get; set; }
        [MaxLength(100)]
        public string CybersourceProfileId { get; set; }
        [MaxLength(100)]
        public string CybersourceAccessKey { get; set; }
        [MaxLength(500)]
        public string CybersourceSecretKey { get; set; }
        [MaxLength(500)]
        public string CybersourceTransactionKey { get; set; }

        public int LoginNumberOfTries { get; set; }
        [MaxLength(50)]
        public string CreationUser { get; set; }
        [MaxLength(50)]
        public string LastModificationUser { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastModificationDate { get; set; }
    }
}
