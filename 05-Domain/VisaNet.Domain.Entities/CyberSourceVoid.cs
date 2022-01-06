using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VisaNet.Domain.Entities.Base.Base;

namespace VisaNet.Domain.Entities
{
    [Table("CyberSourceVoids")]
    public class CyberSourceVoid : EntityBase
    {
        [Key]
        public override Guid Id { get; set; }
        public string TransactionId { get; set; }
        public DateTime DateTime { get; set; }
        public string VoidNumber { get; set; }
        public int? VoidCode { get; set; }
        public string ReverseNumber { get; set; }
        public int? ReverseCode { get; set; }
        public string RefundNumber { get; set; }
        public int? RefundCode { get; set; }
        public Guid CyberSourceAcknowledgementId { get; set; }
        public virtual CyberSourceAcknowledgement CyberSourceAcknowledgement { get; set; }
    }
}