using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Domain.EntitiesDtos;

namespace VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration
{
    public interface IDiscountClientService
    {
        Task<ICollection<DiscountDto>> FindAll();
        Task<DiscountDto> Find(Guid id);
        Task Edit(DiscountDto discount);
    }
}
