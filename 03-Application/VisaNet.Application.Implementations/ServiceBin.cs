using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data.Entity.SqlServer;
using System.Linq;
using Newtonsoft.Json;
using VisaNet.Application.Implementations.Base;
using VisaNet.Application.Interfaces;
using VisaNet.Common.Exceptions;
using VisaNet.Common.FrameworkExtensions;
using VisaNet.Common.Logging.NLog;
using VisaNet.Domain.Entities;
using VisaNet.Domain.Entities.Enums;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Repository.Interfaces.Interfaces;
using VisaNet.Common.AzureUpload;

namespace VisaNet.Application.Implementations
{
    public class ServiceBin : BaseService<Bin, BinDto>, IServiceBin
    {
        private readonly IRepositoryCard _repositoryCard;
        private readonly IRepositoryBin repository;
        private readonly IServiceBinGroup _serviceBinGroup;
        private readonly IServiceEmailMessage _serviceNotificationMessage;
        private readonly IRepositoryParameters _repositoryParameters;
        private readonly IRepositoryBinGroup _repositoryBinGroup;
        private readonly IServiceAffiliationCard _serviceAffiliationCard;

        private string _folderBlob = ConfigurationManager.AppSettings["AzureBinsImagesUrl"];

        public ServiceBin(IRepositoryBin repository, IRepositoryCard repositoryCard, IServiceEmailMessage serviceNotificationMessage,
            IRepositoryParameters repositoryParameters, IRepositoryBinGroup repositoryBinGroup, IServiceBinGroup serviceBinGroup, IServiceAffiliationCard serviceAffiliationCard)
            : base(repository)
        {
            _repositoryCard = repositoryCard;
            _serviceNotificationMessage = serviceNotificationMessage;
            _repositoryParameters = repositoryParameters;
            this.repository = repository;
            _repositoryBinGroup = repositoryBinGroup;
            _serviceBinGroup = serviceBinGroup;
            _serviceAffiliationCard = serviceAffiliationCard;
        }

        public IEnumerable<BinDto> GetDataForTable(BinFilterDto filters)
        {
            var query = GetQuery(filters);

            //Sort
            switch (filters.OrderBy)
            {
                case "Name":
                    query = filters.SortDirection == SortDirection.Asc ? query.OrderBy(x => x.Name) : query.OrderByDescending(x => x.Name);
                    break;
                case "Value":
                    query = filters.SortDirection == SortDirection.Asc ? query.OrderBy(x => x.Value) : query.OrderByDescending(x => x.Value);
                    break;
                case "Country":
                    query = filters.SortDirection == SortDirection.Asc ? query.OrderBy(x => x.Country) : query.OrderByDescending(x => x.Country);
                    break;
                case "Bank":
                    query = filters.SortDirection == SortDirection.Asc ? query.OrderBy(x => x.Bank != null ? x.Bank.Name : null) : query.OrderByDescending(x => x.Bank != null ? x.Bank.Name : null);
                    break;
                case "CardType":
                    query = filters.SortDirection == SortDirection.Asc ? query.OrderBy(x => x.CardType) : query.OrderByDescending(x => x.CardType);
                    break;
                case "AffiliationCard":
                    query = filters.SortDirection == SortDirection.Asc ? query.OrderBy(x => x.AffiliationCard != null ? x.AffiliationCard.Name : null) : query.OrderByDescending(x => x.AffiliationCard != null ? x.AffiliationCard.Name : null);
                    break;
                case "Active":
                    query = filters.SortDirection == SortDirection.Asc ? query.OrderBy(x => x.Active) : query.OrderByDescending(x => x.Active);
                    break;
                default:
                    query = filters.SortDirection == SortDirection.Asc ? query.OrderBy(x => x.Name) : query.OrderByDescending(x => x.Name);
                    break;
            }

            //Skip & Take
            query = query.Skip(filters.DisplayStart);

            if (filters.DisplayLength.HasValue)
                query = query.Take(filters.DisplayLength.Value);

            return query.ToList().Select(t => new BinDto
            {
                Id = t.Id,
                Name = t.Name,
                GatewayName = t.Gateway == null ? string.Empty : t.Gateway.Name,
                Value = t.Value,
                CardType = (CardTypeDto)(int)t.CardType,
                Active = t.Active,
                BankDto = t.Bank == null ? null : new BankDto
                {
                    Code = t.Bank.Code,
                    Id = t.Bank.Id,
                    Name = t.Bank.Name
                },
                AffiliationCardId = t.AffiliationCardId,
                AffiliationCardDto = t.AffiliationCard == null ? null : new AffiliationCardDto()
                {
                    Name = t.AffiliationCard.Name,
                    Active = t.AffiliationCard.Active,
                    Code = t.AffiliationCard.Code,
                    BankId = t.AffiliationCard.BankId,
                },
                Country = t.Country
            }).ToList();
        }

        public int GetDataForTableCount(BinFilterDto filters)
        {
            var query = GetQuery(filters);
            return query.Count();
        }

        private IQueryable<Bin> GetQuery(BinFilterDto filters)
        {
            var query = Repository.AllNoTracking(null, x => x.Bank, x => x.Gateway, b => b.BinAuthorizationAmountTypeList, b => b.AffiliationCard);

            if (!string.IsNullOrEmpty(filters.Name))
                query = query.Where(sc => sc.Name.ToLower().Contains(filters.Name.ToLower()));

            if (!string.IsNullOrEmpty(filters.Value))
                query = query.Where(sc => SqlFunctions.StringConvert((double)sc.Value).Contains(filters.Value));

            if (!string.IsNullOrEmpty(filters.Gateway))
                query = query.Where(sc => sc.Gateway.Name.ToLower().Contains(filters.Gateway.ToLower()));

            if (!string.IsNullOrEmpty(filters.Country))
                query = query.Where(sc => sc.Country.ToLower().Contains(filters.Country.ToLower()));

            if (!string.IsNullOrEmpty(filters.Bank))
                query = query.Where(sc => sc.Bank.Name.ToLower().Contains(filters.Bank.ToLower()));

            int cardType;
            if (!string.IsNullOrEmpty(filters.CardType) && Int32.TryParse(filters.CardType, out cardType))
            {
                query = query.Where(sc => sc.CardType == (CardType)cardType);
            }

            if (!string.IsNullOrEmpty(filters.State))
            {
                if (filters.State == "1")
                {
                    query = query.Where(sc => sc.Active);
                }
                else if (filters.State == "0")
                {
                    query = query.Where(sc => !sc.Active);
                }
            }

            int from;
            if (!string.IsNullOrEmpty(filters.ValueFrom) && Int32.TryParse(filters.ValueFrom, out from))
            {
                query = query.Where(sc => sc.Value >= from);
            }

            int to;
            if (!string.IsNullOrEmpty(filters.ValueTo) && Int32.TryParse(filters.ValueTo, out to))
            {
                query = query.Where(sc => sc.Value <= to);
            }

            return query;
        }

        public override IQueryable<Bin> GetDataForTable()
        {
            throw new NotImplementedException();
        }

        public override BinDto Converter(Bin entity)
        {
            if (entity == null) return null;
            var dto = new BinDto()
            {
                Id = entity.Id,
                Name = entity.Name,
                Value = entity.Value,
                Description = entity.Description,
                GatewayName = entity.Gateway != null ? entity.Gateway.Name : "",
                GatewayId = entity.GatewayId,
                ImageName = entity.ImageName,
                ImageUrl = FileStorage.Instance.GetImageUrl(_folderBlob, entity.Id, entity.ImageName),
                CardType = (CardTypeDto)(int)entity.CardType,
                BankDtoId = entity.BankId,
                Active = entity.Active,
                Country = entity.Country,
                EditedFromBO = entity.EditedFromBO,
                AffiliationCardId = entity.AffiliationCardId,
            };

            if (entity.Bank != null)
            {
                dto.BankDto = new BankDto()
                {
                    Code = entity.Bank.Code,
                    Id = entity.Bank.Id,
                    Name = entity.Bank.Name,
                    QuotesPermited = entity.Bank.QuotesPermited
                };
            }

            if (entity.BinGroups != null && entity.BinGroups.Any())
            {
                dto.BinGroups = new Collection<BinGroupDto>();
                foreach (var binGroup in entity.BinGroups)
                {
                    dto.BinGroups.Add(_serviceBinGroup.Converter(binGroup));
                }
            }

            if (entity.BinAuthorizationAmountTypeList != null && entity.BinAuthorizationAmountTypeList.Any())
            {
                dto.BinAuthorizationAmountTypeDtoList =
                    entity.BinAuthorizationAmountTypeList.Select(x => new BinAuthorizationAmountTypeDto()
                    {
                        Id = x.Id,
                        AuthorizationAmountTypeDto = (AuthorizationAmountTypeDto)x.AuthorizationAmountType,
                        BinId = x.BinId,
                        LawDto = (DiscountTypeDto)x.Law,
                    }).ToList();
            }
            if (entity.AffiliationCard != null)
            {
                dto.AffiliationCardDto = new AffiliationCardDto()
                {
                    Name = entity.AffiliationCard.Name,
                    Active = entity.AffiliationCard.Active,
                    Code = entity.AffiliationCard.Code,
                    BankId = entity.AffiliationCard.BankId,
                };
            }
            return dto;
        }

        public override Bin Converter(BinDto dto)
        {
            var entity = new Bin()
            {
                Id = dto.Id,
                Name = dto.Name,
                Value = dto.Value,
                Description = dto.Description,
                GatewayId = dto.GatewayId,
                ImageName = dto.ImageName,
                CardType = (CardType)(int)dto.CardType,
                Active = dto.Active,
                Country = dto.Country,
                EditedFromBO = dto.EditedFromBO,
                BankId = dto.BankDtoId,
                AffiliationCardId = dto.AffiliationCardId,
            };
            if (dto.BankDto != null)
            {
                entity.Bank = new Bank()
                {
                    Code = dto.BankDto.Code,
                    Id = dto.BankDto.Id,
                    Name = dto.BankDto.Name,
                    QuotesPermited = dto.BankDto.QuotesPermited
                };
            }
            if (dto.BinAuthorizationAmountTypeDtoList != null && dto.BinAuthorizationAmountTypeDtoList.Any())
            {
                entity.BinAuthorizationAmountTypeList =
                    dto.BinAuthorizationAmountTypeDtoList.Select(x => new BinAuthorizationAmountType()
                    {
                        Id = x.Id,
                        AuthorizationAmountType = (AuthorizationAmountType)x.AuthorizationAmountTypeDto,
                        BinId = x.BinId,
                        Law = (DiscountType)x.LawDto,
                    }).ToList();
            }
            return entity;
        }

        public override BinDto Create(BinDto entity, bool returnEntity = false)
        {
            if (Repository.AllNoTracking(s => s.Value == entity.Value).Any())
            {
                throw new BusinessException(CodeExceptions.BIN_VALUE_USED);
            }

            entity.Country = entity.Country == null ? "" : entity.Country.ToUpper();

            if (entity.Country.ToLower() == "uy" && !(entity.CardType == CardTypeDto.Credit || entity.CardType == CardTypeDto.Debit || entity.CardType == CardTypeDto.NationalPrepaid))
            {
                throw new BusinessException(CodeExceptions.BIN_COUNTRY_MUST_NOT_BE_UY);
            }
            if (entity.Country.ToLower() != "uy" && (entity.CardType == CardTypeDto.Credit || entity.CardType == CardTypeDto.Debit || entity.CardType == CardTypeDto.NationalPrepaid))
            {
                throw new BusinessException(CodeExceptions.BIN_COUNTRY_MUST_BE_UY);
            }

            if (entity.AffiliationCardId != null && entity.AffiliationCardId != Guid.Empty)
            {
                var affilicationCard = _serviceAffiliationCard.GetById(entity.AffiliationCardId.Value);
                if (affilicationCard.BankId != entity.BankDtoId)
                {
                    throw new BusinessException(CodeExceptions.BIN_BANKID_AFFILIATIONCARD_BANKID);
                }
            }

            entity.EditedFromBO = true;
            if (entity.BankDtoId == Guid.Empty)
                entity.BankDtoId = null;

            var dto = base.Create(entity, true);

            try
            {
                Repository.ContextTrackChanges = true;
                var bin = Repository.GetById(dto.Id);
                var defaultBinGroupId = GetDefaultGroupBin(entity.CardType);
                var group = _repositoryBinGroup.GetByIdTracking(Guid.Parse(defaultBinGroupId), x => x.Bins);

                group.Bins.Add(bin);
                _repositoryBinGroup.Edit(group);
                _repositoryBinGroup.Save();
                Repository.ContextTrackChanges = false;
            }
            catch (Exception exception)
            {
                NLogLogger.LogEvent(exception);
            }

            return dto;
        }

        public override void Edit(BinDto entity)
        {

            if (Repository.AllNoTracking(s => s.Value == entity.Value).Any(s => s.Id != entity.Id))
            {
                throw new BusinessException(CodeExceptions.BIN_VALUE_USED);
            }

            entity.Country = entity.Country == null ? "" : entity.Country.ToUpper();

            if (entity.Country.ToLower() == "uy" && !(entity.CardType == CardTypeDto.Credit || entity.CardType == CardTypeDto.Debit || entity.CardType == CardTypeDto.NationalPrepaid))
            {
                throw new BusinessException(CodeExceptions.BIN_COUNTRY_MUST_NOT_BE_UY);
            }
            if (entity.Country.ToLower() != "uy" && (entity.CardType == CardTypeDto.Credit || entity.CardType == CardTypeDto.Debit || entity.CardType == CardTypeDto.NationalPrepaid))
            {
                throw new BusinessException(CodeExceptions.BIN_COUNTRY_MUST_BE_UY);
            }

            if (entity.AffiliationCardId != null && entity.AffiliationCardId != Guid.Empty)
            {
                var affilicationCard = _serviceAffiliationCard.GetById(entity.AffiliationCardId.Value);
                if (affilicationCard.BankId != entity.BankDtoId)
                {
                    throw new BusinessException(CodeExceptions.BIN_BANKID_AFFILIATIONCARD_BANKID);
                }
            }

            Repository.ContextTrackChanges = true;
            var entity_db = Repository.GetById(entity.Id, b => b.BinAuthorizationAmountTypeList);

            if (entity_db.Value.ToString() == ConfigurationManager.AppSettings["DefaultBin"])
            {
                throw new BusinessException(CodeExceptions.DEFAULT_BIN_CANNOT_BE_EDITED);
            }

            var newCardType = entity.CardType;
            var oldCardType = entity_db.CardType;

            entity_db.Name = entity.Name;
            entity_db.Description = entity.Description;
            entity_db.Value = entity.Value;
            entity_db.GatewayId = entity.GatewayId;
            entity_db.CardType = (CardType)entity.CardType;
            entity_db.BankId = entity.BankDtoId == Guid.Empty ? null : entity.BankDtoId;
            entity_db.Country = entity.Country;
            entity_db.ImageName = entity.ImageName;
            entity_db.EditedFromBO = true;
            entity_db.AffiliationCardId = entity.AffiliationCardId;

            foreach (var binAuthorizationAmountTypeDto in entity.BinAuthorizationAmountTypeDtoList)
            {
                var i = (AuthorizationAmountType)binAuthorizationAmountTypeDto.AuthorizationAmountTypeDto;
                var temp = entity_db.BinAuthorizationAmountTypeList.FirstOrDefault(x => x.Id == binAuthorizationAmountTypeDto.Id && x.AuthorizationAmountType != i);
                if (temp != null)
                {
                    temp.AuthorizationAmountType = (AuthorizationAmountType)binAuthorizationAmountTypeDto.AuthorizationAmountTypeDto;
                }
            }

            Repository.Edit(entity_db);
            Repository.Save();
            Repository.ContextTrackChanges = false;

            //ACTUALIZO GRUPO DE BIN SI ES NECESARIO
            try
            {
                if ((int)newCardType != (int)oldCardType)
                {
                    Repository.ContextTrackChanges = true;
                    var newDefaultBinGroupId = GetDefaultGroupBin(newCardType);
                    var oldDefaultBinGroupId = GetDefaultGroupBin((CardTypeDto)oldCardType);

                    var bin = Repository.GetById(entity.Id);
                    var oldGroup = _repositoryBinGroup.GetByIdTracking(Guid.Parse(oldDefaultBinGroupId), x => x.Bins);
                    oldGroup.Bins.Remove(bin);
                    _repositoryBinGroup.Edit(oldGroup);

                    var newGroup = _repositoryBinGroup.GetByIdTracking(Guid.Parse(newDefaultBinGroupId), x => x.Bins);
                    newGroup.Bins.Add(bin);
                    _repositoryBinGroup.Edit(newGroup);

                    _repositoryBinGroup.Save();
                    Repository.ContextTrackChanges = false;
                }
            }
            catch (Exception exception)
            {
                NLogLogger.LogEvent(exception);
            }
        }

        public override void Delete(Guid id)
        {
            var bin = Repository.GetById(id);
            if (bin.Value.ToString() == ConfigurationManager.AppSettings["DefaultBin"])
            {
                throw new BusinessException(CodeExceptions.DEFAULT_BIN_CANNOT_BE_DELETED);
            }
            base.Delete(id);
        }

        public BinDto Find(int value)
        {
            var bins = Repository.AllNoTracking(b => b.Value == value, b => b.Bank, b => b.BinGroups, b => b.BinAuthorizationAmountTypeList, b => b.AffiliationCard);
            return bins.Any() ? Converter(bins.FirstOrDefault()) : GetDefaultBin();
        }

        public void BinNotDefinedNotification(string bin)
        {
            var parameter = _repositoryParameters.AllNoTracking().First();
            _serviceNotificationMessage.SendBinNotDefined(parameter, bin);
        }

        public IEnumerable<BinDto> GetBinsFromMask(IList<int> mask)
        {
            var query = Repository.AllNoTracking(b => mask.Contains(b.Value), b => b.Bank, b => b.BinAuthorizationAmountTypeList);

            var list = query.Select(t => new BinDto
            {
                Id = t.Id,
                Name = t.Name,
                Value = t.Value,
                GatewayId = t.GatewayId,
                Active = t.Active,
                CardType = (CardTypeDto)t.CardType,
                AffiliationCardId = t.AffiliationCardId,
                ImageName = t.ImageName,
                BankDto = new BankDto
                {
                    Id = t.Bank == null ? Guid.Empty : t.Bank.Id,
                    Code = t.Bank == null ? 0 : t.Bank.Code,
                    Name = t.Bank == null ? null : t.Bank.Name,
                    QuotesPermited = t.Bank == null ? null : t.Bank.QuotesPermited
                },
                BinAuthorizationAmountTypeDtoList =
                    t.BinAuthorizationAmountTypeList.Select(x => new BinAuthorizationAmountTypeDto()
                    {
                        Id = x.Id,
                        AuthorizationAmountTypeDto = (AuthorizationAmountTypeDto)x.AuthorizationAmountType,
                        BinId = x.BinId,
                        LawDto = (DiscountTypeDto)x.Law,
                    }).ToList(),
                AffiliationCardDto = t.AffiliationCard != null ? new AffiliationCardDto()
                {
                    Name = t.AffiliationCard.Name,
                    Active = t.AffiliationCard.Active,
                    Code = t.AffiliationCard.Code,
                    BankId = t.AffiliationCard.BankId,
                } : null,
            }).ToList();

            foreach (var b in list)
            {
                b.ImageUrl = FileStorage.Instance.GetImageUrl(_folderBlob, b.Id, b.ImageName);
            }

            return list;
        }

        public BinDto GetDefaultBin()
        {
            var defaultBinValue = int.Parse(ConfigurationManager.AppSettings["DefaultBin"]);
            return Find(defaultBinValue);
        }

        public BinDto FindByGuid(Guid cardId)
        {
            var card = _repositoryCard.GetById(cardId);
            var binDto = Find(Convert.ToInt32(card.MaskedNumber.Substring(0, 6)));
            return binDto;
        }

        public void ChangeStatus(Guid binId)
        {
            var defaultBinValue = int.Parse(ConfigurationManager.AppSettings["DefaultBin"]);

            Repository.ContextTrackChanges = true;
            var entityDb = Repository.GetById(binId);

            if (entityDb.Value != defaultBinValue)
            {
                entityDb.Active = !entityDb.Active;
            }
            else
            {
                entityDb.Active = true;
            }

            Repository.Edit(entityDb);
            Repository.Save();
            Repository.ContextTrackChanges = false;
        }

        public ConcurrentDictionary<int, Guid> GetBinsEditedFromBO()
        {
            var data = new ConcurrentDictionary<int, Guid>(Repository.AllNoTracking().Where(b => b.EditedFromBO).Select(p => new { p.Value, p.Id }).AsEnumerable().ToDictionary(k => k.Value, i => i.Id));
            return data;
        }

        public ConcurrentDictionary<int, BinDto> GetBinsNotEditedFromBO()
        {
            var data = new ConcurrentDictionary<int, BinDto>(Repository.AllNoTracking().Where(b => !b.EditedFromBO)
                .Select(p => new
                {
                    p.Value,
                    p.Id,
                    p.CardType,
                    p.Country,
                    p.IssuerBin,
                    p.ProcessorBin,
                    p.GatewayId,
                    p.BinAuthorizationAmountTypeList
                }).AsEnumerable()
                .ToDictionary(k => k.Value, v => new BinDto
                {
                    Id = v.Id,
                    Value = v.Value,
                    CardType = (CardTypeDto)v.CardType,
                    Country = v.Country,
                    IssuerBin = v.IssuerBin,
                    ProcessorBin = v.ProcessorBin,
                    GatewayId = v.GatewayId,
                }));
            return data;
        }

        //Utilizamos este metodo para saber si hacer update de un bin leido del archivo de bines o si tiene los mismos datos que la ultima vez que fue procesado
        public bool MustUpdate(BinDto oldBin, BinDto newBin)
        {
            return !(oldBin.CardType == newBin.CardType && oldBin.Country == newBin.Country &&
                //SE QUITA PORQUE NO SE USA. EL RANGO DEL ARCHIVO ES DE 9 CHARS PERO EL BIN SOLO 6. ESO HACE Q TENGAMOS MUCHAS LINEAS PARA UN SOLO BIN CON DISTINTO ISSUERBIN. NO SIRVE DE NADA ESTE DATO
                //oldBin.IssuerBin == newBin.IssuerBin && oldBin.ProcessorBin == newBin.ProcessorBin && 
                oldBin.GatewayId == newBin.GatewayId);
        }

        public string GetDefaultGroupBin(CardTypeDto cardTypeDto)
        {
            switch (cardTypeDto)
            {
                case CardTypeDto.Credit:
                    return ConfigurationManager.AppSettings["CreditoNacional"];
                case CardTypeDto.InternationalCredit:
                    return ConfigurationManager.AppSettings["CreditoInternacional"];
                case CardTypeDto.Debit:
                    return ConfigurationManager.AppSettings["DebitoNacional"];
                case CardTypeDto.InternationalDebit:
                    return ConfigurationManager.AppSettings["DebitoInternacional"];
                case CardTypeDto.InternationalPrepaid:
                    return ConfigurationManager.AppSettings["PrepagoInternacional"];
                case CardTypeDto.NationalPrepaid:
                    return ConfigurationManager.AppSettings["PrepagoNacional"];
            }

            return null;
        }
    }
}