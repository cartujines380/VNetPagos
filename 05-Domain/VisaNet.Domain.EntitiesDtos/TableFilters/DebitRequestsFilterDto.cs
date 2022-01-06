using System;
using System.Collections.Generic;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Domain.EntitiesDtos.TableFilters.Base;

namespace VisaNet.Domain.EntitiesDtos.TableFilters
{
    public class DebitRequestsFilterDto : BaseFilter
    {
        public DebitRequestsFilterDto()
        {
            OrderBy = "Date";
            SortDirection = SortDirection.Desc;
        }

        public Guid UserId { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public DebitRequestTypeDto DebitType { get; set; }
        public DebitRequestStateDto DebitState { get; set; }
        public string Service { get; set; }
        public Guid CardId { get; set; }

        public string Email { get; set; } 

        public override IDictionary<string, string> GetFilterDictionary()
        {
            return new Dictionary<string, string>
            {
                { "UserId", UserId.ToString() },
                { "DateFrom", DateFrom.ToString() },
                { "DateTo", DateTo.ToString() },
                { "DebitType", DebitType.ToString() },
                { "DebitState", DebitState.ToString() },
                { "Service", Service },
                { "CardId", CardId.ToString() },
                { "Email", Email },

                {"GenericSearch",GenericSearch},
                {"DisplayStart",DisplayStart.ToString()},
                {"DisplayLength",DisplayLength.ToString()},
                {"OrderBy",OrderBy},
                {"SortDirection",SortDirection.ToString()},
            };
        }
    }
}
