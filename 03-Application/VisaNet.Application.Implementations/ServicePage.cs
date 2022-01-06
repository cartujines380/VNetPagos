using System.Linq;
using VisaNet.Application.Implementations.Base;
using VisaNet.Application.Interfaces;
using VisaNet.Domain.Entities;
using VisaNet.Domain.Entities.Enums;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Repository.Interfaces.Interfaces;

namespace VisaNet.Application.Implementations
{
    public class ServicePage : BaseService<Page, PageDto>, IServicePage
    {
        public ServicePage(IRepositoryPage repository)
            : base(repository)
        {
        }

        public override IQueryable<Page> GetDataForTable()
        {
            return Repository.AllNoTracking();
        }

        public override PageDto Converter(Page entity)
        {
            if (entity == null) return null;

            return new PageDto
            {
                Id = entity.Id,
                Content = entity.Content,
                PageType = (PageTypeDto)(int)entity.PageType
            };
        }

        public override Page Converter(PageDto entity)
        {
            if (entity == null) return null;

            return new Page
            {
                Id = entity.Id,
                Content = entity.Content,
                PageType = (PageType)(int)entity.PageType
            };
        }

        public override void Edit(PageDto entity)
        {
            var entity_db = Repository.GetById(entity.Id);

            entity_db.Content = entity.Content;

            Repository.Edit(entity_db);
            Repository.Save();
        }

        
    }
}
