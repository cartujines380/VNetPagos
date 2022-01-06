using System;
using System.Threading.Tasks;
using VisaNet.Common.Logging.Entities;

namespace VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration
{
    public interface ILogClientService
    {
        Task<LogDto> Find(string transactionId);
        Task Put(LogModel model);
        Task Put(Guid id, LogDto model);
    }
}
