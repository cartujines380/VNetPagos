using System.Collections.Generic;
using System.Runtime.Serialization;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.WebService.VisaWCF.EntitiesDto
{
    /// <summary>
    /// Resultado de la obtención de facturas a pagar.
    /// </summary>
    [DataContract]
    public class BillsResponse : VisaNetAccessBaseResponse
    {
        /// <summary>
        /// Listado de facturas a pagar.
        /// </summary>
        [DataMember]
        public List<VisaNetBillResponse> Response { get; set; }

        public BillsResponse(VisaNetAccessResponseCode responseCode) : base(responseCode) { }
    }

    /// <summary>
    /// Información de la factura.
    /// </summary>
    [DataContract]
    public class VisaNetBillResponse
    {
        /// <summary>
        /// Identificador único de VisaNet de la factura.
        /// </summary>
        [DataMember]
        public string BillId { get; set; }
        /// <summary>
        /// Identificador externo de la factura.
        /// </summary>
        [DataMember]
        public string BillNumber { get; set; }
        /// <summary>
        /// Fecha de vencimiento de la factura.
        /// </summary>
        [DataMember]
        public string ExpirationDate { get; set; }

        ///// <summary>
        ///// Número de factura a pagar (utilizado por Sucive y Geocom)
        ///// </summary>
        //[DataMember]
        //public string PreBillNumber { get; set; }

        /// <summary>
        /// Moneda de la factura: { "UYU", "USD" }
        /// </summary>
        [DataMember]
        public string Currency { get; set; }
        /// <summary>
        /// Concepto de la factura.
        /// </summary>
        [DataMember]
        public string Description { get; set; }

        /// <summary>
        /// Identificador de la pasarela de la transacción (utilizado por Sistarbanc).
        /// </summary>
        [DataMember]
        public string GatewayTransactionId { get; set; }
        //public string GatewayTransactionBrouId { get; set; }
        //public string BanredTransactionId { get; set; }

        /// <summary>
        /// Valor que indica si la factura está habilitada para pago.
        /// </summary>
        [DataMember]
        public bool Payable { get; set; }
        /// <summary>
        /// Valor que indica si la factura es de un consumidor final.
        /// </summary>
        [DataMember]
        public bool FinalConsumer { get; set; }
        /// <summary>
        /// Valor que indica si la factura tiene descuento aplicado.
        /// </summary>
        [DataMember]
        public bool DiscountApplyed { get; set; }

        /// <summary>
        /// Importe sobre el que se aplica el impuesto.
        /// </summary>
        [DataMember]
        public double TotalTaxedAmount { get; set; }
        /// <summary>
        /// Importe de la factura.
        /// </summary>
        [DataMember]
        public double TotalAmount { get; set; }
        /// <summary>
        /// Importe del descuento.
        /// </summary>
        [DataMember]
        public double Discount { get; set; }
        /// <summary>
        /// Importe total a pagar en CyberSource (Importe original - Descuento).
        /// </summary>
        [DataMember]
        public double AmountToCybersource { get; set; }

        /// <summary>
        /// Pasarela utilizada para obtener la factura: { "Banred", "Sistarbanc", "Sucive", "Geocom", "Carretera" }
        /// </summary>
        [DataMember]
        public string Gateway { get; set; }

        /// <summary>
        /// Descripción de las deudas a pagar (utilizado por Sucive y Geocom).
        /// </summary>
        [DataMember]
        public string Lines { get; set; }
        /// <summary>
        /// Número de padrón del vehículo a pagar (utilizado por Sucive y Geocom).
        /// </summary>
        [DataMember]
        public string CensusId { get; set; }
        
        //TODO (yani) estos campos son requeridos, agregar alguna validacion en VisaNetAccess
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

        /// <summary>
        /// Valor para identificar las facturas dentro un servicio, correspondiente a la propiedad "ServiceReferenceName" del servicio al que corresponde esta factura. 
        /// </summary>
        [DataMember]
        public string ServiceReferenceNumber { get; set; }
        /// <summary>
        /// Valor para identificar las facturas dentro un servicio, correspondiente a la propiedad "ServiceReferenceName2" del servicio al que corresponde esta factura. 
        /// </summary>
        [DataMember]
        public string ServiceReferenceNumber2 { get; set; }
        /// <summary>
        /// Valor para identificar las facturas dentro un servicio, correspondiente a la propiedad "ServiceReferenceName3" del servicio al que corresponde esta factura. 
        /// </summary>
        [DataMember]
        public string ServiceReferenceNumber3 { get; set; }
        /// <summary>
        /// Valor para identificar las facturas dentro un servicio, correspondiente a la propiedad "ServiceReferenceName4" del servicio al que corresponde esta factura. 
        /// </summary>
        [DataMember]
        public string ServiceReferenceNumber4 { get; set; }
        /// <summary>
        /// Valor para identificar las facturas dentro un servicio, correspondiente a la propiedad "ServiceReferenceName5" del servicio al que corresponde esta factura. 
        /// </summary>
        [DataMember]
        public string ServiceReferenceNumber5 { get; set; }
        /// <summary>
        /// Valor para identificar las facturas dentro un servicio, correspondiente a la propiedad "ServiceReferenceName6" del servicio al que corresponde esta factura. 
        /// </summary>
        [DataMember]
        public string ServiceReferenceNumber6 { get; set; }

        /// <summary>
        /// Los primeros 6 dígitos del número de la tarjeta de pago.
        /// </summary>
        [DataMember]
        public int CardBinNumbers { get; set; }

        /// <summary>
        /// Código de referencia del ente.
        /// </summary>
        [DataMember]
        public string MerchantReferenceCode { get; set; }

        /// <summary>
        /// Descripción del tipo/categoría del servicio (por ejemplo: Telefonía, Gas, Emergencias).
        /// </summary>
        [DataMember]
        public string ServiceType { get; set; }

        /// <summary>
        /// Valor que indica si el servicio acepta múltiples facturas consolidadas en una única en el momento de pagar.
        /// De lo contrario, las facturas se deben pagar una a una.
        /// </summary>
        [DataMember]
        public bool MultipleBillsAllowed { get; set; }

        /// <summary>
        /// Fecha de inicio del proceso de pago, es decir, la obtención de facturas (utilizado por Sistarbanc).
        /// </summary>
        [DataMember]
        public string CreationDate { get; set; }

        /// <summary>
        /// Identificador del objeto descuento utilizado.
        /// </summary>
        [DataMember]
        public string DiscountObjId { get; set; }
    }
}