using System.Collections.Generic;
using VisaNet.Application.Interfaces.Base;
using VisaNet.Domain.Entities;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters;

namespace VisaNet.Application.Interfaces
{
    public interface IServiceLocation : IService<Location, LocationDto>
    {
        IEnumerable<LocationDto> GetDataForTable(LocationFilterDto filters);
        IEnumerable<LocationDto> GetDataForList(LocationFilterDto filters);
    }
}
