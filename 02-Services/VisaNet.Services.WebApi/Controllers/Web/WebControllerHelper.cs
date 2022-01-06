using VisaNet.Domain.Entities;
using VisaNet.Domain.EntitiesDtos;

namespace VisaNet.Services.WebApi.Controllers.Web
{
    public static class WebControllerHelper
    {
        public static void LoadServiceReferenceParams(ServiceDto serviceDto)
        {
            //Si el servicio hijo pide referencias y no tiene, se usan las del contenedor
            if (serviceDto != null
                && serviceDto.AskUserForReferences
                && string.IsNullOrEmpty(serviceDto.ReferenceParamName)
                && serviceDto.ServiceContainerDto != null
                && !string.IsNullOrEmpty(serviceDto.ServiceContainerDto.ReferenceParamName))
            {
                serviceDto.ReferenceParamName = serviceDto.ServiceContainerDto.ReferenceParamName;
                serviceDto.ReferenceParamName2 = serviceDto.ServiceContainerDto.ReferenceParamName2;
                serviceDto.ReferenceParamName3 = serviceDto.ServiceContainerDto.ReferenceParamName3;
                serviceDto.ReferenceParamName4 = serviceDto.ServiceContainerDto.ReferenceParamName4;
                serviceDto.ReferenceParamName5 = serviceDto.ServiceContainerDto.ReferenceParamName5;
                serviceDto.ReferenceParamName6 = serviceDto.ServiceContainerDto.ReferenceParamName6;
                serviceDto.ReferenceParamRegex = serviceDto.ServiceContainerDto.ReferenceParamRegex;
                serviceDto.ReferenceParamRegex2 = serviceDto.ServiceContainerDto.ReferenceParamRegex2;
                serviceDto.ReferenceParamRegex3 = serviceDto.ServiceContainerDto.ReferenceParamRegex3;
                serviceDto.ReferenceParamRegex4 = serviceDto.ServiceContainerDto.ReferenceParamRegex4;
                serviceDto.ReferenceParamRegex5 = serviceDto.ServiceContainerDto.ReferenceParamRegex5;
                serviceDto.ReferenceParamRegex6 = serviceDto.ServiceContainerDto.ReferenceParamRegex6;
            }
        }

        public static void LoadServiceReferenceParams(Service service)
        {
            //Si el servicio hijo pide referencias y no tiene, se usan las del contenedor
            if (service != null
                && service.AskUserForReferences
                && string.IsNullOrEmpty(service.ReferenceParamName)
                && service.ServiceContainer != null
                && !string.IsNullOrEmpty(service.ServiceContainer.ReferenceParamName))
            {
                service.ReferenceParamName = service.ServiceContainer.ReferenceParamName;
                service.ReferenceParamName2 = service.ServiceContainer.ReferenceParamName2;
                service.ReferenceParamName3 = service.ServiceContainer.ReferenceParamName3;
                service.ReferenceParamName4 = service.ServiceContainer.ReferenceParamName4;
                service.ReferenceParamName5 = service.ServiceContainer.ReferenceParamName5;
                service.ReferenceParamName6 = service.ServiceContainer.ReferenceParamName6;
                service.ReferenceParamRegex = service.ServiceContainer.ReferenceParamRegex;
                service.ReferenceParamRegex2 = service.ServiceContainer.ReferenceParamRegex2;
                service.ReferenceParamRegex3 = service.ServiceContainer.ReferenceParamRegex3;
                service.ReferenceParamRegex4 = service.ServiceContainer.ReferenceParamRegex4;
                service.ReferenceParamRegex5 = service.ServiceContainer.ReferenceParamRegex5;
                service.ReferenceParamRegex6 = service.ServiceContainer.ReferenceParamRegex6;
            }
        }

    }
}