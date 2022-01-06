using System.Collections.Generic;

namespace VisaNet.Domain.EntitiesDtos
{
    public class CardMigrationTestDto
    {
        public List<ServiceAssociatedDto> FailedServices { get; set; }
        public List<ServiceAssociatedDto> SuccessfulServices { get; set; }
    }
}