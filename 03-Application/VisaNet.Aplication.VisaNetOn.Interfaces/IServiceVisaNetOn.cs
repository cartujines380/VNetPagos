using System.Collections.Generic;

namespace VisaNet.Aplication.VisaNetOn.Interfaces
{
    public interface IServiceVisaNetOn
    {
        void ProcessOperation(IDictionary<string, string> formData);
    }
}