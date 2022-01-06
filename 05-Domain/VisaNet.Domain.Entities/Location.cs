using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VisaNet.Domain.Entities.Base.Base;
using VisaNet.Domain.Entities.Enums;

namespace VisaNet.Domain.Entities
{
    [Table("Locations")]
    [TrackChanges]
    public class Location : EntityBase
    {
        [Key]
        public override Guid Id { get; set; }
        [TrackChangesAditionalInfo(Index = 0)]
        public string Name { get; set; }
        public string Value { get; set; }
        public DepartamentType Departament { get; set; }
        public bool Active { get; set; }
        public GatewayEnum GatewayEnum { get; set; }

    }
}
