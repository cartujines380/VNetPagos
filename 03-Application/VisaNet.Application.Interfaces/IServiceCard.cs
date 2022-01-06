using System;
using System.Collections.Generic;
using VisaNet.Application.Interfaces.Base;
using VisaNet.Domain.Entities;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.ReportsModel;
using VisaNet.Domain.EntitiesDtos.TableFilters;

namespace VisaNet.Application.Interfaces
{
    public interface IServiceCard : IService<Card, CardDto>
    {
        IEnumerable<CardDto> GetDataForTable(CardFilterDto filters);
        CardDto GetByToken(string token);
        void ActivateCard(CardOperationDto cardOperationDto);
        void DesactivateCard(CardOperationDto cardOperationDto);
        void EliminateCard(CardOperationDto cardOperationDto,bool notifymail = true);
        void MigrateServices(Guid userId, Guid oldCardId, Guid newCardId);
        CardDto GenerateExternalId(Guid id);
        CardMigrationTestDto TestMigration(Guid oldCardId, Guid newCardId, Guid applicationUserId);
        IEnumerable<ServiceAssociatedDto> GetAssociatedServices(Guid cardId);        
        IEnumerable<ReportCardsViewDto> ReportsCardsData(ReportsCardsFilterDto filters);
        int ReportsCardsDataCount(ReportsCardsFilterDto filters);
        string GetQuotasForBinAndService(int cardBin, Guid serviceId);
        string GetQuotasForBin(int cardBin);
        void EditCardDescription(Guid id, string description);
        void CardDeletedFromCS(Guid id, bool del);

        void DeleteOldCardsToken();

        void SendUserNotificationMail(CardOperationDto cardOperationDto, Card card, ApplicationUserDto user);
    }
}