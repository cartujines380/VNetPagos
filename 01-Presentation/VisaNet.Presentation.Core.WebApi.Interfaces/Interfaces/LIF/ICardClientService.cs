using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Lif.Domain.EntitesDtos;

namespace VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.LIF
{
    public interface ICardClientService
    {
        Task<BinDto> GetCardInfo(string bin, bool includeIssuingCompany);
        Task<ICollection<BinDto>> GetNationalBins(bool includeIssuingCompany);
    }
}