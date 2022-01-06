using System;
using System.Collections.Generic;
using VisaNet.Domain.EntitiesDtos.TableFilters.Base;

namespace VisaNet.Domain.EntitiesDtos.TableFilters
{
    public class ReportsConciliationFilterDto : BaseFilter
    {
        public ReportsConciliationFilterDto()
        {
            OrderBy = "From";
        }

        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public string RequestId { get; set; }
        public string UniqueIdenfifier { get; set; }
        public string Comments { get; set; }
        public string ServiceId { get; set; }
        public string Email { get; set; }
        public string State { get; set; }
        public string Applications { get; set; }
        public int Origin { get; set; }

        public override IDictionary<string, string> GetFilterDictionary()
        {
            return new Dictionary<string, string>
            {
                {"RequestId",RequestId},
                {"UniqueIdenfifier",UniqueIdenfifier},
                {"From", From.ToString()},
                {"To", To.ToString()},
                {"Comments",Comments},
                {"ServiceId",ServiceId},
                {"Email",Email},
                {"Applications",Applications},
                {"State",State},
                {"Origin", Origin.ToString()},

                {"DisplayStart",DisplayStart.ToString()},
                {"DisplayLength",DisplayLength.ToString()},
                {"OrderBy",OrderBy},
                {"SortDirection",SortDirection.ToString()},
            };
        }
    }
}
