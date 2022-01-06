using System;

namespace VisaNet.Domain.EntitiesDtos
{
    public class CyberSourceVoidDto
    {
        public Guid Id { get; set; }
        public string TransactionId { get; set; }
        public DateTime DateTime { get; set; }
        public string VoidNumber { get; set; }
        public int? VoidCode { get; set; }
        public string ReverseNumber { get; set; }
        public int? ReverseCode { get; set; }
        public string RefundNumber { get; set; }
        public int? RefundCode { get; set; }
        public Guid CyberSourceAcknowledgementId { get; set; }
        public virtual CyberSourceAcknowledgementDto CyberSourceAcknowledgementDto { get; set; }
    }
}