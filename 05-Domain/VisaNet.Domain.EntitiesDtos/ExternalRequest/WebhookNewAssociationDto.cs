using System;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Domain.EntitiesDtos.ExternalRequest
{
    public class WebhookNewAssociationDto
    {
        public Guid Id { get; set; }
        public UserDataInputDto UserData { get; set; }
        public string IdApp { get; set; }
        public string IdUser { get; set; }
        public string IdCard { get; set; }
        public string CardDueDate { get; set; }
        public string CardMask { get; set; }
        public string RefCliente1 { get; set; }
        public string RefCliente2 { get; set; }
        public string RefCliente3 { get; set; }
        public string RefCliente4 { get; set; }
        public string RefCliente5 { get; set; }
        public string RefCliente6 { get; set; }
        public string IdOperationApp { get; set; }
        public string IdOperation { get; set; }
        public bool IsAssociation { get; set; }
        public bool IsPayment { get; set; }
        public string TransactionNumber { get; set; }
        public double DiscountAmount { get; set; }
        public CardTypeDto? CardType { get; set; }
        public string CardBank { get; set; }
        public string CardBankCode { get; set; }
        public string CardAffiliation { get; set; }
        public string CardAffiliationCode { get; set; }

        public string HttpResponseCode { get; set; }

        public DateTime CreationDate { get; set; }
    }
}