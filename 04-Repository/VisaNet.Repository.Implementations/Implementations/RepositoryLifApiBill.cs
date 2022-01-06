using System;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using VisaNet.Common.Security;
using VisaNet.Domain.Entities;
using VisaNet.Repository.Implementations.Base;
using VisaNet.Repository.Interfaces.Interfaces;

namespace VisaNet.Repository.Implementations.Implementations
{
    public class RepositoryLifApiBill : BaseRepository<LifApiBill>, IRepositoryLifApiBill
    {
        public RepositoryLifApiBill(DbContext context, IWebApiTransactionContext transactionContext)
            : base(context, transactionContext)
        {
        }
    }
}
