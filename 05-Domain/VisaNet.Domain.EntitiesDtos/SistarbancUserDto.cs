using System;

namespace VisaNet.Domain.EntitiesDtos
{
    public class SistarbancUserDto 
    {
        public Guid Id { get; set; }
        public Int64 UniqueIdentifier { get; set; }
        public bool Active { get; set; }
    }
}
