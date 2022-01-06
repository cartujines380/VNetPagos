using System.Collections.Generic;
using System.Runtime.Serialization;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.WebService.VisaWCF.EntitiesDto
{
    /// <summary>
    /// Código de respuesta.
    /// </summary>
    public enum VisaNetAccessResponseCode
    {
        Ok = 1,
        Error = 2
    }

    /// <summary>
    /// Propiedades comunes a los resultados de todos los métodos del Web Service.
    /// </summary>
    [DataContract]
    public class VisaNetAccessBaseResponse
    {
        /// <summary>
        /// Código de respuesta.
        /// </summary>
        [DataMember]
        public VisaNetAccessResponseCode ResponseCode { get; set; }
        
        /// <summary>
        /// Información de resultado de error.
        /// </summary>
        [DataMember]
        public ResponseError ResponseError { get; set; }

        public VisaNetAccessBaseResponse(VisaNetAccessResponseCode responseCode)
        {
            ResponseCode = responseCode;
        }
    }
}