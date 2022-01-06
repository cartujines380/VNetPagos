using VisaNet.Domain.EntitiesDtos;
using VisaNet.Presentation.Web.Models;

namespace VisaNet.Presentation.Web.Mappers
{
    public static class SubscriberMapper
    {
        public static SubscriberDto ToDto(this SubscriberModel entity)
        {
            return new SubscriberDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Surname = entity.Surname,
                Email = entity.Email
            };
        }

        public static SubscriberModel ToModel(this SubscriberDto entity)
        {
            return new SubscriberModel
            {
                Id = entity.Id,
                Name = entity.Name,
                Surname = entity.Surname,
                Email = entity.Email
            };
        }
    }
}