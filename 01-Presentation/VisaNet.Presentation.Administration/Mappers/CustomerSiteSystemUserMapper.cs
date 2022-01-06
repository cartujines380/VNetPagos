using System;
using VisaNet.CustomerSite.EntitiesDtos;
using VisaNet.Presentation.Administration.Models;

namespace VisaNet.Presentation.Administration.Mappers
{
    public static class CustomerSiteSystemUserMapper
    {
        public static CustomerSiteSystemUserDto ToDto(this CustomerSiteSystemUserModel entity)
        {
            var dto = new CustomerSiteSystemUserDto
            {
                Id = entity.Id,
                Email = entity.Email,
                Name = entity.Name,
                Surname = entity.Surname,
                PhoneNumber = entity.PhoneNumber,
                Master = entity.UserType == CustomerSystemUserUserType.Master,
                CommerceDto = new CustomerSiteCommerceDto()
                {
                    Id = entity.CommerceId
                },
                MembershipIdentifierObj = new CustomerSiteMembershipUserDto()
                {
                    Blocked = entity.Disabled
                },
                SendEmailActivation = entity.SendEmailActivation
            };
            return dto;
        }

        public static CustomerSiteSystemUserModel ToModel(this CustomerSiteSystemUserDto entity)
        {
            var model = new CustomerSiteSystemUserModel
            {
                Id = entity.Id,
                Email = entity.Email,
                Name = entity.Name,
                Surname = entity.Surname,
                PhoneNumber = entity.PhoneNumber,
                UserType = entity.Master == true ? CustomerSystemUserUserType.Master : CustomerSystemUserUserType.Alternative,
                CommerceId = entity.CommerceDto.Id,
                Disabled = entity.MembershipIdentifierObj.Blocked,
                SendEmailActivation = entity.SendEmailActivation,
                FailLogInCount = entity.MembershipIdentifierObj.FailLogInCount,
                LastAttemptToLogIn = entity.MembershipIdentifierObj.LastAttemptToLogIn,
                LastResetPassword = entity.MembershipIdentifierObj.LastResetPassword
            };
            return model;
        }
    }
}