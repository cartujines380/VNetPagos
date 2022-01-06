using System;
using System.Collections.Generic;

namespace VisaNet.Domain.EntitiesDtos
{
    public class RoleDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public IList<int> ActionsIds { get; set; }
    }
}
