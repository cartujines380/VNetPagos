namespace VisaNet.Domain.EntitiesDtos.Enums
{
    public enum ConciliationRunStateDto
    {
        Started = 1,
        CompletedOk = 2,
        CompletedWithErrors = 3,
        TerminatedWithException = 4
    }
}