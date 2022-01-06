using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.WebPages;
using VisaNet.Common.Resource.EntitiesDto;
using VisaNet.Common.Resource.Enums;
using VisaNet.Common.Resource.Helpers;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Presentation.Administration.Models;

namespace VisaNet.Presentation.Administration.Mappers
{
    public static class PaymentMapper
    {
        public static PaymentModel ToModel(this PaymentDto entity, IEnumerable<BinDto> bins)
        {
            var binValue = Int32.Parse(entity.Card.MaskedNumber.Substring(0, 6));
            var bin = bins.FirstOrDefault(b => b.Value == binValue);

            var bill = entity.Bills != null ? entity.Bills.FirstOrDefault() : null;

            var referenceNumber = entity.ReferenceNumber;
            if (!String.IsNullOrEmpty(entity.ReferenceNumber2))
                referenceNumber += " - " + entity.ReferenceNumber2;
            if (!String.IsNullOrEmpty(entity.ReferenceNumber3))
                referenceNumber += " - " + entity.ReferenceNumber3;
            if (!String.IsNullOrEmpty(entity.ReferenceNumber4))
                referenceNumber += " - " + entity.ReferenceNumber4;
            if (!String.IsNullOrEmpty(entity.ReferenceNumber5))
                referenceNumber += " - " + entity.ReferenceNumber5;
            if (!String.IsNullOrEmpty(entity.ReferenceNumber6))
                referenceNumber += " - " + entity.ReferenceNumber6;

            return new PaymentModel
            {
                Date = entity.Date.ToString("dd/MM/yyyy HH:mm:ss"),
                ClientEmail = entity.AnonymousUserId != null ? entity.AnonymousUser.Email : entity.RegisteredUser.Email,
                ClientName = entity.AnonymousUserId != null ? entity.AnonymousUser.Name : entity.RegisteredUser.Name,
                ClientSurname = entity.AnonymousUserId != null ? entity.AnonymousUser.Surname : entity.RegisteredUser.Surname,
                ClientAddress = entity.AnonymousUserId != null ? entity.AnonymousUser.Address : entity.RegisteredUser.Address,
                Gateway = entity.GatewayDto.Name,
                UniqueIdentifier = entity.PaymentIdentifierDto.UniqueIdentifier.ToString(),
                TransactionNumber = entity.TransactionNumber,
                PaymentType = EnumHelpers.GetName(typeof(PaymentTypeDto), (int)entity.PaymentType, EnumsStrings.ResourceManager),
                ServiceName = entity.ServiceDto.Name,
                ServiceCategoryName = entity.ServiceDto.ServiceCategory.Name,
                ReferenceNumber = referenceNumber,
                Description = entity.Description,
                CardMaskedNumber = entity.Card.MaskedNumber,
                CardDueDate = entity.Card.DueDate.ToShortDateString(),
                CardBin = bin != null ? bin.Name : "",
                CardType = bin != null ? EnumHelpers.GetName(typeof(CardTypeDto), (int)bin.CardType, EnumsStrings.ResourceManager) : "",
                BillExternalId = bill != null ? bill.BillExternalId : "",
                BillExpirationDate = bill != null ? bill.ExpirationDate.ToShortDateString() : "",
                BillFinalConsumer = bill != null ? bill.FinalConsumer ? PresentationCoreMessages.Common_Yes : PresentationCoreMessages.Common_No : "",
                BillCurrency = bill != null ? bill.Currency : "",
                BillAmount = bill != null ? bill.Amount.ToString("##,#0.00", CultureInfo.CurrentCulture) : "",
                BillTaxedAmount = bill != null ? bill.TaxedAmount.ToString("##,#0.00", CultureInfo.CurrentCulture) : "",
                BillDiscountApplied = entity.DiscountApplyed ? PresentationCoreMessages.Common_Yes : PresentationCoreMessages.Common_No,
                BillDiscountAmount = entity.Discount > 0 ? entity.Discount.ToString("##,#0.00", CultureInfo.CurrentCulture) : "0,00",
                Id = entity.Id,
                PaymentStatus = entity.PaymentStatus.ToString(),
                PaymentStatusVal = (int)entity.PaymentStatus,
            };
        }

        public static PaymentModel ToModel(this PaymentBillDto entity, IEnumerable<BinDto> bins)
        {
            var binValue = Int32.Parse(entity.CardMaskedNumber.Substring(0, 6));
            var bin = bins.FirstOrDefault(b => b.Value == binValue);

            var platform = EnumHelpers.GetName(typeof (PaymentPlatformDto), entity.PaymentPlatform,
                EnumsStrings.ResourceManager);
            var paymentType = EnumHelpers.GetName(typeof (PaymentTypeDto), entity.PaymentType,
                EnumsStrings.ResourceManager);

            var pOrigin = platform + " - " + paymentType;
            
            return new PaymentModel
            {
                Date = entity.PaymentDate.ToString("dd/MM/yyyy HH:mm:ss"),
                ClientEmail = entity.UserEmail,
                ClientName = entity.UserName,
                ClientSurname = entity.UserSurname,
                Gateway = entity.GatewayName,
                UniqueIdentifier = entity.PaymentUniqueIdentifier.ToString(),
                TransactionNumber = entity.TransactionNumber,
                PaymentType = pOrigin,
                ServiceName = entity.ServiceName,
                ServiceCategoryName = entity.ServiceCategoryName,
                CardMaskedNumber = entity.CardMaskedNumber,
                CardDueDate = entity.CardDueDate.ToShortDateString(),
                CardBin = bin != null ? bin.Name : "",
                CardType = bin != null ? EnumHelpers.GetName(typeof(CardTypeDto), (int)bin.CardType, EnumsStrings.ResourceManager) : "",
                BillCurrency = entity.BillCurrency,
                BillAmount = entity.BillAmount.ToString(),
                BillTaxedAmount = entity.BillTaxedAmount.ToString(),
                BillDiscountAmount = entity.BillDiscountAmount.ToString(),
                Id = entity.PaymentId,
                PaymentStatus = ((PaymentStatusDto)entity.PaymentStatus).ToString(),
                PaymentStatusVal = entity.PaymentStatus,
                WsBillPaymentOnlinesOperationId = entity.WsBillPaymentOnlinesOperationId,
                WebhookRegistrationsOperationId = entity.WebhookRegistrationsOperationId,
            };
        }

        public static PaymentExcelModel ToExcelModel(this PaymentBillDto entity, IEnumerable<BinDto> bins)
        {
            var binValue = Int32.Parse(entity.CardMaskedNumber.Substring(0, 6));
            var bin = bins.FirstOrDefault(b => b.Value == binValue);

            var references = entity.ReferenceNumber;

            if (!entity.ReferenceNumber2.IsEmpty())
                references += "|" + entity.ReferenceNumber2;
            if (!entity.ReferenceNumber3.IsEmpty())
                references += "|" + entity.ReferenceNumber3;
            if (!entity.ReferenceNumber4.IsEmpty())
                references += "|" + entity.ReferenceNumber4;
            if (!entity.ReferenceNumber5.IsEmpty())
                references += "|" + entity.ReferenceNumber5;
            if (!entity.ReferenceNumber6.IsEmpty())
                references += "|" + entity.ReferenceNumber6;

            return new PaymentExcelModel
            {
                ServiceName = entity.ServiceName,

                GatewayName = entity.GatewayName,
                PaymentDate = entity.PaymentDate.ToString("dd/MM/yyyy HH:mm:ss"),
                PaymentType = EnumHelpers.GetName(typeof(PaymentTypeDto), (int)entity.PaymentType, EnumsStrings.ResourceManager),
                ServiceCategoryName = entity.ServiceCategoryName,
                CSTransactionIdentifier = entity.CSTransactionIdentifier,
                PaymentUniqueIdentifier = entity.PaymentUniqueIdentifier.ToString(),

                BillTaxedAmount = entity.BillTaxedAmount.ToString(),
                BillAmount = entity.BillAmount.ToString(),
                BillCurrency = entity.BillCurrency,
                //TODO AGREGAR LINEA QUE INDIQUE LA LEY. MODIFICAR VIEW PARA OTRA COLUMNA
                //BillDiscount = EnumHelpers.GetName(typeof(DiscountTypeDto), (int)entity.BillDiscount, EnumsStrings.ResourceManager),
                BillDiscount = entity.BillDiscount.ToString(),
                BillDiscountAmount = entity.BillDiscountAmount.ToString(),

                CardDueDate = entity.CardDueDate.ToString("MM/yyyy"),
                CardMaskedNumber = entity.CardMaskedNumber,
                CardType = bin != null ? EnumHelpers.GetName(typeof(CardTypeDto), (int)bin.CardType, EnumsStrings.ResourceManager) : "",

                UserEmail = entity.UserEmail,
                UserName = entity.UserName,
                UserSurname = entity.UserSurname,

                PaymentStatus = ((PaymentStatusDto)entity.PaymentStatus).ToString(),
                TransactionNumber = entity.TransactionNumber,
                PaymentCSRequestCurrency = entity.PaymentCSRequestCurrency,
                PaymentCSAuthCode = entity.PaymentCSAuthCode,
                PaymentCSAuthTime = entity.PaymentCSAuthTime,
                PaymentTotalAmount = entity.PaymentTotalAmount.ToString(),
                PaymentTaxedAmount = entity.PaymentTaxedAmount.ToString(),
                PaymentDiscount = entity.PaymentDiscount.ToString(),
                PaymentAmountToCS = entity.PaymentAmountToCS.ToString(),

                BillExpirationDate = entity.BillExpirationDate.ToString("dd/MM/yyyy"),
                BillExternalId = entity.BillExternalId,
                GatewayTransactionId = entity.GatewayTransactionId,
                BillFinalConsumer = entity.BillFinalConsumer ? "Sí" : "No",
                BillSucivePreBillNumber = entity.BillSucivePreBillNumber,

                ServiceAssociatedDescription = entity.ServiceAssociatedDescription,
                ReferenceNumbers = references,
            };
        }
    }
}