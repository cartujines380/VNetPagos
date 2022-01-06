using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisaNet.Common.Security;
using VisaNet.Domain.Entities;
using VisaNet.Repository.Implementations.Base;
using VisaNet.Repository.Interfaces.Interfaces;

namespace VisaNet.Repository.Implementations.Implementations
{
    public class RepositorySistarbancUser : BaseRepository<SistarbancUser>, IRepositorySistarbancUser
    {
        public RepositorySistarbancUser(DbContext context, IWebApiTransactionContext transactionContext)
            : base(context, transactionContext)
        {
        }

        public void ActiveUserSistarbanc(Guid id)
        {
            ContextTrackChanges = true;

            var sistarbancUser = this.GetById(id);
            sistarbancUser.Active = true;

            this.Edit(sistarbancUser);
            this.Save();

            ContextTrackChanges = false;
        }
    }
}
