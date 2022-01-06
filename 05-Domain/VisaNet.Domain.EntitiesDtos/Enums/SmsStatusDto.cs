namespace VisaNet.Domain.EntitiesDtos.Enums
{
    /// <summary>
    /// Basado en los status de twilo. No cambiar los nombres 
    /// </summary>
    public enum SmsStatusDto
    {
        New = 0,
        Accepted = 1,
        Queued = 2,
        Sending = 3,
        Sent = 4,
        Delivered = 5,
        Failed =6,
    }
}
