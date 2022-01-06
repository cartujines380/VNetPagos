using System;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Domain.EntitiesDtos
{
    public class CyberSourceMerchantDefinedDataDto
    {
        //MDD1
        public string ServiceType { get; set; }
        //MDD2
        public string OperationTypeDto { get; set; }
        //MDD3
        public string UserRegistered { get; set; }
        //MDD4
        public string UserRegisteredDays { get; set; }
        //MDD5
        public string ReferenceNumber1 { get; set; }
        //MDD6
        public string ReferenceNumber2 { get; set; }
        //MDD7
        public string ReferenceNumber3 { get; set; }
        //MDD8
        public string ReferenceNumber4 { get; set; }
        //MDD9
        public string ReferenceNumber5 { get; set; }
        //MDD10
        public string ReferenceNumber6 { get; set; }
        //MDD11
        public string RedirctTo { get; set; }
        //MDD12
        public string DiscountApplyed { get; set; }
        //MDD13
        public string TotalAmount { get; set; }
        //MDD14
        public string TotalTaxedAmount { get; set; }
        //MDD15
        public string Discount { get; set; }
        //MDD16
        public string BillNumber { get; set; }
        //MDD17 - Número de factura adicional por facturación electrónica
        public string AditionalNumberElectornicBill { get; set; }
        //MDD18 - Nombre de TH
        public string NameTh { get; set; }
        //MDD19 - Cantidad de pagos anteriores a mismo servicio
        public int PaymentCount { get; set; }
        //MDD20 - Dirección del usuario
        public string UserRegisteredAddress { get; set; }
        //MDD21 - Teléfono móvil (solo para usuarios registrados)
        public string UserMobile { get; set; }
        //MDD22 - CI (solo para usuarios registrados)
        public string UserCi { get; set; }
        //MDD23 - Merchand Id 
        public string MerchandId { get; set; }
        //MDD24 - CardBin 
        public string CardBin { get; set; }
        //MDD25 - ServiceName 
        public string ServiceName { get; set; }
        //MDD26 - CallcenterUser 
        public string CallcenterUser { get; set; }
        //MDD27 - Operation Id
        public string OperationId { get; set; }
        //MDD28 - PLATAFORMA
        public string Plataform { get; set; }
        //MDD29 - ID de transaccion temporal
        public string TemporaryTransactionIdentifier { get; set; }
        //MDD30 - 
        public Guid ServiceId { get; set; }
        //MDD31 - 
        public Guid UserId { get; set; }
        //MDD32 - 
        public Guid CardId { get; set; }
        //MDD33 -
        public Guid GatewayId { get; set; }
        //MDD34 -
        public Guid DiscountObjId { get; set; }
        //MDD35 -
        public PaymentTypeDto PaymentTypeDto { get; set; }
        //MDD36 -
        public string BillExpirationDate { get; set; }
        //MDD37 -
        public string BillDescription { get; set; }
        //MDD38 -
        public string BillGatewayTransactionId { get; set; }
        //MDD39 -
        public string BillSucivePreBillNumber { get; set; }
        //MDD40 -
        public string BillFinalConsumer { get; set; }
        //MDD41 -
        public string BillDiscount { get; set; }
        //MDD42 -
        public string BillDateInitTransaccion { get; set; }
        //MDD43 -
        public string BillGatewayTransactionBrouId { get; set; }
        //MDD44 -
        public string NotificationConfig { get; set; }
        //MDD45 -
        public string DescriptionService { get; set; }
        //MDD46 -
        public string PasswordHashed { get; set; }
        //MDD47 -
        public int Quota { get; set; }
        //MDD48 -
        public CardTypeDto CardTypeDto { get; set; }

        //MDD50 -
        public string CommerceAndProduct { get; set; }
        //MDD51 -
        public Guid CommerceId { get; set; }
        //MDD52 -
        public Guid ProductId { get; set; }

    }
}
