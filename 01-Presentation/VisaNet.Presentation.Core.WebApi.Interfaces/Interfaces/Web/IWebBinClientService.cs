using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Domain.EntitiesDtos;

namespace VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Web
{
    public interface IWebBinClientService
    {
        Task<BinDto> Find(int value);
        Task<ICollection<BinDto>> GetBinsFromMask(IList<int> mask);
        Task<BinDto> FindByGuid(Guid cardId);
    }
}
