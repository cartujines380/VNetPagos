namespace VisaNet.Domain.Entities.Enums
{
    public enum DebitRequestState
    {
        Pending = 1,
        Synchronized = 2,
        Accepted = 3,
        Rejected = 4,
        Cancelled = 5,
        ManualSynchronization = 6,
        PendingCancellation = 7,
        AcceptedCancellation = 8,
        RejectedCancellation = 9
    }
}
