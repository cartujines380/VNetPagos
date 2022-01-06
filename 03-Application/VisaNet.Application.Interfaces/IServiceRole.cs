using System;
using System.Collections.Generic;
using VisaNet.Application.Interfaces.Base;
using VisaNet.Common.Security.Entities;
using VisaNet.Common.Security.Entities.Security;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using Action = VisaNet.Common.Security.Entities.Action;

namespace VisaNet.Application.Interfaces
{
    public interface IServiceRole : IService<Role, RoleDto>
    {
        IEnumerable<RoleDto> GetDataForTable(RoleFilterDto filters);
        IEnumerable<FunctionalityGroup> GetFunctionalityGroupsNoTracking();
        IEnumerable<Action> GetActionNoTracking();
        IEnumerable<FunctionalityGroup> GetFunctionalityGroupsFromRoles(IEnumerable<Guid> roleIds);

    }
}
