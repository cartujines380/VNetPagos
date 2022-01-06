using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Domain.EntitiesDtos
{
    public class SystemUserDto
    {
        public SystemUserDto()
        {
            Roles = new List<RoleDto>();
            Enabled = true;
        }
        public Guid Id { get; set; }

        [StringLength(50, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string LDAPUserName { get; set; }
        public bool Enabled { get; set; }

        public IEnumerable<RoleDto> Roles { get; set; }
        public SystemUserTypeDto SystemUserType { get; set; }
    }
}
