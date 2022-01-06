using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;
using VisaNet.Common.DependencyInjection;
using VisaNet.Application.Interfaces;
using VisaNet.Domain.Entities.BatchProcesses.AutomaticPaymentProcess.Implementations;
using VisaNet.Domain.Entities.BatchProcesses.AutomaticPaymentProcess.Interfaces;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Repository.Interfaces.Interfaces;

namespace VisaNet.AutomaticPaymentTests
{
    [TestClass]
    public class LastRunTests : AutomaticPaymentTests
    {
        private readonly IServiceProcessHistory _serviceProcessHistory;
        private RunState _state;

        public LastRunTests()
        {
            var kernel = new StandardKernel();
            NinjectRegister.Register(kernel, true);

            _serviceProcessHistory = NinjectRegister.Get<IServiceProcessHistory>();
        }

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            LoadDataForTests();
        }

        [TestMethod]
        public void ServiceWithoutBills_Success()
        {
            // Arrange
            var stateFirstRun = NinjectRegister.Get<TestFirstRunState>();
            stateFirstRun.ServicesToRetry.Add(new PendingAutomaticPaymentDto
            {
                Date = DateTime.Now,
                PendingServiceAssociateId = ServiceAssociated3Id,
                //Status = PendingRegisterStatusDto.ErrorRetry,
                ProcessHistoryId = stateFirstRun.ProcessHistoryId,
                LastProcessHistoryId = stateFirstRun.ProcessHistoryId,
                ErrorMessage = "Test"
            });
            stateFirstRun.UpdateProcessHistory();

            _state = NinjectRegister.Get<TestLastRunState>();
            var serviceAssociatedDto =
                _state.GetServices().FirstOrDefault(x => x.Id == ServiceAssociated3Id);

            // Act
            var result = ProcessResult(_state, serviceAssociatedDto);
            _serviceProcessHistory.Delete(stateFirstRun.ProcessHistoryId);
            _serviceProcessHistory.Delete(_state.ProcessHistoryId);

            // Assert
            Assert.AreEqual(PaymentResultTypeDto.Success, result);
        }

        [TestMethod]
        public void ServiceDefaultCardInvalid_InvalidCard()
        {
            // Arrange
            var stateFirstRun = NinjectRegister.Get<TestFirstRunState>();
            stateFirstRun.ServicesToRetry.Add(new PendingAutomaticPaymentDto
            {
                Date = DateTime.Now,
                PendingServiceAssociateId = ServiceAssociated5Id,
                //Status = PendingRegisterStatusDto.ErrorRetry,
                ProcessHistoryId = stateFirstRun.ProcessHistoryId,
                LastProcessHistoryId = stateFirstRun.ProcessHistoryId,
                ErrorMessage = "Test"
            });
            stateFirstRun.UpdateProcessHistory();

            _state = NinjectRegister.Get<TestLastRunState>();
            var serviceAssociatedDto =
                _state.GetServices().FirstOrDefault(x => x.Id == ServiceAssociated5Id);

            // Act
            var result = ProcessResult(_state, serviceAssociatedDto);
            _serviceProcessHistory.Delete(stateFirstRun.ProcessHistoryId);
            _serviceProcessHistory.Delete(_state.ProcessHistoryId);

            // Assert
            Assert.AreEqual(PaymentResultTypeDto.InvalidCard, result);
        }

        [TestMethod]
        public void ServiceHasAutomaticPaymentDisabled_AutomaticPaymentDisabled()
        {
            // Arrange
            var stateFirstRun = NinjectRegister.Get<TestFirstRunState>();
            stateFirstRun.ServicesToRetry.Add(new PendingAutomaticPaymentDto
            {
                Date = DateTime.Now,
                PendingServiceAssociateId = ServiceAssociated2Id,
                //Status = PendingRegisterStatusDto.ErrorRetry,
                ProcessHistoryId = stateFirstRun.ProcessHistoryId,
                LastProcessHistoryId = stateFirstRun.ProcessHistoryId,
                ErrorMessage = "Test"
            });
            stateFirstRun.UpdateProcessHistory();

            _state = NinjectRegister.Get<TestLastRunState>();
            var serviceAssociatedDto =
                _state.GetServices().FirstOrDefault(x => x.Id == ServiceAssociated2Id);

            // Act
            var result = ProcessResult(_state, serviceAssociatedDto);
            _serviceProcessHistory.Delete(stateFirstRun.ProcessHistoryId);
            _serviceProcessHistory.Delete(_state.ProcessHistoryId);

            // Assert
            Assert.AreEqual(PaymentResultTypeDto.AutomaticPaymentDisabled, result);
        }

        [TestMethod]
        public void ServiceNotAllowsAutomaticPayment()
        {
            // Arrange
            var stateFirstRun = NinjectRegister.Get<TestFirstRunState>();
            stateFirstRun.ServicesToRetry.Add(new PendingAutomaticPaymentDto
            {
                Date = DateTime.Now,
                PendingServiceAssociateId = ServiceAssociated6Id,
                //Status = PendingRegisterStatusDto.ErrorRetry,
                ProcessHistoryId = stateFirstRun.ProcessHistoryId,
                LastProcessHistoryId = stateFirstRun.ProcessHistoryId,
                ErrorMessage = "Test"
            });
            stateFirstRun.UpdateProcessHistory();

            _state = NinjectRegister.Get<TestLastRunState>();
            var serviceAssociatedDto =
                _state.GetServices().FirstOrDefault(x => x.Id == ServiceAssociated6Id);

            // Act
            var result = ProcessResult(_state, serviceAssociatedDto);
            _serviceProcessHistory.Delete(stateFirstRun.ProcessHistoryId);
            _serviceProcessHistory.Delete(_state.ProcessHistoryId);

            // Assert
            Assert.AreEqual(PaymentResultTypeDto.ServiceNotAllowsAutomaticPayment, result);
        }

        [TestMethod]
        public void ServiceExceededPaymentsCount_ExceededPaymentsCount()
        {
            // Arrange
            var stateFirstRun = NinjectRegister.Get<TestFirstRunState>();
            stateFirstRun.ServicesToRetry.Add(new PendingAutomaticPaymentDto
            {
                Date = DateTime.Now,
                PendingServiceAssociateId = ServiceAssociated4Id,
                //Status = PendingRegisterStatusDto.ErrorRetry,
                ProcessHistoryId = stateFirstRun.ProcessHistoryId,
                LastProcessHistoryId = stateFirstRun.ProcessHistoryId,
                ErrorMessage = "Test"
            });
            stateFirstRun.UpdateProcessHistory();

            _state = NinjectRegister.Get<TestLastRunState>();
            var serviceAssociatedDto =
                _state.GetServices().FirstOrDefault(x => x.Id == ServiceAssociated4Id);

            // Act
            var result = ProcessResult(_state, serviceAssociatedDto);
            _serviceProcessHistory.Delete(stateFirstRun.ProcessHistoryId);
            _serviceProcessHistory.Delete(_state.ProcessHistoryId);

            // Assert
            Assert.AreEqual(PaymentResultTypeDto.ExceededPaymentsCount, result);
        }

        [TestMethod]
        public void BillAmountIsZero_AmountIsZeroError()
        {
            // Arrange
            var stateFirstRun = NinjectRegister.Get<TestFirstRunState>();
            stateFirstRun.ServicesToRetry.Add(new PendingAutomaticPaymentDto
            {
                Date = DateTime.Now,
                PendingServiceAssociateId = ServiceAssociated7Id,
                //Status = PendingRegisterStatusDto.ErrorRetry,
                ProcessHistoryId = stateFirstRun.ProcessHistoryId,
                LastProcessHistoryId = stateFirstRun.ProcessHistoryId,
                ErrorMessage = "Test"
            });
            stateFirstRun.UpdateProcessHistory();

            _state = NinjectRegister.Get<TestLastRunState>();
            var serviceAssociatedDto =
                _state.GetServices().FirstOrDefault(x => x.Id == ServiceAssociated7Id);

            // Act
            var result = ProcessResult(_state, serviceAssociatedDto);
            _serviceProcessHistory.Delete(stateFirstRun.ProcessHistoryId);
            _serviceProcessHistory.Delete(_state.ProcessHistoryId);

            // Assert
            Assert.AreEqual(PaymentResultTypeDto.AmountIsZeroError, result);
        }

        [TestMethod]
        public void SuccessfulPayment_Success()
        {
            // Arrange
            var stateFirstRun = NinjectRegister.Get<TestFirstRunState>();
            stateFirstRun.ServicesToRetry.Add(new PendingAutomaticPaymentDto
            {
                Date = DateTime.Now,
                PendingServiceAssociateId = ServiceAssociated1Id,
                //Status = PendingRegisterStatusDto.ErrorRetry,
                ProcessHistoryId = stateFirstRun.ProcessHistoryId,
                LastProcessHistoryId = stateFirstRun.ProcessHistoryId,
                ErrorMessage = "Test"
            });
            stateFirstRun.UpdateProcessHistory();

            _state = NinjectRegister.Get<TestLastRunState>();
            var serviceAssociatedDto =
                _state.GetServices().FirstOrDefault(x => x.Id == ServiceAssociated1Id);

            // Act
            var result = ProcessResult(_state, serviceAssociatedDto);
            _serviceProcessHistory.Delete(stateFirstRun.ProcessHistoryId);
            _serviceProcessHistory.Delete(_state.ProcessHistoryId);

            // Assert
            Assert.AreEqual(PaymentResultTypeDto.Success, result);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            CleanDataForTests();
        }

    }
}