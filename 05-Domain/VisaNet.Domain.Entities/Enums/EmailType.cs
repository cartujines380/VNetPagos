namespace VisaNet.Domain.Entities.Enums
{
    public enum EmailType
    {
        NewUser = 0,
        ResetPassword = 1,
        ContactForm = 2,

        NewBill = 3,
        NewPayment = 4,
        CopyPayment = 5,

        ServiceDeletedNotification = 6,

        ExpiringCard = 7,
        PaymentCancellationError = 8,

        BinNotDefined = 9,

        GeneralNotification = 10,
        InternalErrorNotification = 11,
        InternalGeneralNotification = 12,

        AutomaticPaymentNotification = 13,

        BillAboutToExpired = 14,
        BillExpired = 15,

        CybersourceError = 16,
        PaymentDoneCancellation = 17,

        HighwayProcessed = 18,
        HighwayError = 19,
        HighwayRejected = 20,
        HighwayTransactionReportsOk = 21,
        ExtractBanred = 22,
        ExtractImporte = 23,
        ExtractGeocom = 24,

        BinFileProcessed = 25,

        UserAutomaticPaymentNotification = 26,
        UserBillNotification = 27,

        NewUserRequestPassword = 28,

        ConciliationResults = 29,

        CustomerSiteSystemUserCreation = 30,
        CustomerSiteBillEmail = 31,
        CustomerSiteResetPassword = 32,

        DebitSuscriptionNotification = 33,
        DeleteCustomerCard = 34
    }
}