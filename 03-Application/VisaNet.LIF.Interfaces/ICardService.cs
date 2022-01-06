using System.Collections.Generic;
using VisaNet.Lif.Domain.EntitesDtos;

namespace VisaNet.LIF.Interfaces
{
    public interface ICardService
    {
        BinDto GetCardInfo(string bin, bool includeIssuingCompany);
        ICollection<BinDto> GetNationalBins(bool includeIssuingCompany);
    }
}
