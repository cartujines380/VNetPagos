using System;
using VisaNet.Domain.Entities.Enums;

namespace VisaNet.Domain.EntitiesDtos
{
    
    public class LocationDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public DepartamentType Departament { get; set; }
        public bool Active { get; set; }
        public GatewayEnum GatewayEnum { get; set; }
    }
}
