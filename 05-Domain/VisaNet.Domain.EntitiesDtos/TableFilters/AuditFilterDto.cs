using System;
using System.Collections.Generic;
using VisaNet.Common.Logging.Entities;
using VisaNet.Domain.EntitiesDtos.TableFilters.Base;

namespace VisaNet.Domain.EntitiesDtos.TableFilters
{
    public class AuditFilterDto : BaseFilter
    {
        public AuditFilterDto()
        {
            From = DateTime.Today.Date.AddDays(-1);
            To = DateTime.Today.Date;
            OrderBy = "";
        }

        public Guid? SystemUserId { get; set; }
        public Guid? UserId { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public string Ip { get; set; }
        public string Message { get; set; }
        public int HoursFrom { get; set; }
        public int HoursTo { get; set; }
        public int MinutesFrom { get; set; }
        public int MinutesTo { get; set; }

        public LogType? LogType { get; set; }
        public LogUserType? LogUserType { get; set; }
        public LogCommunicationType? LogCommunicationType { get; set; }
        public LogOperationType? LogOperationType { get; set; }
        public string User { get; set; }


        public override IDictionary<string, string> GetFilterDictionary()
        {
            return new Dictionary<string, string>
            {
                {"SystemUserId", SystemUserId.HasValue ? SystemUserId.ToString() : ((Guid?) null).ToString()},
                {"UserId", UserId.HasValue ? UserId.ToString() : ((Guid?) null).ToString()},
                {"From", From.ToString()},
                {"To", To.ToString()},
                {"Ip", Ip.ToString()},
                {"LogType", LogType.HasValue ?  ((int) LogType).ToString() : ((int?)null).ToString()},
                {"LogUserType", LogUserType.HasValue ?  ((int) LogUserType).ToString() : ((int?)null).ToString()},
                {"LogCommunicationType", LogCommunicationType.HasValue ?  ((int) LogCommunicationType).ToString() : ((int?)null).ToString()},
                {"LogOperationType", LogOperationType.HasValue ?  ((int) LogOperationType).ToString() : ((int?)null).ToString()},
                {"User", User},
                {"Message", Message},
                {"HoursFrom", HoursFrom.ToString()},
                {"HoursTo", HoursTo.ToString()},
                {"MinutesFrom", MinutesFrom.ToString()},
                {"MinutesTo", MinutesTo.ToString()},

                {"GenericSearch", GenericSearch},
                {"DisplayStart", DisplayStart.ToString()},
                {"DisplayLength", DisplayLength.ToString()},
                {"OrderBy", OrderBy},
                {"SortDirection", SortDirection.ToString()},
            };
        }
    }
}
