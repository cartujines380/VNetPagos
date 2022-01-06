using System;
using System.Configuration;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;
using VisaNet.Application.Interfaces;
using VisaNet.Common.DependencyInjection;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Testing.CSAcknowledgement.Tests
{
    [TestClass]
    public class IntegrationTests : CSAcknowledgementTests
    {
        private readonly IServiceCyberSourceAcknowledgement _serviceCyberSourceAcknowledgement;

        //Pre-condición: que exista el usuario indicado en el app.config, y que haya realizado al menos 2 pagos
        private static bool _userWithPaymentsExists;

        public IntegrationTests()
        {
            var kernel = new StandardKernel();
            NinjectRegister.Register(kernel, true);

            _serviceCyberSourceAcknowledgement = NinjectRegister.Get<IServiceCyberSourceAcknowledgement>();
        }

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            _userWithPaymentsExists = VerifyUserWithPaymentsExists();
            if (_userWithPaymentsExists)
            {
                LoadDataForTests();
            }
        }

        [TestMethod]
        public void OnlyCsPostArrived_ShouldExecuteVoid_True()
        {
            // Arrange
            var ackWasInserted = false;
            var voidExecuted = false;

            var transactionId = Guid.NewGuid().ToString();
            var reqTransactionId = Guid.NewGuid().ToString();
            var ackMarginMinutes = int.Parse(ConfigurationManager.AppSettings["CyberSourceAcknowledgementTime"]) + 1;

            var cybersourcePost = new CyberSourceAcknowledgementDto
            {
                Id = CsAck1Id,
                ReasonCode = 100,
                TransactionId = transactionId,
                UserId = ApplicationUserId.ToString(),
                Decision = "ACCEPT",
                Message = "TEST_CYBERSOURCE_POST",
                BillTransRefNo = transactionId,
                ReqProfileId = "hxuy857",
                ReqCardType = "001",
                ReqPaymentMethod = "card",
                ReqTransactionType = "sale",
                ReqTransactionUuid = reqTransactionId,
                ReqCurrency = "UYU",
                ReqReferenceNumber = reqTransactionId,
                ReqAmount = 12345,
                AuthAvsCode = "Y",
                AuthCode = "831000",
                AuthAmount = "123.45",
                AuthTime = DateTime.Now.ToString("yyyy-MM-dd") + "T100010Z",
                AuthResponse = "00",
                AuthTransRefNo = transactionId,
                PaymentToken = CardPaymentToken,
                DateTime = DateTime.Now.AddMinutes(-ackMarginMinutes),
                ServiceId = Service1Id,
                OperationId = "",
                Platform = PaymentPlatformDto.VisaNet,
                CardId = CardId
            };

            // Act
            _serviceCyberSourceAcknowledgement.Process(cybersourcePost);
            var csAcknowledgement = RepositoryCyberSourceAcknowledgement.GetById(CsAck1Id);
            ackWasInserted = (csAcknowledgement != null);

            _serviceCyberSourceAcknowledgement.VoidPayments();
            var voidRegister =
                RepositoryCyberSourceVoid.AllNoTracking(x => x.CyberSourceAcknowledgementId == CsAck1Id).FirstOrDefault();
            voidExecuted = (voidRegister != null && voidRegister.VoidCode != null);

            // Assert
            Assert.AreEqual(true, ackWasInserted);
            Assert.AreEqual(true, voidExecuted);
        }

        [TestMethod]
        public void PaymentExistedWhenCsPostArrived_InsertAckRegister_False()
        {
            // Arrange
            var ackWasInserted = false;
            var voidExecuted = false;

            var ackMarginMinutes = int.Parse(ConfigurationManager.AppSettings["CyberSourceAcknowledgementTime"]) + 1;

            var cybersourcePost = new CyberSourceAcknowledgementDto
            {
                Id = CsAck2Id,
                ReasonCode = 100,
                TransactionId = Transaction2Id,
                UserId = ApplicationUserId.ToString(),
                Decision = "ACCEPT",
                Message = "TEST_CYBERSOURCE_POST",
                BillTransRefNo = Transaction2Id,
                ReqProfileId = "hxuy857",
                ReqCardType = "001",
                ReqPaymentMethod = "card",
                ReqTransactionType = "sale",
                ReqTransactionUuid = ReqTransaction2Id,
                ReqCurrency = "UYU",
                ReqReferenceNumber = ReqTransaction2Id,
                ReqAmount = 12345,
                AuthAvsCode = "Y",
                AuthCode = "831000",
                AuthAmount = "123.45",
                AuthTime = DateTime.Now.ToString("yyyy-MM-dd") + "T100010Z",
                AuthResponse = "00",
                AuthTransRefNo = Transaction2Id,
                PaymentToken = CardPaymentToken,
                DateTime = DateTime.Now.AddMinutes(-ackMarginMinutes),
                ServiceId = Service2Id,
                OperationId = "",
                Platform = PaymentPlatformDto.VisaNet,
                CardId = CardId
            };

            // Act
            _serviceCyberSourceAcknowledgement.Process(cybersourcePost);
            var csAcknowledgement = RepositoryCyberSourceAcknowledgement.GetById(CsAck2Id);
            ackWasInserted = (csAcknowledgement != null);

            var voidRegister =
                RepositoryCyberSourceVoid.AllNoTracking(x => x.CyberSourceAcknowledgementId == CsAck2Id).FirstOrDefault();
            voidExecuted = (voidRegister != null);

            // Assert
            Assert.AreEqual(false, ackWasInserted);
            Assert.AreEqual(false, voidExecuted);
        }

        [TestMethod]
        public void CsPostArrivedThenPaymentRegistered_DeleteAckRegister_True()
        {
            // Arrange
            var ackWasInserted = false;
            var ackWasDeleted = false;
            var voidExecuted = false;

            var ackMarginMinutes = int.Parse(ConfigurationManager.AppSettings["CyberSourceAcknowledgementTime"]) + 1;

            var cybersourcePost = new CyberSourceAcknowledgementDto
            {
                Id = CsAck3Id,
                ReasonCode = 100,
                TransactionId = Transaction3Id,
                UserId = ApplicationUserId.ToString(),
                Decision = "ACCEPT",
                Message = "TEST_CYBERSOURCE_POST",
                BillTransRefNo = Transaction3Id,
                ReqProfileId = "hxuy857",
                ReqCardType = "001",
                ReqPaymentMethod = "card",
                ReqTransactionType = "sale",
                ReqTransactionUuid = ReqTransaction3Id,
                ReqCurrency = "UYU",
                ReqReferenceNumber = ReqTransaction3Id,
                ReqAmount = 12345,
                AuthAvsCode = "Y",
                AuthCode = "831000",
                AuthAmount = "123.45",
                AuthTime = DateTime.Now.ToString("yyyy-MM-dd") + "T100010Z",
                AuthResponse = "00",
                AuthTransRefNo = Transaction3Id,
                PaymentToken = CardPaymentToken,
                DateTime = DateTime.Now.AddMinutes(-ackMarginMinutes),
                ServiceId = Service3Id,
                OperationId = "",
                Platform = PaymentPlatformDto.VisaNet,
                CardId = CardId
            };

            // Act
            _serviceCyberSourceAcknowledgement.Process(cybersourcePost);
            var csAckBeforeVoid = RepositoryCyberSourceAcknowledgement.GetById(CsAck3Id);
            ackWasInserted = (csAckBeforeVoid != null);

            InsertLatePayment();

            _serviceCyberSourceAcknowledgement.VoidPayments();
            var csAckAfterVoid = RepositoryCyberSourceAcknowledgement.GetById(CsAck3Id);
            ackWasDeleted = (csAckAfterVoid == null);

            var voidRegister =
                RepositoryCyberSourceVoid.AllNoTracking(x => x.CyberSourceAcknowledgementId == CsAck3Id).FirstOrDefault();
            voidExecuted = (voidRegister != null);

            // Assert
            Assert.AreEqual(true, ackWasInserted);
            Assert.AreEqual(true, ackWasDeleted);
            Assert.AreEqual(false, voidExecuted);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            if (_userWithPaymentsExists)
            {
                CleanDataForTests();
            }
        }

    }
}