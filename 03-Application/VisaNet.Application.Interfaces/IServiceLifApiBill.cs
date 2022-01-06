using System.Collections.Generic;
using VisaNet.Application.Interfaces.Base;
using VisaNet.Domain.Entities;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters;

namespace VisaNet.Application.Interfaces
{
    public interface IServiceLifApiBill : IService<LifApiBill, LifApiBillDto>
    {

        List<LifApiBillDto> GetDataForTable(LifApiBillFilterDto filterDto);
        int GetDataForLifApiBillCount(LifApiBillFilterDto filterDto);
    }
}
