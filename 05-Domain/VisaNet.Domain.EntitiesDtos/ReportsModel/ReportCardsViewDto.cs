using System;

namespace VisaNet.Domain.EntitiesDtos.ReportsModel
{
    public class ReportCardsViewDto
    {
        public Guid ApplicationUserId { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string IdentityNumber { get; set; }
        public string MobileNumber { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }

        public Guid CardId { get; set; }
        public string CardMaskedNumber { get; set; }
        public DateTime CardDueDate { get; set; }
        public DateTime? LastPaymentDate { get; set; }
        public int NumServicesAsociated { get; set; }
        public bool CardActive { get; set; }
        public bool CardDeleted { get; set; }
        public bool DeletedFromCs { get; set; }
        public Guid? BinId { get; set; }
        public int? BinValue { get; set; }
        public int? CardType { get; set; }

    }
}