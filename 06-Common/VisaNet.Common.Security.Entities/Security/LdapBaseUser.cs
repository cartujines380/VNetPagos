using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using VisaNet.Domain.Entities.Base.Base;

namespace VisaNet.Common.Security.Entities.Security
{    
    [TrackChanges]
    public abstract class LDAPBaseUser : EntityBase
    {
        protected LDAPBaseUser()
        {
            Roles = new Collection<Role>();
        }
        [Key]
        public override Guid Id { get; set; }

        [MaxLength(50)]
        [TrackChangesAditionalInfo(Index = 0)]
        public string LDAPUserName { get; set; }
        
        public DateTime? LastAttemptToLogIn { get; set; }
        public int FailLogInCount { get; set; }

        public DateTime? LastResetPassword { get; set; }

        public string ResetPasswordToken { get; set; }

        public bool Enabled { get; set; }

        public virtual ICollection<Role> Roles { get; set; }
    }
}
