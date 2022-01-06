using System.Data.Entity;
using VisaNet.Common.Security;
using VisaNet.Common.Security.Entities.Security;
using VisaNet.Repository.Implementations.Base;
using VisaNet.Repository.Interfaces.Interfaces;

namespace VisaNet.Repository.Implementations.Implementations
{
    public class RepositoryMembershipUser : BaseRepository<MembershipUser>, IRepositoryMembershipUser
    {
        public RepositoryMembershipUser(DbContext context, IWebApiTransactionContext transactionContext)
            : base(context, transactionContext)
        {
        }
    }
}
