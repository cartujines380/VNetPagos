using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VisaNet.Domain.Entities.Base.Audit;
using VisaNet.Domain.Entities.Base.Base;
using VisaNet.Domain.Entities.Enums;

namespace VisaNet.Domain.Entities
{
    [Table("ConciliationVisanetCallback")]
    public class ConciliationVisanetCallback : EntityBase, IAuditable
    {
        [Key]
        public override Guid Id { get; set; }

        public string RegisterType { get; set; }
        public string FileId { get; set; }
        public string RegisterId { get; set; }
        public string IgnoreCode { get; set; }
        public string Merchant { get; set; }
        public string Branch { get; set; }
        public string Currency { get; set; }
        public string SalePlan { get; set; }
        public string Refund { get; set; }
        public string CardMaskedNumber { get; set; }
        public float Amount { get; set; }
        public string AuthorizationCode { get; set; }
        public string CyberSourceId { get; set; }
        public VisanetCallbackState State { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public DateTime RegisterDate { get; set; }
        public DateTime TimeStamp { get; set; }

        public string CreationUser { get; set; }
        public string LastModificationUser { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastModificationDate { get; set; }
    }
}
