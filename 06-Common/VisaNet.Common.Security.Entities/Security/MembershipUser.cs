using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VisaNet.Domain.Entities.Base.Audit;
using VisaNet.Domain.Entities.Base.Base;

namespace VisaNet.Common.Security.Entities.Security
{
    [Table("MembershipUsers")]
    public class MembershipUser : EntityBase
    {
        [Key]
        public override Guid Id { get; set; }

        public string Password { get; set; }
        public string PasswordSalt { get; set; }

        public DateTime? LastAttemptToLogIn { get; set; }
        public int FailLogInCount { get; set; }

        public DateTime? LastResetPassword { get; set; }
        public string ResetPasswordToken { get; set; }

        public string ConfirmationToken { get; set; }

        public bool Active { get; set; }

        public bool Blocked { get; set; }

        public bool PasswordHasBeenChangedFromBO { get; set; }
    }
}
