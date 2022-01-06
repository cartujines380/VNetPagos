using VisaNet.Domain.Entities.Enums;

namespace VisaNet.Application.Interfaces
{
    public interface IServicePostSignatureFactory
    {
        IServicePostSignature GetPostSignatureVersion(UrlIntegrationVersionEnum integrationVersion);
    }
}