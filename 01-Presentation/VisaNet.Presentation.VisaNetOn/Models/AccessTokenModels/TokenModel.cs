using System.ComponentModel.DataAnnotations;
using VisaNet.Common.Resource.Presentation;

namespace VisaNet.Presentation.VisaNetOn.Models.AccessTokenModels
{
    public abstract class TokenModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredFieldIdApp", ErrorMessageResourceType = typeof(PresentationVisaNetOnStrings))]
        [MaxLength(50, ErrorMessageResourceName = "FieldIdAppMax", ErrorMessageResourceType = typeof(PresentationVisaNetOnStrings))]
        public string IdApp { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredFieldIdOperacion", ErrorMessageResourceType = typeof(PresentationVisaNetOnStrings))]
        [MaxLength(50, ErrorMessageResourceName = "FieldIdOperacionMax", ErrorMessageResourceType = typeof(PresentationVisaNetOnStrings))]
        public string IdOperacion { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredFieldUrlCallback", ErrorMessageResourceType = typeof(PresentationVisaNetOnStrings))]
        [MaxLength(255, ErrorMessageResourceName = "FieldUrlCallbackMax", ErrorMessageResourceType = typeof(PresentationVisaNetOnStrings))]
        public string UrlCallback { get; set; }

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

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredFieldFirmaDigital", ErrorMessageResourceType = typeof(PresentationVisaNetOnStrings))]
        public string FirmaDigital { get; set; }

    }
}