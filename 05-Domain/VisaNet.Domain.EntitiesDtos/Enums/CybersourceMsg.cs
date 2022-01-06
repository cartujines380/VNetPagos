namespace VisaNet.Domain.EntitiesDtos.Enums
{
    public enum CybersourceMsg
    {
        Accepted = 100,
        InvalidFields = 102,
        ConfigurationKeysInvalids = 104,
        PartialAmmountAproved = 110,
        CardIssuer = 150,
        AVSCheckInvalid = 200,
        AuthorizationRejected = 201,
        ExpiredCard = 202,
        GeneralDecline = 203,
        InsufficientFunds = 204,
        StolenLostCard = 205,
        BankUnavailable = 207,
        InactiveUnAuthorizedCard = 208,
        CreditLimitReached = 210,
        InvalidCVN = 211,
        AccountFrozen = 222,
        CVNCheckInvalid = 230,
        InvalidAccountNumber = 231,
        CardTypeNotAccepted = 232,
        GeneralDeclineByProcessor = 233,
        UserCybersourceError = 234,
        ProcessorFailure = 236,
        InvalidCardTypeOrNotCorrelateWithCardNumber = 240,
        PayerAuthenticationError = 475,
        PayerAuthenticationNotAuthenticated = 476,
        DecisionManager = 481,
        AuthorizationDeclinedByCyberSourceSmartAuthorizationSettings = 520,
        
    }
}