using System;
using VisaNet.Domain.EntitiesDtos.CustomAttributes;

namespace VisaNet.Presentation.Administration.Models
{
    public class HomePageModel
    {
        public Guid Id { get; set; }

        public Guid HomePageItem1Id { get; set; }
        [CustomDisplay("HomePageDto_HomePageItem1")]
        public HomePageItemModel HomePageItem1 { get; set; }

        public Guid HomePageItem2Id { get; set; }
        [CustomDisplay("HomePageDto_HomePageItem2")]
        public HomePageItemModel HomePageItem2 { get; set; }

        public Guid HomePageItem3Id { get; set; }
        [CustomDisplay("HomePageDto_HomePageItem3")]
        public HomePageItemModel HomePageItem3 { get; set; }
    }
}
