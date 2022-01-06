using System;
using System.Collections.Generic;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Domain.EntitiesDtos.TableFilters.Base;

namespace VisaNet.Domain.EntitiesDtos.TableFilters
{
    public class ReportsUserFilterDto : BaseFilter
    {
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public string Email { get; set; }
        public UserType UserType { get; set; }
        public ActiveOrInactiveEnumDto ActiveOrInactive { get; set; }

        public override IDictionary<string, string> GetFilterDictionary()
        {
            return new Dictionary<string, string>
            {
                {"DateFrom",DateFrom.ToString()},
                {"DateTo",DateTo.ToString()},
                {"Email",Email},
                {"UserType",UserType.ToString()},
                {"ActiveOrInactive",ActiveOrInactive.ToString()},

                {"GenericSearch",GenericSearch},
                {"DisplayStart",DisplayStart.ToString()},
                {"DisplayLength",DisplayLength.ToString()},
                {"OrderBy",OrderBy},
                {"SortDirection",SortDirection.ToString()}
            };
        }
    }
}