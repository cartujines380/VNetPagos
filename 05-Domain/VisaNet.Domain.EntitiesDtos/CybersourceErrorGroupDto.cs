using System;
using System.Collections.Generic;

namespace VisaNet.Domain.EntitiesDtos
{
    public class CybersourceErrorGroupDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public int Order { get; set; }

        public virtual ICollection<CybersourceErrorDto> CybersourceErrors { get; set; }
    }
}
