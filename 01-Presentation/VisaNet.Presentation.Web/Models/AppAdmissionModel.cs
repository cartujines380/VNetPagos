using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.CustomAttributes;
using VisaNet.Utilities.Helpers;

namespace VisaNet.Presentation.Web.Models
{
    public class AppAdmissionModel
    {
        public Guid? UserId { get; set; }
        public ApplicationUserDto User { get; set; }
        public Guid? CardId { get; set; }

        public Guid ServiceId { get; set; }
        public string ServiceUrlName { get; set; }
        public string ServiceName { get; set; }
        public string ServiceImage { get; set; }

        public bool RegisteredEmail { get; set; } //si el mail esta registrado en VNP

        public bool LoadRegistredData { get; set; }
        public bool LoadNewData { get; set; }

        public bool SuccessFulAssociation { get; set; }


        public bool AllowsNewEmail { get; set; }
        public string IdOperation { get; set; }
        public string UrlCallback { get; set; }

        //EXISTING USER
        //[EmailValidator(ErrorMessage = null, ErrorMessageResourceName = "InvalidEmail", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        //[Display(Name = "Security_Email", ResourceType = typeof(PresentationCoreMessages))]
        //public string Email_AppUser { get; set; }

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
        [EmailValidator(ErrorMessage = null, ErrorMessageResourceName = "InvalidEmail", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        [StringLength(50, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string Email { get; set; }

        [CustomAttributes.CustomDisplay("Profile_identitynumber")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string IdentityNumber { get; set; }

        [CustomAttributes.CustomDisplay("Profile_Phone")]
        [StringLength(50, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string PhoneNumber { get; set; }

        [CustomAttributes.CustomDisplay("Profile_Mobile")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        [StringLength(50, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string MobileNumber { get; set; }

        [CustomAttributes.CustomDisplay("Profile_Address")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        [StringLength(50, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string Address { get; set; }

        /*
        [CustomAttributes.CustomDisplay("Profile_CCKey")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        [StringLength(6, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string CallCenterPin { get; set; }
*/

        [CustomAttributes.CustomDisplay("Profile_Password")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        [DataType(DataType.Password)]
        [RegularExpression(@"^.*(?=.{8,})(?=.*[a-z])(?=.*[A-Z])(?=.*\d).*$", ErrorMessageResourceName = "Security_PasswordFormatInvalid", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string Password { get; set; }
        public string PasswordHashed { get; set; }
        [CustomAttributes.CustomDisplay("Profile_Confirmation")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        [Compare("Password", ErrorMessageResourceName = "Security_ComparePasswords", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string PasswordConfirmation { get; set; }


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

        public string PostAssociationDesc { get; set; }
        public string[] PostAssociationDescLines { get; set; }


        public string TermsAndConditionsService { get; set; }
        public string[] TermsAndConditionsServiceLines { get; set; }

        //ASK REFS
        [Required(AllowEmptyStrings = false, ErrorMessage = "El campo es requerido")]
        [RegexValidation("ReferenceRegex")]
        [StringLength(100, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string ReferenceValue { get; set; }
        public string ReferenceName { get; set; }
        public string ReferenceRegex { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "El campo es requerido")]
        [RegexValidation("ReferenceRegex2")]
        [StringLength(100, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string ReferenceValue2 { get; set; }
        public string ReferenceName2 { get; set; }
        public string ReferenceRegex2 { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "El campo es requerido")]
        [RegexValidation("ReferenceRegex3")]
        [StringLength(100, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string ReferenceValue3 { get; set; }
        public string ReferenceName3 { get; set; }
        public string ReferenceRegex3 { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "El campo es requerido")]
        [RegexValidation("ReferenceRegex4")]
        [StringLength(100, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string ReferenceValue4 { get; set; }
        public string ReferenceName4 { get; set; }
        public string ReferenceRegex4 { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "El campo es requerido")]
        [RegexValidation("ReferenceRegex5")]
        [StringLength(100, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string ReferenceValue5 { get; set; }
        public string ReferenceName5 { get; set; }
        public string ReferenceRegex5 { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "El campo es requerido")]
        [RegexValidation("ReferenceRegex6")]
        [StringLength(100, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string ReferenceValue6 { get; set; }
        public string ReferenceName6 { get; set; }
        public string ReferenceRegex6 { get; set; }
        
        public bool AskReferences { get; set; }

        public bool ShowCardsAfterCsFail { get; set; }
    }
}