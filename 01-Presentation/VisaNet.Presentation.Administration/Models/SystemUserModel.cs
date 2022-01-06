using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VisaNet.Domain.EntitiesDtos.CustomAttributes;

namespace VisaNet.Presentation.Administration.Models
{
    public class SystemUserModel
    {
        public SystemUserModel()
        {
            Roles = new List<RoleModel>();
            Enabled = true;
        }

        public Guid Id { get; set; }

        [CustomDisplay("SystemUserDto_LDAPUserName")]
        [Required]
        public string LDAPUserName { get; set; }

        [CustomDisplay("SystemUserDto_SystemUserType")]
        [Required]
        public int SystemUserTypeId { get; set; }

        [CustomDisplay("SystemUserDto_Enabled")]
        [Required]
        public bool Enabled { get; set; }

        [CustomDisplay("SystemUserDto_Roles")]
        public IList<RoleModel> Roles { get; set; }
    }
}
