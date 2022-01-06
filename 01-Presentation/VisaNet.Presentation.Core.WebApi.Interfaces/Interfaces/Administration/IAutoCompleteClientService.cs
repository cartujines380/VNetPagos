using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Domain.EntitiesDtos;

namespace VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration
{
    public interface IAutoCompleteClientService
    {
        Task<ICollection<ServiceDto>> AutoCompleteServices(string contains);
        Task<ICollection<ApplicationUserDto>> AutoCompleteApplicationUsers(string contains);
    }
}
