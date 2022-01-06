using System;
using System.Collections.Generic;
using VisaNet.Domain.EntitiesDtos.TableFilters.Base;

namespace VisaNet.Domain.EntitiesDtos.TableFilters
{
    public class ReportsTransactionsFilterDto : BaseFilter
    {
        public ReportsTransactionsFilterDto()
        {
            OrderBy = "PaymentUniqueIdentifier";
        }

        public string ClientEmail { get; set; }
        public string ClientName { get; set; }
        public string ClientSurname { get; set; }
        public Guid? GatewayId { get; set; }
        public int PaymentType { get; set; }
        public Guid? ServiceId { get; set; }
        public Guid? ServiceCategoryId { get; set; }
        public string PaymentTransactionNumber { get; set; }
        public Int64? PaymentUniqueIdentifier { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public string DateFromString { get; set; }
        public string DateToString { get; set; }
        public int? PaymentStatus { get; set; }
        public Guid? ServiceAssociatedId { get; set; } //para GET por reporte de servicios asociados
        public int Platform { get; set; } //para GET por reporte de servicios asociados

        public override IDictionary<string, string> GetFilterDictionary()
        {
            return new Dictionary<string, string>
            {
                {"ClientEmail", ClientEmail},
                {"ClientName", ClientName},
                {"ClientSurname", ClientSurname},
                {"GatewayId", GatewayId.ToString()},
                {"PaymentType", PaymentType.ToString()},
                {"PaymentTransactionNumber", PaymentTransactionNumber},
                {"PaymentUniqueIdentifier", PaymentUniqueIdentifier.ToString()},
                {"DateFrom",DateFrom.ToString()},
                {"DateTo",DateTo.ToString()},
                {"DateFromString",DateFromString},
                {"DateToString",DateToString},
                {"ServiceId", ServiceId.ToString()},
                {"ServiceCategoryId", ServiceCategoryId.ToString()},
                {"PaymentStatus", PaymentStatus.ToString()},
                {"ServiceAssociatedId", ServiceAssociatedId.ToString()},
                {"Platform",Platform.ToString()},

                {"GenericSearch",GenericSearch},
                {"DisplayStart",DisplayStart.ToString()},
                {"DisplayLength",DisplayLength.ToString()},
                {"OrderBy",OrderBy},
                {"SortDirection",SortDirection.ToString()}
            };
        }
    }
}