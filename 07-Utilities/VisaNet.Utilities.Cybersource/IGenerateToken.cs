using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Utilities.Cybersource
{
    public interface IGenerateToken
    {
        //MDD2 - Pago puntual o suscripcion
        OperationTypeDto OperationTypeDto { get; set; }

        //MDD5 - Nro Ref 1
        string ReferenceNumber1 { get; set; }
        //MDD6 - Nro Ref 2
        string ReferenceNumber2 { get; set; }
        //MDD7 - Nro Ref 3
        string ReferenceNumber3 { get; set; }
        //MDD8 - Nro Ref 4
        string ReferenceNumber4 { get; set; }
        //MDD9 - Nro Ref 5
        string ReferenceNumber5 { get; set; }
        //MDD10 - Nro Ref 6
        string ReferenceNumber6 { get; set; }

        //MDD11 - Dato interno de flujo
        string RedirectTo { get; set; }
        string UrlReturn { get; set; }

        //MDD18 - Nombre de TH
        string NameTh { get; set; }
        //MDD24 - CardBin 
        string CardBin { get; set; }

        //MDD26 - CallcenterUser 
        string CallcenterUser { get; set; }

        //MDD27 - Operation Id
        string OperationId { get; set; }

        //MDD28 - Plataforma
        string Platform { get; set; }

        //MDD29 - ID de transaccion temporal
        string TemporaryTransactionIdentifier { get; set; }

        //MDD35 -
        PaymentTypeDto PaymentTypeDto { get; set; }

        //MDD48 -
        CardTypeDto CardTypeDto { get; set; }

        //ID único para pagos
        string CyberSourceIdentifier { get; set; }

        string TransactionReferenceNumber { get; set; }

        string FingerPrint { get; set; }
    }
}
