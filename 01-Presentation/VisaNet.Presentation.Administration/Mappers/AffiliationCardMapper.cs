using System;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Presentation.Administration.Models;

namespace VisaNet.Presentation.Administration.Mappers
{
    public static class AffiliationCardMapper
    {
        public static AffiliationCardDto ToDto(this AffiliationCardModel entity)
        {
            var dto = new AffiliationCardDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Code = entity.Code,
                Active = entity.Active
            };
            if (entity.BankId == null || entity.BankId == Guid.Empty)
            {
                dto.BankId = null;
            }
            else
            {
                dto.BankId = entity.BankId;
            }
            return dto;
        }

        public static AffiliationCardModel ToModel(this AffiliationCardDto entity)
        {
            var model = new AffiliationCardModel
            {
                Id = entity.Id,
                Name = entity.Name,
                Code = entity.Code,
                BankId = entity.BankId,
                BankName = entity.BankDto != null ? entity.BankDto.Name : string.Empty,
                Active = entity.Active
            };
            return model;
        }
    }
}