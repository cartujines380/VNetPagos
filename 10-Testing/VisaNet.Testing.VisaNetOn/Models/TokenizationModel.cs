using System.ComponentModel.DataAnnotations;
using VisaNet.Common.Resource.Presentation;

namespace VisaNet.Testing.VisaNetOn.Models
{
    public class TokenizationModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredFieldIdApp", ErrorMessageResourceType = typeof(PresentationVisaNetOnStrings))]
        [MaxLength(50, ErrorMessageResourceName = "FieldIdAppMax", ErrorMessageResourceType = typeof(PresentationVisaNetOnStrings))]
        public string IdApp { get; set; }

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

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredFieldIdOperacion", ErrorMessageResourceType = typeof(PresentationVisaNetOnStrings))]
        [MaxLength(50, ErrorMessageResourceName = "FieldIdOperacionMax", ErrorMessageResourceType = typeof(PresentationVisaNetOnStrings))]
        public string IdOperacion { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredFieldUrlCallback", ErrorMessageResourceType = typeof(PresentationVisaNetOnStrings))]
        [MaxLength(255, ErrorMessageResourceName = "FieldUrlCallbackMax", ErrorMessageResourceType = typeof(PresentationVisaNetOnStrings))]
        public string UrlCallback { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredFieldFirmaDigital", ErrorMessageResourceType = typeof(PresentationVisaNetOnStrings))]
        public string FirmaDigital { get; set; }

        [MaxLength(100, ErrorMessageResourceName = "FieldRefCliente1Max", ErrorMessageResourceType = typeof(PresentationVisaNetOnStrings))]
        public string RefCliente1 { get; set; }

        [MaxLength(100, ErrorMessageResourceName = "FieldRefCliente2Max", ErrorMessageResourceType = typeof(PresentationVisaNetOnStrings))]
        public string RefCliente2 { get; set; }

        [MaxLength(100, ErrorMessageResourceName = "FieldRefCliente3Max", ErrorMessageResourceType = typeof(PresentationVisaNetOnStrings))]
        public string RefCliente3 { get; set; }

        [MaxLength(100, ErrorMessageResourceName = "FieldRefCliente4Max", ErrorMessageResourceType = typeof(PresentationVisaNetOnStrings))]
        public string RefCliente4 { get; set; }

        [MaxLength(100, ErrorMessageResourceName = "FieldRefCliente5Max", ErrorMessageResourceType = typeof(PresentationVisaNetOnStrings))]
        public string RefCliente5 { get; set; }

        [MaxLength(100, ErrorMessageResourceName = "FieldRefCliente6Max", ErrorMessageResourceType = typeof(PresentationVisaNetOnStrings))]
        public string RefCliente6 { get; set; }
    }
}