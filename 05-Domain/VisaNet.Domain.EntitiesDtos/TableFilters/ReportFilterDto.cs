using System;
using System.Collections.Generic;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Domain.EntitiesDtos.TableFilters.Base;

namespace VisaNet.Domain.EntitiesDtos.TableFilters
{
    public class ReportFilterDto : BaseFilter
    {
        public ReportFilterDto()
        {
            OrderBy = "Date";
            SortDirection = SortDirection.Desc;
        }

        public Guid UserId { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public string Categories { get; set; }
        public ReportFilterGrouper GroupedBy { get; set; }
        public override IDictionary<string, string> GetFilterDictionary()
        {
            throw new NotImplementedException();
        }
    }

    
}