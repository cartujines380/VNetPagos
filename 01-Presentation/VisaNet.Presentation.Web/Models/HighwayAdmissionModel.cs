using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Domain.EntitiesDtos.CustomAttributes;
using VisaNet.Utilities.Helpers;

namespace VisaNet.Presentation.Web.Models
{
    public class HighwayAdmissionModel
    {
        public Guid? UserId { get; set; }
        public Guid? CardId { get; set; }

        public Guid ServiceId { get; set; }
        public string ServiceUrlName { get; set; }
        public string ServiceName { get; set; }
        public string ServiceImage { get; set; }

        public bool SuccessFulAssociation { get; set; }

        //EXISTING USER
        [EmailValidator(ErrorMessageResourceName = "InvalidEmail", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        [Display(Name = "Security_Email", ResourceType = typeof(PresentationCoreMessages))]
        public string Email_AppUser { get; set; }

        [Display(Name = "Security_Password", ResourceType = typeof(PresentationCoreMessages))]
        public string Password_AppUser { get; set; }


        //NEW USER     
        [CustomAttributes.CustomDisplay("Profile_Nombre")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        [StringLength(50, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string Name { get; set; }

        [CustomAttributes.CustomDisplay("Profile_Surname")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        [StringLength(50, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string Surname { get; set; }

        [CustomAttributes.CustomDisplay("Profile_Email")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        [StringLength(50, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        [EmailValidator(ErrorMessageResourceName = "InvalidEmail", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string Email { get; set; }

        [CustomAttributes.CustomDisplay("Profile_identitynumber")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string IdentityNumber { get; set; }

        [CustomAttributes.CustomDisplay("Profile_Phone")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        [StringLength(50, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string PhoneNumber { get; set; }

        [CustomAttributes.CustomDisplay("Profile_Mobile")]
        [StringLength(50, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string MobileNumber { get; set; }

        [CustomAttributes.CustomDisplay("Profile_Address")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        [StringLength(50, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string Address { get; set; }

        [CustomAttributes.CustomDisplay("Profile_CCKey")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        [StringLength(6, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string CallCenterPin { get; set; }

        [CustomAttributes.CustomDisplay("Profile_Password")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        [DataType(DataType.Password)]
        [RegularExpression(@"^.*(?=.{8,})(?=.*[a-z])(?=.*[A-Z])(?=.*\d).*$", ErrorMessageResourceName = "Security_PasswordFormatInvalid", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string Password { get; set; }

        [CustomAttributes.CustomDisplay("Profile_Confirmation")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        [Compare("Password", ErrorMessageResourceName = "Security_ComparePasswords", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string PasswordConfirmation { get; set; }


        //REFERENCES
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        [MaxLength(100)]
        public string ReferenceNumber { get; set; }
        [MaxLength(100)]
        public string ReferenceNumber2 { get; set; }
        [MaxLength(100)]
        public string ReferenceNumber3 { get; set; }
        [MaxLength(100)]
        public string ReferenceNumber4 { get; set; }
        [MaxLength(100)]
        public string ReferenceNumber5 { get; set; }
        [MaxLength(100)]
        public string ReferenceNumber6 { get; set; }

        public string ReferenceName { get; set; }
        public string ReferenceName2 { get; set; }
        public string ReferenceName3 { get; set; }
        public string ReferenceName4 { get; set; }
        public string ReferenceName5 { get; set; }
        public string ReferenceName6 { get; set; }

        //CARD
        [CustomDisplay("CardDto_Name")]
        [StringLength(50, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string CardHolderName { get; set; }

        [CustomDisplay("CardDto_MaskedNumber")]
        [StringLength(25, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string CardNumber { get; set; }

        [CustomDisplay("CardDto_SecurityCode")]
        [StringLength(6, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string CardCode { get; set; }

        [CustomDisplay("CardDto_DueDate")]
        public string CardDueDate { get; set; }

        public string CardBin { get; set; }


        //ASSOCIATED CARDS
        public IEnumerable<HighwayCardModel> AssociatedCards { get; set; }


        public bool LoadRegistredData { get; set; }
        public bool LoadNewData { get; set; }

        public string PostAssociationDesc { get; set; }

        public bool AcceptTermsAndConditionsVisa { get; set; }
        public bool AcceptTermsAndConditionsService { get; set; }

        public string TermsAndConditionsService { get; set; }
        public string[] TermsAndConditionsServiceLines { get; set; }
    }
}