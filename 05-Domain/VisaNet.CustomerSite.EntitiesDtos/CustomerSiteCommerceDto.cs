using System;
using System.Collections.Generic;
using VisaNet.CustomerSite.EntitiesDtos.Debit;

namespace VisaNet.CustomerSite.EntitiesDtos
{
    public class CustomerSiteCommerceDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhoneNumber { get; set; }
        public string ContactAddress { get; set; }

        public string ServiceId { get; set; }

        public bool Disabled { get; set; }

        public int? DebitmerchantId { get; set; }
        public int? DebitMerchantGroupId { get; set; }
        public string DebitMerchantGroupName { get; set; }

        public string ImageName { get; set; }

        public string ImageBlobName
        {
            get { return string.IsNullOrEmpty(ImageName) ? string.Empty : string.Format("{0}{1}", Id.ToString(), ImageName.Substring(ImageName.LastIndexOf("."))); }
        }
        public string ImageUrl { get; set; }

        public Guid? CommerceCathegoryId { get; set; }
        public CommerceCathegoryDto CommerceCathegoryDto { get; set; }

        public int Sort { get; set; }

        public ICollection<CustomerSiteBranchDto> BranchesDto { get; set; }

        public ICollection<ProductDto> ProductosListDto { get; set; }        
    }
}