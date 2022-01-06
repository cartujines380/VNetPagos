using System.Collections.Generic;
using System.Runtime.Serialization;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.WebService.VisaWCF.EntitiesDto
{
    /// <summary>
    /// Resultado de la obtención de servicios habilitados para pago.
    /// </summary>
    [DataContract]
    public class ServicesResponse : VisaNetAccessBaseResponse
    {
        /// <summary>
        /// Listado de servicios habilitados para pago.
        /// </summary>
        [DataMember]
        public List<VisaNetServices> Response { get; set; }

        public ServicesResponse(VisaNetAccessResponseCode responseCode) : base(responseCode) { }
    }

    /// <summary>
    /// Contiene la información del servicio habilitado para pago.
    /// </summary>
    [DataContract]
    public class VisaNetServices
    {
        /// <summary>
        /// Valor que indica si el servicio está activo.
        /// </summary>
        [DataMember]
        public bool Active { get; set; }
        /// <summary>
        /// Descripción del tipo/categoría del servicio (por ejemplo: Telefonía, Gas, Emergencias).
        /// </summary>
        [DataMember]
        public string ServiceType { get; set; }
        /// <summary>
        /// Nombre del servicio.
        /// </summary>
        [DataMember]
        public string ServiceName { get; set; }
        /// <summary>
        /// Nombre de las propiedades para identificar una factura del servicio (por ejemplo: Número de documento o Matrícula, Padrón y Departamento).
        /// </summary>
        [DataMember]
        public string ServiceReferenceName { get; set; }
        /// <summary>
        /// Nombre de las propiedades para identificar una factura del servicio (por ejemplo: Número de documento o Matrícula, Padrón y Departamento).
        /// </summary>
        [DataMember]
        public string ServiceReferenceName2 { get; set; }
        /// <summary>
        /// Nombre de las propiedades para identificar una factura del servicio (por ejemplo: Número de documento o Matrícula, Padrón y Departamento).
        /// </summary>
        [DataMember]
        public string ServiceReferenceName3 { get; set; }
        /// <summary>
        /// Nombre de las propiedades para identificar una factura del servicio (por ejemplo: Número de documento o Matrícula, Padrón y Departamento).
        /// </summary>
        [DataMember]
        public string ServiceReferenceName4 { get; set; }
        /// <summary>
        /// Nombre de las propiedades para identificar una factura del servicio (por ejemplo: Número de documento o Matrícula, Padrón y Departamento).
        /// </summary>
        [DataMember]
        public string ServiceReferenceName5 { get; set; }
        /// <summary>
        /// Nombre de las propiedades para identificar una factura del servicio (por ejemplo: Número de documento o Matrícula, Padrón y Departamento).
        /// </summary>
        [DataMember]
        public string ServiceReferenceName6 { get; set; }

        /// <summary>
        /// Identificador único de VisaNet del servicio.
        /// </summary>
        [DataMember]
        public string ServiceId { get; set; }

        /// <summary>
        /// Identificador externo del servicio.
        /// </summary>
        [DataMember]
        public string MerchantId { get; set; }
        //public string CybersourceTransactionKey { get; set; }

        /// <summary>
        /// Listado de pasarelas habilitadas para la obtención y pago de facturas del servicio. Las mismas pueden ser: { "Banred", "Sistarbanc", "Sucive", "Geocom", "Carretera" }
        /// </summary>
        [DataMember]
        public List<string> Gateways { get; set; }

        /// <summary>
        /// Valor que indica si el servicio acepta tarjetas de débito como forma de pago.
        /// </summary>
        [DataMember]
        public bool DebitCard { get; set; }
        /// <summary>
        /// Valor que indica si el servicio acepta tarjetas de crédito como forma de pago.
        /// </summary>
        [DataMember]
        public bool CreditCard { get; set; }
        /// <summary>
        /// Valor que indica si el servicio acepta tarjetas de débito extranjeras como forma de pago.
        /// </summary>
        [DataMember]
        public bool DebitCardInternational { get; set; }
        /// <summary>
        /// Valor que indica si el servicio acepta tarjetas de crédito extranjeras como forma de pago.
        /// </summary>
        [DataMember]
        public bool CreditCardInternational { get; set; }

        /// <summary>
        /// Valor que indica si el servicio acepta múltiples facturas consolidadas en una única en el momento de pagar.
        /// De lo contrario, las facturas se deben pagar una a una.
        /// </summary>
        [DataMember]
        public bool MultipleBillsAllowed { get; set; }
    }
}