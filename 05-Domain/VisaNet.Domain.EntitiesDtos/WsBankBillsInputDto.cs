using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Domain.EntitiesDtos
{
    public class WsBankBillsInputDto
    {
        
        public string ServiceId { get; set; }

        public GatewayEnumDto GatewayEnumDto { get; set; }
        
        public string ServiceReferenceNumber { get; set; }
        public string ServiceReferenceNumber2 { get; set; }
        public string ServiceReferenceNumber3 { get; set; }
        public string ServiceReferenceNumber4 { get; set; }
        public string ServiceReferenceNumber5 { get; set; }
        public string ServiceReferenceNumber6 { get; set; }

        //public int CardBinNumbers { get; set; }

        public string UserEmail { get; set; }
        public string UserCi { get; set; }
        public string UserAddress { get; set; }
        public string UserName { get; set; }
        public string UserSurname { get; set; }
        
    }
}
