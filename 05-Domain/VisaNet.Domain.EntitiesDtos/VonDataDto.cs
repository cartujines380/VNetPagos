using System;

namespace VisaNet.Domain.EntitiesDtos
{
    public class VonDataDto
    {
        public Guid Id { get; set; }

        //Service
        public string AppId { get; set; }

        //User
        public Guid AnonymousUserId { get; set; }
        public AnonymousUserDto AnonymousUserDto { get; set; }
        public string UserExternalId { get; set; }
        public string ReferenceNumber { get; set; }
        public string ReferenceNumber2 { get; set; }
        public string ReferenceNumber3 { get; set; }
        public string ReferenceNumber4 { get; set; }
        public string ReferenceNumber5 { get; set; }
        public string ReferenceNumber6 { get; set; }

        //Card
        public string CardExternalId { get; set; }
        public string CardName { get; set; }
        public string CardMaskedNumber { get; set; }
        public string CardToken { get; set; }
        public DateTime CardDueDate { get; set; }

        //Additional
        public DateTime CreationDate { get; set; }
    }
}