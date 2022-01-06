using System;
using System.Data.Entity;
using VisaNet.Common.Security;
using VisaNet.Domain.Entities;
using VisaNet.Repository.Implementations.Base;
using VisaNet.Repository.Interfaces.Interfaces;

namespace VisaNet.Repository.Implementations.Implementations
{
    public class RepositoryCard : BaseRepository<Card>, IRepositoryCard
    {
        public RepositoryCard(DbContext context, IWebApiTransactionContext transactionContext)
            : base(context, transactionContext)
        {
        }


        public Card GenerateExternalId(Guid id)
        {
            ContextTrackChanges = true;
            var card = GetById(id);
            if (!card.ExternalId.HasValue)
            {
                card.ExternalId = Guid.NewGuid();
            }
            Edit(card);
            Save();
            ContextTrackChanges = false;
            return GetById(id);
        }

    }
}
