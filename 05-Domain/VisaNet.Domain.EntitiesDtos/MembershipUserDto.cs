using System;
using VisaNet.Domain.Entities.Base.Base;
using VisaNet.Domain.EntitiesDtos.CustomAttributes;

namespace VisaNet.Domain.EntitiesDtos
{
    public class MembershipUserDto : EntityBase
    {
        public override Guid Id { get; set; }

        public DateTime? LastAttemptToLogIn { get; set; }
        [CustomDisplay("MembershipUserDto_FailLogInCount")]
        public int FailLogInCount { get; set; }
        public DateTime? LastResetPassword { get; set; }
        public string ResetPasswordToken { get; set; }
        public string ConfirmationToken { get; set; }

        [CustomDisplay("MembershipUserDto_Active")]
        public bool Active { get; set; }
        public bool Blocked { get; set; }
        public bool PasswordHasBeenChangedFromBO { get; set; }
    }
}
