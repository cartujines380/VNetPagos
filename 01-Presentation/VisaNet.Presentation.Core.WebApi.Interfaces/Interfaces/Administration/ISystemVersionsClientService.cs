using System.Collections.Generic;
using System.Threading.Tasks;

namespace VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration
{
    public interface ISystemVersionsClientService
    {
        Task<IDictionary<string, string>> GetSystemVersions();
    }
}
