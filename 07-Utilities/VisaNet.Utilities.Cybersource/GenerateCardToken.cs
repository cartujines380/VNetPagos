using System;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Utilities.Cybersource
{
    public class GenerateCardToken : IGenerateToken
    {
        public GenerateCardToken()
        {
            UserId = Guid.Empty;
            ServiceId = Guid.Empty;
            TransactionReferenceNumber = string.Empty;
            RedirctTo = string.Empty;
            NameTh = string.Empty;
            CyberSourceIdentifier = string.Empty;
            CardBin = string.Empty;
            CallcenterUser = string.Empty;
            OperationId = string.Empty;
            Platform = string.Empty;
            TemporaryTransactionIdentifier = Guid.NewGuid().ToString();
            PaymentTypeDto = PaymentTypeDto.Manual;
            UrlReturn = string.Empty;
            DescriptionService = string.Empty;
            ReferenceNumber1 = string.Empty;
            ReferenceNumber2 = string.Empty;
            ReferenceNumber3 = string.Empty;
            ReferenceNumber4 = string.Empty;
            ReferenceNumber5 = string.Empty;
            ReferenceNumber6 = string.Empty;
        }

        public Guid UserId { get; set; }
        public Guid ServiceId { get; set; }
        public string TransactionReferenceNumber { get; set; }
        public string CyberSourceIdentifier { get; set; }
        public string RedirctTo { get; set; }
        public string NameTh { get; set; }
        public string CardBin { get; set; }
        public string CallcenterUser { get; set; }
        public string OperationId { get; set; }
        public string Platform { get; set; }
        public string TemporaryTransactionIdentifier { get; set; }
        public PaymentTypeDto PaymentTypeDto { get; set; }
        public string UrlReturn { get; set; }
        public NotificationConfigDto NotificationsConfig { get; set; }
        public string DescriptionService { get; set; }

        public string ReferenceNumber1 { get; set; }
        public string ReferenceNumber2 { get; set; }
        public string ReferenceNumber3 { get; set; }
        public string ReferenceNumber4 { get; set; }
        public string ReferenceNumber5 { get; set; }
        public string ReferenceNumber6 { get; set; }
    }
}
