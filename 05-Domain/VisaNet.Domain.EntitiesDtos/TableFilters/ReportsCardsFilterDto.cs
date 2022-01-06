using System.Collections.Generic;
using VisaNet.Domain.EntitiesDtos.TableFilters.Base;

namespace VisaNet.Domain.EntitiesDtos.TableFilters
{
    public class ReportsCardsFilterDto : BaseFilter
    {
        public ReportsCardsFilterDto()
        {
            OrderBy = "ClientEmail";
        }
        public string ClientEmail { get; set; }
        public string ClientName { get; set; }
        public string ClientSurname { get; set; }
        public string CardMaskedNumber { get; set; }
        public string CardBin { get; set; }
        public int CardType { get; set; }
        public int CardState { get; set; }

        public override IDictionary<string, string> GetFilterDictionary()
        {
            return new Dictionary<string, string>
            {
                {"ClientEmail", ClientEmail},
                {"ClientName", ClientName},
                {"ClientSurname", ClientSurname},
                {"CardMaskedNumber", CardMaskedNumber},
                {"CardBin", CardBin},
                {"CardType", CardType.ToString()},
                {"CardState", CardState.ToString()},

                {"GenericSearch",GenericSearch},
                {"DisplayStart",DisplayStart.ToString()},
                {"DisplayLength",DisplayLength.ToString()},
                {"OrderBy",OrderBy},
                {"SortDirection",SortDirection.ToString()},
            };
        }

    }
}