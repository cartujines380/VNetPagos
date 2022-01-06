namespace VisaNet.Domain.Entities.Enums
{
    public enum TransactionType
    {
        Payment = 1,
        Void = 2,
        Refund = 3,
        Reverse = 4,
        CardToken = 5,
        CardDelete = 6
    }
}
