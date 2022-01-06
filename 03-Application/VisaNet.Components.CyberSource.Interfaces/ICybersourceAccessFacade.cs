using System.Collections.Generic;
using VisaNet.Utilities.Cybersource;

namespace VisaNet.Components.CyberSource.Interfaces
{
    public interface ICybersourceAccessFacade
    {
        IDictionary<string, string> GenerateKeys(IGenerateToken token);
    }
}