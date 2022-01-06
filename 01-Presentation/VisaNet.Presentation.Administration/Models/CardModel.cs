using VisaNet.Domain.EntitiesDtos.CustomAttributes;

namespace VisaNet.Presentation.Administration.Models
{
    public class CardModel
    {
        [CustomDisplay("CardDto_ClientEmail")]
        public string ClientEmail { get; set; }
        [CustomDisplay("CardDto_ClientName")]
        public string ClientName { get; set; }
        [CustomDisplay("CardDto_ClientSurname")]
        public string ClientSurname { get; set; }
        [CustomDisplay("CardDto_ClientIdentityNumber")]
        public string ClientIdentityNumber { get; set; }
        [CustomDisplay("CardDto_ClientMobileNumber")]
        public string ClientMobileNumber { get; set; }
        [CustomDisplay("CardDto_ClientPhoneNumber")]
        public string ClientPhoneNumber { get; set; }
        [CustomDisplay("CardDto_ClientAddress")]
        public string ClientAddress { get; set; }

        [CustomDisplay("CardDto_MaskedNumber")]
        public string CardMaskedNumber { get; set; }
        [CustomDisplay("CardDto_DueDate")]
        public string CardDueDate { get; set; }
        [CustomDisplay("CardDto_Bin")]
        public string CardBin { get; set; }
        [CustomDisplay("CardDto_Type")]
        public string CardType { get; set; }
        [CustomDisplay("CardDto_Active")]
        public string CardActive { get; set; }
        [CustomDisplay("CardDto_Deleted")]
        public string CardDeleted { get; set; }
    }
}