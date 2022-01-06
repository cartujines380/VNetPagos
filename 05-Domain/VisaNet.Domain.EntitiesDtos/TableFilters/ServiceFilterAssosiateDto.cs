using System;
using System.Collections.Generic;
using VisaNet.Domain.EntitiesDtos.TableFilters.Base;

namespace VisaNet.Domain.EntitiesDtos.TableFilters
{
    public class ServiceFilterAssosiateDto : BaseFilter
    {
        public ServiceFilterAssosiateDto()
        {
            OrderBy = "Service";
        }
        public Guid ServiceAssociatedId { get; set; }
        public string Service { get; set; }
        public string ReferenceNumber { get; set; }
        public Guid UserId { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public bool IncludeDeleted { get; set; }
        public int WithAutomaticPaymentsInt { get; set; }

        public override IDictionary<string, string> GetFilterDictionary()
        {
            return new Dictionary<string, string>
            {
                {"ServiceId",ServiceAssociatedId.ToString()},
                {"Service",Service},
                {"ReferenceNumber",ReferenceNumber},
                {"UserId", UserId.ToString()},
                {"From", From.ToString()},
                {"To", To.ToString()},
                {"IncludeDeleted", IncludeDeleted.ToString()},
                {"WithAutomaticPaymentsInt", WithAutomaticPaymentsInt.ToString()},

                {"GenericSearch",GenericSearch},
                {"DisplayStart",DisplayStart.ToString()},
                {"DisplayLength",DisplayLength.ToString()},
                {"OrderBy",OrderBy},
                {"SortDirection",SortDirection.ToString()},
            };
        }
    }
}
