using System;
using System.Collections.Generic;
using System.Linq;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Presentation.Administration.Models;

namespace VisaNet.Presentation.Administration.Mappers
{
    public static class DebitRequestMapper
    {
        public static DebitRequestModel ToModel(this DebitRequestDto entity)
        {
            return new DebitRequestModel
            {
                Id = entity.Id,
                DebitProductId = entity.DebitProductId,
                CardId = entity.CardId,
                CardModel = entity.CardDto != null ? new CardModel()
                {
                    CardMaskedNumber = entity.CardDto.MaskedNumber,
                    CardDueDate = entity.CardDto.DueDate.ToString("MM/yyyy")
                }: null,
                UserId = entity.UserId,
                ApplicationUserModel = entity.ApplicationUserDto != null ? new ApplicationUserModel()
                {
                    Email = entity.ApplicationUserDto.Email,
                } : null,
                CreationDate = entity.CreationDate,
                State = (DebitRequestStateModel)(int)entity.State,
                Type = (DebitRequestTypeModel)(int)entity.Type,
                References = entity.References != null ? entity.References.Select(x => x.ToModel()).ToList() : null,
                CommerceModel = entity.CommerceDto != null ? entity.CommerceDto.ToCommerceModel() : null
            };
        }

    }
}