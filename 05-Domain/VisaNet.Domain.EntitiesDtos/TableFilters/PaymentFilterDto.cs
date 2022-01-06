using System;
using System.Collections.Generic;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Domain.EntitiesDtos.TableFilters.Base;

namespace VisaNet.Domain.EntitiesDtos.TableFilters
{
    public class PaymentFilterDto : BaseFilter
    {
        public PaymentFilterDto()
        {
            OrderBy = "Date";
            SortDirection = SortDirection.Desc;
        }

        //public string UserName { get; set; }
        public Guid UserId { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public String ServiceAssociatedDto { get; set; }
        public PaymentTypeFilterDto PaymentTypeFilterDto { get; set; }
        public int Status { get; set; }
        public string TrnsNumber { get; set; }

        public override IDictionary<string, string> GetFilterDictionary()
        {
            return new Dictionary<string, string>
            {
                {"UserId", UserId.ToString()},
                {"From", From.ToString()},
                {"To", To.ToString()},
                {"TrnsNumber", TrnsNumber},
                {"ServiceAssociatedDto", ServiceAssociatedDto},
                {"PaymentTypeFilterDto", PaymentTypeFilterDto.ToString()},
                {"Status", Status.ToString()},
                {"DisplayStart",DisplayStart.ToString()}
            };
        }
    }
}