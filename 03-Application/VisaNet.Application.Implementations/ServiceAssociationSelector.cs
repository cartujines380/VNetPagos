using System.Linq;
using VisaNet.Application.Interfaces;
using VisaNet.Domain.EntitiesDtos;

namespace VisaNet.Application.Implementations
{
    public class ServiceAssociationSelector : IServiceAssociationSelector
    {
        private readonly IServiceServiceAssosiate _serviceServiceAssosiate;
        private readonly IServiceVonData _serviceVonData;

        public ServiceAssociationSelector(IServiceServiceAssosiate serviceServiceAssosiate, IServiceVonData serviceVonData)
        {
            _serviceServiceAssosiate = serviceServiceAssosiate;
            _serviceVonData = serviceVonData;
        }

        public IUserDto FindUserByIdAppAndExternalId(string idApp, string userExternalId)
        {
            IUserDto user = null;

            user = _serviceVonData.Find(idApp, userExternalId).Select(x => x.AnonymousUserDto).FirstOrDefault();
            if (user == null)
            {
                user = _serviceServiceAssosiate.GetServiceAssociatedDtoFromIdUserExternal(idApp, userExternalId).RegisteredUserDto;
            }

            return user;
        }

        public IUserDto FindUserByIdAppAndExternalId(string idApp, string userExternalId, string cardExternalId)
        {
            IUserDto user = null;

            user = _serviceVonData.Find(idApp, userExternalId, cardExternalId).AnonymousUserDto;
            if (user == null)
            {
                user = _serviceServiceAssosiate.GetServiceAssociatedDtoFromIdUserExternal(idApp, userExternalId).RegisteredUserDto;
            }

            return user;
        }

        public IAssociationInfoDto FindServiceByIdAppAndExternalId(string idApp, string userExternalId, string cardExternalId = null)
        {
            //Primero se busca como usuario recurrente (tabla VonData)
            IAssociationInfoDto associatedDto = null;
            if (!string.IsNullOrEmpty(cardExternalId))
            {
                associatedDto = _serviceVonData.GetAsAssociationDto(idApp, userExternalId, cardExternalId);
            }
            else
            {
                associatedDto = _serviceVonData.GetAsAssociationDto(idApp, userExternalId);
            }

            if (associatedDto == null)
            {
                //Si no se encuentra, se busca como servicio asociado de usuario registrado
                associatedDto = _serviceServiceAssosiate.GetServiceAssociatedDtoFromIdUserExternal(userExternalId, idApp);
            }
            return associatedDto;
        }

        public bool DeleteAssociationCard(IAssociationInfoDto associationDto, CardDto card)
        {
            var result = false;
            if (associationDto.GetType() == typeof(ServiceAssociatedDto))
            {
                result = _serviceServiceAssosiate.DeleteCardFromService(associationDto.Id, card.Id, associationDto.UserId, false);
            }
            else
            {
                result = _serviceVonData.DeleteCard(associationDto.ServiceDto.UrlName, associationDto.IdUserExternal.ToString(), card.ExternalId.ToString());
            }
            return result;
        }

        public bool DeleteAssociation(IAssociationInfoDto associationDto)
        {
            var result = false;
            if (associationDto.GetType() == typeof(ServiceAssociatedDto))
            {
                result = _serviceServiceAssosiate.DeleteService(associationDto.Id, false);
            }
            else
            {
                result = _serviceVonData.DeleteAssociation(associationDto.ServiceDto.UrlName, associationDto.IdUserExternal.ToString());
            }
            return result;
        }

    }
}