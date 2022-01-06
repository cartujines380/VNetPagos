using System.Linq;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Presentation.Administration.Models;

namespace VisaNet.Presentation.Administration.Mappers
{
    public static class SystemUserMapper
    {
        public static SystemUserDto ToDto(this SystemUserModel entity)
        {
            return new SystemUserDto
            {
                Id = entity.Id,
                LDAPUserName = entity.LDAPUserName,
                Enabled = entity.Enabled,
                SystemUserType = (SystemUserTypeDto)entity.SystemUserTypeId,
                Roles = entity.Roles.Select(r => new RoleDto { Id = r.Id }).ToList(),
            };
        }

        public static SystemUserModel ToModel(this SystemUserDto entity)
        {
            return new SystemUserModel
            {
                Id = entity.Id,
                LDAPUserName = entity.LDAPUserName,
                Enabled = entity.Enabled,
                SystemUserTypeId = (int)entity.SystemUserType,
                Roles = entity.Roles.Select(r => new RoleModel { Id = r.Id }).ToList(),
            };
        }
    }
}