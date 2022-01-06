using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Domain.EntitiesDtos
{
    public class PaymentDto
    {
        public PaymentDto()
        {
            CyberSourceData = new CyberSourceDataDto();
            VerifyByVisaData = new VerifyByVisaDataDto();

            Bills = new Collection<BillDto>();
        }
        public Guid Id { get; set; }

        public DateTime Date { get; set; }

        public Guid CardId { get; set; }
        public CardDto Card { get; set; }

        public Guid GatewayId { get; set; }
        public GatewayDto GatewayDto { get; set; }

        public Guid ServiceId { get; set; }
        public ServiceDto ServiceDto { get; set; }

        public Guid? ServiceAssociatedId { get; set; }
        public ServiceAssociatedDto ServiceAssociatedDto { get; set; }

        [StringLength(100, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string ReferenceNumber { get; set; }

        [StringLength(100, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string ReferenceNumber2 { get; set; }

        [StringLength(100, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string ReferenceNumber3 { get; set; }

        [StringLength(100, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string ReferenceNumber4 { get; set; }

        [StringLength(100, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string ReferenceNumber5 { get; set; }

        [StringLength(100, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string ReferenceNumber6 { get; set; }

        [StringLength(250, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string Description { get; set; }

        public ICollection<BillDto> Bills { get; set; }

        public Guid? RegisteredUserId { get; set; }
        public ApplicationUserDto RegisteredUser { get; set; }

        public Guid? AnonymousUserId { get; set; }
        public AnonymousUserDto AnonymousUser { get; set; }

        public PaymentTypeDto PaymentType { get; set; }

        public CyberSourceDataDto CyberSourceData { get; set; }
        public VerifyByVisaDataDto VerifyByVisaData { get; set; }

        public Guid PaymentIdentifierId { get; set; }
        public PaymentIdentifierDto PaymentIdentifierDto { get; set; }

        [StringLength(100)]
        public string TransactionNumber { get; set; }

        public GatewayEnumDto GatewayEnum { get; set; }


        public string Currency { get; set; }
        //Aplica descuento
        public bool DiscountApplyed { get; set; }
        //Total facturas
        public double TotalAmount { get; set; }
        //Total gravado
        public double TotalTaxedAmount { get; set; }
        //Descuento
        public double Discount { get; set; }

        public Guid? DiscountObjId { get; set; }
        public DiscountDto DiscountObj { get; set; }

        //Monto enviado a cybersource 
        public double AmountTocybersource { get; set; }

        public PaymentPlatformDto PaymentPlatform { get; set; }
        public PaymentStatusDto PaymentStatus { get; set; }

        public int Quotas { get; set; }

        public string IdOperation { get; set; }
        public string IdUserExternal { get; set; }
    }

    public class CyberSourceDataDto
    {
        public string Decision { get; set; }
        public string ReasonCode { get; set; }
        public string TransactionId { get; set; }
        public string Message { get; set; }
        public string BillTransRefNo { get; set; }

        public string ReqCardNumber { get; set; }
        public string ReqCardExpiryDate { get; set; }
        public string ReqProfileId { get; set; }
        public string ReqCardType { get; set; }
        public string ReqPaymentMethod { get; set; }
        public string ReqTransactionType { get; set; }
        public string ReqTransactionUuid { get; set; }
        public string ReqCurrency { get; set; }
        public string ReqReferenceNumber { get; set; }
        public string ReqAmount { get; set; }

        public string AuthAvsCode { get; set; }
        public string AuthCode { get; set; }
        public string AuthAmount { get; set; }
        public string AuthTime { get; set; }
        public string AuthResponse { get; set; }
        public string AuthTransRefNo { get; set; }

        public string PaymentToken { get; set; }
    }

    public class VerifyByVisaDataDto
    {
        public string PayerAuthenticationEci { get; set; }
        public string PayerAuthenticationXid { get; set; }
        public string PayerAuthenticationCavv { get; set; }
        public string PayerAuthenticationProofXml { get; set; }
    }

    public class PaymentIdentifierDto
    {
        public Guid Id { get; set; }
        public string CyberSourceTransactionIdentifier { get; set; }
        public Int64 UniqueIdentifier { get; set; }
    }
}
