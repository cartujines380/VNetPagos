using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace VisaNet.WebService.VisaWCF.EntitiesDto
{
    /// <summary>
    /// Resultado de las operaciones de CyberSource.
    /// </summary>
    [DataContract]
    public class CyberSourceData
    {
        /// <summary>
        /// Resultado del pago en CyberSource. Si es null, o bien no se realizó o hubo un error de VisaNet al hacerlo.
        /// </summary>
        [DataMember]
        public CsResponse PaymentData { get; set; }
        /// <summary>
        /// Resultado del void en CyberSource. Si es null, o bien no se realizó o hubo un error de VisaNet al hacerlo.
        /// </summary>
        [DataMember]
        public CsResponse VoidData { get; set; }
        /// <summary>
        /// Resultado del refund en CyberSource. Si es null, o bien no se realizó o hubo un error de VisaNet al hacerlo.
        /// </summary>
        [DataMember]
        public CsResponse RefundData { get; set; }
        /// <summary>
        /// Resultado del reversal en CyberSource. Si es null, o bien no se realizó o hubo un error de VisaNet al hacerlo.
        /// </summary>
        [DataMember]
        public CsResponse ReversalData { get; set; }
    }

    /// <summary>
    /// Resultado de operación de CyberSource.
    /// </summary>
    [DataContract]
    public class CsResponse
    {
        /// <summary>
        /// Código de respuesta (100 = OK)
        /// </summary>
        [DataMember]
        public string PaymentResponseCode { get; set; }
        /// <summary>
        /// Id de la transacción.
        /// </summary>
        [DataMember]
        public string PaymentRequestId { get; set; }
    }
}