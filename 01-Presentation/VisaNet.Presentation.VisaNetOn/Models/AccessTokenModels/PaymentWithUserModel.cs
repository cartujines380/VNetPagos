using System.ComponentModel.DataAnnotations;
using VisaNet.Common.Resource.Presentation;

namespace VisaNet.Presentation.VisaNetOn.Models.AccessTokenModels
{
    public class PaymentWithUserModel : PaymentTokenModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredFieldIdUsuario", ErrorMessageResourceType = typeof(PresentationVisaNetOnStrings))]
        [MaxLength(36, ErrorMessageResourceName = "FieldIdUsuarioMax", ErrorMessageResourceType = typeof(PresentationVisaNetOnStrings))]
        public string IdUsuario { get; set; }

        [MaxLength(1, ErrorMessageResourceName = "FieldSendEmailMax", ErrorMessageResourceType = typeof(PresentationVisaNetOnStrings))]
        public string SendEmail { get; set; }
    }
}