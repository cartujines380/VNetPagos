using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using VisaNet.Application.Implementations.Base;
using VisaNet.Application.Interfaces;
using VisaNet.Common.AzureUpload;
using VisaNet.Domain.Entities;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Repository.Interfaces.Interfaces;

namespace VisaNet.Application.Implementations
{
    public class ServicePromotion : BaseService<Promotion, PromotionDto>, IServicePromotion
    {
        private string _folderBlob = ConfigurationManager.AppSettings["AzurePromotionsImagesUrl"];

        public ServicePromotion(IRepositoryPromotion repository)
            : base(repository)
        {
        }

        public override IQueryable<Promotion> GetDataForTable()
        {
            throw new NotImplementedException();
        }

        public override PromotionDto Converter(Promotion entity)
        {
            var dto = new PromotionDto()
            {
                Active = entity.Active,
                Id = entity.Id,
                Name = entity.Name,
                ImageName = entity.ImageName,
                ImageUrl = FileStorage.Instance.GetImageUrl(_folderBlob, entity.Id, entity.ImageName)
            };

            return dto;
        }

        public override Promotion Converter(PromotionDto entity)
        {
            var promotion = new Promotion()
            {
                Id = entity.Id,
                Name = entity.Name,
                ImageName = entity.ImageName,
                Active = entity.Active
            };

            return promotion;
        }

        public override PromotionDto Create(PromotionDto entity, bool returnEntity = false)
        {
            if (entity.Active)
            {
                var all = Repository.AllNoTracking();
                foreach (var proms in all)
                {
                    proms.Active = false;
                    Repository.Edit(proms);
                }
            }
            return base.Create(entity, returnEntity);
        }

        public override void Edit(PromotionDto promotion)
        {
            Repository.ContextTrackChanges = true;

            //si la promotion que se edita esta activa, desactivo el resto.
            if (promotion.Active)
            {
                var all = Repository.AllNoTracking(p => p.Id != promotion.Id);
                foreach (var proms in all)
                {
                    proms.Active = false;
                    Repository.Edit(proms);
                }
            }

            var entity = Repository.GetById(promotion.Id);
            entity.Active = promotion.Active;
            entity.Name = promotion.Name;
            entity.ImageName = promotion.ImageName;

            Repository.Edit(entity);
            Repository.Save();
            Repository.ContextTrackChanges = false;
        }

        public IEnumerable<PromotionDto> GetDataForTable(PromotionFilterDto filters)
        {
            var query = Repository.AllNoTracking();

            if (!string.IsNullOrEmpty(filters.GenericSearch))
                query = query.Where(sc => sc.Name.Contains(filters.GenericSearch.ToLower()));

            if (!string.IsNullOrEmpty(filters.Name))
                query = query.Where(sc => sc.Name.Contains(filters.Name));

            //if (filters.SortDirection == SortDirection.Asc)
            //    query = query.OrderByStringProperty(filters.OrderBy);
            //else
            //    query = query.OrderByStringPropertyDescending(filters.OrderBy);

            var list = query.Select(t => new PromotionDto
            {
                Id = t.Id,
                Name = t.Name,
                ImageName = t.ImageName,
                Active = t.Active
            }).ToList();

            foreach (var p in list)
            {
                p.ImageUrl = FileStorage.Instance.GetImageUrl(_folderBlob, p.Id, p.ImageName);
            }

            return list;
        }

        public override void Delete(Guid id)
        {
            var item = Repository.GetById(id);
            base.Delete(id);
        }

        public IEnumerable<PromotionDto> GetLastActive()
        {
            var prom = Repository.AllNoTracking(p => p.Active).OrderByDescending(p => p.CreationDate);
            var promotions = prom.Select(t => new PromotionDto()
            {
                ImageName = t.ImageName,
            }).ToList();

            foreach (var p in promotions)
            {
                p.ImageUrl = FileStorage.Instance.GetImageUrl(_folderBlob, p.Id, p.ImageName);
            }

            return promotions;
        }
    }
}