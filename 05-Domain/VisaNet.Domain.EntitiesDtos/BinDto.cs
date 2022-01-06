using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Domain.EntitiesDtos
{
    public class BinDto
    {
        public Guid Id { get; set; }

        public Guid GatewayId { get; set; }

        [StringLength(50, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string GatewayName { get; set; }
        [StringLength(50, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string Name { get; set; }
        [MaxLength(250, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string Description { get; set; }

        [StringLength(2, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string Country { get; set; }

        public int Value { get; set; }

        public string ImageName { get; set; }
        public string ImageUrl { get; set; }
        public bool ImageDeleted { get; set; }

        public CardTypeDto CardType { get; set; }

        public Guid? BankDtoId { get; set; }
        public virtual BankDto BankDto { get; set; }

        public bool Active { get; set; }

        public bool EditedFromBO { get; set; }

        [MaxLength(6, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string IssuerBin { get; set; }

        [MaxLength(6, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string ProcessorBin { get; set; }

        public ICollection<BinGroupDto> BinGroups { get; set; }
        public virtual ICollection<BinAuthorizationAmountTypeDto> BinAuthorizationAmountTypeDtoList { get; set; }

        public Guid? AffiliationCardId { get; set; }
        public virtual AffiliationCardDto AffiliationCardDto { get; set; }
    }
}
