using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using VisaNet.Domain.Entities;
using VisaNet.Repository.Implementations.Base;
using VisaNet.Repository.Interfaces.Interfaces;

namespace VisaNet.Repository.Implementations.Implementations
{
    public class RepositoryConciliationDailySummary : IRepositoryConciliationDailySummary
    {
        private readonly AppContext _context;

        public RepositoryConciliationDailySummary(AppContext context)
        {
            _context = context;
        }

        public ICollection<ConciliationDailySummary> GetConciliationDailySummary(DateTime dateFrom, DateTime dateTo)
        {
            return _context.Database.SqlQuery<ConciliationDailySummary>("exec [StoreProcedure_VisaNet_ConciliationDaily] @From, @To",
                new SqlParameter("From", dateFrom), new SqlParameter("To", dateTo)).ToList();
        }

    }
}