using System;
using System.Collections.Generic;
using System.Linq;
using VisaNet.Application.Implementations.Base;
using VisaNet.Application.Interfaces;
using VisaNet.Common.Exceptions;
using VisaNet.Common.FrameworkExtensions;
using VisaNet.Domain.Entities;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Repository.Interfaces.Interfaces;

namespace VisaNet.Application.Implementations
{
    public class ServiceSubscriber : BaseService<Subscriber, SubscriberDto>, IServiceSubscriber
    {
        public ServiceSubscriber(IRepositorySubscriber repository)
            : base(repository)
        {
        }

        public override IQueryable<Subscriber> GetDataForTable()
        {
            return Repository.AllNoTracking();
        }

        public override SubscriberDto Converter(Subscriber entity)
        {
            if (entity == null) return null;

            return new SubscriberDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Surname = entity.Surname,
                Email = entity.Email
            };
        }

        public override Subscriber Converter(SubscriberDto entity)
        {
            if (entity == null) return null;

            return new Subscriber
            {
                Id = entity.Id,
                Name = entity.Name,
                Surname = entity.Surname,
                Email = entity.Email
            };
        }

        public override SubscriberDto Create(SubscriberDto entity, bool returnEntity = false)
        {
            if (Repository.AllNoTracking(s => s.Email.Equals(entity.Email, StringComparison.InvariantCultureIgnoreCase)).Any())
                throw new BusinessException(CodeExceptions.SUBSCRIBER_EMAIL_ALREADY_USED);

            return base.Create(entity, returnEntity);
        }

        public IEnumerable<SubscriberDto> GetDataForTable(SubscriberFilterDto filters)
        {
            var query = Repository.AllNoTracking();

            if (!string.IsNullOrEmpty(filters.GenericSearch))
                query = query.Where(sc => sc.Name.ToLower().Contains(filters.GenericSearch.ToLower()) ||
                                          sc.Surname.ToLower().Contains(filters.GenericSearch.ToLower()) ||
                                          sc.Email.ToLower().Contains(filters.GenericSearch.ToLower()));


            if (!string.IsNullOrEmpty(filters.Name))
                query = query.Where(sc => sc.Name.ToLower().Contains(filters.Name.ToLower()));

            if (!string.IsNullOrEmpty(filters.Surname))
                query = query.Where(sc => sc.Surname.ToLower().Contains(filters.Surname.ToLower()));

            if (!string.IsNullOrEmpty(filters.Email))
                query = query.Where(sc => sc.Email.ToLower().Contains(filters.Email.ToLower()));


            if (filters.SortDirection == SortDirection.Asc)
                query = query.OrderByStringProperty(filters.OrderBy);
            else
                query = query.OrderByStringPropertyDescending(filters.OrderBy);


            return query.Select(t => new SubscriberDto { Id = t.Id, Name = t.Name, Surname = t.Surname, Email = t.Email }).ToList();
        }

        public IEnumerable<SubscriberDto> GetDashboardData(ReportsDashboardFilterDto filters)
        {
            var query = Repository.AllNoTracking();

            if (filters.From != default(DateTime))
            {
                query = query.Where(s => s.CreationDate >= filters.From);
            }

            if (filters.To != default(DateTime))
            {
                filters.To = filters.To.AddDays(1);
                query = query.Where(s => s.CreationDate < filters.To);
            }

            return query.Select(t => new SubscriberDto
            {
                Id = t.Id,
                Name = t.Name,
                Surname = t.Surname,
                Email = t.Email
            }).ToList();
        }

        public int GetDashboardDataCount(ReportsDashboardFilterDto filters)
        {
            var query = Repository.AllNoTracking();

            if (filters.From != default(DateTime))
            {
                query = query.Where(s => s.CreationDate >= filters.From);
            }

            if (filters.To != default(DateTime))
            {
                filters.To = filters.To.AddDays(1);
                query = query.Where(s => s.CreationDate < filters.To);
            }

            return query.Count();
        }

        public void DeleteByEmail(string email)
        {
            var entity =
                Repository.AllNoTracking(s => s.Email.Equals(email, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();

            if (entity != null)
            {
                base.Delete(entity.Id);
            }
        }

        public bool ExistsEmail(string email)
        {
            return Repository.AllNoTracking(s => s.Email.Equals(email, StringComparison.InvariantCultureIgnoreCase))
                .Any();
        }

    }
}
