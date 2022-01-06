using System;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Presentation.Administration.CustomAttributes;

namespace VisaNet.Presentation.Administration.Models
{
    public class BinAuthorizationAmountModel
    {
        public Guid Id { get; set; }
        
        [CustomDisplay("Bin_AuthorizationAmountTypeDto")]
        public int AuthorizationAmountTypeDto { get; set; }
        
        [CustomDisplay("Bin_DiscountTypeDto")]
        public DiscountTypeDto LawDto { get; set; }
        public Guid BinId { get; set; }

        public string Label { get; set; }
    }
}
