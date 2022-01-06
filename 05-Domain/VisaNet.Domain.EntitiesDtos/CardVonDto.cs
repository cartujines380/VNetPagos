using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisaNet.Domain.EntitiesDtos
{
    public class CardVonDto
    {
        public Guid Id { get; set; }
        public string CardName { get; set; }
        public string CardMaskedNumber { get; set; }
        public DateTime CardDueDate { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
