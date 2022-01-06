using System;
using System.Collections.Generic;

namespace VisaNet.CustomerSite.EntitiesDtos
{
    public class CustomerSiteMembershipUserDto
    {
        public Guid Id { get; set; }

        public DateTime? LastAttemptToLogIn { get; set; }
        public int FailLogInCount { get; set; }
        public DateTime? LastResetPassword { get; set; }
        
        public bool Active { get; set; }
        public bool Blocked { get; set; }
        public bool PasswordHasBeenChangedFromBo { get; set; }
        public string ConfirmationToken { get; set; }
    }
}
