using System;
using System.Collections.Generic;
using VisaNet.Domain.EntitiesDtos.TableFilters.Base;

namespace VisaNet.Domain.EntitiesDtos.TableFilters
{
    public class ReportsHighwayBillFilterDto : BaseFilter
    {
        public ReportsHighwayBillFilterDto()
        {
            From = DateTime.Now.Date.AddDays(-7);
            To = DateTime.Now.Date;
            DateFromString = DateTime.Now.Date.AddDays(-7).ToString("dd/MM/yyyy");
            DateToString = DateTime.Now.Date.ToString("dd/MM/yyyy");
            CodComercio = null;
            CodSucursal = null;
            OrderBy = "";
        }

        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public String DateFromString { get; set; }
        public String DateToString { get; set; }
        public Int32? CodComercio { get; set; }
        public Int32? CodSucursal { get; set; }
        public String NroFactura { get; set; }

        public override IDictionary<string, string> GetFilterDictionary()
        {
            return new Dictionary<string, string>
            {
                {"From", From.ToString()},
                {"To", To.ToString()},
                {"DateFromString", DateFromString},
                {"DateToString", DateToString},
                {"CodComercio", CodComercio.ToString()},
                {"CodSucursal", CodSucursal.ToString()},
                {"NroFactura", NroFactura},

                {"GenericSearch", GenericSearch},
                {"DisplayStart", DisplayStart.ToString()},
                {"DisplayLength", DisplayLength.ToString()},
                {"OrderBy", OrderBy},
                {"SortDirection", SortDirection.ToString()},
            };
        }
    }
}
