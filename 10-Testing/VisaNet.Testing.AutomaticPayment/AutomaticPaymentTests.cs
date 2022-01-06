using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using Ninject;
using VisaNet.Application.Interfaces;
using VisaNet.Common.DependencyInjection;
using VisaNet.Domain.Entities;
using VisaNet.Domain.Entities.Enums;
using VisaNet.Domain.Entities.NotificationHelpersEntities;
using VisaNet.Repository.Implementations.Base;
using VisaNet.Repository.Interfaces.Interfaces;

namespace VisaNet.Testing.AutomaticPayment
{
    public abstract class AutomaticPaymentTests
    {
        private static IServiceServiceCategory _serviceServiceCategory;
        private static IRepositoryGateway _repositoryGateway;
        private static IRepositoryProcessHistory _repositoryProcessHistory;
        private static IRepositoryApplicationUser _repositoryApplicationUser;
        private static IRepositoryPayment _repositoryPayment;
        private static IRepositoryBill _repositoryBill;
        private static IRepositoryNotification _repositoryNotification;
        private static IRepositoryNewBillNotificationInfo _repositoryNewBillNotificationInfo;
        private static IRepositoryServiceAsociated _repositoryServiceAsociated;
        private static IRepositoryHighwayBill _repositoryHighwayBill;
        private static IRepositoryService _repositoryService;
        private static IRepositoryBinGroup _repositoryBinGroup;
        private static IRepositoryDiscount _repositoryDiscount;

        private static Guid _applicationUserId;
        private static Guid _service1Id;
        private static Guid _service2Id;
        private static Guid _serviceGateway1Id;
        private static Guid _serviceGateway2Id;
        private static Guid _card1Id;
        private static Guid _card2Id;
        private static Guid _automaticPayment1Id;
        private static Guid _automaticPayment3Id;
        private static Guid _automaticPayment4Id;
        private static Guid _automaticPayment5Id;
        private static Guid _automaticPayment6Id;
        private static Guid _automaticPayment7Id;
        private static Guid _automaticPayment8Id;
        private static Guid _notificationConfigId;
        private static Guid _highwayEmail1Id;
        private static Guid _highwayEmail2Id;
        private static Guid _highwayBill1Id;
        private static Guid _highwayBill2Id;
        private static Guid _highwayBill3Id;
        private static Guid _highwayBill4Id;
        private static Guid _highwayBill5Id;
        private static Guid _highwayBill6Id;
        private static Guid _highwayBill7Id;
        private static Guid _highwayBill8Id;
        private static Guid _highwayBill9Id;
        private static Guid _highwayBill10Id;
        private static Guid _highwayBill11Id;
        private static Guid _binGroupId;

        protected static Guid ServiceAssociated1Id;
        protected static Guid ServiceAssociated2Id;
        protected static Guid ServiceAssociated3Id;
        protected static Guid ServiceAssociated4Id;
        protected static Guid ServiceAssociated5Id;
        protected static Guid ServiceAssociated6Id;
        protected static Guid ServiceAssociated7Id;
        protected static Guid ServiceAssociated8Id;

        protected AutomaticPaymentTests()
        {
            var kernel = new StandardKernel();
            NinjectRegister.Register(kernel, true);

            _serviceServiceCategory = NinjectRegister.Get<IServiceServiceCategory>();
            _repositoryGateway = NinjectRegister.Get<IRepositoryGateway>();
            _repositoryProcessHistory = NinjectRegister.Get<IRepositoryProcessHistory>();
            _repositoryApplicationUser = NinjectRegister.Get<IRepositoryApplicationUser>();
            _repositoryPayment = NinjectRegister.Get<IRepositoryPayment>();
            _repositoryBill = NinjectRegister.Get<IRepositoryBill>();
            _repositoryNotification = NinjectRegister.Get<IRepositoryNotification>();
            _repositoryNewBillNotificationInfo = NinjectRegister.Get<IRepositoryNewBillNotificationInfo>();
            _repositoryServiceAsociated = NinjectRegister.Get<IRepositoryServiceAsociated>();
            _repositoryHighwayBill = NinjectRegister.Get<IRepositoryHighwayBill>();
            _repositoryService = NinjectRegister.Get<IRepositoryService>();
            _repositoryBinGroup = NinjectRegister.Get<IRepositoryBinGroup>();
            _repositoryDiscount = NinjectRegister.Get<IRepositoryDiscount>();
        }

        protected static void LoadDataForTests()
        {
            LoadGuids();

            //Pre-condicion: debe existir el usuario cuyo email se especifica en el app.config y tenga una tarjeta valida
            var emailForTests = ConfigurationManager.AppSettings["EmailForTests"];
            var appUser = _repositoryApplicationUser.All(null, x => x.Cards).FirstOrDefault(x => x.Email == emailForTests);
            var card1 = appUser.Cards.FirstOrDefault(x => x.Active && !x.Deleted && x.MaskedNumber == "411111xxxxxx1111");

            _applicationUserId = appUser.Id;
            _card1Id = card1.Id;

            //Pre-condicion: debe existir el bingroup que contenga la tarjeta 4111
            var binGroups = _repositoryBinGroup.All(null, x => x.Bins).ToList();
            var binGroupId = binGroups.FirstOrDefault(x => x.Bins != null && x.Bins.Any(b => b.Value == 411111)).Id;

            using (var context = new AppContext())
            {
                var servicesTable = context.Set<Service>();
                var servicesAssociatedTable = context.Set<ServiceAssociated>();
                var automaticPaymentsTable = context.Set<Domain.Entities.AutomaticPayment>();
                var cardsTable = context.Set<Card>();
                var notificationsConfigTable = context.Set<NotificationConfig>();
                var highwayEmailsTable = context.Set<HighwayEmail>();
                var highwayBillsTable = context.Set<HighwayBill>();
                var binGroupsTable = context.Set<BinGroup>();

                var binGroup = binGroupsTable.Single(x => x.Id == binGroupId);
                _binGroupId = binGroup.Id;

                var cards = CardsDataForTest();
                cardsTable.AddRange(cards);

                var services = ServicesDataForTest();
                servicesTable.AddRange(services);

                var notificationConfig = NotificationConfigDataForTest();
                notificationsConfigTable.Add(notificationConfig);

                var automaticPayments = AutomaticPaymentsDataForTest();
                automaticPaymentsTable.AddRange(automaticPayments);

                var servicesAssociated = ServicesAssociatedDataForTest();
                servicesAssociatedTable.AddRange(servicesAssociated);

                var highwayEmails = HighwayEmailDataForTest();
                highwayEmailsTable.AddRange(highwayEmails);

                var highwayBills = HighwayBillsForTest();
                highwayBillsTable.AddRange(highwayBills);

                foreach (var service in services)
                {
                    if (service.BinGroups == null)
                    {
                        service.BinGroups = new Collection<BinGroup>();
                    }
                    service.BinGroups.Add(binGroup);
                }

                context.SaveChanges();
            }
        }

        private static void LoadGuids()
        {
            _service1Id = Guid.NewGuid();
            _service2Id = Guid.NewGuid();
            _serviceGateway1Id = Guid.NewGuid();
            _serviceGateway2Id = Guid.NewGuid();
            ServiceAssociated1Id = Guid.NewGuid();
            ServiceAssociated2Id = Guid.NewGuid();
            ServiceAssociated3Id = Guid.NewGuid();
            ServiceAssociated4Id = Guid.NewGuid();
            ServiceAssociated5Id = Guid.NewGuid();
            ServiceAssociated6Id = Guid.NewGuid();
            ServiceAssociated7Id = Guid.NewGuid();
            ServiceAssociated8Id = Guid.NewGuid();
            _card2Id = Guid.NewGuid();
            _automaticPayment1Id = Guid.NewGuid();
            _automaticPayment3Id = Guid.NewGuid();
            _automaticPayment4Id = Guid.NewGuid();
            _automaticPayment5Id = Guid.NewGuid();
            _automaticPayment6Id = Guid.NewGuid();
            _automaticPayment7Id = Guid.NewGuid();
            _automaticPayment8Id = Guid.NewGuid();
            _notificationConfigId = Guid.NewGuid();
            _highwayEmail1Id = Guid.NewGuid();
            _highwayEmail2Id = Guid.NewGuid();
            _highwayBill1Id = Guid.NewGuid();
            _highwayBill2Id = Guid.NewGuid();
            _highwayBill3Id = Guid.NewGuid();
            _highwayBill4Id = Guid.NewGuid();
            _highwayBill5Id = Guid.NewGuid();
            _highwayBill6Id = Guid.NewGuid();
            _highwayBill7Id = Guid.NewGuid();
            _highwayBill8Id = Guid.NewGuid();
            _highwayBill9Id = Guid.NewGuid();
            _highwayBill10Id = Guid.NewGuid();
            _highwayBill11Id = Guid.NewGuid();
        }

        private static IEnumerable<Service> ServicesDataForTest()
        {
            var serviceList = new List<Service>();
            var serviceCategoryId = _serviceServiceCategory.AllNoTracking().FirstOrDefault().Id;
            var carreteraGateway = _repositoryGateway.All().FirstOrDefault(x => x.Name.ToUpper() == "CARRETERA");

            var service1 = new Service
            {
                Id = _service1Id,
                ServiceCategoryId = serviceCategoryId,
                Name = "Carretera Test (sin pago programado)",
                Description = "TEST_AUTOMATIC_PAYMENT",
                LinkId = "http://",
                Active = true,
                MerchantId = "visanetuy8",
                CybersourceProfileId = "hxuy857",
                CybersourceAccessKey = "2eb7f45bd63539678ef7d0fa4a47e424",
                CybersourceSecretKey = "11fa01fa2c82468da78a4c0e0fd9965bbb63b36e9f7947968c4e18cf0addaceaae41f04f88144a8d8f8d1229e2b24116b9b81fbdc9db4b7a82f30cb9f9557754df4bec65b8594295b7569b5e4f2d611edb1bea54faeb4520af1085d5e4701530cbe8fe1128104dcdbb165166591e9f2503221e4e05dc41d881a6fe66dfbbd612",
                CybersourceTransactionKey = "QVDS9IAorVdQO1dLgKeQsWuoTEXiwMg+8Q91uJ11cyRnU6orSs8w8tGjCOehYHceqR0Z7nQdOt5sPlhsfyynQmyC06laBU3iUAU+LfDw4CLYqHJ7QqC8yZr8qDvMJSdSQE+J3CPjsH+Rmz1HNdfHWiMEd5Tx+1e/rLrIrzaW3UgVo2GpEoaQIikhGSerbCu6e0KXgQ42gx7eaMOkSaDBWU/3GQkj+7VtcFgt+cy7DP52/1bCxRpYsvGErX9Gj/mDPczvqsBRK+LhNZtpwh6rZTQZ9MsKipgXwtyegJtOSKGiE04kt8vYDXfBm3pGN+GdXKGkTdeO+wpUokiLg1wLEg==",
                ReferenceParamName = "Número de cliente",
                CreationUser = "admin",
                LastModificationUser = "admin",
                CreationDate = DateTime.Today,
                LastModificationDate = DateTime.Today,
                EnableMultipleBills = false,
                Departament = DepartamentType.NotSucive,
                ExtractEmail = "sherrera@hexacta.com",
                EnableAutomaticPayment = false,
                EnablePublicPayment = true,
                EnablePrivatePayment = true,
                EnablePartialPayment = false,
                UrlName = "",
                DiscountType = DiscountType.FinancialInclusion,
                Container = false,
                EnableAssociation = true,
                AllowSelectContentAssociation = false,
                AllowSelectContentPayment = false,
                AskUserForReferences = true,
                AllowMultipleCards = false,
                IntroContent = "",
                MaxQuotaAllow = 1,
                ServiceGateways = new Collection<ServiceGateway>
                {
                    new ServiceGateway
                    {
                        Id = _serviceGateway1Id,
                        Active = true,
                        ReferenceId = "2062940",
                        ServiceType = "700",
                        SendExtract = true,
                        GatewayId = carreteraGateway.Id
                    }
                }
            };

            var service2 = new Service
            {
                Id = _service2Id,
                ServiceCategoryId = serviceCategoryId,
                Name = "Carretera Test (con pago programado)",
                Description = "TEST_AUTOMATIC_PAYMENT",
                LinkId = "http://",
                Active = true,
                MerchantId = "visanetuy8",
                CybersourceProfileId = "hxuy857",
                CybersourceAccessKey = "2eb7f45bd63539678ef7d0fa4a47e424",
                CybersourceSecretKey = "11fa01fa2c82468da78a4c0e0fd9965bbb63b36e9f7947968c4e18cf0addaceaae41f04f88144a8d8f8d1229e2b24116b9b81fbdc9db4b7a82f30cb9f9557754df4bec65b8594295b7569b5e4f2d611edb1bea54faeb4520af1085d5e4701530cbe8fe1128104dcdbb165166591e9f2503221e4e05dc41d881a6fe66dfbbd612",
                CybersourceTransactionKey = "QVDS9IAorVdQO1dLgKeQsWuoTEXiwMg+8Q91uJ11cyRnU6orSs8w8tGjCOehYHceqR0Z7nQdOt5sPlhsfyynQmyC06laBU3iUAU+LfDw4CLYqHJ7QqC8yZr8qDvMJSdSQE+J3CPjsH+Rmz1HNdfHWiMEd5Tx+1e/rLrIrzaW3UgVo2GpEoaQIikhGSerbCu6e0KXgQ42gx7eaMOkSaDBWU/3GQkj+7VtcFgt+cy7DP52/1bCxRpYsvGErX9Gj/mDPczvqsBRK+LhNZtpwh6rZTQZ9MsKipgXwtyegJtOSKGiE04kt8vYDXfBm3pGN+GdXKGkTdeO+wpUokiLg1wLEg==",
                ReferenceParamName = "Número de cliente",
                CreationUser = "admin",
                LastModificationUser = "admin",
                CreationDate = DateTime.Today,
                LastModificationDate = DateTime.Today,
                EnableMultipleBills = false,
                Departament = DepartamentType.NotSucive,
                ExtractEmail = "sherrera@hexacta.com",
                EnableAutomaticPayment = true,
                EnablePublicPayment = true,
                EnablePrivatePayment = true,
                EnablePartialPayment = false,
                UrlName = "",
                DiscountType = DiscountType.FinancialInclusion,
                Container = false,
                EnableAssociation = true,
                AllowSelectContentAssociation = false,
                AllowSelectContentPayment = false,
                AskUserForReferences = true,
                AllowMultipleCards = false,
                IntroContent = "",
                MaxQuotaAllow = 1,
                ServiceGateways = new Collection<ServiceGateway>
                {
                    new ServiceGateway
                    {
                        Id = _serviceGateway2Id,
                        Active = true,
                        ReferenceId = "2062940",
                        ServiceType = "700",
                        SendExtract = true,
                        GatewayId = carreteraGateway.Id
                    }
                }
            };

            serviceList.Add(service1);
            serviceList.Add(service2);

            return serviceList;
        }

        private static IEnumerable<ServiceAssociated> ServicesAssociatedDataForTest()
        {
            var serviceAssociatedList = new List<ServiceAssociated>();

            var serviceAssociated1 = new ServiceAssociated
            {
                Id = ServiceAssociated1Id,
                Active = true,
                ServiceId = _service2Id,
                ReferenceNumber = "4",
                AutomaticPaymentId = _automaticPayment1Id,
                RegisteredUserId = _applicationUserId,
                NotificationConfigId = _notificationConfigId,
                Enabled = true,
                CreationUser = "test",
                LastModificationUser = "test",
                CreationDate = DateTime.Today,
                LastModificationDate = DateTime.Today,
                DefaultCardId = _card1Id,
                Description = "TEST_AUTOMATIC_PAYMENT"
            };

            var serviceAssociated2 = new ServiceAssociated
            {
                Id = ServiceAssociated2Id,
                Active = true,
                ServiceId = _service2Id,
                ReferenceNumber = "7",
                AutomaticPaymentId = null,
                RegisteredUserId = _applicationUserId,
                NotificationConfigId = _notificationConfigId,
                Enabled = true,
                CreationUser = "test",
                LastModificationUser = "test",
                CreationDate = DateTime.Today,
                LastModificationDate = DateTime.Today,
                DefaultCardId = _card1Id,
                Description = "TEST_AUTOMATIC_PAYMENT"
            };

            var serviceAssociated3 = new ServiceAssociated
            {
                Id = ServiceAssociated3Id,
                Active = true,
                ServiceId = _service2Id,
                ReferenceNumber = "3",
                AutomaticPaymentId = _automaticPayment3Id,
                RegisteredUserId = _applicationUserId,
                NotificationConfigId = _notificationConfigId,
                Enabled = true,
                CreationUser = "test",
                LastModificationUser = "test",
                CreationDate = DateTime.Today,
                LastModificationDate = DateTime.Today,
                DefaultCardId = _card1Id,
                Description = "TEST_AUTOMATIC_PAYMENT"
            };

            var serviceAssociated4 = new ServiceAssociated
            {
                Id = ServiceAssociated4Id,
                Active = true,
                ServiceId = _service2Id,
                ReferenceNumber = "6",
                AutomaticPaymentId = _automaticPayment4Id,
                RegisteredUserId = _applicationUserId,
                NotificationConfigId = _notificationConfigId,
                Enabled = true,
                CreationUser = "test",
                LastModificationUser = "test",
                CreationDate = DateTime.Today,
                LastModificationDate = DateTime.Today,
                DefaultCardId = _card1Id,
                Description = "TEST_AUTOMATIC_PAYMENT"
            };

            var serviceAssociated5 = new ServiceAssociated
            {
                Id = ServiceAssociated5Id,
                Active = true,
                ServiceId = _service2Id,
                ReferenceNumber = "9",
                AutomaticPaymentId = _automaticPayment5Id,
                RegisteredUserId = _applicationUserId,
                NotificationConfigId = _notificationConfigId,
                Enabled = true,
                CreationUser = "test",
                LastModificationUser = "test",
                CreationDate = DateTime.Today,
                LastModificationDate = DateTime.Today,
                DefaultCardId = _card2Id,
                Description = "TEST_AUTOMATIC_PAYMENT"
            };

            var serviceAssociated6 = new ServiceAssociated
            {
                Id = ServiceAssociated6Id,
                Active = true,
                ServiceId = _service1Id,
                ReferenceNumber = "10",
                AutomaticPaymentId = _automaticPayment6Id,
                RegisteredUserId = _applicationUserId,
                NotificationConfigId = _notificationConfigId,
                Enabled = true,
                CreationUser = "test",
                LastModificationUser = "test",
                CreationDate = DateTime.Today,
                LastModificationDate = DateTime.Today,
                DefaultCardId = _card1Id,
                Description = "TEST_AUTOMATIC_PAYMENT"
            };

            var serviceAssociated7 = new ServiceAssociated
            {
                Id = ServiceAssociated7Id,
                Active = true,
                ServiceId = _service2Id,
                ReferenceNumber = "11",
                AutomaticPaymentId = _automaticPayment7Id,
                RegisteredUserId = _applicationUserId,
                NotificationConfigId = _notificationConfigId,
                Enabled = true,
                CreationUser = "test",
                LastModificationUser = "test",
                CreationDate = DateTime.Today,
                LastModificationDate = DateTime.Today,
                DefaultCardId = _card1Id,
                Description = "TEST_AUTOMATIC_PAYMENT"
            };

            var serviceAssociated8 = new ServiceAssociated
            {
                Id = ServiceAssociated8Id,
                Active = true,
                ServiceId = _service2Id,
                ReferenceNumber = "12",
                AutomaticPaymentId = _automaticPayment8Id,
                RegisteredUserId = _applicationUserId,
                NotificationConfigId = _notificationConfigId,
                Enabled = true,
                CreationUser = "test",
                LastModificationUser = "test",
                CreationDate = DateTime.Today,
                LastModificationDate = DateTime.Today,
                DefaultCardId = _card1Id,
                Description = "TEST_AUTOMATIC_PAYMENT"
            };

            serviceAssociatedList.Add(serviceAssociated1);
            serviceAssociatedList.Add(serviceAssociated2);
            serviceAssociatedList.Add(serviceAssociated3);
            serviceAssociatedList.Add(serviceAssociated4);
            serviceAssociatedList.Add(serviceAssociated5);
            serviceAssociatedList.Add(serviceAssociated6);
            serviceAssociatedList.Add(serviceAssociated7);
            serviceAssociatedList.Add(serviceAssociated8);

            return serviceAssociatedList;
        }

        private static IEnumerable<Card> CardsDataForTest()
        {
            var cardList = new List<Card>();

            //var card1 = new Card
            //{
            //    Id = _card1Id,
            //    MaskedNumber = "411111xxxxxx1111",
            //    PaymentToken = "4798371386436060000000",
            //    DueDate = new DateTime(2020, 12, 01),
            //    CybersourceTransactionId = "4798371386436060000000",
            //    Active = true,
            //    Name = "Prueba Vigente",
            //    ExternalId = Guid.NewGuid(), //VER
            //    Deleted = false
            //};

            var card2 = new Card //Vencida
            {
                Id = _card2Id,
                MaskedNumber = "424242xxxxxx4242",
                PaymentToken = "4815627499446810000000",
                DueDate = new DateTime(2016, 01, 01),
                CybersourceTransactionId = "4815627499446810000000",
                Active = true,
                Name = "Prueba Vencida",
                ExternalId = Guid.NewGuid(), //VER
                Deleted = false
            };

            //cardList.Add(card1);
            cardList.Add(card2);

            return cardList;
        }

        private static NotificationConfig NotificationConfigDataForTest()
        {
            var notificationConfig = new NotificationConfig
            {
                Id = _notificationConfigId,
                DaysBeforeDueDate = 5,
                BeforeDueDateConfig = new DaysBeforeDueDateConfig
                {
                    Email = true,
                    Sms = true,
                    Web = true
                },
                SuccessPayment = new SuccessPayment
                {
                    Email = true,
                    Sms = true,
                    Web = true
                },
                ExpiredBill = new ExpiredBill
                {
                    Email = true,
                    Sms = true,
                    Web = true
                },
                FailedAutomaticPayment = new FailedAutomaticPayment
                {
                    Email = true,
                    Sms = true,
                    Web = true
                },
                NewBill = new NewBill
                {
                    Email = true,
                    Sms = true,
                    Web = true
                },
                ServiceAsociatedId = ServiceAssociated1Id
            };
            return notificationConfig;
        }

        private static IEnumerable<Domain.Entities.AutomaticPayment> AutomaticPaymentsDataForTest()
        {
            var automaticPaymentList = new List<Domain.Entities.AutomaticPayment>();

            var automaticPayment1 = new Domain.Entities.AutomaticPayment
            {
                Id = _automaticPayment1Id,
                DaysBeforeDueDate = 5,
                Maximum = 0,
                Quotas = 0,
                QuotasDone = 0,
                UnlimitedQuotas = true,
                SuciveAnnualPatent = false,
                UnlimitedAmount = true
            };

            var automaticPayment3 = new Domain.Entities.AutomaticPayment
            {
                Id = _automaticPayment3Id,
                DaysBeforeDueDate = 5,
                Maximum = 0,
                Quotas = 0,
                QuotasDone = 0,
                UnlimitedQuotas = true,
                SuciveAnnualPatent = false,
                UnlimitedAmount = true
            };

            var automaticPayment4 = new Domain.Entities.AutomaticPayment
            {
                Id = _automaticPayment4Id,
                DaysBeforeDueDate = 5,
                Maximum = 0,
                Quotas = 2,
                QuotasDone = 3,
                UnlimitedQuotas = false,
                SuciveAnnualPatent = false,
                UnlimitedAmount = true
            };

            var automaticPayment5 = new Domain.Entities.AutomaticPayment
            {
                Id = _automaticPayment5Id,
                DaysBeforeDueDate = 5,
                Maximum = 0,
                Quotas = 0,
                QuotasDone = 0,
                UnlimitedQuotas = true,
                SuciveAnnualPatent = false,
                UnlimitedAmount = true
            };

            var automaticPayment6 = new Domain.Entities.AutomaticPayment
            {
                Id = _automaticPayment6Id,
                DaysBeforeDueDate = 5,
                Maximum = 0,
                Quotas = 0,
                QuotasDone = 0,
                UnlimitedQuotas = true,
                SuciveAnnualPatent = false,
                UnlimitedAmount = true
            };

            var automaticPayment7 = new Domain.Entities.AutomaticPayment
            {
                Id = _automaticPayment7Id,
                DaysBeforeDueDate = 5,
                Maximum = 0,
                Quotas = 0,
                QuotasDone = 0,
                UnlimitedQuotas = true,
                SuciveAnnualPatent = false,
                UnlimitedAmount = true
            };

            var automaticPayment8 = new Domain.Entities.AutomaticPayment
            {
                Id = _automaticPayment8Id,
                DaysBeforeDueDate = 5,
                Maximum = 10000,
                Quotas = 0,
                QuotasDone = 0,
                UnlimitedQuotas = true,
                SuciveAnnualPatent = false,
                UnlimitedAmount = false
            };

            automaticPaymentList.Add(automaticPayment1);
            automaticPaymentList.Add(automaticPayment3);
            automaticPaymentList.Add(automaticPayment4);
            automaticPaymentList.Add(automaticPayment5);
            automaticPaymentList.Add(automaticPayment6);
            automaticPaymentList.Add(automaticPayment7);
            automaticPaymentList.Add(automaticPayment8);

            return automaticPaymentList;
        }

        private static IEnumerable<HighwayEmail> HighwayEmailDataForTest()
        {
            var highwayEmailList = new List<HighwayEmail>();

            var highwayEmail1 = new HighwayEmail
            {
                Id = _highwayEmail1Id,
                RecipientEmail = "",
                Subject = "",
                AttachmentInputName = "ENVIO_VNP_2062940_700_20161212.txt",
                AttachmentOutputName = "ENVIO_VNP_2062940_700_20161212_1000102.txt",
                TimeStampSeconds = "",
                CodCommerce = 2062940,
                CodBranch = 700,
                Status = HighwayEmailStatus.Accepted,
                CreationUser = "test",
                LastModificationUser = "test",
                CreationDate = DateTime.Now,
                LastModificationDate = DateTime.Now,
                Sender = "",
                AttachmentCreationDate = DateTime.Now,
                Transaccion = 1000102,
                Type = HighwayEmailType.Manual,
                ClientIp = "1.1.1.1",
                ServiceId = _service1Id
            };

            var highwayEmail2 = new HighwayEmail
            {
                Id = _highwayEmail2Id,
                RecipientEmail = "",
                Subject = "",
                AttachmentInputName = "ENVIO_VNP_2062940_700_20161212.txt",
                AttachmentOutputName = "ENVIO_VNP_2062940_700_20161212_1000102.txt",
                TimeStampSeconds = "",
                CodCommerce = 2062940,
                CodBranch = 700,
                Status = HighwayEmailStatus.Accepted,
                CreationUser = "test",
                LastModificationUser = "test",
                CreationDate = DateTime.Now,
                LastModificationDate = DateTime.Now,
                Sender = "",
                AttachmentCreationDate = DateTime.Now,
                Transaccion = 1000102,
                Type = HighwayEmailType.Manual,
                ClientIp = "1.1.1.1",
                ServiceId = _service2Id
            };

            highwayEmailList.Add(highwayEmail1);
            highwayEmailList.Add(highwayEmail2);

            return highwayEmailList;
        }

        private static IEnumerable<HighwayBill> HighwayBillsForTest()
        {
            var billsList = new List<HighwayBill>();

            var bill1 = new HighwayBill
            {
                Id = _highwayBill1Id,
                CreationDate = DateTime.Now,
                LastModificationDate = DateTime.Now,
                CreationUser = "test",
                CodComercio = 2062940,
                CodSucursal = 700,
                RefCliente = "4",
                NroFactura = "APTest 4.1",
                Descripcion = "Desc 1",
                FchFactura = DateTime.Today.AddDays(-1),
                FchVencimiento = DateTime.Today.AddDays(-1),
                DiasPagoVenc = 365,
                Moneda = "UYU",
                MontoTotal = 6062.09,
                MontoGravado = 4968.93,
                MontoMinimo = 6062.09,
                ConsFinal = true,
                Cuotas = 1,
                PagoDebito = false,
                Type = HighwayBillType.Pending,
                ErrorCode = 0,
                HighwayEmailId = _highwayEmail2Id,
                ServiceId = _service2Id
            };

            var bill2 = new HighwayBill
            {
                Id = _highwayBill2Id,
                CreationDate = DateTime.Now,
                LastModificationDate = DateTime.Now,
                CreationUser = "test",
                CodComercio = 2062940,
                CodSucursal = 700,
                RefCliente = "5",
                NroFactura = "APTest 5.1",
                Descripcion = "Desc 1",
                FchFactura = DateTime.Today.AddDays(-2),
                FchVencimiento = DateTime.Today.AddDays(-2),
                DiasPagoVenc = 365,
                Moneda = "UYU",
                MontoTotal = 6062.09,
                MontoGravado = 4968.93,
                MontoMinimo = 6062.09,
                ConsFinal = true,
                Cuotas = 1,
                PagoDebito = false,
                Type = HighwayBillType.Pending,
                ErrorCode = 0,
                HighwayEmailId = _highwayEmail2Id,
                ServiceId = _service2Id
            };

            var bill3 = new HighwayBill
            {
                Id = _highwayBill3Id,
                CreationDate = DateTime.Now,
                LastModificationDate = DateTime.Now,
                CreationUser = "test",
                CodComercio = 2062940,
                CodSucursal = 700,
                RefCliente = "6",
                NroFactura = "APTest 6.1",
                Descripcion = "Desc 1",
                FchFactura = DateTime.Today.AddDays(-3),
                FchVencimiento = DateTime.Today.AddDays(-3),
                DiasPagoVenc = 365,
                Moneda = "UYU",
                MontoTotal = 6062.09,
                MontoGravado = 4968.93,
                MontoMinimo = 6062.09,
                ConsFinal = true,
                Cuotas = 1,
                PagoDebito = false,
                Type = HighwayBillType.Pending,
                ErrorCode = 0,
                HighwayEmailId = _highwayEmail2Id,
                ServiceId = _service2Id
            };

            var bill4 = new HighwayBill
            {
                Id = _highwayBill4Id,
                CreationDate = DateTime.Now,
                LastModificationDate = DateTime.Now,
                CreationUser = "test",
                CodComercio = 2062940,
                CodSucursal = 700,
                RefCliente = "7",
                NroFactura = "APTest 7.1",
                Descripcion = "Desc 1",
                FchFactura = DateTime.Today.AddDays(-4),
                FchVencimiento = DateTime.Today.AddDays(-4),
                DiasPagoVenc = 365,
                Moneda = "UYU",
                MontoTotal = 6062.09,
                MontoGravado = 4968.93,
                MontoMinimo = 6062.09,
                ConsFinal = true,
                Cuotas = 1,
                PagoDebito = false,
                Type = HighwayBillType.Pending,
                ErrorCode = 0,
                HighwayEmailId = _highwayEmail2Id,
                ServiceId = _service2Id
            };

            var bill5 = new HighwayBill
            {
                Id = _highwayBill5Id,
                CreationDate = DateTime.Now,
                LastModificationDate = DateTime.Now,
                CreationUser = "test",
                CodComercio = 2062940,
                CodSucursal = 700,
                RefCliente = "9",
                NroFactura = "APTest 9.1",
                Descripcion = "Desc 1",
                FchFactura = DateTime.Today.AddDays(-7),
                FchVencimiento = DateTime.Today.AddDays(-7),
                DiasPagoVenc = 365,
                Moneda = "UYU",
                MontoTotal = 6062.09,
                MontoGravado = 4968.93,
                MontoMinimo = 6062.09,
                ConsFinal = true,
                Cuotas = 1,
                PagoDebito = false,
                Type = HighwayBillType.Pending,
                ErrorCode = 0,
                HighwayEmailId = _highwayEmail2Id,
                ServiceId = _service2Id
            };

            var bill6 = new HighwayBill
            {
                Id = _highwayBill6Id,
                CreationDate = DateTime.Now,
                LastModificationDate = DateTime.Now,
                CreationUser = "test",
                CodComercio = 2062940,
                CodSucursal = 700,
                RefCliente = "10",
                NroFactura = "APTest 10.1",
                Descripcion = "Desc 1",
                FchFactura = DateTime.Today.AddDays(-8),
                FchVencimiento = DateTime.Today.AddDays(-8),
                DiasPagoVenc = 365,
                Moneda = "UYU",
                MontoTotal = 6062.09,
                MontoGravado = 4968.93,
                MontoMinimo = 6062.09,
                ConsFinal = true,
                Cuotas = 1,
                PagoDebito = false,
                Type = HighwayBillType.Pending,
                ErrorCode = 0,
                HighwayEmailId = _highwayEmail1Id,
                ServiceId = _service1Id
            };

            var bill7 = new HighwayBill
            {
                Id = _highwayBill7Id,
                CreationDate = DateTime.Now,
                LastModificationDate = DateTime.Now,
                CreationUser = "test",
                CodComercio = 2062940,
                CodSucursal = 700,
                RefCliente = "11",
                NroFactura = "APTest 11.1",
                Descripcion = "Desc 1",
                FchFactura = DateTime.Today.AddDays(-9),
                FchVencimiento = DateTime.Today.AddDays(-9),
                DiasPagoVenc = 365,
                Moneda = "UYU",
                MontoTotal = 0,
                MontoGravado = 0,
                MontoMinimo = 0,
                ConsFinal = true,
                Cuotas = 1,
                PagoDebito = false,
                Type = HighwayBillType.Pending,
                ErrorCode = 0,
                HighwayEmailId = _highwayEmail2Id,
                ServiceId = _service2Id
            };

            var bill8 = new HighwayBill
            {
                Id = _highwayBill8Id,
                CreationDate = DateTime.Now,
                LastModificationDate = DateTime.Now,
                CreationUser = "test",
                CodComercio = 2062940,
                CodSucursal = 700,
                RefCliente = "12",
                NroFactura = "APTest 12.1",
                Descripcion = "Desc 1",
                FchFactura = DateTime.Today.AddDays(5),
                FchVencimiento = DateTime.Today.AddDays(5),
                DiasPagoVenc = 365,
                Moneda = "UYU",
                MontoTotal = 16062.09, //supera monto configurado
                MontoGravado = 9968.93,
                MontoMinimo = 16062.09,
                ConsFinal = true,
                Cuotas = 1,
                PagoDebito = false,
                Type = HighwayBillType.Pending,
                ErrorCode = 0,
                HighwayEmailId = _highwayEmail2Id,
                ServiceId = _service2Id
            };

            var bill9 = new HighwayBill
            {
                Id = _highwayBill9Id,
                CreationDate = DateTime.Now,
                LastModificationDate = DateTime.Now,
                CreationUser = "test",
                CodComercio = 2062940,
                CodSucursal = 700,
                RefCliente = "12",
                NroFactura = "APTest 12.2",
                Descripcion = "Desc 1",
                FchFactura = DateTime.Today.AddDays(5),
                FchVencimiento = DateTime.Today.AddDays(5),
                DiasPagoVenc = 365,
                Moneda = "UYU",
                MontoTotal = 0,
                MontoGravado = 0,
                MontoMinimo = 0,
                ConsFinal = true,
                Cuotas = 1,
                PagoDebito = false,
                Type = HighwayBillType.Pending,
                ErrorCode = 0,
                HighwayEmailId = _highwayEmail2Id,
                ServiceId = _service2Id
            };

            var bill10 = new HighwayBill
            {
                Id = _highwayBill10Id,
                CreationDate = DateTime.Now,
                LastModificationDate = DateTime.Now,
                CreationUser = "test",
                CodComercio = 2062940,
                CodSucursal = 700,
                RefCliente = "12",
                NroFactura = "APTest 12.3",
                Descripcion = "Desc 1",
                FchFactura = DateTime.Today.AddDays(5),
                FchVencimiento = DateTime.Today.AddDays(5),
                DiasPagoVenc = 365,
                Moneda = "UYU",
                MontoTotal = 6062.09,
                MontoGravado = 4968.93,
                MontoMinimo = 6062.09,
                ConsFinal = true,
                Cuotas = 1,
                PagoDebito = false,
                Type = HighwayBillType.Pending,
                ErrorCode = 0,
                HighwayEmailId = _highwayEmail2Id,
                ServiceId = _service2Id
            };

            var bill11 = new HighwayBill
            {
                Id = _highwayBill11Id,
                CreationDate = DateTime.Now,
                LastModificationDate = DateTime.Now,
                CreationUser = "test",
                CodComercio = 2062940,
                CodSucursal = 700,
                RefCliente = "12",
                NroFactura = "APTest 12.4",
                Descripcion = "Desc 1",
                FchFactura = DateTime.Today.AddDays(10),
                FchVencimiento = DateTime.Today.AddDays(10),
                DiasPagoVenc = 365,
                Moneda = "UYU",
                MontoTotal = 6062.09,
                MontoGravado = 4968.93,
                MontoMinimo = 6062.09,
                ConsFinal = true,
                Cuotas = 1,
                PagoDebito = false,
                Type = HighwayBillType.Pending,
                ErrorCode = 0,
                HighwayEmailId = _highwayEmail2Id,
                ServiceId = _service2Id
            };

            billsList.Add(bill1);
            billsList.Add(bill2);
            billsList.Add(bill3);
            billsList.Add(bill4);
            billsList.Add(bill5);
            billsList.Add(bill6);
            billsList.Add(bill7);
            billsList.Add(bill8);
            billsList.Add(bill9);
            billsList.Add(bill10);
            billsList.Add(bill11);

            return billsList;
        }

        protected static void CleanDataForTests()
        {
            var processHistoriesIds =
                _repositoryProcessHistory.All(x =>
                    x.Additional != null && x.Additional.ToUpper() == "TEST_AUTOMATIC_PAYMENT").Select(x => x.Id).ToList();

            var servicesAssociatedIds =
                _repositoryServiceAsociated.All(x =>
                    x.Description != null && x.Description.ToUpper() == "TEST_AUTOMATIC_PAYMENT").Select(x => x.Id).ToList();

            var paymentsDoneIds =
                    _repositoryPayment.All(x =>
                        servicesAssociatedIds.Contains((Guid)x.ServiceAssosiatedId)).Select(x => x.Id).ToList();

            var billsPayedIds =
                    _repositoryBill.All(x =>
                        paymentsDoneIds.Contains((Guid)x.PaymentId)).Select(x => x.Id).ToList();

            var newBillNotificationIds =
                    _repositoryNewBillNotificationInfo.All(x =>
                        x.BillNumber != null && x.BillNumber.ToUpper().StartsWith("APTEST")).Select(x => x.Id).ToList();

            var highwayBillsIds =
                    _repositoryHighwayBill.All(x =>
                        x.NroFactura != null && x.NroFactura.ToUpper().StartsWith("APTEST")).Select(x => x.Id).ToList();

            var services =
                    _repositoryService.All(x =>
                        x.Description != null && x.Description.ToUpper() == "TEST_AUTOMATIC_PAYMENT", s => s.ServiceGateways).ToList();

            var servicesIds = services.Select(x => x.Id).ToList();

            var notificationsIds =
                    _repositoryNotification.All(x =>
                        servicesIds.Contains((Guid)x.ServiceId)).Select(x => x.Id).ToList();

            using (var context = new AppContext())
            {
                var card2 = context.Set<Card>().Find(_card2Id);
                context.Entry(card2).State = EntityState.Deleted;

                var notificationConfig = context.Set<NotificationConfig>().Find(_notificationConfigId);
                context.Entry(notificationConfig).State = EntityState.Deleted;

                var highwayEmail1 = context.Set<HighwayEmail>().Find(_highwayEmail1Id);
                context.Entry(highwayEmail1).State = EntityState.Deleted;
                var highwayEmail2 = context.Set<HighwayEmail>().Find(_highwayEmail2Id);
                context.Entry(highwayEmail2).State = EntityState.Deleted;

                var binGroup = context.Set<BinGroup>().Find(_binGroupId);
                foreach (var service in binGroup.Services.ToList())
                {
                    if (service.Description != null && service.Description.ToUpper() == "TEST_AUTOMATIC_PAYMENT")
                    {
                        binGroup.Services.Remove(service);
                    }
                }

                //Se controla si es null porque algunos resultados borran el pago programado
                var automaticPayment1 = context.Set<Domain.Entities.AutomaticPayment>().Find(_automaticPayment1Id);
                if (automaticPayment1 != null)
                    context.Entry(automaticPayment1).State = EntityState.Deleted;
                var automaticPayment3 = context.Set<Domain.Entities.AutomaticPayment>().Find(_automaticPayment3Id);
                if (automaticPayment3 != null)
                    context.Entry(automaticPayment3).State = EntityState.Deleted;
                var automaticPayment4 = context.Set<Domain.Entities.AutomaticPayment>().Find(_automaticPayment4Id);
                if (automaticPayment4 != null)
                    context.Entry(automaticPayment4).State = EntityState.Deleted;
                var automaticPayment5 = context.Set<Domain.Entities.AutomaticPayment>().Find(_automaticPayment5Id);
                if (automaticPayment5 != null)
                    context.Entry(automaticPayment5).State = EntityState.Deleted;
                var automaticPayment6 = context.Set<Domain.Entities.AutomaticPayment>().Find(_automaticPayment6Id);
                if (automaticPayment6 != null)
                    context.Entry(automaticPayment6).State = EntityState.Deleted;
                var automaticPayment7 = context.Set<Domain.Entities.AutomaticPayment>().Find(_automaticPayment7Id);
                if (automaticPayment7 != null)
                    context.Entry(automaticPayment7).State = EntityState.Deleted;
                var automaticPayment8 = context.Set<Domain.Entities.AutomaticPayment>().Find(_automaticPayment8Id);
                if (automaticPayment8 != null)
                    context.Entry(automaticPayment8).State = EntityState.Deleted;

                foreach (var id in processHistoriesIds)
                {
                    var processHistory = context.Set<ProcessHistory>().Find(id);
                    context.Entry(processHistory).State = EntityState.Deleted;
                }

                foreach (var id in servicesAssociatedIds)
                {
                    var serviceAssociated = context.Set<ServiceAssociated>().Find(id);
                    context.Entry(serviceAssociated).State = EntityState.Deleted;
                }

                foreach (var id in paymentsDoneIds)
                {
                    var payment = context.Set<Payment>().Find(id);
                    context.Entry(payment).State = EntityState.Deleted;
                }

                foreach (var id in billsPayedIds)
                {
                    var bill = context.Set<Bill>().Find(id);
                    context.Entry(bill).State = EntityState.Deleted;
                }

                foreach (var id in notificationsIds)
                {
                    var notification = context.Set<Notification>().Find(id);
                    context.Entry(notification).State = EntityState.Deleted;
                }

                foreach (var id in newBillNotificationIds)
                {
                    var newBillNotificationInfo = context.Set<NewBillNotificationInfo>().Find(id);
                    context.Entry(newBillNotificationInfo).State = EntityState.Deleted;
                }

                foreach (var serv in services)
                {
                    var servGateway = serv.ServiceGateways.FirstOrDefault();
                    if (servGateway != null)
                    {
                        var serviceGateway = context.Set<ServiceGateway>().Find(servGateway.Id);
                        context.Entry(serviceGateway).State = EntityState.Deleted;
                    }

                    var service = context.Set<Service>().Find(serv.Id);
                    context.Entry(service).State = EntityState.Deleted;
                }

                foreach (var id in highwayBillsIds)
                {
                    var highwayBill = context.Set<HighwayBill>().Find(id);
                    context.Entry(highwayBill).State = EntityState.Deleted;
                }

                context.SaveChanges();
            }
        }

        protected static bool VerifyUserWithCardExists()
        {
            var exists = false;
            var emailForTests = ConfigurationManager.AppSettings["EmailForTests"];
            var appUser = _repositoryApplicationUser.All(null, x => x.Cards).FirstOrDefault(x => x.Email == emailForTests);
            if (appUser != null)
            {
                var card1 = appUser.Cards.FirstOrDefault(x => x.Active && !x.Deleted && x.MaskedNumber == "411111xxxxxx1111");
                if (card1 != null)
                {
                    exists = true;
                }
            }
            return exists;
        }

        protected static bool VerifyBinGroupExists()
        {
            var exists = false;
            var binGroups = _repositoryBinGroup.All(null, x => x.Bins).ToList();
            if (binGroups.Any())
            {
                var binGroup = binGroups.FirstOrDefault(x => x.Bins != null && x.Bins.Any(b => b.Value == 411111));
                if (binGroup != null)
                {
                    exists = true;
                }
            }
            return exists;
        }

        protected static bool VerifyDiscountExists()
        {
            var exists = false;
            var discounts = _repositoryDiscount.All().ToList();
            if (discounts.Any())
            {
                var discount = discounts.FirstOrDefault(x =>
                    x.DiscountType == DiscountType.FinancialInclusion && x.CardType == CardType.Credit &&
                    x.From < DateTime.Now && x.To >= DateTime.Now);
                if (discount != null)
                {
                    exists = true;
                }
            }
            return exists;
        }

    }
}