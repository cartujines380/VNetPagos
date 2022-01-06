using System;
using System.Collections.Generic;
using VisaNet.Domain.EntitiesDtos.TableFilters.Base;

namespace VisaNet.Domain.EntitiesDtos.TableFilters
{
    public class LogFilterDto : BaseFilter
    {
        public LogFilterDto()
        {
            OrderBy = "DateTime";
        }

        public Guid SelectedUserId { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public int? LogType { get; set; }

        public override IDictionary<string, string> GetFilterDictionary()
        {
            return new Dictionary<string, string>
            {
                {"SelectedUserId", SelectedUserId.ToString()},
                {"From", From.ToString()},
                {"To", To.ToString()},
                {"LogType", LogType.HasValue ? LogType.ToString() : null},

                {"GenericSearch", GenericSearch},
                {"DisplayStart", DisplayStart.ToString()},
                {"DisplayLength", DisplayLength.ToString()},
                {"OrderBy", OrderBy},
                {"SortDirection", SortDirection.ToString()},
            };
        }
    }
}
