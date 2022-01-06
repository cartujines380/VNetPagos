using System;
using System.Collections.Generic;
using System.Linq;
using VisaNet.Application.Implementations.Base;
using VisaNet.Application.Interfaces;
using VisaNet.Common.Exceptions;
using VisaNet.Common.Logging.Services;
using VisaNet.Domain.Entities;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Repository.Interfaces.Interfaces;

namespace VisaNet.Application.Implementations
{
    public class ServiceServiceCategory : BaseService<ServiceCategory, ServiceCategoryDto>, IServiceServiceCategory
    {

        private readonly IRepositoryService _repositoryService;
        private readonly ILoggerService _logerService;

        public ServiceServiceCategory(IRepositoryServiceCategory repository, IRepositoryService repositoryService, ILoggerService logerService)
            : base(repository)
        {
            _repositoryService = repositoryService;
            _logerService = logerService;
        }

        public override IQueryable<ServiceCategory> GetDataForTable()
        {

            return Repository.AllNoTracking();
        }

        public override ServiceCategoryDto Converter(ServiceCategory entity)
        {
            var dto = new ServiceCategoryDto
                          {
                              Id = entity.Id,
                              Name = entity.Name
                          };

            return dto;
        }

        public override ServiceCategory Converter(ServiceCategoryDto entity)
        {
            var scategory = new ServiceCategory()
                                {
                                    Id = entity.Id,
                                    Name = entity.Name
                                };
            return scategory;
        }

        public override ServiceCategoryDto Create(ServiceCategoryDto entity, bool returnEntity = false)
        {
            if (Repository.AllNoTracking(s => s.Name.Equals(entity.Name, StringComparison.InvariantCultureIgnoreCase)).Any())
                throw new BusinessException(CodeExceptions.SERVICECATEGORY_NAME_ALREADY_USED);

            return base.Create(entity, returnEntity);
        }

        public override void Edit(ServiceCategoryDto entity)
        {

            if (Repository.AllNoTracking(s => s.Name.Equals(entity.Name, StringComparison.InvariantCultureIgnoreCase) && s.Id != entity.Id).Any())
                throw new BusinessException(CodeExceptions.SERVICECATEGORY_NAME_ALREADY_USED);
            Repository.ContextTrackChanges = true;
            var entity_db = Repository.GetById(entity.Id);

            entity_db.Name = entity.Name;

            Repository.Edit(entity_db);
            Repository.Save();
            Repository.ContextTrackChanges = false;
        }

        public override void Delete(ServiceCategoryDto entity)
        {
            if (_repositoryService.AllNoTracking(s => s.ServiceCategoryId == entity.Id).Any())
                throw new BusinessException(CodeExceptions.SERVICECATEGORY_IN_SERVICE);
            base.Delete(entity);
        }

        public override void Delete(Guid id)
        {
            if (_repositoryService.AllNoTracking(s => s.ServiceCategoryId == id).Any())
                throw new BusinessException(CodeExceptions.SERVICECATEGORY_IN_SERVICE);

            base.Delete(id);
        }

        public IEnumerable<ServiceCategoryDto> GetDataForTable(ServiceCategoryFilterDto filters)
        {
            var query = Repository.AllNoTracking();

            if (!string.IsNullOrEmpty(filters.GenericSearch))
                query = query.Where(sc => sc.Name.ToLower().Contains(filters.GenericSearch.ToLower()));

            if (!string.IsNullOrEmpty(filters.Name))
                query = query.Where(sc => sc.Name.ToLower().Contains(filters.Name.ToLower()));

            return query.Select(t => new ServiceCategoryDto
            {
                Id = t.Id,
                Name = t.Name
            }).ToList();
        }

    }


}
