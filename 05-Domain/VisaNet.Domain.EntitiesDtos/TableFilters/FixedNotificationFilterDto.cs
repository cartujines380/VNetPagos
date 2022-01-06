using System;
using System.Collections.Generic;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Domain.EntitiesDtos.TableFilters.Base;

namespace VisaNet.Domain.EntitiesDtos.TableFilters
{
    public class FixedNotificationFilterDto : BaseFilter
    {
        public FixedNotificationFilterDto()
        {
            OrderBy = "From";
        }

        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public string Description { get; set; }
        public string Detail { get; set; }
        public bool? Resolved { get; set; }
        public FixedNotificationLevelDto? Level { get; set; }
        public FixedNotificationCategoryDto? Category { get; set; }

        public override IDictionary<string, string> GetFilterDictionary()
        {
            return new Dictionary<string, string>
            {
                {"From", From.ToString()},
                {"To", To.ToString()},
                {"Description", Description},
                {"Detail", Detail},
                {"Resolved", Resolved.ToString()},
                {"Level", Level.ToString()},
                {"Category", Category.ToString()},
                {"DisplayStart",DisplayStart.ToString()},
                {"DisplayLength",DisplayLength.ToString()},
                {"OrderBy",OrderBy},
                {"SortDirection",SortDirection.ToString()},
            };
        }
    }

    public class ResolveAllFixedDto
    {
        public FixedNotificationFilterDto Filter { get; set; }
        public string Comment { get; set; }
    }
}
