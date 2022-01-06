using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using VisaNet.Application.Implementations;
using VisaNet.Application.Interfaces;
using VisaNet.Common.Exceptions;
using VisaNet.Common.Logging.Services;
using VisaNet.Domain.Entities;
using VisaNet.Domain.Entities.Enums;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Repository.Interfaces.Interfaces;

namespace VisaNet.Testing.ServiceReferences
{
    [TestClass]
    public class ReferenceParamsTests
    {
        private IServiceService _serviceService;
        private readonly Mock<IRepositoryService> _respositoryServiceMock = new Mock<IRepositoryService>();
        private readonly Mock<IRepositoryGateway> _repositoryGatewayMock = new Mock<IRepositoryGateway>();
        private readonly Mock<IRepositoryServiceAsociated> _repositoryServiceAsociatedMock = new Mock<IRepositoryServiceAsociated>();
        private readonly Mock<IRepositoryPayment> _repositoryPaymentMock = new Mock<IRepositoryPayment>();
        private readonly Mock<IRepositoryApplicationUser> _repositoryApplicationUserMock = new Mock<IRepositoryApplicationUser>();
        private readonly Mock<IRepositoryBin> _repositoryBinMock = new Mock<IRepositoryBin>();
        private readonly Mock<IServiceHighway> _serviceHighwayMock = new Mock<IServiceHighway>();
        private readonly Mock<IRepositoryNotification> _repositoryNotificationMock = new Mock<IRepositoryNotification>();
        private readonly Mock<ILoggerService> _loggerServiceMock = new Mock<ILoggerService>();
        private readonly Mock<IRepositoryCard> _repositoryCardMock = new Mock<IRepositoryCard>();
        private readonly Mock<IServiceFixedNotification> _serviceFixedNotificationMock = new Mock<IServiceFixedNotification>();
        private readonly Mock<IServiceEmailMessage> _serviceEmailMessageMock = new Mock<IServiceEmailMessage>();
        private readonly Mock<IRepositoryBinGroup> _repositoryBinGroupMock = new Mock<IRepositoryBinGroup>();


        //CREATE SERVICE
        [TestMethod]
        [ExpectedException(typeof(MissingServiceReferenceParamsException))]
        public void Create_SingleServiceAskForReferences_HasNoReferences_Fail()
        {
            //Arrange
            _serviceService = new ServiceService(_respositoryServiceMock.Object, _repositoryGatewayMock.Object, _repositoryServiceAsociatedMock.Object, _repositoryPaymentMock.Object,
                _repositoryApplicationUserMock.Object, _repositoryBinMock.Object, _serviceHighwayMock.Object, _repositoryNotificationMock.Object, _loggerServiceMock.Object,
                _repositoryCardMock.Object, _serviceFixedNotificationMock.Object, _serviceEmailMessageMock.Object, _repositoryBinGroupMock.Object);

            var service = new ServiceDto
            {
                Id = Guid.NewGuid(),
                Name = "Single Service Test References",
                CybersourceAccessKey = "csAccessKey",
                CybersourceSecretKey = "csSecretKey",
                MerchantId = "merchantId",
                Container = false,
                ServiceContainerId = null,
                BinGroups = new List<BinGroupDto> { new BinGroupDto { Id = Guid.NewGuid(), Name = "Single Service BinGroup Test References" } },
                AskUserForReferences = true, //pide referencias
                ReferenceParamName = null, //no tiene referencias
            };

            //Act
            _serviceService.Create(service, true);
        }

        [TestMethod]
        [ExpectedException(typeof(MissingServiceReferenceParamsException))]
        public void Create_ChildServiceAskForReferences_NoneHasReferences_Fail()
        {
            //Arrange
            _serviceService = new ServiceService(_respositoryServiceMock.Object, _repositoryGatewayMock.Object, _repositoryServiceAsociatedMock.Object, _repositoryPaymentMock.Object,
                _repositoryApplicationUserMock.Object, _repositoryBinMock.Object, _serviceHighwayMock.Object, _repositoryNotificationMock.Object, _loggerServiceMock.Object,
                _repositoryCardMock.Object, _serviceFixedNotificationMock.Object, _serviceEmailMessageMock.Object, _repositoryBinGroupMock.Object);

            var containerId = Guid.NewGuid();

            _respositoryServiceMock
                .SetupSequence(x => x.AllNoTracking(It.IsAny<Expression<Func<Service, bool>>>())) //Setup en secuencia (*)
                .Returns(new List<Service>().AsQueryable()) //(*1) La primera vez que compara que no exista Servicio con mismo Name devuelve null
                .Returns(new List<Service>() //(*2) La segunda vez va a buscar el ServiceContainer
                {
                    new Service()
                    {
                        Id = containerId,
                        Name = "Container Service Test References",
                        CybersourceAccessKey = "csAccessKey",
                        CybersourceSecretKey = "csSecretKey",
                        MerchantId = "merchantId",
                        Container = true,
                        ServiceContainerId = null,
                        AskUserForReferences = false, //contenedor no pide referencias (el hijo sí)
                        ReferenceParamName = null, //contenedor no tiene referencias
                    }
                }.AsQueryable());

            var childService = new ServiceDto
            {
                Id = Guid.NewGuid(),
                Name = "Child Service Test References",
                CybersourceAccessKey = "csAccessKey",
                CybersourceSecretKey = "csSecretKey",
                MerchantId = "merchantId",
                Container = false,
                ServiceContainerId = containerId,
                BinGroups = new List<BinGroupDto> { new BinGroupDto { Id = Guid.NewGuid(), Name = "Child Service BinGroup Test References" } },
                AskUserForReferences = true, //hijo pide referencias
                ReferenceParamName = null, //hijo no tiene referencias
            };

            //Act
            _serviceService.Create(childService, true);
        }

        [TestMethod]
        [ExpectedException(typeof(MissingServiceReferenceParamsException))]
        public void Create_ContainerServiceAskForReferences_NoneHasReferences_Fail()
        {
            //Arrange
            _serviceService = new ServiceService(_respositoryServiceMock.Object, _repositoryGatewayMock.Object, _repositoryServiceAsociatedMock.Object, _repositoryPaymentMock.Object,
                _repositoryApplicationUserMock.Object, _repositoryBinMock.Object, _serviceHighwayMock.Object, _repositoryNotificationMock.Object, _loggerServiceMock.Object,
                _repositoryCardMock.Object, _serviceFixedNotificationMock.Object, _serviceEmailMessageMock.Object, _repositoryBinGroupMock.Object);

            var containerId = Guid.NewGuid();

            _respositoryServiceMock
                .SetupSequence(x => x.AllNoTracking(It.IsAny<Expression<Func<Service, bool>>>())) //Setup en secuencia (*)
                .Returns(new List<Service>().AsQueryable()) //(*1) La primera vez que compara que no exista Servicio con mismo Name devuelve null
                .Returns(new List<Service>() //(*2) La segunda vez va a buscar el ServiceContainer
                {
                    new Service()
                    {
                        Id = containerId,
                        Name = "Container Service Test References",
                        CybersourceAccessKey = "csAccessKey",
                        CybersourceSecretKey = "csSecretKey",
                        MerchantId = "merchantId",
                        Container = true,
                        ServiceContainerId = null,
                        AskUserForReferences = true, //contenedor pide referencias
                        ReferenceParamName = null, //contenedor no tiene referencias
                    }
                }.AsQueryable());

            var childService = new ServiceDto
            {
                Id = Guid.NewGuid(),
                Name = "Child Service Test References",
                CybersourceAccessKey = "csAccessKey",
                CybersourceSecretKey = "csSecretKey",
                MerchantId = "merchantId",
                Container = false,
                ServiceContainerId = containerId,
                BinGroups = new List<BinGroupDto> { new BinGroupDto { Id = Guid.NewGuid(), Name = "Child Service BinGroup Test References" } },
                AskUserForReferences = false, //hijo no pide referencias (el contenedor sí)
                ReferenceParamName = null, //hijo no tiene referencias
            };

            //Act
            _serviceService.Create(childService, true);
        }

        [TestMethod]
        public void Create_SingleServiceAskForReferences_HasReferences_Success()
        {
            //Arrange
            _serviceService = new ServiceService(_respositoryServiceMock.Object, _repositoryGatewayMock.Object, _repositoryServiceAsociatedMock.Object, _repositoryPaymentMock.Object,
                _repositoryApplicationUserMock.Object, _repositoryBinMock.Object, _serviceHighwayMock.Object, _repositoryNotificationMock.Object, _loggerServiceMock.Object,
                _repositoryCardMock.Object, _serviceFixedNotificationMock.Object, _serviceEmailMessageMock.Object, _repositoryBinGroupMock.Object);

            _repositoryBinGroupMock
                .Setup(x => x.All(It.IsAny<Expression<Func<BinGroup, bool>>>()))
                .Returns(() => new List<BinGroup> { new BinGroup() }.AsQueryable());

            var singleService = new ServiceDto
            {
                Id = Guid.NewGuid(),
                Name = "Single Service Test References",
                CybersourceAccessKey = "csAccessKey",
                CybersourceSecretKey = "csSecretKey",
                MerchantId = "merchantId",
                Container = false,
                ServiceContainerId = null,
                BinGroups = new List<BinGroupDto> { new BinGroupDto { Id = Guid.NewGuid(), Name = "Single Service BinGroup Test References" } },
                AskUserForReferences = true, //pide referencias
                ReferenceParamName = "Referencia 1", //tiene referencias
            };

            //Act
            var createdService = _serviceService.Create(singleService, true);

            //Assert
            _respositoryServiceMock.Verify(x => x.Create(It.IsAny<Service>()));
            _respositoryServiceMock.Verify(x => x.Save());
            Assert.AreNotEqual(null, createdService);
        }

        [TestMethod]
        public void Create_ChildServiceAskForReferences_ChildHasReferences_Success()
        {
            //Arrange
            _serviceService = new ServiceService(_respositoryServiceMock.Object, _repositoryGatewayMock.Object, _repositoryServiceAsociatedMock.Object, _repositoryPaymentMock.Object,
                _repositoryApplicationUserMock.Object, _repositoryBinMock.Object, _serviceHighwayMock.Object, _repositoryNotificationMock.Object, _loggerServiceMock.Object,
                _repositoryCardMock.Object, _serviceFixedNotificationMock.Object, _serviceEmailMessageMock.Object, _repositoryBinGroupMock.Object);

            var containerId = Guid.NewGuid();

            _repositoryBinGroupMock
                .Setup(x => x.All(It.IsAny<Expression<Func<BinGroup, bool>>>()))
                .Returns(() => new List<BinGroup> { new BinGroup() }.AsQueryable());

            _respositoryServiceMock
                .SetupSequence(x => x.AllNoTracking(It.IsAny<Expression<Func<Service, bool>>>())) //Setup en secuencia (*)
                .Returns(new List<Service>().AsQueryable()) //(*1) La primera vez que compara que no exista Servicio con mismo Name devuelve null
                .Returns(new List<Service>() //(*2) La segunda vez va a buscar el ServiceContainer
                {
                    new Service()
                    {
                        Id = containerId,
                        Name = "Container Service Test References",
                        CybersourceAccessKey = "csAccessKey",
                        CybersourceSecretKey = "csSecretKey",
                        MerchantId = "merchantId",
                        Container = true,
                        ServiceContainerId = null,
                        AskUserForReferences = false, //contenedor no pide referencias (el hijo sí)
                        ReferenceParamName = null, //contenedor no tiene referencias
                    }
                }.AsQueryable());

            var childService = new ServiceDto
            {
                Id = Guid.NewGuid(),
                Name = "Child Service Test References",
                CybersourceAccessKey = "csAccessKey",
                CybersourceSecretKey = "csSecretKey",
                MerchantId = "merchantId",
                Container = false,
                ServiceContainerId = containerId,
                BinGroups = new List<BinGroupDto> { new BinGroupDto { Id = Guid.NewGuid(), Name = "Child Service BinGroup Test References" } },
                AskUserForReferences = true, //hijo pide referencias
                ReferenceParamName = "Referencia 1", //hijo tiene referencias
            };

            //Act
            var createdService = _serviceService.Create(childService, true);

            //Assert
            _respositoryServiceMock.Verify(x => x.Create(It.IsAny<Service>()));
            _respositoryServiceMock.Verify(x => x.Save());
            Assert.AreNotEqual(null, createdService);
        }

        [TestMethod]
        public void Create_ChildServiceAskForReferences_ContainerHasReferences_Success()
        {
            //Arrange
            _serviceService = new ServiceService(_respositoryServiceMock.Object, _repositoryGatewayMock.Object, _repositoryServiceAsociatedMock.Object, _repositoryPaymentMock.Object,
                _repositoryApplicationUserMock.Object, _repositoryBinMock.Object, _serviceHighwayMock.Object, _repositoryNotificationMock.Object, _loggerServiceMock.Object,
                _repositoryCardMock.Object, _serviceFixedNotificationMock.Object, _serviceEmailMessageMock.Object, _repositoryBinGroupMock.Object);

            var containerId = Guid.NewGuid();

            _repositoryBinGroupMock
                .Setup(x => x.All(It.IsAny<Expression<Func<BinGroup, bool>>>()))
                .Returns(() => new List<BinGroup> { new BinGroup() }.AsQueryable());

            _respositoryServiceMock
                .SetupSequence(x => x.AllNoTracking(It.IsAny<Expression<Func<Service, bool>>>())) //Setup en secuencia (*)
                .Returns(new List<Service>().AsQueryable()) //(*1) La primera vez que compara que no exista Servicio con mismo Name devuelve null
                .Returns(new List<Service>() //(*2) La segunda vez va a buscar el ServiceContainer
                {
                    new Service()
                    {
                        Id = containerId,
                        Name = "Container Service Test References",
                        CybersourceAccessKey = "csAccessKey",
                        CybersourceSecretKey = "csSecretKey",
                        MerchantId = "merchantId",
                        Container = true,
                        ServiceContainerId = null,
                        AskUserForReferences = false, //contenedor no pide referencias (el hijo sí)
                        ReferenceParamName = "Referencia 1", //contenedor tiene referencias
                    }
                }.AsQueryable());

            var childService = new ServiceDto
            {
                Id = Guid.NewGuid(),
                Name = "Child Service Test References",
                CybersourceAccessKey = "csAccessKey",
                CybersourceSecretKey = "csSecretKey",
                MerchantId = "merchantId",
                Container = false,
                ServiceContainerId = containerId,
                BinGroups = new List<BinGroupDto> { new BinGroupDto { Id = Guid.NewGuid(), Name = "Child Service BinGroup Test References" } },
                AskUserForReferences = true, //hijo pide referencias
                ReferenceParamName = null, //hijo no tiene referencias
            };

            //Act
            var createdService = _serviceService.Create(childService, true);

            //Assert
            _respositoryServiceMock.Verify(x => x.Create(It.IsAny<Service>()));
            _respositoryServiceMock.Verify(x => x.Save());
            Assert.AreNotEqual(null, createdService);
        }

        [TestMethod]
        public void Create_ContainerServiceAskForReferences_ChildHasReferences_Success()
        {
            //Arrange
            _serviceService = new ServiceService(_respositoryServiceMock.Object, _repositoryGatewayMock.Object, _repositoryServiceAsociatedMock.Object, _repositoryPaymentMock.Object,
                _repositoryApplicationUserMock.Object, _repositoryBinMock.Object, _serviceHighwayMock.Object, _repositoryNotificationMock.Object, _loggerServiceMock.Object,
                _repositoryCardMock.Object, _serviceFixedNotificationMock.Object, _serviceEmailMessageMock.Object, _repositoryBinGroupMock.Object);

            var containerId = Guid.NewGuid();

            _repositoryBinGroupMock
                .Setup(x => x.All(It.IsAny<Expression<Func<BinGroup, bool>>>()))
                .Returns(() => new List<BinGroup> { new BinGroup() }.AsQueryable());

            _respositoryServiceMock
                .SetupSequence(x => x.AllNoTracking(It.IsAny<Expression<Func<Service, bool>>>())) //Setup en secuencia (*)
                .Returns(new List<Service>().AsQueryable()) //(*1) La primera vez que compara que no exista Servicio con mismo Name devuelve null
                .Returns(new List<Service>() //(*2) La segunda vez va a buscar el ServiceContainer
                {
                    new Service()
                    {
                        Id = containerId,
                        Name = "Container Service Test References",
                        CybersourceAccessKey = "csAccessKey",
                        CybersourceSecretKey = "csSecretKey",
                        MerchantId = "merchantId",
                        Container = true,
                        ServiceContainerId = null,
                        AskUserForReferences = true, //contenedor pide referencias
                        ReferenceParamName = null, //contenedor no tiene referencias
                    }
                }.AsQueryable());

            var childService = new ServiceDto
            {
                Id = Guid.NewGuid(),
                Name = "Child Service Test References",
                CybersourceAccessKey = "csAccessKey",
                CybersourceSecretKey = "csSecretKey",
                MerchantId = "merchantId",
                Container = false,
                ServiceContainerId = containerId,
                BinGroups = new List<BinGroupDto> { new BinGroupDto { Id = Guid.NewGuid(), Name = "Child Service BinGroup Test References" } },
                AskUserForReferences = false, //hijo no pide referencias (el contenedor sí)
                ReferenceParamName = "Referencia 1", //hijo tiene referencias
            };

            //Act
            var createdService = _serviceService.Create(childService, true);

            //Assert
            _respositoryServiceMock.Verify(x => x.Create(It.IsAny<Service>()));
            _respositoryServiceMock.Verify(x => x.Save());
            Assert.AreNotEqual(null, createdService);
        }

        [TestMethod]
        public void Create_ContainerServiceAskForReferences_ContainerHasReferences_Success()
        {
            //Arrange
            _serviceService = new ServiceService(_respositoryServiceMock.Object, _repositoryGatewayMock.Object, _repositoryServiceAsociatedMock.Object, _repositoryPaymentMock.Object,
                _repositoryApplicationUserMock.Object, _repositoryBinMock.Object, _serviceHighwayMock.Object, _repositoryNotificationMock.Object, _loggerServiceMock.Object,
                _repositoryCardMock.Object, _serviceFixedNotificationMock.Object, _serviceEmailMessageMock.Object, _repositoryBinGroupMock.Object);

            var containerId = Guid.NewGuid();

            _repositoryBinGroupMock
                .Setup(x => x.All(It.IsAny<Expression<Func<BinGroup, bool>>>()))
                .Returns(() => new List<BinGroup> { new BinGroup() }.AsQueryable());

            _respositoryServiceMock
                .SetupSequence(x => x.AllNoTracking(It.IsAny<Expression<Func<Service, bool>>>())) //Setup en secuencia (*)
                .Returns(new List<Service>().AsQueryable()) //(*1) La primera vez que compara que no exista Servicio con mismo Name devuelve null
                .Returns(new List<Service>() //(*2) La segunda vez va a buscar el ServiceContainer
                {
                    new Service()
                    {
                        Id = containerId,
                        Name = "Container Service Test References",
                        CybersourceAccessKey = "csAccessKey",
                        CybersourceSecretKey = "csSecretKey",
                        MerchantId = "merchantId",
                        Container = true,
                        ServiceContainerId = null,
                        AskUserForReferences = true, //contenedor pide referencias
                        ReferenceParamName = "Referencia 1", //contenedor tiene referencias
                    }
                }.AsQueryable());

            var childService = new ServiceDto
            {
                Id = Guid.NewGuid(),
                Name = "Child Service Test References",
                CybersourceAccessKey = "csAccessKey",
                CybersourceSecretKey = "csSecretKey",
                MerchantId = "merchantId",
                Container = false,
                ServiceContainerId = containerId,
                BinGroups = new List<BinGroupDto> { new BinGroupDto { Id = Guid.NewGuid(), Name = "Child Service BinGroup Test References" } },
                AskUserForReferences = false, //hijo no pide referencias (el contenedor sí)
                ReferenceParamName = null, //hijo no tiene referencias
            };

            //Act
            var createdService = _serviceService.Create(childService, true);

            //Assert
            _respositoryServiceMock.Verify(x => x.Create(It.IsAny<Service>()));
            _respositoryServiceMock.Verify(x => x.Save());
            Assert.AreNotEqual(null, createdService);
        }


        //EDIT SERVICE
        [TestMethod]
        [ExpectedException(typeof(MissingServiceReferenceParamsException))]
        public void Edit_SingleServiceAskForReferences_HasNoReferences_Fail()
        {
            //Arrange
            _serviceService = new ServiceService(_respositoryServiceMock.Object, _repositoryGatewayMock.Object, _repositoryServiceAsociatedMock.Object, _repositoryPaymentMock.Object,
                _repositoryApplicationUserMock.Object, _repositoryBinMock.Object, _serviceHighwayMock.Object, _repositoryNotificationMock.Object, _loggerServiceMock.Object,
                _repositoryCardMock.Object, _serviceFixedNotificationMock.Object, _serviceEmailMessageMock.Object, _repositoryBinGroupMock.Object);

            var singleService = new ServiceDto
            {
                Id = Guid.NewGuid(),
                Name = "Single Service Test References",
                CybersourceAccessKey = "csAccessKey",
                CybersourceSecretKey = "csSecretKey",
                MerchantId = "merchantId",
                Container = false,
                ServiceContainerId = null,
                BinGroups = new List<BinGroupDto> { new BinGroupDto { Id = Guid.NewGuid(), Name = "Single Service BinGroup Test References" } },
                AskUserForReferences = true, //pide referencias
                ReferenceParamName = null, //no tiene referencias
            };

            //Act
            _serviceService.Edit(singleService);
        }

        [TestMethod]
        [ExpectedException(typeof(MissingServiceReferenceParamsException))]
        public void Edit_ChildServiceAskForReferences_NoneHasReferences_Fail()
        {
            //Arrange
            _serviceService = new ServiceService(_respositoryServiceMock.Object, _repositoryGatewayMock.Object, _repositoryServiceAsociatedMock.Object, _repositoryPaymentMock.Object,
                _repositoryApplicationUserMock.Object, _repositoryBinMock.Object, _serviceHighwayMock.Object, _repositoryNotificationMock.Object, _loggerServiceMock.Object,
                _repositoryCardMock.Object, _serviceFixedNotificationMock.Object, _serviceEmailMessageMock.Object, _repositoryBinGroupMock.Object);

            Guid? containerId = Guid.NewGuid();

            _respositoryServiceMock
                .SetupSequence(x => x.AllNoTracking(It.IsAny<Expression<Func<Service, bool>>>())) //Setup en secuencia (*)
                .Returns(new List<Service>().AsQueryable()) //(*1) La primera vez que compara que no exista Servicio con mismo Name devuelve null
                .Returns(new List<Service>() //(*2) La segunda vez va a buscar el ServiceContainer
                {
                    new Service()
                    {
                        Id = containerId.Value,
                        Name = "Container Service Test References",
                        CybersourceAccessKey = "csAccessKey",
                        CybersourceSecretKey = "csSecretKey",
                        MerchantId = "merchantId",
                        Container = true,
                        ServiceContainerId = null,
                        AskUserForReferences = false, //contenedor no pide referencias (el hijo sí)
                        ReferenceParamName = null, //contenedor no tiene referencias
                    }
                }.AsQueryable());

            var childService = new ServiceDto
            {
                Id = Guid.NewGuid(),
                Name = "Child Service Test References",
                CybersourceAccessKey = "csAccessKey",
                CybersourceSecretKey = "csSecretKey",
                MerchantId = "merchantId",
                Container = false,
                ServiceContainerId = containerId,
                BinGroups = new List<BinGroupDto> { new BinGroupDto { Id = Guid.NewGuid(), Name = "Child Service BinGroup Test References" } },
                AskUserForReferences = true, //hijo pide referencias
                ReferenceParamName = null, //hijo no tiene referencias
            };

            //Act
            _serviceService.Edit(childService);
        }

        [TestMethod]
        [ExpectedException(typeof(MissingServiceReferenceParamsException))]
        public void Edit_ContainerServiceAskForReferences_NoneHasReferences_Fail()
        {
            //Arrange
            _serviceService = new ServiceService(_respositoryServiceMock.Object, _repositoryGatewayMock.Object, _repositoryServiceAsociatedMock.Object, _repositoryPaymentMock.Object,
                _repositoryApplicationUserMock.Object, _repositoryBinMock.Object, _serviceHighwayMock.Object, _repositoryNotificationMock.Object, _loggerServiceMock.Object,
                _repositoryCardMock.Object, _serviceFixedNotificationMock.Object, _serviceEmailMessageMock.Object, _repositoryBinGroupMock.Object);

            var containerId = Guid.NewGuid();

            _respositoryServiceMock
                .SetupSequence(x => x.AllNoTracking(It.IsAny<Expression<Func<Service, bool>>>())) //Setup en secuencia (*)
                .Returns(new List<Service>().AsQueryable()) //(*1) La primera vez que compara que no exista Servicio con mismo Name devuelve null
                .Returns(new List<Service>() //(*2) La segunda vez va a buscar el ServiceContainer
                {
                    new Service()
                    {
                        Id = containerId,
                        Name = "Container Service Test References",
                        CybersourceAccessKey = "csAccessKey",
                        CybersourceSecretKey = "csSecretKey",
                        MerchantId = "merchantId",
                        Container = true,
                        ServiceContainerId = null,
                        AskUserForReferences = true, //contenedor pide referencias
                        ReferenceParamName = null, //contenedor no tiene referencias
                    }
                }.AsQueryable());

            var childService = new ServiceDto
            {
                Id = Guid.NewGuid(),
                Name = "Child Service Test References",
                CybersourceAccessKey = "csAccessKey",
                CybersourceSecretKey = "csSecretKey",
                MerchantId = "merchantId",
                Container = false,
                ServiceContainerId = containerId,
                BinGroups = new List<BinGroupDto> { new BinGroupDto { Id = Guid.NewGuid(), Name = "Child Service BinGroup Test References" } },
                AskUserForReferences = false, //hijo no pide referencias (el contenedor sí)
                ReferenceParamName = null, //hijo no tiene referencias
            };

            //Act
            _serviceService.Edit(childService);
        }

        [TestMethod]
        public void Edit_SingleServiceAskForReferences_HasReferences_Success()
        {
            //Arrange
            _serviceService = new ServiceService(_respositoryServiceMock.Object, _repositoryGatewayMock.Object, _repositoryServiceAsociatedMock.Object, _repositoryPaymentMock.Object,
                _repositoryApplicationUserMock.Object, _repositoryBinMock.Object, _serviceHighwayMock.Object, _repositoryNotificationMock.Object, _loggerServiceMock.Object,
                _repositoryCardMock.Object, _serviceFixedNotificationMock.Object, _serviceEmailMessageMock.Object, _repositoryBinGroupMock.Object);

            var serviceId = Guid.NewGuid();
            var binGroupId = Guid.NewGuid();

            _repositoryBinGroupMock
                .Setup(x => x.All(It.IsAny<Expression<Func<BinGroup, bool>>>()))
                .Returns(() => new List<BinGroup> { new BinGroup() }.AsQueryable());

            _repositoryGatewayMock
                .Setup(x => x.AllNoTracking(It.IsAny<Expression<Func<Gateway, bool>>>()))
                .Returns(() => new List<Gateway> { new Gateway { Id = Guid.NewGuid(), Enum = (int)GatewayEnum.Apps } }.AsQueryable());

            _respositoryServiceMock
                .Setup(x => x.GetById(serviceId, It.IsAny<Expression<Func<Service, object>>[]>()))
                .Returns(() =>
                    new Service()
                    {
                        Id = serviceId,
                        Name = "Single Service Test References",
                        CybersourceAccessKey = "csAccessKey",
                        CybersourceSecretKey = "csSecretKey",
                        MerchantId = "merchantId",
                        Container = false,
                        ServiceContainerId = null,
                        BinGroups = new List<BinGroup> { new BinGroup { Id = binGroupId, Name = "Single Service BinGroup Test References" } },
                        AskUserForReferences = true, //pide referencias
                        ReferenceParamName = "Referencia 1", //tiene referencias
                    }
                );

            var singleService = new ServiceDto
            {
                Id = serviceId,
                Name = "Single Service Test References",
                CybersourceAccessKey = "csAccessKey",
                CybersourceSecretKey = "csSecretKey",
                MerchantId = "merchantId",
                Container = false,
                ServiceContainerId = null,
                BinGroups = new List<BinGroupDto> { new BinGroupDto { Id = binGroupId, Name = "Single Service BinGroup Test References" } },
                AskUserForReferences = true, //pide referencias
                ReferenceParamName = "Referencia 1", //tiene referencias
            };

            //Act
            _serviceService.Edit(singleService);

            //Assert
            _respositoryServiceMock.Verify(x => x.Edit(It.IsAny<Service>()));
            _respositoryServiceMock.Verify(x => x.Save());
        }

        [TestMethod]
        public void Edit_ChildServiceAskForReferences_ChildHasReferences_Success()
        {
            //Arrange
            _serviceService = new ServiceService(_respositoryServiceMock.Object, _repositoryGatewayMock.Object, _repositoryServiceAsociatedMock.Object, _repositoryPaymentMock.Object,
                _repositoryApplicationUserMock.Object, _repositoryBinMock.Object, _serviceHighwayMock.Object, _repositoryNotificationMock.Object, _loggerServiceMock.Object,
                _repositoryCardMock.Object, _serviceFixedNotificationMock.Object, _serviceEmailMessageMock.Object, _repositoryBinGroupMock.Object);

            var serviceId = Guid.NewGuid();
            var binGroupId = Guid.NewGuid();
            var containerId = Guid.NewGuid();

            _repositoryBinGroupMock
                .Setup(x => x.All(It.IsAny<Expression<Func<BinGroup, bool>>>()))
                .Returns(() => new List<BinGroup> { new BinGroup() }.AsQueryable());

            _repositoryGatewayMock
                .Setup(x => x.AllNoTracking(It.IsAny<Expression<Func<Gateway, bool>>>()))
                .Returns(() => new List<Gateway> { new Gateway { Id = Guid.NewGuid(), Enum = (int)GatewayEnum.Apps } }.AsQueryable());

            _respositoryServiceMock
                .SetupSequence(x => x.AllNoTracking(It.IsAny<Expression<Func<Service, bool>>>())) //Setup en secuencia (*)
                .Returns(new List<Service>().AsQueryable()) //(*1) La primera vez que compara que no exista Servicio con mismo Name devuelve null
                .Returns(new List<Service>() //(*2) La segunda vez va a buscar el ServiceContainer
                {
                    new Service()
                    {
                        Id = containerId,
                        Name = "Container Service Test References",
                        CybersourceAccessKey = "csAccessKey",
                        CybersourceSecretKey = "csSecretKey",
                        MerchantId = "merchantId",
                        Container = true,
                        ServiceContainerId = null,
                        AskUserForReferences = false, //contenedor no pide referencias (el hijo sí)
                        ReferenceParamName = null, //contenedor no tiene referencias
                    }
                }.AsQueryable());

            _respositoryServiceMock
                .Setup(x => x.GetById(serviceId, It.IsAny<Expression<Func<Service, object>>[]>()))
                .Returns(() =>
                    new Service()
                    {
                        Id = serviceId,
                        Name = "Child Service Test References",
                        CybersourceAccessKey = "csAccessKey",
                        CybersourceSecretKey = "csSecretKey",
                        MerchantId = "merchantId",
                        Container = false,
                        ServiceContainerId = containerId,
                        BinGroups = new List<BinGroup> { new BinGroup { Id = binGroupId, Name = "Child Service BinGroup Test References" } },
                        AskUserForReferences = true, //hijo pide referencias
                        ReferenceParamName = "Referencia 1", //hijo tiene referencias
                    }
                );

            var childService = new ServiceDto
            {
                Id = serviceId,
                Name = "Child Service Test References",
                CybersourceAccessKey = "csAccessKey",
                CybersourceSecretKey = "csSecretKey",
                MerchantId = "merchantId",
                Container = false,
                ServiceContainerId = containerId,
                BinGroups = new List<BinGroupDto> { new BinGroupDto { Id = binGroupId, Name = "Child Service BinGroup Test References" } },
                AskUserForReferences = true, //hijo pide referencias
                ReferenceParamName = "Referencia 1", //hijo tiene referencias
            };

            //Act
            _serviceService.Edit(childService);

            //Assert
            _respositoryServiceMock.Verify(x => x.Edit(It.IsAny<Service>()));
            _respositoryServiceMock.Verify(x => x.Save());
        }

        [TestMethod]
        public void Edit_ChildServiceAskForReferences_ContainerHasReferences_Success()
        {
            //Arrange
            _serviceService = new ServiceService(_respositoryServiceMock.Object, _repositoryGatewayMock.Object, _repositoryServiceAsociatedMock.Object, _repositoryPaymentMock.Object,
                _repositoryApplicationUserMock.Object, _repositoryBinMock.Object, _serviceHighwayMock.Object, _repositoryNotificationMock.Object, _loggerServiceMock.Object,
                _repositoryCardMock.Object, _serviceFixedNotificationMock.Object, _serviceEmailMessageMock.Object, _repositoryBinGroupMock.Object);

            var serviceId = Guid.NewGuid();
            var binGroupId = Guid.NewGuid();
            var containerId = Guid.NewGuid();

            _repositoryBinGroupMock
                .Setup(x => x.All(It.IsAny<Expression<Func<BinGroup, bool>>>()))
                .Returns(() => new List<BinGroup> { new BinGroup() }.AsQueryable());

            _repositoryGatewayMock
                .Setup(x => x.AllNoTracking(It.IsAny<Expression<Func<Gateway, bool>>>()))
                .Returns(() => new List<Gateway> { new Gateway { Id = Guid.NewGuid(), Enum = (int)GatewayEnum.Apps } }.AsQueryable());

            _respositoryServiceMock
                .SetupSequence(x => x.AllNoTracking(It.IsAny<Expression<Func<Service, bool>>>())) //Setup en secuencia (*)
                .Returns(new List<Service>().AsQueryable()) //(*1) La primera vez que compara que no exista Servicio con mismo Name devuelve null
                .Returns(new List<Service>() //(*2) La segunda vez va a buscar el ServiceContainer
                {
                    new Service()
                    {
                        Id = containerId,
                        Name = "Container Service Test References",
                        CybersourceAccessKey = "csAccessKey",
                        CybersourceSecretKey = "csSecretKey",
                        MerchantId = "merchantId",
                        Container = true,
                        ServiceContainerId = null,
                        AskUserForReferences = false, //contenedor no pide referencias (el hijo sí)
                        ReferenceParamName = "Referencia 1", //contenedor tiene referencias
                    }
                }.AsQueryable());

            _respositoryServiceMock
                .Setup(x => x.GetById(serviceId, It.IsAny<Expression<Func<Service, object>>[]>()))
                .Returns(() =>
                    new Service()
                    {
                        Id = serviceId,
                        Name = "Child Service Test References",
                        CybersourceAccessKey = "csAccessKey",
                        CybersourceSecretKey = "csSecretKey",
                        MerchantId = "merchantId",
                        Container = false,
                        ServiceContainerId = containerId,
                        BinGroups = new List<BinGroup> { new BinGroup { Id = binGroupId, Name = "Child Service BinGroup Test References" } },
                        AskUserForReferences = true, //hijo pide referencias
                        ReferenceParamName = null, //hijo no tiene referencias
                    }
                );

            var childService = new ServiceDto
            {
                Id = serviceId,
                Name = "Child Service Test References",
                CybersourceAccessKey = "csAccessKey",
                CybersourceSecretKey = "csSecretKey",
                MerchantId = "merchantId",
                Container = false,
                ServiceContainerId = containerId,
                BinGroups = new List<BinGroupDto> { new BinGroupDto { Id = binGroupId, Name = "Child Service BinGroup Test References" } },
                AskUserForReferences = true, //hijo pide referencias
                ReferenceParamName = null, //hijo no tiene referencias
            };

            //Act
            _serviceService.Edit(childService);

            //Assert
            _respositoryServiceMock.Verify(x => x.Edit(It.IsAny<Service>()));
            _respositoryServiceMock.Verify(x => x.Save());
        }

        [TestMethod]
        public void Edit_ContainerServiceAskForReferences_ChildHasReferences_Success()
        {
            //Arrange
            _serviceService = new ServiceService(_respositoryServiceMock.Object, _repositoryGatewayMock.Object, _repositoryServiceAsociatedMock.Object, _repositoryPaymentMock.Object,
                _repositoryApplicationUserMock.Object, _repositoryBinMock.Object, _serviceHighwayMock.Object, _repositoryNotificationMock.Object, _loggerServiceMock.Object,
                _repositoryCardMock.Object, _serviceFixedNotificationMock.Object, _serviceEmailMessageMock.Object, _repositoryBinGroupMock.Object);

            var serviceId = Guid.NewGuid();
            var binGroupId = Guid.NewGuid();
            var containerId = Guid.NewGuid();

            _repositoryBinGroupMock
                .Setup(x => x.All(It.IsAny<Expression<Func<BinGroup, bool>>>()))
                .Returns(() => new List<BinGroup> { new BinGroup() }.AsQueryable());

            _repositoryGatewayMock
                .Setup(x => x.AllNoTracking(It.IsAny<Expression<Func<Gateway, bool>>>()))
                .Returns(() => new List<Gateway> { new Gateway { Id = Guid.NewGuid(), Enum = (int)GatewayEnum.Apps } }.AsQueryable());

            _respositoryServiceMock
                .SetupSequence(x => x.AllNoTracking(It.IsAny<Expression<Func<Service, bool>>>())) //Setup en secuencia (*)
                .Returns(new List<Service>().AsQueryable()) //(*1) La primera vez que compara que no exista Servicio con mismo Name devuelve null
                .Returns(new List<Service>() //(*2) La segunda vez va a buscar el ServiceContainer
                {
                    new Service()
                    {
                        Id = containerId,
                        Name = "Container Service Test References",
                        CybersourceAccessKey = "csAccessKey",
                        CybersourceSecretKey = "csSecretKey",
                        MerchantId = "merchantId",
                        Container = true,
                        ServiceContainerId = null,
                        AskUserForReferences = true, //contenedor pide referencias
                        ReferenceParamName = null, //contenedor no tiene referencias
                    }
                }.AsQueryable());

            _respositoryServiceMock
                .Setup(x => x.GetById(serviceId, It.IsAny<Expression<Func<Service, object>>[]>()))
                .Returns(() =>
                    new Service()
                    {
                        Id = serviceId,
                        Name = "Child Service Test References",
                        CybersourceAccessKey = "csAccessKey",
                        CybersourceSecretKey = "csSecretKey",
                        MerchantId = "merchantId",
                        Container = false,
                        ServiceContainerId = containerId,
                        BinGroups = new List<BinGroup> { new BinGroup { Id = binGroupId, Name = "Child Service BinGroup Test References" } },
                        AskUserForReferences = false, //hijo no pide referencias (el contenedor sí)
                        ReferenceParamName = "Referencia 1", //hijo tiene referencias
                    }
                );

            var childService = new ServiceDto
            {
                Id = serviceId,
                Name = "Child Service Test References",
                CybersourceAccessKey = "csAccessKey",
                CybersourceSecretKey = "csSecretKey",
                MerchantId = "merchantId",
                Container = false,
                ServiceContainerId = containerId,
                BinGroups = new List<BinGroupDto> { new BinGroupDto { Id = binGroupId, Name = "Child Service BinGroup Test References" } },
                AskUserForReferences = false, //hijo no pide referencias (el contenedor sí)
                ReferenceParamName = "Referencia 1", //hijo tiene referencias
            };

            //Act
            _serviceService.Edit(childService);

            //Assert
            _respositoryServiceMock.Verify(x => x.Edit(It.IsAny<Service>()));
            _respositoryServiceMock.Verify(x => x.Save());
        }

        [TestMethod]
        public void Edit_ContainerServiceAskForReferences_ContainerHasReferences_Success()
        {
            //Arrange
            _serviceService = new ServiceService(_respositoryServiceMock.Object, _repositoryGatewayMock.Object, _repositoryServiceAsociatedMock.Object, _repositoryPaymentMock.Object,
                _repositoryApplicationUserMock.Object, _repositoryBinMock.Object, _serviceHighwayMock.Object, _repositoryNotificationMock.Object, _loggerServiceMock.Object,
                _repositoryCardMock.Object, _serviceFixedNotificationMock.Object, _serviceEmailMessageMock.Object, _repositoryBinGroupMock.Object);

            var serviceId = Guid.NewGuid();
            var binGroupId = Guid.NewGuid();
            var containerId = Guid.NewGuid();

            _repositoryBinGroupMock
                .Setup(x => x.All(It.IsAny<Expression<Func<BinGroup, bool>>>()))
                .Returns(() => new List<BinGroup> { new BinGroup() }.AsQueryable());

            _repositoryGatewayMock
                .Setup(x => x.AllNoTracking(It.IsAny<Expression<Func<Gateway, bool>>>()))
                .Returns(() => new List<Gateway> { new Gateway { Id = Guid.NewGuid(), Enum = (int)GatewayEnum.Apps } }.AsQueryable());

            _respositoryServiceMock
                .SetupSequence(x => x.AllNoTracking(It.IsAny<Expression<Func<Service, bool>>>())) //Setup en secuencia (*)
                .Returns(new List<Service>().AsQueryable()) //(*1) La primera vez que compara que no exista Servicio con mismo Name devuelve null
                .Returns(new List<Service>() //(*2) La segunda vez va a buscar el ServiceContainer
                {
                    new Service()
                    {
                        Id = containerId,
                        Name = "Container Service Test References",
                        CybersourceAccessKey = "csAccessKey",
                        CybersourceSecretKey = "csSecretKey",
                        MerchantId = "merchantId",
                        Container = true,
                        ServiceContainerId = null,
                        AskUserForReferences = true, //contenedor pide referencias
                        ReferenceParamName = "Referencia 1", //contenedor tiene referencias
                    }
                }.AsQueryable());

            _respositoryServiceMock
                .Setup(x => x.GetById(serviceId, It.IsAny<Expression<Func<Service, object>>[]>()))
                .Returns(() =>
                    new Service()
                    {
                        Id = serviceId,
                        Name = "Child Service Test References",
                        CybersourceAccessKey = "csAccessKey",
                        CybersourceSecretKey = "csSecretKey",
                        MerchantId = "merchantId",
                        Container = false,
                        ServiceContainerId = containerId,
                        BinGroups = new List<BinGroup> { new BinGroup { Id = binGroupId, Name = "Child Service BinGroup Test References" } },
                        AskUserForReferences = false, //hijo no pide referencias (el contenedor sí)
                        ReferenceParamName = null, //hijo no tiene referencias
                    }
                );

            var childService = new ServiceDto
            {
                Id = serviceId,
                Name = "Child Service Test References",
                CybersourceAccessKey = "csAccessKey",
                CybersourceSecretKey = "csSecretKey",
                MerchantId = "merchantId",
                Container = false,
                ServiceContainerId = containerId,
                BinGroups = new List<BinGroupDto> { new BinGroupDto { Id = binGroupId, Name = "Child Service BinGroup Test References" } },
                AskUserForReferences = false, //hijo no pide referencias (el contenedor sí)
                ReferenceParamName = null, //hijo no tiene referencias
            };

            //Act
            _serviceService.Edit(childService);

            //Assert
            _respositoryServiceMock.Verify(x => x.Edit(It.IsAny<Service>()));
            _respositoryServiceMock.Verify(x => x.Save());
        }


        //GET SERVICE REFERENCE PARAMS
        /*
        [TestMethod]
        public void ChildServiceHasReferences_ContainerHasntReferences_TakeChilds()
        {
            //¿Cómo se puede simular que se invoca desde el Web para que haga la transformación que hace la API?
        }

        [TestMethod]
        public void ChildServiceHasntReferences_ContainerHasReferences_TakeContainers()
        {
            //¿Cómo se puede simular que se invoca desde el Web para que haga la transformación que hace la API?
        }

        [TestMethod]
        public void ChildServiceHasReferences_ContainerHasReferences_TakeChilds()
        {
            //¿Cómo se puede simular que se invoca desde el Web para que haga la transformación que hace la API?
        }
        */

    }
}