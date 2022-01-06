using System.Collections.Generic;
using System.Linq;
using VisaNet.Application.Implementations.Base;
using VisaNet.Application.Interfaces;
using VisaNet.Domain.Entities;
using VisaNet.Domain.Entities.Enums;
using VisaNet.Domain.Entities.ReportsModel;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Domain.EntitiesDtos.ReportsModel;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Repository.Interfaces.Interfaces;

namespace VisaNet.Application.Implementations
{
    public class ServiceConciliationSummary : BaseService<ConciliationSummary, ConciliationSummaryDto>, IServiceConciliationSummary
    {
        private readonly IRepositoryConciliationSummary _repositoryConciliationSummary;

        public ServiceConciliationSummary(IRepositoryConciliationSummary repository)
            : base(repository)
        {
            _repositoryConciliationSummary = repository;
        }

        public override IQueryable<ConciliationSummary> GetDataForTable()
        {
            return Repository.AllNoTracking();
        }

        public override ConciliationSummaryDto Converter(ConciliationSummary entity)
        {
            var dto = new ConciliationSummaryDto
            {
                Id = entity.Id,
                Date = entity.Date,
                TransactionNumber = entity.TransactionNumber,
                VisaTransactionId = entity.VisaTransactionId,
                Type = (ConciliationTypeDto)(int)entity.Type,
                GeneralComment = entity.GeneralComment,
                ConciliationCybersourceSummary = new ConciliationCybersourceSummaryDto
                {
                    ConciliationCybersourceId = entity.ConciliationCybersourceSummary.ConciliationCybersourceId,
                    State = (ConciliationStateDto)(int)entity.ConciliationCybersourceSummary.State
                },
                ConciliationGatewaySummary = new ConciliationGatewaySummaryDto
                {
                    Gateway = entity.ConciliationGatewaySummary.Gateway,
                    ConciliationGatewayId = entity.ConciliationGatewaySummary.ConciliationGatewayId,
                    State = (ConciliationStateDto)(int)entity.ConciliationGatewaySummary.State
                },
                ConciliationVisaNetSummary = new ConciliationVisaNetSummaryDto
                {
                    ConciliationVisaNetId = entity.ConciliationVisaNetSummary.ConciliationVisaNetId,
                    State = (ConciliationStateDto)(int)entity.ConciliationVisaNetSummary.State
                },
                ConciliationBatchSummary = new ConciliationBatchSummaryDto
                {
                    ConciliationVisaNetCallbackId = entity.ConciliationBatchSummary.ConciliationVisaNetCallbackId,
                    State = (ConciliationStateDto)(int)entity.ConciliationBatchSummary.State
                },
                State = (ConciliationStateDto)(int)entity.State,
                TransactionType = entity.TransactionType,
                ConciliationPortalState = (ConciliationStateDto)(int)entity.ConciliationPortalState
            };

            return dto;
        }

        public override ConciliationSummary Converter(ConciliationSummaryDto entity)
        {
            var conciliation = new ConciliationSummary
            {
                Id = entity.Id,
                Date = entity.Date,
                TransactionNumber = entity.TransactionNumber,
                VisaTransactionId = entity.VisaTransactionId,
                Type = (ConciliationType)(int)entity.Type,
                GeneralComment = entity.GeneralComment,
                ConciliationCybersourceSummary = new ConciliationCybersourceSummary
                {
                    ConciliationCybersourceId = entity.ConciliationCybersourceSummary.ConciliationCybersourceId,
                    State = (ConciliationState)(int)entity.ConciliationCybersourceSummary.State
                },
                ConciliationGatewaySummary = new ConciliationGatewaySummary
                {
                    Gateway = entity.ConciliationGatewaySummary.Gateway,
                    ConciliationGatewayId = entity.ConciliationGatewaySummary.ConciliationGatewayId,
                    State = (ConciliationState)(int)entity.ConciliationGatewaySummary.State
                },
                ConciliationVisaNetSummary = new ConciliationVisaNetSummary
                {
                    ConciliationVisaNetId = entity.ConciliationVisaNetSummary.ConciliationVisaNetId,
                    State = (ConciliationState)(int)entity.ConciliationVisaNetSummary.State
                },
                ConciliationBatchSummary = new ConciliationBatchSummary
                {
                    ConciliationVisaNetCallbackId = entity.ConciliationBatchSummary.ConciliationVisaNetCallbackId,
                    State = (ConciliationState)(int)entity.ConciliationBatchSummary.State
                },
                State = (ConciliationState)(int)entity.State,
                TransactionType = entity.TransactionType,
                ConciliationPortalState = (ConciliationState)(int)entity.ConciliationPortalState
            };
            return conciliation;
        }

        public override void Edit(ConciliationSummaryDto entity)
        {
            var entity_db = Repository.GetById(entity.Id);

            entity_db.Date = entity.Date;
            entity_db.TransactionNumber = entity.TransactionNumber;
            entity_db.VisaTransactionId = entity.VisaTransactionId;
            entity_db.GeneralComment = entity.GeneralComment;
            entity_db.Type = (ConciliationType)(int)entity.Type;

            entity_db.ConciliationCybersourceSummary.ConciliationCybersourceId = entity.ConciliationCybersourceSummary.ConciliationCybersourceId;
            entity_db.ConciliationCybersourceSummary.State = (ConciliationState)(int)entity.ConciliationCybersourceSummary.State;

            entity_db.ConciliationGatewaySummary.Gateway = entity.ConciliationGatewaySummary.Gateway;
            entity_db.ConciliationGatewaySummary.ConciliationGatewayId = entity.ConciliationGatewaySummary.ConciliationGatewayId;
            entity_db.ConciliationGatewaySummary.State = (ConciliationState)(int)entity.ConciliationGatewaySummary.State;

            entity_db.ConciliationVisaNetSummary.ConciliationVisaNetId = entity.ConciliationVisaNetSummary.ConciliationVisaNetId;
            entity_db.ConciliationVisaNetSummary.State = (ConciliationState)(int)entity.ConciliationVisaNetSummary.State;

            entity_db.ConciliationBatchSummary.ConciliationVisaNetCallbackId = entity.ConciliationBatchSummary.ConciliationVisaNetCallbackId;
            entity_db.ConciliationBatchSummary.State = (ConciliationState)(int)entity.ConciliationBatchSummary.State;

            entity_db.State = (ConciliationState)(int)entity.State;

            entity_db.TransactionType = (TransactionType)(int)entity.TransactionType;

            entity_db.ConciliationPortalState = (ConciliationState)(int)entity.ConciliationPortalState;

            Repository.Edit(entity_db);
            Repository.Save();
        }

        public void GenerateSummary()
        {
            _repositoryConciliationSummary.GenerateSummary();
        }

        public IList<ReportConciliationDetailDto> GetDataForTable(ReportsConciliationFilterDto filtersDto)
        {
            return _repositoryConciliationSummary.ObtainTableInfo(filtersDto.From, filtersDto.To, filtersDto.RequestId, filtersDto.UniqueIdenfifier,
                filtersDto.ServiceId, filtersDto.Email, filtersDto.State, null, filtersDto.Applications, filtersDto.Comments, filtersDto.SortDirection.ToString(),
                filtersDto.DisplayLength.HasValue ? filtersDto.DisplayLength.Value.ToString() : "1", filtersDto.DisplayStart.ToString()).
                Select(ConvertReportConciliationDetailDto).ToList();
        }

        private ReportConciliationDetailDto ConvertReportConciliationDetailDto(ReportConciliationDetail entity)
        {
            return new ReportConciliationDetailDto()
            {
                TransactionType = entity.TransactionType,
                ConciliationCybersourceSummaryConciliationCybersourceId = entity.ConciliationCybersourceSummaryConciliationCybersourceId,
                ConciliationCybersourceSummaryState = entity.ConciliationCybersourceSummaryState,
                ConciliationGatewaySummaryConciliationGatewayId = entity.ConciliationGatewaySummaryConciliationGatewayId,
                ConciliationGatewaySummaryGateway = entity.ConciliationGatewaySummaryGateway,
                ConciliationGatewaySummaryState = entity.ConciliationGatewaySummaryState,
                ConciliationVisaNetSummaryConciliationVisaNetId = entity.ConciliationVisaNetSummaryConciliationVisaNetId,
                ConciliationVisaNetSummaryState = entity.ConciliationVisaNetSummaryState,
                ConciliationBatchSummaryConciliationVisaNetCallbackId = entity.ConciliationBatchSummaryConciliationVisaNetCallbackId,
                ConciliationBatchSummaryState = entity.ConciliationBatchSummaryState,
                Date = entity.Date,
                GeneralComment = entity.GeneralComment,
                Id = entity.Id,
                State = entity.State,
                TransactionNumber = entity.TransactionNumber,
                Type = entity.Type,
                VisaTransactionId = entity.VisaTransactionId,
                RowsCount = entity.RowsCount,
                ConciliationPortalState = entity.ConciliationPortalState,
            };
        }

    }
}