using System;
using System.Collections.Generic;
using VisaNet.Domain.Entities;

namespace VisaNet.Repository.Interfaces.Interfaces
{
    public interface IRepositoryConciliationDailySummary
    {
        ICollection<ConciliationDailySummary> GetConciliationDailySummary(DateTime dateFrom, DateTime dateTo);
    }
}