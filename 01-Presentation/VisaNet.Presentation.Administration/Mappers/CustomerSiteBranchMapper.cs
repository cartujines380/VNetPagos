using System;
using VisaNet.CustomerSite.EntitiesDtos;
using VisaNet.Presentation.Administration.Models;

namespace VisaNet.Presentation.Administration.Mappers
{
    public static class CustomerSiteBranchMapper
    {
        public static CustomerSiteBranchDto ToDto(this CustomerSiteBranchModel entity)
        {
            var dto = new CustomerSiteBranchDto
            {
                Id = entity.Id,
                Name = entity.Name,
                ContactAddress = entity.ContactAddress,
                ContactEmail = entity.ContactEmail,
                ContactPhoneNumber = entity.ContactPhoneNumber,
                Disabled = entity.Disabled,

                CommerceId = entity.CustomerSiteCommerce,
                ServiceId = entity.ServiceId,
            };
            return dto;
        }

        public static CustomerSiteBranchModel ToModel(this CustomerSiteBranchDto entity)
        {
            var model = new CustomerSiteBranchModel
            {
                Id = entity.Id,
                Name = entity.Name,
                ContactAddress = entity.ContactAddress,
                ContactEmail = entity.ContactEmail,
                ContactPhoneNumber = entity.ContactPhoneNumber,
                Disabled = entity.Disabled,

                CustomerSiteCommerce = entity.CommerceId.ToString(),
                ServiceId = entity.ServiceId.ToString()
            };
            return model;
        }
    }
}