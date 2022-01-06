namespace VisaNet.Common.Security.Entities.Enums
{
    public enum Actions
    {
        //#region [TEMPLATE]
        //XList = t00,
        //XCreate = t01,
        //XEdit = t02,
        //XDelete = t03,
        //XDetails = t04,
        //#endregion

        #region Roles
        RolesList = 400,
        RolesCreate = 401,
        RolesEdit = 402,
        RolesDelete = 403,
        RolesDetails = 404,
        #endregion

        #region SystemUsers
        SystemUsersList = 410,
        SystemUsersCreate = 411,
        SystemUsersEdit = 412,
        SystemUsersDelete = 413,
        SystemUsersDetails = 414,
        #endregion

        #region Service
        ServiceList = 110,
        ServiceCreate = 111,
        ServiceEdit = 112,
        ServiceDelete = 113,
        ServiceDetails = 114,
        ServiceTestGateways = 115,
        ServiceTestCybersource = 116,
        #endregion

        #region ServiceCategory
        ServiceCategoryList = 100,
        ServiceCategoryCreate = 101,
        ServiceCategoryEdit = 102,
        ServiceCategoryDelete = 103,
        ServiceCategoryDetails = 104,
        #endregion

        #region ServiceContainer
        ServiceContainerList = 250,
        ServiceContainerCreate = 251,
        ServiceContainerEdit = 252,
        ServiceContainerDelete = 253,
        ServiceContainerDetails = 254,
        ServiceContainerTestGateways = 255,
        ServiceContainerTestCybersource = 256,
        #endregion

        #region Contact
        ContactList = 120,
        ContactCreate = 121,
        ContactEdit = 122,
        ContactDelete = 123,
        ContactDetails = 124,
        #endregion

        #region Faq
        FaqList = 130,
        FaqCreate = 131,
        FaqEdit = 132,
        FaqDelete = 133,
        FaqDetails = 134,
        #endregion

        #region HomePage
        HomePageList = 140,
        HomePageEdit = 141,
        HomePageDetails = 142,
        #endregion

        #region InstitutionalContent
        InstitutionalContentEdit = 150,
        InstitutionalContentDetails = 151,
        #endregion

        #region LegalPages
        LegalPagesEdit = 160,
        LegalPagesDetails = 161,
        #endregion

        #region Notices
        NoticesEdit = 170,
        NoticesDetails = 171,
        #endregion

        #region Subscriber
        SubscriberList = 180,
        SubscriberCreate = 181,
        SubscriberEdit = 182,
        SubscriberDelete = 183,
        SubscriberDetails = 184,
        #endregion

        #region Parameters
        ParametersEdit = 190,
        ParametersDetails = 191,
        #endregion

        #region Discount
        DiscountList = 200,
        DiscountEdit = 201,
        DiscountDetails = 202,
        #endregion

        #region Bins
        BinsList = 210,
        BinsCreate = 211,
        BinsEdit = 212,
        BinsDelete = 213,
        BinsDetails = 214,
        #endregion

        #region Reports
        ReportsServicesAssociatedDetails = 220,
        ReportsTransactionsDetails = 221,
        ReportsDashboardDetails = 222,
        ReportsTransactionsAmountDetails = 223,
        ReportsCybersourceTransactionsDetails = 224,
        ReportsConciliationDetails = 225,
        ReportsCardsDetails = 226,
        ReportsTc33 = 227,
        ReportsHighwayEmails = 228,
        ReportsHighwayBills = 229,
        ReportsIntegration = 450,
        ReportsAutomaticPaymentsDetails = 451,
        ReportsVersionDetails = 452,
        ReportsMailsList = 480,
        //ReportsMailsCreate = 481,
        ReportsMailsResend = 482,
        ReportsMailsCancel = 483,
        ReportsMailsDetails = 484,
        ReportsMailsResendAll = 485,
        ReportsTransactionCancellation = 486,
        ReportsVisaNetOn = 487,
        #endregion

        #region web Users
        ReportsUsersList = 490,
        ReportsUsersCreate = 491,
        ReportsUsersEdit = 492,
        ReportsUsersDelete = 493,
        ReportsUsersDetails = 494,
        ReportsUsersChangePassword = 495,
        #endregion

        #region Von Users
        ReportsUsersVonList = 490,
        #endregion

        #region HelpPage
        HelpEdit = 232,
        HelpDetails = 233,
        #endregion

        #region Audit
        AuditList = 500,
        AuditDetails = 501,
        AuditChangeLog = 510,
        #endregion

        #region Promotion
        PromotionList = 240,
        PromotionCreate = 241,
        PromotionEdit = 242,
        PromotionDelete = 243,
        PromotionDetails = 244,
        #endregion

        #region Notifications
        NotificationsList = 600,
        NotificationsEdit = 601,
        NotificationsDetails = 602,
        #endregion

        #region Conciliation
        ConciliationDailyList = 700,
        ConciliationVNP = 720,
        ReportsConciliationRun = 730,
        #endregion

        #region BinGroup
        BinGroupList = 800,
        BinGroupCreate = 801,
        BinGroupEdit = 802,
        BinGroupDelete = 803,
        BinGroupDetails = 804,
        #endregion

        #region Banks
        BankList = 260,
        BankCreate = 261,
        BankEdit = 262,
        BankDelete = 263,
        BankDetails = 264,
        #endregion

        #region ReportsLifApiBill
        ReportsLifApiBillList = 350,
        ReportsLifApiBillDetails = 354,
        #endregion

        #region Interpreters 
        InterpreterList = 270,
        InterpreterCreate = 271,
        InterpreterEdit = 272,
        InterpreterDelete = 273,
        InterpreterDetails = 274,
        #endregion

        #region CustomerSite
        CustomerSiteCommerceList = 900,
        CustomerSiteCommerceCreate = 901,
        CustomerSiteCommerceEdit = 902,
        CustomerSiteCommerceDelete = 903,
        CustomerSiteCommerceDetails = 904,
        CustomerSiteCommerceEnable = 905,

        CustomerSiteBranchList = 910,
        CustomerSiteBranchCreate = 911,
        CustomerSiteBranchEdit = 912,
        CustomerSiteBranchDelete = 913,
        CustomerSiteBranchDetails = 914,
        CustomerSiteBranchEnable = 915,

        CustomerSiteSystemUserList = 920,
        CustomerSiteSystemUserCreate = 921,
        CustomerSiteSystemUserEdit = 922,
        CustomerSiteSystemUserDelete = 923,
        CustomerSiteSystemUserDetails = 924,
        CustomerSiteSystemUserEnable = 925,
        #endregion

        #region DebitCommerce

        #endregion

        #region Debit
        DebitCommerceList = 810,
        //DebitCommerceCreate = 811,
        DebitCommerceEdit = 812,
        //DebitCommerceDelete = 813,
        DebitCommerceDetails = 814,
        DebitSuscriptionList = 820,
        DebitSuscriptionDetails = 824,
        #endregion

        #region AffiliationCard
        AffiliationCardList = 280,
        AffiliationCardCreate = 281,
        AffiliationCardEdit = 282,
        AffiliationCardDelete = 283,
        AffiliationCardDetails = 284,
        AffiliationCardDisable = 285,
        #endregion

    }
}