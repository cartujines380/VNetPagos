using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisaNet.Domain.EntitiesDtos
{
    public class CardMigrationDebitsDto
    {
        public Guid ApplicationUserId { get; set; }
        public Guid OldCardId { get; set; }
        public Guid NewCardId { get; set; }
    }
}
