using System;
using System.Collections.Generic;
using System.Linq;
using VisaNet.Domain.Entities;
using VisaNet.Repository.Implementations.Base;
using VisaNet.Repository.Interfaces.Interfaces;

namespace VisaNet.Repository.Implementations.Implementations
{
    public class RepositoryReports : IRepositoryReports
    {
        public List<Dashboard> GetDashboardSP(DateTime dateFrom, DateTime dateTo, string currency)
        {
            using (var context = new AppContext())
            {
                var result = context.Database.SqlQuery<Dashboard>(
                   "StoreProcedure_VisaNet_DashboardBO2 @inputDateFrom = {0}, @inputDateTo = {1}, @inputCurrency = {2}",
                   dateFrom, dateTo, currency);
                return result.ToList();
            }
        }
    }
}
