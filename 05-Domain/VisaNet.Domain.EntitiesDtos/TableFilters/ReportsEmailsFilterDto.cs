using System;
using System.Collections.Generic;
using VisaNet.Domain.EntitiesDtos.TableFilters.Base;

namespace VisaNet.Domain.EntitiesDtos.TableFilters
{
    public class ReportsEmailsFilterDto : BaseFilter
    {
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public string DateFromString { get; set; }
        public string DateToString { get; set; }
        public int Status { get; set; }
        public string To { get; set; }
        public int EmailType { get; set; }

        public override IDictionary<string, string> GetFilterDictionary()
        {
            return new Dictionary<string, string>
            {
                {"DateFrom",DateFrom.ToString()},
                {"DateTo",DateTo.ToString()},
                {"DateFromString",DateFromString},
                {"DateToString",DateToString},
                {"Status",Status.ToString()},
                {"EmailType",EmailType.ToString()},
                {"To",To},
                
                {"GenericSearch",GenericSearch},
                {"DisplayStart",DisplayStart.ToString()},
                {"DisplayLength",DisplayLength.ToString()},
                {"OrderBy",OrderBy},
                {"SortDirection",SortDirection.ToString()}
            };
        }
    }
}