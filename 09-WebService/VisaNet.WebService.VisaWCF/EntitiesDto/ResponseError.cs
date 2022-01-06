using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace VisaNet.WebService.VisaWCF.EntitiesDto
{
    /// <summary>
    /// Información del resultado de error.
    /// </summary>
    [ComplexType]
    [DataContract]
    public class ResponseError
    {
        /// <summary>
        /// Código de error.
        /// </summary>
        [DataMember]
        public int ResponseType { get; set; }

        /// <summary>
        /// Mensaje de error.
        /// </summary>
        [DataMember]
        public string ResponseText { get; set; }
    }
}