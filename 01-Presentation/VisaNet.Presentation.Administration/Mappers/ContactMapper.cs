using VisaNet.Common.Resource.EntitiesDto;
using VisaNet.Common.Resource.Enums;
using VisaNet.Common.Resource.Helpers;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Presentation.Administration.Models;

namespace VisaNet.Presentation.Administration.Mappers
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
                QueryType = EnumHelpers.GetName(typeof(QueryTypeDto), (int)entity.QueryType, EnumsStrings.ResourceManager),
                Subject = entity.Subject,
                Message = entity.Message,
                Date = entity.Date.ToShortDateString(),
                Comments = entity.Comments,
                Taken = entity.Taken,
                User = entity.UserTook != null ? entity.UserTook.LDAPUserName : "",
                PhoneNumber = entity.PhoneNumber
            };
        }
    }
}