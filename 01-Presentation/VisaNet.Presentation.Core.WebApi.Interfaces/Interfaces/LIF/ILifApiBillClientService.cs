using System.Threading.Tasks;
using VisaNet.Domain.EntitiesDtos;

namespace VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.LIF
{
    public interface ILifApiBillClientService
    {
        Task Create(LifApiBillDto lifApiBill);
    }
}