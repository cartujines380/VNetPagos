using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VisaNet.Presentation.VisaNetOn.Models
{
    public class Register
    {
        [EmailAddress]
        [Required(AllowEmptyStrings = false)]
        [MaxLength(50, ErrorMessage = "El largo máximo permitido son 50 letras (contando espacios vacios)")]
        public string Email { get; set; }
        public Guid? RegisteredUserId { get; set; }
        public Guid? AnonymousUserId { get; set; }
        public bool NewUser { get; set; }
        public bool RememberUser { get; set; }

        public IList<RegisteredCard> CardList { get; set; }
        public Guid SelectedCard { get; set; }
        public int CardBin { get; set; }
        public bool NewCard { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }

        public Guid ServiceId { get; set; }
        public string ServiceName { get; set; }

        public string BillNumber { get; set; }

    }
}