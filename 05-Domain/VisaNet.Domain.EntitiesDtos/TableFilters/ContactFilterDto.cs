using System.Collections.Generic;
using VisaNet.Domain.EntitiesDtos.TableFilters.Base;

namespace VisaNet.Domain.EntitiesDtos.TableFilters
{
    public class ContactFilterDto : BaseFilter
    {
        public ContactFilterDto()
        {
            OrderBy = "Name";
        }

        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public int QueryType { get; set; }

        public override IDictionary<string, string> GetFilterDictionary()
        {
            return new Dictionary<string, string>
            {
                {"Name",Name},
                {"Surname",Surname},
                {"Email",Email},
                {"QueryType",QueryType.ToString()},

                {"GenericSearch",GenericSearch},
                {"DisplayStart",DisplayStart.ToString()},
                {"DisplayLength",DisplayLength.ToString()},
                {"OrderBy",OrderBy},
                {"SortDirection",SortDirection.ToString()},
            };
        }
    }
}
