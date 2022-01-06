using System;
using System.Collections.Generic;
using VisaNet.Domain.EntitiesDtos.TableFilters.Base;

namespace VisaNet.Domain.EntitiesDtos.TableFilters
{
    public class InterpreterFilterDto : BaseFilter
    {
        public InterpreterFilterDto()
        {
            OrderBy = "Name";
        }
        public string Name { get; set; }
        public string Description { get; set; }
        public string FileName { get; set; }
         

        public override IDictionary<string, string> GetFilterDictionary()
        {
            return new Dictionary<string, string>
            {
                {"Name",Name},
                {"Description",Description},
                {"FileName",FileName},

                {"GenericSearch",GenericSearch},
                {"DisplayStart",DisplayStart.ToString()},
                {"DisplayLength",DisplayLength.ToString()},
                {"OrderBy",OrderBy},
                {"SortDirection",SortDirection.ToString()},
            };
        }
    }
}
