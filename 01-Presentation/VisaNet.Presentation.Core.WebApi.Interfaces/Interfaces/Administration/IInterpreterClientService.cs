using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters.Base;

namespace VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration
{
    public interface IInterpreterClientService
    {
        Task<ICollection<InterpreterDto>> FindAll();
        Task<ICollection<InterpreterDto>> FindAll(BaseFilter filtersDto);
        Task<InterpreterDto> Find(Guid id);
        Task<InterpreterDto> Create(InterpreterDto bin);
        Task Edit(InterpreterDto bin);
        Task Delete(Guid id);
        Task<int> GetDataForInterpreterCount(BaseFilter filterDto);
    }
}
