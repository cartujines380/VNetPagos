using VisaNet.Common.Resource.Enums;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Presentation.Administration.Models;

namespace VisaNet.Presentation.Administration.Mappers
{
    public static class ApplicationUserMapper
    {
        public static ApplicationUserDto ToDto(this ApplicationUserModel entity)
        {
            var dto = new ApplicationUserDto
            {
                Id = entity.Id,
		        Email = entity.Email,
		        Name = entity.Name,
		        Surname = entity.Surname,
		        IdentityNumber = entity.IdentityNumber,
		        MobileNumber = entity.MobileNumber,
		        PhoneNumber = entity.PhoneNumber,
		        Address = entity.Address,
		        CallCenterKey = entity.CallCenterKey,
		        Platform = (PlatformDto)entity.PlatformId
            };
            return dto;
        }

        public static ApplicationUserModel ToModel(this ApplicationUserDto entity)
        {
            var model = new ApplicationUserModel
            {
                Id = entity.Id,
                Email = entity.Email,
                Name = entity.Name,
                Surname = entity.Surname,
                IdentityNumber = entity.IdentityNumber,
                MobileNumber = entity.MobileNumber,
                PhoneNumber = entity.PhoneNumber,
                Address = entity.Address,
                CallCenterKey = entity.CallCenterKey,
                PlatformId = (int)entity.Platform,
                Status = entity.MembershipIdentifierObj.Blocked ? EnumsStrings.ActiveOrInactiveEnumDto_Blocked :
                    entity.MembershipIdentifierObj.Active ? EnumsStrings.ActiveOrInactiveEnumDto_Active : EnumsStrings.ActiveOrInactiveEnumDto_Inactive,
                FailLogInCount = entity.MembershipIdentifierObj != null ? entity.MembershipIdentifierObj.FailLogInCount : 0,
                CreationDate = entity.CreationDate
            };

            return model;
        }
    }
}