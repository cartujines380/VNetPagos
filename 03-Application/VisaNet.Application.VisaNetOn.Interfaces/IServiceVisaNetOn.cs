using System.Collections.Generic;
using VisaNet.Domain.EntitiesDtos;

namespace VisaNet.Application.VisaNetOn.Interfaces
{
    public interface IServiceVisaNetOn
    {
        ResultDto ProcessOperation(IDictionary<string, string> formData);
    }
}