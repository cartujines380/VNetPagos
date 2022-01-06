using System;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.WebApi.Models
{
    public class CyberSourcePostModel
    {
        public string ReasonCode { get; set; }
        public string TransactionId { get; set; }
        public string UserId { get; set; }
        public string Decision { get; set; }
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
        public DateTime DateTime { get; set; }
        public string Platform { get; set; }
        public string OperationId { get; set; }
        public string ServiceId { get; set; }
        public string CardId { get; set; }
        public bool IsUserRegistered { get; set; }

        public CyberSourceAcknowledgementDto ToDto()
        {
            return new CyberSourceAcknowledgementDto
            {
                ReasonCode = int.Parse(ReasonCode),
                TransactionId = TransactionId,
                UserId = UserId,
                Decision = Decision,
                Message = Message,
                BillTransRefNo = BillTransRefNo,
                ReqCardNumber = ReqCardNumber,
                ReqCardExpiryDate = ReqCardExpiryDate,
                ReqProfileId = ReqProfileId,
                ReqCardType = ReqCardType,
                ReqPaymentMethod = ReqPaymentMethod,
                ReqTransactionType = ReqTransactionType,
                ReqTransactionUuid = ReqTransactionUuid,
                ReqCurrency = ReqCurrency,
                ReqReferenceNumber = ReqReferenceNumber,
                ReqAmount = double.Parse(ReqAmount),
                AuthAvsCode = AuthAvsCode,
                AuthCode = AuthCode,
                AuthAmount = AuthAmount,
                AuthTime = AuthTime,
                AuthResponse = AuthResponse,
                AuthTransRefNo = AuthTransRefNo,
                PaymentToken = PaymentToken,
                DateTime = DateTime,
                Platform = (PaymentPlatformDto)Enum.Parse(typeof(PaymentPlatformDto), Platform),
                OperationId = OperationId,
                ServiceId = Guid.Parse(ServiceId),
                CardId = Guid.Parse(CardId),
                IsUserRegistered = IsUserRegistered
            };
        }
    }
}