using VisaNet.Domain.EntitiesDtos;
using VisaNet.Presentation.Web.Models;

namespace VisaNet.Presentation.Web.Mappers
{
    public static class RegistrationMapper
    {
        public static RegistrationModel ToModel(this ApplicationUserCreateEditDto entity)
        {
            return new RegistrationModel
            {
                Id = entity.Id,
                Name = entity.Name,
                Surname = entity.Surname,
                Email = entity.Email,
                Address = entity.Address,
                CallCenterKey = entity.CallCenterKey,
                IdentityNumber = entity.IdentityNumber,
                MobileNumber = entity.MobileNumber,
                PhoneNumber = entity.PhoneNumber,
            };
        }

        public static ApplicationUserCreateEditDto ToDto(this RegistrationModel entity)
        {
            return new ApplicationUserCreateEditDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Surname = entity.Surname,
                Email = entity.Email,
                Address = entity.Address,
                CallCenterKey = entity.CallCenterKey,
                IdentityNumber = entity.IdentityNumber,
                MobileNumber = entity.MobileNumber,
                PhoneNumber = entity.PhoneNumber,
                Password = entity.Password,
            };
        }
    }
}