using System.Collections.Generic;
using VisaNet.Domain.EntitiesDtos.TableFilters.Base;

namespace VisaNet.Domain.EntitiesDtos.TableFilters
{
    public class RoleFilterDto : BaseFilter
    {
        public RoleFilterDto()
        {
            OrderBy = "Name";
        }

        public string Name { get; set; }

        public override IDictionary<string, string> GetFilterDictionary()
        {
            return new Dictionary<string, string>
            {
                {"Name", Name},

                {"GenericSearch", GenericSearch},
                {"DisplayStart", DisplayStart.ToString()},
                {"DisplayLength", DisplayLength.ToString()},
                {"OrderBy", OrderBy},
                {"SortDirection", SortDirection.ToString()},
            };
        }
    }
}
