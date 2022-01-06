using System.ComponentModel.DataAnnotations;
using VisaNet.Common.Resource.Presentation;

namespace VisaNet.Presentation.VisaNetOn.Models.AccessTokenModels
{
    public class PaymentModel : PaymentTokenModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredFieldEmail", ErrorMessageResourceType = typeof(PresentationVisaNetOnStrings))]
        [MaxLength(50, ErrorMessageResourceName = "FieldEmailMax", ErrorMessageResourceType = typeof(PresentationVisaNetOnStrings))]
        public string Email { get; set; }

        [MaxLength(50, ErrorMessageResourceName = "FieldNombreMax", ErrorMessageResourceType = typeof(PresentationVisaNetOnStrings))]
        public string Nombre { get; set; }

        [MaxLength(50, ErrorMessageResourceName = "FieldApellidoMax", ErrorMessageResourceType = typeof(PresentationVisaNetOnStrings))]
        public string Apellido { get; set; }

        [MaxLength(50, ErrorMessageResourceName = "FieldDireccionMax", ErrorMessageResourceType = typeof(PresentationVisaNetOnStrings))]
        public string Direccion { get; set; }

        [MaxLength(50, ErrorMessageResourceName = "FieldTelefonoMax", ErrorMessageResourceType = typeof(PresentationVisaNetOnStrings))]
        public string Telefono { get; set; }

        [MaxLength(50, ErrorMessageResourceName = "FieldMovilMax", ErrorMessageResourceType = typeof(PresentationVisaNetOnStrings))]
        public string Movil { get; set; }

        [MaxLength(50, ErrorMessageResourceName = "FieldCIMax", ErrorMessageResourceType = typeof(PresentationVisaNetOnStrings))]
        public string CI { get; set; }

        [MaxLength(1, ErrorMessageResourceName = "FieldPermiteCambioEmailMax", ErrorMessageResourceType = typeof(PresentationVisaNetOnStrings))]
        public string PermiteCambioEmail { get; set; }

        [MaxLength(1, ErrorMessageResourceName = "FieldSendEmailMax", ErrorMessageResourceType = typeof(PresentationVisaNetOnStrings))]
        public string SendEmail { get; set; }

        [MaxLength(1, ErrorMessageResourceName = "FieldPermiteRecordarUsuarioMax", ErrorMessageResourceType = typeof(PresentationVisaNetOnStrings))]
        public string PermiteRecordarUsuario { get; set; }
    }
}