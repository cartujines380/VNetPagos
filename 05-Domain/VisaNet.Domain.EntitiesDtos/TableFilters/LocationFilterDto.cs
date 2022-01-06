using System.Collections.Generic;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Domain.EntitiesDtos.TableFilters.Base;

namespace VisaNet.Domain.EntitiesDtos.TableFilters
{
    public class LocationFilterDto : BaseFilter
    {
        public LocationFilterDto()
        {
            OrderBy = "Name";
        }

        public string Name { get; set; }
        public GatewayEnumDto GatewayEnumDto { get; set; }
        public DepartamentDtoType DepartamentDtoType { get; set; }

        public override IDictionary<string, string> GetFilterDictionary()
        {
            return new Dictionary<string, string>
            {
                {"Name", Name},
                {"GatewayEnumDto", GatewayEnumDto.ToString()},
                {"DepartamentDtoType", DepartamentDtoType.ToString()},

                {"GenericSearch", GenericSearch},
                {"DisplayStart", DisplayStart.ToString()},
                {"DisplayLength", DisplayLength.ToString()},
                {"OrderBy", OrderBy},
                {"SortDirection", SortDirection.ToString()},
            };
        }
    }
}
