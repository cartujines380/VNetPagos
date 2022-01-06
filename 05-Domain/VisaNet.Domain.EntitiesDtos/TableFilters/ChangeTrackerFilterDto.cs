using System;
using System.Collections.Generic;
using VisaNet.Domain.EntitiesDtos.ChangeTracker;
using VisaNet.Domain.EntitiesDtos.TableFilters.Base;

namespace VisaNet.Domain.EntitiesDtos.TableFilters
{
    public class ChangeTrackerFilterDto : BaseFilter
    {
        public ChangeTrackerFilterDto()
        {
            From = DateTime.Today.Date.AddDays(-1);
            To = DateTime.Today.Date;
            OrderBy = "";
        }

        public DateTime From { get; set; }
        public int HoursFrom { get; set; }
        public int MinutesFrom { get; set; }
        public DateTime To { get; set; }
        public int HoursTo { get; set; }
        public int MinutesTo { get; set; }
        public string UserName { get; set; }
        public EventTypeDto? EventType { get; set; }
        public string TableName { get; set; }
        public string AditionalInfo { get; set; }

        public override IDictionary<string, string> GetFilterDictionary()
        {
            return new Dictionary<string, string>
            {
                {"UserName", UserName},
                {"From", From.ToString()},
                {"To", To.ToString()},
                {"EventType", EventType.HasValue ?  ((int) EventType).ToString() : ((int?)null).ToString()},
                {"TableName", TableName},
                {"HoursFrom", HoursFrom.ToString()},
                {"HoursTo", HoursTo.ToString()},
                {"MinutesFrom", MinutesFrom.ToString()},
                {"MinutesTo", MinutesTo.ToString()},
                {"AditionalInfo", AditionalInfo},
                {"GenericSearch", GenericSearch},
                {"DisplayStart", DisplayStart.ToString()},
                {"DisplayLength", DisplayLength.ToString()},
                {"OrderBy", OrderBy},
                {"SortDirection", SortDirection.ToString()},
            };
        }
    }
}