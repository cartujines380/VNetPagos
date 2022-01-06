using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VisaNet.Domain.EntitiesDtos;

namespace VisaNet.Presentation.Web.Areas.Private.Models
{
    public class SevicesAssosiatedViewModel
    {
        public List<ServiceAssociatedDto> ServicesAssociated { get; set; }
        public List<DebitAssociatedDto> DebitsAssociated { get; set; }
    }
}