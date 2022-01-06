using System;
using System.Linq;
using VisaNet.Application.Implementations.Base;
using VisaNet.Application.Interfaces;
using VisaNet.Domain.Entities;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Repository.Interfaces.Interfaces;

namespace VisaNet.Application.Implementations
{
    public class ServiceHomePageItem : BaseService<HomePageItem, HomePageItemDto>, IServiceHomePageItem
    {
        public ServiceHomePageItem(IRepositoryHomePageItem repository)
            : base(repository)
        {
        }

        public override IQueryable<HomePageItem> GetDataForTable()
        {
            return Repository.AllNoTracking(null, h => h.Image);
        }

        public override HomePageItemDto Converter(HomePageItem entity)
        {
            if (entity == null) return null;

            var homePageItemDto = new HomePageItemDto
                   {
                       Id = entity.Id,
                       Headline1 = entity.Headline1,
                       Headline2 = entity.Headline2,
                       Description = entity.Description,
                       ImageId = entity.ImageId,
                       LinkUrl = entity.LinkUrl,
                       FileId = entity.FileId,
                       LinkName = entity.LinkName
                   };

            if (entity.Image != null)
            {
                homePageItemDto.Image = new ImageDto
                                        {
                                            Id = entity.Image.Id,
                                            InternalName = entity.Image.InternalName,
                                            OriginalName = entity.Image.OriginalName
                                        };
            }

            if (entity.File != null)
            {
                homePageItemDto.File = new ImageDto
                {
                    Id = entity.File.Id,
                    InternalName = entity.File.InternalName,
                    OriginalName = entity.File.OriginalName
                };
            }

            return homePageItemDto;
        }

        public override HomePageItem Converter(HomePageItemDto entity)
        {
            if (entity == null) return null;

            var homePageItemDto =  new HomePageItem
            {
                Id = entity.Id,
                Headline1 = entity.Headline1,
                Headline2 = entity.Headline2,
                Description = entity.Description,
                ImageId = entity.ImageId,
                LinkUrl = entity.LinkUrl,
                FileId = entity.FileId,
                LinkName = entity.LinkName
            };

            if (entity.Image != null)
            {
                homePageItemDto.Image = new Image()
                {
                    Id = entity.Image.Id,
                    InternalName = entity.Image.InternalName,
                    OriginalName = entity.Image.OriginalName
                };
                if (homePageItemDto.Image.Id == Guid.Empty)
                {
                    homePageItemDto.Image.GenerateNewIdentity();
                }
            }

            if (entity.File != null)
            {
                homePageItemDto.File = new Image()
                {
                    Id = entity.File.Id,
                    InternalName = entity.File.InternalName,
                    OriginalName = entity.File.OriginalName
                };
                if (homePageItemDto.File.Id == Guid.Empty)
                {
                    homePageItemDto.File.GenerateNewIdentity();
                }
            }

            return homePageItemDto;
        }

        public override void Edit(HomePageItemDto entity)
        {
            Repository.ContextTrackChanges = true;

            var entity_db = Repository.GetById(entity.Id, h => h.Image);

            entity_db.Headline1 = entity.Headline1;
            entity_db.Headline2 = entity.Headline2;
            entity_db.Description = entity.Description;

            //si en Bd hay imagen pero en la entidad no, elimino solo de BD
            if (entity_db.Image != null && entity.Image == null)
            {
                Repository.DeleteEntitiesNoRepository(entity_db.Image);
            }
            else if (entity.Image != null)
            {
                if (entity_db.Image == null)
                {
                    entity_db.Image = new Image()
                    {

                        InternalName = entity.Image.InternalName,
                        OriginalName = entity.Image.OriginalName
                    };
                    entity_db.Image.GenerateNewIdentity();
                }
                else
                {
                    entity_db.Image.InternalName = entity.Image.InternalName;
                    entity_db.Image.OriginalName = entity.Image.OriginalName;
                }

            }

            entity_db.LinkUrl = entity.LinkUrl;

            if (entity_db.File != null && entity.File == null)
            {
                Repository.DeleteEntitiesNoRepository(entity_db.File);
            }
            else if (entity.File != null)
            {
                if (entity_db.File == null)
                {
                    entity_db.File = new Image()
                    {

                        InternalName = entity.File.InternalName,
                        OriginalName = entity.File.OriginalName
                    };
                    entity_db.File.GenerateNewIdentity();
                }
                else
                {
                    entity_db.File.InternalName = entity.File.InternalName;
                    entity_db.File.OriginalName = entity.File.OriginalName;
                }

            }

            entity_db.LinkName = entity.LinkName;
            
            Repository.Edit(entity_db);
            Repository.Save();

            Repository.ContextTrackChanges = false;
        }
    }
}
