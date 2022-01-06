using System;
using System.Collections.Generic;
using VisaNet.Application.Interfaces.Base;
using VisaNet.Domain.Entities;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.TableFilters;

namespace VisaNet.Application.Interfaces
{
    public interface IServiceDebitRequest : IService<DebitRequest, DebitRequestDto>
    {
        IEnumerable<DebitRequestDto> GetByUserId(Guid userId);
        IEnumerable<DebitRequestSyncDto> GetDebitToSync();
        List<DebitRequestTableDto> GetDataForFromList(DebitRequestsFilterDto filters);
        List<DebitRequestDto> GetDebitSuscriptionList(DebitRequestsFilterDto filters);
        int GetDebitSuscriptionListCount(DebitRequestsFilterDto filters);
        CybersourceCreateDebitWithNewCardDto ProccesDataFromCybersource(IDictionary<string, string> csDictionary);

        bool ValidateCardType(int binValue);
        bool CancelDebitRequest(Guid id);

        void SendDebitSuscriptionNotification(Guid debitRequestId);
        void SetRequestSynchronizated(Guid id, int referenceId);
        void SetRequestErrorSynchronization(Guid id, string errorMessage);

        IEnumerable<DebitRequestExcelDto> ExcelExportManualSynchronization();

        IEnumerable<DebitAssociatedDto> GetAssociatedDebits(Guid cardId);
    }
}