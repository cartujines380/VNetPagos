using System;
using System.Collections.Generic;
using VisaNet.Application.Interfaces.Base;
using VisaNet.Domain.Entities;
using VisaNet.Domain.EntitiesDtos;

namespace VisaNet.Application.Interfaces
{
    public interface IServiceVonData : IService<VonData, VonDataDto>
    {
        VonDataDto Find(string idApp, string idUserExternal, string idCardExternal);
        VonDataDto Find(string idApp, Guid anonymousUserId, string idCardExternal);
        IEnumerable<VonDataDto> Find(string idApp, string idUserExternal);
        IEnumerable<VonDataDto> Find(string idApp, Guid anonymousUserId);
        string GetCardPaymentToken(Guid anonymousUserId, string idCardExternal);
        string GetCardPaymentToken(string idUserExternal, string idCardExternal);
        VonDataAssociationDto GetAsAssociationDto(string idApp, string idUserExternal);
        VonDataAssociationDto GetAsAssociationDto(string idApp, string idUserExternal, string idCardExternal);
        bool DeleteCard(string appId, string userExternalId, string cardExternalId);
        bool DeleteAssociation(string appId, string userExternalId);
    }
}