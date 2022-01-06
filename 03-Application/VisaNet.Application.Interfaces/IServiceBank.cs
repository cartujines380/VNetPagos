using System.Collections.Generic;
using System.Linq;
using VisaNet.Application.Interfaces.Base;
using VisaNet.Domain.Entities;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters;

namespace VisaNet.Application.Interfaces
{
    public interface IServiceBank : IService<Bank, BankDto>
    {
        List<BankDto> GetDataForTable(BankFilterDto filterDto);
        int GetDataForBankCount(BankFilterDto filterDto);
        bool IsQuotaForbidden(int quota, int binValue);
    }
}
