using System;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Domain.EntitiesDtos
{
    public class ConciliationSuciveDto
    {
        public Guid Id { get; set; }

        public DateTime Date { get; set; }

        public string BillExternalId { get; set; }
        public string Currency { get; set; }
        public double Amount { get; set; }
        public DepartamentDtoType Departament { get; set; }

    }
}
