using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisaNet.Domain.EntitiesDtos.TableFilters.Base;

namespace VisaNet.Domain.EntitiesDtos.TableFilters
{
    public class ReportsUserVonFilterDto : BaseFilter
    {
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public string Email { get; set; }
        public string Service { get; set; }

        public override IDictionary<string, string> GetFilterDictionary()
        {
            return new Dictionary<string, string>
            {
                {"DateFrom",DateFrom.ToString("yyyy-MM-dd")},
                {"DateTo",DateTo.ToString("yyyy-MM-dd")},
                {"Email",Email},
                {"Service",Service},

                {"GenericSearch",GenericSearch},
                {"DisplayStart",DisplayStart.ToString()},
                {"DisplayLength",DisplayLength.ToString()},
                {"OrderBy",OrderBy},
                {"SortDirection",SortDirection.ToString()}
            };
        }
    }
}
