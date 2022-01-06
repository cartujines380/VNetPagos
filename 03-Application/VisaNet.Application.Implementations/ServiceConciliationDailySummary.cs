using System;
using System.Collections.Generic;
using System.Linq;
using VisaNet.Application.Interfaces;
using VisaNet.Domain.Entities;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Repository.Interfaces.Interfaces;

namespace VisaNet.Application.Implementations
{
    public class ServiceConciliationDailySummary : IServiceConciliationDailySummary
    {
        private readonly IRepositoryConciliationDailySummary _conciliationDailyRepository;

        public ServiceConciliationDailySummary(IRepositoryConciliationDailySummary conciliationDailyRepository)
        {
            _conciliationDailyRepository = conciliationDailyRepository;
        }

        public ICollection<ConciliationDailySummaryDto> GetConciliationDailySummary(DailyConciliationFilterDto filter)
        {
            var data = _conciliationDailyRepository.GetConciliationDailySummary(filter.From, filter.To).Select(CreateDto);
            return data.Where(Filter(filter)).ToList();
        }

        private ConciliationDailySummaryDto CreateDto(ConciliationDailySummary entity)
        {
            return new ConciliationDailySummaryDto
            {
                Date = entity.Date,
                TotalPortal = entity.TotalPortal,
                TotalExternas = entity.TotalExternas,
                PortalState = GetState(entity.SiteRojas, entity.SiteAmarillas, entity.SiteVerdes, entity.SiteAzules),
                CybersourceState = GetState(entity.CsRojas, entity.CsAmarillas, entity.CsVerdes, entity.CsAzules),
                BanredState = GetState(entity.BrRojas, entity.BrAmarillas, entity.BrVerdes, entity.BrAzules),
                SistarbancState = GetState(entity.SbRojas, entity.SbAmarillas, entity.SbVerdes, entity.SbAzules),
                SuciveState = GetState(entity.SuRojas, entity.SuAmarillas, entity.SuVerdes, entity.SuAzules),
                Tc33State = GetState(entity.TcRojas, entity.TcAmarillas, entity.TcVerdes, entity.TcAzules),
                BatchState = GetState(entity.BatRojas, entity.BatAmarillas, entity.BatVerdes, entity.BatAzules),

                SiteRojas = entity.SiteRojas,
                SiteAmarillas = entity.SiteAmarillas,
                SiteVerdes = entity.SiteVerdes,
                SiteAzules = entity.SiteAzules,
                SiteNoaplica = entity.SiteNoaplica,

                CsRojas = entity.CsRojas,
                CsAmarillas = entity.CsAmarillas,
                CsVerdes = entity.CsVerdes,
                CsAzules = entity.CsAzules,
                CsDetailVNP = entity.CsDetailVNP,
                CsDetailTotalCs = entity.CsDetailTotalCs,
                CsDetailOk = entity.CsDetailOk,
                CsDetailDif = entity.CsDetailDif,
                CsDetailRev = entity.CsDetailRev,
                CsDetailSiteNoCs = entity.CsDetailSiteNoCs,
                CsDetailCsNoSite = entity.CsDetailCsNoSite,

                BrRojas = entity.BrRojas,
                BrAmarillas = entity.BrAmarillas,
                BrVerdes = entity.BrVerdes,
                BrAzules = entity.BrAzules,
                BrDetailVNP = entity.BrDetailVNP,
                BrDetailTotalBr = entity.BrDetailTotalBr,
                BrDetailOk = entity.BrDetailOk,
                BrDetailDif = entity.BrDetailDif,
                BrDetailRev = entity.BrDetailRev,
                BrDetailSiteNoBr = entity.BrDetailSiteNoBr,
                BrDetailBrNoSite = entity.BrDetailBrNoSite,

                SbRojas = entity.SbRojas,
                SbAmarillas = entity.SbAmarillas,
                SbVerdes = entity.SbVerdes,
                SbAzules = entity.SbAzules,
                SbDetailVNP = entity.SbDetailVNP,
                SbDetailTotalSb = entity.SbDetailTotalSb,
                SbDetailOk = entity.SbDetailOk,
                SbDetailDif = entity.SbDetailDif,
                SbDetailRev = entity.SbDetailRev,
                SbDetailSiteNoSb = entity.SbDetailSiteNoSb,
                SbDetailSbNoSite = entity.SbDetailSbNoSite,

                SuRojas = entity.SuRojas,
                SuAmarillas = entity.SuAmarillas,
                SuVerdes = entity.SuVerdes,
                SuAzules = entity.SuAzules,
                SuDetailVNP = entity.SuDetailVNP,
                SuDetailTotalSu = entity.SuDetailTotalSu,
                SuDetailOk = entity.SuDetailOk,
                SuDetailDif = entity.SuDetailDif,
                SuDetailRev = entity.SuDetailRev,
                SuDetailSiteNoSu = entity.SuDetailSiteNoSu,
                SuDetailSuNoSite = entity.SuDetailSuNoSite,

                TcRojas = entity.TcRojas,
                TcAmarillas = entity.TcAmarillas,
                TcVerdes = entity.TcVerdes,
                TcAzules = entity.TcAzules,
                TcDetailVNP = entity.TcDetailVNP,
                TcDetailTotalTc = entity.TcDetailTotalTc,
                TcDetailOk = entity.TcDetailOk,
                TcDetailDif = entity.TcDetailDif,
                TcDetailRev = entity.TcDetailRev,
                TcDetailSiteNoTc = entity.TcDetailSiteNoTc,
                TcDetailTcNoSite = entity.TcDetailTcNoSite,
                TcDetailExtVNP = entity.TcDetailExtVNP,
                TcDetailExtTotalTc = entity.TcDetailExtTotalTc,
                TcDetailExtOk = entity.TcDetailExtOk,
                TcDetailExtDif = entity.TcDetailExtDif,
                TcDetailExtRev = entity.TcDetailExtRev,
                TcDetailExtSiteNoTc = entity.TcDetailExtSiteNoTc,
                TcDetailTcExtNoSite = entity.TcDetailTcExtNoSite,

                BatRojas = entity.BatRojas,
                BatAmarillas = entity.BatAmarillas,
                BatVerdes = entity.BatVerdes,
                BatAzules = entity.BatAzules,
                BatDetailVNP = entity.BatDetailVNP,
                BatDetailTotalBat = entity.BatDetailTotalBat,
                BatDetailOk = entity.BatDetailOk,
                BatDetailDif = entity.BatDetailDif,
                BatDetailRev = entity.BatDetailRev,
                BatDetailSiteNoBat = entity.BatDetailSiteNoBat,
                BatDetailBatNoSite = entity.BatDetailBatNoSite,
            };
        }

        private ConciliationStateDto GetState(int red, int yellow, int green, int blue)
        {
            if (red > 0)
                return ConciliationStateDto.NotFound;

            if (yellow > 0)
                return ConciliationStateDto.Difference;

            if (green > 0)
                return ConciliationStateDto.Ok;

            if (blue > 0)
                return ConciliationStateDto.Checked;

            return ConciliationStateDto.DoesNotApply;

        }

        private Func<ConciliationDailySummaryDto, bool> Filter(DailyConciliationFilterDto filter)
        {
            if (filter.Applications == null || !filter.Applications.Any())
                return f => true;

            //If All is selected
            if (filter.State == 0)
                return f => true;

            var function = new Func<ConciliationDailySummaryDto, bool>(f =>
                (filter.Applications.Any(a => a == (int)ConciliationAppDto.CyberSource) && f.CybersourceState == (ConciliationStateDto)filter.State)
                || (filter.Applications.Any(a => a == (int)ConciliationAppDto.Banred) && f.BanredState == (ConciliationStateDto)filter.State)
                || (filter.Applications.Any(a => a == (int)ConciliationAppDto.Sistarbanc) && f.SistarbancState == (ConciliationStateDto)filter.State)
                || (filter.Applications.Any(a => a == (int)ConciliationAppDto.Sucive) && f.SuciveState == (ConciliationStateDto)filter.State)
                || (filter.Applications.Any(a => a == (int)ConciliationAppDto.Tc33) && f.Tc33State == (ConciliationStateDto)filter.State)
                || (filter.Applications.Any(a => a == (int)ConciliationAppDto.Batch) && f.BatchState == (ConciliationStateDto)filter.State)
                || (filter.Applications.Any(a => a == (int)ConciliationAppDto.Site) && f.PortalState == (ConciliationStateDto)filter.State)
                );

            return function;
        }

    }
}