using System;
using System.Collections.Generic;
using VisaNet.Domain.Entities;
using VisaNet.Domain.Entities.ReportsModel;
using VisaNet.Repository.Interfaces.Base;

namespace VisaNet.Repository.Interfaces.Interfaces
{
    public interface IRepositoryConciliationSummary : IRepository<ConciliationSummary>
    {
        IEnumerable<T> ExecuteSQL<T>(string sql, object[] parameters);
        void GenerateSummary();

        IList<ReportConciliationDetail> ObtainTableInfo(DateTime from, DateTime to, string requestId = null,
            string uniqueIdenfifier = null, string serviceId = null,
            string email = null, string state = null, string gatewayName = null, string column = null, string comment = null,
            string order = "DESC", string displayLength = "1", string displayStart = "0");
    }
}
