using System;
using System.Collections.Generic;
using System.Linq;

namespace VisaNet.Presentation.Web.Models
{
    public class ServiceProviderViewModel
    {
        public Guid? ServiceId { get; set; }
        public List<ServiceModel> Services { get; set; }
    }
}