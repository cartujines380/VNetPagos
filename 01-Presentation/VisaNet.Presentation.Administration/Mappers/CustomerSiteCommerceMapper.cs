using VisaNet.CustomerSite.EntitiesDtos;
using VisaNet.Presentation.Administration.Models;

namespace VisaNet.Presentation.Administration.Mappers
{
    public static class CustomerSiteCommerceMapper
    {
        public static CustomerSiteCommerceDto ToDto(this CustomerSiteCommerceModel entity)
        {
            var dto = new CustomerSiteCommerceDto
            {
                Id = entity.Id,
                Name = entity.Name,
                ContactAddress = entity.ContactAddress,
                ContactEmail = entity.ContactEmail,
                ContactPhoneNumber = entity.ContactPhoneNumber,
                Disabled = entity.Disabled,
                ServiceId = entity.ServiceId,
                ImageName = entity.ImageName,
                ImageUrl = entity.ImageUrl
            };
            return dto;
        }

        public static CustomerSiteCommerceModel ToModel(this CustomerSiteCommerceDto entity)
        {
            var model = new CustomerSiteCommerceModel
            {
                Id = entity.Id,
                Name = entity.Name,
                ContactAddress = entity.ContactAddress,
                ContactEmail = entity.ContactEmail,
                ContactPhoneNumber = entity.ContactPhoneNumber,
                Disabled = entity.Disabled,
                ServiceId = entity.ServiceId,
                ImageName = entity.ImageName,
                ImageUrl = entity.ImageUrl
            };
            return model;
        }

    }
}