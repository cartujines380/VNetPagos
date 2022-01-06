using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VisaNet.Domain.Entities.Base.Base;

namespace VisaNet.Domain.Entities
{
    [Table("VonData")]
    public class VonData : EntityBase
    {
        //Los registros son unicos por [AppId, UserExternalId, CardExternalId] y tambien por [AppId, AnonymousUserId, CardExternalId]

        //Service
        [Key]
        [MaxLength(50)]
        public string AppId { get; set; }

        //User
        public Guid AnonymousUserId { get; set; }
        [ForeignKey("AnonymousUserId")]
        public virtual AnonymousUser AnonymousUser { get; set; }
        [Key]
        [MaxLength(36)]
        public string UserExternalId { get; set; }
        [MaxLength(100)]
        public string ReferenceNumber { get; set; }
        [MaxLength(100)]
        public string ReferenceNumber2 { get; set; }
        [MaxLength(100)]
        public string ReferenceNumber3 { get; set; }
        [MaxLength(100)]
        public string ReferenceNumber4 { get; set; }
        [MaxLength(100)]
        public string ReferenceNumber5 { get; set; }
        [MaxLength(100)]
        public string ReferenceNumber6 { get; set; }

        //Card
        [Key]
        [MaxLength(36)]
        public string CardExternalId { get; set; }
        [MaxLength(50)]
        public string CardName { get; set; }
        [MaxLength(25)]
        public string CardMaskedNumber { get; set; }
        [MaxLength(50)]
        public string CardToken { get; set; }
        public DateTime CardDueDate { get; set; }

        //Additional
        public DateTime CreationDate { get; set; }
    }
}