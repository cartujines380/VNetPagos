using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Presentation.Administration.Models
{
    public class DailyConciliationDetailModel
    {
        public int TotalVnp { get; set; }
        public int TotalGetaway { get; set; }
        public int TotalOk { get; set; }
        public int TotalDiff { get; set; }
        public int SiteNoGetaway { get; set; }
        public int GetawayNoSite { get; set; }
        public string GetawayName { get; set; }
        public int TotalChecked { get; set; }
        public ConciliationAppDto ConciliationApp { get; set; }
    }
}