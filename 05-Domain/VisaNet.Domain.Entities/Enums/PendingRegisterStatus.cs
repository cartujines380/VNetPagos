namespace VisaNet.Domain.Entities.Enums
{
    public enum PendingRegisterStatus
    {
        Success = 1,
        ErrorControlled = 2,
        ErrorRetry = 3,
        ErrorDefinitive = 4,
    }
}
