using System.Configuration;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Presentation.Administration.Models;

namespace VisaNet.Presentation.Administration.Mappers
{
    public static class HomePageItemMapper
    {
        public static HomePageItemDto ToDto(this HomePageItemModel entity)
        {
            var homePageItemDto = new HomePageItemDto
                   {
                       Id = entity.Id,
                       Headline1 = entity.Headline1,
                       Headline2 = entity.Headline2,
                       Description = entity.Description,
                       LinkUrl = entity.LinkUrl,
                       LinkName = entity.LinkName
                   };

            if (entity.Image_internalname != null)
            {
                homePageItemDto.Image = new ImageDto()
                {
                    InternalName = entity.Image_internalname,
                    OriginalName = entity.Image_originalname
                };
            }

            if (entity.File_internalname != null)
            {
                homePageItemDto.File = new ImageDto()
                {
                    InternalName = entity.File_internalname,
                    OriginalName = entity.File_originalname
                };
            }

            return homePageItemDto;
        }

        public static HomePageItemModel ToModel(this HomePageItemDto entity)
        {
            var homePageItemModel = new HomePageItemModel
                    {
                        Id = entity.Id,
                        Headline1 = entity.Headline1,
                        Headline2 = entity.Headline2,
                        Description = entity.Description,
                        LinkUrl = entity.LinkUrl,
                        LinkName = entity.LinkName
                   };

            if (entity.Image != null)
            {
                homePageItemModel.Image_originalname = entity.Image.OriginalName;
                homePageItemModel.Image_internalname = ConfigurationManager.AppSettings["SharedImagesFolderVD"] + entity.Image.InternalName;
                homePageItemModel.ImagenBD = entity.Image.InternalName;
            }

            if (entity.File != null)
            {
                homePageItemModel.File_originalname = entity.File.OriginalName;
                homePageItemModel.File_internalname = ConfigurationManager.AppSettings["SharedImagesFolderVD"] + entity.File.InternalName;
                homePageItemModel.FileBD = entity.File.InternalName;
            }

            return homePageItemModel;
        }
    }
}