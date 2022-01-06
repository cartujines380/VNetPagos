using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Common.Security.Entities;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters.Base;

namespace VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Web
{
    public interface IWebSystemUserClientService
    {
        Task<ICollection<SystemUserDto>> FindAll();
        Task<ICollection<SystemUserDto>> FindAll(BaseFilter filtersDto);
        Task<SystemUserDto> Find(Guid id);
        Task<SystemUserDto> Find(string username);
        Task Create(SystemUserDto entity);
        Task Edit(SystemUserDto entity);
        Task Delete(Guid id);

        Task<bool> ValidateUserInRole(ValidateUserInRoleDto entity);
        Task<bool> ValidateUser(ValidateUserDto entity);
        Task<IEnumerable<FunctionalityGroup>> GetPermissionsFromRoles(IEnumerable<Guid> roleIds);

    }
}
