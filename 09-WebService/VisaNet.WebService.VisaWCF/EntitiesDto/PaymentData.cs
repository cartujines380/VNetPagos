using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace VisaNet.WebService.VisaWCF.EntitiesDto
{
    /// <summary>
    /// Parámetros para la realización del pago de una factura.
    /// </summary>
    [DataContract]
    public class PaymentData : VisaNetAccessBaseData
    {
        /// <summary>
        /// Factura a pagar, obtenida dentro del resultado del preprocesamiento.
        /// </summary>
        [Required(ErrorMessage = "Bill")]
        [DataMember]
        public VisaNetBillResponse Bill { get; set; }
        
        /// <summary>
        /// Resultado de la operación en CyberSource.
        /// </summary>
        [Required(ErrorMessage = "CyberSourceData")]
        [DataMember]
        public VisaNetCyberSourceData CyberSourceData { get; set; }
        
        /// <summary>
        /// Información del usuario que realiza el pago.
        /// </summary>
        [Required(ErrorMessage = "UserInfo")]
        [DataMember]
        public UserData UserInfo { get; set; }
        
        /// <summary>
        /// Información de la tarjeta con la que se efectuará el pago.
        /// </summary>
        [Required(ErrorMessage = "CardData")]
        [DataMember]
        public CardData CardData { get; set; }

        /// <summary>
        /// Identificador único de VisaNet del servicio a pagar.
        /// </summary>
        [Required(ErrorMessage = "ServiceId")]
        [DataMember]
        public string ServiceId { get; set; }
    }

    //TODO (yani) detallar parámetros
    /// <summary>
    /// Resultado de la operación en CyberSource.
    /// </summary>
    [DataContract]
    public class VisaNetCyberSourceData
    {
        /// <summary>
        /// AuthAmount
        /// </summary>
        [DataMember]
        public string AuthAmount { get; set; }
        /// <summary>
        /// AuthTime
        /// </summary>
        [DataMember]
        public string AuthTime { get; set; }
        /// <summary>
        /// AuthCode
        /// </summary>
        [DataMember]
        public string AuthCode { get; set; }
        /// <summary>
        /// AuthAvsCode
        /// </summary>
        [DataMember]
        public string AuthAvsCode { get; set; }
        /// <summary>
        /// AuthResponse
        /// </summary>
        [DataMember]
        public string AuthResponse { get; set; }
        /// <summary>
        /// AuthTransRefNo
        /// </summary>
        [DataMember]
        public string AuthTransRefNo { get; set; }
        /// <summary>
        /// Decision
        /// </summary>
        [DataMember]
        public string Decision { get; set; }
        /// <summary>
        /// BillTransRefNo
        /// </summary>
        [DataMember]
        public string BillTransRefNo { get; set; }
        /// <summary>
        /// PaymentToken
        /// </summary>
        [DataMember]
        public string PaymentToken { get; set; }
        /// <summary>
        /// ReasonCode
        /// </summary>
        [DataMember]
        public string ReasonCode { get; set; }
        /// <summary>
        /// ReqAmount
        /// </summary>
        [DataMember]
        public string ReqAmount { get; set; }
        /// <summary>
        /// ReqCurrency
        /// </summary>
        [DataMember]
        public string ReqCurrency { get; set; }
        /// <summary>
        /// TransactionId
        /// </summary>
        [DataMember]
        public string TransactionId { get; set; }
        /// <summary>
        /// ReqTransactionUuid
        /// </summary>
        [DataMember]
        public string ReqTransactionUuid { get; set; }
        /// <summary>
        /// ReqReferenceNumber
        /// </summary>
        [DataMember]
        public string ReqReferenceNumber { get; set; }
        /// <summary>
        /// ReqTransactionType
        /// </summary>
        [DataMember]
        public string ReqTransactionType { get; set; }
    }

    /// <summary>
    /// Información de la tarjeta de pago.
    /// </summary>
    [DataContract]
    public class CardData
    {
        /// <summary>
        /// Los primeros 6 dígitos del número de la tarjeta de pago.
        /// </summary>
        [Required(ErrorMessage = "CardBinNumbers")]
        [Range(0, 999999, ErrorMessage = "CardBinNumbers")]
        [DataMember]
        public int CardBinNumbers { get; set; }
        /// <summary>
        /// Número de la tarjeta enmascarado (en formato: "411111xxxxxx1111")
        /// </summary>
        [DataMember]
        public string MaskedNumber { get; set; }
        /// <summary>
        /// Fecha de vencimiento de la tarjeta (en formato: "MMyy").
        /// </summary>
        [DataMember]
        public string DueDate { get; set; }
        /// <summary>
        /// Nombre del titular de la tarjeta.
        /// </summary>
        [DataMember]
        public string Name { get; set; }
        /// <summary>
        /// Teléfono.
        /// </summary>
        [DataMember]
        public string Phone { get; set; }
    }
}