using System.ComponentModel.DataAnnotations;
using VisaNet.Common.Resource.Presentation;

namespace VisaNet.Testing.VisaNetOn.Models
{
    public class Linea
    {
        [MaxLength(2, ErrorMessageResourceName = "FieldOrdenMax", ErrorMessageResourceType = typeof(PresentationVisaNetOnStrings))]
        public string Orden { get;set; }
        [MaxLength(100, ErrorMessageResourceName = "FieldConceptoMax", ErrorMessageResourceType = typeof(PresentationVisaNetOnStrings))]
        public string Concepto { get;set; }
        [MaxLength(12, ErrorMessageResourceName = "FieldImporteMax", ErrorMessageResourceType = typeof(PresentationVisaNetOnStrings))]
        public string Importe { get; set; }
    }
}