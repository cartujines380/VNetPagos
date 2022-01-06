using VisaNet.Domain.EntitiesDtos.ExternalRequest;

namespace VisaNet.Domain.EntitiesDtos
{
    public class CybersourceCreateAppAssociationDto
    {
        public CybersourceCreateServiceAssociatedDto CybersourceCreateServiceAssociatedDto{ get; set; }
        public WebhookRegistrationDto WebhookRegistrationDto { get; set; }
    }
}
