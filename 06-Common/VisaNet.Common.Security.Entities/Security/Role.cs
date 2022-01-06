using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VisaNet.Domain.Entities.Base.Audit;
using VisaNet.Domain.Entities.Base.Base;

namespace VisaNet.Common.Security.Entities.Security
{
    [Table("Roles")]
    [TrackChanges]
    public class Role : EntityBase, IAuditable
    {
        public Role()
        {
            Actions = new Collection<Action>();
            SystemUsers = new Collection<LDAPBaseUser>();
        }

        [Key]
        public override Guid Id { get; set; }

        public string Name { get; set; }

        public virtual ICollection<Action> Actions { get; set; }

        public virtual ICollection<LDAPBaseUser> SystemUsers { get; set; }

        public string CreationUser { get; set; }
        public string LastModificationUser { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastModificationDate { get; set; }
    }
}
