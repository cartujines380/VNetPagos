using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Common.Security.Entities;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters.Base;
using Action = VisaNet.Common.Security.Entities.Action;

namespace VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration
{
    public interface IRoleClientService
    {
        Task<ICollection<RoleDto>> FindAll();
        Task<ICollection<RoleDto>> FindAll(BaseFilter filtersDto);
        Task<RoleDto> Find(Guid id);
        Task Create(RoleDto serviceCategory);
        Task Edit(RoleDto serviceCategory);
        Task Delete(Guid id);

        Task<IEnumerable<FunctionalityGroup>> GetFunctionalityGroups();
        Task<IEnumerable<Action>> GetActions();
    }
}
