using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using VisaNet.Common.Logging.Entities;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Utilities.Exportation.ExtensionMethods;
using VisaNet.WebService.VisaWCF.EntitiesDto;

namespace VisaNet.WebService.VisaWCF.Mappers
{
    public static class PaymentMapper
    {
        public static WsBankPaymentInputDto ToPaymentDto(this PaymentData data)
        {
            return new WsBankPaymentInputDto
            {
                BillId = data.Bill.BillId,
                BillNumber = data.Bill.BillNumber,
                ExpirationDate = data.Bill.ExpirationDate,
                Currency = data.Bill.Currency,
                Description = data.Bill.Description,
                GatewayTransactionId = data.Bill.GatewayTransactionId,
                //GatewayTransactionBrouId = data.Bill.GatewayTransactionBrouId,
                Payable = data.Bill.Payable,
                FinalConsumer = data.Bill.FinalConsumer,
                DiscountApplyed = data.Bill.DiscountApplyed,
                TotalTaxedAmount = data.Bill.TotalTaxedAmount,
                TotalAmount = data.Bill.TotalAmount,
                Discount = data.Bill.Discount,
                AmountToCybersource = data.Bill.AmountToCybersource,
                Gateway = data.Bill.Gateway,
                MerchantReferenceCode = data.Bill.MerchantReferenceCode,
                AuthAmount = data.CyberSourceData.AuthAmount,
                AuthTime = data.CyberSourceData.AuthTime,
                AuthCode = data.CyberSourceData.AuthCode,
                AuthAvsCode = data.CyberSourceData.AuthAvsCode,
                AuthResponse = data.CyberSourceData.AuthResponse,
                AuthTransRefNo = data.CyberSourceData.AuthTransRefNo,
                Decision = data.CyberSourceData.Decision,
                BillTransRefNo = data.CyberSourceData.BillTransRefNo,
                PaymentToken = data.CyberSourceData.PaymentToken,
                ReasonCode = data.CyberSourceData.ReasonCode,
                ReqAmount = !String.IsNullOrEmpty(data.CyberSourceData.ReqAmount) ? data.CyberSourceData.ReqAmount : "0",
                ReqCurrency = data.CyberSourceData.ReqCurrency,
                TransactionId = data.CyberSourceData.TransactionId,
                ReqTransactionUuid = data.CyberSourceData.ReqTransactionUuid,
                ReqReferenceNumber = data.CyberSourceData.ReqReferenceNumber,
                ReqTransactionType = data.CyberSourceData.ReqTransactionType,
                Email = data.UserInfo.Email,
                Ci = data.UserInfo.Ci,
                Address = data.UserInfo.Address,
                Name = data.UserInfo.Name,
                Surname = data.UserInfo.Surname,
                CardBinNumbers = data.CardData.CardBinNumbers,
                CardMaskedNumber = data.CardData.MaskedNumber,
                CardDueDate = data.CardData.DueDate,
                CardName = data.CardData.Name,
                CardPhone = data.CardData.Phone,
                ServiceId = data.ServiceId,
                PaymentPlatform = data.PaymentPlatform,
                ServiceReferenceNumber = data.Bill.ServiceReferenceNumber,
                ServiceReferenceNumber2 = data.Bill.ServiceReferenceNumber2,
                ServiceReferenceNumber3 = data.Bill.ServiceReferenceNumber3,
                ServiceReferenceNumber4 = data.Bill.ServiceReferenceNumber4,
                ServiceReferenceNumber5 = data.Bill.ServiceReferenceNumber5,
                ServiceReferenceNumber6 = data.Bill.ServiceReferenceNumber6,
                SucivePreBillNumber = (GatewayEnumDto)Enum.Parse(typeof(GatewayEnumDto), data.Bill.Gateway, true) == GatewayEnumDto.Geocom ||
                                      (GatewayEnumDto)Enum.Parse(typeof(GatewayEnumDto), data.Bill.Gateway, true) == GatewayEnumDto.Sucive ? data.Bill.BillNumber : default(string),
                TransactionCreationDate = data.Bill.CreationDate,
            };
        }

        public static VisaNetPayment ToVisaNetPaymentResult(this WsBankPaymentResultDto payment, PaymentData data)
        {
            return new VisaNetPayment()
            {
                Bill = data.Bill,
                DateTime = payment.Payment.Date,
                GatewayTransactionId = data.Bill.GatewayTransactionId,
                TransactionId = payment.Payment.TransactionNumber,
                UserInfo = data.UserInfo
            };
        }

        public static WsBankReverseInputDto ToReverseDto(this PaymentData data)
        {
            return new WsBankReverseInputDto
            {
                Name = data.UserInfo != null ? data.UserInfo.Name : "",
                ServiceId = data.ServiceId,
                Token = data.CyberSourceData.PaymentToken,
                RequestId = data.CyberSourceData.TransactionId,
                Amount = data.Bill.AmountToCybersource.SignificantDigits(2).ToString(CultureInfo.CreateSpecificCulture("en-US")),
                IdTransaccion = data.CyberSourceData.TransactionId,
                Currency = data.Bill.Currency,
                UserType = LogUserType.NoRegistered,
                PaymentPlatform = PaymentPlatformDto.VisaNet
            };
        }

        public static CyberSourceData ToVisaNetCyberSourceData(this CyberSourceOperationData data)
        {
            return data != null ? new CyberSourceData
            {
                PaymentData = data.PaymentData != null ? new CsResponse
                {
                    PaymentRequestId = data.PaymentData.PaymentRequestId,
                    PaymentResponseCode = data.PaymentData.PaymentResponseCode.ToString()
                } : null,
                RefundData = data.RefundData != null ? new CsResponse
                {
                    PaymentRequestId = data.RefundData.PaymentRequestId,
                    PaymentResponseCode = data.RefundData.PaymentResponseCode.ToString()
                } : null,
                ReversalData = data.ReversalData != null ? new CsResponse
                {
                    PaymentRequestId = data.ReversalData.PaymentRequestId,
                    PaymentResponseCode = data.ReversalData.PaymentResponseCode.ToString()
                } : null,
                VoidData = data.VoidData != null ? new CsResponse
                {
                    PaymentRequestId = data.VoidData.PaymentRequestId,
                    PaymentResponseCode = data.VoidData.PaymentResponseCode.ToString()
                } : null
            } : null;
        }

        public static String[] ToParamsArray(this PaymentData data)
        {
            var paramsList = new List<String> { data.PaymentPlatform };

            if (data.Bill != null)
            {
                paramsList.Add(String.Format(CultureInfo.GetCultureInfo("en-US"), "{0:0.00}", data.Bill.AmountToCybersource));
                if (data.Bill.BillId != null) paramsList.Add(data.Bill.BillId);
                if (data.Bill.BillNumber != null) paramsList.Add(data.Bill.BillNumber);
                if (data.Bill.Currency != null) paramsList.Add(data.Bill.Currency);
                if (data.Bill.Description != null) paramsList.Add(data.Bill.Description);
                paramsList.Add(String.Format(CultureInfo.GetCultureInfo("en-US"), "{0:0.00}", data.Bill.Discount));
                paramsList.Add(data.Bill.DiscountApplyed.ToString().ToLower());
                if (data.Bill.ExpirationDate != null) paramsList.Add(DateTime.ParseExact(data.Bill.ExpirationDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyyMMdd"));
                paramsList.Add(data.Bill.FinalConsumer.ToString().ToLower());
                if (data.Bill.Gateway != null) paramsList.Add(data.Bill.Gateway);
                if (data.Bill.GatewayTransactionId != null) paramsList.Add(data.Bill.GatewayTransactionId);
                if (data.Bill.CensusId != null) paramsList.Add(data.Bill.CensusId);
                if (data.Bill.Lines != null) paramsList.Add(data.Bill.Lines);
                if (data.Bill.ServiceId != null) paramsList.Add(data.Bill.ServiceId);
                paramsList.Add(data.Bill.Payable.ToString().ToLower());
                paramsList.Add(String.Format(CultureInfo.GetCultureInfo("en-US"), "{0:0.00}", data.Bill.TotalAmount));
                paramsList.Add(String.Format(CultureInfo.GetCultureInfo("en-US"), "{0:0.00}", data.Bill.TotalTaxedAmount));
                paramsList.Add(data.Bill.CardBinNumbers.ToString());
                //if (data.Bill.PreBillNumber != null) paramsList.Add(data.Bill.PreBillNumber);
                if (data.Bill.ServiceReferenceNumber != null) paramsList.Add(data.Bill.ServiceReferenceNumber);
                if (data.Bill.ServiceReferenceNumber2 != null) paramsList.Add(data.Bill.ServiceReferenceNumber2);
                if (data.Bill.ServiceReferenceNumber3 != null) paramsList.Add(data.Bill.ServiceReferenceNumber3);
                if (data.Bill.ServiceReferenceNumber4 != null) paramsList.Add(data.Bill.ServiceReferenceNumber4);
                if (data.Bill.ServiceReferenceNumber5 != null) paramsList.Add(data.Bill.ServiceReferenceNumber5);
                if (data.Bill.ServiceReferenceNumber6 != null) paramsList.Add(data.Bill.ServiceReferenceNumber6);
                if (data.Bill.MerchantReferenceCode != null) paramsList.Add(data.Bill.MerchantReferenceCode);
                if (data.Bill.MerchantId != null) paramsList.Add(data.Bill.MerchantId);
                if (data.Bill.ServiceType != null) paramsList.Add(data.Bill.ServiceType);
                paramsList.Add(data.Bill.MultipleBillsAllowed.ToString().ToLower());
                if (data.Bill.CreationDate != null) paramsList.Add(data.Bill.CreationDate);
                if (!string.IsNullOrEmpty(data.Bill.DiscountObjId)) paramsList.Add(data.Bill.DiscountObjId);
            }

            if (data.CardData != null)
            {
                paramsList.Add(data.CardData.CardBinNumbers.ToString());
                if (data.CardData.DueDate != null) paramsList.Add(data.CardData.DueDate);
                if (data.CardData.MaskedNumber != null) paramsList.Add(data.CardData.MaskedNumber);
                if (data.CardData.Name != null) paramsList.Add(data.CardData.Name);
            }

            if (data.CyberSourceData != null)
            {
                if (data.CyberSourceData.TransactionId != null) paramsList.Add(data.CyberSourceData.TransactionId);
                if (data.CyberSourceData.PaymentToken != null) paramsList.Add(data.CyberSourceData.PaymentToken);
                if (data.CyberSourceData.ReasonCode != null) paramsList.Add(data.CyberSourceData.ReasonCode);
            }

            paramsList.Add(data.ServiceId);

            if (data.UserInfo != null)
            {
                //if (data.UserInfo.Ci != null) paramsList.Add(data.UserInfo.Ci);
                if (data.UserInfo.Email != null) paramsList.Add(data.UserInfo.Email);
                //if (data.UserInfo.Name != null) paramsList.Add(data.UserInfo.Name);
                //if (data.UserInfo.Surname != null) paramsList.Add(data.UserInfo.Surname);
                //if (data.UserInfo.Address != null) paramsList.Add(data.UserInfo.Address);
            }

            

            return paramsList.ToArray();
        }
    }
}