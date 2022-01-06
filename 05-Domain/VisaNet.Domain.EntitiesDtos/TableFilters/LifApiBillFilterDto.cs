using System;
using System.Collections.Generic;
using VisaNet.Domain.EntitiesDtos.TableFilters.Base;

namespace VisaNet.Domain.EntitiesDtos.TableFilters
{
    public class LifApiBillFilterDto : BaseFilter
    {
        public LifApiBillFilterDto()
        {
            OrderBy = "Name";
        }

        public string IdOperation { get; set; }
        public string IdApp { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public string DateFromString { get; set; }
        public string DateToString { get; set; }
        public string BinValue { get; set; }
        public string LawIndi { get; set; }

        public override IDictionary<string, string> GetFilterDictionary()
        {
            return new Dictionary<string, string>
            {
                {"IdOperation", IdOperation},
                {"IdApp", IdApp},
                {"DateFrom", DateFrom.ToString()},
                {"DateTo", DateTo.ToString()},
                {"DateFromString", DateFromString},
                {"DateToString", DateToString},
                {"BinValue", BinValue},
                {"LawIndi", LawIndi},

                {"GenericSearch",GenericSearch},
                {"DisplayStart",DisplayStart.ToString()},
                {"DisplayLength",DisplayLength.ToString()},
                {"OrderBy",OrderBy},
                {"SortDirection",SortDirection.ToString()},
            };
        }
    }
}
