using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Application.VisaNetOn.Interfaces
{
    public interface IServiceVisaNetOnFactory
    {
        IServiceVisaNetOn GetVisaNetOnService(RedirectEnums action);
    }
}
