using System;
using System.Collections.Generic;
using System.Linq;
using VisaNet.Application.Interfaces;
using VisaNet.Common.Logging.Entities;
using VisaNet.Common.Logging.Services;
using VisaNet.Common.Resource.EntitiesDto;
using VisaNet.Common.Resource.Enums;
using VisaNet.Common.Resource.Helpers;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters;

namespace VisaNet.Application.Implementations
{
    public class ServiceAudit : IServiceAudit
    {
        private readonly ILoggerService _loggerService;

        public ServiceAudit(ILoggerService loggerService)
        {
            _loggerService = loggerService;
        }

        public IEnumerable<LogDto> GetDetails(Guid id)
        {
            var result = _loggerService.All(null, l => l.TransactionIdentifier == id).ToList();

            return result;
        }

        public IEnumerable<AuditTransactionLogDto> GetDataForTable(AuditFilterDto filterDto)
        {
            //filterDto.From = filterDto.From.Date;
            //filterDto.To = filterDto.To.Date;

            if (filterDto.DisplayLength.HasValue == false)
                filterDto.DisplayLength = 10;

            var x =
                _loggerService.Transactions(filterDto.From, filterDto.To,
                                            filterDto.SystemUserId, filterDto.UserId,
                                            filterDto.Ip, filterDto.LogUserType, filterDto.LogOperationType, filterDto.User,
                                            filterDto.DisplayStart, filterDto.DisplayLength.Value, filterDto.Message)
                                            .ToList();

            return x;
        }

        public IEnumerable<AuditExcelDto> ExcelExport(AuditFilterDto filterDto)
        {
            var x =
                _loggerService.Transactions(filterDto.From, filterDto.To,
                                            filterDto.SystemUserId, filterDto.UserId,
                                            filterDto.Ip, filterDto.LogUserType, filterDto.LogOperationType, filterDto.User, filterDto.DisplayStart, 214748364, filterDto.Message)
                                            .ToList();

            return x.Select(z => new AuditExcelDto
            {
                AnonymousUser = z.AnonymousUserEmail,
                ApplicationUser = z.ApplicationUserEmail,
                Date = z.TransactionDateTime.ToString("dd/mm/yyyy HH:MM:ss"),
                Ip = z.IP,
                OperationType = EnumHelpers.GetName(typeof(LogOperationType), (int)z.LogOperationType, EnumsStrings.ResourceManager),
                SystemUser = z.LDAPUserName,
                UserType = EnumHelpers.GetName(typeof(LogUserType), (int)z.LogUserType, EnumsStrings.ResourceManager),
                Message = z.Message,
                LogType = EnumHelpers.GetName(typeof(LogType), (int)z.LogType, EnumsStrings.ResourceManager),
                LogCommunicationType = EnumHelpers.GetName(typeof(LogCommunicationType), (int)z.LogCommunicationType, EnumsStrings.ResourceManager)
            });
        }
    }
}
