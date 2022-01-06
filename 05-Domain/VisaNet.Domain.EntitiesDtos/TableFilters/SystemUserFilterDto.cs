using System.Collections.Generic;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Domain.EntitiesDtos.TableFilters.Base;

namespace VisaNet.Domain.EntitiesDtos.TableFilters
{
    public class SystemUserFilterDto : BaseFilter
    {
        public SystemUserFilterDto()
        {
            OrderBy = "UserName";
        }

        public string UserName { get; set; }
        public bool? Enabled { get; set; }
        public SystemUserTypeDto SystemUserType { get; set; }


        public override IDictionary<string, string> GetFilterDictionary()
        {
            return new Dictionary<string, string>
            {
                {"UserName",UserName},
                {"SystemUserType",SystemUserType.ToString()},
                {"Enabled",Enabled.ToString()},

                {"GenericSearch",GenericSearch},
                {"DisplayStart",DisplayStart.ToString()},
                {"DisplayLength",DisplayLength.ToString()},
                {"OrderBy",OrderBy},
                {"SortDirection",SortDirection.ToString()},
            };
        }
    }
}
