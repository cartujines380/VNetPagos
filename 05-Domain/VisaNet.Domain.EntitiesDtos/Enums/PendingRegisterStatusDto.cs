namespace VisaNet.Domain.EntitiesDtos.Enums
{
    public enum PendingRegisterStatusDto
    {
        Success = 1,
        ErrorControlled = 2,
        ErrorRetry = 3,
        ErrorDefinitive = 4,
    }
}