namespace VisaNet.Domain.Entities.Enums
{
    public enum ConciliationRunState
    {
        Started = 1,
        CompletedOk = 2,
        CompletedWithErrors = 3,
        TerminatedWithException = 4
    }
}