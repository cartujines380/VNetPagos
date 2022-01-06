using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VisaNet.Domain.Entities
{
    [ComplexType]
    public class Email
    {
        [MaxLength(50)]
        public string EmailAddress { get; set; }
    }
}
