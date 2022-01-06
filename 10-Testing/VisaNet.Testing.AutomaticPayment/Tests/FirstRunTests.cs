using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;
using VisaNet.Application.Interfaces;
using VisaNet.AutomaticPaymentTests;
using VisaNet.Common.DependencyInjection;
using VisaNet.ConsoleApplication.PaymentProcess.PaymentsProcessor;
using VisaNet.Domain.Entities.BatchProcesses.AutomaticPaymentProcess.Interfaces;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Testing.AutomaticPayment.Tests
{
    [TestClass]
    public class FirstRunTests : AutomaticPaymentTests
    {
        private readonly IStateFactory _stateFactory;
        private readonly ILoggerHelper _loggerHelper;
        private readonly IValidationHelper _validationHelper;
        private readonly ISystemNotificationFactory _systemNotificationFactory;
        private readonly IServiceServiceAssosiate _serviceServiceAssosiate;

        private static bool _userWithCardExists;
        private static bool _binGroupExists;
        private static bool _discountExists;

        public FirstRunTests()
        {
            var kernel = new StandardKernel();
            NinjectRegister.Register(kernel, true);

            _loggerHelper = NinjectRegister.Get<ILoggerHelper>();
            _validationHelper = NinjectRegister.Get<IValidationHelper>();
            _systemNotificationFactory = NinjectRegister.Get<ISystemNotificationFactory>();
            _serviceServiceAssosiate = NinjectRegister.Get<IServiceServiceAssosiate>();
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
        public void FirstRun_Test()
        {
            //Pre-condicion: que no hayan corridas del proceso para el día de hoy

            Assert.AreEqual(true, _userWithCardExists);
            Assert.AreEqual(true, _binGroupExists);
            Assert.AreEqual(true, _discountExists);

            // Arrange
            var automaticPaymentHandler = new AutomaticPaymentHandler(_stateFactory, _loggerHelper, _validationHelper, _systemNotificationFactory);

            // Act
            automaticPaymentHandler.Start();

            var serviceAssociate1 = _serviceServiceAssosiate.GetById(ServiceAssociated1Id, x => x.AutomaticPayment);
            var serviceAssociate3 = _serviceServiceAssosiate.GetById(ServiceAssociated3Id, x => x.AutomaticPayment);
            var serviceAssociate4 = _serviceServiceAssosiate.GetById(ServiceAssociated4Id, x => x.AutomaticPayment);
            var serviceAssociate5 = _serviceServiceAssosiate.GetById(ServiceAssociated5Id, x => x.AutomaticPayment);
            var serviceAssociate6 = _serviceServiceAssosiate.GetById(ServiceAssociated6Id, x => x.AutomaticPayment);
            var serviceAssociate7 = _serviceServiceAssosiate.GetById(ServiceAssociated7Id, x => x.AutomaticPayment);
            var serviceAssociate8 = _serviceServiceAssosiate.GetById(ServiceAssociated8Id, x => x.AutomaticPayment);

            // Asserts
            Assert.AreEqual(PaymentResultTypeDto.NoBills, serviceAssociate3.AutomaticPaymentDto.LastRunResult);

            Assert.AreEqual(PaymentResultTypeDto.InvalidCardDueDate, serviceAssociate5.AutomaticPaymentDto.LastRunResult);
            Assert.AreEqual(PaymentResultTypeDto.InvalidCardDueDate, serviceAssociate5.AutomaticPaymentDto.LastErrorResult);

            Assert.AreEqual(PaymentResultTypeDto.ServiceNotAllowsAutomaticPayment, serviceAssociate6.AutomaticPaymentDto.LastRunResult);
            Assert.AreEqual(PaymentResultTypeDto.ServiceNotAllowsAutomaticPayment, serviceAssociate6.AutomaticPaymentDto.LastErrorResult);

            Assert.AreEqual(PaymentResultTypeDto.ExceededPaymentsCount, serviceAssociate4.AutomaticPaymentDto.LastRunResult);
            Assert.AreEqual(PaymentResultTypeDto.ExceededPaymentsCount, serviceAssociate4.AutomaticPaymentDto.LastErrorResult);

            Assert.AreEqual(PaymentResultTypeDto.InvalidModel, serviceAssociate7.AutomaticPaymentDto.LastRunResult);
            Assert.AreEqual(PaymentResultTypeDto.InvalidModel, serviceAssociate7.AutomaticPaymentDto.LastErrorResult);

            Assert.AreEqual(PaymentResultTypeDto.Success, serviceAssociate1.AutomaticPaymentDto.LastRunResult);

            Assert.AreEqual(PaymentResultTypeDto.ServiceErrorControlled, serviceAssociate8.AutomaticPaymentDto.LastRunResult);
            Assert.AreEqual(PaymentResultTypeDto.ServiceErrorControlled, serviceAssociate8.AutomaticPaymentDto.LastErrorResult);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            if (_userWithCardExists && _binGroupExists && _discountExists)
            {
                CleanDataForTests();
            }
        }

    }
}