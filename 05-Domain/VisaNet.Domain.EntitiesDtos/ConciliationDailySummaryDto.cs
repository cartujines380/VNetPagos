using System;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Domain.EntitiesDtos
{
    public class ConciliationDailySummaryDto
    {
        public DateTime Date { get; set; }
        public int TotalPortal { get; set; }
        public int TotalExternas { get; set; }

        public ConciliationStateDto CybersourceState { get; set; }
        public ConciliationStateDto BanredState { get; set; }
        public ConciliationStateDto SistarbancState { get; set; }
        public ConciliationStateDto SuciveState { get; set; }
        public ConciliationStateDto Tc33State { get; set; }
        public ConciliationStateDto BatchState { get; set; }
        public ConciliationStateDto PortalState { get; set; }

        //Site
        public int SiteRojas { get; set; }
        public int SiteAmarillas { get; set; }
        public int SiteVerdes { get; set; }
        public int SiteAzules { get; set; }
        public int SiteNoaplica { get; set; }

        //CyberSource
        public int CsRojas { get; set; }
        public int CsAmarillas { get; set; }
        public int CsVerdes { get; set; }
        public int CsAzules { get; set; }
        public int CsDetailVNP { get; set; }
        public int CsDetailTotalCs { get; set; }
        public int CsDetailOk { get; set; }
        public int CsDetailDif { get; set; }
        public int CsDetailRev { get; set; }
        public int CsDetailSiteNoCs { get; set; }
        public int CsDetailCsNoSite { get; set; }

        //Banred
        public int BrRojas { get; set; }
        public int BrAmarillas { get; set; }
        public int BrVerdes { get; set; }
        public int BrAzules { get; set; }
        public int BrDetailVNP { get; set; }
        public int BrDetailTotalBr { get; set; }
        public int BrDetailOk { get; set; }
        public int BrDetailDif { get; set; }
        public int BrDetailRev { get; set; }
        public int BrDetailSiteNoBr { get; set; }
        public int BrDetailBrNoSite { get; set; }

        //Sistarbanc
        public int SbRojas { get; set; }
        public int SbAmarillas { get; set; }
        public int SbVerdes { get; set; }
        public int SbAzules { get; set; }
        public int SbDetailVNP { get; set; }
        public int SbDetailTotalSb { get; set; }
        public int SbDetailOk { get; set; }
        public int SbDetailDif { get; set; }
        public int SbDetailRev { get; set; }
        public int SbDetailSiteNoSb { get; set; }
        public int SbDetailSbNoSite { get; set; }

        //Sucive
        public int SuRojas { get; set; }
        public int SuAmarillas { get; set; }
        public int SuVerdes { get; set; }
        public int SuAzules { get; set; }
        public int SuDetailVNP { get; set; }
        public int SuDetailTotalSu { get; set; }
        public int SuDetailOk { get; set; }
        public int SuDetailDif { get; set; }
        public int SuDetailRev { get; set; }
        public int SuDetailSiteNoSu { get; set; }
        public int SuDetailSuNoSite { get; set; }

        //TC33
        public int TcRojas { get; set; }
        public int TcAmarillas { get; set; }
        public int TcVerdes { get; set; }
        public int TcAzules { get; set; }
        public int TcDetailVNP { get; set; }
        public int TcDetailTotalTc { get; set; }
        public int TcDetailOk { get; set; }
        public int TcDetailDif { get; set; }
        public int TcDetailRev { get; set; }
        public int TcDetailSiteNoTc { get; set; }
        public int TcDetailTcNoSite { get; set; }
        public int TcDetailExtVNP { get; set; }
        public int TcDetailExtTotalTc { get; set; }
        public int TcDetailExtOk { get; set; }
        public int TcDetailExtDif { get; set; }
        public int TcDetailExtRev { get; set; }
        public int TcDetailExtSiteNoTc { get; set; }
        public int TcDetailTcExtNoSite { get; set; }

        //Batch
        public int BatRojas { get; set; }
        public int BatAmarillas { get; set; }
        public int BatVerdes { get; set; }
        public int BatAzules { get; set; }
        public int BatDetailVNP { get; set; }
        public int BatDetailTotalBat { get; set; }
        public int BatDetailOk { get; set; }
        public int BatDetailDif { get; set; }
        public int BatDetailRev { get; set; }
        public int BatDetailSiteNoBat { get; set; }
        public int BatDetailBatNoSite { get; set; }
    }
}