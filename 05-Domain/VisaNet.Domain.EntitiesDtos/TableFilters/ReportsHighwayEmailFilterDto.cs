using System;
using System.Collections.Generic;
using VisaNet.Domain.EntitiesDtos.TableFilters.Base;

namespace VisaNet.Domain.EntitiesDtos.TableFilters
{
    public class ReportsHighwayEmailFilterDto : BaseFilter
    {
        public ReportsHighwayEmailFilterDto()
        {
            From = DateTime.Now.AddDays(-7);
            To = DateTime.Now.Date;
            Commerce = null;
            Branch = null;
            OrderBy = "CreationDate";
            SortDirection = SortDirection.Desc;
            
        }

        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public Int32? Commerce { get; set; }
        public Int32? Branch { get; set; }
        public String Email { get; set; }

        public override IDictionary<string, string> GetFilterDictionary()
        {
            return new Dictionary<string, string>
            {
                {"From", From.ToString()},
                {"To", To.ToString()},
                {"Commerce", Commerce.ToString()},
                {"Branch", Branch.ToString()},
                {"Email", Email},

                {"GenericSearch", GenericSearch},
                {"DisplayStart", DisplayStart.ToString()},
                {"DisplayLength", DisplayLength.ToString()},
                {"OrderBy", OrderBy},
                {"SortDirection", SortDirection.ToString()},
            };
        }
    }
}
