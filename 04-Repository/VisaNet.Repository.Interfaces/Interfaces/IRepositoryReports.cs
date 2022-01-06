
using System;
using System.Collections.Generic;
using VisaNet.Domain.Entities;

namespace VisaNet.Repository.Interfaces.Interfaces
{
    public interface IRepositoryReports
    {
        List<Dashboard> GetDashboardSP(DateTime dateFrom, DateTime dateTo, string currency);
    }
}
