﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VisaNet.Domain.Entities.Base.Base;
using VisaNet.Domain.Entities.Enums;

namespace VisaNet.Domain.Entities
{
    [Table("CyberSourceAcknowledgements")]
    public class CyberSourceAcknowledgement : EntityBase
    {
        [Key]
        public override Guid Id { get; set; }
        public int ReasonCode { get; set; }
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
        public double ReqAmount { get; set; }
        public string AuthAvsCode { get; set; }
        public string AuthCode { get; set; }
        public string AuthAmount { get; set; }
        public string AuthTime { get; set; }
        public string AuthResponse { get; set; }
        public string AuthTransRefNo { get; set; }
        public string PaymentToken { get; set; }
        public DateTime DateTime { get; set; }
        public PaymentPlatform Platform { get; set; }
        public Guid ServiceId { get; set; }
        public string OperationId { get; set; }
        public Guid CardId { get; set; }
        public bool Processed { get; set; }
    }
}