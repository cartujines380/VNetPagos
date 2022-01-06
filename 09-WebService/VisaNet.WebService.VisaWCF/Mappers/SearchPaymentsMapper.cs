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
    public static class SearchPaymentsMapper
    {
        public static WsBankSearchPaymentsInputDto ToSearchPaymentsDto(this SearchPaymentsData data)
        {
            return new WsBankSearchPaymentsInputDto
            {
                TransactionId = data.TransactionId,
                ServiceId = data.ServiceId,
                ToDate = data.ToDate,
                FromDate = data.FromDate
            };
        }

        public static VisaNetPayment ToVisaNetSearchPaymentsResult(this PaymentDto paymentDto)
        {
            return new VisaNetPayment
            {
                Bill = paymentDto.Bills.Select(b => new VisaNetBillResponse
                {
                    BillId = b.Id.ToString(),
                    BillNumber = b.BillExternalId,
                    Currency = b.Currency,
                    Description = b.Description,
                    ExpirationDate = b.ExpirationDate.ToString("dd/MM/yyyy"),
                    FinalConsumer = b.FinalConsumer,
                    Gateway = paymentDto.GatewayDto.Name,
                    TotalAmount = b.Amount,
                    Payable = b.Payable,
                    GatewayTransactionId = b.GatewayTransactionId,
                    Lines = b.Line,
                    CensusId = b.IdPadron.ToString(),
                    //PreBillNumber = b.SucivePreBillNumber,
                    ServiceId = paymentDto.ServiceId.ToString(),
                    ServiceReferenceNumber = paymentDto.ReferenceNumber,
                    ServiceReferenceNumber2 = paymentDto.ReferenceNumber2,
                    ServiceReferenceNumber3 = paymentDto.ReferenceNumber3,
                    ServiceReferenceNumber4 = paymentDto.ReferenceNumber4,
                    ServiceReferenceNumber5 = paymentDto.ReferenceNumber5,
                    ServiceReferenceNumber6 = paymentDto.ReferenceNumber6,
                    CardBinNumbers = Int32.Parse(paymentDto.Card.MaskedNumber.Substring(0, 6)),
                    MerchantId = paymentDto.ServiceDto.MerchantId,
                    //MerchantReferenceCode = TODO (yani)
                    ServiceType = paymentDto.ServiceDto.ServiceCategory.Name,
                    MultipleBillsAllowed = paymentDto.ServiceDto.EnableMultipleBills,
                    CreationDate = b.DateInitTransaccion
                }).FirstOrDefault(),
                DateTime = paymentDto.Date,
                GatewayTransactionId = paymentDto.Bills.FirstOrDefault().GatewayTransactionId,
                TransactionId = paymentDto.TransactionNumber,
                UserInfo = new UserData
                {
                    Address = paymentDto.AnonymousUser.Address,
                    Ci = paymentDto.AnonymousUser.IdentityNumber,
                    Email = paymentDto.AnonymousUser.Email,
                    Name = paymentDto.AnonymousUser.Name,
                    Surname = paymentDto.AnonymousUser.Surname
                }
            };
        }

        public static String[] ToParamsArray(this SearchPaymentsData data)
        {
            var paramsList = new List<String> { data.PaymentPlatform };

            if (data.TransactionId != null) paramsList.Add(data.TransactionId);
            if (data.ServiceId != null) paramsList.Add(data.ServiceId);
            if (data.FromDate != null) paramsList.Add(DateTime.ParseExact(data.FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyyMMdd"));
            if (data.ToDate != null) paramsList.Add(DateTime.ParseExact(data.ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyyMMdd"));

            return paramsList.ToArray();
        }
    }
}