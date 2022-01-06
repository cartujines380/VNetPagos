using System.Collections.Generic;
using VisaNet.Application.Interfaces.Base;
using VisaNet.Common.Security.Entities.Enums;
using VisaNet.Domain.Entities;
using VisaNet.Domain.Entities.Enums;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters;

namespace VisaNet.Application.Interfaces
{
    public interface IServiceSystemUser : IService<SystemUser, SystemUserDto>
    {
        IEnumerable<SystemUserDto> GetDataForTable(SystemUserFilterDto filters);
        bool ValidateUserInRole(string username, SystemUserType systemUserType);
        bool ValidateUserAction(string username, Actions action);
        bool ValidateUser(string userName, string password);
        SystemUserDto GetUserByUserName(string username);
        string GetEmailByUserName(string username);
    }
}
