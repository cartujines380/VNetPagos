using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VisaNet.Common.Security.Entities.Security;
using VisaNet.Domain.Entities.Base.Audit;
using VisaNet.Domain.Entities.Enums;

namespace VisaNet.Domain.Entities
{
    [Table("SystemUsers")]
    [TrackChanges]
    public class SystemUser : LDAPBaseUser, IAuditable
    {
        public SystemUserType SystemUserType { get; set; }

        [MaxLength(50)]
        public string CreationUser { get; set; }
        [MaxLength(50)]
        public string LastModificationUser { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastModificationDate { get; set; }
    }
}
