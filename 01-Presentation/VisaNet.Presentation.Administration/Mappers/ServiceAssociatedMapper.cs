using System;
using System.Collections.Generic;
using System.Linq;
using VisaNet.Common.Resource.EntitiesDto;
using VisaNet.Common.Resource.Enums;
using VisaNet.Common.Resource.Helpers;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Presentation.Administration.Models;

namespace VisaNet.Presentation.Administration.Mappers
{
    public static class ServiceAssociatedMapper
    {
        public static ServiceAssociatedModel ToModel(this ServiceAssociatedDto entity, IEnumerable<BinDto> bins)
        {
            var card = entity.DefaultCardId != default(Guid) ? entity.DefaultCard : null;
            var binValue = card != null ? Int32.Parse(card.MaskedNumber.Substring(0, 6)) : 0;
            var bin = binValue != 0 ? bins.FirstOrDefault(b => b.Value == binValue) : null;

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

            return new ServiceAssociatedModel
            {
                ClientEmail = entity.RegisteredUserDto.Email,
                ClientName = entity.RegisteredUserDto.Name,
                ClientSurname = entity.RegisteredUserDto.Surname,
                ClientIdentityNumber = entity.RegisteredUserDto.IdentityNumber,
                ClientMobileNumber = entity.RegisteredUserDto.MobileNumber,
                ClientPhoneNumber = entity.RegisteredUserDto.PhoneNumber,
                ClientAddress = entity.RegisteredUserDto.Address,
                ServiceName = entity.ServiceDto.Name,
                ServiceCategoryName = entity.ServiceDto.ServiceCategory.Name,
                ReferenceNumber = referenceNumber,
                Description = entity.Description,
                CardMaskedNumber = card != null ? card.MaskedNumber : "",
                CardDueDate = card != null ? card.DueDate.ToShortDateString() : "",
                CardBin = bin != null ? bin.Name : "",
                CardType = bin != null ? EnumHelpers.GetName(typeof(CardTypeDto), (int)bin.CardType, EnumsStrings.ResourceManager) : "",
                AutomaticPayment = entity.AutomaticPaymentDtoId.HasValue && entity.AutomaticPaymentDtoId.Value != default(Guid) ? PresentationCoreMessages.Common_Yes : PresentationCoreMessages.Common_No,
                Enabled = entity.Enabled ? PresentationCoreMessages.Common_Yes : PresentationCoreMessages.Common_No,
            };
        }
    }
}