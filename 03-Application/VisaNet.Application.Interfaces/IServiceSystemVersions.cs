using System.Collections.Generic;

namespace VisaNet.Application.Interfaces
{
    public interface IServiceSystemVersions
    {
        IDictionary<string, string> GetSystemVersions();
    }
}
