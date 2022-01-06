using System;

namespace VisaNet.Domain.EntitiesDtos
{
    public class HomePageDto
    {
        public Guid Id { get; set; }

        public Guid HomePageItem1Id { get; set; }
        public virtual HomePageItemDto HomePageItem1 { get; set; }

        public Guid HomePageItem2Id { get; set; }
        public virtual HomePageItemDto HomePageItem2 { get; set; }

        public Guid HomePageItem3Id { get; set; }
        public virtual HomePageItemDto HomePageItem3 { get; set; }
    }
}
