using System;
using System.ComponentModel.DataAnnotations;
using VisaNet.Common.Resource.LIF;

namespace VisaNet.LIF.WebApi.Models
{
    public class BillModel
    {
        /// <summary>
        /// Monto de la factura. El valor esperado debe estar multiplicado por 100 ya que se manejarán números con 2 decimales.
        /// </summary>
        [Range(0, int.MaxValue, ErrorMessageResourceType = typeof(LIFStrings), ErrorMessageResourceName = "BillAmount_MinValue")]
        public int Amount { get; set; }

        /// <summary>
        /// Monto gravado de la factura. El valor esperado debe estar multiplicado por 100 ya que se manejarán números con 2 decimales.
        /// </summary>
        [Range(0, int.MaxValue, ErrorMessageResourceType = typeof(LIFStrings), ErrorMessageResourceName = "BillTaxedAmount_MinValue")]
        public int TaxedAmount { get; set; }

        /// <summary>
        /// Indicador de si es o no consumidor final
        /// </summary>
        public bool IsFinalConsumer { get; set; }

        /// <summary>
        /// Moneda en formato ISO 4217 de la factura
        /// </summary>
        [Required(ErrorMessageResourceType = typeof(LIFStrings), ErrorMessageResourceName = "BillCurrency_Required")]
        [MaxLength(3, ErrorMessageResourceType = typeof(LIFStrings), ErrorMessageResourceName = "BillCurrency_MaxLenght")]
        [MinLength(3, ErrorMessageResourceType = typeof(LIFStrings), ErrorMessageResourceName = "BillCurrency_MinLenght")]
        public string Currency { get; set; }

        /// <summary>
        /// Indicador de la ley a aplicar. Valores posibles: 1- Ley 17.934 o 18.999 (Actividad Turistica). 6- Ley 19.210 (Inclusión Financiera)
        /// </summary>
        [Range(1, 6, ErrorMessageResourceType = typeof(LIFStrings), ErrorMessageResourceName = "BillLawId_Required")]
        public int LawId { get; set; }
    }
}