using System.ComponentModel.DataAnnotations;

namespace VisaNet.Presentation.VisaNetOn.Models
{
    public class AnonymousUserPageModel
    {
        [Required(AllowEmptyStrings = false)]
        [MaxLength(50, ErrorMessage = "El largo máximo permitido son 50 letras (contando espacios vacios)")]
        public string Name { get; set; }
        [Required(AllowEmptyStrings = false)]
        [MaxLength(50, ErrorMessage = "El largo máximo permitido son 50 letras (contando espacios vacios)")]
        public string Surname { get; set; }
        [EmailAddress]
        [Required(AllowEmptyStrings = false)]
        [MaxLength(50, ErrorMessage = "El largo máximo permitido son 50 letras (contando espacios vacios)")]
        public string Email { get; set; }
        [Required(AllowEmptyStrings = false)]
        [MaxLength(50, ErrorMessage = "El largo máximo permitido son 50 letras (contando espacios vacios)")]
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Mobile { get; set; }
        public string Identity { get; set; }

    }
}