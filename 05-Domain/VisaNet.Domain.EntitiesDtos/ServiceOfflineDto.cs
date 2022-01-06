using System;

namespace VisaNet.Domain.EntitiesDtos
{
    public class ServiceOfflineDto
    {
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public string[] IdApps { get; set; }

    }
}
