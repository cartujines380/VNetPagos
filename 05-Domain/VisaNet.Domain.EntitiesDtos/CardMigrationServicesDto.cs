using System;

namespace VisaNet.Domain.EntitiesDtos
{
    public class CardMigrationServicesDto
    {
        public Guid ApplicationUserId { get; set; }
        public Guid OldCardId { get; set; }
        public Guid NewCardId { get; set; }
    }
}
