using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Utilities.Cybersource;

namespace VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Web
{
    public interface IWebCyberSourceAccessClientService
    {
        Task<IDictionary<string, string>> GenerateKeys(IGenerateToken item);
    }
}
