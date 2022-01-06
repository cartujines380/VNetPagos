using System;

namespace VisaNet.Domain.EntitiesDtos
{
    public class CybersourceErrorDto
    {
        public Guid Id { get; set; }

        public int ReasonCode { get; set; }

        public Guid CybersourceErrorGroupId { get; set; }
        public virtual CybersourceErrorGroupDto CybersourceErrorGroup { get; set; }
    }
}
