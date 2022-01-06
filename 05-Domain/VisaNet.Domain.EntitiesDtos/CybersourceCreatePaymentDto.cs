using System;

namespace VisaNet.Domain.EntitiesDtos
{
    public class CybersourceCreatePaymentDto
    {
        public CyberSourceOperationData CyberSourceOperationData { get; set; }
        public PaymentDto NewPaymentDto { get; set; }
        public Exception ExceptionCapture { get; set; }

        public CybersourceCreatePaymentDto()
        {
            CyberSourceOperationData = new CyberSourceOperationData();
        }
    }
}

