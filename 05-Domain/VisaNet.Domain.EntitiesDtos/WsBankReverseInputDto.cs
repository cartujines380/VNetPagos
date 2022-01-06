using System;
using System.Collections.Generic;
using VisaNet.Common.Logging.Entities;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Domain.EntitiesDtos
{
    public class WsBankReverseInputDto
    {
        public string Name { get; set; }

        public string ServiceId { get; set; }
        public string RequestId { get; set; }
        public string Token { get; set; }
        public string Amount { get; set; }
        public string IdTransaccion { get; set; }
        public string Currency { get; set; }
        public LogUserType UserType { get; set; }
        public PaymentPlatformDto PaymentPlatform { get; set; }
    }
}
