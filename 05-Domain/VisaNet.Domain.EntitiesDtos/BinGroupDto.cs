using System;
using System.Collections.Generic;

namespace VisaNet.Domain.EntitiesDtos
{

    public class BinGroupDto
    {
        public Guid Id { get; set; }

        public virtual ICollection<BinDto> Bins { get; set; }

        public virtual ICollection<ServiceDto> Services { get; set; }

        public string Name { get; set; }

        public List<BinDto> AddedBins { get; set; }
        public List<BinDto> DeletedBins { get; set; }           
    }
}
