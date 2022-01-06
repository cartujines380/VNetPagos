using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using VisaNet.Application.Implementations.Base;
using VisaNet.Application.Interfaces;
using VisaNet.Common.Logging.NLog;
using VisaNet.Domain.Entities;
using VisaNet.Domain.Entities.Enums;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Repository.Interfaces.Interfaces;

namespace VisaNet.Application.Implementations
{
    public class ServiceNotification : BaseService<Notification, NotificationDto>, IServiceNotification
    {
        public ServiceNotification(IRepositoryNotification repository)
            : base(repository)
        {
           
        }

        public override IQueryable<Notification> GetDataForTable()
        {
            return Repository.AllNoTracking();
        }

        public override NotificationDto Converter(Notification entity)
        {
            return new NotificationDto
            {
                Id = entity.Id,
                ServiceId = entity.ServiceId,
                Date = entity.Date,
                Message = entity.Message,
                NotificationPrupose = (NotificationPruposeDto)entity.NotificationPrupose,
                RegisteredUserId = entity.RegisteredUserId
            };
        }

        public override Notification Converter(NotificationDto entity)
        {
            return new Notification
            {
                Id = entity.Id,
                ServiceId = entity.ServiceId,
                Date = entity.Date,
                Message = entity.Message,
                NotificationPrupose = (NotificationPrupose)entity.NotificationPrupose,
                RegisteredUserId = entity.RegisteredUserId
            };
        }

        public IEnumerable<NotificationDto> GetDataForTable(NotificationFilterDto filters)
        {
            var query = Repository.AllNoTracking();

            if (!string.IsNullOrEmpty(filters.GenericSearch))
                query = query.Where(sc => sc.Service.Name.Contains(filters.GenericSearch.ToLower()) ||
                                          sc.Message.Contains(filters.GenericSearch.ToLower()));

            if (filters.UserId != default(Guid))
                query = query.Where(sc => sc.RegisteredUserId == filters.UserId);

            if (filters.From != default(DateTime))
                query = query.Where(sc => filters.From <= sc.Date);

            if (filters.To != default(DateTime))
            {
                filters.To = filters.To.AddDays(1).Date;
                query = query.Where(sc => sc.Date < filters.To);
            }


            if (!string.IsNullOrEmpty(filters.Service))
                query = query.Where(sc => sc.Service.Name.ToLower().Contains(filters.Service.ToLower()));

            //if (filters.NotificationPrupose != 0)
            //    query = query.Where(sc => ((int)sc.NotificationPrupose) == filters.NotificationPrupose);


            //if (filters.SortDirection == SortDirection.Asc)
            //    query = query.OrderByStringProperty(filters.OrderBy);
            //else
            //    query = query.OrderByStringPropertyDescending(filters.OrderBy);

            query = query.OrderByDescending(n => n.Date);


            if (filters.DisplayLength.HasValue)
            {
                query = query.Skip(filters.DisplayStart * filters.DisplayLength.Value);
                query = query.Take(filters.DisplayLength.Value);
            }
            else
            {
                query = query.Skip(filters.DisplayStart);
            }

            return query.Select(t => new NotificationDto
            {
                Id = t.Id,
                Date = t.Date,
                Message = t.Message,
                ServiceId = t.ServiceId,
                Service = new ServiceDto
                {
                    Name = t.Service.Name,
                    Description = t.Service.Description,
                    ImageName = t.Service.ImageName
                },
                RegisteredUserId = t.RegisteredUserId,
                RegisteredUser = new ApplicationUserDto
                {
                    Email = t.RegisteredUser.Email,
                    MobileNumber = t.RegisteredUser.MobileNumber
                },
                NotificationPrupose = (NotificationPruposeDto)t.NotificationPrupose,
            }).ToList();
        }

        public IEnumerable<NotificationDto> GetDashboardData(ReportsDashboardFilterDto filters)
        {
            var query = Repository.AllNoTracking();

            if (filters.From != default(DateTime))
            {
                query = query.Where(n => n.Date >= filters.From);
            }

            if (filters.To != default(DateTime))
            {
                filters.To = filters.To.AddDays(1);
                query = query.Where(n => n.Date < filters.To);
            }

            return query.Select(t => new NotificationDto
            {
                Id = t.Id,
                Date = t.Date,
                Message = t.Message
            }).ToList();
        }

        public int GetDashboardDataCount(ReportsDashboardFilterDto filters)
        {
            var query = Repository.AllNoTracking();

            if (filters.From != default(DateTime))
            {
                query = query.Where(n => n.Date >= filters.From);
            }

            if (filters.To != default(DateTime))
            {
                filters.To = filters.To.AddDays(1);
                query = query.Where(n => n.Date < filters.To);
            }

            return query.Count();
        }

        public override NotificationDto Create(NotificationDto entity, bool returnEntity = false)
        {
            try
            {
                return base.Create(entity, returnEntity);
            }
            catch (DbEntityValidationException dbEx)
            {
                Exception raise = dbEx;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        string message = string.Format("{0}:{1}",
                            validationErrors.Entry.Entity.ToString(),
                            validationError.ErrorMessage);
                        // raise a new exception nesting
                        // the current instance as InnerException
                        raise = new InvalidOperationException(message, raise);
                    }
                }
                NLogLogger.LogEvent(raise);
                throw raise;
            }
        }

    }
}