using VisaNet.Application.Interfaces.Base;
using VisaNet.Domain.Entities;
using VisaNet.Domain.EntitiesDtos;

namespace VisaNet.Application.Interfaces
{
    public interface IServiceAnonymousUser : IService<AnonymousUser, AnonymousUserDto>
    {
        AnonymousUserDto GetUserByEmailIdentityNumber(string email, string identityNumber);
        AnonymousUserDto CreateOrEditAnonymousUser(AnonymousUserDto anonymousUser);
        long GetNextCyberSourceIdentifier();
    }
}