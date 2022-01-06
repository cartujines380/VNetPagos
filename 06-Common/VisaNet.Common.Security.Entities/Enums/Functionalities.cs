namespace VisaNet.Common.Security.Entities.Enums
{
    public enum Functionalities
    {
        #region Administration
        ServiceCategories = 10,
        Services = 11,
        Contacts = 13,
        Parameters = 14,
        Bins = 21,
        ServiceContainers = 25,
        BinGroup = 80,
        Bank = 26,
        Interpreters = 27,
        AffiliationCard = 28,
        #endregion

        #region PublicPortal
        Faqs = 15,
        HomePage = 16,
        InstitutionalContent = 17,
        LegalPages = 18,
        Notices = 19,
        Subscribers = 20,
        Help = 23,
        Promotion = 24,
        Discounts = 22,
        #endregion

        #region Reports
        ReportsServicesAssociated = 30,
        ReportsTransactions = 31,
        ReportsDashboard = 32,
        ReportsTransactionsAmount = 33,
        ReportsCybersourceTransactions = 34,
        ReportsLifApiBill = 35,
        ReportsCards = 36,
        ReportsTc33 = 37,
        ReportsHighwayEmail = 38,
        ReportsHighwayBill = 39,
        ReportsIntegration = 45,
        ReportsAutomaticPayments = 46,
        ReportsVersion = 47,
        ReportsMails = 48,
        ReportsUsers = 49,
        ReportsVisaNetOn = 53,
        ReportsVonUsers = 200,
        #endregion

        #region Security
        Roles = 40,
        SystemUsers = 41,
        #endregion

        #region Audit
        Audit = 50,
        AuditChangeLog = 51,
        #endregion

        #region Notifications
        Notifications = 60,
        #endregion

        #region Conciliation
        ConciliationDaily = 70,
        ReportsConciliation = 71,
        ConciliationVNP = 72,
        ReportsConciliationRun = 73,
        #endregion

        #region CustomerSite
        CustomerSiteCommerce = 90,
        CustomerSiteBranch = 91,
        CustomerSiteSystemUser = 92,
        #endregion

        #region Debit
        DebitCommerce = 81,
        ReportsDebitsSuscription = 82,
        #endregion

    }
}