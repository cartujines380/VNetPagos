using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace VisaNet.WebService.VisaWCF.EntitiesDto
{
    /// <summary>
    /// Parámetros para la obtención de servicios habilitados para pago.
    /// </summary>
    [DataContract]
    public class ServicesData : VisaNetAccessBaseData
    {
        /// <summary>
        /// Fecha y hora de la solicitud (opcional).
        /// </summary>
        [DataMember]
        public string TimeStamp { get; set; } // yyyyMMddhhmmss
    }
}