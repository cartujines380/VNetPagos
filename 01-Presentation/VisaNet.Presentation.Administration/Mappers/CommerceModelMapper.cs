using System.Linq;
using VisaNet.CustomerSite.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Presentation.Administration.Models;

namespace VisaNet.Presentation.Administration.Mappers
{
    public static class CommerceModelMapper
    {
        public static CommerceModel ToCommerceModel(this CustomerSiteCommerceDto entity)
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
                ImageName = entity.ImageName,
                ImageUrl = entity.ImageUrl,

                CommerceCathegoryModel = entity.CommerceCathegoryDto == null
                    ? null
                    : new CommerceCathegoryModel
                        {
                            Id = entity.CommerceCathegoryDto.Id,
                            Name = entity.CommerceCathegoryDto.Name
                        },
                ProductosListModel = entity.ProductosListDto
                    .Select(y =>
                        new ProductModel
                        {
                            Id = y.Id,
                            DebitMerchantId = y.DebitMerchantId,
                            DebitProductid = y.DebitProductid,
                            Description = y.Description,
                            Expiration = y.Expiration,
                            ProductPropertyModelList = y.ProductPropertyList
                                .OrderBy(z => z.InputSequence)
                                .Select(z =>
                                    new ProductPropertyModel
                                    {
                                        Id = z.Id,
                                        Name = z.Name,
                                        ContentType = z.ContentType,
                                        InputSequence = z.InputSequence,
                                        MaxSize = z.MaxSize,
                                        Requiered = z.Requiered,
                                        DebitProductId = z.DebitProductId,
                                        DebitProductPropertyId = z.DebitProductPropertyId,

                                    })
                                .ToList()
                        })
                    .ToList()
            };

            return model;
        }

        public static CommerceModel ToCommerceModel(this CommerceDto entity)
        {
            var model = new CommerceModel
            {
                Id = entity.Id,
                Tags = string.Empty,
                Name = entity.Name,
                ServiceId = entity.ServiceId,
                ImageName = entity.ImageName,
                ImageUrl = entity.ImageUrl,
                ProductosListModel = entity.ProductosListDto == null ? null :
                    entity.ProductosListDto.Select(y => new ProductModel()
                    {
                        Id = y.Id,
                        Description = y.Description,
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