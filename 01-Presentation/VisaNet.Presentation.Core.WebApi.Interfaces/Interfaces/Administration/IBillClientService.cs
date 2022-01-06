using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Domain.EntitiesDtos;

namespace VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration
{
    public interface IBillClientService
    {
        Task<ICollection<BillDto>> TestGatewayGetBills(TestGatewaysFilterDto filterDto);
    }
}
