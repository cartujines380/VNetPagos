using System;
using System.Collections.Generic;
using VisaNet.Application.Interfaces.Base;
using VisaNet.Common.Logging.Entities;
using VisaNet.Domain.Entities;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters;

namespace VisaNet.Application.Interfaces
{
    public interface IServiceTc33 : IService<Tc33, Tc33Dto>
    {
        IEnumerable<Tc33Dto> GetDataForTable(ReportsTc33FilterDto filters);
        int GetDataForTableCount(ReportsTc33FilterDto filters);
        IList<string> GetTC33Transactions(Guid id);
        void DownloadDetails(Guid id, out byte[] renderedBytes, out string mimeType);
        bool WasAlreadyProccessed(string requestId);
        LogDto GetLogFromDb(string requestId, int intento);
    }
}