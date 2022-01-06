using System;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Domain.EntitiesDtos
{
    public class ConciliationRunDto
    {
        public Guid Id { get; set; }

        public ConciliationAppDto App { get; set; }
        public bool IsManualRun { get; set; } //indica si es una corrida que se hizo manualmente desde el Admin

        public string InputFileName { get; set; }
        public DateTime? ConciliationDateFrom { get; set; }
        public DateTime? ConciliationDateTo { get; set; }

        public ConciliationRunStateDto State { get; set; }
        public string ResultDescription { get; set; }
        public string ExceptionMessage { get; set; }

        public string CreationUser { get; set; }
        public string LastModificationUser { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastModificationDate { get; set; }
    }
}