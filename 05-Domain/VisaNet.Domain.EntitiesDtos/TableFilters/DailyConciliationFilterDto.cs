using System;
using System.Collections.Generic;
using VisaNet.Domain.EntitiesDtos.TableFilters.Base;

namespace VisaNet.Domain.EntitiesDtos.TableFilters
{
    public class DailyConciliationFilterDto : BaseFilter
    {
        public DailyConciliationFilterDto()
        {
            OrderBy = "From";
            //DisplayLength = 50;
        }

        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public int State { get; set; }
        public List<int> Applications { get; set; }
        public int Origin { get; set; }

        public override IDictionary<string, string> GetFilterDictionary()
        {
            return new Dictionary<string, string>
            {
                {"From", From.ToString()},
                {"To", To.ToString()},
                {"State", State.ToString()},
                //{"Applications", ListToJson(Applications)},
                {"Origin", Origin.ToString()},

                {"DisplayStart",DisplayStart.ToString()},
                {"DisplayLength",DisplayLength.ToString()},
                {"OrderBy",OrderBy},
                {"SortDirection",SortDirection.ToString()},
            };
        }

        //private string ListToJson(List<int> applications)
        //{
        //    return string.Format("[{0}]", string.Join(",", applications));
        //}
    }
}
