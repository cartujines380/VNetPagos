using System.Collections.Generic;
using VisaNet.Aplication.VisaNetOn.Interfaces;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Aplication.VisaNetOn.Implementation
{
    public class ServiceVisaNetOnIntegration : IServiceVisaNetOnIntegration
    {
        private readonly IServiceVisaNetOnFactory _serviceVisaNetOnFactory;

        public ServiceVisaNetOnIntegration(IServiceVisaNetOnFactory serviceVisaNetOnFactory)
        {
            _serviceVisaNetOnFactory = serviceVisaNetOnFactory;
        }

        public void ProcessOperation(IDictionary<string, string> formData, RedirectEnums action)
        {
            var serviceVisaNetOn = _serviceVisaNetOnFactory.GetVisaNetOnService(action);
            serviceVisaNetOn.ProcessOperation(formData);
        }

    }
}