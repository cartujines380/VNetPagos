using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using VisaNet.Application.Implementations.Base;
using VisaNet.Application.Interfaces;
using VisaNet.Common.Exceptions;
using VisaNet.Common.Logging.NLog;
using VisaNet.Domain.Entities;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Repository.Interfaces.Interfaces;

namespace VisaNet.Application.Implementations
{
    public class ServiceAffiliationCard : BaseService<AffiliationCard, AffiliationCardDto>, IServiceAffiliationCard
    {
        public ServiceAffiliationCard(IRepositoryAffiliationCard repository) : base(repository)
        {
        }

        public override IQueryable<AffiliationCard> GetDataForTable()
        {
            throw new NotImplementedException();
        }

        public override AffiliationCardDto Converter(AffiliationCard entity)
        {
            var dto = new AffiliationCardDto()
            {
                Code = entity.Code,
                Name = entity.Name,
                BankId = entity.BankId,
                Id = entity.Id,
                Active = entity.Active
            };
            if (entity.Bank != null)
            {
                dto.BankDto = new BankDto()
                {
                    Name = entity.Bank.Name,
                    Code = entity.Bank.Code,
                };
            }
            return dto;
        }

        public override AffiliationCard Converter(AffiliationCardDto entity)
        {
            return new AffiliationCard()
            {
                Code = entity.Code,
                Name = entity.Name,
                BankId = entity.BankId,
                Id = entity.Id,
                Active = entity.Active
            };
        }

        public override void Edit(AffiliationCardDto entity)
        {
            Repository.ContextTrackChanges = true;
            var entity_db = Repository.GetById(entity.Id);
            try
            {
                entity_db.Code = entity.Code;
                entity_db.Name = entity.Name;
                if (entity.BankId == null || entity.BankId == Guid.Empty)
                {
                    entity_db.BankId = null;
                }
                else
                {
                    entity_db.BankId = entity.BankId;
                }
                Repository.Edit(entity_db);
                Repository.Save();
            }
            catch (DbUpdateException exception)
            {
                Repository.ContextTrackChanges = false;
                NLogLogger.LogEvent(exception);
                if (exception.Message.Contains("IX_AffiliationCard_Code") ||
                    (exception.InnerException != null && exception.InnerException.Message.Contains("IX_AffiliationCard_Code")) ||
                    (exception.InnerException.InnerException != null && exception.InnerException.InnerException.Message.Contains("IX_AffiliationCard_Code")))
                {
                    //IDOPERACION REPETIDO
                    throw new BusinessException(CodeExceptions.AFFILIATIONCARD_CODE_REPETED);
                }
            }

            Repository.ContextTrackChanges = false;
        }

        public override AffiliationCardDto Create(AffiliationCardDto entity, bool returnEntity = false)
        {
            Repository.ContextTrackChanges = true;
            var efEntity = Converter(entity);

            try
            {
                efEntity.GenerateNewIdentity();
                Repository.Create(efEntity);
                Repository.Save();
            }
            catch (DbUpdateException exception)
            {
                Repository.ContextTrackChanges = false;
                NLogLogger.LogEvent(exception);
                if (exception.Message.Contains("IX_AffiliationCard_Code") ||
                    (exception.InnerException != null && exception.InnerException.Message.Contains("IX_AffiliationCard_Code")) ||
                    (exception.InnerException.InnerException != null && exception.InnerException.InnerException.Message.Contains("IX_AffiliationCard_Code")))
                {
                    //IDOPERACION REPETIDO
                    throw new BusinessException(CodeExceptions.AFFILIATIONCARD_CODE_REPETED);
                }
                //TODO subo bussinesexception ?
            }

            Repository.ContextTrackChanges = false;
            return returnEntity ? GetById(efEntity.Id) : null;
        }

        public void ChangeStatus(Guid id)
        {
            Repository.ContextTrackChanges = true;
            var entityDb = Repository.GetById(id);

            entityDb.Active = !entityDb.Active;

            Repository.Edit(entityDb);
            Repository.Save();
            Repository.ContextTrackChanges = false;
        }

        public List<AffiliationCardDto> GetDataForTable(AffiliationCardFilterDto filterDto)
        {
            var query = GetDataForAffiliationCard(filterDto);

            if (filterDto.OrderBy == null)
                filterDto.OrderBy = "0";
            //ordeno, skip y take
            if (filterDto.OrderBy.Equals("0"))
            {
                query = filterDto.SortDirection == SortDirection.Asc ? query.OrderBy(x => x.Name) : query.OrderByDescending(x => x.Name);
            }
            else if (filterDto.OrderBy.Equals("1"))
            {
                query = filterDto.SortDirection == SortDirection.Asc ? query.OrderBy(x => x.Code) : query.OrderByDescending(x => x.Code);
            }
            else if (filterDto.OrderBy.Equals("2"))
            {
                query = filterDto.SortDirection == SortDirection.Asc ? query.OrderBy(x => x.Bank.Name) : query.OrderByDescending(x => x.Bank.Name);
            }
            else if (filterDto.OrderBy.Equals("3"))
            {
                query = filterDto.SortDirection == SortDirection.Asc ? query.OrderBy(x => x.Active) : query.OrderByDescending(x => x.Active);
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

        public int GetDataForAffiliationCardCount(AffiliationCardFilterDto filterDto)
        {
            var query = GetDataForAffiliationCard(filterDto);
            return query.Count();
        }

        private IQueryable<AffiliationCard> GetDataForAffiliationCard(AffiliationCardFilterDto filterDto)
        {
            var query = Repository.AllNoTracking(null, x => x.Bank);

            if (!string.IsNullOrEmpty(filterDto.Name))
                query = query.Where(x => x.Name.Contains(filterDto.Name));

            if (filterDto.Code > 0)
                query = query.Where(x => x.Code == filterDto.Code);

            if (filterDto.BankId != Guid.Empty)
            {
                query = query.Where(x => x.BankId == filterDto.BankId);
            }

            if (filterDto.Active != null)
            {
                query = query.Where(x => x.Active == filterDto.Active);
            }

            return query;
        }

    }
}