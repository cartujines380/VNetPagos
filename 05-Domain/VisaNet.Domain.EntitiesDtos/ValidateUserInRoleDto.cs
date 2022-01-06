using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Domain.EntitiesDtos
{
    public class ValidateUserInRoleDto
    {
        public string UserName { get; set; }
        public SystemUserTypeDto SystemUserTypeDto { get; set; }
    }
}
