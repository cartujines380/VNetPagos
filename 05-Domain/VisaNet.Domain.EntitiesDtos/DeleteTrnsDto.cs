using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisaNet.Domain.EntitiesDtos
{
    public class DeleteTrnsDto
    {
        public Guid UserId { get; set; }
        public Guid CardId { get; set; }
        public bool Notify { get; set; }
        public string TransactionNumber { get; set; }
    }
}
