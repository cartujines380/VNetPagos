using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using VisaNet.Common.Logging.Entities;
using VisaNet.Common.Logging.Resources;
using VisaNet.Common.Security;
using VisaNet.Domain.Entities;
using VisaNet.Repository.Implementations.Base;
using VisaNet.Repository.Interfaces.Interfaces;

namespace VisaNet.Repository.Implementations.Implementations
{
    public class RepositoryApplicationUser : BaseRepository<ApplicationUser>, IRepositoryApplicationUser
    {
        
        public RepositoryApplicationUser(DbContext context, IWebApiTransactionContext transactionContext)
            : base(context, transactionContext)
        {
        }

        public long GetNextCyberSourceIdentifier()
        {
            return _db.SqlQuery<long>("SELECT NEXT VALUE FOR ApplicationUsers_CyberSourceIdentifier").First();
        }

        public Card AddCardToUser(Guid userId, Card card)
        {
            this.ContextTrackChanges = true;
            var entity = GetById(userId, s => s.Cards);
            card.GenerateNewIdentity();
            if (entity.Cards == null)
                entity.Cards = new Collection<Card>();

            entity.Cards.Add(card);
            Edit(entity);
            Save();
            ContextTrackChanges = false;
            
            return card;
        }

        public IWebApiTransactionContext GetTransactionContext()
        {
            return TransactionContext;
        }
    }
}