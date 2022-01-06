using System;

namespace VisaNet.Domain.EntitiesDtos
{
    public class PromotionDto
    {
        public Guid Id { get; set; }

        public bool Active { get; set; }
        public string Name { get; set; }

        public string ImageName { get; set; }
        public string ImageUrl { get; set; }
    }
}