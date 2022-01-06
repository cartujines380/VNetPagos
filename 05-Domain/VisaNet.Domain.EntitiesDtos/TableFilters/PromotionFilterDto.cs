using System;
using System.Collections.Generic;
using VisaNet.Domain.EntitiesDtos.TableFilters.Base;

namespace VisaNet.Domain.EntitiesDtos.TableFilters
{
    public class PromotionFilterDto : BaseFilter
    {
        public PromotionFilterDto()
        {
            OrderBy = "Name";
        }
        public string Name { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        
        public override IDictionary<string, string> GetFilterDictionary()
        {
            return new Dictionary<string, string>
            {
                {"Name",Name},
                {"From", From.ToString()},
                {"To", To.ToString()},

                {"GenericSearch",GenericSearch},
                {"DisplayStart",DisplayStart.ToString()},
                {"DisplayLength",DisplayLength.ToString()},
                {"OrderBy",OrderBy},
                {"SortDirection",SortDirection.ToString()},
            };
        }
    }
}
