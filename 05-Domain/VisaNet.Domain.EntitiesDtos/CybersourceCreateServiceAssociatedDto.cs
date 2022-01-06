using System;

namespace VisaNet.Domain.EntitiesDtos
{
    public class CybersourceCreateServiceAssociatedDto
    {
        public CybersourceCreateCardDto CybersourceCreateCardDto{ get; set; }
        public ServiceAssociatedDto ServiceAssociatedDto{ get; set; }
        public Exception ExceptionCapture { get; set; }
        public int AssociationInternalErrorCode { get; set; }
        public string AssociationInternalErrorDesc { get; set; }

        public CyberSourceDataDto CyberSourceData { get; set; }
        public VerifyByVisaDataDto VerifyByVisaData { get; set; }

        public CyberSourceMerchantDefinedDataDto CyberSourceMerchantDefinedData { get; set; }
    }
}
