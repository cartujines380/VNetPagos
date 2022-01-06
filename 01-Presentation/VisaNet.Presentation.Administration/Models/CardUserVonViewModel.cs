using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VisaNet.Presentation.Administration.Models
{
    public class CardUserVonViewModel
    {
        public Guid Id { get; set; }
        public string CardName { get; set; }
        public string CardMaskedNumber { get; set; }
        public string CardDueDate { get; set; }
        public string CreationDate { get; set; }
    }
}