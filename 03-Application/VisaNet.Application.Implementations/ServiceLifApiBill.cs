using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using VisaNet.Application.Implementations.Base;
using VisaNet.Application.Interfaces;
using VisaNet.Domain.Entities;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Repository.Interfaces.Interfaces;

namespace VisaNet.Application.Implementations
{
    public class ServiceLifApiBill : BaseService<LifApiBill, LifApiBillDto>, IServiceLifApiBill
    {
        public ServiceLifApiBill(IRepositoryLifApiBill repository) : base(repository)
        { }

        public override IQueryable<LifApiBill> GetDataForTable()
        {
            throw new System.NotImplementedException();
        }

        public override LifApiBillDto Converter(LifApiBill entity)
        {
            var dto = new LifApiBillDto()
            {
                Id = entity.Id,
                AppId = entity.AppId,
                OperationId = entity.OperationId,
                Currency = entity.Currency,
                Amount = entity.Amount,
                TaxedAmount = entity.TaxedAmount,
                IsFinalConsumer = entity.IsFinalConsumer,
                LawId = entity.LawId,
                BinValue = entity.BinValue,
                CardType = entity.CardType,
                IssuingCompany = entity.IssuingCompany,
                DiscountAmount = entity.DiscountAmount,
                AmountToCyberSource = entity.AmountToCyberSource,
                CreationDate = entity.CreationDate,
                Error = entity.Error
            };

            return dto;
        }

        public override LifApiBill Converter(LifApiBillDto entity)
        {
            return new LifApiBill()
            {
                Id = entity.Id,
                AppId = entity.AppId,
                OperationId = entity.OperationId,
                Currency = entity.Currency,
                Amount = entity.Amount,
                TaxedAmount = entity.TaxedAmount,
                IsFinalConsumer = entity.IsFinalConsumer,
                LawId = entity.LawId,
                BinValue = entity.BinValue,
                CardType = entity.CardType,
                IssuingCompany = entity.IssuingCompany,
                DiscountAmount = entity.DiscountAmount,
                AmountToCyberSource = entity.AmountToCyberSource,
                CreationDate = entity.CreationDate,
                Error = entity.Error
            };
        }

        public List<LifApiBillDto> GetDataForTable(LifApiBillFilterDto filterDto)
        {
            var query = GetDataForLifApiBill(filterDto);

            //ordeno, skip y take
            if (filterDto.OrderBy.Equals("0"))
            {
                query = filterDto.SortDirection == SortDirection.Asc ? query.OrderBy(x => x.CreationDate) : query.OrderByDescending(x => x.CreationDate);
            }
            else if (filterDto.OrderBy.Equals("1"))
            {
                query = filterDto.SortDirection == SortDirection.Asc ? query.OrderBy(x => x.AppId) : query.OrderByDescending(x => x.AppId);
            }
            else if (filterDto.OrderBy.Equals("2"))
            {
                query = filterDto.SortDirection == SortDirection.Asc ? query.OrderBy(x => x.OperationId) : query.OrderByDescending(x => x.OperationId);
            }
            else if (filterDto.OrderBy.Equals("3"))
            {
                query = filterDto.SortDirection == SortDirection.Asc ? query.OrderBy(x => x.Amount) : query.OrderByDescending(x => x.Amount);
            }
            else if (filterDto.OrderBy.Equals("4"))
            {
                query = filterDto.SortDirection == SortDirection.Asc ? query.OrderBy(x => x.TaxedAmount) : query.OrderByDescending(x => x.TaxedAmount);
            }
            else
            {
                query = filterDto.SortDirection == SortDirection.Asc ? query.OrderBy(x => x.CreationDate) : query.OrderByDescending(x => x.CreationDate);
            }

            query = query.Skip(filterDto.DisplayStart);
            if (filterDto.DisplayLength.HasValue)
                query = query.Take(filterDto.DisplayLength.Value);

            return query.ToList().Select(Converter).ToList();
        }

        public int GetDataForLifApiBillCount(LifApiBillFilterDto filterDto)
        {
            var query = GetDataForLifApiBill(filterDto);
            return query.Count();
        }

        private IQueryable<LifApiBill> GetDataForLifApiBill(LifApiBillFilterDto filterDto)
        {
            var query = Repository.AllNoTracking();

            if (!string.IsNullOrEmpty(filterDto.IdApp))
            {
                var idapp = filterDto.IdApp.ToLower();
                query = query.Where(x => x.AppId.ToLower().Contains(idapp));
            }

            if (!string.IsNullOrEmpty(filterDto.IdOperation))
            {
                var idOperation = filterDto.IdOperation.ToLower();
                query = query.Where(x => x.OperationId.ToLower().Contains(idOperation));
            }

            if (!string.IsNullOrEmpty(filterDto.BinValue))
            {
                var idOperation = filterDto.BinValue.ToLower();
                query = query.Where(x => x.BinValue.ToLower().Contains(idOperation));
            }

            if (!string.IsNullOrEmpty(filterDto.LawIndi))
            {
                var lawIndi = int.Parse(filterDto.LawIndi);
                query = query.Where(x => x.LawId == lawIndi);
            }

            DateTime from = DateTime.MinValue;
            if (!String.IsNullOrEmpty(filterDto.DateFromString))
            {
                from = DateTime.Parse(filterDto.DateFromString, new CultureInfo("es-UY"));
            }

            DateTime to = DateTime.MinValue;
            if (!String.IsNullOrEmpty(filterDto.DateToString))
            {
                to = DateTime.Parse(filterDto.DateToString, new CultureInfo("es-UY"));
            }

            if (!from.Equals(DateTime.MinValue))
            {
                query = query.Where(p => p.CreationDate.CompareTo(from) >= 0);
            }

            if (!to.Equals(DateTime.MinValue))
            {
                query = query.Where(p => p.CreationDate.CompareTo(to) <= 0);
            }

            return query;
        }

    }
}