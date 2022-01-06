using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Domain.EntitiesDtos;

namespace VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration
{
    public interface IParametersClientService
    {
        Task<ICollection<ParametersDto>> FindAll();
        Task<ParametersDto> Find(Guid id);
        Task Edit(ParametersDto parameters);
    }
}
