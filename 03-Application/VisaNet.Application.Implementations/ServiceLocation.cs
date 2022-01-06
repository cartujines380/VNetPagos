using System;
using System.Collections.Generic;
using System.Linq;
using VisaNet.Application.Implementations.Base;
using VisaNet.Application.Interfaces;
using VisaNet.Domain.Entities;
using VisaNet.Domain.Entities.Enums;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Repository.Interfaces.Interfaces;

namespace VisaNet.Application.Implementations
{
    public class ServiceLocation : BaseService<Location, LocationDto>, IServiceLocation
    {
        public ServiceLocation(IRepositoryLocation repository)
            : base(repository)
        {

        }

        public override IQueryable<Location> GetDataForTable()
        {
            return Repository.AllNoTracking();
        }

        public override LocationDto Converter(Location entity)
        {
            return new LocationDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Value = entity.Value,
                Departament = entity.Departament,
                GatewayEnum = entity.GatewayEnum,
                Active = entity.Active
            };
        }

        public override Location Converter(LocationDto entity)
        {
            return new Location
            {
                Id = entity.Id,
                Name = entity.Name,
                Value = entity.Value,
                Departament = entity.Departament,
                GatewayEnum = entity.GatewayEnum,
                Active = entity.Active
            };
        }

        public IEnumerable<LocationDto> GetDataForTable(LocationFilterDto filters)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<LocationDto> GetDataForList(LocationFilterDto filters)
        {
            var query = Repository.AllNoTracking(x => x.Active);
            query = query.Where(x => x.Departament == (DepartamentType)filters.DepartamentDtoType);
            query = query.Where(x => x.GatewayEnum == (GatewayEnum)filters.GatewayEnumDto);

            return query.Select(x => new LocationDto()
                              {
                                  Name = x.Name,
                                  Value = x.Value
                              }).OrderBy(x => x.Name).ToList();
        }
    }
}
