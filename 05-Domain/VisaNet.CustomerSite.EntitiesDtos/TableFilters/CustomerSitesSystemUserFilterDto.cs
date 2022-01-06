using System;
using System.Collections.Generic;
using VisaNet.Domain.EntitiesDtos.TableFilters.Base;

namespace VisaNet.CustomerSite.EntitiesDtos.TableFilters
{
    public class CustomerSitesSystemUserFilterDto : BaseFilter
    {
        public CustomerSitesSystemUserFilterDto()
        {
            OrderBy = "Email";
        }

        public string Email { get; set; }
        public Guid CommerceId { get; set; }
        public bool? IsDebitCommerce { get; set; }

        public override IDictionary<string, string> GetFilterDictionary()
        {
            return new Dictionary<string, string>
            {
                {"Email",Email},
                {"CommerceId",CommerceId.ToString()},
                {"IsDebitCommerce",IsDebitCommerce.ToString()},

                {"GenericSearch",GenericSearch},
                {"DisplayStart",DisplayStart.ToString()},
                {"DisplayLength",DisplayLength.ToString()},
                {"OrderBy",OrderBy},
                {"SortDirection",SortDirection.ToString()},
            };
        }

    }
}