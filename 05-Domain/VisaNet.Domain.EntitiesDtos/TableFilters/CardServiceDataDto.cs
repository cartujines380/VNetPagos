using System;

namespace VisaNet.Domain.EntitiesDtos.TableFilters
{
    public class CardServiceDataDto
    {
        public Guid ServiceId { get; set; }
        public Guid CardId { get; set; }
        public Guid UserId { get; set; }
        public string OperationId { get; set; } 
    }
}
