using System;

namespace VisaNet.Domain.EntitiesDtos
{
    public class AffiliationCardDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
        public int Code { get; set; }
        public bool Active { get; set; }

        public Guid? BankId { get; set; }
        public virtual BankDto BankDto { get; set; }

    }
}
