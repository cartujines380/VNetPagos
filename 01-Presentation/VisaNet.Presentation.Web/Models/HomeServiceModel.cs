using System.ComponentModel.DataAnnotations;
using VisaNet.Common.Resource.Presentation;

namespace VisaNet.Presentation.Web.Models
{
    public class HomeServiceModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string ServiceName { get; set; }

        public string serviceId { get; set; }

        public string CommerceId { get; set; }
    }

}