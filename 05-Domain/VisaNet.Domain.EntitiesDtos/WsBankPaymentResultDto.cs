using System.Collections.Generic;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Domain.EntitiesDtos
{
    public class WsBankPaymentResultDto
    {
        public ErrorCodeDto ErrorCode { get; set; }
        public string ErrorMessage { get; set; }

        public CyberSourceOperationData CyberSourceOperationData { get; set; }

        public PaymentDto Payment { get; set; }

        public WsBankPaymentResultDto(ErrorCodeDto errorCode)
        {
            ErrorCode = errorCode;
        }
    }
}
