using System;
using System.ComponentModel.DataAnnotations;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Presentation.Administration.CustomAttributes;

namespace VisaNet.Presentation.Administration.Models
{
    public class ServiceEnableEmailModel
    {
        public Guid Id { get; set; }
        
        [CustomDisplay("Email")]
        [StringLength(50, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        //[RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessageResourceName = "Email_InvalidFormat", ErrorMessageResourceType = typeof(PresentationAdminStrings))]
        public string Email { get; set; }

    }
}