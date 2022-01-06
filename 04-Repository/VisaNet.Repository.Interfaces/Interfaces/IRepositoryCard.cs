using System;
using VisaNet.Domain.Entities;
using VisaNet.Repository.Interfaces.Base;

namespace VisaNet.Repository.Interfaces.Interfaces
{
    public interface IRepositoryCard : IRepository<Card>
    {
        Card GenerateExternalId(Guid id);
    }
}
