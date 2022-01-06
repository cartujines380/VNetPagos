namespace VisaNet.Common.Logging.Entities
{
    public enum LogOperationType
    {
        BillPayment = 0,
        LogInFail = 1,
        UserCreation = 2,
        UserEdit = 3,
        UserChangePassword = 4,
        UserResetPassword = 5,
        ServiceAssociated = 6,
        AutomaticServiceAssociated = 7,
        NewCardAdded = 8,
        ServiceAssociatedBadCardConfiguration = 9,
        ServiceAssociatedBadBinNumber = 10,
        ServiceAssociatedPaymentException = 11,
        DiscountCalculation = 12,
        AutomaticPaymentBatch = 13,
        Cybersource = 14,
        DebitPaymentBatch = 15,
        WebServicePayment = 16,
        Webhooks = 17,
        Void = 18,
        Refund = 19,
        Reverse = 20,
        ServiceAssociatedElimination = 21,
        WebServiceCardRemove = 22,
        UserBlocked = 23,
    }
}