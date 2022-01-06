using System;
using System.Collections.Generic;
using VisaNet.Domain.EntitiesDtos.TableFilters.Base;

namespace VisaNet.Domain.EntitiesDtos.TableFilters
{
    public class ReportsIntegrationFilterDto : BaseFilter
    {

        public ReportsIntegrationFilterDto()
        {
            DateTo = DateTime.Now;
            DateFrom = DateTime.Now.AddDays(-7);
            DisplayLength = 10;
        }

        public int ExternalRequestType { get; set; }
        public string IdOperation { get; set; }
        public string IdApp { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public string DateFromString { get; set; }
        public string DateToString { get; set; }
        public string TransactionNumber { get; set; }

        public override IDictionary<string, string> GetFilterDictionary()
        {
            return new Dictionary<string, string>
            {
                {"ExternalRequestType", ExternalRequestType.ToString()},
                {"IdOperation", IdOperation},
                {"IdApp", IdApp},
                {"DateFrom", DateFrom.ToString()},
                {"DateTo", DateTo.ToString()},
                {"DateFromString", DateFromString},
                {"DateToString", DateToString},
                {"TransactionNumber", TransactionNumber},

                {"GenericSearch", GenericSearch},
                {"DisplayStart", DisplayStart.ToString()},
                {"DisplayLength", DisplayLength.ToString()},
                {"OrderBy", OrderBy},
                {"SortDirection", SortDirection.ToString()},
            };
        }
    }
}
