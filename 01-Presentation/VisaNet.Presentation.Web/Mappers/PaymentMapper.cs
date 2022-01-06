using System.Configuration;
using System.Linq;
using VisaNet.Common.AzureUpload;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Presentation.Web.Models;
using VisaNet.Utilities.Cybersource;

namespace VisaNet.Presentation.Web.Mappers
{
    public static class PaymentMapper
    {
        private static readonly string FolderBlob = ConfigurationManager.AppSettings["AzureServicesImagesUrl"];

        public static PaymentDto ToDto(this PaymentModel entity)
        {
            return new PaymentDto
            {
                Id = entity.Id,
                Date = entity.Date,
                CardId = entity.CardId,
                Card = entity.Card,
                ServiceAssociatedId = entity.ServicesAssosiatedId,
                ReferenceNumber = entity.ReferenceValue,
                ReferenceNumber2 = entity.ReferenceValue2,
                ReferenceNumber3 = entity.ReferenceValue3,
                ReferenceNumber4 = entity.ReferenceValue4,
                ReferenceNumber5 = entity.ReferenceValue5,
                ReferenceNumber6 = entity.GatewayEnum == GatewayEnumDto.Sucive || entity.GatewayEnum == GatewayEnumDto.Geocom ? entity.IdPadron.ToString() : entity.ReferenceValue6,
                Description = entity.Description,
                Bills = entity.Bills,
                RegisteredUserId = entity.RegisteredUserId,
                RegisteredUser = entity.RegisteredUser,
                AnonymousUserId = entity.AnonymousUserId,
                AnonymousUser = entity.AnonymousUser,
                PaymentType = entity.PaymentType,
                TransactionNumber = entity.TransactionNumber,
                ServiceId = entity.ServiceId,
                GatewayEnum = entity.GatewayEnum,
                Currency = entity.Bills.First().Currency,
                Discount = entity.Discount,
                DiscountApplyed = entity.DiscountApplyed,
                TotalAmount = entity.TotalAmount,
                TotalTaxedAmount = entity.TotalTaxedAmount,
                AmountTocybersource = entity.AmountTocybersource,
                DiscountObj = entity.DiscountObj,
                DiscountObjId = entity.DiscountObjId,
            };
        }

        public static PaymentModel ToModel(this PaymentDto entity)
        {
            return new PaymentModel
            {
                Id = entity.Id,
                Date = entity.Date,
                CardId = entity.CardId,
                Card = entity.Card,
                ServicesAssosiatedId = entity.ServiceAssociatedId,
                ServiceId = entity.ServiceId,
                ServiceName = entity.ServiceDto.Name,
                ServiceContainerName = entity.ServiceDto.ServiceContainerName,
                ServiceDesc = entity.ServiceAssociatedDto.Description,
                Service = entity.ServiceDto,
                ReferenceName = entity.ServiceDto.ReferenceParamName,
                ReferenceName2 = entity.ServiceDto.ReferenceParamName2,
                ReferenceName3 = entity.ServiceDto.ReferenceParamName3,
                ReferenceName4 = entity.ServiceDto.ReferenceParamName4,
                ReferenceName5 = entity.ServiceDto.ReferenceParamName5,
                ReferenceName6 = entity.ServiceDto.ReferenceParamName6,
                ReferenceValue = entity.ReferenceNumber,
                ReferenceValue2 = entity.ReferenceNumber2,
                ReferenceValue3 = entity.ReferenceNumber3,
                ReferenceValue4 = entity.ReferenceNumber4,
                ReferenceValue5 = entity.ReferenceNumber5,
                ReferenceValue6 = entity.ReferenceNumber6,
                Description = entity.Description,
                Bills = entity.Bills,
                RegisteredUserId = entity.RegisteredUserId,
                RegisteredUser = entity.RegisteredUser,
                AnonymousUserId = entity.AnonymousUserId,
                AnonymousUser = entity.AnonymousUser,
                PaymentType = entity.PaymentType,
                TransactionNumber = entity.TransactionNumber,
                GatewayEnum = entity.GatewayEnum,
                Currency = entity.Currency,
                Discount = entity.Discount,
                DiscountApplyed = entity.DiscountApplyed,
                TotalAmount = entity.TotalAmount,
                TotalTaxedAmount = entity.TotalTaxedAmount,
                AmountDolars = entity.Bills != null ? entity.Bills.Where(b => b.Currency.Equals(Currency.DOLAR_AMERICANO)).Sum(b => b.Amount) : 0,
                AmountPesos = entity.Bills != null ? entity.Bills.Where(b => b.Currency.Equals(Currency.PESO_URUGUAYO)).Sum(b => b.Amount) : 0,
                //AmountDolars = entity.Currency.Equals(Currency.DOLAR_AMERICANO) ? entity.TotalAmount : 0 ,
                //AmountPesos = entity.Currency.Equals(Currency.PESO_URUGUAYO) ? entity.TotalAmount : 0,
                AmountTocybersource = entity.AmountTocybersource,
                DiscountObj = entity.DiscountObj,
                DiscountObjId = entity.DiscountObjId,
                Quota = entity.Quotas,
                ServiceImageName = !string.IsNullOrEmpty(entity.ServiceDto.ImageUrl)
                    ? entity.ServiceDto.ImageUrl
                    : !string.IsNullOrEmpty(entity.ServiceDto.ServiceContainerImageUrl)
                    ? entity.ServiceDto.ServiceContainerImageUrl
                    : string.Empty,
            };
        }

    }
}