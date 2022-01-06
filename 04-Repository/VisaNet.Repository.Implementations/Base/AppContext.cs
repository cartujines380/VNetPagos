using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using TrackerEnabledDbContext;
using VisaNet.Common.Logging.Entities;
using VisaNet.Common.Security;
using VisaNet.Common.Security.Entities;
using VisaNet.Common.Security.Entities.Security;
using VisaNet.Domain.Entities;
using VisaNet.Domain.Entities.ExternalRequest;
using VisaNet.Domain.Entities.NotificationHelpersEntities;
using Action = VisaNet.Common.Security.Entities.Action;

namespace VisaNet.Repository.Implementations.Base
{
    public class AppContext : TrackerContext
    {
        public AppContext()
            : base()
        {
            Database.SetInitializer<AppContext>(null);
        }

        public AppContext(IWebApiTransactionContext transactionContext)
            : base(transactionContext)
        {
            Database.SetInitializer<AppContext>(null);
        }

        public IDbSet<AutomaticPayment> AutomaticPayments { get; set; }
        public IDbSet<SistarbancUser> SistarbancUser { get; set; }
        public IDbSet<Bill> Bills { get; set; }
        public IDbSet<Card> Cards { get; set; }
        public IDbSet<Contact> Contacts { get; set; }
        public IDbSet<Faq> Faqs { get; set; }
        public IDbSet<HomePage> HomePage { get; set; }
        public IDbSet<HomePageItem> HomePageItems { get; set; }
        public IDbSet<Notification> Notifications { get; set; }
        public IDbSet<NotificationConfig> NotificationConfigs { get; set; }
        public IDbSet<Page> Pages { get; set; }
        public IDbSet<Payment> Payments { get; set; }
        public IDbSet<PaymentIdentifier> PaymentIdentifier { get; set; }
        public IDbSet<Service> Services { get; set; }
        public IDbSet<ServiceCategory> ServicesCategories { get; set; }
        public IDbSet<Subscriber> Subscribers { get; set; }
        public IDbSet<Gateway> Gateways { get; set; }
        public IDbSet<Parameters> Parameters { get; set; }
        public IDbSet<Bin> Bin { get; set; }
        public IDbSet<Discount> Discounts { get; set; }
        public IDbSet<Quotation> Quotations { get; set; }
        public IDbSet<Promotion> Promotions { get; set; }
        public IDbSet<CybersourceErrorGroup> CybersourceErrorGroups { get; set; }
        public IDbSet<CybersourceError> CybersourceErrors { get; set; }
        public IDbSet<ConciliationBanred> ConciliationBanred { get; set; }
        public IDbSet<ConciliationSistarbanc> ConciliationSistarbanc { get; set; }
        public IDbSet<ConciliationCybersource> ConciliationCybersource { get; set; }
        public IDbSet<ConciliationSucive> ConciliationSucive { get; set; }
        public IDbSet<ConciliationVisanet> ConciliationVisanet { get; set; }
        public IDbSet<ConciliationSummary> ConciliationSummary { get; set; }
        public IDbSet<ConciliationVisanetCallback> ConciliationVisanetCallback { get; set; }
        public IDbSet<Tc33> Tc33 { get; set; }
        public IDbSet<Tc33Transaction> Tc33Transactions { get; set; }
        public IDbSet<Bank> Bank { get; set; }
        public IDbSet<Location> Location { get; set; }
        public IDbSet<HighwayBill> HighwayBill { get; set; }
        public IDbSet<HighwayEmail> HighwayEmail { get; set; }
        public IDbSet<ServiceEnableEmail> ServiceEnableEmail { get; set; }
        public IDbSet<HighwayEmailError> HighwayEmailError { get; set; }
        public IDbSet<ProcessHistory> ProcessHistory { get; set; }
        public IDbSet<BinGroup> BinGroups { get; set; }
        public IDbSet<Interpreter> Interpreters { get; set; }
        public IDbSet<AffiliationCard> AffiliationCard { get; set; }
        public IDbSet<ConciliationRun> ConciliationRuns { get; set; }

        //Security
        public IDbSet<MembershipUser> MembershipUsers { get; set; }
        public IDbSet<Role> Roles { get; set; }
        public IDbSet<Action> Actions { get; set; }
        public IDbSet<Functionality> Functionalities { get; set; }
        public IDbSet<FunctionalityGroup> FunctionalitiesGroups { get; set; }
        public IDbSet<LifApiBill> LifApiBill { get; set; }

        //Users
        public IDbSet<ApplicationUser> ApplicationUsers { get; set; }
        public IDbSet<SystemUser> SystemUsers { get; set; }
        public IDbSet<AnonymousUser> AnonymousUsers { get; set; }
        public IDbSet<VonData> VonData { get; set; }

        //Logs
        public IDbSet<Log> Logs { get; set; }

        //Notifications (mails/sms)
        public IDbSet<EmailMessage> MailMessages { get; set; }
        public IDbSet<SmsMessage> SmsMessages { get; set; }
        //public IDbSet<NotificationMessage> NotificationMessages { get; set; }
        public IDbSet<FixedNotification> FixedNotification { get; set; }

        public IDbSet<NewBillNotificationInfo> NewBillNotificationInfos { get; set; }

        //Webhook and WebService
        public IDbSet<WebhookRegistration> WebhookRegistration { get; set; }
        public IDbSet<WebhookNewAssociation> WebhookNewAssociations { get; set; }
        public IDbSet<WebhookDown> WebhookDown { get; set; }
        public IDbSet<WsBillPaymentOnline> WsBillPaymentOnline { get; set; }
        public IDbSet<WsBillQuery> WsBillQuery { get; set; }
        public IDbSet<WsCommerceQuery> WsCommerceQuery { get; set; }
        public IDbSet<WsPaymentCancellation> WsPaymentCancellation { get; set; }
        public IDbSet<WsCardRemove> WsCardRemove { get; set; }
        public IDbSet<WebhookAccessToken> WebhookAccessToken { get; set; }


        public IDbSet<CyberSourceAcknowledgement> CyberSourceAcknowledgements { get; set; }
        public IDbSet<CyberSourceVoid> CyberSourceVoids { get; set; }


        //DEBIT
        public IDbSet<DebitRequest> DebitRequest { get; set; }
        public IDbSet<DebitRequestReference> DebitRequestReference { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingEntitySetNameConvention>();
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();

            modelBuilder.Entity<HighwayBill>()
                .HasMany(o => o.AuxiliarData)
                .WithRequired(i => i.HighwayBill)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<ServiceAssociated>()
                .HasMany(c => c.Cards)
                .WithMany(x => x.ServicesAssociated)
                .Map(a => a.ToTable("ServicesAssociatedCards"));

            modelBuilder.Entity<Card>()
                .HasMany(c => c.ServicesAssociated)
                .WithMany(x => x.Cards);

            modelBuilder.Entity<ProcessHistory>()
                .HasMany(o => o.PendingAutomaticPayments)
                .WithRequired(i => i.ProcessHistory)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<VonData>()
                .HasKey(x => new { x.AppId, x.UserExternalId, x.CardExternalId });

            modelBuilder.Entity<Bin>()
                .HasMany(c => c.BinAuthorizationAmountTypeList)
                .WithRequired(x => x.Bin)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<BinGroup>()
               .HasMany(s => s.Services)
               .WithMany(c => c.BinGroups)
               .Map(cs =>
               {
                   cs.MapLeftKey("BinGroup_Id");
                   cs.MapRightKey("Service_Id");
                   cs.ToTable("BinGroupServices");
               });

            modelBuilder.Entity<Bin>()
               .HasMany(s => s.BinGroups)
               .WithMany(c => c.Bins)
               .Map(cs =>
               {
                   cs.MapLeftKey("Bin_Id");
                   cs.MapRightKey("BinGroup_Id");
                   cs.ToTable("BinBinGroups");
               });
        }

    }
}