using System;
using System.Collections.Generic;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Domain.EntitiesDtos
{
    public class DebitRequestSyncDto
    {
        public Guid Id { get; set; }
        //public string UserFullName { get; set; }
        public int MerchantGroupId { get; set; }
        public int MerchantId { get; set; }
        public int MerchantProductId { get; set; }
        public DebitRequestTypeDto Type { get; set; }
        public string CardNumber { get; set; }
        public int CardMonth { get; set; }
        public int CardYear { get; set; }
        public UserSyncDto User { get; set; }

        public IList<DebitRequestReferenceDto> References { get; set; }
    }

    public class UserSyncDto
    {
        public string FullName { get; set; }
        public string IdentityNumber { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
    }
}
