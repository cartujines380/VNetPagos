using System;
using System.Collections.Generic;
using VisaNet.Domain.EntitiesDtos.TableFilters.Base;

namespace VisaNet.Domain.EntitiesDtos.TableFilters
{
    public class BankFilterDto : BaseFilter
    {
        public BankFilterDto()
        {
            OrderBy = "Name";
        }

        public string Name { get; set; }
        public int Code { get; set; }
        public int BinValue { get; set; }

        public override IDictionary<string, string> GetFilterDictionary()
        {
            return new Dictionary<string, string>
            {
                {"Name",Name},
                {"BinValue",BinValue.ToString()},
                {"Code",Code.ToString()},

                {"GenericSearch",GenericSearch},
                {"DisplayStart",DisplayStart.ToString()},
                {"DisplayLength",DisplayLength.ToString()},
                {"OrderBy",OrderBy},
                {"SortDirection",SortDirection.ToString()},
            };
        }
    }
}
