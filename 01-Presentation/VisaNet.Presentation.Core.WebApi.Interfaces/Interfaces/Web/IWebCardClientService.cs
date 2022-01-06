using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters.Base;

namespace VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Web
{
    public interface IWebCardClientService
    {
        Task<ICollection<CardDto>> FindAll();
        Task<ICollection<CardDto>> FindAll(BaseFilter filtersDto);
        Task<CardDto> Find(Guid id);
        Task<CardDto> FindByToken(String token);
        Task<CardDto> Create(CardDto entity);
        Task Edit(CardDto entity);
        Task ActivateCard(CardOperationDto cardOperationDto);
        Task DesactivateCard(CardOperationDto cardOperationDto);
        Task EliminateCard(CardOperationDto cardOperationDto);
        Task MigrateServices(Guid userId, Guid oldCardId, Guid newCardId);
        Task<CardDto> GenerateExternalId(CardOperationDto dto);
        Task<CardDto> FindWithServices(Guid id);
        Task<CardMigrationTestDto> TestMigration(Guid oldCardId, Guid newCardId, Guid userUd);
        Task<ICollection<ServiceAssociatedDto>> GetAssociatedServices(Guid cardId);
        Task<ICollection<DebitAssociatedDto>> GetAssociatedDebits(Guid cardId);
        Task<string> GetQuotasForBin(int cardBin);
        Task<string> GetQuotasForBinAndService(int cardBin, Guid serviceId);
        Task EditCardDescription(Guid cardId, string description);
    }
}