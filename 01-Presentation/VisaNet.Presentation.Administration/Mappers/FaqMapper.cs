using VisaNet.Domain.EntitiesDtos;
using VisaNet.Presentation.Administration.Models;

namespace VisaNet.Presentation.Administration.Mappers
{
    public static class FaqMapper
    {
        public static FaqDto ToDto(this FaqModel entity)
        {
            return new FaqDto
            {
                Id = entity.Id,
                Order = entity.Order,
                Question = entity.Question,
                Answer = entity.Answer
            };
        }

        public static FaqModel ToModel(this FaqDto entity)
        {
            return new FaqModel
            {
                Id = entity.Id,
                Order = entity.Order,
                Question = entity.Question,
                Answer = entity.Answer
            };
        }
    }
}