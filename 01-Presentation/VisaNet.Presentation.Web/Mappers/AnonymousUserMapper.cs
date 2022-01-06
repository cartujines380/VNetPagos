using VisaNet.Domain.EntitiesDtos;
using VisaNet.Presentation.Web.Models;

namespace VisaNet.Presentation.Web.Mappers
{
    public static class AnonymousUserMapper
    {
        public static AnonymousUserDto ToDto(this AnonymousUserModel entity)
        {
            if (entity == null) return null;
            return new AnonymousUserDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Surname = entity.Surname,
                Email = entity.Email,
                Address = entity.Address,
                PhoneNumber = entity.PhoneNumber,
                IdentityNumber = entity.IdentityNumber,
                MobileNumber = entity.MobileNumber,
            };
        }

        public static AnonymousUserModel ToModel(this AnonymousUserDto entity)
        {
            if (entity == null) return null;
            return new AnonymousUserModel
            {
                Id = entity.Id,
                Name = entity.Name,
                Surname = entity.Surname,
                Email = entity.Email,
                Address = entity.Address,
                PhoneNumber = entity.PhoneNumber,
                IdentityNumber = entity.IdentityNumber,
                MobileNumber = entity.MobileNumber
            };
        }
    }
}