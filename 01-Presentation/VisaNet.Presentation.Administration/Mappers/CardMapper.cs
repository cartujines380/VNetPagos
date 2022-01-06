using System;
using System.Collections.Generic;
using System.Linq;
using VisaNet.Common.Resource.Enums;
using VisaNet.Common.Resource.Helpers;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Presentation.Administration.Models;

namespace VisaNet.Presentation.Administration.Mappers
{
    public static class CardMapper
    {
        public static IEnumerable<CardModel> ToModel(this ApplicationUserDto entity, IEnumerable<BinDto> bins, ReportsCardsFilterDto filter)
        {
            var list = new List<CardModel>();

            foreach (var card in entity.CardDtos)
            {
                var binValue = Int32.Parse(card.MaskedNumber.Substring(0, 6));
                var bin = binValue != 0 ? bins.FirstOrDefault(b => b.Value == binValue) : null;

                var appliesFilters = true;

                if (filter != null && !string.IsNullOrEmpty(filter.CardMaskedNumber))
                    appliesFilters = card.MaskedNumber.ToLower().Contains(filter.CardMaskedNumber.ToLower());

                if (filter != null && !string.IsNullOrEmpty(filter.CardBin))
                    appliesFilters = appliesFilters && bin != null && bin.Value.ToString() == filter.CardBin;

                if (filter != null && filter.CardType != 0)
                    appliesFilters = appliesFilters && bin != null && (int)bin.CardType == filter.CardType;

                if (filter != null && filter.CardState == 1)
                    appliesFilters = appliesFilters && card.Active;

                if (filter != null && filter.CardState == 2)
                    appliesFilters = appliesFilters && !card.Active;

                if (appliesFilters)
                {
                    list.Add(new CardModel
                    {
                        ClientEmail = entity.Email,
                        ClientName = entity.Name,
                        ClientSurname = entity.Surname,
                        ClientIdentityNumber = entity.IdentityNumber,
                        ClientMobileNumber = entity.MobileNumber,
                        ClientPhoneNumber = entity.PhoneNumber,
                        ClientAddress = entity.Address,
                        CardMaskedNumber = card.MaskedNumber,
                        CardDueDate = card.DueDate.ToShortDateString(),
                        CardBin = bin != null ? bin.Name : "",
                        CardType = bin != null ? EnumHelpers.GetName(typeof(CardTypeDto), (int)bin.CardType, EnumsStrings.ResourceManager) : "",
                        CardActive = card.Active ? PresentationCoreMessages.Common_Yes : PresentationCoreMessages.Common_No,
                        CardDeleted = card.Deleted ? PresentationCoreMessages.Common_Yes : PresentationCoreMessages.Common_No,
                    });
                }
            }

            return list;
        }

    }
}