using System.Collections.Generic;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Aplication.VisaNetOn.Interfaces
{
    public interface IServiceVisaNetOnIntegration
    {
        void ProcessOperation(IDictionary<string, string> formData, RedirectEnums action);
    }
}