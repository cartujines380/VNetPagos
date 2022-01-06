namespace VisaNet.Domain.EntitiesDtos.Enums
{
    public enum ExternalRequestTypeDto
    {
        WebhookDown = 1,
        WebhookNewAssociation = 2,
        WebhookRegistration = 3,
        WsBillPaymentOnline = 4,
        WsBillQuery = 5,
        WsCommerceQuery = 6,
        WsPaymentCancellation = 7,
        WsCardRemove = 8
    }
}