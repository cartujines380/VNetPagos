using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using VisaNet.Common.Security;
using VisaNet.Common.Security.Entities;
using VisaNet.Common.Security.Entities.Security;
using VisaNet.Repository.Implementations.Base;

using VisaNet.Repository.Interfaces.Interfaces;

namespace VisaNet.Repository.Implementations.Implementations
{
    public class RepositoryRole : BaseRepository<Role>, IRepositoryRole
    {
        private readonly DbContext _context;

        public RepositoryRole(DbContext context, IWebApiTransactionContext transactionContext)
            : base(context, transactionContext)
        {
            _context = context;
        }

        public IList<Action> Actions
        {
            get { return _context.Set<Action>().Include(a => a.Functionality.FunctionalityGroup).ToList(); }
        }

        public IList<Action> ActionsNoTracking
        {
            get { return _context.Set<Action>().AsNoTracking().Include(a => a.Functionality.FunctionalityGroup).ToList(); }
        }

        public IList<Functionality> Functionalities
        {
            get { return _context.Set<Functionality>().AsNoTracking().Include(f => f.Actions).ToList(); }
        }

        public IList<FunctionalityGroup> FunctionalityGroups
        {
            get { return _context.Set<FunctionalityGroup>().Include(fg => fg.Functionalities.Select(f => f.Actions)).ToList(); }
        }

        public IList<FunctionalityGroup> FunctionalityGroupsNoTracking
        {
            get { return _context.Set<FunctionalityGroup>().AsNoTracking().Include(fg => fg.Functionalities.Select(f => f.Actions)).ToList(); }
        }
    }
}
