using System.Collections.Generic;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Domain.EntitiesDtos
{
    public class ProcessOperationDto
    {
        public IDictionary<string, string> FormData { get; set; }
        public RedirectEnums Action { get; set; }
    }
}