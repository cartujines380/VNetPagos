using System;
using System.Collections.Generic;
using VisaNet.Domain.EntitiesDtos.TableFilters.Base;

namespace VisaNet.CustomerSite.EntitiesDtos.TableFilters
{
    public class CustomerSiteBranchFilterDto : BaseFilter
    {
        public CustomerSiteBranchFilterDto()
        {
            OrderBy = "Name";
        }

        public string Name { get; set; }
        public Guid Service { get; set; }
        public Guid CommerceId { get; set; }
        public bool? IsDebitCommerce { get; set; }

        public override IDictionary<string, string> GetFilterDictionary()
        {
            return new Dictionary<string, string>
            {
                {"Name",Name},
                {"Service",Service.ToString()},
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