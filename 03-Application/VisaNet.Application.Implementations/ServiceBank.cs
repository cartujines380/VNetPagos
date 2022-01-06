using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.XPath;
using VisaNet.Application.Implementations.Base;
using VisaNet.Application.Interfaces;
using VisaNet.Common.Exceptions;
using VisaNet.Common.Logging.NLog;
using VisaNet.Domain.Entities;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Repository.Interfaces.Interfaces;

namespace VisaNet.Application.Implementations
{
    public class ServiceBank : BaseService<Bank, BankDto>, IServiceBank
    {
        private readonly IServiceBin _serviceBin;
        //private readonly IServiceAffiliationCard _serviceAffiliationCard;

        public ServiceBank(IRepositoryBank repository, IServiceBin serviceBin/*, IServiceAffiliationCard serviceAffiliationCard*/)
            : base(repository)
        {
            _serviceBin = serviceBin;
            //_serviceAffiliationCard = serviceAffiliationCard;
        }

        public override IQueryable<Bank> GetDataForTable()
        {
            return Repository.All();
        }

        public override BankDto Converter(Bank entity)
        {
            var bankdto = new BankDto()
                   {
                       Id = entity.Id,
                       Code = entity.Code,
                       Name = entity.Name,
                       QuotesPermited = entity.QuotesPermited
                   };

            if (entity.Bins != null && entity.Bins.Any())
            {
                bankdto.BinsDto = entity.Bins.Select(x => new BinDto()
                                                          {
                                                              Value = x.Value,
                                                              Active = x.Active,
                                                              CardType = (CardTypeDto)x.CardType
                                                          }).ToList();
            }
            
            return bankdto;
        }

        public override Bank Converter(BankDto entity)
        {
            return new Bank()
            {
                Id = entity.Id,
                Code = entity.Code,
                Name = entity.Name,
                QuotesPermited = entity.QuotesPermited                 
            };
        }

        public BankDto Create(BankDto entity, bool returnEntity = false)
        {
            CheckValidations(entity);
            return base.Create(entity, returnEntity);
        }

        public void CheckValidations(BankDto entity)
        {
            if (Repository.AllNoTracking(s => s.Name.Equals(entity.Name, StringComparison.InvariantCultureIgnoreCase)).Any(s => s.Id != entity.Id))
            {
                throw new BusinessException(CodeExceptions.BANK_NAME_USED);
            }
            if (Repository.AllNoTracking(s => s.Code == entity.Code).Any(s => s.Id != entity.Id))
            {
                throw new BusinessException(CodeExceptions.BANK_CODE_USED);
            }
            if (string.IsNullOrEmpty(entity.QuotesPermited))
            {
                throw new BusinessException(CodeExceptions.BANK_QUOTA_EMPTY);
            }
            if (!entity.QuotesPermited.Contains("1"))
            {
                throw new BusinessException(CodeExceptions.BANK_FIRST_QUOTA_NOT_SELECTED);
            }
        }

        public override void Edit(BankDto entity)
        {

            CheckValidations(entity);

            Repository.ContextTrackChanges = true;
            var entity_db = Repository.GetById(entity.Id);

            //TIENE QUE EXISTIR UN BANCO BROU PARA LAS TRANSACCIONES EN SISTARBANC
            if (!entity_db.Name.Equals("BROU"))
            {
                entity_db.Name = entity.Name;
            }
            entity_db.Code = entity.Code;
            entity_db.QuotesPermited = entity.QuotesPermited;

            Repository.Edit(entity_db);
            Repository.Save();

            Repository.ContextTrackChanges = false;
        }

        public virtual void Delete(Guid id)
        {
            var entity_db = Repository.GetById(id);

            //TIENE QUE EXISTIR UN BANCO BROU PARA LAS TRANSACCIONES EN SISTARBANC
            if (entity_db.Name.Equals("BROU"))
            {
                throw new BusinessException(CodeExceptions.BANK_BROU_MUSTEXISTS);
            }

            Repository.Delete(id);
            Repository.Save();

        }

        public List<BankDto> GetDataForTable(BankFilterDto filterDto)
        {
            var query = GetDataForBank(filterDto);

            //ordeno, skip y take
            if (filterDto.OrderBy.Equals("0"))
            {
                query = filterDto.SortDirection == SortDirection.Asc ? query.OrderBy(x => x.Name) : query.OrderByDescending(x => x.Name);
            }
            else if (filterDto.OrderBy.Equals("1"))
            {
                query = filterDto.SortDirection == SortDirection.Asc ? query.OrderBy(x => x.Code) : query.OrderByDescending(x => x.Code);
            }
            else
            {
                query = filterDto.SortDirection == SortDirection.Asc ? query.OrderBy(x => x.Name) : query.OrderByDescending(x => x.Name);
            }

            query = query.Skip(filterDto.DisplayStart);
            if (filterDto.DisplayLength.HasValue)
                query = query.Take(filterDto.DisplayLength.Value);

            return query.ToList().Select(Converter).ToList();
        }
        public int GetDataForBankCount(BankFilterDto filterDto)
        {
            var query = GetDataForBank(filterDto);
            return query.Count();
        }

        private IQueryable<Bank> GetDataForBank(BankFilterDto filterDto)
        {
            var query = Repository.AllNoTracking();

            if (!string.IsNullOrEmpty(filterDto.Name))
                query = query.Where(x => x.Name.Contains(filterDto.Name));

            if (filterDto.Code > 0)
                query = query.Where(x => x.Code == filterDto.Code);

            if (filterDto.BinValue > 0)
                query = query.Where(x => x.Bins.Any(y => y.Value == filterDto.BinValue));

            return query;
        }

        //public ICollection<ApplicationUserDto> GetDataForReportsUser(ReportsUserFilterDto filterDto)
        //{
        //    var query = GetDataForTableReportsUser(filterDto);

        //    //ordeno, skip y take
        //    if (filterDto.OrderBy.Equals("0"))
        //    {
        //        query = filterDto.SortDirection == SortDirection.Asc ? query.OrderBy(x => x.CreationDate) : query.OrderByDescending(x => x.CreationDate);
        //    }
        //    else if (filterDto.OrderBy.Equals("1"))
        //    {
        //        query = filterDto.SortDirection == SortDirection.Asc ? query.OrderBy(x => x.Email) : query.OrderByDescending(x => x.Email);
        //    }
        //    else
        //    {
        //        query = filterDto.SortDirection == SortDirection.Asc ? query.OrderBy(x => x.CreationDate) : query.OrderByDescending(x => x.CreationDate);
        //    }

        //    query = query.Skip(filterDto.DisplayStart);
        //    if (filterDto.DisplayLength.HasValue)
        //        query = query.Take(filterDto.DisplayLength.Value);

        //    return query.ToList().Select(Converter).ToList();
        //}

        //public int GetDataForReportsUserCount(ReportsUserFilterDto filterDto)
        //{
        //    var query = GetDataForTableReportsUser(filterDto);
        //    return query.Count();
        //}
        //private IQueryable<ApplicationUser> GetDataForTableReportsUser(ReportsUserFilterDto filterDto)
        //{
        //    var query = Repository.AllNoTracking(null, x => x.Cards, x => x.Payments, x => x.ServicesAssociated, x => x.MembershipIdentifierObj);

        //    filterDto.DateTo = filterDto.DateTo.AddDays(1);

        //    query = query.Where(p => p.CreationDate.CompareTo(filterDto.DateFrom) >= 0);
        //    query = query.Where(p => p.CreationDate.CompareTo(filterDto.DateTo) < 0);

        //    if (!string.IsNullOrEmpty(filterDto.Email))
        //        query = query.Where(x => x.Email.Contains(filterDto.Email));

        //    if (filterDto.ActiveOrInactive == ActiveOrInactiveEnumDto.Active)
        //    {
        //        query = query.Where(x => x.MembershipIdentifierObj.Active);
        //    }
        //    if (filterDto.ActiveOrInactive == ActiveOrInactiveEnumDto.Inactive)
        //    {
        //        query = query.Where(x => !x.MembershipIdentifierObj.Active);
        //    }
        //    if (filterDto.ActiveOrInactive == ActiveOrInactiveEnumDto.Blocked)
        //    {
        //        query = query.Where(x => x.MembershipIdentifierObj.Blocked);
        //    }

        //    return query;
        //}

        public bool IsQuotaForbidden(int quota, int binValue)
        {
            //SI HAY CUOTA > 1 CHEQUEAMOS EL BANCO, SINO SE PERMITE.
            if (quota < 2)
            {
                //Si viene cuota 0 lo considero como cuota 1 que siempre se permite
                return false;
            }

            var bin = _serviceBin.Find(binValue);
            var bank = bin.BankDto;

            if (bank == null)
            {
                if (quota > 1)
                {
                    NLogLogger.LogEvent(NLogType.Info,
                        string.Format(
                            "El bin {0} no se encuentra definido en un Banco emisor por lo tanto la cuota ({1}) no puede ser mayor a 1.",
                            binValue, quota));
                    return true;
                }
                return false;
            }

            return !bank.QuotesPermited.Contains(quota.ToString());
        }

        public bool IsQuotaForbidden(int quota, Guid cardId)
        {
            var bin = _serviceBin.FindByGuid(cardId);
            return IsQuotaForbidden(quota, bin.Value);
        }
    }
}