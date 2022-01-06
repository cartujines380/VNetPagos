using System;

namespace VisaNet.Presentation.Web.Models
{
    public class ServiceModel
    {
        public Guid Id { get; set; }
        public Guid CommerceId { get; set; }

        public string ImgUrl { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ServiceCategoryName { get; set; }
        public Guid ServiceCategoryId { get; set; }
        public int Sort { get; set; }

        public bool EnableDebit { get; set; }
        public bool EnableBill { get; set; }
    }
}