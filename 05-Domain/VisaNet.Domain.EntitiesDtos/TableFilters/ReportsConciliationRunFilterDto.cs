using System;
using System.Collections.Generic;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Domain.EntitiesDtos.TableFilters.Base;

namespace VisaNet.Domain.EntitiesDtos.TableFilters
{
    public class ReportsConciliationRunFilterDto : BaseFilter
    {
        public ReportsConciliationRunFilterDto()
        {
            OrderBy = "CreationDate";
            SortDirection = SortDirection.Desc;
        }

        public DateTime CreationDateFrom { get; set; }
        public DateTime CreationDateTo { get; set; }
        public ConciliationAppDto? App { get; set; }
        public bool? IsManualRun { get; set; }
        public ConciliationRunStateDto? State { get; set; }
        public string InputFileName { get; set; }
        public DateTime? ConciliationDateFrom { get; set; }
        public DateTime? ConciliationDateTo { get; set; }

        public override IDictionary<string, string> GetFilterDictionary()
        {
            return new Dictionary<string, string>
            {
                {"CreationDateFrom", CreationDateFrom.ToString("yyyy-MM-dd")},
                {"CreationDateTo", CreationDateTo.ToString("yyyy-MM-dd")},
                {"App", App.ToString()},
                {"IsManualRun", IsManualRun.ToString()},
                {"State", State.ToString()},
                {"InputFileName", InputFileName},
                {"ConciliationDateFrom", ConciliationDateFrom.HasValue ? ConciliationDateFrom.Value.ToString("yyyy-MM-dd") : null},
                {"ConciliationDateTo", ConciliationDateTo.HasValue ? ConciliationDateTo.Value.ToString("yyyy-MM-dd") : null},

                {"GenericSearch", GenericSearch},
                {"DisplayStart", DisplayStart.ToString()},
                {"DisplayLength", DisplayLength.ToString()},
                {"OrderBy", OrderBy},
                {"SortDirection", SortDirection.ToString()},
            };
        }

    }
}