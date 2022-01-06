using VisaNet.Domain.EntitiesDtos;
using VisaNet.Presentation.Web.Models;

namespace VisaNet.Presentation.Web.Mappers
{
    public static class ContactMapper
    {
        public static ContactModel ToModel(this ContactDto entity)
        {
            return new ContactModel
            {
                Id = entity.Id,
                Name = entity.Name,
                Surname = entity.Surname,
                Email = entity.Email,
                QueryType = entity.QueryType,
                Subject = entity.Subject,
                Message = entity.Message,
                Date = entity.Date,
                PhoneNumber = entity.PhoneNumber
            };
        }

        public static ContactDto ToDto(this ContactModel entity)
        {
            return new ContactDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Surname = entity.Surname,
                Email = entity.Email,
                QueryType = entity.QueryType,
                Subject = entity.Subject,
                Message = entity.Message,
                Date = entity.Date,
                PhoneNumber = entity.PhoneNumber
            };
        }
    }
}