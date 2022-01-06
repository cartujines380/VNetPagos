using System.Linq;
using VisaNet.CustomerSite.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Presentation.Web.Models;

namespace VisaNet.Presentation.Web.Areas.Debit.Mappers
{
    public static class CommerceMapper
    {
        public static CommerceModel ToModel(this CustomerSiteCommerceDto entity)
        {
            var model = new CommerceModel
            {
                Id = entity.Id,
                Tags = string.Empty,
                Name = entity.Name,
                ContactAddress = entity.ContactAddress,
                ContactEmail = entity.ContactEmail,
                ContactPhoneNumber = entity.ContactPhoneNumber,
                Disabled = entity.Disabled,

                ServiceId = entity.ServiceId,

                CommerceCathegoryModel = entity.CommerceCathegoryDto == null ? null : new CommerceCathegoryModel()
                {
                    Id = entity.CommerceCathegoryDto.Id,
                    Name = entity.CommerceCathegoryDto.Name
                },
                ProductosListModel= entity.ProductosListDto == null ? null :
                    entity.ProductosListDto.Select(y => new ProductModel()
                    {
                        Id = y.Id,
                        DebitMerchantId = y.DebitMerchantId,
                        DebitProductid = y.DebitProductid,
                        Description = y.Description,
                        Expiration = y.Expiration,
                        ProductPropertyModelList = y.ProductPropertyList == null ? null :
                            y.ProductPropertyList.OrderBy(z => z.InputSequence).Select(z => new ProductPropertyModel()
                            {
                                Id = z.Id,
                                Name = z.Name,
                                ContentType = z.ContentType,
                                InputSequence = z.InputSequence,
                                MaxSize = z.MaxSize,
                                Requiered = z.Requiered,
                                DebitProductId = z.DebitProductId,
                                DebitProductPropertyId = z.DebitProductPropertyId,
                                
                            }).ToList()
                    }).ToList(),
            };

            return model;
        }

    }
}