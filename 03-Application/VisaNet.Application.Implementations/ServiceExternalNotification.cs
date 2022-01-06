using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using VisaNet.Application.Interfaces;
using VisaNet.Common.Logging.NLog;
using VisaNet.Domain.Entities;
using VisaNet.Domain.Entities.Enums;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Domain.EntitiesDtos.ExternalRequest;
using VisaNet.Repository.Interfaces.Interfaces;

namespace VisaNet.Application.Implementations
{
    public class ServiceExternalNotification : IServiceExternalNotification
    {
        private readonly IServicePostSignatureFactory _servicePostSignatureFactory;
        private readonly IServicePostNotification _servicePostNotification;
        private readonly IServiceWebhookNewAssociation _serviceWebhookNewAssociation;
        private readonly IServiceWebhookDown _serviceWebhookDown;
        private readonly IServiceAnonymousUser _serviceAnonymousUser;
        private readonly IServiceApplicationUser _serviceApplicationUser;
        private readonly IServiceService _serviceService;
        private readonly IRepositoryBin _repositoryBin;

        public ServiceExternalNotification(IServicePostSignatureFactory servicePostSignatureFactory, IServicePostNotification servicePostNotification,
            IServiceWebhookNewAssociation serviceWebhookNewAssociation, IServiceWebhookDown serviceWebhookDown, IServiceAnonymousUser serviceAnonymousUser,
            IServiceApplicationUser serviceApplicationUser, IServiceService serviceService, IRepositoryBin repositoryBin)
        {
            _servicePostSignatureFactory = servicePostSignatureFactory;
            _servicePostNotification = servicePostNotification;
            _serviceWebhookNewAssociation = serviceWebhookNewAssociation;
            _serviceWebhookDown = serviceWebhookDown;
            _serviceAnonymousUser = serviceAnonymousUser;
            _serviceApplicationUser = serviceApplicationUser;
            _serviceService = serviceService;
            _repositoryBin = repositoryBin;
        }


        //ALTAS
        public bool NotifyExternalSourceNewAssociation(ServiceAssociated entity, Card card, string operationId)
        {
            var service = GetIntegrationService(entity.ServiceId);

            if (string.IsNullOrEmpty(service.ExternalUrlAdd) ||
                service.UrlIntegrationVersion == UrlIntegrationVersionEnumDto.NotApply)
            {
                return true;
            }

            var cardBin = GetCardBin(card.BIN);

            var dto = new WebhookNewAssociationDto
            {
                CardDueDate = card.DueDate.ToString("MM/yy"),
                CardMask = card.MaskedNumber.Substring(12, 4),
                IdCard = card.ExternalId.ToString(),
                IdApp = service.UrlName,
                IdUser = entity.IdUserExternal.ToString(),
                IdOperationApp = operationId,
                IdOperation = Guid.NewGuid().ToString(),
                RefCliente1 = entity.ReferenceNumber,
                RefCliente2 = entity.ReferenceNumber2,
                RefCliente3 = entity.ReferenceNumber3,
                RefCliente4 = entity.ReferenceNumber4,
                RefCliente5 = entity.ReferenceNumber5,
                RefCliente6 = entity.ReferenceNumber6,
                IsAssociation = true,
                IsPayment = false,
                CardType = service.InformCardType ? ((CardTypeDto)(int)cardBin.CardType) : (CardTypeDto?)null,
                CardBank = service.InformCardBank && cardBin.Bank != null ? cardBin.Bank.Name : string.Empty,
                CardBankCode = service.InformCardBank && cardBin.Bank != null ? cardBin.Bank.Code.ToString() : string.Empty,
                CardAffiliation = service.InformCardAffiliation && cardBin.AffiliationCard != null ? cardBin.AffiliationCard.Name : string.Empty,
                CardAffiliationCode = service.InformCardAffiliation && cardBin.AffiliationCard != null ? cardBin.AffiliationCard.Code.ToString() : string.Empty,

                UserData = new UserDataInputDto
                {
                    Address = entity.RegisteredUser.Address,
                    Email = entity.RegisteredUser.Email,
                    IdentityNumber = entity.RegisteredUser.IdentityNumber,
                    MobileNumber = entity.RegisteredUser.MobileNumber,
                    Name = entity.RegisteredUser.Name,
                    Surname = entity.RegisteredUser.Surname,
                    PhoneNumber = entity.RegisteredUser.PhoneNumber
                },
            };

            //Firmar los campos segun version
            var servicePostSignature = _servicePostSignatureFactory.GetPostSignatureVersion((UrlIntegrationVersionEnum)((int)service.UrlIntegrationVersion));
            var fieldsToSign = servicePostSignature.GetFieldsForNewUserSignature(dto);
            var signature = servicePostSignature.GetSignature(fieldsToSign, service.CertificateThumbprintVisa);

            //Notificar al comercio
            var httpCode = NotifyCommerceNewTransaction(dto, service.ExternalUrlAdd, signature, fieldsToSign);

            return httpCode == HttpStatusCode.OK;
        }

        public bool NotifyExternalSourceNewAssociation(PaymentDto paymentDto, string idUserExternal, string idCardExternal)
        {
            var service = GetIntegrationService(paymentDto.ServiceId);

            if (string.IsNullOrEmpty(service.ExternalUrlAdd) ||
                service.UrlIntegrationVersion == UrlIntegrationVersionEnumDto.NotApply)
            {
                return true;
            }

            var cardBin = GetCardBin(paymentDto.Card.BIN);
            var user = GetUserData(paymentDto);

            var dto = new WebhookNewAssociationDto
            {
                CardDueDate = paymentDto.Card.DueDate.ToString("MM/yy"),
                CardMask = paymentDto.Card.MaskedNumber.Substring(12, 4),
                IdCard = idCardExternal,
                IdApp = service.UrlName,
                IdUser = idUserExternal,
                IdOperationApp = paymentDto.IdOperation,
                IdOperation = Guid.NewGuid().ToString(),
                RefCliente1 = paymentDto.ReferenceNumber,
                RefCliente2 = paymentDto.ReferenceNumber2,
                RefCliente3 = paymentDto.ReferenceNumber3,
                RefCliente4 = paymentDto.ReferenceNumber4,
                RefCliente5 = paymentDto.ReferenceNumber5,
                RefCliente6 = paymentDto.ReferenceNumber6,
                IsAssociation = true,
                IsPayment = false,
                CardType = service.InformCardType ? ((CardTypeDto)(int)cardBin.CardType) : (CardTypeDto?)null,
                CardBank = service.InformCardBank && cardBin.Bank != null ? cardBin.Bank.Name : string.Empty,
                CardBankCode = service.InformCardBank && cardBin.Bank != null ? cardBin.Bank.Code.ToString() : string.Empty,
                CardAffiliation = service.InformCardAffiliation && cardBin.AffiliationCard != null ? cardBin.AffiliationCard.Name : string.Empty,
                CardAffiliationCode = service.InformCardAffiliation && cardBin.AffiliationCard != null ? cardBin.AffiliationCard.Code.ToString() : string.Empty,
                UserData = new UserDataInputDto
                {
                    Address = user.Address,
                    Email = user.Email,
                    IdentityNumber = user.IdentityNumber,
                    MobileNumber = user.MobileNumber,
                    Name = user.Name,
                    Surname = user.Surname,
                    PhoneNumber = user.PhoneNumber
                },
            };

            //Firmar los campos segun version
            var servicePostSignature = _servicePostSignatureFactory.GetPostSignatureVersion((UrlIntegrationVersionEnum)service.UrlIntegrationVersion);
            var fieldsToSign = servicePostSignature.GetFieldsForNewUserSignature(dto);
            var signature = servicePostSignature.GetSignature(fieldsToSign, service.CertificateThumbprintVisa);

            //Notificar al comercio
            var httpCode = NotifyCommerceNewTransaction(dto, service.ExternalUrlAdd, signature, fieldsToSign);

            return httpCode == HttpStatusCode.OK;
        }

        public bool NotifyExternalSourceNewCard(ServiceAssociated entity, Card card, string operationId)
        {
            var service = GetIntegrationService(entity.ServiceId);

            if (string.IsNullOrEmpty(service.ExternalUrlAdd) ||
                service.UrlIntegrationVersion == UrlIntegrationVersionEnumDto.NotApply)
            {
                return true;
            }

            var cardBin = GetCardBin(card.BIN);

            var dto = new WebhookNewAssociationDto
            {
                CardDueDate = card.DueDate.ToString("MM/yy"),
                CardMask = card.MaskedNumber.Substring(12, 4),
                IdCard = card.ExternalId.ToString(),
                IdApp = service.UrlName,
                IdUser = entity.IdUserExternal.ToString(),
                IdOperationApp = operationId,
                IdOperation = Guid.NewGuid().ToString(),
                RefCliente1 = entity.ReferenceNumber,
                RefCliente2 = entity.ReferenceNumber2,
                RefCliente3 = entity.ReferenceNumber3,
                RefCliente4 = entity.ReferenceNumber4,
                RefCliente5 = entity.ReferenceNumber5,
                RefCliente6 = entity.ReferenceNumber6,
                IsAssociation = true,
                IsPayment = false,
                CardType = service.InformCardType ? ((CardTypeDto)(int)cardBin.CardType) : (CardTypeDto?)null,
                CardBank = service.InformCardBank && cardBin.Bank != null ? cardBin.Bank.Name : string.Empty,
                CardBankCode = service.InformCardBank && cardBin.Bank != null ? cardBin.Bank.Code.ToString() : string.Empty,
                CardAffiliation = service.InformCardAffiliation && cardBin.AffiliationCard != null ? cardBin.AffiliationCard.Name : string.Empty,
                CardAffiliationCode = service.InformCardAffiliation && cardBin.AffiliationCard != null ? cardBin.AffiliationCard.Code.ToString() : string.Empty,
            };

            //Firmar los campos segun version
            var servicePostSignature = _servicePostSignatureFactory.GetPostSignatureVersion((UrlIntegrationVersionEnum)((int)service.UrlIntegrationVersion));
            var fieldsToSign = servicePostSignature.GetFieldsForNewCardSignature(dto);
            var signature = servicePostSignature.GetSignature(fieldsToSign, service.CertificateThumbprintVisa);

            //Notificar al comercio
            var httpCode = NotifyCommerceNewTransaction(dto, service.ExternalUrlAdd, signature, fieldsToSign);

            return httpCode == HttpStatusCode.OK;
        }

        public bool NotifyExternalSourceNewCard(PaymentDto paymentDto, string idUserExternal, string idCardExternal)
        {
            var service = GetIntegrationService(paymentDto.ServiceId);

            if (string.IsNullOrEmpty(service.ExternalUrlAdd) ||
                service.UrlIntegrationVersion == UrlIntegrationVersionEnumDto.NotApply)
            {
                return true;
            }

            var cardBin = GetCardBin(paymentDto.Card.BIN);

            var dto = new WebhookNewAssociationDto
            {
                CardDueDate = paymentDto.Card.DueDate.ToString("MM/yy"),
                CardMask = paymentDto.Card.MaskedNumber.Substring(12, 4),
                IdCard = idCardExternal,
                IdApp = service.UrlName,
                IdUser = idUserExternal,
                IdOperationApp = paymentDto.IdOperation,
                IdOperation = Guid.NewGuid().ToString(),
                RefCliente1 = paymentDto.ReferenceNumber,
                RefCliente2 = paymentDto.ReferenceNumber2,
                RefCliente3 = paymentDto.ReferenceNumber3,
                RefCliente4 = paymentDto.ReferenceNumber4,
                RefCliente5 = paymentDto.ReferenceNumber5,
                RefCliente6 = paymentDto.ReferenceNumber6,
                IsAssociation = true,
                IsPayment = false,
                CardType = service.InformCardType ? ((CardTypeDto)(int)cardBin.CardType) : (CardTypeDto?)null,
                CardBank = service.InformCardBank && cardBin.Bank != null ? cardBin.Bank.Name : string.Empty,
                CardBankCode = service.InformCardBank && cardBin.Bank != null ? cardBin.Bank.Code.ToString() : string.Empty,
                CardAffiliation = service.InformCardAffiliation && cardBin.AffiliationCard != null ? cardBin.AffiliationCard.Name : string.Empty,
                CardAffiliationCode = service.InformCardAffiliation && cardBin.AffiliationCard != null ? cardBin.AffiliationCard.Code.ToString() : string.Empty,
            };

            //Firmar los campos segun version
            var servicePostSignature = _servicePostSignatureFactory.GetPostSignatureVersion((UrlIntegrationVersionEnum)service.UrlIntegrationVersion);
            var fieldsToSign = servicePostSignature.GetFieldsForNewCardSignature(dto);
            var signature = servicePostSignature.GetSignature(fieldsToSign, service.CertificateThumbprintVisa);

            //Notificar al comercio
            var httpCode = NotifyCommerceNewTransaction(dto, service.ExternalUrlAdd, signature, fieldsToSign);

            return httpCode == HttpStatusCode.OK;
        }

        public bool NotifyExternalSourceNewPayment(NotifyPaymentDto notify)
        {
            var service = GetIntegrationService(notify.PaymentDto.ServiceId);

            if (string.IsNullOrEmpty(service.ExternalUrlAdd) ||
                (int)service.UrlIntegrationVersion < (int)UrlIntegrationVersionEnumDto.ThirdVersion) return true;

            var cardBin = GetCardBin(notify.PaymentDto.Card.BIN);
            var user = GetUserData(notify.PaymentDto);

            var dto = new WebhookNewAssociationDto
            {
                CardDueDate = notify.PaymentDto.Card.DueDate.ToString("MM/yy"),
                CardMask = notify.PaymentDto.Card.MaskedNumber.Substring(12, 4),
                IdCard = notify.PaymentDto.Card.ExternalId.ToString(),
                IdApp = service.UrlName,
                IdUser = notify.PaymentDto.ServiceAssociatedDto != null ?
                    notify.PaymentDto.ServiceAssociatedDto.IdUserExternal.ToString() :
                    notify.PaymentDto.IdUserExternal,
                IdOperationApp = notify.PaymentDto.IdOperation,
                IdOperation = Guid.NewGuid().ToString(),
                RefCliente1 = notify.References[0],
                RefCliente2 = notify.References[1],
                RefCliente3 = notify.References[2],
                RefCliente4 = notify.References[3],
                RefCliente5 = notify.References[4],
                RefCliente6 = notify.References[5],
                IsAssociation = false,
                IsPayment = true,
                CardType = service.InformCardType ? ((CardTypeDto)(int)cardBin.CardType) : (CardTypeDto?)null,
                CardBank = service.InformCardBank && cardBin.Bank != null ? cardBin.Bank.Name : string.Empty,
                CardBankCode = service.InformCardBank && cardBin.Bank != null ? cardBin.Bank.Code.ToString() : string.Empty,
                CardAffiliation = service.InformCardAffiliation && cardBin.AffiliationCard != null ? cardBin.AffiliationCard.Name : string.Empty,
                CardAffiliationCode = service.InformCardAffiliation && cardBin.AffiliationCard != null ? cardBin.AffiliationCard.Code.ToString() : string.Empty,
                TransactionNumber = notify.PaymentDto.TransactionNumber,
                DiscountAmount = notify.PaymentDto.Discount,
                UserData = new UserDataInputDto
                {
                    Address = user.Address,
                    Email = user.Email,
                    IdentityNumber = user.IdentityNumber,
                    MobileNumber = user.MobileNumber,
                    Name = user.Name,
                    Surname = user.Surname,
                    PhoneNumber = user.PhoneNumber
                },
            };

            //Firmar los campos segun version
            var servicePostSignature = _servicePostSignatureFactory.GetPostSignatureVersion((UrlIntegrationVersionEnum)((int)service.UrlIntegrationVersion));
            var fieldsToSign = servicePostSignature.GetFieldsForNewPaymentSignature(dto);
            var signature = servicePostSignature.GetSignature(fieldsToSign, service.CertificateThumbprintVisa);

            //Notificar al comercio
            var httpCode = NotifyCommerceNewTransaction(dto, service.ExternalUrlAdd, signature, fieldsToSign, notify.GatewayEnum);

            return httpCode == HttpStatusCode.OK;
        }

        public bool NotifyExternalSourceNewPayment(PaymentDto paymentDto, string idUserExternal, string idCardExternal, bool withAssociation)
        {
            var service = GetIntegrationService(paymentDto.ServiceId);

            if (string.IsNullOrEmpty(service.ExternalUrlAdd) ||
                (int)service.UrlIntegrationVersion < (int)UrlIntegrationVersionEnumDto.ThirdVersion) return true;

            var cardBin = GetCardBin(paymentDto.Card.BIN);
            var user = GetUserData(paymentDto);

            var dto = new WebhookNewAssociationDto
            {
                CardDueDate = paymentDto.Card.DueDate.ToString("MM/yy"),
                CardMask = paymentDto.Card.MaskedNumber.Substring(12, 4),
                IdCard = idCardExternal,
                IdApp = service.UrlName,
                IdUser = idUserExternal,
                IdOperationApp = paymentDto.IdOperation,
                IdOperation = Guid.NewGuid().ToString(),
                RefCliente1 = paymentDto.ReferenceNumber,
                RefCliente2 = paymentDto.ReferenceNumber2,
                RefCliente3 = paymentDto.ReferenceNumber3,
                RefCliente4 = paymentDto.ReferenceNumber4,
                RefCliente5 = paymentDto.ReferenceNumber5,
                RefCliente6 = paymentDto.ReferenceNumber6,
                IsAssociation = withAssociation,
                IsPayment = true,
                CardType = service.InformCardType ? ((CardTypeDto)(int)cardBin.CardType) : (CardTypeDto?)null,
                CardBank = service.InformCardBank && cardBin.Bank != null ? cardBin.Bank.Name : string.Empty,
                CardBankCode = service.InformCardBank && cardBin.Bank != null ? cardBin.Bank.Code.ToString() : string.Empty,
                CardAffiliation = service.InformCardAffiliation && cardBin.AffiliationCard != null ? cardBin.AffiliationCard.Name : string.Empty,
                CardAffiliationCode = service.InformCardAffiliation && cardBin.AffiliationCard != null ? cardBin.AffiliationCard.Code.ToString() : string.Empty,
                TransactionNumber = paymentDto.TransactionNumber,
                DiscountAmount = paymentDto.Discount,
                UserData = new UserDataInputDto
                {
                    Address = user.Address,
                    Email = user.Email,
                    IdentityNumber = user.IdentityNumber,
                    MobileNumber = user.MobileNumber,
                    Name = user.Name,
                    Surname = user.Surname,
                    PhoneNumber = user.PhoneNumber
                },
            };

            //Firmar los campos segun version
            var servicePostSignature = _servicePostSignatureFactory.GetPostSignatureVersion((UrlIntegrationVersionEnum)((int)service.UrlIntegrationVersion));
            var fieldsToSign = servicePostSignature.GetFieldsForNewPaymentSignature(dto);
            var signature = servicePostSignature.GetSignature(fieldsToSign, service.CertificateThumbprintVisa);

            //Notificar al comercio
            var httpCode = NotifyCommerceNewTransaction(dto, service.ExternalUrlAdd, signature, fieldsToSign, paymentDto.GatewayEnum);

            return httpCode == HttpStatusCode.OK;
        }

        public bool NotifyExternalSourceNewPaymentAnonymous(PaymentDto paymentDto)
        {
            var service = GetIntegrationService(paymentDto.ServiceId);

            if (string.IsNullOrEmpty(service.ExternalUrlAdd) ||
                (int)service.UrlIntegrationVersion < (int)UrlIntegrationVersionEnumDto.ThirdVersion) return true;

            var cardBin = GetCardBin(paymentDto.Card.BIN);
            var user = GetUserData(paymentDto);

            var dto = new WebhookNewAssociationDto
            {
                CardDueDate = paymentDto.Card.DueDate.ToString("MM/yy"),
                CardMask = paymentDto.Card.MaskedNumber.Substring(12, 4),
                IdApp = service.UrlName,
                IdOperationApp = paymentDto.IdOperation,
                IdOperation = Guid.NewGuid().ToString(),
                RefCliente1 = paymentDto.ReferenceNumber,
                RefCliente2 = paymentDto.ReferenceNumber2,
                RefCliente3 = paymentDto.ReferenceNumber3,
                RefCliente4 = paymentDto.ReferenceNumber4,
                RefCliente5 = paymentDto.ReferenceNumber5,
                RefCliente6 = paymentDto.ReferenceNumber6,
                IsAssociation = false,
                IsPayment = true,
                CardType = service.InformCardType ? ((CardTypeDto)(int)cardBin.CardType) : (CardTypeDto?)null,
                CardBank = service.InformCardBank && cardBin.Bank != null ? cardBin.Bank.Name : string.Empty,
                CardBankCode = service.InformCardBank && cardBin.Bank != null ? cardBin.Bank.Code.ToString() : string.Empty,
                CardAffiliation = service.InformCardAffiliation && cardBin.AffiliationCard != null ? cardBin.AffiliationCard.Name : string.Empty,
                CardAffiliationCode = service.InformCardAffiliation && cardBin.AffiliationCard != null ? cardBin.AffiliationCard.Code.ToString() : string.Empty,
                TransactionNumber = paymentDto.TransactionNumber,
                DiscountAmount = paymentDto.Discount,
                UserData = new UserDataInputDto
                {
                    Address = user.Address,
                    Email = user.Email,
                    IdentityNumber = user.IdentityNumber,
                    MobileNumber = user.MobileNumber,
                    Name = user.Name,
                    Surname = user.Surname,
                    PhoneNumber = user.PhoneNumber
                },
            };

            //Firmar los campos segun version
            var servicePostSignature = _servicePostSignatureFactory.GetPostSignatureVersion((UrlIntegrationVersionEnum)((int)service.UrlIntegrationVersion));
            var fieldsToSign = servicePostSignature.GetFieldsForNewPaymentSignature(dto);
            var signature = servicePostSignature.GetSignature(fieldsToSign, service.CertificateThumbprintVisa);

            //Notificar al comercio
            var httpCode = NotifyCommerceNewTransaction(dto, service.ExternalUrlAdd, signature, fieldsToSign, paymentDto.GatewayEnum);

            return httpCode == HttpStatusCode.OK;
        }

        private HttpStatusCode NotifyCommerceNewTransaction(WebhookNewAssociationDto dto, string externalUrlAdd, string signature,
            IDictionary<string, string> fieldsToSign, GatewayEnumDto gateway = GatewayEnumDto.Carretera)
        {
            var httpCode = HttpStatusCode.InternalServerError;
            WebhookNewAssociationDto webhookNewAssociationDtoCreated = null;

            try
            {
                //Persistir en BD
                webhookNewAssociationDtoCreated = _serviceWebhookNewAssociation.Create(dto, true);
            }
            catch (Exception e)
            {
                NLogLogger.LogEvent(NLogType.Info, "ServiceExternalNotification - NotifyCommerceNewTransaction - Error al persistir WebhookNewAssociation");
                NLogLogger.LogEvent(e);
            }

            //Notificar App
            if (gateway == GatewayEnumDto.PagoLink && 1 == 0)
            {
                //TODO: por ahora no se va a usar (por eso el 1==0)
                string connectionString = "Endpoint=sb://portalcomerciotest.servicebus.windows.net/;SharedAccessKeyName=hexacta;SharedAccessKey=WfXnQFdoNcjCGNYq3BFN7wKM7vROTbw4OQxSADuTTq0=";
                var queueName = "vontransactions";

                var factory = MessagingFactory.CreateFromConnectionString(connectionString);

                //Sending a message
                MessageSender testQueueSender = factory.CreateMessageSender(queueName);
                BrokeredMessage message = new BrokeredMessage(webhookNewAssociationDtoCreated);
                testQueueSender.Send(message);
            }
            else
            {
                try
                {
                    //Notificar App
                    httpCode = _servicePostNotification.NotifyExternalSourcePostWithSignature(externalUrlAdd, signature, "FirmaDigital", fieldsToSign);
                    NLogLogger.LogEvent(NLogType.Info, string.Format("ServiceExternalNotification - NotifyCommerceNewTransaction - Se notificó al comercio. " +
                        "IdApp: {0}, IdOperation: {1}, Código HTTP: {2}", dto.IdApp, dto.IdOperationApp, httpCode));

                    //Actualizar estado BD
                    webhookNewAssociationDtoCreated.HttpResponseCode = ((int)httpCode).ToString();
                    _serviceWebhookNewAssociation.Edit(webhookNewAssociationDtoCreated);
                }
                catch (Exception e)
                {
                    NLogLogger.LogEvent(NLogType.Info, "ServiceExternalNotification - NotifyCommerceNewTransaction - Error al actualizar HttpResponseCode de WebhookNewAssociation");
                    NLogLogger.LogEvent(e);
                }
            }

            return httpCode;
        }


        //BAJAS
        public bool NotifyExternalSourceDownService(ServiceAssociated entity)
        {
            var service = GetIntegrationService(entity.ServiceId);

            if (string.IsNullOrEmpty(service.ExternalUrlAdd) ||
                service.UrlIntegrationVersion == UrlIntegrationVersionEnumDto.NotApply)
            {
                return true;
            }

            var dto = new WebhookDownDto()
            {
                IdUser = entity.IdUserExternal.ToString(),
                IdOperation = Guid.NewGuid().ToString(),
                IdApp = service.UrlName
            };

            //Firmar los campos segun version
            var servicePostSignature = _servicePostSignatureFactory.GetPostSignatureVersion((UrlIntegrationVersionEnum)((int)service.UrlIntegrationVersion));
            var fieldsToSign = servicePostSignature.GetFieldsForUserDownSignature(dto);
            var signature = servicePostSignature.GetSignature(fieldsToSign, service.CertificateThumbprintVisa);

            //Notificar al comercio
            var httpCode = NotifyCommerceAssociationDown(dto, service.ExternalUrlRemove, signature, fieldsToSign);

            return httpCode == HttpStatusCode.OK;
        }

        public bool NotifyExternalSourceDownCard(ServiceAssociated entity, Guid externalCardId)
        {
            var service = GetIntegrationService(entity.ServiceId);

            if (string.IsNullOrEmpty(service.ExternalUrlAdd) ||
                service.UrlIntegrationVersion == UrlIntegrationVersionEnumDto.NotApply)
            {
                return true;
            }

            var dto = new WebhookDownDto()
            {
                IdUser = entity.IdUserExternal.ToString(),
                IdOperation = Guid.NewGuid().ToString(),
                IdCard = externalCardId.ToString(),
                IdApp = service.UrlName
            };

            //Firmar los campos segun version
            var servicePostSignature = _servicePostSignatureFactory.GetPostSignatureVersion((UrlIntegrationVersionEnum)((int)service.UrlIntegrationVersion));
            var fieldsToSign = servicePostSignature.GetFieldsForCardDownSignature(dto);
            var signature = servicePostSignature.GetSignature(fieldsToSign, service.CertificateThumbprintVisa);

            //Notificar al comercio
            var httpCode = NotifyCommerceAssociationDown(dto, service.ExternalUrlRemove, signature, fieldsToSign);

            return httpCode == HttpStatusCode.OK;
        }

        private HttpStatusCode NotifyCommerceAssociationDown(WebhookDownDto dto, string externalUrlDown, string signature,
            IDictionary<string, string> fieldsToSign)
        {
            var httpCode = HttpStatusCode.InternalServerError;
            WebhookDownDto webhookDownDtoCreated = null;

            try
            {
                //Persistir en BD
                webhookDownDtoCreated = _serviceWebhookDown.Create(dto, true);
            }
            catch (Exception e)
            {
                NLogLogger.LogEvent(NLogType.Info, "ServiceExternalNotification - NotifyCommerceAssociationDown - Error al persistir WebhookNewAssociation");
                NLogLogger.LogEvent(e);
            }

            try
            {
                //Notificar App
                httpCode = _servicePostNotification.NotifyExternalSourcePostWithSignature(externalUrlDown, signature, "FirmaDigital", fieldsToSign);
                NLogLogger.LogEvent(NLogType.Info, string.Format("ServiceExternalNotification - NotifyCommerceAssociationDown - Se notificó al comercio. " +
                    "IdApp: {0}, IdOperation: {1}, Código HTTP: {2}", dto.IdApp, dto.IdOperation, httpCode));

                //Actualizar estado BD
                webhookDownDtoCreated.HttpResponseCode = ((int)httpCode).ToString();
                _serviceWebhookDown.Edit(webhookDownDtoCreated);
            }
            catch (Exception e)
            {
                NLogLogger.LogEvent(NLogType.Info, "ServiceExternalNotification - NotifyCommerceAssociationDown - Error al actualizar HttpResponseCode de WebhookNewAssociation");
                NLogLogger.LogEvent(e);
            }

            return httpCode;
        }


        //AUXILIARES
        public bool AlreadyNotifiedExternalCard(string idApp, string idUserExternal, string idCardExternal)
        {
            return _serviceWebhookNewAssociation.AlreadyNotifiedExternalCard(idApp, idUserExternal, idCardExternal);
        }

        private Bin GetCardBin(int bin)
        {
            var cardBin = _repositoryBin.All(x => x.Value == bin, x => x.Bank, x => x.AffiliationCard).FirstOrDefault();
            if (cardBin == null)
            {
                var defaultBin = Convert.ToInt32(ConfigurationManager.AppSettings["DefaultBin"]);
                cardBin = _repositoryBin.All(x => x.Value == defaultBin, x => x.Bank, x => x.AffiliationCard).FirstOrDefault();
            }
            return cardBin;
        }

        private ServiceDto GetIntegrationService(Guid serviceId)
        {
            //Si tiene servicio contenedor, devuelve a su contenedor (que tiene los datos de integracion), sino se devuelve al obtenido.
            var service = _serviceService.GetById(serviceId, x => x.ServiceContainer);
            if (service.ServiceContainerDto == null)
            {
                return service;
            }
            return service.ServiceContainerDto;
        }

        private IUserDto GetUserData(PaymentDto paymentDto)
        {
            if (paymentDto.AnonymousUserId.HasValue)
            {
                return _serviceAnonymousUser.GetById(paymentDto.AnonymousUserId.Value);
            }
            return _serviceApplicationUser.GetById(paymentDto.RegisteredUserId.Value);
        }


    }
}