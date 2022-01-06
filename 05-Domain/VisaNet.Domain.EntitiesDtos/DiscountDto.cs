using System;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Domain.EntitiesDtos
{
    public class DiscountDto
    {
        public Guid Id { get; set; }

        public CardTypeDto CardType { get; set; }

        public DateTime From { get; set; }

        public DateTime To { get; set; }

        public int Fixed { get; set; }

        public int Additional { get; set; }

        public int MaximumAmount { get; set; }

        public DiscountTypeDto DiscountType { get; set; }

        public DiscountLabelTypeDto DiscountLabel { get; set; }
        
        public string DiscountLawDescription { get; set; }

        public int GetTotalDiscount()
        {
            return Fixed + Additional;
        }
    }
}