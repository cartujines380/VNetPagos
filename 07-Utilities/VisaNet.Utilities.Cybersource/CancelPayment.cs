using System;
using VisaNet.Common.Logging.Entities;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Utilities.Cybersource
{
    public class CancelPayment
    {
        public Guid UserId { get; set; }
        public Guid ServiceId { get; set; }
        public ServiceDto ServiceDto { get; set; }
        public string RequestId { get; set; }
        public string Token { get; set; }
        public string Amount { get; set; }
        public string IdTransaccion { get; set; }
        public string Currency { get; set; }
        public LogUserType UserType { get; set; }
        public PaymentPlatformDto PaymentPlatform { get; set; }
        public string IdOperation { get; set; }
        public string UserEmail { get; set; }
    }
}
