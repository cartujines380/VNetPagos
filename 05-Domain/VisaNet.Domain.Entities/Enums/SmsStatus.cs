namespace VisaNet.Domain.Entities.Enums
{
    public enum SmsStatus
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
