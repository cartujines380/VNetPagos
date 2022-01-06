using System;
using System.Collections.Generic;
using VisaNet.Domain.EntitiesDtos.TableFilters.Base;

namespace VisaNet.Domain.EntitiesDtos.TableFilters
{
    public class NotificationFilterDto : BaseFilter
    {
        public NotificationFilterDto()
        {
            OrderBy = "Date";
        }

        public Guid UserId { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public string Service { get; set; }
        public int? NotificationPrupose { get; set; }
        public string Description { get; set; }

        public override IDictionary<string, string> GetFilterDictionary()
        {
            return new Dictionary<string, string>
            {
                {"UserId", UserId.ToString()},
                {"From", From.ToString()},
                {"To", To.ToString()},
                {"Service", Service},
                {"NotificationPrupose", NotificationPrupose.HasValue ? NotificationPrupose.ToString() : string.Empty},
                {"DisplayStart", DisplayStart.ToString()}
            };
        }
    }
}
