using VisaNet.Domain.EntitiesDtos;
using VisaNet.Presentation.Administration.Models;

namespace VisaNet.Presentation.Administration.Mappers
{
    public static class RoleMapper
    {
        public static RoleDto ToDto(this RoleModel entity)
        {
            return new RoleDto
            {
                Id = entity.Id,
                Name = entity.Name,
                ActionsIds = entity.ActionsIds,
            };
        }

        public static RoleModel ToModel(this RoleDto entity)
        {
            return new RoleModel
            {
                Id = entity.Id,
                Name = entity.Name,
                ActionsIds = entity.ActionsIds,
            };
        }
    }
}