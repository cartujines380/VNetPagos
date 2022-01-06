using VisaNet.Aplication.VisaNetOn.Interfaces;
using VisaNet.Application.Interfaces;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Aplication.VisaNetOn.Implementation
{
    public class ServiceVisaNetOnFactory : IServiceVisaNetOnFactory
    {
        private readonly IServicePayment _servicePayment;

        public ServiceVisaNetOnFactory(IServicePayment servicePayment)
        {
            _servicePayment = servicePayment;
        }

        public IServiceVisaNetOn GetVisaNetOnService(RedirectEnums action)
        {
            IServiceVisaNetOn service = null;
            switch (action)
            {
                case RedirectEnums.VisaNetOnTokenization:
                    service = new ServiceVisaNetOnTokenization();
                    break;
                case RedirectEnums.VisaNetOnTokenizationNewToken:
                    service = new ServiceVisaNetOnTokenizationNewToken();
                    break;
                case RedirectEnums.VisaNetOnTokenizationNewUser:
                    service = new ServiceVisaNetOnTokenizationNewUser();
                    break;
                case RedirectEnums.VisaNetOnPaymentAnonymous:
                    service = new ServiceVisaNetOnPaymentAnonymous(_servicePayment);
                    break;
                case RedirectEnums.VisaNetOnPaymentRegisteredNewToken:
                    service = new ServiceVisaNetOnPaymentRegisteredNewToken();
                    break;
                case RedirectEnums.VisaNetOnPaymentRegisteredWithToken:
                    service = new ServiceVisaNetOnPaymentRegisteredWithToken();
                    break;
            }
            return service;
        }

    }
}