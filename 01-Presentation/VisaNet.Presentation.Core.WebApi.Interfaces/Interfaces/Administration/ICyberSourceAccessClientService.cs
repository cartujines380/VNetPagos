using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Utilities.Cybersource;

namespace VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration
{
    public interface ICyberSourceAccessClientService
    {
        Task<IDictionary<string, string>> GenerateKeys(IGenerateToken item);

        Task<string> GetCardNumberByToken(CybersourceGetCardNameDto dto);
    }
}