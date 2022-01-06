using System;
using System.Collections.Generic;
using VisaNet.Application.Interfaces.Base;
using VisaNet.Domain.Entities;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters;

namespace VisaNet.Application.Interfaces
{
    public interface IServiceAffiliationCard : IService<AffiliationCard, AffiliationCardDto>
    {
        void ChangeStatus(Guid id);
        List<AffiliationCardDto> GetDataForTable(AffiliationCardFilterDto filterDto);
        int GetDataForAffiliationCardCount(AffiliationCardFilterDto filterDto);
    }
}
