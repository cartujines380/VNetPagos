using System.Threading.Tasks;
using VisaNet.Domain.EntitiesDtos;

namespace VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration
{
    public interface ICyberSourceAcknowledgementClientService
    {
        Task Process(CyberSourceAcknowledgementDto post);
    }
}