namespace VisaNet.Domain.EntitiesDtos.Enums
{
    public enum SystemVersionDto
    {
        //Webs
        AdminVersion = 101,
        WebVersion = 102,
        VonRegisterVersion = 103,
        VisaNetOnVersion = 104,
        CustomerSiteVersion = 105,

        //Public-APIs
        NotificationPublicApiVersion = 201,
        LifPublicApiVersion = 202,

        //Private-APIs
        CoreApiVersion = 301,
        LifApiVersion = 302,
        CustomerSiteApiVersion = 303,

        //Processes
        PaymentProcessVersion = 401,
        NotificationProcessVersion = 402,
        ConciliationProcessVersion = 403,
        CsAckProcessVersion = 404,
        DebitSynchronizationProcessVersion = 405,

    }
}