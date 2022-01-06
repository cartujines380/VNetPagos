using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;
using VisaNet.Application.Interfaces;
using VisaNet.Common.DependencyInjection;
using VisaNet.Domain.Entities.BatchProcesses.AutomaticPaymentProcess.Implementations;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Testing.AutomaticPayment.Tests
{
    [TestClass]
    public class NotificationTests
    {
        private readonly IServiceServiceAssosiate _serviceServiceAssosiate;
        private readonly IServiceProcessHistory _processHistoryService;

        public NotificationTests()
        {
            var kernel = new StandardKernel();
            NinjectRegister.Register(kernel, true);

            _serviceServiceAssosiate = NinjectRegister.Get<IServiceServiceAssosiate>();
            _processHistoryService = NinjectRegister.Get<IServiceProcessHistory>();
        }

        [TestMethod]
        public void FirstRun_SendNotifications_True()
        {
            //En la primer corrida se notifica al usuario si no ocurrieron errores de reintento

            // Arrange
            var firstRunState = new FirstRunState(_serviceServiceAssosiate, _processHistoryService);

            var billResultsDictionary = new Dictionary<Guid, PaymentResultTypeDto>
            {
                {Guid.NewGuid(), PaymentResultTypeDto.Success}, //ok
                {Guid.NewGuid(), PaymentResultTypeDto.ExceededPaymentsAmount}, //error controlado
                {Guid.NewGuid(), PaymentResultTypeDto.Success}, //ok
                {Guid.NewGuid(), PaymentResultTypeDto.ExceededPaymentsCount}, //error controlado
                {Guid.NewGuid(), PaymentResultTypeDto.BillExpired}, //error controlado
                {Guid.NewGuid(), PaymentResultTypeDto.CsExpiredCard}, //error controlado
            };

            // Act
            firstRunState.SetNotificationFlag(billResultsDictionary);

            // Asserts
            Assert.AreEqual(true, firstRunState.ShouldNotifyUser);
        }

        [TestMethod]
        public void FirstRun_SendNotifications_False()
        {
            //En la primer corrida si ocurre algun error de reintento no se notifica al usuario

            // Arrange
            var firstRunState = new FirstRunState(_serviceServiceAssosiate, _processHistoryService);

            var billResultsDictionary = new Dictionary<Guid, PaymentResultTypeDto>
            {
                {Guid.NewGuid(), PaymentResultTypeDto.Success}, //ok
                {Guid.NewGuid(), PaymentResultTypeDto.Success}, //ok
                {Guid.NewGuid(), PaymentResultTypeDto.UnhandledException}, //error de reintento
                {Guid.NewGuid(), PaymentResultTypeDto.Success}, //ok
                {Guid.NewGuid(), PaymentResultTypeDto.ExceededPaymentsAmount}, //error controlado
                {Guid.NewGuid(), PaymentResultTypeDto.ExceededPaymentsCount}, //error controlado
                {Guid.NewGuid(), PaymentResultTypeDto.CsExpiredCard}, //error controlado
            };

            // Act
            firstRunState.SetNotificationFlag(billResultsDictionary);

            // Asserts
            Assert.AreEqual(false, firstRunState.ShouldNotifyUser);
        }

        [TestMethod]
        public void FirstRun_SendNotifications_True_Then_False()
        {
            //Se prueba que notifica a un usuario y al siguiente no (porque son independientes)

            // Arrange
            var firstRunState = new FirstRunState(_serviceServiceAssosiate, _processHistoryService);

            var billResultsDictionaryCaseTrue = new Dictionary<Guid, PaymentResultTypeDto>
            {
                {Guid.NewGuid(), PaymentResultTypeDto.Success}, //ok
                {Guid.NewGuid(), PaymentResultTypeDto.ExceededPaymentsAmount}, //error controlado
                {Guid.NewGuid(), PaymentResultTypeDto.Success}, //ok
                {Guid.NewGuid(), PaymentResultTypeDto.ExceededPaymentsCount}, //error controlado
                {Guid.NewGuid(), PaymentResultTypeDto.BillExpired}, //error controlado
                {Guid.NewGuid(), PaymentResultTypeDto.CsExpiredCard}, //error controlado
            };

            var billResultsDictionaryCaseFalse = new Dictionary<Guid, PaymentResultTypeDto>
            {
                {Guid.NewGuid(), PaymentResultTypeDto.Success}, //ok
                {Guid.NewGuid(), PaymentResultTypeDto.Success}, //ok
                {Guid.NewGuid(), PaymentResultTypeDto.UnhandledException}, //error de reintento
                {Guid.NewGuid(), PaymentResultTypeDto.Success}, //ok
                {Guid.NewGuid(), PaymentResultTypeDto.ExceededPaymentsAmount}, //error controlado
                {Guid.NewGuid(), PaymentResultTypeDto.ExceededPaymentsCount}, //error controlado
                {Guid.NewGuid(), PaymentResultTypeDto.CsExpiredCard}, //error controlado
            };

            // Act & Asserts
            firstRunState.SetNotificationFlag(billResultsDictionaryCaseTrue);
            Assert.AreEqual(true, firstRunState.ShouldNotifyUser);

            firstRunState.SetNotificationFlag(billResultsDictionaryCaseFalse);
            Assert.AreEqual(false, firstRunState.ShouldNotifyUser);
        }

        [TestMethod]
        public void RetryRun_SendNotifications_True()
        {
            //En una corrida intermedia se notifica al usuario si no ocurrieron errores de reintento

            // Arrange
            var retryRunState = new RetryRunState(_serviceServiceAssosiate, _processHistoryService);

            var billResultsDictionary = new Dictionary<Guid, PaymentResultTypeDto>
            {
                {Guid.NewGuid(), PaymentResultTypeDto.Success}, //ok
                {Guid.NewGuid(), PaymentResultTypeDto.ExceededPaymentsAmount}, //error controlado
                {Guid.NewGuid(), PaymentResultTypeDto.Success}, //ok
                {Guid.NewGuid(), PaymentResultTypeDto.ExceededPaymentsCount}, //error controlado
                {Guid.NewGuid(), PaymentResultTypeDto.BillExpired}, //error controlado
                {Guid.NewGuid(), PaymentResultTypeDto.CsExpiredCard}, //error controlado
            };

            // Act
            retryRunState.SetNotificationFlag(billResultsDictionary);

            // Asserts
            Assert.AreEqual(true, retryRunState.ShouldNotifyUser);
        }

        [TestMethod]
        public void RetryRun_SendNotifications_False()
        {
            //En una corrida intermedia si ocurre algun error de reintento no se notifica al usuario

            // Arrange
            var retryRunState = new RetryRunState(_serviceServiceAssosiate, _processHistoryService);

            var billResultsDictionary = new Dictionary<Guid, PaymentResultTypeDto>
            {
                {Guid.NewGuid(), PaymentResultTypeDto.Success}, //ok
                {Guid.NewGuid(), PaymentResultTypeDto.Success}, //ok
                {Guid.NewGuid(), PaymentResultTypeDto.UnhandledException}, //error de reintento
                {Guid.NewGuid(), PaymentResultTypeDto.Success}, //ok
                {Guid.NewGuid(), PaymentResultTypeDto.ExceededPaymentsAmount}, //error controlado
                {Guid.NewGuid(), PaymentResultTypeDto.ExceededPaymentsCount}, //error controlado
                {Guid.NewGuid(), PaymentResultTypeDto.CsExpiredCard}, //error controlado
            };

            // Act
            retryRunState.SetNotificationFlag(billResultsDictionary);

            // Asserts
            Assert.AreEqual(false, retryRunState.ShouldNotifyUser);
        }

        [TestMethod]
        public void RetryRun_SendNotifications_True_Then_False()
        {
            //Se prueba que notifica a un usuario y al siguiente no (porque son independientes)

            // Arrange
            var retryRunState = new RetryRunState(_serviceServiceAssosiate, _processHistoryService);

            var billResultsDictionaryCaseTrue = new Dictionary<Guid, PaymentResultTypeDto>
            {
                {Guid.NewGuid(), PaymentResultTypeDto.Success}, //ok
                {Guid.NewGuid(), PaymentResultTypeDto.ExceededPaymentsAmount}, //error controlado
                {Guid.NewGuid(), PaymentResultTypeDto.Success}, //ok
                {Guid.NewGuid(), PaymentResultTypeDto.ExceededPaymentsCount}, //error controlado
                {Guid.NewGuid(), PaymentResultTypeDto.BillExpired}, //error controlado
                {Guid.NewGuid(), PaymentResultTypeDto.CsExpiredCard}, //error controlado
            };

            var billResultsDictionaryCaseFalse = new Dictionary<Guid, PaymentResultTypeDto>
            {
                {Guid.NewGuid(), PaymentResultTypeDto.Success}, //ok
                {Guid.NewGuid(), PaymentResultTypeDto.Success}, //ok
                {Guid.NewGuid(), PaymentResultTypeDto.UnhandledException}, //error de reintento
                {Guid.NewGuid(), PaymentResultTypeDto.Success}, //ok
                {Guid.NewGuid(), PaymentResultTypeDto.ExceededPaymentsAmount}, //error controlado
                {Guid.NewGuid(), PaymentResultTypeDto.ExceededPaymentsCount}, //error controlado
                {Guid.NewGuid(), PaymentResultTypeDto.CsExpiredCard}, //error controlado
            };

            // Act & Asserts
            retryRunState.SetNotificationFlag(billResultsDictionaryCaseTrue);
            Assert.AreEqual(true, retryRunState.ShouldNotifyUser);

            retryRunState.SetNotificationFlag(billResultsDictionaryCaseFalse);
            Assert.AreEqual(false, retryRunState.ShouldNotifyUser);
        }

        [TestMethod]
        public void LastRun_SendNotifications_True()
        {
            //En la ultima corrida siempre se notifica al usuario

            // Arrange
            var lastRunState = new LastRunState(_serviceServiceAssosiate, _processHistoryService);

            var billResultsDictionary = new Dictionary<Guid, PaymentResultTypeDto>
            {
                {Guid.NewGuid(), PaymentResultTypeDto.Success}, //ok
                {Guid.NewGuid(), PaymentResultTypeDto.AmountIsZeroError}, //error controlado
                {Guid.NewGuid(), PaymentResultTypeDto.Success}, //ok
                {Guid.NewGuid(), PaymentResultTypeDto.ExceededPaymentsAmount}, //error controlado
                {Guid.NewGuid(), PaymentResultTypeDto.ExceededPaymentsCount}, //error controlado
                {Guid.NewGuid(), PaymentResultTypeDto.UnhandledException}, //error de reintento
                {Guid.NewGuid(), PaymentResultTypeDto.CsExpiredCard}, //error controlado
            };

            // Act
            lastRunState.SetNotificationFlag(billResultsDictionary);

            // Asserts
            Assert.AreEqual(true, lastRunState.ShouldNotifyUser);
        }

        [TestMethod]
        public void ResultCodesAreCategorized()
        {
            // Arrange
            var resultList = Enum.GetValues(typeof(PaymentResultTypeDto)).Cast<PaymentResultTypeDto>().ToList();
            var successCodes = RunState.SuccessCodes();
            var retryErrorCodes = RunState.RetryErrorCodes();
            var controlledErrorCodes = RunState.ControlledErrorCodes();
            var deleteAutomaticPaymentCodes = RunState.DeleteAutomaticPaymentErrorCodes();

            // Asserts
            foreach (var resultType in resultList)
            {
                Assert.IsTrue(
                    successCodes.Contains(resultType) ||
                    retryErrorCodes.Contains(resultType) ||
                    deleteAutomaticPaymentCodes.Contains(resultType) ||
                    controlledErrorCodes.Contains(resultType)
                );
            }
        }

    }
}