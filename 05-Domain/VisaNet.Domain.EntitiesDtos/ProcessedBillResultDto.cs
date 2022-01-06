using System;
using VisaNet.Domain.Entities.Enums;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Domain.EntitiesDtos
{
    public class ProcessedBillResultDto
    {
        public Guid Id { get; set; }
        public BillType BillType { get; set; }
        public string BillExternalId { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string ExpirationDateMessage { get; set; }
        public string Currency { get; set; }
        public double Amount { get; set; }
        public bool AlreadyNotified { get; set; }
        public PaymentResultTypeDto PaymentResultType { get; set; }
        public string PaymentResultMessage { get; set; }
    }
}
