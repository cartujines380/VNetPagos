using System.Linq;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Presentation.Web.Areas.Debit.Models;
using CardModel = VisaNet.Presentation.Web.Areas.Debit.Models.CardModel;

namespace VisaNet.Presentation.Web.Areas.Debit.Mappers
{
    public static class ApplicationUserMapper
    {
        public static ApplicationUserModel ToAppModel(this ApplicationUserDto entity)
        {
            return new ApplicationUserModel()
            {
                Id = entity.Id,
                Name = entity.Name,
                Surname = entity.Surname,
                Email = entity.Email,
                IdentityNumber = entity.IdentityNumber,
                CallCenterKey = entity.CallCenterKey,
                MobileNumber = entity.MobileNumber,
                Password = entity.Password,
                Cards = entity.CardDtos != null ? entity.CardDtos.Where(x => !x.Deleted && x.Active).Select(x => new CardModel()
                {
                    Id = x.Id,
                    Number = x.MaskedNumber,
                    DueDate = x.DueDate.ToString("MM/yyyy"),
                }).ToList() : null
            };
        }

        public static ApplicationUserDto ToAppDto(this ApplicationUserModel entity)
        {
            return new ApplicationUserDto()
            {
                Id = entity.Id,
                Name = entity.Name,
                Surname = entity.Surname,
                Email = entity.Email,
                IdentityNumber = entity.IdentityNumber,
                CallCenterKey = entity.CallCenterKey,
                MobileNumber = entity.MobileNumber,
            };
        }
    }
}