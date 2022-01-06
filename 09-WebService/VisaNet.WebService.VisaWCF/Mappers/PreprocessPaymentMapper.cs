using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.WebService.VisaWCF.EntitiesDto;

namespace VisaNet.WebService.VisaWCF.Mappers
{
    public static class PreprocessPaymentMapper
    {
        public static WsBankPreprocessPaymentInputDto ToPreprocessPaymentDto(this PreprocessPaymentData data)
        {
            return new WsBankPreprocessPaymentInputDto
            {
                ServiceId = data.Bills.FirstOrDefault().ServiceId,
                ServiceType = data.Bills.FirstOrDefault().ServiceType,
                MultipleBillsAllowed = data.Bills.FirstOrDefault().MultipleBillsAllowed,
                ServiceReferenceNumber = data.Bills.FirstOrDefault().ServiceReferenceNumber,
                ServiceReferenceNumber2 = data.Bills.FirstOrDefault().ServiceReferenceNumber2,
                ServiceReferenceNumber3 = data.Bills.FirstOrDefault().ServiceReferenceNumber3,
                ServiceReferenceNumber4 = data.Bills.FirstOrDefault().ServiceReferenceNumber4,
                ServiceReferenceNumber5 = data.Bills.FirstOrDefault().ServiceReferenceNumber5,
                ServiceReferenceNumber6 = data.Bills.FirstOrDefault().ServiceReferenceNumber6,
                CardBinNumbers = data.Bills.FirstOrDefault().CardBinNumbers,
                MerchantReferenceCode = data.Bills.FirstOrDefault().MerchantReferenceCode,
                Bills = data.Bills.Select(b => new BillDto
                {
                    Id = Guid.Parse(b.BillId),
                    BillExternalId = b.BillNumber,
                    ExpirationDate = DateTime.ParseExact(b.ExpirationDate, "dd/MM/yyyy", CultureInfo.InvariantCulture),
                    Currency = b.Currency,
                    Description = b.Description,
                    GatewayTransactionId = b.GatewayTransactionId,
                    Payable = b.Payable,
                    FinalConsumer = b.FinalConsumer,
                    Amount = b.TotalAmount,
                    TaxedAmount = b.TotalTaxedAmount,
                    Gateway = (GatewayEnumDto)Enum.Parse(typeof(GatewayEnumDto), b.Gateway, true),
                    Line = b.Lines,
                    IdPadron = !string.IsNullOrEmpty(b.CensusId) ? Int32.Parse(b.CensusId) : default(int),
                    //SucivePreBillNumber = b.PreBillNumber,
                    DateInitTransaccion = b.CreationDate
                }).ToList()
            };
        }

        public static VisaNetBillResponse ToVisaNetPreprocessPaymentResult(this BillDto b, PreprocessPaymentData data, CyberSourceExtraDataDto cybersourceExtraData, WsBankBillsResultDto payment)
        {
            return new VisaNetBillResponse
            {
                BillId = b.Id.ToString(),
                BillNumber = !String.IsNullOrEmpty(b.BillExternalId) ? b.BillExternalId : b.SucivePreBillNumber,
                ExpirationDate = b.ExpirationDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                Currency = b.Currency,
                Description = b.Description,
                GatewayTransactionId = b.GatewayTransactionId,
                Payable = b.Payable,
                FinalConsumer = b.FinalConsumer,
                TotalAmount = b.Amount,
                Gateway = b.Gateway.ToString(),
                Lines = b.Line,
                CensusId = b.IdPadron.ToString(),
                ServiceId = data.Bills.FirstOrDefault().ServiceId,
                ServiceReferenceNumber = data.Bills.FirstOrDefault().ServiceReferenceNumber,
                ServiceReferenceNumber2 = data.Bills.FirstOrDefault().ServiceReferenceNumber2,
                ServiceReferenceNumber3 = data.Bills.FirstOrDefault().ServiceReferenceNumber3,
                ServiceReferenceNumber4 = data.Bills.FirstOrDefault().ServiceReferenceNumber4,
                ServiceReferenceNumber5 = data.Bills.FirstOrDefault().ServiceReferenceNumber5,
                ServiceReferenceNumber6 = data.Bills.FirstOrDefault().ServiceReferenceNumber6,
                CardBinNumbers = data.Bills.FirstOrDefault().CardBinNumbers,
                //PreBillNumber = b.SucivePreBillNumber,
                Discount = cybersourceExtraData.BillDto.DiscountAmount,
                DiscountApplyed = cybersourceExtraData.BillDto.DiscountAmount > 0,
                //TotalAmount = cybersourceExtraData.TotalAmount,
                TotalTaxedAmount = cybersourceExtraData.BillDto.TaxedAmount,
                AmountToCybersource = cybersourceExtraData.CybersourceAmount,
                MerchantId = payment.MerchantId,
                MerchantReferenceCode = data.Bills.FirstOrDefault().MerchantReferenceCode,
                ServiceType = data.Bills.FirstOrDefault().ServiceType,
                MultipleBillsAllowed = data.Bills.FirstOrDefault().MultipleBillsAllowed,
                CreationDate = b.DateInitTransaccion,
                DiscountObjId = cybersourceExtraData.DiscountDto != null ? cybersourceExtraData.DiscountDto.Id.ToString() : Guid.Empty.ToString()
            };
        }

        public static String[] ToParamsArray(this PreprocessPaymentData data)
        {
            var paramsList = new List<String> { data.PaymentPlatform };

            foreach (var bill in data.Bills)
            {
                paramsList.Add(String.Format(CultureInfo.GetCultureInfo("en-US"), "{0:0.00}", bill.AmountToCybersource));
                if (bill.BillId != null) paramsList.Add(bill.BillId);
                if (bill.BillNumber != null) paramsList.Add(bill.BillNumber);
                if (bill.Currency != null) paramsList.Add(bill.Currency);
                if (bill.Description != null) paramsList.Add(bill.Description);
                paramsList.Add(String.Format(CultureInfo.GetCultureInfo("en-US"), "{0:0.00}", bill.Discount));
                paramsList.Add(bill.DiscountApplyed.ToString().ToLower());
                if (bill.ExpirationDate != null) paramsList.Add(DateTime.ParseExact(bill.ExpirationDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyyMMdd"));
                paramsList.Add(bill.FinalConsumer.ToString().ToLower());
                if (bill.Gateway != null) paramsList.Add(bill.Gateway);
                if (bill.GatewayTransactionId != null) paramsList.Add(bill.GatewayTransactionId);
                if (bill.CensusId != null) paramsList.Add(bill.CensusId);
                if (bill.Lines != null) paramsList.Add(bill.Lines);
                if (bill.ServiceId != null) paramsList.Add(bill.ServiceId);
                paramsList.Add(bill.Payable.ToString().ToLower());
                paramsList.Add(String.Format(CultureInfo.GetCultureInfo("en-US"), "{0:0.00}", bill.TotalAmount));
                paramsList.Add(String.Format(CultureInfo.GetCultureInfo("en-US"), "{0:0.00}", bill.TotalTaxedAmount));
                paramsList.Add(bill.CardBinNumbers.ToString());
                //if (bill.PreBillNumber != null) paramsList.Add(bill.PreBillNumber);
                if (bill.ServiceReferenceNumber != null) paramsList.Add(bill.ServiceReferenceNumber);
                if (bill.ServiceReferenceNumber2 != null) paramsList.Add(bill.ServiceReferenceNumber2);
                if (bill.ServiceReferenceNumber3 != null) paramsList.Add(bill.ServiceReferenceNumber3);
                if (bill.ServiceReferenceNumber4 != null) paramsList.Add(bill.ServiceReferenceNumber4);
                if (bill.ServiceReferenceNumber5 != null) paramsList.Add(bill.ServiceReferenceNumber5);
                if (bill.ServiceReferenceNumber6 != null) paramsList.Add(bill.ServiceReferenceNumber6);
                if (bill.MerchantReferenceCode != null) paramsList.Add(bill.MerchantReferenceCode);
                if (bill.MerchantId != null) paramsList.Add(bill.MerchantId);
                if (bill.ServiceType != null) paramsList.Add(bill.ServiceType);
                paramsList.Add(bill.MultipleBillsAllowed.ToString().ToLower());
                if (bill.CreationDate != null) paramsList.Add(bill.CreationDate);
            }

            return paramsList.ToArray();
        }
    }
}