namespace VisaNet.Domain.Entities.Enums
{
    public enum PaymentResultType
    {
        Success = 100,
        NoBills = 90,

        //Validations
        InvalidCardToken = 10,
        InvalidCardBin = 11,
        InvalidCardDueDate = 12,
        AutomaticPaymentDisabled = 13,
        ServiceNotAllowsAutomaticPayment = 14,
        ExceededPaymentsCount = 15,
        ExceededPaymentsAmount = 16,

        //Payment
        AmountIsZeroError = 20,
        GatewayNotificationError = 21,
        PaymentGeneralError = 22,
        InvalidModel = 23,
        BankDontAllowQuota = 24,
        BinNotValidForService = 25,
        DiscountCalculationError = 26,

        //Exceptions
        UnhandledException = 30,
        GetBillsException = 31,

        //Service
        ServiceErrorRetry = 40,
        ServiceErrorControlled = 41,
        ServiceErrorDeleteAutomaticPayment = 42,

        //Bill
        BillOk = 50,
        BillExpired = 51,

        //Cybersource
        CsInvalidData = 102,
        CsInvalidAccessKey = 104,
        CsPartialAmount = 110,
        CsGeneralSystemFailure = 150,
        CsServerTimeout = 151,
        CsServiceTimeout = 152,
        CsAddressVerificationFailure = 200,
        CsIssuiungBankRequestDeclined = 201,
        CsExpiredCard = 202,
        CsCardGeneralDeclined = 203,
        CsInsufficientFunds = 204,
        CsStolenCard = 205,
        CsIssuiungBankUnavailable = 207,
        CsInactiveCard = 208,
        CsCardCreditLimitReached = 210,
        CsCardInvalidCVN = 211,
        CsCustomerDeclined = 221,
        CsAccountFrozen = 222,
        CsCardInvalidCVNByCybersource = 230,
        CsInvalidAccountNumber = 231,
        CsCardTypeNotAccepted = 232,
        CsGeneralDecline = 233,
        CsAccountInformationProblem = 234,
        CsProcessorFailure = 236,
        CsCardTypeInvalid = 240,
        CsCardholderAuthenticationNeeded = 475,
        CsPayerAuthenticationFail = 476,
        CsDecisionManagerError = 520

    }
}
