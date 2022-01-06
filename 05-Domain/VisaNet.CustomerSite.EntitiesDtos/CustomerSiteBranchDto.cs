using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisaNet.CustomerSite.EntitiesDtos
{
    public class CustomerSiteBranchDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhoneNumber { get; set; }
        public string ContactAddress { get; set; }

        public bool Disabled { get; set; }

        public Guid? SystemUserId { get; set; }
        public virtual CustomerSiteSystemUserDto SystemUserDto { get; set; }

        public string CommerceId { get; set; }
        public CustomerSiteCommerceDto CommerceDto { get; set; }

        public string ServiceId { get; set; }
    }
}
