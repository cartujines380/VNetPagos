using VisaNet.Domain.EntitiesDtos.CustomAttributes;

namespace VisaNet.Presentation.Administration.Models
{
    public class ServiceAssociatedModel
    {
        [CustomDisplay("ServiceAssociatedDto_ClientEmail")]
        public string ClientEmail { get; set; }
        [CustomDisplay("ServiceAssociatedDto_ClientName")]
        public string ClientName { get; set; }
        [CustomDisplay("ServiceAssociatedDto_ClientSurname")]
        public string ClientSurname { get; set; }
        [CustomDisplay("ServiceAssociatedDto_ClientIdentityNumber")]
        public string ClientIdentityNumber { get; set; }
        [CustomDisplay("ServiceAssociatedDto_ClientMobileNumber")]
        public string ClientMobileNumber { get; set; }
        [CustomDisplay("ServiceAssociatedDto_ClientPhoneNumber")]
        public string ClientPhoneNumber { get; set; }
        [CustomDisplay("ServiceAssociatedDto_ClientAddress")]
        public string ClientAddress { get; set; }

        [CustomDisplay("ServiceAssociatedDto_ServiceName")]
        public string ServiceName { get; set; }
        [CustomDisplay("ServiceAssociatedDto_ServiceCategoryName")]
        public string ServiceCategoryName { get; set; }
        
        [CustomDisplay("ServiceAssociatedDto_ReferenceNumber")]
        public string ReferenceNumber { get; set; }
        [CustomDisplay("ServiceAssociatedDto_Description")]
        public string Description { get; set; }

        [CustomDisplay("ServiceAssociatedDto_CardMaskedNumber")]
        public string CardMaskedNumber { get; set; }
        [CustomDisplay("ServiceAssociatedDto_CardDueDate")]
        public string CardDueDate { get; set; }
        [CustomDisplay("ServiceAssociatedDto_CardBin")]
        public string CardBin { get; set; }
        [CustomDisplay("ServiceAssociatedDto_CardType")]
        public string CardType { get; set; }
        
        [CustomDisplay("ServiceAssociatedDto_AutomaticPayment")]
        public string AutomaticPayment { get; set; }
        [CustomDisplay("ServiceAssociatedDto_Enabled")]
        public string Enabled { get; set; }
    }
}