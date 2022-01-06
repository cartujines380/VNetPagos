using System;
using System.ComponentModel.DataAnnotations;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Presentation.Administration.CustomAttributes;
using VisaNet.Utilities.Helpers;

namespace VisaNet.Presentation.Administration.Models
{
    public class CustomerSiteSystemUserModel
    {
        public Guid Id { get; set; }
        
        [CustomDisplay("CustomerSite_ContactEmail")]
        [StringLength(50, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        [EmailValidator(ErrorMessageResourceName = "InvalidEmail", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string Email { get; set; }

        [CustomDisplay("CustomerSite_Name")]
        [MaxLength(50)]
        public string Name { get; set; }
        [CustomDisplay("CustomerSite_Surname")]
        [MaxLength(50)]
        public new string Surname { get; set; }
        [CustomDisplay("CustomerSite_PhoneNumber")]
        [MaxLength(50)]
        public new string PhoneNumber { get; set; }

        [CustomDisplay("CustomerSite_Disabled")]
        public bool Disabled { get; set; }

        [CustomDisplay("CustomerSite_Commerce")]
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public Guid CommerceId { get; set; }

        [CustomDisplay("CustomerSite_Branch")]
        public Guid BranchId { get; set; }

        [CustomDisplay("CustomerSite_Master")]
        public CustomerSystemUserUserType UserType { get; set; }

        [CustomDisplay("CustomerSite_LastAttemptToLogIn")]
        public DateTime? LastAttemptToLogIn { get; set; }
        [CustomDisplay("CustomerSite_FailLogInCount")]
        public int FailLogInCount { get; set; }
        [CustomDisplay("CustomerSite_LastResetPassword")]
        public DateTime? LastResetPassword { get; set; }
        [CustomDisplay("SendEmailActivation")]
        public bool SendEmailActivation { get; set; }
        
    }

    public enum CustomerSystemUserUserType
    {
        Master = 0,
        Alternative =1
    }

}