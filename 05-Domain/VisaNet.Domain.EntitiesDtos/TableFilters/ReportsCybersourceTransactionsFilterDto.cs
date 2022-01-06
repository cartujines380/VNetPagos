using System;
using System.Collections.Generic;
using VisaNet.Domain.EntitiesDtos.TableFilters.Base;

namespace VisaNet.Domain.EntitiesDtos.TableFilters
{
    public class ReportsCybersourceTransactionsFilterDto : BaseFilter
    {
        public ReportsCybersourceTransactionsFilterDto()
        {
            From = new DateTime(DateTime.Now.Year, 1, 1);
            To = DateTime.Now.Date;
            OrderBy = "From";
        }

        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public bool Sale { get; set; }
        public int TransactionType { get; set; }
        public string Bin { get; set; }

        public override IDictionary<string, string> GetFilterDictionary()
        {
            return new Dictionary<string, string>
            {
                {"From", From.ToString()},
                {"To", To.ToString()},
                {"Sale", Sale.ToString()},
                {"TransactionType", TransactionType.ToString()},
                {"Bin", Bin},

                {"DisplayStart",DisplayStart.ToString()},
                {"DisplayLength",DisplayLength.ToString()},
                {"OrderBy",OrderBy},
                {"SortDirection",SortDirection.ToString()},
            };
        }
    }
}
