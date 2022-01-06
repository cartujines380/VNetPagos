using System;
using VisaNet.Common.Logging.Entities;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Domain.EntitiesDtos.ExternalRequest;

namespace VisaNet.Utilities.Cybersource
{
    public class GeneratePayment
    {
        public Guid ApplicationUserId { get; set; }
        public string MerchandId { get; set; }
        public string Key { get; set; }
        public string TransaccionId { get; set; }
        public string Currency { get; set; }
        public string GrandTotalAmount { get; set; }
        public string Token { get; set; }

        public LogUserType UserType { get; set; }
        public PaymentPlatformDto PaymentPlatform { get; set; }

        public string DeviceFingerprint { get; set; }
        public CustomerShippingAddresDto CustomerShippingAddresDto { get; set; }
        public string CustomerIp { get; set; }

        public AdditionalInfo AdditionalInfo { get; set; }
        public int Quota { get; set; }
    }

    public class AdditionalInfo
    {
        public CardTypeDto CardTypeDto { get; set; }
        public DiscountLabelTypeDto DiscountLabelTypeDto { get; set; }
        public int BinValue { get; set; }
    }
}