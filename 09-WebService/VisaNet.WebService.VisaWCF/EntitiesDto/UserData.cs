using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace VisaNet.WebService.VisaWCF.EntitiesDto
{
    /// <summary>
    /// Información del usuario que realiza el pago.
    /// </summary>
    [ComplexType]
    [DataContract]
    public class UserData
    {
        /// <summary>
        /// Email
        /// </summary>
        [DataMember]
        [Required(ErrorMessage = "Email")]
        public string Email { get; set; }
        /// <summary>
        /// Documento de identidad
        /// </summary>
        [DataMember]
        public string Ci { get; set; }
        /// <summary>
        /// Dirección
        /// </summary>
        [DataMember]
        public string Address { get; set; }
        /// <summary>
        /// Nombre
        /// </summary>
        [DataMember]
        public string Name { get; set; }
        /// <summary>
        /// Apellido
        /// </summary>
        [DataMember]
        public string Surname { get; set; }
    }
}