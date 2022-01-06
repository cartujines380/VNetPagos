using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.WebService.VisaWCF.EntitiesDto
{
    /// <summary>
    /// Parámetros para la obtención de facturas a pagar.
    /// </summary>
    [DataContract]
    public class BillsData : VisaNetAccessBaseData
    {
        /// <summary>
        /// Identificador único de VisaNet del servicio a pagar.
        /// </summary>
        [Required(ErrorMessage = "ServiceId")]
        [DataMember]
        public string ServiceId { get; set; }
        
        /// <summary>
        /// Pasarela a utilizar para obtener y pagar facturas: { "Banred", "Sistarbanc", "Sucive", "Geocom", "Carretera" }
        /// Es opcional. Si se ingresa null, se utiliza por defecto alguna de las pasarelas habilitadas por el servicio.
        /// </summary>
        [DataMember]
        public string GatewayEnumDto { get; set; }
        
        /// <summary>
        /// Valor para identificar las facturas dentro un servicio, correspondiente a la propiedad "ServiceReferenceName" del servicio. 
        /// </summary>
        [Required(ErrorMessage = "ServiceReferenceNumber")]
        [DataMember]
        public string ServiceReferenceNumber { get; set; }
        /// <summary>
        /// Valor para identificar las facturas dentro un servicio, correspondiente a la propiedad "ServiceReferenceName2" del servicio.
        /// </summary>
        [DataMember]
        public string ServiceReferenceNumber2 { get; set; }
        /// <summary>
        /// Valor para identificar las facturas dentro un servicio, correspondiente a la propiedad "ServiceReferenceName3" del servicio.
        /// </summary>
        [DataMember]
        public string ServiceReferenceNumber3 { get; set; }
        /// <summary>
        /// Valor para identificar las facturas dentro un servicio, correspondiente a la propiedad "ServiceReferenceName4" del servicio.
        /// </summary>
        [DataMember]
        public string ServiceReferenceNumber4 { get; set; }
        /// <summary>
        /// Valor para identificar las facturas dentro un servicio, correspondiente a la propiedad "ServiceReferenceName5" del servicio.
        /// </summary>
        [DataMember]
        public string ServiceReferenceNumber5 { get; set; }
        /// <summary>
        /// Valor para identificar las facturas dentro un servicio, correspondiente a la propiedad "ServiceReferenceName6" del servicio.
        /// </summary>
        [DataMember]
        public string ServiceReferenceNumber6 { get; set; }

        //[Required(ErrorMessage = "CardBinNumbers")]
        //[Range(0, 999999, ErrorMessage = "CardBinNumbers")]
        //public int CardBinNumbers { get; set; }

        /// <summary>
        /// Información del usuario que realiza el pago.
        /// </summary>
        [DataMember]
        [Required(ErrorMessage = "UserData")]
        public UserData UserData { get; set; }
    }
}