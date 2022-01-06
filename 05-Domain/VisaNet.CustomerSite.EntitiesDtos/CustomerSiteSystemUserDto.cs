using System;
using System.Collections.Generic;

namespace VisaNet.CustomerSite.EntitiesDtos
{
    public class CustomerSiteSystemUserDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        
        public bool Disabled { get; set; }
        public bool Master { get; set; }
        public bool SendEmailActivation { get; set; }
        
        public CustomerSiteCommerceDto CommerceDto { get; set; }
        public CustomerSiteMembershipUserDto MembershipIdentifierObj { get; set; }
        public IList<CustomerSiteBranchDto> BranchesDto { get; set; }
    }
}
