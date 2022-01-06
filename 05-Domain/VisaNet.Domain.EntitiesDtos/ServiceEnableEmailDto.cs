using System;
using System.ComponentModel.DataAnnotations;

namespace VisaNet.Domain.EntitiesDtos
{
    public class ServiceEnableEmailDto 
    {

        public Guid Id{ get; set; }
        [MaxLength(50)]
        public string Email{ get; set; }

        public string RouteId { get; set; }

        public Guid ServiceId { get; set; }
        public ServiceDto Service { get; set; }

    }
}
