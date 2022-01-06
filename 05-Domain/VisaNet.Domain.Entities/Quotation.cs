using System;
using System.ComponentModel.DataAnnotations;
using VisaNet.Domain.Entities.Base.Base;

namespace VisaNet.Domain.Entities
{
    [TrackChanges]
    public class Quotation : EntityBase
    {
        [Key]
        [TrackChangesAditionalInfo(Index = 0)]
        public override Guid Id { get; set; }

        public DateTime DateFrom { get; set; }

        public string Currency { get; set; }

        public double ValueInPesos { get; set; }
    }
}
