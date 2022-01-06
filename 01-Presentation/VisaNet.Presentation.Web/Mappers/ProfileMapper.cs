using VisaNet.Domain.EntitiesDtos;
using VisaNet.Presentation.Web.Areas.Private.Models;

namespace VisaNet.Presentation.Web.Mappers
{
    public static class ProfileMapper
    {
        public static ProfileModel ToModel(this ApplicationUserDto entity)
        {
            return new ProfileModel
            {
                Id = entity.Id,
                Name = entity.Name,
                Surname = entity.Surname,
                Email = entity.Email,
                OldEmail = entity.Email,
                Address = entity.Address,
                CallCenterKey = entity.CallCenterKey,
                IdentityNumber = entity.IdentityNumber,
                MobileNumber = entity.MobileNumber,
                PhoneNumber = entity.PhoneNumber,
                RecieveNewsletter = entity.RecieveNewsletter
            };
        }

        public static ApplicationUserDto ToDto(this ProfileModel entity)
        {
            return new ApplicationUserDto
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
                RecieveNewsletter = entity.RecieveNewsletter
            };
        }
    }
}