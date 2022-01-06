namespace VisaNet.Domain.EntitiesDtos.Enums
{
    public enum RedirectEnums
    {
        PrivateAssosiate = 1,
        Payment = 2,
        PrivateAddCardToUser = 7,
        HighwayAdmission = 9,
        AppAdmission = 10,
        TestCsSecureAcceptance = 11,
        VisanetMobileAddCard = 13,
        VisanetMobilePayment = 15,

        VisaNetOnPaymentAnonymous = 16,
        VisaNetOnPaymentNewUser = 17,
        VisaNetOnPaymentRegisteredWithToken = 18,
        VisaNetOnPaymentRegisteredNewToken = 19,
        VisaNetOnPaymentRecurrentWithToken = 20,
        VisaNetOnPaymentRecurrentNewToken = 21,
        VisaNetOnTokenizationNewUser = 22,
        VisaNetOnTokenizationRegistered = 23,
        VisaNetOnTokenizationRecurrent = 24,

        Debit = 30,
    }
}