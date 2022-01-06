using VisaNet.Domain.EntitiesDtos;

namespace VisaNet.Application.Interfaces
{
    public interface IServiceAssociationSelector
    {
        IUserDto FindUserByIdAppAndExternalId(string idApp, string userExternalId);
        IUserDto FindUserByIdAppAndExternalId(string idApp, string userExternalId, string cardExternalId);
        IAssociationInfoDto FindServiceByIdAppAndExternalId(string idApp, string userExternalId, string cardExternalId = null);
        bool DeleteAssociationCard(IAssociationInfoDto associationDto, CardDto card);
        bool DeleteAssociation(IAssociationInfoDto associationDto);
    }
}