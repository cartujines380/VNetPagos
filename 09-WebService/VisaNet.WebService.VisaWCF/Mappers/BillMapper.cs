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
    public static class BillMapper
    {
        public static WsBankBillsInputDto ToBillDto(this BillsData data)
        {
            var input = new WsBankBillsInputDto()
            {
                //CardBinNumbers = data.CardBinNumbers,
                ServiceId = data.ServiceId,
                ServiceReferenceNumber = data.ServiceReferenceNumber,
                ServiceReferenceNumber2 = data.ServiceReferenceNumber2,
                ServiceReferenceNumber3 = data.ServiceReferenceNumber3,
                ServiceReferenceNumber4 = data.ServiceReferenceNumber4,
                ServiceReferenceNumber5 = data.ServiceReferenceNumber5,
                ServiceReferenceNumber6 = data.ServiceReferenceNumber6,
            };
            if (!string.IsNullOrEmpty(data.GatewayEnumDto))
            {
                input.GatewayEnumDto = (GatewayEnumDto)Enum.Parse(typeof(GatewayEnumDto), data.GatewayEnumDto, true);
            }
            if (data.UserData != null)
            {
                input.UserEmail = data.UserData.Email;
                input.UserAddress = data.UserData.Address;
                input.UserCi = data.UserData.Ci;
                input.UserName = data.UserData.Name;
                input.UserSurname = data.UserData.Surname;
            }
            return input;
        }

        public static VisaNetBillResponse ToVisaNetBillResult(this BillDto x, BillsData data, WsBankBillsResultDto result, string merchantReferenceCode)
        {
            var a = new VisaNetBillResponse()
            {
                BillId = Guid.NewGuid().ToString(),
                BillNumber = x.BillExternalId,
                Currency = x.Currency,
                Description = x.Description,
                ExpirationDate = x.ExpirationDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                FinalConsumer = x.FinalConsumer,
                Gateway = x.Gateway.ToString(),
                TotalAmount = x.Amount,
                TotalTaxedAmount = x.TaxedAmount,
                Payable = x.Payable || x.ItauPayable,
                GatewayTransactionId = x.GatewayTransactionId,
                Lines = x.Line,
                CensusId = x.IdPadron.ToString(),
                //PreBillNumber = x.SucivePreBillNumber,
                ServiceId = data.ServiceId,
                MerchantId = result.MerchantId,
                ServiceReferenceNumber = data.ServiceReferenceNumber,
                ServiceReferenceNumber2 = data.ServiceReferenceNumber2,
                ServiceReferenceNumber3 = data.ServiceReferenceNumber3,
                ServiceReferenceNumber4 = data.ServiceReferenceNumber4,
                ServiceReferenceNumber5 = data.ServiceReferenceNumber5,
                ServiceReferenceNumber6 = data.ServiceReferenceNumber6,
                //CardBinNumbers = data.CardBinNumbers,
                MerchantReferenceCode = merchantReferenceCode,
                ServiceType = result.ServiceType,
                MultipleBillsAllowed = result.MultipleBillsAllowed,
                CreationDate = x.DateInitTransaccion
            };
            return a;
        }

        public static String[] ToParamsArray(this BillsData data)
        {
            var paramsList = new List<String> { data.PaymentPlatform, data.ServiceId };

            if (data.ServiceReferenceNumber != null) paramsList.Add(data.ServiceReferenceNumber);
            if (data.ServiceReferenceNumber2 != null) paramsList.Add(data.ServiceReferenceNumber2);
            if (data.ServiceReferenceNumber3 != null) paramsList.Add(data.ServiceReferenceNumber3);
            if (data.ServiceReferenceNumber4 != null) paramsList.Add(data.ServiceReferenceNumber4);
            if (data.ServiceReferenceNumber5 != null) paramsList.Add(data.ServiceReferenceNumber5);
            if (data.ServiceReferenceNumber6 != null) paramsList.Add(data.ServiceReferenceNumber6);
            if (data.GatewayEnumDto != null) paramsList.Add(data.GatewayEnumDto);

            if (data.UserData != null)
            {
                if (!string.IsNullOrEmpty(data.UserData.Ci)) paramsList.Add(data.UserData.Ci);
                if (!string.IsNullOrEmpty(data.UserData.Email)) paramsList.Add(data.UserData.Email);
                if (!string.IsNullOrEmpty(data.UserData.Name)) paramsList.Add(data.UserData.Name);
                if (!string.IsNullOrEmpty(data.UserData.Surname)) paramsList.Add(data.UserData.Surname);
                if (!string.IsNullOrEmpty(data.UserData.Address)) paramsList.Add(data.UserData.Address);
            }

            return paramsList.ToArray();
        }
    }
}