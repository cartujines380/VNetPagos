using System;
using System.Collections.Generic;
using VisaNet.Domain.EntitiesDtos.TableFilters.Base;

namespace VisaNet.Domain.EntitiesDtos.TableFilters
{
    public class ReportsServicesAssociatedFilterDto : BaseFilter
    {
        public ReportsServicesAssociatedFilterDto()
        {
            OrderBy = "ClientEmail";
        }

        public DateTime CreationDateFrom { get; set; }
        public DateTime CreationDateTo { get; set; }
        public string CreationDateFromString { get; set; }
        public string CreationDateToString { get; set; }
        public string ClientEmail { get; set; }
        public string ClientName { get; set; }
        public string ClientSurname { get; set; }
        public string ServiceNameAndDesc { get; set; }
        public Guid ServiceCategoryId { get; set; }
        public int Enabled { get; set; }
        //public int Deleted { get; set; }
        public int HasAutomaticPayment { get; set; }

        public override IDictionary<string, string> GetFilterDictionary()
        {
            return new Dictionary<string, string>
            {
                {"CreationDateFrom", CreationDateFrom.ToString()},
                {"CreationDateTo", CreationDateTo.ToString()},
                {"CreationDateFromString", CreationDateFromString},
                {"CreationDateToString", CreationDateToString},
                {"ClientEmail", ClientEmail},
                {"ClientName", ClientName},
                {"ClientSurname", ClientSurname},
                {"ServiceNameAndDesc",ServiceNameAndDesc},
                {"ServiceCategoryId", ServiceCategoryId.ToString()},
                {"Enabled", Enabled.ToString()},
                //{"Deleted", Deleted.ToString()},
                {"HasAutomaticPayment", HasAutomaticPayment.ToString()},

                {"GenericSearch",GenericSearch},
                {"DisplayStart",DisplayStart.ToString()},
                {"DisplayLength",DisplayLength.ToString()},
                {"OrderBy",OrderBy},
                {"SortDirection",SortDirection.ToString()},
            };
        }
    }
}
