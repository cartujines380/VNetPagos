using System;
using System.Linq;
using VisaNet.Application.Implementations.Base;
using VisaNet.Application.Interfaces;
using VisaNet.Common.Exceptions;
using VisaNet.Domain.Entities;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Repository.Interfaces.Interfaces;

namespace VisaNet.Application.Implementations
{
    public class ServiceAnonymousUser : BaseService<AnonymousUser, AnonymousUserDto>, IServiceAnonymousUser
    {
        private readonly IRepositorySistarbancUser _repositorySistarbancUser;

        public ServiceAnonymousUser(IRepositoryAnonymousUser repository, IRepositorySistarbancUser repositorySistarbancUser)
            : base(repository)
        {
            _repositorySistarbancUser = repositorySistarbancUser;
        }

        public override IQueryable<AnonymousUser> GetDataForTable()
        {
            return Repository.AllNoTracking();
        }

        public override AnonymousUserDto Converter(AnonymousUser entity)
        {
            if (entity == null) return null;

            return new AnonymousUserDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Surname = entity.Surname,
                Email = entity.Email,
                Address = entity.Address,
                PhoneNumber = entity.PhoneNumber,
                MobileNumber = entity.MobileNumber,
                IdentityNumber = entity.IdentityNumber,
                SistarbancUserId = entity.SistarbancUserId,
                SistarbancUser = entity.SistarbancUser != null ? new SistarbancUserDto()
                                                                 {
                                                                     Active = entity.SistarbancUser.Active,
                                                                     UniqueIdentifier = entity.SistarbancUser.UniqueIdentifier,
                                                                     Id = entity.SistarbancUser.Id
                                                                 } : null,
                SistarbancBrouUserId = entity.SistarbancBrouUserId,
                SistarbancBrouUser = entity.SistarbancBrouUser != null ? new SistarbancUserDto()
                {
                    Active = entity.SistarbancBrouUser.Active,
                    UniqueIdentifier = entity.SistarbancBrouUser.UniqueIdentifier,
                    Id = entity.SistarbancBrouUser.Id
                } : null,
                CyberSourceIdentifier = entity.CyberSourceIdentifier,
                IsPortalUser = entity.IsPortalUser,
                IsVonUser = entity.IsVonUser,
            };
        }

        public override AnonymousUser Converter(AnonymousUserDto entity)
        {
            if (entity == null) return null;

            return new AnonymousUser
             {
                 Id = entity.Id,
                 Name = entity.Name,
                 Surname = entity.Surname,
                 Email = entity.Email,
                 Address = entity.Address,
                 PhoneNumber = entity.PhoneNumber,
                 MobileNumber = entity.MobileNumber,
                 IdentityNumber = entity.IdentityNumber,
                 CyberSourceIdentifier = entity.CyberSourceIdentifier,
                 SistarbancBrouUserId = entity.SistarbancBrouUserId,
                 SistarbancUserId = entity.SistarbancUserId,

                 SistarbancUser = entity.SistarbancUser != null ? new SistarbancUser()
                 {
                     Active = entity.SistarbancUser.Active,
                     UniqueIdentifier = entity.SistarbancUser.UniqueIdentifier,
                     Id = entity.SistarbancUser.Id
                 } : null,
                 SistarbancBrouUser = entity.SistarbancBrouUser != null ? new SistarbancUser()
                 {
                     Active = entity.SistarbancBrouUser.Active,
                     UniqueIdentifier = entity.SistarbancBrouUser.UniqueIdentifier,
                     Id = entity.SistarbancBrouUser.Id
                 } : null,
                 IsPortalUser = entity.IsPortalUser,
                 IsVonUser = entity.IsVonUser,
             };
        }

        public override void Edit(AnonymousUserDto entity)
        {
            var efEntity = Repository.All(u => u.Id == entity.Id).FirstOrDefault();

            if (efEntity == null)
                throw new FatalException(CodeExceptions.USER_NOT_EXIST);

            efEntity.Name = entity.Name;
            efEntity.Surname = entity.Surname;
            efEntity.Email = entity.Email;
            efEntity.PhoneNumber = entity.PhoneNumber;
            efEntity.Address = entity.Address;

            if (!efEntity.IsPortalUser)
            {
                efEntity.IsPortalUser = entity.IsPortalUser;
            }
            if (!efEntity.IsVonUser)
            {
                efEntity.IsVonUser = entity.IsVonUser;
            }

            Repository.Edit(efEntity);
            Repository.Save();
        }

        public AnonymousUserDto GetUserByEmailIdentityNumber(string email, string identityNumber)
        {
            var user = Repository.AllNoTracking(u => (!String.IsNullOrEmpty(email) && u.Email.Equals(email)) || (!String.IsNullOrEmpty(identityNumber) && u.IdentityNumber.Equals(identityNumber)), u => u.SistarbancUser).FirstOrDefault();
            if (user == null) return null;

            return new AnonymousUserDto()
                   {
                       Address = user.Address,
                       Id = user.Id,
                       Email = user.Email,
                       Name = user.Name,
                       PhoneNumber = user.PhoneNumber,
                       Surname = user.Surname,
                       SistarbancUserId = user.SistarbancUserId,
                       IdentityNumber = user.IdentityNumber,
                       MobileNumber = user.MobileNumber,
                       SistarbancUser = user.SistarbancUser == null
                           ? null
                           : new SistarbancUserDto()
                             {
                                 Active = user.SistarbancUser.Active,
                                 UniqueIdentifier = user.SistarbancUser.UniqueIdentifier,
                                 Id = user.SistarbancUser.Id
                             },
                       CyberSourceIdentifier = user.CyberSourceIdentifier,
                       IsPortalUser = user.IsPortalUser,
                       IsVonUser = user.IsVonUser
                   };
        }

        public override AnonymousUserDto Create(AnonymousUserDto entity, bool returnEntity = false)
        {
            Repository.ContextTrackChanges = true;
            if (
                Repository.AllNoTracking(u => u.Email.Equals(entity.Email, StringComparison.InvariantCultureIgnoreCase))
                    .Any())
                throw new BusinessException(CodeExceptions.ANONYMOUS_USER_EMAIL_DUPLICATED);

            if (!String.IsNullOrEmpty(entity.IdentityNumber) &&
                Repository.AllNoTracking(u => u.IdentityNumber.Equals(entity.IdentityNumber, StringComparison.InvariantCultureIgnoreCase))
                    .Any())
                throw new BusinessException(CodeExceptions.ANONYMOUS_USER_CI_DUPLICATED);

            //OBTENGO EL SIGUIENTE CyberSourceIdentifier
            entity.CyberSourceIdentifier = ((IRepositoryAnonymousUser)Repository).GetNextCyberSourceIdentifier();

            entity.Email = entity.Email.Trim();
            entity.SistarbancUser = new SistarbancUserDto();
            entity.SistarbancBrouUser = new SistarbancUserDto();

            var user = Converter(entity);
            user.GenerateNewIdentity();

            user.SistarbancUser.GenerateNewIdentity();
            user.SistarbancUserId = user.SistarbancUser.Id;

            _repositorySistarbancUser.Create(user.SistarbancUser);

            user.SistarbancBrouUser.GenerateNewIdentity();
            user.SistarbancBrouUserId = user.SistarbancBrouUser.Id;

            _repositorySistarbancUser.Create(user.SistarbancBrouUser);

            Repository.Create(user);
            Repository.Save();

            Repository.ContextTrackChanges = false;

            return returnEntity ? GetUserByEmailIdentityNumber(entity.Email, entity.IdentityNumber) : null;
        }

        public AnonymousUserDto CreateOrEditAnonymousUser(AnonymousUserDto anonymousUser)
        {
            var userDto = GetUserByEmailIdentityNumber(anonymousUser.Email, null);
            if (userDto == null)
            {
                return Create(anonymousUser, true);
            }

            if ((userDto.Address == null || !userDto.Address.Equals(anonymousUser.Address)) ||
                (userDto.Name == null || !userDto.Name.Equals(anonymousUser.Name)) ||
                (userDto.Surname == null || !userDto.Surname.Equals(anonymousUser.Surname)) ||
                (!anonymousUser.IsVonUser && userDto.IsVonUser) ||
                (!anonymousUser.IsPortalUser && userDto.IsPortalUser))
            {
                userDto.Address = anonymousUser.Address;
                userDto.Name = anonymousUser.Name;
                userDto.Surname = anonymousUser.Surname;
                userDto.IsVonUser = anonymousUser.IsVonUser;
                userDto.IsPortalUser = anonymousUser.IsPortalUser;

                Edit(userDto);
            }
            return userDto;
        }

        public long GetNextCyberSourceIdentifier()
        {
            return ((IRepositoryAnonymousUser)Repository).GetNextCyberSourceIdentifier();
        }

    }
}