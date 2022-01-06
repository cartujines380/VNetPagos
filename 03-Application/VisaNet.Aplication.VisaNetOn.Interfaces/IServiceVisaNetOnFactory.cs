using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Aplication.VisaNetOn.Interfaces
{
    public interface IServiceVisaNetOnFactory
    {
        IServiceVisaNetOn GetVisaNetOnService(RedirectEnums action);
    }
}
