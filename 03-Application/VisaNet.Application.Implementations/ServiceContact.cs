using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using VisaNet.Application.Implementations.Base;
using VisaNet.Application.Interfaces;
using VisaNet.Common.FrameworkExtensions;
using VisaNet.Domain.Entities;
using VisaNet.Domain.Entities.Enums;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Repository.Interfaces.Interfaces;

namespace VisaNet.Application.Implementations
{
    public class ServiceContact : BaseService<Contact, ContactDto>, IServiceContact
    {
        
        private readonly IRepositoryParameters _repositoryParameters;
        private readonly IServiceEmailMessage _serviceNotificationMessage;

        public ServiceContact(IRepositoryContact repository, IRepositoryParameters repositoryParameters, IServiceEmailMessage serviceNotificationMessage)
            : base(repository)
        {
            _repositoryParameters = repositoryParameters;
            _serviceNotificationMessage = serviceNotificationMessage;
        }

        public override IQueryable<Contact> GetDataForTable()
        {
            return Repository.AllNoTracking();
        }

        public override ContactDto Converter(Contact entity)
        {
            if (entity == null) return null;

            var contact =  new ContactDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Surname = entity.Surname,
                Email = entity.Email,
                QueryType = (QueryTypeDto)(int)entity.QueryType,
                Subject = entity.Subject,
                Message = entity.Message,
                Date = entity.Date,
                Taken = entity.Taken,
                Comments = entity.Comments,
                UserTookId = entity.UserTookId,
                PhoneNumber = entity.PhoneNumber
            };

            if (entity.UserTook != null)
            {
                contact.UserTook = new SystemUserDto()
                {
                    Id = entity.Id,
                    LDAPUserName = entity.UserTook.LDAPUserName,
                    Enabled = entity.UserTook.Enabled,
                    Roles =
                        entity.UserTook.Roles.Select(r => new RoleDto {Id = r.Id, Name = r.Name})
                        .ToList(),
                    SystemUserType = (SystemUserTypeDto) entity.UserTook.SystemUserType,
                };
            }


            return contact;
        }

        public override Contact Converter(ContactDto entity)
        {
            if (entity == null) return null;

            return new Contact
            {
                Id = entity.Id,
                Name = entity.Name,
                Surname = entity.Surname,
                Email = entity.Email,
                QueryType = (QueryType)(int)entity.QueryType,
                Subject = entity.Subject,
                Message = entity.Message,
                Date = entity.Date,
                Comments = entity.Comments,
                Taken = entity.Taken,
                UserTookId = entity.UserTookId,
                PhoneNumber = entity.PhoneNumber
            };
        }

        public IEnumerable<ContactDto> GetDataForTable(ContactFilterDto filters)
        {
            var query = Repository.AllNoTracking();

            if (!string.IsNullOrEmpty(filters.GenericSearch))
                query = query.Where(sc => sc.Name.ToLower().Contains(filters.GenericSearch.ToLower()) ||
                                          sc.Surname.ToLower().Contains(filters.GenericSearch.ToLower()) ||
                                          sc.Email.ToLower().Contains(filters.GenericSearch.ToLower()) ||
                    //sc.QueryType.ToString().ToLower().Contains(filters.GenericSearch.ToLower()) ||
                                          sc.Subject.ToLower().Contains(filters.GenericSearch.ToLower()));


            if (!string.IsNullOrEmpty(filters.Name))
                query = query.Where(sc => sc.Name.ToLower().Contains(filters.Name.ToLower()));

            if (!string.IsNullOrEmpty(filters.Surname))
                query = query.Where(sc => sc.Surname.ToLower().Contains(filters.Surname.ToLower()));

            if (!string.IsNullOrEmpty(filters.Email))
                query = query.Where(sc => sc.Email.ToLower().Contains(filters.Email.ToLower()));

            if (filters.QueryType != 0)
                query = query.Where(sc => ((int)sc.QueryType) == filters.QueryType);


            if (filters.SortDirection == SortDirection.Asc)
                query = query.OrderByStringProperty(filters.OrderBy);
            else
                query = query.OrderByStringPropertyDescending(filters.OrderBy);


            return query.Select(t => new ContactDto { Id = t.Id, Name = t.Name, Surname = t.Surname, Email = t.Email, QueryType = (QueryTypeDto)(int)t.QueryType, Subject = t.Subject, Date = t.Date }).ToList();
        }

        public override ContactDto Create(ContactDto entity, bool returnEntity = false)
        {
            entity.Date = DateTime.Now;

            var parameters = _repositoryParameters.AllNoTracking().FirstOrDefault();

            if (parameters == null) throw new Exception();


            var contact = Converter(entity);
            Repository.Create(contact);

            _serviceNotificationMessage.SendContactFormEmail(contact, parameters);

            Repository.Save();

            return returnEntity ? Converter(contact) : null;
        }

        public override void Edit(ContactDto entity)
        {
            Repository.ContextTrackChanges = true;

            var contact = Repository.GetById(entity.Id);

            contact.Id = entity.Id;
            contact.Taken = entity.Taken;
            contact.UserTookId = entity.UserTookId;
            contact.Comments = entity.Comments;
            contact.PhoneNumber = entity.PhoneNumber;

            Repository.Edit(contact);
            Repository.Save();

            Repository.ContextTrackChanges = false;
        }

        public IEnumerable<ContactDto> GetDashboardData(ReportsDashboardFilterDto filters)
        {
            var query = Repository.AllNoTracking();

            if (filters.From != default(DateTime))
            {
                query = query.Where(c => c.Date >= filters.From);
            }

            if (filters.To != default(DateTime))
            {
                filters.To = filters.To.AddDays(1);
                query = query.Where(c => c.Date < filters.To);
            }

            return query.Select(t => new ContactDto
            {
                Id = t.Id, 
                Name = t.Name, 
                Surname = t.Surname, 
                Email = t.Email, 
                QueryType = (QueryTypeDto)(int)t.QueryType, 
                Subject = t.Subject, 
                Date = t.Date
            }).ToList();
        }

        //nuevo
        public int[] GetDashboardDataCount(ReportsDashboardFilterDto filters)
        {
            var array = new int[5];
            
            var query = Repository.AllNoTracking();

            if (filters.From != default(DateTime))
            {
                query = query.Where(c => c.Date >= filters.From);
            }

            if (filters.To != default(DateTime))
            {
                filters.To = filters.To.AddDays(1);
                query = query.Where(c => c.Date < filters.To);
            }

            array[0] = query.Count();
            array[1] = query.Count(c => c.QueryType == QueryType.Complaint);
            array[2] = query.Count(c => c.QueryType == QueryType.Question);
            array[3] = query.Count(c => c.QueryType == QueryType.Suggestion);
            array[4] = query.Count(c => c.QueryType == QueryType.Other);

            return array;
        }
    }
}
