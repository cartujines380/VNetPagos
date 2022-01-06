using System;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Domain.EntitiesDtos
{
    public class ConciliationVisanetCallbackDto
    {
        public Guid Id { get; set; }
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
        public VisanetCallbackStateDto State { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public DateTime RegisterDate { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
