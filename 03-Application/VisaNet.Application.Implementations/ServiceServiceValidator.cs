using System;
using System.Linq;
using VisaNet.Application.Interfaces;
using VisaNet.Common.Logging.NLog;
using VisaNet.Domain.Entities.Enums;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Repository.Interfaces.Interfaces;

namespace VisaNet.Application.Implementations
{
    public class ServiceServiceValidator : IServiceServiceValidator
    {
        private readonly IServiceService _serviceService;
        private readonly IServiceEmailMessage _serviceEmailMessage;
        private readonly IRepositoryParameters _repositoryParameters;

        public ServiceServiceValidator(IServiceService serviceService, IServiceEmailMessage serviceEmailMessage, IRepositoryParameters repositoryParameters)
        {
            _serviceService = serviceService;
            _serviceEmailMessage = serviceEmailMessage;
            _repositoryParameters = repositoryParameters;
        }

        //Pago Link
        public bool ValidateLinkService(Guid serviceId)
        {
            var service = _serviceService.GetById(serviceId, x => x.ServiceGateways, x => x.ServiceGateways.Select(y => y.Gateway));
            return ValidateLinkService(service);
        }

        public bool ValidateLinkService(ServiceDto service)
        {
            var valid = true;
            var msg = "";
            ServiceDto serviceContainer = null;
            var hasServiceContainer = false;

            //SI TIENE SERVICIO CONTENEDOR LO OBTENGO
            if (service.ServiceContainerId.HasValue && service.ServiceContainerId.Value != Guid.Empty)
            {
                serviceContainer = _serviceService.GetById(service.ServiceContainerId.Value, x => x.ServiceGateways, x => x.ServiceGateways.Select(y => y.Gateway));
                hasServiceContainer = true;
            }

            //QUE TENGA IDAPP EL O SU PADRE
            var idApp = hasServiceContainer ? serviceContainer.UrlName : service.UrlName;
            if (string.IsNullOrEmpty(idApp))
            {
                NLogLogger.LogEvent(NLogType.Error, "ServiceServiceValidator - ValidateLinkService - IdApp vacío. ServiceId: " + (hasServiceContainer ? serviceContainer.Id : service.Id));
                msg += "IdApp vacío. ";
                valid = false;
            }

            //QUE TENGA PASARELA PAGO LINK
            var servicesGatewaysList = service.ServiceGatewaysDto.Where(x => x.Active).ToList();
            var pagoLinkGateway = servicesGatewaysList.FirstOrDefault(x => x.Gateway.Enum == (int)GatewayEnum.PagoLink);
            if (pagoLinkGateway == null)
            {
                NLogLogger.LogEvent(NLogType.Error, "ServiceServiceValidator - ValidateLinkService - No tiene pasarela Pago Link. ServiceId: " + service.Id);
                msg += "No tiene pasarela Pago Link. ";
                valid = false;
            }

            //QUE TENGA VISANETON ACTIVADO
            var allowsVon = hasServiceContainer ? serviceContainer.AllowVon : service.AllowVon;
            if (!allowsVon)
            {
                NLogLogger.LogEvent(NLogType.Error, "ServiceServiceValidator - ValidateLinkService - No tiene habilitado VisaNetOn. ServiceId: " + (hasServiceContainer ? serviceContainer.Id : service.Id));
                msg += "No tiene habilitado VisaNetOn. ";
                valid = false;
            }

            //QUE TENGA URL_TRANSACCION
            var urlTransaction = hasServiceContainer ? serviceContainer.ExternalUrlAdd : service.ExternalUrlAdd;
            if (string.IsNullOrEmpty(urlTransaction))
            {
                NLogLogger.LogEvent(NLogType.Error, "ServiceServiceValidator - ValidateLinkService - No tiene configurado URL Transacción. ServiceId: " + (hasServiceContainer ? serviceContainer.Id : service.Id));
                msg += "No tiene configurado url transacción. ";
                valid = false;
            }

            //QUE TENGA THUMBPRINT DE VISA
            var thumbprintVisa = hasServiceContainer ? serviceContainer.CertificateThumbprintVisa : service.CertificateThumbprintVisa;
            if (string.IsNullOrEmpty(thumbprintVisa))
            {
                NLogLogger.LogEvent(NLogType.Error, "ServiceServiceValidator - ValidateLinkService - No tiene configurado el Certificado de Visa. ServiceId: " + (hasServiceContainer ? serviceContainer.Id : service.Id));
                msg += "No tiene configurado el certificado de Visa. ";
                valid = false;
            }

            //QUE SEA VERSION INTEGRACION v4 EN ADELANTE
            var integrationVersion = hasServiceContainer ? (int)serviceContainer.UrlIntegrationVersion : (int)service.UrlIntegrationVersion;
            if (integrationVersion < (int)UrlIntegrationVersionEnumDto.FourthVersion)
            {
                NLogLogger.LogEvent(NLogType.Error, "ServiceServiceValidator - ValidateLinkService - No tiene la versión de integración adecuada. ServiceId: " + (hasServiceContainer ? serviceContainer.Id : service.Id));
                msg += "No tiene la versión de integración adecuada. ";
                valid = false;
            }

            //MANDAR MAIL A VISA
            if (!valid)
            {
                SendVisaInternalErrorEmail(service.Name, " para Pago Link", msg);
            }

            return valid;
        }


        //VisaNetOn
        public bool ValidateVisaNetOnService(Guid serviceId)
        {
            var service = _serviceService.GetById(serviceId, x => x.ServiceGateways, x => x.ServiceGateways.Select(y => y.Gateway));
            return ValidateVisaNetOnService(service);
        }

        public bool ValidateVisaNetOnService(ServiceDto service)
        {
            var valid = true;
            var msg = "";
            ServiceDto serviceContainer = null;
            var hasServiceContainer = false;

            //SI TIENE SERVICIO CONTENEDOR LO OBTENGO
            if (service.ServiceContainerId.HasValue && service.ServiceContainerId.Value != Guid.Empty)
            {
                serviceContainer = _serviceService.GetById(service.ServiceContainerId.Value, x => x.ServiceGateways, x => x.ServiceGateways.Select(y => y.Gateway));
                hasServiceContainer = true;
            }

            //QUE TENGA IDAPP EL O SU PADRE
            var idApp = hasServiceContainer ? serviceContainer.UrlName : service.UrlName;
            if (string.IsNullOrEmpty(idApp))
            {
                NLogLogger.LogEvent(NLogType.Error, "ServiceServiceValidator - ValidateVisaNetOnService - IdApp vacío. ServiceId: " + (hasServiceContainer ? serviceContainer.Id : service.Id));
                msg += "IdApp vacío. ";
                valid = false;
            }

            //QUE TENGA VISANETON ACTIVADO
            var allowsVon = hasServiceContainer ? serviceContainer.AllowVon : service.AllowVon;
            if (!allowsVon)
            {
                NLogLogger.LogEvent(NLogType.Error, "ServiceServiceValidator - ValidateVisaNetOnService - No tiene habilitado VisaNetOn. ServiceId: " + (hasServiceContainer ? serviceContainer.Id : service.Id));
                msg += "No tiene habilitado VisaNetOn. ";
                valid = false;
            }

            //QUE TENGA URL_TRANSACCION
            var urlTransaction = hasServiceContainer ? serviceContainer.ExternalUrlAdd : service.ExternalUrlAdd;
            if (string.IsNullOrEmpty(urlTransaction))
            {
                NLogLogger.LogEvent(NLogType.Error, "ServiceServiceValidator - ValidateVisaNetOnService - No tiene configurado URL Transacción. ServiceId: " + (hasServiceContainer ? serviceContainer.Id : service.Id));
                msg += "No tiene configurado url transacción. ";
                valid = false;
            }

            //QUE TENGA THUMBPRINT DE VISA
            var thumbprintVisa = hasServiceContainer ? serviceContainer.CertificateThumbprintVisa : service.CertificateThumbprintVisa;
            if (string.IsNullOrEmpty(thumbprintVisa))
            {
                NLogLogger.LogEvent(NLogType.Error, "ServiceServiceValidator - ValidateVisaNetOnService - No tiene configurado el Certificado de Visa. ServiceId: " + (hasServiceContainer ? serviceContainer.Id : service.Id));
                msg += "No tiene configurado el certificado de Visa. ";
                valid = false;
            }

            //QUE TENGA THUMBPRINT EXTERNO
            var thumbprintExternal = hasServiceContainer ? serviceContainer.CertificateThumbprintExternal : service.CertificateThumbprintExternal;
            if (string.IsNullOrEmpty(thumbprintExternal))
            {
                NLogLogger.LogEvent(NLogType.Error, "ServiceServiceValidator - ValidateVisaNetOnService - No tiene configurado el Certificado externo. ServiceId: " + (hasServiceContainer ? serviceContainer.Id : service.Id));
                msg += "No tiene configurado el certificado externo. ";
                valid = false;
            }

            //QUE SEA VERSION INTEGRACION v4 EN ADELANTE
            var integrationVersion = hasServiceContainer ? (int)serviceContainer.UrlIntegrationVersion : (int)service.UrlIntegrationVersion;
            if (integrationVersion < (int)UrlIntegrationVersionEnumDto.FourthVersion)
            {
                NLogLogger.LogEvent(NLogType.Error, "ServiceServiceValidator - ValidateVisaNetOnService - No tiene la versión de integración adecuada. ServiceId: " + (hasServiceContainer ? serviceContainer.Id : service.Id));
                msg += "No tiene la versión de integración adecuada. ";
                valid = false;
            }

            //MANDAR MAIL A VISA
            if (!valid)
            {
                SendVisaInternalErrorEmail(service.Name, " para VisaNetOn", msg);
            }

            return valid;
        }


        //Debito
        public bool ValidateDebitService(Guid serviceId)
        {
            var service = _serviceService.GetById(serviceId, x => x.ServiceContainer, x => x.ServiceGateways, x => x.ServiceGateways.Select(y => y.Gateway));
            return ValidateDebitService(service);
        }

        public bool ValidateDebitService(ServiceDto service)
        {
            throw new NotImplementedException();
        }


        //Validar pasarela
        public bool ValidateServiceGatewayActive(Guid serviceId, GatewayEnumDto gatewayEnum)
        {
            var service = _serviceService.GetById(serviceId, x => x.ServiceGateways, x => x.ServiceGateways.Select(y => y.Gateway));
            return ValidateServiceGatewayActive(service, gatewayEnum);
        }

        public bool ValidateServiceGatewayActive(ServiceDto service, GatewayEnumDto gatewayEnum)
        {
            var valid = true;
            var msg = "";
            var serviceId = service.Id;

            //QUE TENGA LA PASARELA INDICADA ACTIVA
            var servicesGatewaysList = service.ServiceGatewaysDto.Where(x => x.Active).ToList();
            var activeGateway = servicesGatewaysList.FirstOrDefault(x => x.Gateway.Enum == (int)gatewayEnum);
            if (activeGateway == null)
            {
                NLogLogger.LogEvent(NLogType.Error, "ServiceServiceValidator - ValidateServiceGatewayActive - No tiene activa la pasarela " + gatewayEnum + ". ServiceId: " + serviceId);
                msg += "No tiene activa la pasarela " + gatewayEnum + ". ";
                valid = false;
            }

            //MANDAR MAIL A VISA
            if (!valid)
            {
                SendVisaInternalErrorEmail(service.Name, string.Empty, msg);
            }

            return valid;
        }


        //Auxiliares
        private void SendVisaInternalErrorEmail(string serviceName, string configurationContext, string msg)
        {
            var parameters = _repositoryParameters.AllNoTracking().First();
            const string title = "VISANETPAGOS - ERROR INTERNO - CRITICO";
            var emailMsg = "Error de configuración del servicio " + serviceName + configurationContext + ". Se encontraron los siguientes errores: " + msg;
            _serviceEmailMessage.SendInternalErrorNotification(parameters, title, null, emailMsg, string.Empty, string.Empty, null);
        }

    }
}