using System;
using System.Collections.Generic;
using VisaNet.Domain.EntitiesDtos.TableFilters.Base;

namespace VisaNet.Domain.EntitiesDtos.TableFilters
{
    public class ReportsTc33FilterDto : BaseFilter
    {
        public ReportsTc33FilterDto()
        {
            OrderBy = "CreationDate";
        }

        public string InputFileName { get; set; }
        public string Transaction { get; set; }
        public DateTime CreationDateFrom { get; set; }
        public string CreationDateFromString { get; set; }
        public DateTime CreationDateTo { get; set; }
        public string CreationDateToString { get; set; }

        public override IDictionary<string, string> GetFilterDictionary()
        {
            return new Dictionary<string, string>
            {
                {"InputFileName", InputFileName},
                {"Transaction", Transaction},
                {"CreationDateFrom", CreationDateFrom.ToString()},
                {"CreationDateTo", CreationDateTo.ToString()},
                {"CreationDateFromString", CreationDateFromString},
                {"CreationDateToString", CreationDateToString},

                {"GenericSearch",GenericSearch},
                {"DisplayStart",DisplayStart.ToString()},
                {"DisplayLength",DisplayLength.ToString()},
                {"OrderBy",OrderBy},
                {"SortDirection",SortDirection.ToString()},
            };
        }
    }
}
