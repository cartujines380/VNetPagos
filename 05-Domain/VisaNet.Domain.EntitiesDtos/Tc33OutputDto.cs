using System.Collections.Generic;

namespace VisaNet.Domain.EntitiesDtos
{
    public class Tc33OutputDto
    {
        public string OutputFileName { get; set; }
        public IEnumerable<string> Errors { get; set; }
        public IEnumerable<string> NotFound { get; set; }
    }
}
