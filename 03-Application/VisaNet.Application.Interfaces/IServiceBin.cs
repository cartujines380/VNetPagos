using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using VisaNet.Application.Interfaces.Base;
using VisaNet.Domain.Entities;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Domain.EntitiesDtos.TableFilters;

namespace VisaNet.Application.Interfaces
{
    public interface IServiceBin : IService<Bin, BinDto>
    {
        IEnumerable<BinDto> GetDataForTable(BinFilterDto filters);
        int GetDataForTableCount(BinFilterDto filters);
        BinDto Find(int value);
        void BinNotDefinedNotification(string bin);
        IEnumerable<BinDto> GetBinsFromMask(IList<int> mask);
        BinDto GetDefaultBin();
        BinDto FindByGuid(Guid cardId);
        void ChangeStatus(Guid serviceId);
        ConcurrentDictionary<int, Guid> GetBinsEditedFromBO();
        ConcurrentDictionary<int, BinDto> GetBinsNotEditedFromBO();
        bool MustUpdate(BinDto oldBin, BinDto newBin);
        string GetDefaultGroupBin(CardTypeDto cardTypeDto);
    }
}
