using System;
using System.Collections.Generic;
using VisaNet.Domain.EntitiesDtos.TableFilters.Base;

namespace VisaNet.Domain.EntitiesDtos.TableFilters
{
    public class ReportsAutomaticPaymentsFilterDto : BaseFilter
    {
        public ReportsAutomaticPaymentsFilterDto()
        {
            OrderBy = "ClientEmail";
        }

        public DateTime CreationDateFrom { get; set; }
        public DateTime CreationDateTo { get; set; }
        public string CreationDateFromString { get; set; }
        public string CreationDateToString { get; set; }
        public string ClientEmail { get; set; }
        public string ServiceNameAndDesc { get; set; }
        public Guid ServiceAssociatedId { get; set; }

        public override IDictionary<string, string> GetFilterDictionary()
        {
            return new Dictionary<string, string>
            {
                {"CreationDateFrom", CreationDateFrom.ToString()},
                {"CreationDateTo", CreationDateTo.ToString()},
                {"CreationDateFromString", CreationDateFromString},
                {"CreationDateToString", CreationDateToString},
                {"ClientEmail", ClientEmail},
                {"ServiceNameAndDesc", ServiceNameAndDesc},
                {"ServiceAssociatedId", ServiceAssociatedId.ToString()},

                {"GenericSearch",GenericSearch},
                {"DisplayStart",DisplayStart.ToString()},
                {"DisplayLength",DisplayLength.ToString()},
                {"OrderBy",OrderBy},
                {"SortDirection",SortDirection.ToString()},
            };
        }
    }
}
