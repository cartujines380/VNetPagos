using System;
using System.Collections.Generic;
using System.Linq;
using VisaNet.Application.Implementations.Base;
using VisaNet.Application.Interfaces;
using VisaNet.Common.Exceptions;
using VisaNet.Common.FrameworkExtensions;
using VisaNet.Common.Security.Entities;
using VisaNet.Common.Security.Entities.Security;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Repository.Interfaces.Interfaces;
using Action = VisaNet.Common.Security.Entities.Action;

namespace VisaNet.Application.Implementations
{
    public class ServiceRole : BaseService<Role, RoleDto>, IServiceRole
    {
        private readonly IRepositorySystemUser _repositorySystemUser;

        public ServiceRole(IRepositoryRole repository, IRepositorySystemUser repositorySystemUser)
            : base(repository)
        {
            _repositorySystemUser = repositorySystemUser;
        }

        public override RoleDto Converter(Role entity)
        {
            return new RoleDto
            {
                Id = entity.Id,
                Name = entity.Name,
                ActionsIds = entity.Actions.Select(a => a.Id).ToList()
            };
        }

        public override Role Converter(RoleDto entity)
        {
            return new Role
            {
                Id = entity.Id,
                Name = entity.Name,
            };
        }


        public override IQueryable<Role> GetDataForTable()
        {
            return Repository.AllNoTracking();
        }

        public IEnumerable<RoleDto> GetDataForTable(RoleFilterDto filters)
        {
            var query = Repository.AllNoTracking();

            if (!string.IsNullOrEmpty(filters.GenericSearch))
                query = query.Where(sc => sc.Name.ToLower().Contains(filters.GenericSearch.ToLower()));

            if (!string.IsNullOrEmpty(filters.Name))
                query = query.Where(sc => sc.Name.Equals(filters.Name, StringComparison.InvariantCultureIgnoreCase));


            if (filters.SortDirection == SortDirection.Asc)
                query = query.OrderByStringProperty(filters.OrderBy);
            else
                query = query.OrderByStringPropertyDescending(filters.OrderBy);

            return query.Select(t => new RoleDto { Id = t.Id, Name = t.Name }).ToList();
        }


        public override RoleDto Create(RoleDto entity, bool returnEntity = false)
        {
            if (Repository.AllNoTracking(s => s.Name.Equals(entity.Name, StringComparison.InvariantCultureIgnoreCase)).Any())
                throw new BusinessException(CodeExceptions.SERVICECATEGORY_NAME_ALREADY_USED);

            Repository.ContextTrackChanges = true;

            var actions = ((IRepositoryRole)Repository).Actions.Where(a => entity.ActionsIds.Contains(a.Id)).ToList();

            var role = new Role
            {
                Name = entity.Name,
                Actions = actions,
            };

            Repository.Create(role);
            Repository.Save();
            Repository.ContextTrackChanges = false;

            return returnEntity ? Converter(role) : null;
        }

        public override void Edit(RoleDto entity)
        {
            if (Repository.AllNoTracking(s => s.Name.Equals(entity.Name, StringComparison.InvariantCultureIgnoreCase) && s.Id != entity.Id).Any())
                throw new BusinessException(CodeExceptions.SERVICECATEGORY_NAME_ALREADY_USED);


            Repository.ContextTrackChanges = true;

            var actions = ((IRepositoryRole)Repository).Actions.Where(a => entity.ActionsIds.Contains(a.Id)).ToList();

            var entity_db = Repository.GetById(entity.Id, r => r.Actions);

            entity_db.Name = entity.Name;
            entity_db.Actions.Clear();
            entity_db.Actions = actions;

            Repository.Edit(entity_db);
            Repository.Save();
            Repository.ContextTrackChanges = false;
        }

        public override void Delete(RoleDto entity)
        {
            Delete(entity.Id);
        }

        public override void Delete(Guid id)
        {
            if (_repositorySystemUser.AllNoTracking(s => s.Roles.Any(r => r.Id == id), s => s.Roles).Any())
                throw new BusinessException(CodeExceptions.ROLE_USERS_ASSOCIATED);

            var role = Repository.All(r => r.Id == id, r => r.Actions).FirstOrDefault();

            if (role != null) role.Actions.Clear();

            base.Delete(id);
        }


        public IEnumerable<FunctionalityGroup> GetFunctionalityGroupsNoTracking()
        {
            return ((IRepositoryRole)Repository).FunctionalityGroups.ToList();
        }

        public IEnumerable<Action> GetActionNoTracking()
        {
            return ((IRepositoryRole)Repository).Actions.ToList();
        }

        public IEnumerable<FunctionalityGroup> GetFunctionalityGroupsFromRoles(IEnumerable<Guid> roleIds)
        {
            var actionsFromRoles = Repository.AllNoTracking(r => roleIds.Contains(r.Id), r => r.Actions).SelectMany(a => a.Actions).ToList();
            var actionsIds = actionsFromRoles.Select(r => r.Id).ToList();

            if (!actionsFromRoles.Any())
                return new List<FunctionalityGroup>();

            var functionalities = ((IRepositoryRole)Repository).Functionalities.Where(f => f.Actions.Any(a => actionsIds.Contains(a.Id))).ToList();
            var functionalitiesGroups = ((IRepositoryRole)Repository).FunctionalityGroups.Where(fg => fg.Functionalities.Any(f => functionalities.Any(t => t.Id == f.Id))).ToList();

            return functionalitiesGroups.Select(fg =>
                new FunctionalityGroup
                {
                    Id = fg.Id,
                    Name = fg.Name,
                    IconClass = fg.IconClass,
                    Order = fg.Order,
                    Functionalities = functionalities.Where(f => f.FunctionalityGroupId == fg.Id).Select(f => new Functionality
                                                                    {
                                                                        Id = f.Id,
                                                                        Name = f.Name,
                                                                        IconClass = f.IconClass,
                                                                        Order = f.Order,
                                                                        FunctionalityGroupId = f.FunctionalityGroupId,
                                                                        MemberOfFunctionalityId = f.MemberOfFunctionalityId,
                                                                        Actions = actionsFromRoles.Where(a => a.FunctionalityId == f.Id).Select(a => new Action
                                                                                                        {
                                                                                                            Id = a.Id,
                                                                                                            Name = a.Name,
                                                                                                            IsDefaultAction = a.IsDefaultAction,
                                                                                                            MvcAction = a.MvcAction,
                                                                                                            MvcController = a.MvcController,
                                                                                                            FunctionalityId = a.FunctionalityId,
                                                                                                            ActionRequiredId = a.ActionRequiredId,
                                                                                                            ActionType = a.ActionType,
                                                                                                        }).ToList(),
                                                                    }).ToList(),

                }).ToList();
        }
    }
}
