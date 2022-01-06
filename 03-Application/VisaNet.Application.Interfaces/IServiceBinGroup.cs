using System;
using System.Collections.Generic;
using VisaNet.Application.Interfaces.Base;
using VisaNet.Domain.Entities;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters;

namespace VisaNet.Application.Interfaces
{
    public interface IServiceBinGroup : IService<BinGroup, BinGroupDto>
    {
        IEnumerable<BinGroupDto> GetDataForTable(BinGroupFilterDto filters);
        BinGroupDto GetById(Guid id);
    }
}
