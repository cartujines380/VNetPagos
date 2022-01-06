using System;
using System.Collections.Generic;
using System.Linq;
using VisaNet.Application.Implementations.Base;
using VisaNet.Application.Interfaces;
using VisaNet.Common.Exceptions;
using VisaNet.Domain.Entities;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Repository.Interfaces.Interfaces;

namespace VisaNet.Application.Implementations
{
    public class ServiceVonData : BaseService<VonData, VonDataDto>, IServiceVonData
    {
        private readonly IServiceService _serviceService;

        public ServiceVonData(IRepositoryVonData repository, IServiceService serviceService)
            : base(repository)
        {
            _serviceService = serviceService;
        }

        public override IQueryable<VonData> GetDataForTable()
        {
            return Repository.AllNoTracking();
        }

        public override VonDataDto Converter(VonData entity)
        {
            if (entity == null) return null;

            return new VonDataDto
            {
                Id = entity.Id,
                AppId = entity.AppId,
                AnonymousUserId = entity.AnonymousUserId,
                AnonymousUserDto = entity.AnonymousUser != null ?
                    new AnonymousUserDto
                    {
                        Id = entity.AnonymousUser.Id,
                        Email = entity.AnonymousUser.Email,
                        Name = entity.AnonymousUser.Name,
                        Surname = entity.AnonymousUser.Surname,
                        Address = entity.AnonymousUser.Address,
                        IdentityNumber = entity.AnonymousUser.IdentityNumber,
                        CyberSourceIdentifier = entity.AnonymousUser.CyberSourceIdentifier,
                        IsPortalUser = entity.AnonymousUser.IsPortalUser,
                        IsVonUser = entity.AnonymousUser.IsVonUser,
                    } : null,
                UserExternalId = entity.UserExternalId,
                CardExternalId = entity.CardExternalId,
                CardName = entity.CardName,
                CardMaskedNumber = entity.CardMaskedNumber,
                CardToken = entity.CardToken,
                CardDueDate = entity.CardDueDate,
                ReferenceNumber = entity.ReferenceNumber,
                ReferenceNumber2 = entity.ReferenceNumber2,
                ReferenceNumber3 = entity.ReferenceNumber3,
                ReferenceNumber4 = entity.ReferenceNumber4,
                ReferenceNumber5 = entity.ReferenceNumber5,
                ReferenceNumber6 = entity.ReferenceNumber6,
                CreationDate = entity.CreationDate
            };
        }

        public override VonData Converter(VonDataDto entity)
        {
            if (entity == null) return null;

            return new VonData
            {
                Id = entity.Id,
                AppId = entity.AppId,
                AnonymousUserId = entity.AnonymousUserId,
                AnonymousUser = entity.AnonymousUserDto != null ?
                    new AnonymousUser
                    {
                        Id = entity.AnonymousUserDto.Id,
                        Email = entity.AnonymousUserDto.Email,
                        Name = entity.AnonymousUserDto.Name,
                        Surname = entity.AnonymousUserDto.Surname,
                        Address = entity.AnonymousUserDto.Address,
                        IdentityNumber = entity.AnonymousUserDto.IdentityNumber,
                        CyberSourceIdentifier = entity.AnonymousUserDto.CyberSourceIdentifier,
                        IsPortalUser = entity.AnonymousUserDto.IsPortalUser,
                        IsVonUser = entity.AnonymousUserDto.IsVonUser,
                    } : null,
                UserExternalId = entity.UserExternalId,
                CardExternalId = entity.CardExternalId,
                CardName = entity.CardName,
                CardMaskedNumber = entity.CardMaskedNumber,
                CardToken = entity.CardToken,
                CardDueDate = entity.CardDueDate,
                ReferenceNumber = entity.ReferenceNumber,
                ReferenceNumber2 = entity.ReferenceNumber2,
                ReferenceNumber3 = entity.ReferenceNumber3,
                ReferenceNumber4 = entity.ReferenceNumber4,
                ReferenceNumber5 = entity.ReferenceNumber5,
                ReferenceNumber6 = entity.ReferenceNumber6,
                CreationDate = entity.CreationDate
            };
        }

        public override void Edit(VonDataDto entity)
        {
            //TODO: ver para que se va a usar el Edit
            var entityDb = Repository.GetById(entity.Id);

            entityDb.CardExternalId = entity.CardExternalId;
            entityDb.CardToken = entity.CardToken;

            Repository.Edit(entityDb);
            Repository.Save();
        }

        public VonDataDto Find(string idApp, string idUserExternal, string idCardExternal)
        {
            var entity = Repository.All(x =>
                    x.AppId == idApp &&
                    x.UserExternalId == idUserExternal &&
                    x.CardExternalId == idCardExternal,
                t => t.AnonymousUser).FirstOrDefault();

            return Converter(entity);
        }

        public VonDataDto Find(string idApp, Guid anonymousUserId, string idCardExternal)
        {
            var entity = Repository.All(x =>
                    x.AppId == idApp &&
                    x.AnonymousUserId == anonymousUserId &&
                    x.CardExternalId == idCardExternal,
                t => t.AnonymousUser).FirstOrDefault();

            return Converter(entity);
        }

        public IEnumerable<VonDataDto> Find(string idApp, string idUserExternal)
        {
            var result = new List<VonDataDto>();

            var data = Repository.All(x =>
                    x.AppId == idApp &&
                    x.UserExternalId == idUserExternal,
                t => t.AnonymousUser);

            if (data != null)
            {
                data = data.OrderByDescending(x => x.CreationDate);
                result = data.Select(Converter).ToList();
            }

            return result;
        }

        public IEnumerable<VonDataDto> Find(string idApp, Guid anonymousUserId)
        {
            var result = new List<VonDataDto>();

            var data = Repository.All(x =>
                    x.AppId == idApp &&
                    x.AnonymousUserId == anonymousUserId,
                t => t.AnonymousUser);

            if (data != null)
            {
                data = data.OrderByDescending(x => x.CreationDate);
                result = data.Select(Converter).ToList();
            }

            return result;
        }

        public string GetCardPaymentToken(Guid anonymousUserId, string idCardExternal)
        {
            var entity = Repository.All(x =>
                                x.AnonymousUserId == anonymousUserId &&
                                x.CardExternalId == idCardExternal).FirstOrDefault();

            return entity != null ? entity.CardToken : null;
        }

        public string GetCardPaymentToken(string idUserExternal, string idCardExternal)
        {
            var entity = Repository.All(x =>
                                x.UserExternalId == idUserExternal &&
                                x.CardExternalId == idCardExternal).FirstOrDefault();

            return entity != null ? entity.CardToken : null;
        }

        public VonDataAssociationDto GetAsAssociationDto(string idApp, string idUserExternal)
        {
            VonDataAssociationDto associationDto = null;

            var data = Find(idApp, idUserExternal);

            if (data.Any())
            {
                var service = _serviceService.GetserviceByUrlName(idApp);

                var firstData = data.First();
                associationDto = new VonDataAssociationDto
                {
                    ServiceId = service.Id,
                    ServiceDto = service,
                    UserId = firstData.AnonymousUserId,
                    AnonymousUserDto = firstData.AnonymousUserDto,
                    IdUserExternal = new Guid(firstData.UserExternalId),
                    ReferenceNumber = firstData.ReferenceNumber,
                    ReferenceNumber2 = firstData.ReferenceNumber2,
                    ReferenceNumber3 = firstData.ReferenceNumber3,
                    ReferenceNumber4 = firstData.ReferenceNumber4,
                    ReferenceNumber5 = firstData.ReferenceNumber5,
                    ReferenceNumber6 = firstData.ReferenceNumber6,
                    DefaultCardId = Guid.Empty,
                    DefaultCard = null,
                    CardDtos = data.Select(x => new CardDto
                    {
                        Active = true,
                        DueDate = x.CardDueDate,
                        ExternalId = new Guid(x.CardExternalId),
                        MaskedNumber = x.CardMaskedNumber,
                        Name = x.CardName,
                        PaymentToken = x.CardToken
                    }).ToList()
                };
            }
            return associationDto;
        }

        public VonDataAssociationDto GetAsAssociationDto(string idApp, string idUserExternal, string idCardExternal)
        {
            VonDataAssociationDto associationDto = null;

            var data = Find(idApp, idUserExternal, idCardExternal);

            if (data != null)
            {
                var service = _serviceService.GetserviceByUrlName(idApp);

                associationDto = new VonDataAssociationDto
                {
                    ServiceId = service.Id,
                    ServiceDto = service,
                    UserId = data.AnonymousUserId,
                    AnonymousUserDto = data.AnonymousUserDto,
                    IdUserExternal = new Guid(data.UserExternalId),
                    DefaultCardId = new Guid(data.CardExternalId),
                    ReferenceNumber = data.ReferenceNumber,
                    ReferenceNumber2 = data.ReferenceNumber2,
                    ReferenceNumber3 = data.ReferenceNumber3,
                    ReferenceNumber4 = data.ReferenceNumber4,
                    ReferenceNumber5 = data.ReferenceNumber5,
                    ReferenceNumber6 = data.ReferenceNumber6,
                    DefaultCard = new CardDto
                    {
                        Active = true,
                        DueDate = data.CardDueDate,
                        ExternalId = new Guid(data.CardExternalId),
                        MaskedNumber = data.CardMaskedNumber,
                        Name = data.CardName,
                        PaymentToken = data.CardToken
                    },
                };
            }
            return associationDto;
        }

        public bool DeleteCard(string appId, string userExternalId, string cardExternalId)
        {
            var vonData = Find(appId, userExternalId, cardExternalId);
            if (vonData != null)
            {
                Delete(vonData.Id);
            }
            else
            {
                throw new BusinessException(CodeExceptions.USER_CARD_NOT_MATCH); //Es necesario?? (en ServiceAssociate si no lo encuentra tira esto)
            }
            return true;
        }

        public bool DeleteAssociation(string appId, string userExternalId)
        {
            var vonData = Find(appId, userExternalId);
            if (vonData != null && vonData.Any())
            {
                foreach (var dto in vonData)
                {
                    Delete(dto.Id);
                }
            }
            else
            {
                throw new BusinessException(CodeExceptions.USER_CARD_NOT_MATCH); //Es necesario?? (en ServiceAssociate si no lo encuentra tira esto)
            }
            return true;
        }

    }
}