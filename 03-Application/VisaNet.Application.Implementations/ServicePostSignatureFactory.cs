using VisaNet.Application.Interfaces;
using VisaNet.Domain.Entities.Enums;

namespace VisaNet.Application.Implementations
{
    public class ServicePostSignatureFactory : IServicePostSignatureFactory
    {
        public IServicePostSignature GetPostSignatureVersion(UrlIntegrationVersionEnum integrationVersion)
        {
            IServicePostSignature servicePostSignatureVersion = null;
            switch (integrationVersion)
            {
                case UrlIntegrationVersionEnum.FirstVersion:
                case UrlIntegrationVersionEnum.SecondVersion:
                    servicePostSignatureVersion = new ServicePostSignatureFirstAndSecondVersion();
                    break;
                case UrlIntegrationVersionEnum.ThirdVersion:
                    servicePostSignatureVersion = new ServicePostSignatureThirdVersion();
                    break;
                case UrlIntegrationVersionEnum.FourthVersion:
                    servicePostSignatureVersion = new ServicePostSignatureFourthVersion();
                    break;
                case UrlIntegrationVersionEnum.FifthVersion:
                    servicePostSignatureVersion = new ServicePostSignatureFifthVersion();
                    break;
            }
            return servicePostSignatureVersion;
        }

    }
}