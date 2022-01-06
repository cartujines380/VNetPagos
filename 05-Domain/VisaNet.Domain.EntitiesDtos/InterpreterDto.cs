using System;
using System.Collections.Generic;

namespace VisaNet.Domain.EntitiesDtos
{
    public class InterpreterDto 
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<ServiceDto> Services { get; set; }
    }
}
