using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Presentation.Administration.Models
{
    public class DailyConciliationModel
    {
        public ConciliationAppDto App { get; set; }
        public ConciliationDailySummaryDto Detail { get; set; }
    }
}