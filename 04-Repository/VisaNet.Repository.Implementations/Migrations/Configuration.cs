using System;
using System.Collections.ObjectModel;
using System.Data.Entity.Migrations;
using System.Linq;
using VisaNet.Common.Security.Entities;
using VisaNet.Common.Security.Entities.Enums;
using VisaNet.Common.Security.Entities.Resources;
using VisaNet.Common.Security.Entities.Security;
using VisaNet.Domain.Entities;
using VisaNet.Domain.Entities.Enums;
using VisaNet.Repository.Implementations.Base;
using Action = VisaNet.Common.Security.Entities.Action;

namespace VisaNet.Repository.Implementations.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<AppContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(AppContext context)
        {
            //if (System.Diagnostics.Debugger.IsAttached == false)
            //    System.Diagnostics.Debugger.Launch();

            //LoadSecurityData(context);
            //LoadData(context);
            //context.SaveChanges();
        }

        public void LoadSecurityData(AppContext context)
        {
            #region Actions

            #region Roles

            const string rolesMvcController = "Role";

            var actionRolesList = new Action
            {
                Id = (int)Actions.RolesList,
                Name = ActionsStrings.RolesList,
                MvcController = rolesMvcController,
                MvcAction = "Index",
                IsDefaultAction = true,
                ActionType = (int)ActionType.List,
            };


            var actionRolesCreate = new Action
            {
                Id = (int)Actions.RolesCreate,
                Name = ActionsStrings.RolesCreate,
                MvcController = rolesMvcController,
                MvcAction = "Create",
                ActionType = (int)ActionType.Create,
                ActionRequired = actionRolesList,
            };

            var actionRolesEdit = new Action
            {
                Id = (int)Actions.RolesEdit,
                Name = ActionsStrings.RolesEdit,
                MvcController = rolesMvcController,
                MvcAction = "Edit",
                ActionType = (int)ActionType.Edit,
                ActionRequired = actionRolesList,
            };

            var actionRolesDelete = new Action
            {
                Id = (int)Actions.RolesDelete,
                Name = ActionsStrings.RolesDelete,
                MvcController = rolesMvcController,
                MvcAction = "Delete",
                ActionType = (int)ActionType.Delete,
                ActionRequired = actionRolesEdit,
            };

            var actionRolesDetails = new Action
            {
                Id = (int)Actions.RolesDetails,
                Name = ActionsStrings.RolesDetails,
                MvcController = rolesMvcController,
                MvcAction = "Details",
                ActionType = (int)ActionType.Details,
                ActionRequired = actionRolesList,
            };

            #endregion

            #region SystemUsers
            const string systemUsersMvcController = "SystemUser";
            var actionSystemUsersList = new Action
            {
                Id = (int)Actions.SystemUsersList,
                Name = ActionsStrings.SystemUsersList,
                MvcController = systemUsersMvcController,
                MvcAction = "Index",
                IsDefaultAction = true,
                ActionType = (int)ActionType.List,
            };

            var actionSystemUsersCreate = new Action
            {
                Id = (int)Actions.SystemUsersCreate,
                Name = ActionsStrings.SystemUsersCreate,
                MvcController = systemUsersMvcController,
                MvcAction = "Create",
                ActionType = (int)ActionType.Create,
                ActionRequired = actionSystemUsersList,
            };

            var actionSystemUsersEdit = new Action
            {
                Id = (int)Actions.SystemUsersEdit,
                Name = ActionsStrings.SystemUsersEdit,
                MvcController = systemUsersMvcController,
                MvcAction = "Edit",
                ActionType = (int)ActionType.Edit,
                ActionRequired = actionSystemUsersList,
            };

            var actionSystemUsersDelete = new Action
            {
                Id = (int)Actions.SystemUsersDelete,
                Name = ActionsStrings.SystemUsersDelete,
                MvcController = systemUsersMvcController,
                MvcAction = "Delete",
                ActionType = (int)ActionType.Delete,
                ActionRequired = actionSystemUsersEdit,
            };

            var actionSystemUsersDetails = new Action
            {
                Id = (int)Actions.SystemUsersDetails,
                Name = ActionsStrings.SystemUsersDetails,
                MvcController = systemUsersMvcController,
                MvcAction = "Details",
                ActionType = (int)ActionType.Details,
                ActionRequired = actionSystemUsersList,
            };
            #endregion

            #region Service
            const string serviceMvcController = "Service";
            var actionServiceList = new Action
            {
                Id = (int)Actions.ServiceList,
                Name = ActionsStrings.List,
                MvcController = serviceMvcController,
                MvcAction = "Index",
                IsDefaultAction = true,
                ActionType = (int)ActionType.List,
            };

            var actionServiceCreate = new Action
            {
                Id = (int)Actions.ServiceCreate,
                Name = ActionsStrings.Create,
                MvcController = serviceMvcController,
                MvcAction = "Create",
                ActionType = (int)ActionType.Create,
                ActionRequired = actionServiceList,
            };

            var actionServiceEdit = new Action
            {
                Id = (int)Actions.ServiceEdit,
                Name = ActionsStrings.Edit,
                MvcController = serviceMvcController,
                MvcAction = "Edit",
                ActionType = (int)ActionType.Edit,
                ActionRequired = actionServiceList,
            };

            var actionServiceDelete = new Action
            {
                Id = (int)Actions.ServiceDelete,
                Name = ActionsStrings.Delete,
                MvcController = serviceMvcController,
                MvcAction = "Delete",
                ActionType = (int)ActionType.Delete,
                ActionRequired = actionServiceEdit,
            };

            var actionServiceDetails = new Action
            {
                Id = (int)Actions.ServiceDetails,
                Name = ActionsStrings.Details,
                MvcController = serviceMvcController,
                MvcAction = "Details",
                ActionType = (int)ActionType.Details,
                ActionRequired = actionServiceList,
            };

            var actionServiceTestGateways = new Action
            {
                Id = (int)Actions.ServiceTestGateways,
                Name = "Test pasarelas",
                MvcController = serviceMvcController,
                MvcAction = "TestGateways",
                ActionType = (int)ActionType.Other,
                ActionRequired = actionServiceList,
            };

            var actionServiceTestCybersource = new Action
            {
                Id = (int)Actions.ServiceTestCybersource,
                Name = "Test Cybersource",
                MvcController = serviceMvcController,
                MvcAction = "TestCybersource",
                ActionType = (int)ActionType.Other,
                ActionRequired = actionServiceList,
            };
            #endregion

            #region ServiceCategory
            const string serviceCategoryMvcController = "ServiceCategory";
            var actionServiceCategoryList = new Action
            {
                Id = (int)Actions.ServiceCategoryList,
                Name = ActionsStrings.List,
                MvcController = serviceCategoryMvcController,
                MvcAction = "Index",
                IsDefaultAction = true,
                ActionType = (int)ActionType.List,
            };

            var actionServiceCategoryCreate = new Action
            {
                Id = (int)Actions.ServiceCategoryCreate,
                Name = ActionsStrings.Create,
                MvcController = serviceCategoryMvcController,
                MvcAction = "Create",
                ActionType = (int)ActionType.Create,
                ActionRequired = actionServiceCategoryList,
            };

            var actionServiceCategoryEdit = new Action
            {
                Id = (int)Actions.ServiceCategoryEdit,
                Name = ActionsStrings.Edit,
                MvcController = serviceCategoryMvcController,
                MvcAction = "Edit",
                ActionType = (int)ActionType.Edit,
                ActionRequired = actionServiceCategoryList,
            };

            var actionServiceCategoryDelete = new Action
            {
                Id = (int)Actions.ServiceCategoryDelete,
                Name = ActionsStrings.Delete,
                MvcController = serviceCategoryMvcController,
                MvcAction = "Delete",
                ActionType = (int)ActionType.Delete,
                ActionRequired = actionServiceCategoryEdit,
            };

            var actionServiceCategoryDetails = new Action
            {
                Id = (int)Actions.ServiceCategoryDetails,
                Name = ActionsStrings.Details,
                MvcController = serviceCategoryMvcController,
                MvcAction = "Details",
                ActionType = (int)ActionType.Details,
                ActionRequired = actionServiceCategoryList,
            };
            #endregion

            #region ServiceContainer
            const string serviceContainerMvcController = "ServiceContainer";
            var actionServiceContainerList = new Action
            {
                Id = (int)Actions.ServiceContainerList,
                Name = ActionsStrings.List,
                MvcController = serviceContainerMvcController,
                MvcAction = "Index",
                IsDefaultAction = true,
                ActionType = (int)ActionType.List,
            };

            var actionServiceContainerCreate = new Action
            {
                Id = (int)Actions.ServiceContainerCreate,
                Name = ActionsStrings.Create,
                MvcController = serviceContainerMvcController,
                MvcAction = "Create",
                ActionType = (int)ActionType.Create,
                ActionRequired = actionServiceContainerList,
            };

            var actionServiceContainerEdit = new Action
            {
                Id = (int)Actions.ServiceContainerEdit,
                Name = ActionsStrings.Edit,
                MvcController = serviceContainerMvcController,
                MvcAction = "Edit",
                ActionType = (int)ActionType.Edit,
                ActionRequired = actionServiceContainerList,
            };

            var actionServiceContainerDelete = new Action
            {
                Id = (int)Actions.ServiceContainerDelete,
                Name = ActionsStrings.Delete,
                MvcController = serviceContainerMvcController,
                MvcAction = "Delete",
                ActionType = (int)ActionType.Delete,
                ActionRequired = actionServiceContainerEdit,
            };

            var actionServiceContainerDetails = new Action
            {
                Id = (int)Actions.ServiceContainerDetails,
                Name = ActionsStrings.Details,
                MvcController = serviceContainerMvcController,
                MvcAction = "Details",
                ActionType = (int)ActionType.Details,
                ActionRequired = actionServiceContainerList,
            };
            #endregion

            #region Contact
            const string contactMvcController = "Contact";
            var actionContactList = new Action
            {
                Id = (int)Actions.ContactList,
                Name = ActionsStrings.ContactList,
                MvcController = contactMvcController,
                MvcAction = "Index",
                IsDefaultAction = true,
                ActionType = (int)ActionType.List,
            };

            var actionContactCreate = new Action
            {
                Id = (int)Actions.ContactCreate,
                Name = ActionsStrings.ContactCreate,
                MvcController = contactMvcController,
                MvcAction = "Create",
                ActionType = (int)ActionType.Create,
                ActionRequired = actionContactList,
            };

            var actionContactEdit = new Action
            {
                Id = (int)Actions.ContactEdit,
                Name = ActionsStrings.ContactEdit,
                MvcController = contactMvcController,
                MvcAction = "Edit",
                ActionType = (int)ActionType.Edit,
                ActionRequired = actionContactList,
            };

            var actionContactDelete = new Action
            {
                Id = (int)Actions.ContactDelete,
                Name = ActionsStrings.ContactDelete,
                MvcController = contactMvcController,
                MvcAction = "Delete",
                ActionType = (int)ActionType.Delete,
                ActionRequired = actionContactEdit,
            };

            var actionContactDetails = new Action
            {
                Id = (int)Actions.ContactDetails,
                Name = ActionsStrings.ContactDetails,
                MvcController = contactMvcController,
                MvcAction = "Details",
                ActionType = (int)ActionType.Details,
                ActionRequired = actionContactList,
            };
            #endregion

            #region Faq
            const string faqMvcController = "Faq";
            var actionFaqList = new Action
            {
                Id = (int)Actions.FaqList,
                Name = ActionsStrings.FaqList,
                MvcController = faqMvcController,
                MvcAction = "Index",
                IsDefaultAction = true,
                ActionType = (int)ActionType.List,
            };

            var actionFaqCreate = new Action
            {
                Id = (int)Actions.FaqCreate,
                Name = ActionsStrings.FaqCreate,
                MvcController = faqMvcController,
                MvcAction = "Create",
                ActionType = (int)ActionType.Create,
                ActionRequired = actionFaqList,
            };

            var actionFaqEdit = new Action
            {
                Id = (int)Actions.FaqEdit,
                Name = ActionsStrings.FaqEdit,
                MvcController = faqMvcController,
                MvcAction = "Edit",
                ActionType = (int)ActionType.Edit,
                ActionRequired = actionFaqList,
            };

            var actionFaqDelete = new Action
            {
                Id = (int)Actions.FaqDelete,
                Name = ActionsStrings.FaqDelete,
                MvcController = faqMvcController,
                MvcAction = "Delete",
                ActionType = (int)ActionType.Delete,
                ActionRequired = actionFaqEdit,
            };

            var actionFaqDetails = new Action
            {
                Id = (int)Actions.FaqDetails,
                Name = ActionsStrings.FaqDetails,
                MvcController = faqMvcController,
                MvcAction = "Details",
                ActionType = (int)ActionType.Details,
                ActionRequired = actionFaqList,
            };
            #endregion

            #region HomePage
            const string homePageMvcController = "HomePage";
            var actionHomePageList = new Action
            {
                Id = (int)Actions.HomePageList,
                Name = ActionsStrings.HomePageList,
                MvcController = homePageMvcController,
                MvcAction = "Index",
                IsDefaultAction = true,
                ActionType = (int)ActionType.List,
            };

            var actionHomePageDetails = new Action
            {
                Id = (int)Actions.HomePageDetails,
                Name = ActionsStrings.HomePageDetails,
                MvcController = homePageMvcController,
                MvcAction = "Details",
                ActionType = (int)ActionType.Details
            };

            var actionHomePageEdit = new Action
            {
                Id = (int)Actions.HomePageEdit,
                Name = ActionsStrings.HomePageEdit,
                MvcController = homePageMvcController,
                MvcAction = "Edit",
                ActionType = (int)ActionType.Edit,
                ActionRequired = actionHomePageDetails,
            };
            #endregion

            #region InstitutionalContent
            const string institutionalContentMvcController = "InstitutionalContent";
            var actionInstitutionalContentDetails = new Action
            {
                Id = (int)Actions.InstitutionalContentDetails,
                Name = ActionsStrings.InstitutionalContentDetails,
                MvcController = institutionalContentMvcController,
                MvcAction = "Details",
                ActionType = (int)ActionType.Details,
                IsDefaultAction = true,
            };

            var actionInstitutionalContentEdit = new Action
            {
                Id = (int)Actions.InstitutionalContentEdit,
                Name = ActionsStrings.InstitutionalContentEdit,
                MvcController = institutionalContentMvcController,
                MvcAction = "Edit",
                ActionType = (int)ActionType.Edit,
                ActionRequired = actionInstitutionalContentDetails,
            };
            #endregion

            #region Help
            const string helpMvcController = "Help";
            var actionHelpDetails = new Action
            {
                Id = (int)Actions.HelpDetails,
                Name = ActionsStrings.Details,
                MvcController = helpMvcController,
                MvcAction = "Details",
                ActionType = (int)ActionType.Details,
                IsDefaultAction = true
            };

            var actionHelpEdit = new Action
            {
                Id = (int)Actions.HelpEdit,
                Name = ActionsStrings.Edit,
                MvcController = helpMvcController,
                MvcAction = "Edit",
                ActionType = (int)ActionType.Edit,
                ActionRequired = actionHelpDetails,
            };
            #endregion

            #region LegalPages
            const string legalPagesMvcController = "LegalPages";
            var actionLegalPagesDetails = new Action
            {
                Id = (int)Actions.LegalPagesDetails,
                Name = ActionsStrings.LegalPagesDetails,
                MvcController = legalPagesMvcController,
                MvcAction = "Details",
                ActionType = (int)ActionType.Details,
                IsDefaultAction = true
            };

            var actionLegalPagesEdit = new Action
            {
                Id = (int)Actions.LegalPagesEdit,
                Name = ActionsStrings.LegalPagesEdit,
                MvcController = legalPagesMvcController,
                MvcAction = "Edit",
                ActionType = (int)ActionType.Edit,
                ActionRequired = actionLegalPagesDetails,
            };
            #endregion

            #region Notices
            const string noticesMvcController = "Notices";
            var actionNoticesDetails = new Action
            {
                Id = (int)Actions.NoticesDetails,
                Name = ActionsStrings.NoticesDetails,
                MvcController = noticesMvcController,
                MvcAction = "Details",
                ActionType = (int)ActionType.Details,
                IsDefaultAction = true
            };

            var actionNoticesEdit = new Action
            {
                Id = (int)Actions.NoticesEdit,
                Name = ActionsStrings.NoticesEdit,
                MvcController = noticesMvcController,
                MvcAction = "Edit",
                ActionType = (int)ActionType.Edit,
                ActionRequired = actionNoticesDetails,
            };
            #endregion

            #region Subscriber
            const string subscriberMvcController = "Subscriber";
            var actionSubscriberList = new Action
            {
                Id = (int)Actions.SubscriberList,
                Name = ActionsStrings.SubscriberList,
                MvcController = subscriberMvcController,
                MvcAction = "Index",
                IsDefaultAction = true,
                ActionType = (int)ActionType.List,
            };

            var actionSubscriberCreate = new Action
            {
                Id = (int)Actions.SubscriberCreate,
                Name = ActionsStrings.SubscriberCreate,
                MvcController = subscriberMvcController,
                MvcAction = "Create",
                ActionType = (int)ActionType.Create,
                ActionRequired = actionSubscriberList,
            };

            var actionSubscriberEdit = new Action
            {
                Id = (int)Actions.SubscriberEdit,
                Name = ActionsStrings.SubscriberEdit,
                MvcController = subscriberMvcController,
                MvcAction = "Edit",
                ActionType = (int)ActionType.Edit,
                ActionRequired = actionSubscriberList,
            };

            var actionSubscriberDelete = new Action
            {
                Id = (int)Actions.SubscriberDelete,
                Name = ActionsStrings.SubscriberDelete,
                MvcController = subscriberMvcController,
                MvcAction = "Delete",
                ActionType = (int)ActionType.Delete,
                ActionRequired = actionSubscriberEdit,
            };

            var actionSubscriberDetails = new Action
            {
                Id = (int)Actions.SubscriberDetails,
                Name = ActionsStrings.SubscriberDetails,
                MvcController = subscriberMvcController,
                MvcAction = "Details",
                ActionType = (int)ActionType.Details,
                ActionRequired = actionSubscriberList,
            };
            #endregion

            #region Parameters
            const string parametersMvcController = "Parameters";
            var actionParametersDetails = new Action
            {
                Id = (int)Actions.ParametersDetails,
                Name = ActionsStrings.ParametersDetails,
                MvcController = parametersMvcController,
                MvcAction = "Details",
                ActionType = (int)ActionType.Details,
                IsDefaultAction = true
            };

            var actionParametersEdit = new Action
            {
                Id = (int)Actions.ParametersEdit,
                Name = ActionsStrings.ParametersEdit,
                MvcController = parametersMvcController,
                MvcAction = "Edit",
                ActionType = (int)ActionType.Edit,
                ActionRequired = actionParametersDetails,
            };
            #endregion

            #region Discount
            const string discountMvcController = "Discount";

            var actionDiscountList = new Action
            {
                Id = (int)Actions.DiscountList,
                Name = ActionsStrings.DiscountList,
                MvcController = discountMvcController,
                MvcAction = "Index",
                IsDefaultAction = true,
                ActionType = (int)ActionType.List,
            };

            var actionDiscountDetails = new Action
            {
                Id = (int)Actions.DiscountDetails,
                Name = ActionsStrings.DiscountDetails,
                MvcController = discountMvcController,
                MvcAction = "Details",
                ActionType = (int)ActionType.Details,
                ActionRequired = actionDiscountList,
            };

            var actionDiscountEdit = new Action
            {
                Id = (int)Actions.DiscountEdit,
                Name = ActionsStrings.DiscountEdit,
                MvcController = discountMvcController,
                MvcAction = "Edit",
                ActionType = (int)ActionType.Edit,
                ActionRequired = actionDiscountList,
            };
            #endregion

            #region Bin
            const string binsMvcController = "Bins";

            var actionBinsList = new Action
            {
                Id = (int)Actions.BinsList,
                Name = ActionsStrings.List,
                MvcController = binsMvcController,
                MvcAction = "Index",
                IsDefaultAction = true,
                ActionType = (int)ActionType.List,
            };


            var actionBinsCreate = new Action
            {
                Id = (int)Actions.BinsCreate,
                Name = ActionsStrings.Create,
                MvcController = binsMvcController,
                MvcAction = "Create",
                ActionType = (int)ActionType.Create,
                ActionRequired = actionBinsList,
            };

            var actionBinsEdit = new Action
            {
                Id = (int)Actions.BinsEdit,
                Name = ActionsStrings.Edit,
                MvcController = binsMvcController,
                MvcAction = "Edit",
                ActionType = (int)ActionType.Edit,
                ActionRequired = actionBinsList,
            };

            var actionBinsDelete = new Action
            {
                Id = (int)Actions.BinsDelete,
                Name = ActionsStrings.Delete,
                MvcController = binsMvcController,
                MvcAction = "Delete",
                ActionType = (int)ActionType.Delete,
                ActionRequired = actionBinsEdit,
            };

            var actionBinsDetails = new Action
            {
                Id = (int)Actions.BinsDetails,
                Name = ActionsStrings.Details,
                MvcController = binsMvcController,
                MvcAction = "Details",
                ActionType = (int)ActionType.Details,
                ActionRequired = actionBinsList,
            };
            #endregion

            #region Reports

            var actionReportsTransactionsDetails = new Action
            {
                Id = (int)Actions.ReportsTransactionsDetails,
                Name = ActionsStrings.ReportsTransactionsDetails,
                MvcController = "ReportsTransactions",
                MvcAction = "Index",
                ActionType = (int)ActionType.Details,
                IsDefaultAction = true
            };

            var actionReportsDashboardDetails = new Action
            {
                Id = (int)Actions.ReportsDashboardDetails,
                Name = ActionsStrings.ReportsDashboardDetails,
                MvcController = "ReportsDashboard",
                MvcAction = "Index",
                ActionType = (int)ActionType.Details,
                IsDefaultAction = true
            };

            var actionReportsTransactionsAmountDetails = new Action
            {
                Id = (int)Actions.ReportsTransactionsAmountDetails,
                Name = ActionsStrings.ReportsTransactionsAmountDetails,
                MvcController = "ReportsTransactionsAmount",
                MvcAction = "Index",
                ActionType = (int)ActionType.Details,
                IsDefaultAction = true
            };

            var actionReportsCybersourceTransactionsDetails = new Action
            {
                Id = (int)Actions.ReportsCybersourceTransactionsDetails,
                Name = ActionsStrings.ReportsCybersourceTransactionsDetails,
                MvcController = "ReportsCybersourceTransactions",
                MvcAction = "Index",
                ActionType = (int)ActionType.Details,
                IsDefaultAction = true
            };

            var actionReportsConciliationDetails = new Action
            {
                Id = (int)Actions.ReportsConciliationDetails,
                Name = ActionsStrings.ReportsConciliationDetails,
                MvcController = "ReportsConciliation",
                MvcAction = "Index",
                ActionType = (int)ActionType.Details,
                IsDefaultAction = true
            };

            var actionReportsCardsDetails = new Action
            {
                Id = (int)Actions.ReportsCardsDetails,
                Name = ActionsStrings.ReportsCardsDetails,
                MvcController = "ReportsCards",
                MvcAction = "Index",
                ActionType = (int)ActionType.Details,
                IsDefaultAction = true
            };
            var actionReportsTc33 = new Action
            {
                Id = (int)Actions.ReportsTc33,
                Name = ActionsStrings.ReportsCardsDetails,
                MvcController = "ReportsTc33",
                MvcAction = "Index",
                ActionType = (int)ActionType.Details,
                IsDefaultAction = true
            };

            var actionReportsHighwayEmails = new Action
            {
                Id = (int)Actions.ReportsHighwayEmails,
                Name = ActionsStrings.ReportsHighwayEmails,
                MvcController = "ReportsHighwayEmail",
                MvcAction = "Index",
                ActionType = (int)ActionType.Details,
                IsDefaultAction = true
            };

            var actionReportsHighwayBills = new Action
            {
                Id = (int)Actions.ReportsHighwayBills,
                Name = ActionsStrings.ReportsHighwayBills,
                MvcController = "ReportsHighwayBill",
                MvcAction = "Index",
                ActionType = (int)ActionType.Details,
                IsDefaultAction = true
            };

            var actionReportsIntegration = new Action
            {
                Id = (int)Actions.ReportsIntegration,
                Name = ActionsStrings.ReportsIntegration,
                MvcController = "ReportsIntegration",
                MvcAction = "Index",
                ActionType = (int)ActionType.Details,
                IsDefaultAction = true
            };

            var actionReportsServicesAssociatedDetails = new Action
            {
                Id = (int)Actions.ReportsServicesAssociatedDetails,
                Name = ActionsStrings.ReportsServicesAssociatedDetails,
                MvcController = "ReportsServicesAssociated",
                MvcAction = "Index",
                ActionType = (int)ActionType.Details,
                IsDefaultAction = true
            };

            var actionReportsAutomaticPaymentsDetails = new Action
            {
                Id = (int)Actions.ReportsAutomaticPaymentsDetails,
                Name = ActionsStrings.ReportsAutomaticPaymentsDetails,
                MvcController = "ReportsAutomaticPayments",
                MvcAction = "Index",
                ActionType = (int)ActionType.Details,
                IsDefaultAction = true
            };

            var actionReportsVersionDetails = new Action
            {
                Id = (int)Actions.ReportsVersionDetails,
                Name = ActionsStrings.ReportsVersionDetails,
                MvcController = "ReportsVersion",
                MvcAction = "Index",
                ActionType = (int)ActionType.Details,
                IsDefaultAction = true
            };

            #endregion

            #region Audit
            const string auditMvcController = "Audit";

            var actionAuditList = new Action
            {
                Id = (int)Actions.AuditList,
                Name = ActionsStrings.List,
                MvcController = auditMvcController,
                MvcAction = "Index",
                IsDefaultAction = true,
                ActionType = (int)ActionType.List,
            };

            var actionAuditDetails = new Action
            {
                Id = (int)Actions.AuditDetails,
                Name = ActionsStrings.Details,
                MvcController = auditMvcController,
                MvcAction = "Details",
                ActionType = (int)ActionType.Details,
                ActionRequired = actionAuditList,
            };

            var actionAuditChangeLog = new Action
            {
                Id = (int)Actions.AuditChangeLog,
                Name = ActionsStrings.Details,
                MvcController = auditMvcController,
                MvcAction = "ChangeLog",
                ActionType = (int)ActionType.Details,
                ActionRequired = null,
                IsDefaultAction = true
            };
            #endregion

            #region Promotion
            const string promotionMvcController = "Promotion";
            var actionPromotionList = new Action
            {
                Id = (int)Actions.PromotionList,
                Name = ActionsStrings.List,
                MvcController = promotionMvcController,
                MvcAction = "Index",
                IsDefaultAction = true,
                ActionType = (int)ActionType.List,
            };

            var actionPromotionCreate = new Action
            {
                Id = (int)Actions.PromotionCreate,
                Name = ActionsStrings.Create,
                MvcController = promotionMvcController,
                MvcAction = "Create",
                ActionType = (int)ActionType.Create,
                ActionRequired = actionPromotionList,
            };

            var actionPromotionEdit = new Action
            {
                Id = (int)Actions.PromotionEdit,
                Name = ActionsStrings.Edit,
                MvcController = promotionMvcController,
                MvcAction = "Edit",
                ActionType = (int)ActionType.Edit,
                ActionRequired = actionPromotionList,
            };

            var actionPromotionDelete = new Action
            {
                Id = (int)Actions.PromotionDelete,
                Name = ActionsStrings.Delete,
                MvcController = promotionMvcController,
                MvcAction = "Delete",
                ActionType = (int)ActionType.Delete,
                ActionRequired = actionPromotionEdit,
            };

            var actionPromotionDetails = new Action
            {
                Id = (int)Actions.PromotionDetails,
                Name = ActionsStrings.Details,
                MvcController = promotionMvcController,
                MvcAction = "Details",
                ActionType = (int)ActionType.Details,
                ActionRequired = actionPromotionList,
            };
            #endregion

            #region Notifications
            const string notificationsMvcController = "Notification";

            var actionNotificationsList = new Action
            {
                Id = (int)Actions.NotificationsList,
                Name = ActionsStrings.List,
                MvcController = notificationsMvcController,
                MvcAction = "Index",
                IsDefaultAction = true,
                ActionType = (int)ActionType.List,
            };

            var actionNotificationsDetails = new Action
            {
                Id = (int)Actions.NotificationsDetails,
                Name = ActionsStrings.Details,
                MvcController = notificationsMvcController,
                MvcAction = "Details",
                ActionType = (int)ActionType.Details,
                ActionRequired = actionNotificationsList,
            };

            var actionNotificationsEdit = new Action
            {
                Id = (int)Actions.NotificationsEdit,
                Name = ActionsStrings.Edit,
                MvcController = notificationsMvcController,
                MvcAction = "Edit",
                ActionType = (int)ActionType.Details,
                ActionRequired = actionNotificationsList,
            };
            #endregion

            #endregion

            #region Functionalities

            var functionalityRoles = new Functionality
            {
                Id = (int)Functionalities.Roles,
                Name = FunctionalityStrings.Roles,
                Actions = new Collection<Action>
                        {
                            actionRolesCreate,
                            actionRolesDelete,
                            actionRolesDetails,
                            actionRolesEdit,
                            actionRolesList,
                        },
                Order = 1,
                IconClass = "fa fa-unlock",
            };

            var functionalitySystemUsers = new Functionality
            {
                Id = (int)Functionalities.SystemUsers,
                Name = FunctionalityStrings.SystemUsers,
                Actions = new Collection<Action>
                        {
                            actionSystemUsersCreate,
                            actionSystemUsersDelete,
                            actionSystemUsersDetails,
                            actionSystemUsersEdit,
                            actionSystemUsersList,
                        },
                Order = 2,
                IconClass = "fa fa-user",
            };

            var functionalityService = new Functionality
            {
                Id = (int)Functionalities.Services,
                Name = FunctionalityStrings.Services,
                Actions = new Collection<Action>
                        {
                            actionServiceCreate,
                            actionServiceDelete,
                            actionServiceDetails,
                            actionServiceEdit,
                            actionServiceList,
                            actionServiceTestGateways,
                            actionServiceTestCybersource,
                        },
                Order = 2,
                IconClass = "fa fa-tag",
            };

            var functionalityServiceCategory = new Functionality
            {
                Id = (int)Functionalities.ServiceCategories,
                Name = FunctionalityStrings.ServiceCategories,
                Actions = new Collection<Action>
                        {
                            actionServiceCategoryCreate,
                            actionServiceCategoryDelete,
                            actionServiceCategoryDetails,
                            actionServiceCategoryEdit,
                            actionServiceCategoryList,
                        },
                Order = 1,
                IconClass = "fa fa-tags",
            };

            var functionalityServiceContainer = new Functionality
            {
                Id = (int)Functionalities.ServiceContainers,
                Name = FunctionalityStrings.ServiceContainers,
                Actions = new Collection<Action>
                        {
                            actionServiceContainerCreate,
                            actionServiceContainerDelete,
                            actionServiceContainerDetails,
                            actionServiceContainerEdit,
                            actionServiceContainerList,
                        },
                Order = 2,
                IconClass = "fa fa-ioxhost",
            };

            var functionalityContact = new Functionality
            {
                Id = (int)Functionalities.Contacts,
                Name = FunctionalityStrings.Contacts,
                Actions = new Collection<Action>
                        {
                            actionContactCreate,
                            actionContactDelete,
                            actionContactDetails,
                            actionContactEdit,
                            actionContactList,
                        },
                Order = 3,
                IconClass = "fa fa-user",
            };

            var functionalityFaq = new Functionality
            {
                Id = (int)Functionalities.Faqs,
                Name = FunctionalityStrings.Faqs,
                Actions = new Collection<Action>
                        {
                            actionFaqCreate,
                            actionFaqDelete,
                            actionFaqDetails,
                            actionFaqEdit,
                            actionFaqList,
                        },
                Order = 4,
                IconClass = "fa fa-question",
            };

            var functionalityHomePage = new Functionality
            {
                Id = (int)Functionalities.HomePage,
                Name = FunctionalityStrings.HomePage,
                Actions = new Collection<Action>
                        {
                            actionHomePageList,
                            actionHomePageDetails,
                            actionHomePageEdit,
                        },
                Order = 1,
                IconClass = "fa fa-home",
            };

            var functionalityInstitutionalContent = new Functionality
            {
                Id = (int)Functionalities.InstitutionalContent,
                Name = FunctionalityStrings.InstitutionalContent,
                Actions = new Collection<Action>
                        {
                            actionInstitutionalContentDetails,
                            actionInstitutionalContentEdit,
                        },
                Order = 2,
                IconClass = "fa fa-align-justify",
            };

            var functionalityHelp = new Functionality
            {
                Id = (int)Functionalities.Help,
                Name = FunctionalityStrings.Help,
                Actions = new Collection<Action>
                        {
                            actionHelpDetails,
                            actionHelpEdit,
                        },
                Order = 7,
                IconClass = "fa fa-question-circle",
            };

            var functionalityLegalPages = new Functionality
            {
                Id = (int)Functionalities.LegalPages,
                Name = FunctionalityStrings.LegalPages,
                Actions = new Collection<Action>
                        {
                            actionLegalPagesDetails,
                            actionLegalPagesEdit,
                        },
                Order = 5,
                IconClass = "fa fa-gavel",
            };

            var functionalityNotices = new Functionality
            {
                Id = (int)Functionalities.Notices,
                Name = FunctionalityStrings.Notices,
                Actions = new Collection<Action>
                        {
                            actionNoticesDetails,
                            actionNoticesEdit,
                        },
                Order = 3,
                IconClass = "fa fa-exclamation",
            };

            var functionalitySubscriber = new Functionality
            {
                Id = (int)Functionalities.Subscribers,
                Name = FunctionalityStrings.Subscribers,
                Actions = new Collection<Action>
                        {
                            actionSubscriberCreate,
                            actionSubscriberDelete,
                            actionSubscriberDetails,
                            actionSubscriberEdit,
                            actionSubscriberList,
                        },
                Order = 6,
                IconClass = "fa fa-thumbs-o-up",
            };

            var functionalityParameters = new Functionality
            {
                Id = (int)Functionalities.Parameters,
                Name = FunctionalityStrings.Parameters,
                Actions = new Collection<Action>
                        {
                            actionParametersDetails,
                            actionParametersEdit,
                        },
                Order = 5,
                IconClass = "fa fa-align-justify",
            };

            var functionalityDiscount = new Functionality
            {
                Id = (int)Functionalities.Discounts,
                Name = FunctionalityStrings.Discounts,
                Actions = new Collection<Action>
                        {
                            actionDiscountList,
                            actionDiscountDetails,
                            actionDiscountEdit,
                        },
                Order = 6,
                IconClass = "fa fa-dollar",
            };

            var functionalityBin = new Functionality
            {
                Id = (int)Functionalities.Bins,
                Name = FunctionalityStrings.Bins,
                Actions = new Collection<Action>
                        {
                            actionBinsCreate,
                            actionBinsDelete,
                            actionBinsDetails,
                            actionBinsEdit,
                            actionBinsList,
                        },
                Order = 4,
                IconClass = "fa fa-credit-card",
            };

            var functionalityPromotion = new Functionality
            {
                Id = (int)Functionalities.Promotion,
                Name = FunctionalityStrings.Promotion,
                Actions = new Collection<Action>
                        {
                            actionPromotionCreate,
                            actionPromotionDelete,
                            actionPromotionDetails,
                            actionPromotionEdit,
                            actionPromotionList,
                        },
                Order = 8,
                IconClass = "fa fa-gift",
            };

            var functionalityAudit = new Functionality
            {
                Id = (int)Functionalities.Audit,
                Name = FunctionalityStrings.Audit,
                Actions = new Collection<Action>
                        {
                            actionAuditList,
                            actionAuditDetails
                        },
                Order = 1,
                IconClass = "fa fa-search",
            };

            var functionalityAuditChangeLog = new Functionality
            {
                Id = (int)Functionalities.AuditChangeLog,
                Name = FunctionalityStrings.AuditChangeLog,
                Actions = new Collection<Action>
                        {
                            actionAuditChangeLog
                        },
                Order = 1,
                IconClass = "fa fa-search",
            };
            var functionalityReportsDashboard = new Functionality
            {
                Id = (int)Functionalities.ReportsDashboard,
                Name = FunctionalityStrings.ReportsDashboard,
                Actions = new Collection<Action>
                        {
                            actionReportsDashboardDetails,
                        },
                Order = 1,
                IconClass = "fa fa-dashboard",
            };

            var functionalityReportsTransactionsAmount = new Functionality
            {
                Id = (int)Functionalities.ReportsTransactionsAmount,
                Name = FunctionalityStrings.ReportsTransactionsAmount,
                Actions = new Collection<Action>
                        {
                            actionReportsTransactionsAmountDetails,
                        },
                Order = 2,
                IconClass = "fa fa-bar-chart-o",
            };

            var functionalityReportsTransactions = new Functionality
            {
                Id = (int)Functionalities.ReportsTransactions,
                Name = FunctionalityStrings.ReportsTransactions,
                Actions = new Collection<Action>
                        {
                            actionReportsTransactionsDetails,
                        },
                Order = 3,
                IconClass = "fa fa-dollar",
            };

            var functionalityReportsCards = new Functionality
            {
                Id = (int)Functionalities.ReportsCards,
                Name = FunctionalityStrings.ReportsCards,
                Actions = new Collection<Action>
                        {
                            actionReportsCardsDetails,
                        },
                Order = 4,
                IconClass = "fa fa-credit-card",
            };
            var functionalityReportsCybersourceTransactions = new Functionality
            {
                Id = (int)Functionalities.ReportsCybersourceTransactions,
                Name = FunctionalityStrings.ReportsCybersourceTransactions,
                Actions = new Collection<Action>
                        {
                            actionReportsCybersourceTransactionsDetails,
                        },
                Order = 5,
                IconClass = "fa fa-align-justify",
            };

            var functionalityReportsConciliation = new Functionality
            {
                Id = (int)Functionalities.ReportsConciliation,
                Name = FunctionalityStrings.ReportsConciliation,
                Actions = new Collection<Action>
                        {
                            actionReportsConciliationDetails,
                        },
                Order = 6,
                IconClass = "fa fa-check",
            };

            var functionalityReportsTc33 = new Functionality
            {
                Id = (int)Functionalities.ReportsTc33,
                Name = FunctionalityStrings.ReportsTc33,
                Actions = new Collection<Action>
                        {
                            actionReportsTc33,
                        },
                Order = 7,
                IconClass = "fa fa-file-text-o",
            };

            var functionalityReportsHighwayEmails = new Functionality
            {
                Id = (int)Functionalities.ReportsHighwayEmail,
                Name = FunctionalityStrings.ReportsHighwayEmail,
                Actions = new Collection<Action>
                        {
                            actionReportsHighwayEmails,
                        },
                Order = 8,
                IconClass = "fa fa-envelope-o",
            };

            var functionalityReportsHighwayBills = new Functionality
            {
                Id = (int)Functionalities.ReportsHighwayBill,
                Name = FunctionalityStrings.ReportsHighwayBill,
                Actions = new Collection<Action>
                        {
                            actionReportsHighwayBills,
                        },
                Order = 9,
                IconClass = "fa fa-ticket",
            };

            var functionalityReportsIntegration = new Functionality
            {
                Id = (int)Functionalities.ReportsIntegration,
                Name = FunctionalityStrings.ReportsIntegration,
                Actions = new Collection<Action>
                        {
                            actionReportsIntegration,
                        },
                Order = 10,
                IconClass = "fa fa-puzzle-piece",
            };

            var functionalityReportsServicesAssociated = new Functionality
            {
                Id = (int)Functionalities.ReportsServicesAssociated,
                Name = FunctionalityStrings.ReportsServicesAssociated,
                Actions = new Collection<Action>
                        {
                            actionReportsServicesAssociatedDetails,
                        },
                Order = 11,
                IconClass = "fa fa-link",
            };

            var functionalityReportsAutomaticPayments = new Functionality
            {
                Id = (int)Functionalities.ReportsAutomaticPayments,
                Name = FunctionalityStrings.ReportsAutomaticPayments,
                Actions = new Collection<Action>
                        {
                            actionReportsAutomaticPaymentsDetails,
                        },
                Order = 12,
                IconClass = "fa fa-calendar",
            };

            var functionalityReportsVersion = new Functionality
            {
                Id = (int)Functionalities.ReportsVersion,
                Name = FunctionalityStrings.ReportsVersion,
                Actions = new Collection<Action>
                        {
                            actionReportsVersionDetails,
                        },
                Order = 13,
                IconClass = "fa fa-list-alt",
            };

            var functionalityNotifications = new Functionality
            {
                Id = (int)Functionalities.Notifications,
                Name = FunctionalityStrings.Notifications,
                Actions = new Collection<Action>
                        {
                            actionNotificationsList,
                            actionNotificationsDetails,
                            actionNotificationsEdit,
                        },
                Order = 1,
                IconClass = "fa fa-bell",
            };

            #endregion

            #region FunctionalitiesGroups

            var functionalityGroupsSecurity = new FunctionalityGroup
            {
                Id = (int)FunctionalitiesGroups.Security,
                Name = FunctionalityGroupStrings.Security,
                Order = 3,
                Functionalities = new Collection<Functionality>
                        {
                            functionalityRoles,
                            functionalitySystemUsers,

                        },
                IconClass = "fa fa-shield"
            };

            var functionalityGroupsAdministration = new FunctionalityGroup
            {
                Id = (int)FunctionalitiesGroups.Administration,
                Name = FunctionalityGroupStrings.Administration,
                Order = 1,
                Functionalities = new Collection<Functionality>
                        {
                            functionalityService,
                            functionalityServiceCategory,
                            functionalityContact,
                            functionalityParameters,
                            functionalityBin,
                            functionalityDiscount,
                            functionalityServiceContainer
                        },
                IconClass = "fa fa-gear"
            };

            var functionalityGroupsPublicPortal = new FunctionalityGroup
            {
                Id = (int)FunctionalitiesGroups.PublicPortal,
                Name = FunctionalityGroupStrings.PublicPortal,
                Order = 4,
                Functionalities = new Collection<Functionality>
                        {
                            functionalityHomePage,
                            functionalityNotices,
                            functionalityFaq,
                            functionalityInstitutionalContent,
                            functionalityLegalPages,
                            functionalitySubscriber,
                            functionalityHelp,
                            functionalityPromotion
                        },
                IconClass = "fa fa-home"
            };

            var functionalityGroupsReports = new FunctionalityGroup
            {
                Id = (int)FunctionalitiesGroups.Reports,
                Name = FunctionalityGroupStrings.Reports,
                Order = 2,
                Functionalities = new Collection<Functionality>
                {
                    functionalityReportsServicesAssociated,
                    functionalityReportsTransactions,
                    functionalityReportsDashboard,
                    functionalityReportsTransactionsAmount,
                    functionalityReportsCybersourceTransactions,
                    functionalityReportsConciliation,
                    functionalityReportsCards,
                    functionalityReportsTc33,
                    functionalityReportsHighwayEmails,
                    functionalityReportsHighwayBills,
                    functionalityReportsIntegration,
                    functionalityReportsAutomaticPayments,
                    functionalityReportsVersion
                },
                IconClass = "fa fa-bar-chart-o"
            };

            var functionalityGroupsAudit = new FunctionalityGroup
            {
                Id = (int)FunctionalitiesGroups.Audit,
                Name = FunctionalityGroupStrings.Audit,
                Order = 2,
                Functionalities = new Collection<Functionality>
                {
                    functionalityAudit,
                    functionalityAuditChangeLog,
                },
                IconClass = "fa fa-search"
            };

            var functionalityGroupsNotifications = new FunctionalityGroup
            {
                Id = (int)FunctionalitiesGroups.Notifications,
                Name = FunctionalityGroupStrings.Notifications,
                Order = 2,
                Functionalities = new Collection<Functionality>
                {
                    functionalityNotifications,
                },
                IconClass = "fa fa-bell"
            };

            #endregion

            #region Roles
            var roleAdmin = new Role
            {
                Id = Guid.NewGuid(),
                Name = "Administrador",
                Actions = new Collection<Action>
                        {
                            actionRolesCreate,
                            actionRolesDelete,
                            actionRolesDetails,
                            actionRolesEdit,
                            actionRolesList,
                            actionSystemUsersCreate,
                            actionSystemUsersDelete,
                            actionSystemUsersDetails,
                            actionSystemUsersEdit,
                            actionSystemUsersList,
                            actionServiceCategoryCreate,
                            actionServiceCategoryEdit,
                            actionServiceCategoryDetails,
                            actionServiceCategoryDelete,
                            actionServiceCategoryList,
                            actionServiceCreate,
                            actionServiceEdit,
                            actionServiceList,
                            actionServiceDelete,
                            actionServiceDetails,
                            actionServiceTestGateways,
                            actionServiceTestCybersource,
                            actionContactCreate,
                            actionContactEdit,
                            actionContactList,
                            actionContactDelete,
                            actionContactDetails,
                            actionFaqCreate,
                            actionFaqEdit,
                            actionFaqList,
                            actionFaqDelete,
                            actionFaqDetails,
                            actionHomePageList,
                            actionHomePageEdit,
                            actionHomePageDetails,
                            actionInstitutionalContentEdit,
                            actionInstitutionalContentDetails,
                            actionLegalPagesEdit,
                            actionLegalPagesDetails,
                            actionNoticesEdit,
                            actionNoticesDetails,
                            actionSubscriberCreate,
                            actionSubscriberEdit,
                            actionSubscriberList,
                            actionSubscriberDelete,
                            actionSubscriberDetails,
                            actionParametersEdit,
                            actionParametersDetails,
                            actionBinsCreate,
                            actionBinsDelete,
                            actionBinsDetails,
                            actionBinsEdit,
                            actionBinsList,
                            actionPromotionCreate,
                            actionPromotionDelete,
                            actionPromotionDetails,
                            actionPromotionEdit,
                            actionPromotionList,
                            actionReportsServicesAssociatedDetails,
                            actionReportsTransactionsDetails,
                            actionReportsDashboardDetails,
                            actionReportsTransactionsAmountDetails,
                            actionReportsCybersourceTransactionsDetails,
                            actionReportsConciliationDetails,
                            actionReportsTc33,
                            actionReportsIntegration,
                            actionReportsAutomaticPaymentsDetails,
                            actionReportsVersionDetails,
                            actionDiscountList,
                            actionDiscountEdit,
                            actionDiscountDetails,
                            actionReportsCardsDetails,
                            actionAuditDetails,
                            actionAuditList,
                            actionHelpDetails,
                            actionHelpEdit,
                        },
                CreationDate = DateTime.Now,
                LastModificationDate = DateTime.Now,
                CreationUser = "System",
                LastModificationUser = "System",
            };
            #endregion

            #region Users
            var systemUser = new SystemUser
            {
                Id = Guid.NewGuid(),
                LDAPUserName = "Admin",
                Enabled = true,
                SystemUserType = SystemUserType.Administration,
                CreationDate = DateTime.Now,
                LastModificationDate = DateTime.Now,
                CreationUser = "System",
                LastModificationUser = "System",
                Roles = new Collection<Role>() { roleAdmin },
            };
            #endregion

            context.FunctionalitiesGroups.AddOrUpdate(f => f.Id, functionalityGroupsSecurity);
            context.FunctionalitiesGroups.AddOrUpdate(f => f.Id, functionalityGroupsAdministration);
            context.FunctionalitiesGroups.AddOrUpdate(f => f.Id, functionalityGroupsPublicPortal);
            context.FunctionalitiesGroups.AddOrUpdate(f => f.Id, functionalityGroupsReports);
            context.FunctionalitiesGroups.AddOrUpdate(f => f.Id, functionalityGroupsAudit);
            context.FunctionalitiesGroups.AddOrUpdate(f => f.Id, functionalityGroupsNotifications);

            context.Roles.AddOrUpdate(r => r.Name, roleAdmin);
            context.SystemUsers.AddOrUpdate(u => u.LDAPUserName, systemUser);


        }

        public void LoadData(AppContext context)
        {
            #region Páginas
            var institutionalContent = new Page
            {
                Id = Guid.NewGuid(),
                Content = "Falta info",
                PageType = PageType.InstitutionalContent,
                CreationDate = DateTime.Now,
                LastModificationDate = DateTime.Now,
                CreationUser = "System",
                LastModificationUser = "System",
            };

            var legalPages = new Page
            {
                Id = Guid.NewGuid(),
                Content = "Falta info",
                PageType = PageType.LegalPages,
                CreationDate = DateTime.Now,
                LastModificationDate = DateTime.Now,
                CreationUser = "System",
                LastModificationUser = "System",
            };

            var notices = new Page
            {
                Id = Guid.NewGuid(),
                Content = "Falta info",
                PageType = PageType.Notices,
                CreationDate = DateTime.Now,
                LastModificationDate = DateTime.Now,
                CreationUser = "System",
                LastModificationUser = "System",
            };

            var help = new Page
            {
                Id = Guid.NewGuid(),
                Content = "Falta info",
                PageType = PageType.Help,
                CreationDate = DateTime.Now,
                LastModificationDate = DateTime.Now,
                CreationUser = "System",
                LastModificationUser = "System",
            };

            if (!context.Pages.Any())
            {
                context.Pages.AddOrUpdate(institutionalContent);
                context.Pages.AddOrUpdate(legalPages);
                context.Pages.AddOrUpdate(notices);
                context.Pages.AddOrUpdate(help);
            }
            #endregion

            #region Página de inicio
            var homepageitem1 = new HomePageItem
            {
                Id = Guid.NewGuid(),
                Headline1 = "Desde cualquier",
                Headline2 = "Dispositivo",
                Description = "Desde tu PC, notebook, tablet o celular podrás acceder a nuestra página web para realizar tus pagos o agendar el pago programado de todos los servicios que te brindamos.",
                ImageId = null,
                Image = null,
                LinkUrl = "https://www.visanetpagos.com.uy/Help",
                FileId = null,
                File = null,
                LinkName = "Ver más",
                CreationDate = DateTime.Now,
                LastModificationDate = DateTime.Now,
                CreationUser = "System",
                LastModificationUser = "System",
            };

            var homepageitem2 = new HomePageItem
            {
                Id = Guid.NewGuid(),
                Headline1 = "Transacciones",
                Headline2 = "100% seguras",
                Description = "Todas las transacciones realizadas en este Portal cuentan con el soporte de Verified by Visa para que puedas operar con la misma seguridad que los pagos que se realizan por mostrador pero sin los riesgos del traslado de efectivo.",
                ImageId = null,
                Image = null,
                LinkUrl = "https://www.visanetpagos.com.uy/LegalPages",
                FileId = null,
                File = null,
                LinkName = "Ver más",
                CreationDate = DateTime.Now,
                LastModificationDate = DateTime.Now,
                CreationUser = "System",
                LastModificationUser = "System",
            };

            var homepageitem3 = new HomePageItem
            {
                Id = Guid.NewGuid(),
                Headline1 = "Maneja tus cuentas",
                Headline2 = "Rápido y fácil",
                Description = "En el momento y lugar que tu elijas podrás realizar el pago de tus cuentas, disponer de avisos de vencimiento o programar pagos programados; todo con simples pasos, las 24 horas, los 365 días.",
                ImageId = null,
                Image = null,
                LinkUrl = "https://www.visanetpagos.com.uy/Faq",
                FileId = null,
                File = null,
                LinkName = "Ver más",
                CreationDate = DateTime.Now,
                LastModificationDate = DateTime.Now,
                CreationUser = "System",
                LastModificationUser = "System",
            };

            var homepage = new HomePage
                           {
                               Id = Guid.NewGuid(),
                               HomePageItem1 = homepageitem1,
                               HomePageItem1Id = homepageitem1.Id,
                               HomePageItem2 = homepageitem2,
                               HomePageItem2Id = homepageitem2.Id,
                               HomePageItem3 = homepageitem3,
                               HomePageItem3Id = homepageitem3.Id,
                               CreationDate = DateTime.Now,
                               LastModificationDate = DateTime.Now,
                               CreationUser = "System",
                               LastModificationUser = "System",
                           };

            if (!context.HomePage.Any())
            {
                context.HomePageItems.AddOrUpdate(homepageitem1);
                context.HomePageItems.AddOrUpdate(homepageitem2);
                context.HomePageItems.AddOrUpdate(homepageitem3);
                context.HomePage.AddOrUpdate(homepage);
            }

            #endregion

            #region Gateway

            //var sistarbank = new Gateway()
            //              {
            //                  Id = Guid.NewGuid(),
            //                  Name = "Sistarbanc",
            //                  Enum = 2
            //              };
            //var banred = new Gateway()
            //{
            //    Id = Guid.NewGuid(),
            //    Name = "Banred",
            //    Enum = 1
            //};
            //var sucive = new Gateway()
            //{
            //    Id = Guid.NewGuid(),
            //    Name = "Sucive",
            //    Enum = 3
            //};
            //if (!context.Gateways.Any())
            //{
            //    context.Gateways.AddOrUpdate(banred);
            //    context.Gateways.AddOrUpdate(sistarbank);
            //    context.Gateways.AddOrUpdate(sucive);
            //}
            #endregion

            #region Parámetros Generales
            var parameters = new Parameters
            {
                Id = Guid.NewGuid(),
                Contact = new Email { EmailAddress = "" },
                ErrorNotification = new Email { EmailAddress = "" },
                SendingEmail = new Email { EmailAddress = "" },
                Banred = new BankCode { Code = "VISANET" },
                Sistarbanc = new BankCode { Code = "3006" },
                Cybersource = new BankCode { Code = "30047" },
                Sucive = new BankCode { Code = "111111" },//ESTO ES UN CODIGO INVENTADO. NO NOS DIERON EL POSTA 20150105
                LoginNumberOfTries = 10,
                CreationDate = DateTime.Now,
                LastModificationDate = DateTime.Now,
                CreationUser = "System",
                LastModificationUser = "System",
            };

            if (!context.Parameters.Any())
            {
                context.Parameters.AddOrUpdate(parameters);
            }
            #endregion

            #region Service
            //var cat = new ServiceCategory()
            //          {
            //              Id = Guid.NewGuid(),
            //              CreationDate = DateTime.Now,
            //              LastModificationDate = DateTime.Now,
            //              CreationUser = "System",
            //              LastModificationUser = "System",
            //              Name = "Categoria 1",
            //          };

            //if (!context.ServicesCategories.Any())
            //{
            //    context.ServicesCategories.AddOrUpdate(cat);
            //}

            //var service = new Service()
            //              {
            //                  Id = Guid.NewGuid(),
            //                  Active = true,
            //                  ServiceCategory = cat,
            //                  Name = "ANTEL - TEL",
            //                  ReferenceParamName = "Nro Cliente",
            //                  MerchantId = "visanetuy",
            //                  CybersourceProfileId = "hxuy857",
            //                  CybersourceAccessKey = "2eb7f45bd63539678ef7d0fa4a47e424",
            //                  CybersourceSecretKey =
            //                      "11fa01fa2c82468da78a4c0e0fd9965bbb63b36e9f7947968c4e18cf0addaceaae41f04f88144a8d8f8d1229e2b24116b9b81fbdc9db4b7a82f30cb9f9557754df4bec65b8594295b7569b5e4f2d611edb1bea54faeb4520af1085d5e4701530cbe8fe1128104dcdbb165166591e9f2503221e4e05dc41d881a6fe66dfbbd612",
            //                  CybersourceTransactionKey =
            //                      "vpYKKN+hLnmwHtRymBsIvf+2Ibg1wbcidC8wVBjSOYCGy5MBRZJ7MsBrznMiySEqGwxonAt5XMvZI3dkiCoC4GJlEUblrOy/vyB8EdCVmg/IyLLzNmJQb5V9mA2OwEdhzYNAe76Rq+RN78GIYxVX7J095Hw/UKRcFlKGZ852Mzo0oMIDTRyFJSBBVVmtECrRQPPN/OCZ7VqkGmlhFZtnAt0ywNvZ2nQIDJ6JBhFzU3AUj0aD/9R2C+XxEBwZzcEmmYd9LEsS5GDLLKbEGx9FqJQZPKJQseuZHTHw5x9dzNWH2DxCVdiPr36BqzQoLOJ7R2MZjutpAQofMfmlqv7L4Q==",
            //                  Tags = "Telefono; Antel",
            //                  CreationDate = DateTime.Now,
            //                  LastModificationDate = DateTime.Now,
            //                  CreationUser = "System",
            //                  LastModificationUser = "System",
            //                  ServiceGateways = new Collection<ServiceGateway>()
            //              };
            //service.ServiceGateways.Add(new ServiceGateway()
            //                            {
            //                                Id = Guid.NewGuid(),
            //                                Active = true,
            //                                ServiceType = "TEL",
            //                                ReferenceId = "ANTEL",
            //                                GatewayId = sistarbank.Id,
            //                            });
            //service.ServiceGateways.Add(new ServiceGateway()
            //{
            //    Id = Guid.NewGuid(),
            //    Active = false,
            //    GatewayId = banred.Id,
            //});
            //service.ServiceGateways.Add(new ServiceGateway()
            //{
            //    Id = Guid.NewGuid(),
            //    Active = false,
            //    GatewayId = sucive.Id,
            //});
            //if (!context.Services.Any())
            //{
            //    context.Services.AddOrUpdate(service);
            //}
            #endregion

            #region Bank

            var bank = new Bank()
                      {
                          Id = Guid.NewGuid(),
                          Name = "BROU",
                          Code = 3004,
                          CreationDate = DateTime.Now,
                          LastModificationDate = DateTime.Now,
                          CreationUser = "System",
                          LastModificationUser = "System",
                      };
            context.Bank.AddOrUpdate(bank);
            #endregion

            #region Bin
            //var bin = new Bin()
            //              {
            //                  Id = Guid.NewGuid(),
            //              Name = "Default",
            //              Value = 999999,
            //              CardType = CardType.Credit,
            //              AuthorizationAmountType = AuthorizationAmountType.Total,
            //                  CreationDate = DateTime.Now,
            //                  LastModificationDate = DateTime.Now,
            //                  CreationUser = "System",
            //                  LastModificationUser = "System",
            //              Gateway = banred
            //              };
            //context.Bin.AddOrUpdate(bin);
            #endregion

            #region Descuentos
            var discountDebit1 = new Discount
            {
                Id = Guid.NewGuid(),
                CardType = CardType.Debit,
                From = new DateTime(2014, 8, 1),
                To = new DateTime(2015, 7, 31),
                Fixed = 2,
                Additional = 0,
                MaximumAmount = 99999999,
                CreationDate = DateTime.Now,
                LastModificationDate = DateTime.Now,
                CreationUser = "System",
                LastModificationUser = "System",
            };

            var discountDebit2 = new Discount
            {
                Id = Guid.NewGuid(),
                CardType = CardType.Debit,
                From = new DateTime(2015, 8, 1),
                To = new DateTime(2016, 7, 31),
                Fixed = 2,
                Additional = 0,
                MaximumAmount = 99999999,
                CreationDate = DateTime.Now,
                LastModificationDate = DateTime.Now,
                CreationUser = "System",
                LastModificationUser = "System",
            };

            var discountDebit3 = new Discount
            {
                Id = Guid.NewGuid(),
                CardType = CardType.Debit,
                From = new DateTime(2016, 8, 1),
                To = new DateTime(2017, 7, 31),
                Fixed = 2,
                Additional = 0,
                MaximumAmount = 99999999,
                CreationDate = DateTime.Now,
                LastModificationDate = DateTime.Now,
                CreationUser = "System",
                LastModificationUser = "System",
            };

            var discountCredit1 = new Discount
            {
                Id = Guid.NewGuid(),
                CardType = CardType.Credit,
                From = new DateTime(2014, 8, 1),
                To = new DateTime(2015, 7, 31),
                Fixed = 0,
                Additional = 2,
                MaximumAmount = 4000,
                CreationDate = DateTime.Now,
                LastModificationDate = DateTime.Now,
                CreationUser = "System",
                LastModificationUser = "System",
            };

            var discountCredit2 = new Discount
            {
                Id = Guid.NewGuid(),
                CardType = CardType.Credit,
                From = new DateTime(2015, 8, 1),
                To = new DateTime(2016, 7, 31),
                Fixed = 0,
                Additional = 1,
                MaximumAmount = 4000,
                CreationDate = DateTime.Now,
                LastModificationDate = DateTime.Now,
                CreationUser = "System",
                LastModificationUser = "System",
            };

            var discountCredit3 = new Discount
            {
                Id = Guid.NewGuid(),
                CardType = CardType.Credit,
                From = new DateTime(2016, 8, 1),
                To = new DateTime(2017, 7, 31),
                Fixed = 0,
                Additional = 0,
                MaximumAmount = 4000,
                CreationDate = DateTime.Now,
                LastModificationDate = DateTime.Now,
                CreationUser = "System",
                LastModificationUser = "System",
            };

            if (!context.Discounts.Any())
            {
                context.Discounts.AddOrUpdate(discountDebit1);
                context.Discounts.AddOrUpdate(discountDebit2);
                context.Discounts.AddOrUpdate(discountDebit3);
                context.Discounts.AddOrUpdate(discountCredit1);
                context.Discounts.AddOrUpdate(discountCredit2);
                context.Discounts.AddOrUpdate(discountCredit3);
            }
            #endregion

            #region Cotizaciones
            var quotation1 = new Quotation
            {
                Id = Guid.NewGuid(),
                DateFrom = System.DateTime.Now,
                Currency = "UI",
                ValueInPesos = 2.9634
            };

            var quotation2 = new Quotation
            {
                Id = Guid.NewGuid(),
                DateFrom = System.DateTime.Now,
                Currency = "USD",
                ValueInPesos = 24.369
            };

            if (!context.Quotations.Any())
            {
                context.Quotations.AddOrUpdate(quotation1);
                context.Quotations.AddOrUpdate(quotation2);
            }
            #endregion

            #region Codiguera de errores Cybersource
            var cybersourceErrorGroup1 = new CybersourceErrorGroup
            {
                Id = Guid.NewGuid(),
                Name = "General",
                Order = 1
            };
            var cybersourceErrorGroup2 = new CybersourceErrorGroup
            {
                Id = Guid.NewGuid(),
                Name = "VbV",
                Order = 2
            };
            var cybersourceErrorGroup3 = new CybersourceErrorGroup
            {
                Id = Guid.NewGuid(),
                Name = "Payment",
                Order = 3
            };
            var cybersourceErrorGroup4 = new CybersourceErrorGroup
            {
                Id = Guid.NewGuid(),
                Name = "Decision Manager",
                Order = 4
            };

            if (!context.CybersourceErrorGroups.Any())
            {
                context.CybersourceErrorGroups.AddOrUpdate(cybersourceErrorGroup1);
                context.CybersourceErrorGroups.AddOrUpdate(cybersourceErrorGroup2);
                context.CybersourceErrorGroups.AddOrUpdate(cybersourceErrorGroup3);
                context.CybersourceErrorGroups.AddOrUpdate(cybersourceErrorGroup4);
            }

            var cybersourceError1 = new CybersourceError
            {
                Id = Guid.NewGuid(),
                ReasonCode = 101,
                CybersourceErrorGroup = cybersourceErrorGroup1
            };
            var cybersourceError2 = new CybersourceError
            {
                Id = Guid.NewGuid(),
                ReasonCode = 102,
                CybersourceErrorGroup = cybersourceErrorGroup1
            };
            var cybersourceError3 = new CybersourceError
            {
                Id = Guid.NewGuid(),
                ReasonCode = 150,
                CybersourceErrorGroup = cybersourceErrorGroup1
            };
            var cybersourceError4 = new CybersourceError
            {
                Id = Guid.NewGuid(),
                ReasonCode = 151,
                CybersourceErrorGroup = cybersourceErrorGroup1
            };
            var cybersourceError5 = new CybersourceError
            {
                Id = Guid.NewGuid(),
                ReasonCode = 152,
                CybersourceErrorGroup = cybersourceErrorGroup1
            };
            var cybersourceError6 = new CybersourceError
            {
                Id = Guid.NewGuid(),
                ReasonCode = 234,
                CybersourceErrorGroup = cybersourceErrorGroup2
            };
            var cybersourceError7 = new CybersourceError
            {
                Id = Guid.NewGuid(),
                ReasonCode = 475,
                CybersourceErrorGroup = cybersourceErrorGroup2
            };
            var cybersourceError8 = new CybersourceError
            {
                Id = Guid.NewGuid(),
                ReasonCode = 476,
                CybersourceErrorGroup = cybersourceErrorGroup2
            };
            var cybersourceError9 = new CybersourceError
            {
                Id = Guid.NewGuid(),
                ReasonCode = 200,
                CybersourceErrorGroup = cybersourceErrorGroup3
            };
            var cybersourceError10 = new CybersourceError
            {
                Id = Guid.NewGuid(),
                ReasonCode = 201,
                CybersourceErrorGroup = cybersourceErrorGroup3
            };
            var cybersourceError11 = new CybersourceError
            {
                Id = Guid.NewGuid(),
                ReasonCode = 202,
                CybersourceErrorGroup = cybersourceErrorGroup3
            };
            var cybersourceError12 = new CybersourceError
            {
                Id = Guid.NewGuid(),
                ReasonCode = 203,
                CybersourceErrorGroup = cybersourceErrorGroup3
            };
            var cybersourceError13 = new CybersourceError
            {
                Id = Guid.NewGuid(),
                ReasonCode = 204,
                CybersourceErrorGroup = cybersourceErrorGroup3
            };
            var cybersourceError14 = new CybersourceError
            {
                Id = Guid.NewGuid(),
                ReasonCode = 205,
                CybersourceErrorGroup = cybersourceErrorGroup3
            };
            var cybersourceError15 = new CybersourceError
            {
                Id = Guid.NewGuid(),
                ReasonCode = 207,
                CybersourceErrorGroup = cybersourceErrorGroup3
            };
            var cybersourceError16 = new CybersourceError
            {
                Id = Guid.NewGuid(),
                ReasonCode = 208,
                CybersourceErrorGroup = cybersourceErrorGroup3
            };
            var cybersourceError17 = new CybersourceError
            {
                Id = Guid.NewGuid(),
                ReasonCode = 209,
                CybersourceErrorGroup = cybersourceErrorGroup3
            };
            var cybersourceError18 = new CybersourceError
            {
                Id = Guid.NewGuid(),
                ReasonCode = 210,
                CybersourceErrorGroup = cybersourceErrorGroup3
            };
            var cybersourceError19 = new CybersourceError
            {
                Id = Guid.NewGuid(),
                ReasonCode = 211,
                CybersourceErrorGroup = cybersourceErrorGroup3
            };
            var cybersourceError20 = new CybersourceError
            {
                Id = Guid.NewGuid(),
                ReasonCode = 220,
                CybersourceErrorGroup = cybersourceErrorGroup3
            };
            var cybersourceError21 = new CybersourceError
            {
                Id = Guid.NewGuid(),
                ReasonCode = 221,
                CybersourceErrorGroup = cybersourceErrorGroup3
            };
            var cybersourceError22 = new CybersourceError
            {
                Id = Guid.NewGuid(),
                ReasonCode = 222,
                CybersourceErrorGroup = cybersourceErrorGroup3
            };
            var cybersourceError23 = new CybersourceError
            {
                Id = Guid.NewGuid(),
                ReasonCode = 230,
                CybersourceErrorGroup = cybersourceErrorGroup3
            };
            var cybersourceError24 = new CybersourceError
            {
                Id = Guid.NewGuid(),
                ReasonCode = 231,
                CybersourceErrorGroup = cybersourceErrorGroup3
            };
            var cybersourceError25 = new CybersourceError
            {
                Id = Guid.NewGuid(),
                ReasonCode = 232,
                CybersourceErrorGroup = cybersourceErrorGroup3
            };
            var cybersourceError26 = new CybersourceError
            {
                Id = Guid.NewGuid(),
                ReasonCode = 233,
                CybersourceErrorGroup = cybersourceErrorGroup3
            };
            var cybersourceError27 = new CybersourceError
            {
                Id = Guid.NewGuid(),
                ReasonCode = 234,
                CybersourceErrorGroup = cybersourceErrorGroup3
            };
            var cybersourceError28 = new CybersourceError
            {
                Id = Guid.NewGuid(),
                ReasonCode = 236,
                CybersourceErrorGroup = cybersourceErrorGroup3
            };
            var cybersourceError29 = new CybersourceError
            {
                Id = Guid.NewGuid(),
                ReasonCode = 240,
                CybersourceErrorGroup = cybersourceErrorGroup3
            };
            var cybersourceError30 = new CybersourceError
            {
                Id = Guid.NewGuid(),
                ReasonCode = 250,
                CybersourceErrorGroup = cybersourceErrorGroup3
            };
            var cybersourceError31 = new CybersourceError
            {
                Id = Guid.NewGuid(),
                ReasonCode = 202,
                CybersourceErrorGroup = cybersourceErrorGroup4
            };
            var cybersourceError32 = new CybersourceError
            {
                Id = Guid.NewGuid(),
                ReasonCode = 231,
                CybersourceErrorGroup = cybersourceErrorGroup4
            };
            var cybersourceError33 = new CybersourceError
            {
                Id = Guid.NewGuid(),
                ReasonCode = 234,
                CybersourceErrorGroup = cybersourceErrorGroup4
            };
            var cybersourceError34 = new CybersourceError
            {
                Id = Guid.NewGuid(),
                ReasonCode = 400,
                CybersourceErrorGroup = cybersourceErrorGroup4
            };
            var cybersourceError35 = new CybersourceError
            {
                Id = Guid.NewGuid(),
                ReasonCode = 480,
                CybersourceErrorGroup = cybersourceErrorGroup4
            };
            var cybersourceError36 = new CybersourceError
            {
                Id = Guid.NewGuid(),
                ReasonCode = 481,
                CybersourceErrorGroup = cybersourceErrorGroup4
            };

            if (!context.CybersourceErrors.Any())
            {
                context.CybersourceErrors.AddOrUpdate(cybersourceError1);
                context.CybersourceErrors.AddOrUpdate(cybersourceError2);
                context.CybersourceErrors.AddOrUpdate(cybersourceError3);
                context.CybersourceErrors.AddOrUpdate(cybersourceError4);
                context.CybersourceErrors.AddOrUpdate(cybersourceError5);
                context.CybersourceErrors.AddOrUpdate(cybersourceError6);
                context.CybersourceErrors.AddOrUpdate(cybersourceError7);
                context.CybersourceErrors.AddOrUpdate(cybersourceError8);
                context.CybersourceErrors.AddOrUpdate(cybersourceError9);
                context.CybersourceErrors.AddOrUpdate(cybersourceError10);
                context.CybersourceErrors.AddOrUpdate(cybersourceError11);
                context.CybersourceErrors.AddOrUpdate(cybersourceError12);
                context.CybersourceErrors.AddOrUpdate(cybersourceError13);
                context.CybersourceErrors.AddOrUpdate(cybersourceError14);
                context.CybersourceErrors.AddOrUpdate(cybersourceError15);
                context.CybersourceErrors.AddOrUpdate(cybersourceError16);
                context.CybersourceErrors.AddOrUpdate(cybersourceError17);
                context.CybersourceErrors.AddOrUpdate(cybersourceError18);
                context.CybersourceErrors.AddOrUpdate(cybersourceError19);
                context.CybersourceErrors.AddOrUpdate(cybersourceError20);
                context.CybersourceErrors.AddOrUpdate(cybersourceError21);
                context.CybersourceErrors.AddOrUpdate(cybersourceError22);
                context.CybersourceErrors.AddOrUpdate(cybersourceError23);
                context.CybersourceErrors.AddOrUpdate(cybersourceError24);
                context.CybersourceErrors.AddOrUpdate(cybersourceError25);
                context.CybersourceErrors.AddOrUpdate(cybersourceError26);
                context.CybersourceErrors.AddOrUpdate(cybersourceError27);
                context.CybersourceErrors.AddOrUpdate(cybersourceError28);
                context.CybersourceErrors.AddOrUpdate(cybersourceError29);
                context.CybersourceErrors.AddOrUpdate(cybersourceError30);
                context.CybersourceErrors.AddOrUpdate(cybersourceError31);
                context.CybersourceErrors.AddOrUpdate(cybersourceError32);
                context.CybersourceErrors.AddOrUpdate(cybersourceError33);
                context.CybersourceErrors.AddOrUpdate(cybersourceError34);
                context.CybersourceErrors.AddOrUpdate(cybersourceError35);
                context.CybersourceErrors.AddOrUpdate(cybersourceError36);
            }
            #endregion

            #region Notifications

            var notification1 = new FixedNotification { Category = FixedNotificationCategory.BatchProcess, CreationDate = DateTime.Now, CreationUser = "Migration", DateTime = DateTime.Now, Description = "Desc 1", Detail = "Detail 1", Id = Guid.NewGuid(), Level = FixedNotificationLevel.Error, Resolved = false, LastModificationDate = DateTime.Now, LastModificationUser = "" };
            var notification2 = new FixedNotification { Category = FixedNotificationCategory.BatchProcess, CreationDate = DateTime.Now, CreationUser = "Migration", DateTime = DateTime.Now, Description = "Desc 2", Detail = "Detail 2", Id = Guid.NewGuid(), Level = FixedNotificationLevel.Warning, Resolved = false, LastModificationDate = DateTime.Now, LastModificationUser = "" };
            var notification3 = new FixedNotification { Category = FixedNotificationCategory.BatchProcess, CreationDate = DateTime.Now, CreationUser = "Migration", DateTime = DateTime.Now, Description = "Desc 3", Detail = "Detail 3", Id = Guid.NewGuid(), Level = FixedNotificationLevel.Error, Resolved = false, LastModificationDate = DateTime.Now, LastModificationUser = "" };
            var notification4 = new FixedNotification { Category = FixedNotificationCategory.BatchProcess, CreationDate = DateTime.Now, CreationUser = "Migration", DateTime = DateTime.Now, Description = "Desc 4", Detail = "Detail 4", Id = Guid.NewGuid(), Level = FixedNotificationLevel.Error, Resolved = false, LastModificationDate = DateTime.Now, LastModificationUser = "" };
            var notification5 = new FixedNotification { Category = FixedNotificationCategory.BatchProcess, CreationDate = DateTime.Now, CreationUser = "Migration", DateTime = DateTime.Now, Description = "Desc 5", Detail = "Detail 5", Id = Guid.NewGuid(), Level = FixedNotificationLevel.Error, Resolved = false, LastModificationDate = DateTime.Now, LastModificationUser = "" };
            var notification6 = new FixedNotification { Category = FixedNotificationCategory.BatchProcess, CreationDate = DateTime.Now, CreationUser = "Migration", DateTime = DateTime.Now, Description = "Desc 6", Detail = "Detail 6", Id = Guid.NewGuid(), Level = FixedNotificationLevel.Info, Resolved = false, LastModificationDate = DateTime.Now, LastModificationUser = "" };

            context.FixedNotification.AddOrUpdate(x => x.Description, notification1);
            context.FixedNotification.AddOrUpdate(x => x.Description, notification2);
            context.FixedNotification.AddOrUpdate(x => x.Description, notification3);
            context.FixedNotification.AddOrUpdate(x => x.Description, notification4);
            context.FixedNotification.AddOrUpdate(x => x.Description, notification5);
            context.FixedNotification.AddOrUpdate(x => x.Description, notification6);
            #endregion
        }
    }
}