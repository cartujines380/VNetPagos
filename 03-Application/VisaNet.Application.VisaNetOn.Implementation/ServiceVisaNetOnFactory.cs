using VisaNet.Application.Interfaces;
using VisaNet.Application.VisaNetOn.Interfaces;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Application.VisaNetOn.Implementation
{
    public class ServiceVisaNetOnFactory : IServiceVisaNetOnFactory
    {
        private readonly IServicePayment _servicePayment;
        private readonly IServiceServiceAssosiate _serviceServiceAssosiate;
        private readonly IServiceVonData _serviceVonData;
        private readonly IServiceService _serviceService;
        private readonly IServiceExternalNotification _serviceExternalNotification;
        private readonly IServiceCard _serviceCard;
        protected readonly IServiceWebhookRegistration _serviceWebhookRegistration;

        public ServiceVisaNetOnFactory(IServicePayment servicePayment, IServiceServiceAssosiate serviceServiceAssosiate,
            IServiceVonData serviceVonData, IServiceService serviceService, IServiceExternalNotification serviceExternalNotification, IServiceCard serviceCard, IServiceWebhookRegistration serviceWebhookRegistration)
        {
            _servicePayment = servicePayment;
            _serviceServiceAssosiate = serviceServiceAssosiate;
            _serviceVonData = serviceVonData;
            _serviceService = serviceService;
            _serviceExternalNotification = serviceExternalNotification;
            _serviceCard = serviceCard;
            _serviceWebhookRegistration = serviceWebhookRegistration;
        }

        public IServiceVisaNetOn GetVisaNetOnService(RedirectEnums action)
        {
            IServiceVisaNetOn service = null;
            switch (action)
            {
                case RedirectEnums.VisaNetOnTokenizationRecurrent:
                    service = new ServiceVisaNetOnTokenizationRecurrent(_servicePayment, _serviceServiceAssosiate, _serviceVonData, _serviceService, _serviceExternalNotification, _serviceWebhookRegistration);
                    break;
                case RedirectEnums.VisaNetOnTokenizationRegistered:
                    service = new ServiceVisaNetOnTokenizationRegistered(_servicePayment, _serviceServiceAssosiate, _serviceVonData, _serviceService, _serviceExternalNotification, _serviceWebhookRegistration);
                    break;
                case RedirectEnums.VisaNetOnTokenizationNewUser:
                    service = new ServiceVisaNetOnTokenizationNewUser(_servicePayment, _serviceServiceAssosiate, _serviceVonData, _serviceService, _serviceExternalNotification, _serviceWebhookRegistration);
                    break;
                case RedirectEnums.VisaNetOnPaymentAnonymous:
                    service = new ServiceVisaNetOnPaymentAnonymous(_servicePayment, _serviceServiceAssosiate, _serviceVonData, _serviceService, _serviceExternalNotification, _serviceWebhookRegistration);
                    break;
                case RedirectEnums.VisaNetOnPaymentNewUser:
                    service = new ServiceVisaNetOnPaymentNewUser(_servicePayment, _serviceServiceAssosiate, _serviceVonData, _serviceService, _serviceExternalNotification, _serviceWebhookRegistration);
                    break;
                case RedirectEnums.VisaNetOnPaymentRegisteredNewToken:
                    service = new ServiceVisaNetOnPaymentRegisteredNewToken(_servicePayment, _serviceServiceAssosiate, _serviceVonData, _serviceService, _serviceExternalNotification, _serviceWebhookRegistration);
                    break;
                case RedirectEnums.VisaNetOnPaymentRegisteredWithToken:
                    service = new ServiceVisaNetOnPaymentRegisteredWithToken(_servicePayment, _serviceServiceAssosiate, _serviceVonData, _serviceService, _serviceExternalNotification, _serviceCard, _serviceWebhookRegistration);
                    break;
                case RedirectEnums.VisaNetOnPaymentRecurrentNewToken:
                    service = new ServiceVisaNetOnPaymentRecurrentNewToken(_servicePayment, _serviceServiceAssosiate, _serviceVonData, _serviceService, _serviceExternalNotification, _serviceWebhookRegistration);
                    break;
                case RedirectEnums.VisaNetOnPaymentRecurrentWithToken:
                    service = new ServiceVisaNetOnPaymentRecurrentWithToken(_servicePayment, _serviceServiceAssosiate, _serviceVonData, _serviceService, _serviceExternalNotification, _serviceWebhookRegistration);
                    break;
            }
            return service;
        }

    }
}