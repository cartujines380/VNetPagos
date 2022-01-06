using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VisaNet.Domain.Entities.ExternalRequest
{
    [ComplexType]
    public class UserDataInput
    {
        [MaxLength(50)]
        public string Email { get; set; }
        [MaxLength(50)]
        public string Name { get; set; }
        [MaxLength(50)]
        public string Surname { get; set; }
        [MaxLength(50)]
        public string IdentityNumber { get; set; }
        [MaxLength(50)]
        public string MobileNumber { get; set; }
        [MaxLength(50)]
        public string PhoneNumber { get; set; }
        [MaxLength(50)]
        public string Address { get; set; }
    }

    [ComplexType]
    public class BillDataInput
    {
        public string ExternalId { get; set; }
        public string Amount { get; set; }
        public string TaxedAmount { get; set; }
        public string Currency { get; set; }
        public string FinalConsumer { get; set; }
        public string GenerationDate { get; set; }
        public string Quota { get; set; }
        public string Description { get; set; }
        public string ExpirationDate { get; set; }
    }
}