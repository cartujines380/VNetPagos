using CyberSource;
using Geocom;
using Ninject;
using Ninject.Web.Common;
using Sucive;
using System.Data.Entity;
using TrackerEnabledDbContext;
using VisaNet.Application.Implementations;
using VisaNet.Application.Implementations.Cache;
using VisaNet.Application.Interfaces;
using VisaNet.Application.Interfaces.Cache;
using VisaNet.Application.VisaNetOn.Implementation;
using VisaNet.Application.VisaNetOn.Interfaces;
using VisaNet.Common.Logging.Context;
using VisaNet.Common.Logging.Repository;
using VisaNet.Common.Logging.Services;
using VisaNet.Common.Security;
using VisaNet.Common.Security.Mailgun;
using VisaNet.Common.Security.WebService;
using VisaNet.Components.Banred.Implementations;
using VisaNet.Components.Banred.Interfaces;
using VisaNet.Components.CyberSource.Interfaces;
using VisaNet.Components.Geocom.Interfaces;
using VisaNet.Components.Sistarbanc.Implementations.Implementations;
using VisaNet.Components.Sistarbanc.Interfaces;
using VisaNet.Components.Sucive.Interfaces;
using VisaNet.DebitRequestBotSynchronization.Implementation;
using VisaNet.DebitRequestSynchronization.Implementation;
using VisaNet.Domain.Debit;
using VisaNet.Domain.Debit.Base;
using VisaNet.Domain.Debit.Interfaces;
using VisaNet.Domain.Entities.BatchProcesses.AutomaticPaymentProcess.Implementations;
using VisaNet.Domain.Entities.BatchProcesses.AutomaticPaymentProcess.Interfaces;
using VisaNet.Domain.Entities.BatchProcesses.CSAcknowledgement.Implementations;
using VisaNet.Domain.Entities.BatchProcesses.CSAcknowledgement.Interfaces;
using VisaNet.Presentation.Core.WebApi.Implementations.Implementations.Administration;
using VisaNet.Presentation.Core.WebApi.Implementations.Implementations.Web;
using VisaNet.Presentation.Core.WebApi.Implementations.Implementations.WebService;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Web;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.WebService;
using VisaNet.Repository.Implementations.Base;
using VisaNet.Repository.Implementations.Implementations;
using VisaNet.Repository.Interfaces.Interfaces;

namespace VisaNet.Common.DependencyInjection
{
    public static class NinjectRegister
    {
        private static IKernel _kernel;

        public static T Get<T>()
        {
            return _kernel.Get<T>();
        }

        public static void Register(IKernel kernel, bool consoleApplication = false)
        {
            _kernel = kernel;

            #region Context

            kernel.Bind<LogContext>().To<LogContext>().InRequestScope();
            kernel.Bind<DbContext>().To<AppContext>().InRequestScope();
            kernel.Bind<IDebitDbContext>().To<DebitContext>().InRequestScope();
            kernel.Bind<TrackerContext>().To<TrackerContext>().InRequestScope();
            //kernel.Bind<ITransactionContext>().To<TestTransactionContext>().InRequestScope();

            kernel.Bind<IMailgunTransactionContext>().To<MailgunTransactionContext>().InRequestScope();
            kernel.Bind<IWebServiceTransactionContext>().To<WebServiceTransactionContext>().InRequestScope();

            kernel.Bind<ITransactionContext>().To<TransactionContext>().InRequestScope();

            if (consoleApplication == false)
            { kernel.Bind<IWebApiTransactionContext>().To<WebApiTransactionContext>().InRequestScope(); }
            else
            { kernel.Bind<IWebApiTransactionContext>().To<ConsoleWebApiTransactionContext>().InSingletonScope(); }

            #endregion Context

            CommonInserts(kernel);
            DebitMerchant(kernel);
        }

        public static void RegisterSingleton(IKernel kernel)
        {
            _kernel = kernel;
            kernel.Bind<LogContext>().To<LogContext>().InSingletonScope();
            kernel.Bind<DbContext>().To<AppContext>().InSingletonScope();
            kernel.Bind<TrackerContext>().To<TrackerContext>().InSingletonScope();
            //kernel.Bind<IMailgunTransactionContext>().To<MailgunTransactionContext>().InSingletonScope();
            //kernel.Bind<IWebServiceTransactionContext>().To<WebServiceTransactionContext>().InSingletonScope();
            kernel.Bind<ITransactionContext>().To<TransactionContext>().InSingletonScope();
            kernel.Bind<IWebApiTransactionContext>().To<ConsoleWebApiTransactionContext>().InSingletonScope();

            CommonInserts(kernel);
            DebitsInserts(kernel);
            DebitsBotInserts(kernel);
        }

        private static void DebitsInserts(IKernel kernel)
        {
            kernel.Bind<IDebitDbContext>().To<DebitContext>().InSingletonScope();
            kernel.Bind<IMerchantRepository>().To<MerchantRepository>().InSingletonScope();
            kernel.Bind<IMerchantDebitRequestRepository>().To<MerchantDebitRequestRepository>().InSingletonScope();
            kernel.Bind<IDebitRequestSynchronizatorService>().To<DebitRequestSynchronizatorService>().InSingletonScope();
        }

        private static void DebitMerchant(IKernel kernel)
        {
            kernel.Bind<IMerchantRepository>().To<MerchantRepository>();
        }

        private static void DebitsBotInserts(IKernel kernel)
        {
            kernel.Bind<IDebitRequestBotSynchronizatorService>().To<DebitRequestBotSynchronizatorService>().InSingletonScope();
        }

        public static void RegisterThreadScope(IKernel kernel, bool consoleApplication = false)
        {
            _kernel = kernel;

            #region Context

            kernel.Bind<LogContext>().To<LogContext>().InThreadScope();
            kernel.Bind<DbContext>().To<AppContext>().InThreadScope();
            kernel.Bind<TrackerContext>().To<TrackerContext>().InThreadScope();
            //kernel.Bind<ITransactionContext>().To<TestTransactionContext>().InRequestScope();

            kernel.Bind<IMailgunTransactionContext>().To<MailgunTransactionContext>().InThreadScope();
            kernel.Bind<IWebServiceTransactionContext>().To<WebServiceTransactionContext>().InThreadScope();

            kernel.Bind<ITransactionContext>().To<TransactionContext>().InThreadScope();

            if (consoleApplication == false)
            { kernel.Bind<IWebApiTransactionContext>().To<WebApiTransactionContext>().InThreadScope(); }
            else
            { kernel.Bind<IWebApiTransactionContext>().To<ConsoleWebApiTransactionContext>().InThreadScope(); }

            #endregion Context

            CommonInserts(kernel);
        }

        private static void CommonInserts(IKernel kernel)
        {
            #region WebApiClients

            kernel.Bind<IServiceCategoryClientService>().To<ServiceCategoryClientService>();
            kernel.Bind<IPageClientService>().To<PageClientService>();
            kernel.Bind<IFaqClientService>().To<FaqClientService>();
            kernel.Bind<IContactClientService>().To<ContactClientService>();
            kernel.Bind<ISubscriberClientService>().To<SubscriberClientService>();
            kernel.Bind<IServiceClientService>().To<ServiceClientService>();
            kernel.Bind<IGatewayClientService>().To<GatewayClientService>();
            kernel.Bind<IHomePageClientService>().To<HomePageClientService>();
            kernel.Bind<ISystemUserClientService>().To<SystemUserClientService>();
            kernel.Bind<IRoleClientService>().To<RoleClientService>();
            kernel.Bind<IRegisterUserClientService>().To<RegisterUserClientService>();
            kernel.Bind<IApplicationUserClientService>().To<ApplicationUserClientService>();
            kernel.Bind<IHomePageItemClientService>().To<HomePageItemClientService>();
            kernel.Bind<IParametersClientService>().To<ParametersClientService>();
            kernel.Bind<IBinsClientService>().To<BinsClientService>();
            kernel.Bind<IPaymentClientService>().To<PaymentClientService>();
            kernel.Bind<INotificationClientService>().To<NotificationClientService>();
            kernel.Bind<IServiceAssociatedClientService>().To<ServiceAssociatedClientService>();
            kernel.Bind<IDiscountClientService>().To<DiscountClientService>();
            kernel.Bind<IPromotionClientService>().To<PromotionClientService>();
            kernel.Bind<IAuditClientService>().To<AuditClientService>();
            kernel.Bind<IReportsClientService>().To<ReportsClientService>();
            kernel.Bind<IConciliationSummaryClientService>().To<ConciliationSummaryClientService>();
            kernel.Bind<ILogClientService>().To<LogClientService>();
            kernel.Bind<ISharingFileClientService>().To<SharingFileClientService>();
            kernel.Bind<ITc33ClientService>().To<Tc33ClientService>();
            kernel.Bind<IReportsHighwayService>().To<ReportsHighwayService>();
            kernel.Bind<IBillClientService>().To<BillClientService>();
            kernel.Bind<IFixedNotificationClientService>().To<FixedNotificationClientService>();
            kernel.Bind<IServiceIntegration>().To<ServiceIntegration>();
            kernel.Bind<ISystemVersionsClientService>().To<SystemVersionsClientService>();
            kernel.Bind<ICyberSourceAccessClientService>().To<CyberSourceAccessClientService>();
            kernel.Bind<IInterpreterClientService>().To<InterpreterClientService>();
            kernel.Bind<IBankClientService>().To<BankClientService>();
            kernel.Bind<ILifApiBillClientService>().To<LifApiBillClientService>();
            kernel.Bind<IWebhookLogClientService>().To<WebhookLogClientService>();
            kernel.Bind<IIntegrationClientService>().To<IntegrationClientService>();
            kernel.Bind<IServiceExternalNotification>().To<ServiceExternalNotification>();
            kernel.Bind<IServiceAssociationSelector>().To<ServiceAssociationSelector>();
            kernel.Bind<IServiceValidatorClientService>().To<ServiceValidatorClientService>();
            kernel.Bind<IDebitCommerceClientService>().To<DebitCommerceClientService>();

            kernel.Bind<IWebApplicationUserClientService>().To<WebApplicationUserClientService>();
            kernel.Bind<IWebBillClientService>().To<WebBillClientService>();
            kernel.Bind<IWebContactClientService>().To<WebContactClientService>();
            kernel.Bind<IWebFaqClientService>().To<WebFaqClientService>();
            kernel.Bind<IWebGatewayClientService>().To<WebGatewayClientService>();
            kernel.Bind<IWebNotificationClientService>().To<WebNotificationClientService>();
            kernel.Bind<IWebPageClientService>().To<WebPageClientService>();
            kernel.Bind<IWebPaymentClientService>().To<WebPaymentClientService>();
            kernel.Bind<IWebRegisterUserClientService>().To<WebRegisterUserClientService>();
            kernel.Bind<IWebServiceClientService>().To<WebServiceClientService>();
            kernel.Bind<IWebSubscriberClientService>().To<WebSubscriberClientService>();
            kernel.Bind<IWebServiceAssosiateClientService>().To<WebServiceAssosiateClientService>();
            kernel.Bind<IWebCardClientService>().To<WebCardClientService>();
            kernel.Bind<IAutoCompleteClientService>().To<WebAutoCompleteClientService>();
            kernel.Bind<IWebSystemUserClientService>().To<WebSystemUserClientService>();
            kernel.Bind<IWebLogClientService>().To<WebLogClientService>();
            kernel.Bind<IWebAnonymousUserClientService>().To<WebAnonymousUserClientService>();
            kernel.Bind<IWebBinClientService>().To<WebBinClientService>();
            kernel.Bind<IWebDiscountClientService>().To<WebDiscountClientService>();
            kernel.Bind<IWebQuotationClientService>().To<WebQuotationClientService>();
            kernel.Bind<IWebPromotionClientService>().To<WebPromotionClientService>();
            kernel.Bind<IWebHomePageClientService>().To<WebHomePageClientService>();
            kernel.Bind<IWebParameterClientService>().To<WebParameterClientService>();
            kernel.Bind<IWebLocationClientService>().To<WebLocationClientService>();
            kernel.Bind<IWebHighwayClientService>().To<WebHighwayClientService>();
            kernel.Bind<IWebWebhookLogClientService>().To<WebWebhookLogClientService>();
            kernel.Bind<IWebReportClientService>().To<WebReportClientService>();
            kernel.Bind<IWebCyberSourceAccessClientService>().To<WebCyberSourceAccessClientService>();
            kernel.Bind<IWebVisaNetOnIntegrationClientService>().To<WebVisaNetOnIntegrationClientService>();
            kernel.Bind<IWebFixedNotificationClientService>().To<WebFixedNotificationClientService>();
            kernel.Bind<IWebDebitClientService>().To<WebDebitClientService>();

            kernel.Bind<IWsExternalAppClientService>().To<WsExternalAppClientService>();
            kernel.Bind<IWsBankClientService>().To<WsBankClientService>();
            kernel.Bind<IWsExternalClientService>().To<WsExternalClientService>();
            kernel.Bind<IWebWebhookRegistrationClientService>().To<WebWebhookRegistrationClientService>();

            kernel.Bind<IChangeTrackerClientService>().To<ChangeTrackerClientService>();
            kernel.Bind<IConciliationDailySummaryClientService>().To<ConciliationDailySummaryClientService>();

            kernel.Bind<IBinGroupClientService>().To<BinGroupClientService>();
            kernel.Bind<IMailgunWebhookService>().To<MailgunWebhookService>();

            kernel.Bind<ICyberSourceAcknowledgementClientService>().To<CyberSourceAcknowledgementClientService>();
            kernel.Bind<IAffiliationCardClientService>().To<AffiliationCardClientService>();

            kernel.Bind<ICustomerSiteCommerceClientService>().To<CustomerSiteCommerceClientService>();
            kernel.Bind<ICustomerSiteBranchClientService>().To<CustomerSiteBranchClientService>();
            kernel.Bind<ICustomerSiteSystemUserClientService>().To<CustomerSiteSystemUserClientService>();

            kernel.Bind<IConciliationVisaNetTc33ClientService>().To<ConciliationVisaNetTc33ClientService>();

            #endregion WebApiClients

            #region Application

            kernel.Bind<IServiceServiceCategory>().To<ServiceServiceCategory>();
            kernel.Bind<IServicePage>().To<ServicePage>();
            kernel.Bind<IServiceFaq>().To<ServiceFaq>();
            kernel.Bind<IServiceContact>().To<ServiceContact>();
            kernel.Bind<IServiceSubscriber>().To<ServiceSubscriber>();
            kernel.Bind<IServiceService>().To<ServiceService>();
            kernel.Bind<ILoggerService>().To<LoggerService>();
            kernel.Bind<IServiceApplicationUser>().To<ServiceApplicationUser>();
            kernel.Bind<IServiceSystemUser>().To<ServiceSystemUser>();
            kernel.Bind<IServiceRole>().To<ServiceRole>();
            kernel.Bind<IServiceHomePage>().To<ServiceHomePage>();
            kernel.Bind<IServiceHomePageItem>().To<ServiceHomePageItem>();
            kernel.Bind<IServiceParameters>().To<ServiceParameters>();
            kernel.Bind<IServiceBin>().To<ServiceBin>();
            kernel.Bind<IServiceBinFile>().To<ServiceBinFile>();
            kernel.Bind<IServicePayment>().To<ServicePayment>();
            kernel.Bind<IServicePaymentIdentifier>().To<ServicePaymentIdentifier>();
            kernel.Bind<IServiceBill>().To<ServiceBill>();
            kernel.Bind<IServiceAnonymousUser>().To<ServiceAnonymousUser>();
            kernel.Bind<IServiceServiceAssosiate>().To<ServiceServiceAssosiate>();
            kernel.Bind<IServiceNotification>().To<ServiceNotification>();
            kernel.Bind<IServiceCard>().To<ServiceCard>();
            kernel.Bind<IServiceDiscount>().To<ServiceDiscount>();
            kernel.Bind<IServiceQuotation>().To<ServiceQuotation>();
            kernel.Bind<IServiceReports>().To<ServiceReports>();

            kernel.Bind<IServiceDiscountCalculator>().To<ServiceDiscountCalculator>();
            kernel.Bind<IServicePromotion>().To<ServicePromotion>();
            kernel.Bind<IServiceAudit>().To<ServiceAudit>();
            kernel.Bind<IServiceCybersourceErrorGroup>().To<ServiceCybersourceErrorGroup>();
            kernel.Bind<IServiceConciliationBanred>().To<ServiceConciliationBanred>();
            kernel.Bind<IServiceConciliationSistarbanc>().To<ServiceConciliationSistarbanc>();
            kernel.Bind<IServiceConciliationSucive>().To<ServiceConciliationSucive>();
            kernel.Bind<IServiceConciliationCybersource>().To<ServiceConciliationCybersource>();
            kernel.Bind<IServiceConciliationSummary>().To<ServiceConciliationSummary>();
            kernel.Bind<IServiceConciliationVisanet>().To<ServiceConciliationVisanet>();
            kernel.Bind<IServiceConciliationVisanetCallback>().To<ServiceConciliationVisanetCallback>();
            kernel.Bind<IServiceTc33>().To<ServiceTc33>();
            kernel.Bind<IServiceTc33Transaction>().To<ServiceTc33Transaction>();
            kernel.Bind<IServiceBank>().To<ServiceBank>();
            kernel.Bind<IServiceLocation>().To<ServiceLocation>();
            kernel.Bind<IServiceHighway>().To<ServiceHighway>();
            kernel.Bind<IServiceHighwayFile>().To<ServiceHighwayFile>();
            kernel.Bind<IServiceFileShipping>().To<ServiceFileShipping>();
            kernel.Bind<IServiceMailgun>().To<ServiceMailgun>();
            kernel.Bind<IServiceWsBank>().To<ServiceWsBank>();

            kernel.Bind<IServiceWebhookRegistration>().To<ServiceWebhookRegistration>();
            kernel.Bind<IServiceWebhookNewAssociation>().To<ServiceWebhookNewAssociation>();
            kernel.Bind<IServiceWebhookDown>().To<ServiceWebhookDown>();

            kernel.Bind<IServiceWsBillPaymentOnline>().To<ServiceWsBillPaymentOnline>();
            kernel.Bind<IServiceWsBillQuery>().To<ServiceWsBillQuery>();
            kernel.Bind<IServiceWsCommerceQuery>().To<ServiceWsCommerceQuery>();
            kernel.Bind<IServiceWsPaymentCancellation>().To<ServiceWsPaymentCancellation>();
            kernel.Bind<IServiceWsCardRemove>().To<ServiceWsCardRemove>();

            kernel.Bind<IServiceProcessHistory>().To<ServiceProcessHistory>();
            kernel.Bind<IServiceSystemVersions>().To<ServiceSystemVersions>();
            kernel.Bind<IServiceAnalyzeCsCall>().To<ServiceAnalyzeCsCall>();

            kernel.Bind<IEmailService>().To<EmailService>();
            kernel.Bind<IServicePaymentTicket>().To<ServicePaymentTicket>();

            kernel.Bind<IServicePostNotification>().To<ServicePostNotification>();
            kernel.Bind<IServicePostSignatureFactory>().To<ServicePostSignatureFactory>();
            kernel.Bind<IServicePostSignature>().To<ServicePostSignature>();
            kernel.Bind<IServiceVonData>().To<ServiceVonData>();

            kernel.Bind<IServiceServiceValidator>().To<ServiceServiceValidator>();
            kernel.Bind<IServiceAffiliationCard>().To<ServiceAffiliationCard>();

            kernel.Bind<IServiceChangeTracker>().To<ServiceChangeTracker>();
            kernel.Bind<IServiceFixedNotification>().To<ServiceFixedNotification>();
            kernel.Bind<IServiceConciliationDailySummary>().To<ServiceConciliationDailySummary>();
            kernel.Bind<IServiceEmailMessage>().To<ServiceEmailMessage>();
            kernel.Bind<IServiceBinGroup>().To<ServiceBinGroup>();
            kernel.Bind<IServiceLifApiBill>().To<ServiceLifApiBill>();

            kernel.Bind<IServiceInterpreter>().To<ServiceInterpreter>();
            kernel.Bind<IConciliationVNPClientService>().To<ConciliationVNPClientService>();

            kernel.Bind<IServiceCyberSourceAcknowledgement>().To<ServiceCyberSourceAcknowledgement>();

            kernel.Bind<IServiceCustomerSite>().To<ServiceCustomerSite>();
            kernel.Bind<IServiceSmsNotification>().To<ServiceSmsNotification>();

            kernel.Bind<IServiceDebitRequest>().To<ServiceDebitRequest>();

            kernel.Bind<IServiceNewBillNotificationInfo>().To<ServiceNewBillNotificationInfo>();
            kernel.Bind<IServiceConciliationRun>().To<ServiceConciliationRun>();
            kernel.Bind<IServiceMemoryCache>().To<ServiceMemoryCache>();
            kernel.Bind<IServiceDataBaseCache>().To<ServiceDatabaseCache>();
            kernel.Bind<IServiceCloudCache>().To<ServiceCloudCache>();
            #endregion Application

            #region Components

            //kernel.Bind<IServiceBanred>().To<MockServiceBanred>();
            kernel.Bind<IServiceBanred>().To<ServiceBanred>();
            //kernel.Bind<ISistarbancAccess>().To<MockSistarbancAccess>();
            kernel.Bind<ISistarbancAccess>().To<SistarbancAccess>();
            kernel.Bind<ICyberSourceAccess>().To<CyberSourceAccess>();
            //kernel.Bind<IServiceComponentsInteraction>().To<ServiceComponentsInteraction>();
            kernel.Bind<ICybersourceAccessFacade>().To<CybersourceAccessFacade>();
            kernel.Bind<IServiceVisaNetOnFactory>().To<ServiceVisaNetOnFactory>();
            kernel.Bind<IServiceVisaNetOnIntegration>().To<ServiceVisaNetOnIntegration>();

            kernel.Bind<ISuciveAccess>().To<SuciveAccess>();
            kernel.Bind<ISucive1>().To<Sucive1>();
            kernel.Bind<ISucive2>().To<Sucive2>();
            kernel.Bind<ISucive3>().To<Sucive3>();
            kernel.Bind<ISucive4>().To<Sucive4>();
            kernel.Bind<ISucive5>().To<Sucive5>();
            kernel.Bind<ISucive6>().To<Sucive6>();
            kernel.Bind<ISucive7>().To<Sucive7>();
            kernel.Bind<ISucive8>().To<Sucive8>();
            kernel.Bind<ISucive9>().To<Sucive9>();
            kernel.Bind<ISucive10>().To<Sucive10>();
            kernel.Bind<ISucive11>().To<Sucive11>();
            kernel.Bind<ISucive12>().To<Sucive12>();
            kernel.Bind<ISucive13>().To<Sucive13>();
            kernel.Bind<ISucive14>().To<Sucive14>();
            kernel.Bind<ISucive15>().To<Sucive15>();
            kernel.Bind<ISucive16>().To<Sucive16>();
            kernel.Bind<ISucive17>().To<Sucive17>();
            kernel.Bind<ISucive18>().To<Sucive18>();
            kernel.Bind<ISucive19>().To<Sucive19>();

            kernel.Bind<IGeocomAccess>().To<GeocomAccess>();
            kernel.Bind<IGeocomMa>().To<GeocomMa>();
            kernel.Bind<IGeocomRo>().To<GeocomRo>();
            kernel.Bind<IGeocomCa>().To<GeocomCa>();
            kernel.Bind<IGeocomFo>().To<GeocomFo>();

            #endregion Components

            #region Repository

            kernel.Bind<IRepositoryServiceCategory>().To<RepositoryServiceCategory>();
            kernel.Bind<IRepositoryPage>().To<RepositoryPage>();
            kernel.Bind<IRepositoryFaq>().To<RepositoryFaq>();
            kernel.Bind<IRepositoryContact>().To<RepositoryContact>();
            kernel.Bind<IRepositorySubscriber>().To<RepositorySubscriber>();
            kernel.Bind<IRepositoryService>().To<RepositoryService>();
            kernel.Bind<IRepositoryGateway>().To<RepositoryGateway>();
            kernel.Bind<ILoggerRepository>().To<LoggerRepository>();
            kernel.Bind<IRepositoryServiceAsociated>().To<RepositoryServiceAsociated>();
            kernel.Bind<IRepositoryHomePage>().To<RepositoryHomePage>();
            kernel.Bind<IRepositoryHomePageItem>().To<RepositoryHomePageItem>();
            kernel.Bind<IRepositoryParameters>().To<RepositoryParameters>();
            kernel.Bind<IRepositoryPayment>().To<RepositoryPayment>();
            kernel.Bind<IRepositoryPaymentIdentifier>().To<RepositoryPaymentIdentifier>();
            kernel.Bind<IRepositoryBill>().To<RepositoryBill>();
            kernel.Bind<IRepositoryCard>().To<RepositoryCard>();
            kernel.Bind<IRepositoryAnonymousUser>().To<RepositoryAnonymousUser>();
            kernel.Bind<IRepositoryNotification>().To<RepositoryNotification>();

            kernel.Bind<IRepositoryApplicationUser>().To<RepositoryApplicationUser>();
            kernel.Bind<IRepositoryMembershipUser>().To<RepositoryMembershipUser>();
            kernel.Bind<IRepositorySystemUser>().To<RepositorySystemUser>();
            kernel.Bind<IRepositoryRole>().To<RepositoryRole>();
            kernel.Bind<IRepositoryBin>().To<RepositoryBin>();
            kernel.Bind<IRepositoryDiscount>().To<RepositoryDiscount>();
            kernel.Bind<IRepositoryQuotation>().To<RepositoryQuotation>();
            kernel.Bind<IRepositoryPromotion>().To<RepositoryPromotion>();
            kernel.Bind<IRepositorySistarbancUser>().To<RepositorySistarbancUser>();
            kernel.Bind<IRepositoryCybersourceErrorGroup>().To<RepositoryCybersourceErrorGroup>();
            kernel.Bind<IRepositoryConciliationBanred>().To<RepositoryConciliationBanred>();
            kernel.Bind<IRepositoryConciliationSistarbanc>().To<RepositoryConciliationSistarbanc>();
            kernel.Bind<IRepositoryConciliationSucive>().To<RepositoryConciliationSucive>();
            kernel.Bind<IRepositoryConciliationCybersource>().To<RepositoryConciliationCybersource>();
            kernel.Bind<IRepositoryConciliationSummary>().To<RepositoryConciliationSummary>();
            kernel.Bind<IRepositoryConciliationVisanet>().To<RepositoryConciliationVisanet>();
            kernel.Bind<IRepositoryConciliationVisanetCallback>().To<RepositoryConciliationVisanetCallback>();
            kernel.Bind<IRepositoryTc33>().To<RepositoryTc33>();
            kernel.Bind<IRepositoryTc33Transaction>().To<RepositoryTc33Transaction>();
            kernel.Bind<IRepositoryBank>().To<RepositoryBank>();
            kernel.Bind<IRepositoryLocation>().To<RepositoryLocation>();

            kernel.Bind<IRepositoryHighwayBill>().To<RepositoryHighwayBill>();
            kernel.Bind<IRepositoryHighwayEmail>().To<RepositoryHighwayEmail>();

            //kernel.Bind<IRepositoryMailMessage>().To<RepositoryMailMessage>();
            kernel.Bind<IRepositorySmsMessage>().To<RepositorySmsMessage>();
            kernel.Bind<IRepositoryNewBillNotificationInfo>().To<RepositoryNewBillNotificationInfo>();

            kernel.Bind<IRepositoryReports>().To<RepositoryReports>();

            kernel.Bind<IRepositoryWebhookRegistration>().To<RepositoryWebhookRegistration>();
            kernel.Bind<IRepositoryWebhookNewAssociation>().To<RepositoryWebhookNewAssociation>();
            kernel.Bind<IRepositoryWebhookDown>().To<RepositoryWebhookDown>();

            kernel.Bind<IRepositoryWsBillPaymentOnline>().To<RepositoryWsBillPaymentOnline>();
            kernel.Bind<IRepositoryWsBillQuery>().To<RepositoryWsBillQuery>();
            kernel.Bind<IRepositoryWsCommerceQuery>().To<RepositoryWsCommerceQuery>();
            kernel.Bind<IRepositoryWsPaymentCancellation>().To<RepositoryWsPaymentCancellation>();
            kernel.Bind<IRepositoryWsCardRemove>().To<RepositoryWsCardRemove>();

            kernel.Bind<IRepositoryProcessHistory>().To<RepositoryProcessHistory>();

            kernel.Bind<IRepositoryChangeTracker>().To<RepositoryChangeTracker>();
            kernel.Bind<IRepositoryFixedNotification>().To<RepositoryFixedNotification>();

            kernel.Bind<IRepositoryConciliationDailySummary>().To<RepositoryConciliationDailySummary>();
            kernel.Bind<IRepositoryEmailMessage>().To<RepositoryEmailMessage>();
            kernel.Bind<IRepositoryBinGroup>().To<RepositoryBinGroup>();
            kernel.Bind<IRepositoryLifApiBill>().To<RepositoryLifApiBill>();

            kernel.Bind<IRepositoryInterpreter>().To<RepositoryInterpreter>();
            kernel.Bind<IRepositoryVonData>().To<RepositoryVonData>();
            kernel.Bind<IRepositoryCyberSourceAcknowledgement>().To<RepositoryCyberSourceAcknowledgement>();
            kernel.Bind<IRepositoryCyberSourceVoid>().To<RepositoryCyberSourceVoid>();

            kernel.Bind<IRepositoryAffiliationCard>().To<RepositoryAffiliationCard>();
            kernel.Bind<IRepositoryDebitRequest>().To<RepositoryDebitRequest>();
            kernel.Bind<IRepositoryConciliationRun>().To<RepositoryConciliationRun>();

            #endregion Repository

            #region Domain

            kernel.Bind<IBillHelper>().To<BillHelper>();
            kernel.Bind<IPaymentHandler>().To<PaymentHandler>();
            kernel.Bind<IStateFactory>().To<StateFactory>();
            kernel.Bind<ISystemNotificationFactory>().To<SystemNotificationFactory>();
            kernel.Bind<IUserNotificationFactory>().To<UserNotificationFactory>();
            kernel.Bind<IValidationHelper>().To<ValidationHelper>();
            kernel.Bind<ILoggerHelper>().To<LoggerHelper>();

            //CyberSource Acknowledgements
            kernel.Bind<ICsAckLoggerHelper>().To<CsAckLoggerHelper>();

            #endregion Domain

            #region LIF

            kernel.Bind<Presentation.Core.WebApi.Interfaces.Interfaces.LIF.IDiscountClientService>().To<Presentation.Core.WebApi.Implementations.Implementations.LIF.DiscountClientService>();
            kernel.Bind<Presentation.Core.WebApi.Interfaces.Interfaces.LIF.ICardClientService>().To<Presentation.Core.WebApi.Implementations.Implementations.LIF.CardClientService>();
            kernel.Bind<Presentation.Core.WebApi.Interfaces.Interfaces.LIF.ILifApiBillClientService>().To<Presentation.Core.WebApi.Implementations.Implementations.LIF.LifApiBillClientService>();

            kernel.Bind<LIF.Interfaces.ICardService>().To<LIF.Implementations.CardService>();
            kernel.Bind<LIF.Interfaces.IDiscountService>().To<LIF.Implementations.DiscountService>();
            kernel.Bind<LIF.Interfaces.IQuotaService>().To<LIF.Implementations.QuotaService>();

            #endregion LIF
        }

    }
}