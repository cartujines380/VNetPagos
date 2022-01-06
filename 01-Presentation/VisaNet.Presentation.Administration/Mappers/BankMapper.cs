using VisaNet.Domain.EntitiesDtos;
using VisaNet.Presentation.Administration.Models;

namespace VisaNet.Presentation.Administration.Mappers
{
    public static class BankMapper
    {
        public static BankDto ToDto(this BankModel entity)
        {
            var dto = new BankDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Code = entity.Code,
                QuotesPermited = entity.QuotesPermited
            };
            return dto;
        }

        public static BankModel ToModel(this BankDto entity)
        {
            var model = new BankModel
            {
                Id = entity.Id,
                Name = entity.Name,
                Code = entity.Code,
                QuotesPermited = entity.QuotesPermited,                
            };
            return model;
        }
    }
}