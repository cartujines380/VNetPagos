using System;
using System.Collections.Generic;
using VisaNet.Domain.EntitiesDtos.TableFilters.Base;

namespace VisaNet.Domain.EntitiesDtos.TableFilters
{
    public class ReportsTransactionsAmountFilterDto : BaseFilter
    {
        public ReportsTransactionsAmountFilterDto()
        {
            OrderBy = "Parameter";
        }

        public int Parameter { get; set; }
        public int DateParameter { get; set; }
        public int Dimension { get; set; }
        public int Currency { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public int TransactionStatus { get; set; }
        public bool ExcludeZeros { get; set; }

        public override IDictionary<string, string> GetFilterDictionary()
        {
            return new Dictionary<string, string>
            {
                {"From", From.ToString()},
                {"To", To.ToString()},
                {"Parameter", Parameter.ToString()},
                {"DateParameter", DateParameter.ToString()},
                {"Dimension", Dimension.ToString()},
                {"Currency", Currency.ToString()},
                {"TransactionStatus", TransactionStatus.ToString()},
                {"ExcludeZeros", ExcludeZeros.ToString()},

                {"DisplayStart",DisplayStart.ToString()},
                {"DisplayLength",DisplayLength.ToString()},
                {"OrderBy",OrderBy},
                {"SortDirection",SortDirection.ToString()},
            };
        }
    }
}
