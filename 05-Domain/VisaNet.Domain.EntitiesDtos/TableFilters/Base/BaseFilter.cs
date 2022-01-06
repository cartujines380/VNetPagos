using System.Collections.Generic;

namespace VisaNet.Domain.EntitiesDtos.TableFilters.Base
{
    public abstract class BaseFilter
    {
        protected BaseFilter()
        {
            DisplayStart = 0;
            DisplayLength = 10;
            SortDirection = SortDirection.Asc;
        }

        public int DisplayStart { get; set; }
        public int? DisplayLength { get; set; }
        public string GenericSearch { get; set; }
        public string OrderBy { get; set; }
        public SortDirection SortDirection { get; set; }

        public abstract IDictionary<string, string> GetFilterDictionary();
    }
}
