using System.ComponentModel.DataAnnotations;
using VisaNet.Common.Resource.LIF;

namespace VisaNet.LIF.WebApi.Models
{
    public class DiscountCalculationAppInModel
    {
        /// <summary>
        /// Valor del BIN de la tarjeta
        /// </summary>
        [MaxLength(6, ErrorMessageResourceType = typeof(LIFStrings), ErrorMessageResourceName = "Bin_MaxLenght")]
        [MinLength(6, ErrorMessageResourceType = typeof(LIFStrings), ErrorMessageResourceName = "Bin_MinLenght")]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(LIFStrings), ErrorMessageResourceName = "Bin_Required")]
        public string Bin { get; set; }

        /// <summary>
        /// Objeto que representa una factura
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(LIFStrings), ErrorMessageResourceName = "Bill_Required")]
        public BillModel Bill { get; set; }

        /// <summary>
        /// Identificador único de la aplicación que llama al servicio
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(LIFStrings), ErrorMessageResourceName = "AppId_Required")]
        public string AppId { get; set; }

        /// <summary>
        /// Identificador único de la llamada al servicio
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(LIFStrings), ErrorMessageResourceName = "OperationId_Required")]
        public string OperationId { get; set; }
    }
}