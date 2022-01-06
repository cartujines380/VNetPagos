using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.WebService.VisaWCF.EntitiesDto
{
    /// <summary>
    /// Parámetros comunes a todos los métodos del Web Service.
    /// </summary>
    [DataContract]
    public class VisaNetAccessBaseData
    {
        /// <summary>
        /// Identificador de la plataforma de pago.
        /// </summary>
        [Required(ErrorMessage = "PaymentPlatform")]
        [DataMember]
        public string PaymentPlatform { get; set; }

        /// <summary> Firma digital. </summary>
        /// <remarks>
        /// <para>La comunicación con VisaNetPagos se realizará mediante SSL. </para>
        /// <para>Para verificar la autenticidad de la información recibida, se utilizará una firma digital que será enviada como parámetro. Esto permitirá asegurar la identidad del servidor que envía la información. </para>
        /// <para>
        /// Para generar dicha firma se deberá:
        /// <list type="number">
        /// <item> <description> Generar un hash (algoritmo SHA-1) concatenando todos los parámetros que se envíen. </description> </item>
        /// <item> <description> Firmar el hash obtenido en el paso anterior, utilizando el algoritmo RSA y una clave privada generada por quien envía la información; el resultado de la misma convertido a formato Base64. </description> </item>
        /// </list>
        /// </para>
        /// <para>Quién envía la información deberá entregar la clave pública (RSA 2048bits) a quien recibe la misma para que éste pueda realizar la verificación de la firma al momento de recibir una solicitud. </para>
        /// <para>A continuación se detalla el orden en que se deben concatenar los parámetros de la firma digital para los diferentes métodos. Tener en cuenta que en caso de que el valor del parámetro sea null, no se agrega. </para>
        /// <list type="table">
        /// <listheader>
        /// <term>Método</term>
        /// <term>Orden de los parámetros</term>
        /// </listheader>
        /// <item>
        /// <term>
        /// GetServices
        /// </term>
        /// <term>
        /// <list type="number"> <item> <description> PaymentPlatform </description> </item> </list>
        /// </term>
        /// </item>
        /// <item>
        /// <term>
        /// SearchBills
        /// </term>
        /// <term>
        /// <list type="number">
        /// <item> <description> PaymentPlatform </description> </item>
        /// <item> <description> ServiceId </description> </item>
        /// <item> <description> ServiceReferenceNumber </description> </item>
        /// <item> <description> ServiceReferenceNumber2 </description> </item>
        /// <item> <description> ServiceReferenceNumber3 </description> </item>
        /// <item> <description> ServiceReferenceNumber4 </description> </item>
        /// <item> <description> ServiceReferenceNumber5 </description> </item>
        /// <item> <description> ServiceReferenceNumber5 </description> </item>
        /// <item> <description> ServiceReferenceNumber6 </description> </item>
        /// <item> <description> GatewayEnumDto </description> </item>
        ///  <item> 
        /// <description> 
        /// Información del usuario (UserInfo): 
        /// <list type="number">
        /// <item> <description> Ci </description> </item>
        /// <item> <description> Email </description> </item>
        /// <item> <description> Name </description> </item>
        /// <item> <description> Surname </description> </item>
        /// <item> <description> Address </description> </item>
        /// </list>
        /// </description> 
        /// </item>
        /// </list>
        /// </term>
        /// </item>
        /// <item>
        /// <term>
        /// PreprocessPayment
        /// </term>
        /// <term>
        /// <list type="number">
        /// <item> <description> PaymentPlatform </description> </item>
        /// <item> 
        /// <description> 
        /// Luego se agrega la información de cada factura (Bills): 
        /// <list type="number">
        /// <item> <description> AmountToCybersource (el separador de decimales debe ser '.') </description> </item>
        /// <item> <description> BillId </description> </item>
        /// <item> <description> BillNumber </description> </item>
        /// <item> <description> Currency </description> </item>
        /// <item> <description> Description </description> </item>
        /// <item> <description> Discount (el separador de decimales debe ser '.') </description> </item>
        /// <item> <description> DiscountApplyed ('true' o 'false') </description> </item>
        /// <item> <description> ExpirationDate (formato: 'yyyyMMdd') </description> </item>
        /// <item> <description> FinalConsumer ('true' o 'false') </description> </item>
        /// <item> <description> Gateway </description> </item>
        /// <item> <description> GatewayTransactionId </description> </item>
        /// <item> <description> CensusId </description> </item>
        /// <item> <description> Lines </description> </item>
        /// <item> <description> ServiceId </description> </item>
        /// <item> <description> Payable ('true' o 'false') </description> </item>
        /// <item> <description> TotalAmount (el separador de decimales debe ser '.') </description> </item>
        /// <item> <description> TotalTaxedAmount (el separador de decimales debe ser '.') </description> </item>
        /// <item> <description> CardBinNumbers </description> </item>
        /// <item> <description> ServiceReferenceNumber </description> </item>
        /// <item> <description> ServiceReferenceNumber2 </description> </item>
        /// <item> <description> ServiceReferenceNumber3 </description> </item>
        /// <item> <description> ServiceReferenceNumber4 </description> </item>
        /// <item> <description> ServiceReferenceNumber5 </description> </item>
        /// <item> <description> ServiceReferenceNumber6 </description> </item>
        /// <item> <description> MerchantReferenceCode </description> </item>
        /// <item> <description> MerchantId </description> </item>
        /// <item> <description> ServiceType </description> </item>
        /// <item> <description> MultipleBillsAllowed ('true' o 'false') </description> </item>
        /// <item> <description> CreationDate </description> </item>
        /// </list>
        /// </description> 
        /// </item>
        /// </list>
        /// </term>
        /// </item>
        /// <item>
        /// <term>
        /// Payment
        /// </term>
        /// <term>
        /// <list type="number">
        /// <item> <description> PaymentPlatform </description> </item>
        /// <item> 
        /// <description> 
        /// Se agrega la información de la factura (Bill): 
        /// <list type="number">
        /// <item> <description> AmountToCybersource (el separador de decimales debe ser '.') </description> </item>
        /// <item> <description> BillId </description> </item>
        /// <item> <description> BillNumber </description> </item>
        /// <item> <description> Currency </description> </item>
        /// <item> <description> Description </description> </item>
        /// <item> <description> Discount (el separador de decimales debe ser '.') </description> </item>
        /// <item> <description> DiscountApplyed ('true' o 'false') </description> </item>
        /// <item> <description> ExpirationDate (formato: 'yyyyMMdd') </description> </item>
        /// <item> <description> FinalConsumer ('true' o 'false') </description> </item>
        /// <item> <description> Gateway </description> </item>
        /// <item> <description> GatewayTransactionId </description> </item>
        /// <item> <description> CensusId </description> </item>
        /// <item> <description> Lines </description> </item>
        /// <item> <description> ServiceId </description> </item>
        /// <item> <description> Payable ('true' o 'false') </description> </item>
        /// <item> <description> TotalAmount (el separador de decimales debe ser '.') </description> </item>
        /// <item> <description> TotalTaxedAmount (el separador de decimales debe ser '.') </description> </item>
        /// <item> <description> CardBinNumbers </description> </item>
        /// <item> <description> ServiceReferenceNumber </description> </item>
        /// <item> <description> ServiceReferenceNumber2 </description> </item>
        /// <item> <description> ServiceReferenceNumber3 </description> </item>
        /// <item> <description> ServiceReferenceNumber4 </description> </item>
        /// <item> <description> ServiceReferenceNumber5 </description> </item>
        /// <item> <description> ServiceReferenceNumber6 </description> </item>
        /// <item> <description> MerchantReferenceCode </description> </item>
        /// <item> <description> MerchantId </description> </item>
        /// <item> <description> ServiceType </description> </item>
        /// <item> <description> MultipleBillsAllowed ('true' o 'false') </description> </item>
        /// <item> <description> CreationDate </description> </item>
        /// <item> <description> DiscountObjId </description> </item>
        /// </list>
        /// </description> 
        /// </item>
        /// <item> 
        /// <description> 
        /// Luego se agrega la información de la tarjeta (CardData): 
        /// <list type="number">
        /// <item> <description> CardBinNumbers </description> </item>
        /// <item> <description> DueDate </description> </item>
        /// <item> <description> MaskedNumber </description> </item>
        /// <item> <description> Name </description> </item>
        /// </list>
        /// </description> 
        /// </item>
        /// <item> 
        /// <description> 
        /// Después se agrega la información de CyberSource (CyberSourceData): 
        /// <list type="number">
        /// <item> <description> TransactionId </description> </item>
        /// <item> <description> PaymentToken </description> </item>
        /// <item> <description> ReasonCode </description> </item>
        /// </list>
        /// </description> 
        /// </item>
        /// <item> <description> ServiceId </description> </item>
        /// <item> 
        /// <description> 
        /// Por último se agrega la información del usuario (UserInfo): 
        /// <list type="number">
        /// <item> <description> Email </description> </item>
        /// </list>
        /// </description> 
        /// </item>
        /// </list>
        /// </term>
        /// </item>
        /// <item>
        /// <term>
        /// SearchPayments
        /// </term>
        /// <term>
        /// <list type="number">
        /// <item> <description> PaymentPlatform </description> </item>
        /// <item> <description> TransactionId </description> </item>
        /// <item> <description> ServiceId </description> </item>
        /// <item> <description> FromDate (formato: 'yyyyMMdd') </description> </item>
        /// <item> <description> ToDate (formato: 'yyyyMMdd') </description> </item>
        /// </list>
        /// </term>
        /// </item>
        /// <item>
        /// <term>
        /// NotifyOperationResult
        /// </term>
        /// <term>
        /// <list type="number">
        /// <item> <description> PaymentPlatform </description> </item>
        /// <item> <description> NotificationType (ejemplo: 'Ok') </description> </item>
        /// <item> <description> Operation (ejemplo: 'Payment') </description> </item>
        /// <item> <description> Message </description> </item>
        /// </list>
        /// </term>
        /// </item>
        /// </list>
        /// </remarks>
        [Required(ErrorMessage = "DigitalSignature")]
        [DataMember]
        public string DigitalSignature { get; set; }
    }
}