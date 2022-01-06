using System.ComponentModel.DataAnnotations;
using VisaNet.Common.Resource.Presentation;

namespace VisaNet.Presentation.VisaNetOn.Models.AccessTokenModels
{
    public abstract class PaymentTokenModel : TokenModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredFieldMerchantId", ErrorMessageResourceType = typeof(PresentationVisaNetOnStrings))]
        [MaxLength(25, ErrorMessageResourceName = "FieldMerchantIdMax", ErrorMessageResourceType = typeof(PresentationVisaNetOnStrings))]
        public string IdMerchant { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredFieldFacturaIdentificador", ErrorMessageResourceType = typeof(PresentationVisaNetOnStrings))]
        [MaxLength(100, ErrorMessageResourceName = "FieldFacturaIdentificadorMax", ErrorMessageResourceType = typeof(PresentationVisaNetOnStrings))]
        public string FacturaIdentificador { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredFieldFacturaImporte", ErrorMessageResourceType = typeof(PresentationVisaNetOnStrings))]
        [MaxLength(12, ErrorMessageResourceName = "FieldFacturaImporteMax", ErrorMessageResourceType = typeof(PresentationVisaNetOnStrings))]
        public string FacturaImporte { get; set; }

        [MaxLength(12, ErrorMessageResourceName = "FieldFacturaImporteGravadoMax", ErrorMessageResourceType = typeof(PresentationVisaNetOnStrings))]
        public string FacturaImporteGravado { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredFieldFacturaMoneda", ErrorMessageResourceType = typeof(PresentationVisaNetOnStrings))]
        [MaxLength(1, ErrorMessageResourceName = "FieldFacturaMonedaMax", ErrorMessageResourceType = typeof(PresentationVisaNetOnStrings))]
        public string FacturaMoneda { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredFieldFacturaConsFinal", ErrorMessageResourceType = typeof(PresentationVisaNetOnStrings))]
        [MaxLength(1, ErrorMessageResourceName = "FieldFacturaConsFinalMax", ErrorMessageResourceType = typeof(PresentationVisaNetOnStrings))]
        public string FacturaConsFinal { get; set; }

        [MaxLength(8, ErrorMessageResourceName = "FieldFacturaFechaMax", ErrorMessageResourceType = typeof(PresentationVisaNetOnStrings))]
        public string FacturaFecha { get; set; }

        [MaxLength(200, ErrorMessageResourceName = "FieldFacturaDescripcionMax", ErrorMessageResourceType = typeof(PresentationVisaNetOnStrings))]
        public string FacturaDescripcion { get; set; }

        [MaxLength(1, ErrorMessageResourceName = "FieldFacturaCuotaMax", ErrorMessageResourceType = typeof(PresentationVisaNetOnStrings))]
        public string FacturaCuota { get; set; }

        public Linea[] FacturaLineas { get; set; }

    }
}