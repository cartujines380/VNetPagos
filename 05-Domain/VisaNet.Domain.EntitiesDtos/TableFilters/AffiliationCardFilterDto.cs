using System;
using System.Collections.Generic;
using VisaNet.Domain.EntitiesDtos.TableFilters.Base;

namespace VisaNet.Domain.EntitiesDtos.TableFilters
{
    public class AffiliationCardFilterDto : BaseFilter
    {
        public string Name { get; set; }
        public int Code { get; set; }
        public Guid BankId { get; set; }
        public bool? Active { get; set; }
        public override IDictionary<string, string> GetFilterDictionary()
        {

            return new Dictionary<string, string>
            {
                {"Name",Name},
                {"BankId",BankId.ToString()},
                {"Code",Code.ToString()},
                 {"Active",Active != null ? Active.ToString() : null},

                {"GenericSearch",GenericSearch},
                {"DisplayStart",DisplayStart.ToString()},
                {"DisplayLength",DisplayLength.ToString()},
                {"OrderBy",OrderBy},
                {"SortDirection",SortDirection.ToString()},
            };
        }
    }
}
