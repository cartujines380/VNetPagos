using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Common.Logging.Entities;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters;

namespace VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration
{
    public interface ITc33ClientService
    {
        Task<IEnumerable<Tc33Dto>> GetDataForTable(ReportsTc33FilterDto filtersDto);
        Task<int> GetDataForTableCount(ReportsTc33FilterDto filtersDto);
        Task<Tc33Dto> GetTC33(Guid id);
        Task<IList<string>> GetTC33Transactions(Guid id);
        Task<byte[]> GenerateDetailsFile(Guid id);
        Task<Tc33Dto> CreateProcess(Tc33Dto dto);
        Task<Tc33Dto> EditProcess(Tc33Dto dto);
        Task<bool> WasAlreadyProccessed(string requestId);
        Task<LogDto> GetLogFromDb(string requestId);
    }
}