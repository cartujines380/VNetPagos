using System;
using System.Collections.Generic;
using VisaNet.Domain.EntitiesDtos.TableFilters.Base;

namespace VisaNet.Domain.EntitiesDtos.TableFilters
{
    public class CardFilterDto : BaseFilter
    {
        public CardFilterDto()
        {
            OrderBy = "MaskedNumber";
        }
        public string MaskedNumber { get; set; }
        public string DueDateMonth { get; set; }
        public string DueDateYear { get; set; }
        public Guid UserId { get; set; }
        public bool? Active { get; set; }
        public string Description { get; set; }

        public override IDictionary<string, string> GetFilterDictionary()
        {
            return new Dictionary<string, string>
            {
                {"MaskedNumber",MaskedNumber},
                {"DueDateMonth",DueDateMonth},
                {"DueDateYear",DueDateYear},
                {"UserId",UserId.ToString()},
                {"Active",Active.ToString()},
                {"Description",Description},
                {"GenericSearch",GenericSearch},
                {"DisplayStart",DisplayStart.ToString()},
                {"DisplayLength",DisplayLength.ToString()},
                {"OrderBy",OrderBy},
                {"SortDirection",SortDirection.ToString()},
            };
        }
    }
}