using System;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Utilities.Cybersource
{
    public class GenerateCardTokenApps : IGenerateToken
    {
        public GenerateCardTokenApps()
        {
            ServiceId = Guid.Empty;
            UserId = Guid.Empty;
            TransactionReferenceNumber = string.Empty;
            ReferenceNumber1 = string.Empty;
            ReferenceNumber2 = string.Empty;
            ReferenceNumber3 = string.Empty;
            ReferenceNumber4 = string.Empty;
            ReferenceNumber5 = string.Empty;
            ReferenceNumber6 = string.Empty;
            RedirctTo = string.Empty;
            FingerPrint = string.Empty;
            NameTh = string.Empty;
            CyberSourceIdentifier = string.Empty;
            CardBin = string.Empty;
            OperationId = string.Empty;
            Platform = string.Empty;
            TemporaryTransactionIdentifier = Guid.NewGuid().ToString();
            GatewayId = Guid.Empty;
        }
        public string TransactionReferenceNumber { get; set; }
        public double CybersourceAmount { get; set; }
        public string FingerPrint { get; set; }
        public string UrlReturn { get; set; }
        
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
        //MDD33 -
        public Guid GatewayId { get; set; }
        //MDD35 -
        public PaymentTypeDto PaymentTypeDto { get; set; }

        public ApplicationUserDto User { get; set; }


    }
}
