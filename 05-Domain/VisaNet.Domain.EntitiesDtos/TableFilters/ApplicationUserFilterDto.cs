using System.Collections.Generic;
using VisaNet.Domain.EntitiesDtos.TableFilters.Base;

namespace VisaNet.Domain.EntitiesDtos.TableFilters
{
    public class ApplicationUserFilterDto : BaseFilter
    {
        public ApplicationUserFilterDto()
        {
            OrderBy = "Name";
        }

        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        
        public override IDictionary<string, string> GetFilterDictionary()
        {
            return new Dictionary<string, string>
            {
                {"UserName",Name},
                {"UserName",Surname},
                {"UserName",Email},

                {"GenericSearch",GenericSearch},
                {"DisplayStart",DisplayStart.ToString()},
                {"DisplayLength",DisplayLength.ToString()},
                {"OrderBy",OrderBy},
                {"SortDirection",SortDirection.ToString()},
            };
        }
    }
}
