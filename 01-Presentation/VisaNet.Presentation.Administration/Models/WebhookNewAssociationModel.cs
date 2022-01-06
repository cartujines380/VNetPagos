using System;

namespace VisaNet.Presentation.Administration.Models
{
    public class WebhookNewAssociationModel
    {
        public Guid Id { get; set; }

        public DateTime CreationDate { get; set; }
        public string IdApp { get; set; }
        public string IdOperationApp { get; set; }
        public string IdOperation { get; set; }
        public string HttpResponseCode { get; set; }

        public string IdUser { get; set; }
        public string UserEmail { get; set; }
        public string UserName { get; set; }
        public string UserSurname { get; set; }
        public string UserIdentityNumber { get; set; }
        public string UserMobileNumber { get; set; }
        public string UserPhoneNumber { get; set; }
        public string UserAddress { get; set; }

        public string IdCard { get; set; }
        public string CardDueDate { get; set; }
        public string CardMask { get; set; }
        public string CardType { get; set; }
        public string CardBank { get; set; }
        public string CardBankCode { get; set; }
        public string CardAffiliation { get; set; }
        public string CardAffiliationCode { get; set; }

        public string RefCliente1 { get; set; }
        public string RefCliente2 { get; set; }
        public string RefCliente3 { get; set; }
        public string RefCliente4 { get; set; }
        public string RefCliente5 { get; set; }
        public string RefCliente6 { get; set; }

        public string IsAssociation { get; set; }
        public string IsPayment { get; set; }

        public string TransactionNumber { get; set; }
        public string DiscountAmount { get; set; }

    }
}