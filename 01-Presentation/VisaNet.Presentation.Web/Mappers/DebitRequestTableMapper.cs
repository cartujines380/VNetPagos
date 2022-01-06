using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Presentation.Web.Areas.Private.Models;

namespace VisaNet.Presentation.Web.Mappers
{
    public static class DebitRequestTableMapper
    {
        public static DebitRequestTableDto ToDto(this DebitRequestTableModel entity)
        {
            return new DebitRequestTableDto
            {
                Id = entity.Id,
                CardId = entity.CardId,
                UserId = entity.UserId,
                CardNumber = entity.CardNumber,
                CardDescription = entity.CardDescription,
                MerchantName = entity.MerchantName,
                MerchantId = entity.MerchantId,
                ReferenceNumber = entity.ReferenceNumber,
                MerchantProductName = entity.MerchantProductName,
                References = entity.References,
                CreationDate = entity.CreationDate,
                State = entity.State,
                Type = entity.Type
            };
        }

        public static DebitRequestTableModel ToModel(this DebitRequestTableDto entity)
        {
            return new DebitRequestTableModel
            {
                Id = entity.Id,
                CardId = entity.CardId,
                UserId = entity.UserId,
                CardNumber = entity.CardNumber,
                CardDescription = entity.CardDescription,
                MerchantName = entity.MerchantName,
                MerchantId = entity.MerchantId,
                ReferenceNumber = entity.ReferenceNumber,
                MerchantProductName = entity.MerchantProductName,
                References = entity.References,
                CreationDate = entity.CreationDate,
                State = entity.State == DebitRequestStateDto.Synchronized || entity.State == DebitRequestStateDto.ManualSynchronization
                    ? DebitRequestStateDto.Pending
                    : entity.State,
                Type = entity.Type,
                DebitImageUrl = entity.DebitImageUrl
            };
        }

    }
}