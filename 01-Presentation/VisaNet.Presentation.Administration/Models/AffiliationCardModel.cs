using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Presentation.Administration.CustomAttributes;

namespace VisaNet.Presentation.Administration.Models
{
    public class AffiliationCardModel
    {

        public Guid Id { get; set; }

        [CustomDisplay("Name")]
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        [StringLength(50, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string Name { get; set; }

        [CustomDisplay("Code")]
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        [Range(0, 999999)]
        public int Code { get; set; }

        [CustomDisplay("Active")]
        public bool Active { get; set; }

        [CustomDisplay("Bank")]
        public Guid? BankId { get; set; }
        public string BankName { get; set; }
        
    }
}