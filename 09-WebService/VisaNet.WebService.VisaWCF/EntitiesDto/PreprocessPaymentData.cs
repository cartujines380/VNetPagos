using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace VisaNet.WebService.VisaWCF.EntitiesDto
{
    /// <summary>
    /// Parámetros para el preprocesamiento de facturas a pagar.
    /// </summary>
    [DataContract]
    public class PreprocessPaymentData : VisaNetAccessBaseData
    {
        /// <summary>
        /// Listado de facturas a pagar, obtenidas como resultado de la obtención de facturas. 
        /// Deben corresponder todas al mismo servicio y pasarela, y haber al menos una.
        /// </summary>
        [Required(ErrorMessage = "Bills")]
        [DataMember]
        public List<VisaNetBillResponse> Bills { get; set; }
    }
}