using System;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;
using VisaNet.Application.Interfaces;
using VisaNet.AutomaticPaymentTests;
using VisaNet.Common.DependencyInjection;
using VisaNet.ConsoleApplication.PaymentProcess.PaymentsProcessor;
using VisaNet.Domain.Entities;
using VisaNet.Domain.Entities.BatchProcesses.AutomaticPaymentProcess.Interfaces;
using VisaNet.Domain.Entities.Enums;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Repository.Implementations.Base;
using VisaNet.Repository.Interfaces.Interfaces;

namespace VisaNet.Testing.AutomaticPayment.Tests
{
    [TestClass]
    public class RetryRunTests : AutomaticPaymentTests
    {
        private readonly IStateFactory _stateFactory;
        private readonly ILoggerHelper _loggerHelper;
        private readonly IValidationHelper _validationHelper;
        private readonly ISystemNotificationFactory _systemNotificationFactory;
        private readonly IServiceServiceAssosiate _serviceServiceAssosiate;
        private readonly IRepositoryProcessHistory _repositoryProcessHistory;

        private static bool _userWithCardExists;
        private static bool _binGroupExists;
        private static bool _discountExists;

        public RetryRunTests()
        {
            var kernel = new StandardKernel();
            NinjectRegister.Register(kernel, true);

            _loggerHelper = NinjectRegister.Get<ILoggerHelper>();
            _validationHelper = NinjectRegister.Get<IValidationHelper>();
            _systemNotificationFactory = NinjectRegister.Get<ISystemNotificationFactory>();
            _serviceServiceAssosiate = NinjectRegister.Get<IServiceServiceAssosiate>();
            _repositoryProcessHistory = NinjectRegister.Get<IRepositoryProcessHistory>();
            _stateFactory = NinjectRegister.Get<TestStateFactory>();
        }

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            _userWithCardExists = VerifyUserWithCardExists();
            _binGroupExists = VerifyBinGroupExists();
            _discountExists = VerifyDiscountExists();
            if (_userWithCardExists && _binGroupExists && _discountExists)
            {
                LoadDataForTests();
            }
        }

        [TestMethod]
        public void RetryRun_Test()
        {
            //Pre-condicion: que no hayan corridas del proceso para el día de hoy

            Assert.AreEqual(true, _userWithCardExists);
            Assert.AreEqual(true, _binGroupExists);
            Assert.AreEqual(true, _discountExists);

            // Arrange
            var automaticPaymentHandler = new AutomaticPaymentHandler(_stateFactory, _loggerHelper, _validationHelper, _systemNotificationFactory);

            // Act
            LoadFirstRunData(); //First run
            automaticPaymentHandler.Start(); //Retry run

            var firstRun =
                _repositoryProcessHistory.All(null, x => x.PendingAutomaticPayments).FirstOrDefault(x =>
                    x.Additional == "TEST_AUTOMATIC_PAYMENT" && x.Count == 1);

            var serviceAssociate1 = _serviceServiceAssosiate.GetById(ServiceAssociated1Id, x => x.AutomaticPayment);
            var serviceAssociate3 = _serviceServiceAssosiate.GetById(ServiceAssociated3Id, x => x.AutomaticPayment);
            var serviceAssociate4 = _serviceServiceAssosiate.GetById(ServiceAssociated4Id, x => x.AutomaticPayment);
            var serviceAssociate5 = _serviceServiceAssosiate.GetById(ServiceAssociated5Id, x => x.AutomaticPayment);
            var serviceAssociate6 = _serviceServiceAssosiate.GetById(ServiceAssociated6Id, x => x.AutomaticPayment);
            var serviceAssociate7 = _serviceServiceAssosiate.GetById(ServiceAssociated7Id, x => x.AutomaticPayment);
            var serviceAssociate8 = _serviceServiceAssosiate.GetById(ServiceAssociated8Id, x => x.AutomaticPayment);

            var pendingService1 = firstRun.PendingAutomaticPayments.FirstOrDefault(x =>
                x.PendingServiceAssociateId == ServiceAssociated1Id);
            var pendingService2 = firstRun.PendingAutomaticPayments.FirstOrDefault(x =>
                x.PendingServiceAssociateId == ServiceAssociated2Id);
            var pendingService3 = firstRun.PendingAutomaticPayments.FirstOrDefault(x =>
                x.PendingServiceAssociateId == ServiceAssociated3Id);
            var pendingService4 = firstRun.PendingAutomaticPayments.FirstOrDefault(x =>
                x.PendingServiceAssociateId == ServiceAssociated4Id);
            var pendingService5 = firstRun.PendingAutomaticPayments.FirstOrDefault(x =>
                x.PendingServiceAssociateId == ServiceAssociated5Id);
            var pendingService6 = firstRun.PendingAutomaticPayments.FirstOrDefault(x =>
                x.PendingServiceAssociateId == ServiceAssociated6Id);
            var pendingService7 = firstRun.PendingAutomaticPayments.FirstOrDefault(x =>
                x.PendingServiceAssociateId == ServiceAssociated7Id);
            var pendingService8 = firstRun.PendingAutomaticPayments.FirstOrDefault(x =>
                x.PendingServiceAssociateId == ServiceAssociated8Id);

            // Asserts
            Assert.AreEqual(PaymentResultTypeDto.NoBills, serviceAssociate3.AutomaticPaymentDto.LastRunResult);
            Assert.AreEqual(PaymentResultType.NoBills, pendingService3.Result);

            Assert.AreEqual(PaymentResultTypeDto.InvalidCardDueDate, serviceAssociate5.AutomaticPaymentDto.LastRunResult);
            Assert.AreEqual(PaymentResultTypeDto.InvalidCardDueDate, serviceAssociate5.AutomaticPaymentDto.LastErrorResult);
            Assert.AreEqual(PaymentResultType.InvalidCardDueDate, pendingService5.Result);

            Assert.AreEqual(PaymentResultType.AutomaticPaymentDisabled, pendingService2.Result);

            Assert.AreEqual(PaymentResultTypeDto.ServiceNotAllowsAutomaticPayment, serviceAssociate6.AutomaticPaymentDto.LastRunResult);
            Assert.AreEqual(PaymentResultTypeDto.ServiceNotAllowsAutomaticPayment, serviceAssociate6.AutomaticPaymentDto.LastErrorResult);
            Assert.AreEqual(PaymentResultType.ServiceNotAllowsAutomaticPayment, pendingService6.Result);

            Assert.AreEqual(PaymentResultTypeDto.ExceededPaymentsCount, serviceAssociate4.AutomaticPaymentDto.LastRunResult);
            Assert.AreEqual(PaymentResultTypeDto.ExceededPaymentsCount, serviceAssociate4.AutomaticPaymentDto.LastErrorResult);
            Assert.AreEqual(PaymentResultType.ExceededPaymentsCount, pendingService4.Result);

            Assert.AreEqual(PaymentResultTypeDto.InvalidModel, serviceAssociate7.AutomaticPaymentDto.LastRunResult);
            Assert.AreEqual(PaymentResultTypeDto.InvalidModel, serviceAssociate7.AutomaticPaymentDto.LastErrorResult);
            Assert.AreEqual(PaymentResultType.InvalidModel, pendingService7.Result);

            Assert.AreEqual(PaymentResultTypeDto.Success, serviceAssociate1.AutomaticPaymentDto.LastRunResult);
            Assert.AreEqual(PaymentResultType.Success, pendingService1.Result);

            Assert.AreEqual(PaymentResultTypeDto.ServiceErrorControlled, serviceAssociate8.AutomaticPaymentDto.LastRunResult);
            Assert.AreEqual(PaymentResultTypeDto.ServiceErrorControlled, serviceAssociate8.AutomaticPaymentDto.LastErrorResult);
            Assert.AreEqual(PaymentResultType.ServiceErrorControlled, pendingService8.Result);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            if (_userWithCardExists && _binGroupExists && _discountExists)
            {
                CleanDataForTests();
            }
        }

        private void LoadFirstRunData()
        {
            using (var context = new AppContext())
            {
                var processHistoryTable = context.Set<ProcessHistory>();

                var processHistory = ProcessHistoryForTest();
                processHistoryTable.Add(processHistory);

                context.SaveChanges();
            }
        }

        private ProcessHistory ProcessHistoryForTest()
        {
            var processHistoryId = Guid.NewGuid();

            var processHistory = new ProcessHistory
            {
                Id = processHistoryId,
                Count = 1,
                Process = ProcessType.AutomaticPayment,
                Status = ProcessStatus.Error,
                Additional = "TEST_AUTOMATIC_PAYMENT",
                CreationDate = DateTime.Now,
                LastModificationDate = DateTime.Now,
                CreationUser = "test",
                LastModificationUser = "test",
                PendingAutomaticPayments = new Collection<PendingAutomaticPayment>
                    {
                        new PendingAutomaticPayment
                        {
                            Id = Guid.NewGuid(),
                            Date = DateTime.Now,
                            ProcessHistoryId = processHistoryId,
                            LastProcessHistoryId = processHistoryId,
                            PendingServiceAssociateId = ServiceAssociated1Id,
                            Result = PaymentResultType.ServiceErrorRetry
                        },
                        new PendingAutomaticPayment
                        {
                            Id = Guid.NewGuid(),
                            Date = DateTime.Now,
                            ProcessHistoryId = processHistoryId,
                            LastProcessHistoryId = processHistoryId,
                            PendingServiceAssociateId = ServiceAssociated2Id,
                            Result = PaymentResultType.ServiceErrorRetry
                        },
                        new PendingAutomaticPayment
                        {
                            Id = Guid.NewGuid(),
                            Date = DateTime.Now,
                            ProcessHistoryId = processHistoryId,
                            LastProcessHistoryId = processHistoryId,
                            PendingServiceAssociateId = ServiceAssociated3Id,
                            Result = PaymentResultType.ServiceErrorRetry
                        },
                        new PendingAutomaticPayment
                        {
                            Id = Guid.NewGuid(),
                            Date = DateTime.Now,
                            ProcessHistoryId = processHistoryId,
                            LastProcessHistoryId = processHistoryId,
                            PendingServiceAssociateId = ServiceAssociated4Id,
                            Result = PaymentResultType.ServiceErrorRetry
                        },
                        new PendingAutomaticPayment
                        {
                            Id = Guid.NewGuid(),
                            Date = DateTime.Now,
                            ProcessHistoryId = processHistoryId,
                            LastProcessHistoryId = processHistoryId,
                            PendingServiceAssociateId = ServiceAssociated5Id,
                            Result = PaymentResultType.ServiceErrorRetry
                        },
                        new PendingAutomaticPayment
                        {
                            Id = Guid.NewGuid(),
                            Date = DateTime.Now,
                            ProcessHistoryId = processHistoryId,
                            LastProcessHistoryId = processHistoryId,
                            PendingServiceAssociateId = ServiceAssociated6Id,
                            Result = PaymentResultType.ServiceErrorRetry
                        },
                        new PendingAutomaticPayment
                        {
                            Id = Guid.NewGuid(),
                            Date = DateTime.Now,
                            ProcessHistoryId = processHistoryId,
                            LastProcessHistoryId = processHistoryId,
                            PendingServiceAssociateId = ServiceAssociated7Id,
                            Result = PaymentResultType.ServiceErrorRetry
                        },
                        new PendingAutomaticPayment
                        {
                            Id = Guid.NewGuid(),
                            Date = DateTime.Now,
                            ProcessHistoryId = processHistoryId,
                            LastProcessHistoryId = processHistoryId,
                            PendingServiceAssociateId = ServiceAssociated8Id,
                            Result = PaymentResultType.ServiceErrorRetry
                        }
                    }
            };
            return processHistory;
        }

    }
}