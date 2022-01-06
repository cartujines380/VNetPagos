using System;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Domain.EntitiesDtos
{
    public class BinAuthorizationAmountTypeDto
    {
        public Guid Id { get; set; }
        public AuthorizationAmountTypeDto AuthorizationAmountTypeDto { get; set; }
        public DiscountTypeDto LawDto { get; set; }

        public Guid BinId { get; set; }
        public BinDto BinDto { get; set; }

    }
}
