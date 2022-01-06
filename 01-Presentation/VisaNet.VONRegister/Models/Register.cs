using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Domain.EntitiesDtos;

namespace VisaNet.VONRegister.Models
{
    public class Register : BaseViewModel<ApplicationUserCreateEditDto>
    {
        [Required(AllowEmptyStrings = false)]
        [MaxLength(50, ErrorMessage = "El largo máximo permitido son 50 letras (contando espacios vacios)")]
        public string Name { get; set; }
        [Required(AllowEmptyStrings = false)]
        [MaxLength(50, ErrorMessage = "El largo máximo permitido son 50 letras (contando espacios vacios)")]
        public string Surname { get; set; }
        [EmailAddress]
        [Required(AllowEmptyStrings = false)]
        [MaxLength(50, ErrorMessage = "El largo máximo permitido son 50 letras (contando espacios vacios)")]
        public string Email { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "La contraseña es requerida")]
        [RegularExpression(@"^.*(?=.{8,})(?=.*[a-z])(?=.*[A-Z])(?=.*\d).*$", ErrorMessageResourceName = "Security_PasswordFormatInvalid", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string Password { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "La contraseña es requerida")]
        public string PasswordReEntered { get; set; }

        public Card Card { get; set; }

        public bool EditableEmail { get; set; }
        public bool AgreeToTermsVisaNetPagos { get; set; }
        public bool AgreeToTermsService { get; set; }
        public Payment Payment { get; set; }
        //public ICollection<CardDto> CardList { get; set; }
        public IList<RegisterdCard> CardList { get; set; }

        //Used only to know if the user is creating a newe card
        public bool NewCard { get; set; }

        public bool NewUser { get; set; }

        public Guid SelectedCard { get; set; }
        public int CardBin { get; set; }
        public bool AskUserForReferences { get; set; }
        [Required(AllowEmptyStrings = false)]
        [MaxLength(50, ErrorMessage = "El largo máximo permitido son 50 letras (contando espacios vacios)")]
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Mobile { get; set; }
        public string Identity { get; set; }
        public string TermsAndConditionsVisa { get; set; }
        public string TermsAndConditionsService { get; set; }
        public string TermsAndConditionsPostConfirm { get; set; }

        public string ServiceName { get; set; }
        public string FpProfiler { get; set; }

        public string MerchantId { get; set; }

        public override ApplicationUserCreateEditDto ToDto()
        {
            return new ApplicationUserCreateEditDto
            {
                //Address = this.Address,
                Email = this.Email,
                Name = this.Name,
                Surname = this.Surname,
                Password = this.Password,
                Address = this.Address,
                PhoneNumber = this.Phone,
                MobileNumber = this.Mobile,
                IdentityNumber = this.Identity
            };
        }
    }
}