using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace VisaNet.WebService.VisaWCF.EntitiesDto
{
    /// <summary>
    /// Parámetros para la búsqueda de pagos realizados.
    /// </summary>
    [DataContract]
    public class SearchPaymentsData : VisaNetAccessBaseData
    {
        /// <summary>
        /// Identificador de la transacción del pago de la factura (opcional)
        /// </summary>
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "TransactionId")]
        [DataMember]
        public string TransactionId { get; set; }

        /// <summary>
        /// Identificador único de VisaNet del servicio (opcional)
        /// </summary>
        [DataMember]
        public string ServiceId { get; set; }

        /// <summary>
        /// Fecha desde (en el formato: "dd/MM/yyyy")
        /// </summary>
        [RegularExpression(@"^(([0-2]?\d{1})|([3][0,1]{1}))/(0[1-9]|1[0-2])/(([1]{1}[9]{1}[9]{1}\d{1})|([2-9]{1}\d{3}))$", ErrorMessage = "FromDate")]
        [DataMember]
        public string FromDate { get; set; }

        /// <summary>
        /// Fecha hasta (en el formato: "dd/MM/yyyy")
        /// </summary>
        [RegularExpression(@"^(([0-2]?\d{1})|([3][0,1]{1}))/(0[1-9]|1[0-2])/(([1]{1}[9]{1}[9]{1}\d{1})|([2-9]{1}\d{3}))$", ErrorMessage = "ToDate")]
        [DataMember]
        public string ToDate { get; set; }
    }
}