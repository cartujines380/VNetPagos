using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Presentation.Web.Filters;

namespace VisaNet.Presentation.Web.Models
{
    public class PaymentBillModel
    {
        public ICollection<BillModel> Bills { get; set; }
        public bool EnablePartialPayment { get; set; }
        public bool EnableImporte { get; set; }
        public bool EnableBills { get; set; }
        public bool NextPage { get; set; }
        public bool EnableMultipleBills { get; set; }

        public int MinPeso { get; set; }
        public int MaxPeso { get; set; }

        public int MinD { get; set; }
        public int MaxD { get; set; }

        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public int Currency { get; set; } //1 peso, 2 dolar
        [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        [DataMinMax("MinPeso", "MaxPeso", "MinD", "MaxD", "Currency", ErrorMessage = "Valor a ingresar debe ser entre {0} y {1} para la moneda seleccionada.")]
        public double ImporteAmount { get; set; }

        public int PaymentMethod { get; set; } // 1 bill, 2 importe

        public bool DisableEditServicePage { get; set; }
    }
}