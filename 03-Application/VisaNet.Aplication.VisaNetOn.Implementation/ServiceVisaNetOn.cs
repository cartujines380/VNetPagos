using System.Collections.Generic;
using VisaNet.Aplication.VisaNetOn.Interfaces;

namespace VisaNet.Application.VisaNetOn.Implementation
{
    public abstract class ServiceVisaNetOn : IServiceVisaNetOn
    {
        public abstract void ProcessOperation(IDictionary<string, string> formData);
    }
}