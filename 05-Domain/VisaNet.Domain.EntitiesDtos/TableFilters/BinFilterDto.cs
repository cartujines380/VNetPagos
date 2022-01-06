using System;
using System.Collections.Generic;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Domain.EntitiesDtos.TableFilters.Base;

namespace VisaNet.Domain.EntitiesDtos.TableFilters
{
    public class BinFilterDto : BaseFilter
    {
        public BinFilterDto()
        {
            OrderBy = "Name";
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public string Value { get; set; }
        public string Gateway { get; set; }
        public string CardType { get; set; }
        public Guid GatewayId { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string Bank { get; set; }
        public string ValueFrom { get; set; }
        public string ValueTo { get; set; }       

        public override IDictionary<string, string> GetFilterDictionary()
        {
            return new Dictionary<string, string>
            {
                {"Name",Name},
                {"Description",Description},
                {"Value",Value},
                {"Gateway",Gateway},
                {"CardType",CardType},
                {"GatewayId",GatewayId.ToString()},
                {"Country",Country},
                {"State",State},
                {"Bank",Bank},
                {"ValueFrom",ValueFrom},
                {"ValueTo",ValueTo},                

                {"GenericSearch",GenericSearch},
                {"DisplayStart",DisplayStart.ToString()},
                {"DisplayLength",DisplayLength.ToString()},
                {"OrderBy",OrderBy},
                {"SortDirection",SortDirection.ToString()},
            };
        }
    }
}
