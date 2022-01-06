using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.WebService.VisaWCF.EntitiesDto
{
    /// <summary>
    /// Resultado de la realización de un pago.
    /// </summary>
    [DataContract]
    public class PaymentResponse : VisaNetAccessBaseResponse
    {
        /// <summary>
        /// Resultado de las operaciones de CyberSource.
        /// </summary>
        [DataMember]
        public CyberSourceData CyberSourceData { get; set; }
        /// <summary>
        /// Pago realizado.
        /// </summary>
        [DataMember]
        public VisaNetPayment Response { get; set; }

        public PaymentResponse(VisaNetAccessResponseCode responseCode) : base(responseCode) { }
    }

    /// <summary>
    /// Información del pago realizado.
    /// </summary>
    [DataContract]
    public class VisaNetPayment
    {
        /// <summary>
        /// Factura pagada.
        /// </summary>
        [DataMember]
        public VisaNetBillResponse Bill { get; set; }
        /// <summary>
        /// Fecha y hora del pago.
        /// </summary>
        [DataMember]
        public DateTime DateTime { get; set; }
        /// <summary>
        /// Identificador de Sistarbanc de la transacción.
        /// </summary>
        [DataMember]
        public string GatewayTransactionId { get; set; }
        /// <summary>
        /// Identificador de la transacción.
        /// </summary>
        [DataMember]
        public string TransactionId { get; set; }
        /// <summary>
        /// Información del usuario.
        /// </summary>
        [DataMember]
        public UserData UserInfo { get; set; }
    }
}