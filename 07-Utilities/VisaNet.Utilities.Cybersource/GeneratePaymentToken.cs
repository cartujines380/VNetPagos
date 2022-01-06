using System;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Utilities.Cybersource
{
    public class GeneratePaymentToken : IGenerateToken
    {
        public GeneratePaymentToken()
        {
            ServiceId = Guid.Empty;
            UserId = Guid.Empty;
            CardId = Guid.Empty;
            TransactionReferenceNumber = string.Empty;
            ReferenceNumber1 = string.Empty;
            ReferenceNumber2 = string.Empty;
            ReferenceNumber3 = string.Empty;
            ReferenceNumber4 = string.Empty;
            ReferenceNumber5 = string.Empty;
            ReferenceNumber6 = string.Empty;
            RedirctTo = string.Empty;
            FingerPrint = string.Empty;
            AditionalNumberElectornicBill = string.Empty;
            NameTh = string.Empty;
            CyberSourceIdentifier = string.Empty;
            CardBin = string.Empty;
            CallcenterUser = string.Empty;
            OperationId = string.Empty;
            Platform = string.Empty;
            TemporaryTransactionIdentifier = Guid.NewGuid().ToString();
            CybersourceAmount = 0;
            GatewayId = Guid.Empty;
            DiscountObjId = Guid.Empty;
            PaymentTypeDto = PaymentTypeDto.AnonymousUser;
            Bill = new BillForToken()
            {
                BillExpirationDate = string.Empty,
                BillDescription = string.Empty,
                BillGatewayTransactionId = string.Empty,
                BillSucivePreBillNumber = string.Empty,
                BillFinalConsumer = string.Empty,
                BillDiscount = string.Empty,
                BillDateInitTransaccion = string.Empty,
                BillGatewayTransactionBrouId = string.Empty,
                Amount = 0,
                Currency = string.Empty,
                DiscountAmount = 0,
                BillNumber = string.Empty,
                TaxedAmount = 0,
                DiscountType = 0,
                IdPadron = string.Empty,
            };
            Quota = 1;
            CardTypeDto = CardTypeDto.Credit;
        }

        public string TransactionReferenceNumber { get; set; }
        public double CybersourceAmount { get; set; }
        public string FingerPrint { get; set; }
        public string UrlReturn { get; set; }

        public BillForToken Bill { get; set; }

        //MDD2 - Pago puntual o suscripcion
        public OperationTypeDto OperationTypeDto { get; set; }

        //MDD5 - Nro Ref 1
        public string ReferenceNumber1 { get; set; }
        //MDD6 - Nro Ref 2
        public string ReferenceNumber2 { get; set; }
        //MDD7 - Nro Ref 3
        public string ReferenceNumber3 { get; set; }
        //MDD8 - Nro Ref 4
        public string ReferenceNumber4 { get; set; }
        //MDD9 - Nro Ref 5
        public string ReferenceNumber5 { get; set; }
        //MDD10 - Nro Ref 6
        public string ReferenceNumber6 { get; set; }
        //MDD11 - Dato interno de flujo
        public string RedirctTo { get; set; }
        //MDD17 - Número de factura adicional por facturación electrónica
        public string AditionalNumberElectornicBill { get; set; }
        //MDD18 - Nombre de TH
        public string NameTh { get; set; }
        //MDD24 - CardBin 
        public string CardBin { get; set; }
        //MDD26 - CallcenterUser 
        public string CallcenterUser { get; set; }
        //ID único para pagos
        public string CyberSourceIdentifier { get; set; }
        //MDD27 - Operation Id
        public string OperationId { get; set; }
        //MDD28 - Plataforma
        public string Platform { get; set; }
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
        //MDD45 -
        public string DescriptionService { get; set; }
        //MDD47 -
        public int Quota { get; set; }
        //MDD48 -
        public CardTypeDto CardTypeDto { get; set; }

        public NotificationConfigDto NotificationsConfig { get; set; }
    }
}
