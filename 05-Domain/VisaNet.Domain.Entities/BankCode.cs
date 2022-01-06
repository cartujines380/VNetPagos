using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VisaNet.Domain.Entities
{
    [ComplexType]
    public class BankCode
    {
        [MaxLength(50)]
        public string Code { get; set; }
    }
}
