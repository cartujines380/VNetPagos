using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VisaNet.Domain.Entities.Base.Audit;
using VisaNet.Domain.Entities.Base.Base;
using VisaNet.Domain.Entities.Enums;

namespace VisaNet.Domain.Entities
{
    [Table("ConciliationRuns")]
    public class ConciliationRun : EntityBase, IAuditable
    {
        [Key]
        public override Guid Id { get; set; }

        public ConciliationApp App { get; set; }
        public bool IsManualRun { get; set; } //indica si es una corrida que se hizo manualmente desde el Admin

        [MaxLength(100)]
        public string InputFileName { get; set; }
        public DateTime? ConciliationDateFrom { get; set; }
        public DateTime? ConciliationDateTo { get; set; }

        public ConciliationRunState State { get; set; }
        public string ResultDescription { get; set; }
        public string ExceptionMessage { get; set; }

        [MaxLength(50)]
        public string CreationUser { get; set; }
        [MaxLength(50)]
        public string LastModificationUser { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastModificationDate { get; set; }
    }
}