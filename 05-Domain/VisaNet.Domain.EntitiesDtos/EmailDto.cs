using System.ComponentModel.DataAnnotations;
using VisaNet.Common.Resource.Presentation;

namespace VisaNet.Domain.EntitiesDtos
{
    public class EmailDto
    {
        [StringLength(50, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(PresentationCoreMessages))]
        public string EmailAddress { get; set; }
    }
}
