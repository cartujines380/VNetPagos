using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using VisaNet.Common.FrameworkExtensions;
using VisaNet.Common.Logging.Entities;
using VisaNet.CustomerSite.EntitiesDtos.TableFilters;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.ChangeTracker;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Presentation.Administration.Models;
using AuditLogDto = VisaNet.Domain.EntitiesDtos.ChangeTracker.AuditLogDto;

namespace VisaNet.Presentation.Administration.Handlers
{
    public static class AjaxHandlers
    {
        public static ServiceCategoryFilterDto AjaxHandlerServiceCategory(HttpRequestBase request, JQueryDataTableParamModel param)
        {
            var filter = new ServiceCategoryFilterDto();

            #region GenericFilter
            //if (!string.IsNullOrEmpty(param.sSearch))
            //    filter.GenericSearch = param.sSearch.ToLower();
            #endregion

            #region CustomFilters
            if (!string.IsNullOrWhiteSpace(request["Name"]))
            {
                filter.Name = Convert.ToString(request["Name"]); ;
                //filteredItems.Where(f => (string.IsNullOrEmpty(namesFilter) || f.Name.Contains(namesFilter)));
            }
            #endregion

            #region Sort
            var sortColumnIndex = Convert.ToInt32(request["iSortCol_0"]);
            var sortableName = Convert.ToBoolean(request["bSortable_0"]);

            Func<int, string> orderingFunction = (c => sortColumnIndex == 0 && sortableName ? "0" : "");

            filter.OrderBy = orderingFunction(sortColumnIndex);

            //Func<int, string> orderingFunction = (c => sortColumnIndex == 0 && sortableName ? GetPropertiesNames.GetPropertyName<ServiceCategoryDto>(p => p.Name) : "");

            //filter.OrderBy = orderingFunction(sortColumnIndex);
            #endregion

            #region SortDirection
            var sortDirection = request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filter.SortDirection = SortDirection.Asc;
            else
                filter.SortDirection = SortDirection.Desc;
            #endregion

            filter.DisplayStart = param.iDisplayStart;

            return filter;
        }

        public static ServiceFilterDto AjaxHandlerService(HttpRequestBase request, JQueryDataTableParamModel param)
        {
            var filter = new ServiceFilterDto();

            #region GenericFilter
            //if (!string.IsNullOrEmpty(param.sSearch))
            //    filter.GenericSearch = param.sSearch.ToLower();
            #endregion

            #region CustomFilters
            filter.Name = request["Name"];
            filter.ServiceContainerId = request["ServiceContainerId"];
            filter.ServiceCategoryId = !string.IsNullOrWhiteSpace(request["ServiceCategoryId"]) ? Guid.Parse(request["ServiceCategoryId"]) : default(Guid);
            filter.Gateway = Convert.ToInt16(request["Gateway"]);
            #endregion

            #region Sort
            var sortColumnIndex = Convert.ToInt32(request["iSortCol_0"]);

            Func<int, string> orderingFunction = (c =>
                sortColumnIndex == 0 ? GetPropertiesNames.GetPropertyName<ServiceDto>(p => p.Name) :
                sortColumnIndex == 1 ? GetPropertiesNames.GetPropertyName<ServiceDto>(p => p.ServiceCategory) :
                sortColumnIndex == 3 ? GetPropertiesNames.GetPropertyName<ServiceDto>(p => p.ServiceContainerDto) :
                sortColumnIndex == 5 ? GetPropertiesNames.GetPropertyName<ServiceDto>(p => p.Active) :
                "");

            filter.OrderBy = orderingFunction(sortColumnIndex);
            #endregion

            #region SortDirection
            var sortDirection = request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filter.SortDirection = SortDirection.Asc;
            else
                filter.SortDirection = SortDirection.Desc;
            #endregion

            filter.DisplayStart = param.iDisplayStart;

            return filter;
        }

        public static ServiceFilterDto AjaxHandlerServiceContainer(HttpRequestBase request, JQueryDataTableParamModel param)
        {
            var filter = new ServiceFilterDto();

            #region GenericFilter
            #endregion

            #region CustomFilters
            filter.Name = request["Name"];
            filter.ServiceCategoryId = !string.IsNullOrWhiteSpace(request["ServiceCategoryId"]) ? Guid.Parse(request["ServiceCategoryId"]) : default(Guid);
            #endregion
            #region Sort
            var sortColumnIndex = Convert.ToInt32(request["iSortCol_0"]);
            var sortableName = Convert.ToBoolean(request["bSortable_0"]);
            var sortableCategory = Convert.ToBoolean(request["bSortable_1"]);
            var sortableState = Convert.ToBoolean(request["bSortable_2"]);
            var sortableContainer = Convert.ToBoolean(request["bSortable_3"]);

            Func<int, string> orderingFunction = (c => sortColumnIndex == 0 && sortableName ? GetPropertiesNames.GetPropertyName<ServiceDto>(p => p.Name) :
                sortColumnIndex == 1 && sortableCategory ? GetPropertiesNames.GetPropertyName<ServiceDto>(p => p.ServiceCategory) :
                sortColumnIndex == 2 && sortableState ? GetPropertiesNames.GetPropertyName<ServiceDto>(p => p.Active) :
                sortColumnIndex == 3 && sortableContainer ? GetPropertiesNames.GetPropertyName<ServiceDto>(p => p.ServiceContainerDto) :
                "");

            filter.OrderBy = orderingFunction(sortColumnIndex);
            #endregion
            #region SortDirection
            var sortDirection = request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filter.SortDirection = SortDirection.Asc;
            else
                filter.SortDirection = SortDirection.Desc;
            #endregion


            filter.DisplayStart = param.iDisplayStart;

            return filter;
        }

        public static ContactFilterDto AjaxHandlerContact(HttpRequestBase request, JQueryDataTableParamModel param)
        {
            var filter = new ContactFilterDto();

            #region GenericFilter
            //if (!string.IsNullOrEmpty(param.sSearch))
            //    filter.GenericSearch = param.sSearch.ToLower();
            #endregion

            #region CustomFilters
            if (!string.IsNullOrWhiteSpace(request["Name"])
               || !string.IsNullOrWhiteSpace(request["Surname"])
               || !string.IsNullOrWhiteSpace(request["Email"])
               || !string.IsNullOrWhiteSpace(request["sSearch_3"]))
            {
                var namesFilter = Convert.ToString(request["Name"]);
                var surnamesFilter = Convert.ToString(request["Surname"]);
                var emailsFilter = Convert.ToString(request["Email"]);

                int querytypesFilter;//string.IsNullOrEmpty(request["sSearch_3"]) ? 0 : Int32.TryParse(request["sSearch_3"], out );

                Int32.TryParse(request["sSearch_3"].Replace(",", ""), out querytypesFilter);

                //filteredItems = filteredItems.Where(f => (string.IsNullOrEmpty(namesFilter) || f.Name.Contains(namesFilter)));

                filter.Name = namesFilter;
                filter.Surname = surnamesFilter;
                filter.Email = emailsFilter;
                filter.QueryType = querytypesFilter;
            }
            #endregion

            #region Sort
            var sortColumnIndex = Convert.ToInt32(request["iSortCol_0"]);

            var sortableDate = Convert.ToBoolean(request["bSortable_0"]);
            var sortableName = Convert.ToBoolean(request["bSortable_1"]);
            var sortableSurname = Convert.ToBoolean(request["bSortable_2"]);
            var sortableEmail = Convert.ToBoolean(request["bSortable_3"]);
            var sortableQueryType = Convert.ToBoolean(request["bSortable_4"]);
            var sortableSubject = Convert.ToBoolean(request["bSortable_5"]);

            Func<int, string> orderingFunction = (c => sortColumnIndex == 0 && sortableDate ? GetPropertiesNames.GetPropertyName<ContactDto>(p => p.Date) :
                                                       sortColumnIndex == 1 && sortableName ? GetPropertiesNames.GetPropertyName<ContactDto>(p => p.Name) :
                                                       sortColumnIndex == 2 && sortableSurname ? GetPropertiesNames.GetPropertyName<ContactDto>(p => p.Surname) :
                                                       sortColumnIndex == 3 && sortableEmail ? GetPropertiesNames.GetPropertyName<ContactDto>(p => p.Email) :
                                                       sortColumnIndex == 4 && sortableQueryType ? GetPropertiesNames.GetPropertyName<ContactDto>(p => p.QueryType) :
                                                       sortColumnIndex == 5 && sortableSubject ? GetPropertiesNames.GetPropertyName<ContactDto>(p => p.Subject) : "");

            filter.OrderBy = orderingFunction(sortColumnIndex);
            #endregion


            #region SortDirection
            var sortDirection = request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filter.SortDirection = SortDirection.Asc;
            else
                filter.SortDirection = SortDirection.Desc;
            #endregion


            filter.DisplayStart = param.iDisplayStart;

            return filter;
        }

        public static FaqFilterDto AjaxHandlerFaq(HttpRequestBase request, JQueryDataTableParamModel param)
        {
            var filter = new FaqFilterDto();

            #region GenericFilter
            //if (!string.IsNullOrEmpty(param.sSearch))
            //    filter.GenericSearch = param.sSearch.ToLower();
            #endregion

            #region CustomFilters
            if (!string.IsNullOrWhiteSpace(request["Order"])
               || !string.IsNullOrWhiteSpace(request["sSearch_1"]))
            {
                var ordersFilter = string.IsNullOrEmpty(request["Order"]) ? 0 : Convert.ToInt32(request["Order"]);
                var questionsFilter = Convert.ToString(request["Question"]);

                filter.Order = ordersFilter;
                filter.Question = questionsFilter;
            }
            #endregion

            #region Sort
            var sortColumnIndex = Convert.ToInt32(request["iSortCol_0"]);

            var sortableOrder = Convert.ToBoolean(request["bSortable_0"]);
            var sortableQuestion = Convert.ToBoolean(request["bSortable_1"]);

            Func<int, string> orderingFunction = (c => sortColumnIndex == 0 && sortableOrder ? GetPropertiesNames.GetPropertyName<FaqDto>(p => p.Order) :
                                                       sortColumnIndex == 1 && sortableQuestion ? GetPropertiesNames.GetPropertyName<FaqDto>(p => p.Question) : "");

            filter.OrderBy = orderingFunction(sortColumnIndex);
            #endregion


            #region SortDirection
            var sortDirection = request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filter.SortDirection = SortDirection.Asc;
            else
                filter.SortDirection = SortDirection.Desc;
            #endregion


            filter.DisplayStart = param.iDisplayStart;

            return filter;
        }

        public static SubscriberFilterDto AjaxHandlerSubscriber(HttpRequestBase request, JQueryDataTableParamModel param)
        {
            var filter = new SubscriberFilterDto();

            #region GenericFilter
            //if (!string.IsNullOrEmpty(param.sSearch))
            //    filter.GenericSearch = param.sSearch.ToLower();
            #endregion

            #region CustomFilters
            if (!string.IsNullOrWhiteSpace(request["Name"])
               || !string.IsNullOrWhiteSpace(request["Surname"])
               || !string.IsNullOrWhiteSpace(request["Email"]))
            {
                var namesFilter = Convert.ToString(request["Name"]);
                var surnamesFilter = Convert.ToString(request["Surname"]);
                var emailsFilter = Convert.ToString(request["Email"]);

                filter.Name = namesFilter;
                filter.Surname = surnamesFilter;
                filter.Email = emailsFilter;
            }
            #endregion

            #region Sort
            var sortColumnIndex = Convert.ToInt32(request["iSortCol_0"]);

            var sortableName = Convert.ToBoolean(request["bSortable_0"]);
            var sortableSurname = Convert.ToBoolean(request["bSortable_1"]);
            var sortableEmail = Convert.ToBoolean(request["bSortable_2"]);

            Func<int, string> orderingFunction = (c => sortColumnIndex == 0 && sortableName ? GetPropertiesNames.GetPropertyName<SubscriberDto>(p => p.Name) :
                                                       sortColumnIndex == 1 && sortableSurname ? GetPropertiesNames.GetPropertyName<SubscriberDto>(p => p.Surname) :
                                                       sortColumnIndex == 2 && sortableEmail ? GetPropertiesNames.GetPropertyName<SubscriberDto>(p => p.Email) : "");

            filter.OrderBy = orderingFunction(sortColumnIndex);
            #endregion


            #region SortDirection
            var sortDirection = request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filter.SortDirection = SortDirection.Asc;
            else
                filter.SortDirection = SortDirection.Desc;
            #endregion


            filter.DisplayStart = param.iDisplayStart;

            return filter;
        }

        public static SystemUserFilterDto AjaxHandlerSystemUser(HttpRequestBase request, JQueryDataTableParamModel param)
        {
            var filter = new SystemUserFilterDto();

            #region GenericFilter
            if (!string.IsNullOrEmpty(param.sSearch))
                filter.GenericSearch = param.sSearch.ToLower();
            #endregion

            #region CustomFilters
            //if (  !string.IsNullOrWhiteSpace(request["sSearch_0"])
            //   || !string.IsNullOrWhiteSpace(request["sSearch_1"])
            //   || !string.IsNullOrWhiteSpace(request["sSearch_2"]))
            //{
            //    var namesFilter = Convert.ToString(request["sSearch_0"]);

            //    filteredItems = filteredItems.Where(f => (string.IsNullOrEmpty(namesFilter) || f.Name.Contains(namesFilter)));
            //}
            #endregion

            #region Sort
            var sortColumnIndex = Convert.ToInt32(request["iSortCol_0"]);
            var sortableName = Convert.ToBoolean(request["bSortable_0"]);
            var sortableSystemUserType = Convert.ToBoolean(request["bSortable_1"]);

            Func<int, string> orderingFunction = (c => sortColumnIndex == 0 && sortableName ? GetPropertiesNames.GetPropertyName<SystemUserDto>(p => p.LDAPUserName)
                                                     : sortColumnIndex == 1 && sortableSystemUserType ? GetPropertiesNames.GetPropertyName<SystemUserDto>(p => p.SystemUserType)
                                                     : "");

            filter.OrderBy = orderingFunction(sortColumnIndex);
            #endregion


            #region SortDirection
            var sortDirection = request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filter.SortDirection = SortDirection.Asc;
            else
                filter.SortDirection = SortDirection.Desc;
            #endregion


            filter.DisplayStart = param.iDisplayStart;

            return filter;
        }

        public static RoleFilterDto AjaxHandlerRole(HttpRequestBase request, JQueryDataTableParamModel param)
        {
            var filter = new RoleFilterDto();

            #region GenericFilter
            if (!string.IsNullOrEmpty(param.sSearch))
                filter.GenericSearch = param.sSearch.ToLower();
            #endregion

            #region CustomFilters
            //if (  !string.IsNullOrWhiteSpace(request["sSearch_0"])
            //   || !string.IsNullOrWhiteSpace(request["sSearch_1"])
            //   || !string.IsNullOrWhiteSpace(request["sSearch_2"]))
            //{
            //    var namesFilter = Convert.ToString(request["sSearch_0"]);

            //    filteredItems = filteredItems.Where(f => (string.IsNullOrEmpty(namesFilter) || f.Name.Contains(namesFilter)));
            //}
            #endregion

            #region Sort
            var sortColumnIndex = Convert.ToInt32(request["iSortCol_0"]);
            var sortableName = Convert.ToBoolean(request["bSortable_0"]);

            Func<int, string> orderingFunction = (c => sortColumnIndex == 0 && sortableName ? GetPropertiesNames.GetPropertyName<RoleFilterDto>(p => p.Name)
                                                     : "");

            filter.OrderBy = orderingFunction(sortColumnIndex);
            #endregion


            #region SortDirection
            var sortDirection = request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filter.SortDirection = SortDirection.Asc;
            else
                filter.SortDirection = SortDirection.Desc;
            #endregion


            filter.DisplayStart = param.iDisplayStart;

            return filter;
        }

        public static BinFilterDto AjaxHandlerBin(HttpRequestBase request, JQueryDataTableParamModel param)
        {
            var filter = new BinFilterDto();

            #region GenericFilter
            //if (!string.IsNullOrEmpty(param.sSearch))
            //    filter.GenericSearch = param.sSearch.ToLower();
            #endregion

            #region CustomFilters

            if (!string.IsNullOrWhiteSpace(request["Name"]))
            {
                var nameFilter = Convert.ToString(request["Name"]);
                filter.Name = nameFilter;
            }

            if (!string.IsNullOrWhiteSpace(request["Value"]))
            {
                var valueFilter = Convert.ToString(request["Value"]);
                filter.Value = valueFilter;
            }

            if (!string.IsNullOrWhiteSpace(request["Gateway"]))
            {
                var gatewayFilter = Convert.ToString(request["Gateway"]);
                filter.Gateway = gatewayFilter;
            }

            if (!string.IsNullOrWhiteSpace(request["Country"]))
            {
                var country = Convert.ToString(request["Country"]);
                filter.Country = country;
            }

            if (!string.IsNullOrWhiteSpace(request["Bank"]))
            {
                var bank = Convert.ToString(request["Bank"]);
                filter.Bank = bank;
            }

            if (!string.IsNullOrWhiteSpace(request["FilterCardType"]))
            {
                var cardType = Convert.ToString(request["FilterCardType"]);
                filter.CardType = cardType;
            }

            if (!string.IsNullOrWhiteSpace(request["FilterStatus"]))
            {
                var state = Convert.ToString(request["FilterStatus"]);
                filter.State = state;
            }

            if (!string.IsNullOrWhiteSpace(request["ValueFrom"]))
            {
                var valueFrom = Convert.ToString(request["ValueFrom"]);
                filter.ValueFrom = valueFrom;
            }

            if (!string.IsNullOrWhiteSpace(request["ValueTo"]))
            {
                var valueTo = Convert.ToString(request["ValueTo"]);
                filter.ValueTo = valueTo;
            }

            #endregion

            #region Sort
            var sortColumnIndex = Convert.ToInt32(request["iSortCol_0"]);

            var sortableName = Convert.ToBoolean(request["bSortable_0"]);
            var sortableValue = Convert.ToBoolean(request["bSortable_1"]);
            var sortableCountry = Convert.ToBoolean(request["bSortable_3"]);
            var sortableBank = Convert.ToBoolean(request["bSortable_4"]);
            var sortableCardType = Convert.ToBoolean(request["bSortable_5"]);
            var sortableAffiliation = Convert.ToBoolean(request["bSortable_6"]);
            var sortableState = Convert.ToBoolean(request["bSortable_8"]);

            Func<int, string> orderingFunction = (c =>
                sortColumnIndex == 0 && sortableName ? "Name" :
                sortColumnIndex == 1 && sortableValue ? "Value" :
                sortColumnIndex == 3 && sortableCountry ? "Country" :
                sortColumnIndex == 4 && sortableBank ? "Bank" :
                sortColumnIndex == 5 && sortableCardType ? "CardType" :
                sortColumnIndex == 6 && sortableAffiliation ? "AffiliationCard" :
                sortColumnIndex == 8 && sortableState ? "Active" : "");

            filter.OrderBy = orderingFunction(sortColumnIndex);

            #endregion

            #region SortDirection
            var sortDirection = request["sSortDir_0"]; // asc or desc
            filter.SortDirection = sortDirection == "asc" ? SortDirection.Asc : SortDirection.Desc;
            #endregion

            filter.DisplayStart = param.iDisplayStart;

            return filter;
        }

        public static ReportsServicesAssociatedFilterDto AjaxHandlerServiceAssociated(HttpRequestBase request, JQueryDataTableParamModel param)
        {
            var filter = new ReportsServicesAssociatedFilterDto();

            #region GenericFilter
            if (!string.IsNullOrEmpty(param.sSearch) && param.sSearch != "undefined")
                filter.GenericSearch = param.sSearch.ToLower();
            #endregion

            #region CustomFilters

            DateTime tmpDateTime;

            filter.CreationDateFrom = DateTime.TryParse(request["CreationDateFrom"], out tmpDateTime) ? tmpDateTime : DateTime.MinValue;
            filter.CreationDateFromString = filter.CreationDateFrom != DateTime.MinValue ? filter.CreationDateFrom.ToString("dd/MM/yyyy") : string.Empty;

            filter.CreationDateTo = DateTime.TryParse(request["CreationDateTo"], out tmpDateTime) ? tmpDateTime : DateTime.MinValue;
            filter.CreationDateToString = filter.CreationDateTo != DateTime.MinValue ? filter.CreationDateTo.ToString("dd/MM/yyyy") : string.Empty;

            if (!string.IsNullOrWhiteSpace(request["ClientEmail"]))
                filter.ClientEmail = request["ClientEmail"];

            if (!string.IsNullOrWhiteSpace(request["ClientName"]))
                filter.ClientName = request["ClientName"];

            if (!string.IsNullOrWhiteSpace(request["ClientSurname"]))
                filter.ClientSurname = request["ClientSurname"];

            if (!string.IsNullOrWhiteSpace(request["ServiceNameAndDesc"]))
                filter.ServiceNameAndDesc = request["ServiceNameAndDesc"];

            filter.ServiceCategoryId = !string.IsNullOrWhiteSpace(request["ServiceCategoryId"]) ? Guid.Parse(request["ServiceCategoryId"]) : default(Guid);

            //if (!string.IsNullOrWhiteSpace(request["ServiceDescription"]))
            //    filter.ServiceDescription = request["ServiceDescription"];

            if (!string.IsNullOrWhiteSpace(request["Enabled"]))
                filter.Enabled = Convert.ToInt32(request["Enabled"]);

            //if (!string.IsNullOrWhiteSpace(request["Deleted"]))
            //    filter.Deleted = Convert.ToInt32(request["Deleted"]);

            if (!string.IsNullOrWhiteSpace(request["HasAutomaticPayment"]))
                filter.HasAutomaticPayment = Convert.ToInt32(request["HasAutomaticPayment"]);

            #endregion

            #region Sort
            var sortColumnIndex = Convert.ToInt32(request["iSortCol_0"]);

            var sortableClientEmail = Convert.ToBoolean(request["bSortable_0"]);
            var sortableClientName = Convert.ToBoolean(request["bSortable_1"]);
            var sortableClientSurname = Convert.ToBoolean(request["bSortable_2"]);
            var sortableServiceNameAndDesc = Convert.ToBoolean(request["bSortable_3"]);
            var sortableServiceCategory = Convert.ToBoolean(request["bSortable_4"]);
            var sortableReferenceNumber = Convert.ToBoolean(request["bSortable_5"]);
            var sortableActive = Convert.ToBoolean(request["bSortable_6"]);
            var sortableDeleted = Convert.ToBoolean(request["bSortable_7"]);
            var sortableAutomaticPayment = Convert.ToBoolean(request["bSortable_8"]);
            var sortableDefaultCardMask = Convert.ToBoolean(request["bSortable_9"]);
            var sortablePaymentsCount = Convert.ToBoolean(request["bSortable_10"]);
            var sortableCreationDate = Convert.ToBoolean(request["bSortable_11"]);
            var sortableLastModificationDate = Convert.ToBoolean(request["bSortable_12"]);

            Func<int, string> orderingFunction = (c => sortColumnIndex == 0 && sortableClientEmail ? "0" :
                                                       sortColumnIndex == 1 && sortableClientName ? "1" :
                                                       sortColumnIndex == 2 && sortableClientSurname ? "2" :
                                                       sortColumnIndex == 3 && sortableServiceNameAndDesc ? "3" :
                                                       sortColumnIndex == 4 && sortableServiceCategory ? "4" :
                                                       sortColumnIndex == 5 && sortableReferenceNumber ? "5" :
                                                       sortColumnIndex == 6 && sortableActive ? "6" :
                                                       sortColumnIndex == 7 && sortableDeleted ? "7" :
                                                       sortColumnIndex == 8 && sortableAutomaticPayment ? "8" :
                                                       sortColumnIndex == 9 && sortableDefaultCardMask ? "9" :
                                                       sortColumnIndex == 10 && sortablePaymentsCount ? "10" :
                                                       sortColumnIndex == 11 && sortableCreationDate ? "11" :
                                                       sortColumnIndex == 12 && sortableLastModificationDate ? "12" : "");

            filter.OrderBy = orderingFunction(sortColumnIndex);
            #endregion

            #region SortDirection

            if (sortColumnIndex == 0)
            {
                var sortDirection = request["sSortDir_0"]; // asc or desc
                filter.SortDirection = sortDirection == "asc" ? SortDirection.Desc : SortDirection.Asc;
            }
            else
            {
                var sortDirection = request["sSortDir_0"]; // asc or desc
                filter.SortDirection = sortDirection == "asc" ? SortDirection.Asc : SortDirection.Desc;
            }

            #endregion

            filter.DisplayStart = param.iDisplayStart;
            filter.DisplayLength = 10;

            return filter;
        }

        public static PromotionFilterDto AjaxHandlerPromotion(HttpRequestBase request, JQueryDataTableParamModel param)
        {
            var filter = new PromotionFilterDto();

            #region GenericFilter
            //if (!string.IsNullOrEmpty(param.sSearch))
            //    filter.GenericSearch = param.sSearch.ToLower();
            #endregion

            #region CustomFilters
            if (!string.IsNullOrWhiteSpace(request["Name"]))
                filter.Name = Convert.ToString(request["Name"]);
            #endregion

            #region Sort
            var sortColumnIndex = Convert.ToInt32(request["iSortCol_0"]);

            var sortableName = Convert.ToBoolean(request["bSortable_0"]);
            var sortableState = Convert.ToBoolean(request["bSortable_1"]);

            Func<int, string> orderingFunction = (c => sortColumnIndex == 0 && sortableName ? "0" :
                                                       sortColumnIndex == 1 && sortableState ? "1" : "");

            filter.OrderBy = orderingFunction(sortColumnIndex);
            #endregion

            #region SortDirection
            var sortDirection = request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filter.SortDirection = SortDirection.Asc;
            else
                filter.SortDirection = SortDirection.Desc;
            #endregion

            filter.DisplayStart = param.iDisplayStart;

            return filter;
        }

        public static ReportsTransactionsFilterDto AjaxHandlerTransactions(HttpRequestBase request, JQueryDataTableParamModel param)
        {
            var filter = new ReportsTransactionsFilterDto();

            #region GenericFilter
            //if (!string.IsNullOrEmpty(param.sSearch))
            //    filter.GenericSearch = param.sSearch.ToLower();

            if (!string.IsNullOrEmpty(param.sSearch) && param.sSearch != "undefined")
                filter.GenericSearch = param.sSearch.ToLower();
            #endregion

            #region CustomFilters

            DateTime tmpDateTime;

            filter.DateFrom = DateTime.TryParse(request["DateFrom"], out tmpDateTime) ? tmpDateTime : DateTime.MinValue;
            filter.DateFromString = filter.DateFrom != DateTime.MinValue ? filter.DateFrom.ToString("dd/MM/yyyy") : string.Empty;

            filter.DateTo = DateTime.TryParse(request["DateTo"], out tmpDateTime) ? tmpDateTime : DateTime.MinValue;
            filter.DateToString = filter.DateTo != DateTime.MinValue ? filter.DateTo.ToString("dd/MM/yyyy") : string.Empty;

            if (!string.IsNullOrWhiteSpace(request["ClientEmail"]))
                filter.ClientEmail = request["ClientEmail"];

            if (!string.IsNullOrWhiteSpace(request["ClientName"]))
                filter.ClientName = request["ClientName"];

            if (!string.IsNullOrWhiteSpace(request["ClientSurname"]))
                filter.ClientSurname = request["ClientSurname"];

            if (!string.IsNullOrWhiteSpace(request["PaymentTransactionNumber"]))
                filter.PaymentTransactionNumber = request["PaymentTransactionNumber"];

            if (!string.IsNullOrWhiteSpace(request["PaymentUniqueIdentifier"]))
                filter.PaymentUniqueIdentifier = Convert.ToInt64(request["PaymentUniqueIdentifier"]);

            filter.GatewayId = !string.IsNullOrWhiteSpace(request["GatewayId"]) ? Guid.Parse(request["GatewayId"]) : default(Guid);

            filter.PaymentType = !string.IsNullOrWhiteSpace(request["PaymentType"]) ? Convert.ToInt32(request["PaymentType"]) : 0;

            filter.ServiceId = !string.IsNullOrWhiteSpace(request["ServiceId"]) ? Guid.Parse(request["ServiceId"]) : default(Guid);

            filter.ServiceCategoryId = !string.IsNullOrWhiteSpace(request["ServiceCategoryId"]) ? Guid.Parse(request["ServiceCategoryId"]) : default(Guid);

            filter.Platform = !string.IsNullOrWhiteSpace(request["Platform"]) ? Convert.ToInt32(request["Platform"]) : 0;

            if (!string.IsNullOrWhiteSpace(request["PaymentStatus"]))
                filter.PaymentStatus = Convert.ToInt32(request["PaymentStatus"]);

            //para GET por reporte de servicios asociados
            filter.ServiceAssociatedId = !string.IsNullOrWhiteSpace(request["ServiceAssociatedId"]) ? Guid.Parse(request["ServiceAssociatedId"]) : default(Guid);

            #endregion

            #region Sort
            var sortColumnIndex = Convert.ToInt32(request["iSortCol_0"]);

            var sortableDate = Convert.ToBoolean(request["bSortable_0"]);
            var sortableClientEmail = Convert.ToBoolean(request["bSortable_1"]);
            var sortableClientName = Convert.ToBoolean(request["bSortable_2"]);
            var sortableClientSurname = Convert.ToBoolean(request["bSortable_3"]);
            var sortableGateway = Convert.ToBoolean(request["bSortable_4"]);
            var sortablePaymentType = Convert.ToBoolean(request["bSortable_5"]);
            var sortableService = Convert.ToBoolean(request["bSortable_6"]);
            var sortableServiceCategory = Convert.ToBoolean(request["bSortable_7"]);
            var sortableCurrency = Convert.ToBoolean(request["bSortable_8"]);
            var sortableAmountPesos = Convert.ToBoolean(request["bSortable_9"]);
            var sortableBillTaxedAmount = Convert.ToBoolean(request["bSortable_10"]);
            var sortableBillDiscountAmount = Convert.ToBoolean(request["bSortable_11"]);
            var sortableTransactionNumber = Convert.ToBoolean(request["bSortable_12"]);
            var sortableUniqueIdentifier = Convert.ToBoolean(request["bSortable_13"]);
            var sortablePaymentStatus = Convert.ToBoolean(request["bSortable_14"]);
            var sortablePlatform = Convert.ToBoolean(request["bSortable_15"]);

            Func<int, string> orderingFunction = (c => sortColumnIndex == 0 && sortableDate ? "0" :
                                                       sortColumnIndex == 1 && sortableClientEmail ? "1" :
                                                       sortColumnIndex == 2 && sortableClientName ? "2" :
                                                       sortColumnIndex == 3 && sortableClientSurname ? "3" :
                                                       sortColumnIndex == 4 && sortableGateway ? "4" :
                                                       sortColumnIndex == 5 && sortablePaymentType ? "5" :
                                                       sortColumnIndex == 6 && sortableService ? "6" :
                                                       sortColumnIndex == 7 && sortableServiceCategory ? "7" :
                                                       sortColumnIndex == 8 && sortableAmountPesos ? "8" :
                                                       sortColumnIndex == 9 && sortableCurrency ? "9" :
                                                       sortColumnIndex == 10 && sortableBillTaxedAmount ? "10" :
                                                       sortColumnIndex == 11 && sortableBillDiscountAmount ? "11" :
                                                       sortColumnIndex == 12 && sortableTransactionNumber ? "12" :
                                                       sortColumnIndex == 13 && sortableUniqueIdentifier ? "13" :
                                                       sortColumnIndex == 14 && sortablePaymentStatus ? "14" :
                                                       sortColumnIndex == 15 && sortablePlatform ? "15" : "");

            filter.OrderBy = orderingFunction(sortColumnIndex);
            #endregion

            #region SortDirection

            //if (sortColumnIndex == 0)
            //{
            //    var sortDirection = request["sSortDir_0"]; // asc or desc
            //    filter.SortDirection = sortDirection == "asc" ? SortDirection.Desc : SortDirection.Asc;
            //}
            //else
            //{
            //    var sortDirection = request["sSortDir_0"]; // asc or desc
            //    filter.SortDirection = sortDirection == "asc" ? SortDirection.Asc : SortDirection.Desc;
            //}
            var sortDirection = request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filter.SortDirection = SortDirection.Asc;
            else
                filter.SortDirection = SortDirection.Desc;

            #endregion

            filter.DisplayStart = param.iDisplayStart;
            filter.DisplayLength = 10;

            return filter;
        }

        public static ReportsEmailsFilterDto AjaxHandlerEmails(HttpRequestBase request, JQueryDataTableParamModel param)
        {
            var filter = new ReportsEmailsFilterDto();

            //#region GenericFilter
            //if (!string.IsNullOrEmpty(param.sSearch) && param.sSearch != "undefined")
            //    filter.GenericSearch = param.sSearch.ToLower();
            //#endregion

            #region CustomFilters

            DateTime tmpDateTime;

            filter.DateFrom = DateTime.TryParse(request["DateFrom"], out tmpDateTime) ? tmpDateTime : DateTime.MinValue;
            filter.DateFromString = filter.DateFrom != DateTime.MinValue ? filter.DateFrom.ToString("dd/MM/yyyy") : string.Empty;

            filter.DateTo = DateTime.TryParse(request["DateTo"], out tmpDateTime) ? tmpDateTime : DateTime.MinValue;
            filter.DateToString = filter.DateTo != DateTime.MinValue ? filter.DateTo.ToString("dd/MM/yyyy") : string.Empty;

            int tmpInt;
            filter.Status = Int32.TryParse(request["Status"], out tmpInt) ? tmpInt : -1;
            filter.EmailType = Int32.TryParse(request["EmailType"], out tmpInt) ? tmpInt : -1;

            if (!string.IsNullOrWhiteSpace(request["To"]))
                filter.To = request["To"];

            #endregion

            #region Sort
            var sortColumnIndex = Convert.ToInt32(request["iSortCol_0"]);

            var sortableDate = Convert.ToBoolean(request["bSortable_0"]);
            var sortableStatus = Convert.ToBoolean(request["bSortable_1"]);
            var sortableType = Convert.ToBoolean(request["bSortable_2"]);
            var sortableEmail = Convert.ToBoolean(request["bSortable_3"]);

            Func<int, string> orderingFunction = (c =>
                sortColumnIndex == 0 && sortableDate ? "0" :
                sortColumnIndex == 1 && sortableStatus ? "1" :
                sortColumnIndex == 2 && sortableType ? "2" :
                sortColumnIndex == 3 && sortableEmail ? "3" : "");

            filter.OrderBy = orderingFunction(sortColumnIndex);
            #endregion

            #region SortDirection

            var sortDirection = request["sSortDir_0"]; // asc or desc
            filter.SortDirection = sortDirection == "asc" ? SortDirection.Asc : SortDirection.Desc;

            #endregion

            filter.DisplayStart = param.iDisplayStart;
            filter.DisplayLength = 10;

            return filter;
        }

        //public static AuditFilterDto AjaxHandlerAudit(HttpRequestBase request, JQueryDataTableParamModel param)
        //{
        //    var filter = new AuditFilterDto();

        //    #region GenericFilter
        //    if (!string.IsNullOrEmpty(param.sSearch))
        //        filter.GenericSearch = param.sSearch.ToLower();
        //    #endregion

        //    #region CustomFilters
        //    if (!string.IsNullOrWhiteSpace(request["sSearch_0"])
        //       || !string.IsNullOrWhiteSpace(request["sSearch_1"])
        //       || !string.IsNullOrWhiteSpace(request["sSearch_2"])
        //       || !string.IsNullOrWhiteSpace(request["sSearch_3"])
        //       || !string.IsNullOrWhiteSpace(request["sSearch_4"]))
        //    {
        //        var dateFrom = Convert.ToString(request["sSearch_0"]);
        //        var dateTo = Convert.ToString(request["sSearch_1"]);
        //        var user = Convert.ToString(request["sSearch_2"]);
        //        var logOperationType = !String.IsNullOrEmpty(request["sSearch_3"]) ? Convert.ToInt32(request["sSearch_3"]) : -1; 
        //        var logUserType = !String.IsNullOrEmpty(request["sSearch_4"]) ? Convert.ToInt32(request["sSearch_4"]) : -1;


        //        filter.From = DateTime.ParseExact(dateFrom, "dd/MM/yyyy", CultureInfo.InvariantCulture);
        //        filter.To = DateTime.ParseExact(dateTo, "dd/MM/yyyy", CultureInfo.InvariantCulture);
        //        filter.User = user;
        //        if(logOperationType > -1)
        //            filter.LogOperationType = (LogOperationType)logOperationType;
        //        else
        //            filter.LogOperationType = null;
        //        if (logUserType > -1)
        //            filter.LogUserType = (LogUserType)logUserType;
        //        else
        //            filter.LogUserType = null;

        //    }
        //    #endregion

        //    #region Sort
        //    var sortColumnIndex = Convert.ToInt32(request["iSortCol_0"]);

        //    var sortableDateTime = Convert.ToBoolean(request["bSortable_0"]);
        //    var sortableIP = Convert.ToBoolean(request["bSortable_1"]);
        //    var sortableLogUserType = Convert.ToBoolean(request["bSortable_2"]);
        //    var sortableLogOperationType = Convert.ToBoolean(request["bSortable_3"]);
        //    var sortableSystemUser = Convert.ToBoolean(request["bSortable_4"]);
        //    var sortableApplicationUser = Convert.ToBoolean(request["bSortable_5"]);
        //    var sortableAnonymousUser = Convert.ToBoolean(request["bSortable_6"]);


        //    Func<int, string> orderingFunction = (c => sortColumnIndex == 0 && sortableDateTime ? "0" :
        //                                               sortColumnIndex == 1 && sortableIP ? "1" :
        //                                               sortColumnIndex == 2 && sortableLogUserType ? "2" :
        //                                               sortColumnIndex == 3 && sortableLogOperationType ? "3" :
        //                                               sortColumnIndex == 4 && sortableSystemUser ? "4" :
        //                                               sortColumnIndex == 5 && sortableApplicationUser ? "5" :
        //                                               sortColumnIndex == 6 && sortableAnonymousUser ? "6": "");

        //    filter.OrderBy = orderingFunction(sortColumnIndex);
        //    #endregion


        //    #region SortDirection
        //    var sortDirection = request["sSortDir_0"]; // asc or desc
        //    if (sortDirection == "asc")
        //        filter.SortDirection = SortDirection.Asc;
        //    else
        //        filter.SortDirection = SortDirection.Desc;
        //    #endregion


        //    filter.DisplayStart = param.iDisplayStart;

        //    return filter;
        //}

        ////OLD

        public static AuditFilterDto AjaxHandlerAudit(HttpRequestBase request, JQueryDataTableParamModel param)
        {
            var filter = new AuditFilterDto();

            filter.DisplayStart = param.iDisplayStart;

            #region GenericFilter
            if (!string.IsNullOrEmpty(param.sSearch) && param.sSearch != "undefined")
                filter.GenericSearch = param.sSearch.ToLower();
            #endregion

            #region CustomFilters
            DateTime tmpDateTime;

            if (DateTime.TryParse(request["From"], out tmpDateTime))
                filter.From = tmpDateTime;

            filter.From = filter.From.AddHours(Convert.ToInt32(request["HoursFrom"])); //ADENTRO DEL IF??
            filter.From = filter.From.AddMinutes(Convert.ToInt32(request["MinutesFrom"]));

            if (DateTime.TryParse(request["To"], out tmpDateTime))
                filter.To = tmpDateTime;

            filter.To = filter.To.AddHours(Convert.ToInt32(request["HoursTo"]));
            filter.To = filter.To.AddMinutes(Convert.ToInt32(request["MinutesTo"]));

            LogOperationType logOperationTypeTmp;
            if (Enum.TryParse(request["LogOperationType"], out logOperationTypeTmp))
                filter.LogOperationType = logOperationTypeTmp;

            LogUserType logUserTypeTmp;
            if (Enum.TryParse(request["LogUserType"], out logUserTypeTmp))
                filter.LogUserType = logUserTypeTmp;

            if (!string.IsNullOrEmpty(request["User"]))
                filter.User = request["User"].Trim();

            if (!string.IsNullOrEmpty(request["Message"]))
                filter.Message = request["Message"];
            #endregion

            #region Sort
            var sortColumnIndex = Convert.ToInt32(request["iSortCol_0"]);

            var sortableDateTime = Convert.ToBoolean(request["bSortable_0"]);
            var sortableIp = Convert.ToBoolean(request["bSortable_1"]);
            var sortableLogUserType = Convert.ToBoolean(request["bSortable_2"]);
            var sortableLogOperationType = Convert.ToBoolean(request["bSortable_3"]);
            var sortableSystemUser = Convert.ToBoolean(request["bSortable_4"]);
            var sortableApplicationUser = Convert.ToBoolean(request["bSortable_5"]);
            var sortableAnonymousUser = Convert.ToBoolean(request["bSortable_6"]);

            Func<int, string> orderingFunction = (c => sortColumnIndex == 0 && sortableDateTime ? "0" :
                                                       sortColumnIndex == 1 && sortableIp ? "1" :
                                                       sortColumnIndex == 2 && sortableLogUserType ? "2" :
                                                       sortColumnIndex == 3 && sortableLogOperationType ? "3" :
                                                       sortColumnIndex == 4 && sortableSystemUser ? "4" :
                                                       sortColumnIndex == 5 && sortableApplicationUser ? "5" :
                                                       sortColumnIndex == 6 && sortableAnonymousUser ? "6" : "");

            filter.OrderBy = orderingFunction(sortColumnIndex);
            #endregion

            #region SortDirection
            var sortDirection = request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filter.SortDirection = SortDirection.Asc;
            else
                filter.SortDirection = SortDirection.Desc;
            #endregion

            return filter;
        }

        public static ReportsConciliationFilterDto AjaxHandlerReportsConciliation(HttpRequestBase request, JQueryDataTableParamModel param)
        {
            var filter = new ReportsConciliationFilterDto()
            {
                From = new DateTime(2015, 01, 01),
                To = DateTime.Now.AddDays(-2),
                SortDirection = SortDirection.Desc,
                DisplayStart = 0,
                State = "0",
            };

            filter.DisplayStart = param.iDisplayStart;
            filter.DisplayLength = param.iDisplayLength;

            #region GenericFilter
            if (!string.IsNullOrEmpty(param.sSearch) && param.sSearch != "undefined")
                filter.GenericSearch = param.sSearch.ToLower();

            #endregion

            #region CustomFilters

            DateTime tmpDateTime;

            if (DateTime.TryParse(request["From"], out tmpDateTime))
                filter.From = tmpDateTime;

            if (DateTime.TryParse(request["To"], out tmpDateTime))
            {
                filter.To = new DateTime(tmpDateTime.Year, tmpDateTime.Month, tmpDateTime.Day, 23, 59, 59);
            }

            filter.RequestId = request["RequestId"];
            filter.UniqueIdenfifier = request["UniqueIdenfifier"];
            filter.Comments = request["Comments"];

            if (!string.IsNullOrEmpty(request["Applications"]))
                filter.Applications = request["Applications"];

            if (!string.IsNullOrEmpty(request["State"]))
                filter.State = request["State"];

            if (!string.IsNullOrEmpty(request["Origin"]))
                filter.Origin = int.Parse(request["Origin"]);

            if (!string.IsNullOrEmpty(request["Email"]))
                filter.Email = request["Email"];
            #endregion

            return filter;
        }

        public static ReportsCardsFilterDto AjaxHandlerCard(HttpRequestBase request, JQueryDataTableParamModel param)
        {
            var filter = new ReportsCardsFilterDto();

            #region GenericFilter
            if (!string.IsNullOrEmpty(param.sSearch) && param.sSearch != "undefined")
                filter.GenericSearch = param.sSearch.ToLower();
            #endregion

            #region CustomFilters

            if (!string.IsNullOrWhiteSpace(request["ClientEmail"]))
                filter.ClientEmail = request["ClientEmail"];

            if (!string.IsNullOrWhiteSpace(request["ClientName"]))
                filter.ClientName = request["ClientName"];

            if (!string.IsNullOrWhiteSpace(request["ClientSurname"]))
                filter.ClientSurname = request["ClientSurname"];

            if (!string.IsNullOrWhiteSpace(request["CardMaskedNumber"]))
                filter.CardMaskedNumber = request["CardMaskedNumber"];

            if (!string.IsNullOrWhiteSpace(request["CardBin"]))
                filter.CardBin = request["CardBin"];

            if (!string.IsNullOrWhiteSpace(request["CardType"]))
                filter.CardType = Convert.ToInt32(request["CardType"]);

            if (!string.IsNullOrWhiteSpace(request["CardState"]))
                filter.CardState = Convert.ToInt32(request["CardState"]);

            #endregion

            #region Sort
            var sortColumnIndex = Convert.ToInt32(request["iSortCol_0"]);

            var sortableClientEmail = Convert.ToBoolean(request["bSortable_0"]);
            var sortableClientName = Convert.ToBoolean(request["bSortable_1"]);
            var sortableClientSurname = Convert.ToBoolean(request["bSortable_2"]);
            var sortableCardMaskedNumber = Convert.ToBoolean(request["bSortable_3"]);
            var sortableCardDueDate = Convert.ToBoolean(request["bSortable_4"]);
            var sortableCardBin = Convert.ToBoolean(request["bSortable_5"]);
            var sortableCardType = Convert.ToBoolean(request["bSortable_6"]);
            var sortableCardActive = Convert.ToBoolean(request["bSortable_7"]);
            var sortableCardDeleted = Convert.ToBoolean(request["bSortable_8"]);

            Func<int, string> orderingFunction = (c => sortColumnIndex == 0 && sortableClientEmail ? "0" :
                                                       sortColumnIndex == 1 && sortableClientName ? "1" :
                                                       sortColumnIndex == 2 && sortableClientSurname ? "2" :
                                                       sortColumnIndex == 3 && sortableCardMaskedNumber ? "3" :
                                                       sortColumnIndex == 4 && sortableCardDueDate ? "4" :
                                                       sortColumnIndex == 5 && sortableCardBin ? "5" :
                                                       sortColumnIndex == 6 && sortableCardType ? "6" :
                                                       sortColumnIndex == 7 && sortableCardActive ? "7" :
                                                       sortColumnIndex == 8 && sortableCardDeleted ? "8" : "");

            filter.OrderBy = orderingFunction(sortColumnIndex);
            #endregion

            #region SortDirection
            var sortDirection = request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filter.SortDirection = SortDirection.Asc;
            else
                filter.SortDirection = SortDirection.Desc;
            #endregion

            filter.DisplayStart = param.iDisplayStart;

            return filter;
        }

        public static ReportsTc33FilterDto AjaxHandlerTc33(HttpRequestBase request, JQueryDataTableParamModel param)
        {
            var filter = new ReportsTc33FilterDto();

            #region GenericFilter
            if (!string.IsNullOrEmpty(param.sSearch) && param.sSearch != "undefined")
                filter.GenericSearch = param.sSearch.ToLower();
            #endregion

            #region CustomFilters

            DateTime tmpDateTime;

            filter.CreationDateFrom = DateTime.TryParse(request["CreationDateFrom"], out tmpDateTime) ? tmpDateTime : DateTime.MinValue;
            filter.CreationDateFromString = filter.CreationDateFrom != DateTime.MinValue ? filter.CreationDateFrom.ToString("dd/MM/yyyy") : string.Empty;

            filter.CreationDateTo = DateTime.TryParse(request["CreationDateTo"], out tmpDateTime) ? tmpDateTime : DateTime.MinValue;
            filter.CreationDateToString = filter.CreationDateTo != DateTime.MinValue ? filter.CreationDateTo.ToString("dd/MM/yyyy") : string.Empty;

            if (!string.IsNullOrWhiteSpace(request["InputFileName"]))
                filter.InputFileName = request["InputFileName"];

            if (!string.IsNullOrWhiteSpace(request["Transaction"]))
                filter.Transaction = request["Transaction"];

            #endregion

            #region Sort
            var sortColumnIndex = Convert.ToInt32(request["iSortCol_0"]);

            var sortableDate = Convert.ToBoolean(request["bSortable_0"]);
            var sortableInputName = Convert.ToBoolean(request["bSortable_1"]);

            Func<int, string> orderingFunction = (c => sortColumnIndex == 0 && sortableDate ? "0" :
                                                       sortColumnIndex == 1 && sortableInputName ? "1" : "");

            filter.OrderBy = orderingFunction(sortColumnIndex);
            #endregion

            #region SortDirection
            var sortDirection = request["sSortDir_0"]; // asc or desc
            filter.SortDirection = sortDirection == "asc" ? SortDirection.Asc : SortDirection.Desc;
            #endregion

            filter.DisplayStart = param.iDisplayStart;
            filter.DisplayLength = 10;

            return filter;
        }

        public static ReportsHighwayEmailFilterDto AjaxHandlerHighwayEmail(HttpRequestBase request, JQueryDataTableParamModel param)
        {
            var filter = new ReportsHighwayEmailFilterDto();

            filter.DisplayStart = param.iDisplayStart;

            #region GenericFilter
            if (!string.IsNullOrEmpty(param.sSearch) && param.sSearch != "undefined")
                filter.GenericSearch = param.sSearch.ToLower();
            #endregion

            #region CustomFilters
            DateTime tmpDateTime;

            if (DateTime.TryParse(request["From"], out tmpDateTime))
                filter.From = tmpDateTime;

            if (DateTime.TryParse(request["To"], out tmpDateTime))
                filter.To = tmpDateTime;

            if (!string.IsNullOrEmpty(request["Email"]))
                filter.Email = request["Email"].Trim();

            if (!string.IsNullOrEmpty(request["Commerce"]))
            {
                Int32 comm = 0;
                var r = Int32.TryParse(request["Commerce"], out comm);
                if (r)
                    filter.Commerce = comm;
            }

            if (!string.IsNullOrEmpty(request["Branch"]))
            {
                Int32 comm = 0;
                var r = Int32.TryParse(request["Branch"], out comm);
                if (r)
                    filter.Branch = comm;
            }
            #endregion

            #region Sort
            var sortColumnIndex = Convert.ToInt32(request["iSortCol_0"]);


            var sortableTransaction = Convert.ToBoolean(request["bSortable_0"]);
            var sortableCommerce = Convert.ToBoolean(request["bSortable_1"]);
            var sortableBranch = Convert.ToBoolean(request["bSortable_2"]);
            var sortableEmail = Convert.ToBoolean(request["bSortable_3"]);
            var sortableCreationDate = Convert.ToBoolean(request["bSortable_4"]);
            var sortableState = Convert.ToBoolean(request["bSortable_5"]);
            var sortableInputName = Convert.ToBoolean(request["bSortable_6"]);
            var sortableOutputName = Convert.ToBoolean(request["bSortable_7"]);

            Func<int, string> orderingFunction = (c => sortColumnIndex == 0 && sortableTransaction ? "0" :
                                                       sortColumnIndex == 1 && sortableCommerce ? "1" :
                                                       sortColumnIndex == 2 && sortableBranch ? "2" :
                                                       sortColumnIndex == 3 && sortableEmail ? "3" :
                                                       sortColumnIndex == 4 && sortableCreationDate ? "4" :
                                                       sortColumnIndex == 5 && sortableState ? "5" :
                                                       sortColumnIndex == 6 && sortableInputName ? "6" :
                                                       sortColumnIndex == 7 && sortableOutputName ? "7" : "");

            filter.OrderBy = orderingFunction(sortColumnIndex);
            #endregion

            #region SortDirection
            var sortDirection = request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filter.SortDirection = SortDirection.Asc;
            else
                filter.SortDirection = SortDirection.Desc;
            #endregion

            return filter;
        }

        public static ReportsHighwayBillFilterDto AjaxHandlerHighwayBill(HttpRequestBase request, JQueryDataTableParamModel param)
        {
            var filter = new ReportsHighwayBillFilterDto();

            filter.DisplayStart = param.iDisplayStart;

            #region GenericFilter
            if (!string.IsNullOrEmpty(param.sSearch) && param.sSearch != "undefined")
                filter.GenericSearch = param.sSearch.ToLower();
            #endregion

            #region CustomFilters

            DateTime tmpDateTime;

            filter.From = DateTime.TryParse(request["From"], out tmpDateTime) ? tmpDateTime : DateTime.MinValue;
            filter.DateFromString = filter.From != DateTime.MinValue ? filter.From.ToString("dd/MM/yyyy") : string.Empty;

            filter.To = DateTime.TryParse(request["To"], out tmpDateTime) ? tmpDateTime : DateTime.MinValue;
            filter.DateToString = filter.To != DateTime.MinValue ? filter.To.ToString("dd/MM/yyyy") : string.Empty;

            if (!string.IsNullOrEmpty(request["CodComercio"]))
            {
                Int32 comm = 0;
                var r = Int32.TryParse(request["CodComercio"], out comm);
                if (r)
                    filter.CodComercio = comm;
            }

            if (!string.IsNullOrEmpty(request["CodSucursal"]))
            {
                Int32 suc = 0;
                var s = Int32.TryParse(request["CodSucursal"], out suc);
                if (s)
                    filter.CodSucursal = suc;
            }

            if (!string.IsNullOrEmpty(request["NroFactura"]))
                filter.NroFactura = request["NroFactura"];
            #endregion

            #region Sort
            var sortColumnIndex = Convert.ToInt32(request["iSortCol_0"]);

            var sortableCreationDate = Convert.ToBoolean(request["bSortable_0"]);
            var sortableCodComercio = Convert.ToBoolean(request["bSortable_1"]);
            var sortableCodSucursal = Convert.ToBoolean(request["bSortable_2"]);
            var sortableRefCliente = Convert.ToBoolean(request["bSortable_3"]);
            var sortableServiceName = Convert.ToBoolean(request["bSortable_4"]);
            var sortableNroFactura = Convert.ToBoolean(request["bSortable_5"]);
            var sortableFchFactura = Convert.ToBoolean(request["bSortable_6"]);
            var sortableFchVencimiento = Convert.ToBoolean(request["bSortable_7"]);
            var sortableDiasPagoVenc = Convert.ToBoolean(request["bSortable_8"]);
            var sortableMoneda = Convert.ToBoolean(request["bSortable_9"]);
            var sortableMontoTotal = Convert.ToBoolean(request["bSortable_10"]);
            var sortableMontoMinimo = Convert.ToBoolean(request["bSortable_11"]);
            var sortableMontoGravado = Convert.ToBoolean(request["bSortable_12"]);
            var sortableConsFinal = Convert.ToBoolean(request["bSortable_13"]);
            //var sortableCuotas = Convert.ToBoolean(request["bSortable_14"]);

            Func<int, string> orderingFunction = (c => sortColumnIndex == 0 && sortableCreationDate ? "0" :
                                                       sortColumnIndex == 1 && sortableCodComercio ? "1" :
                                                       sortColumnIndex == 2 && sortableCodSucursal ? "2" :
                                                       sortColumnIndex == 3 && sortableRefCliente ? "3" :
                                                       sortColumnIndex == 4 && sortableServiceName ? "4" :
                                                       sortColumnIndex == 5 && sortableNroFactura ? "5" :
                                                       sortColumnIndex == 6 && sortableFchFactura ? "6" :
                                                       sortColumnIndex == 7 && sortableFchVencimiento ? "7" :
                                                       sortColumnIndex == 8 && sortableDiasPagoVenc ? "8" :
                                                       sortColumnIndex == 9 && sortableMoneda ? "9" :
                                                       sortColumnIndex == 10 && sortableMontoTotal ? "10" :
                                                       sortColumnIndex == 11 && sortableMontoMinimo ? "11" :
                                                       sortColumnIndex == 12 && sortableMontoGravado ? "12" :
                                                       sortColumnIndex == 13 && sortableConsFinal ? "13" : "");
            //sortColumnIndex == 14 && sortableCuotas ? "14" : "");

            filter.OrderBy = orderingFunction(sortColumnIndex);
            #endregion

            #region SortDirection
            var sortDirection = request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filter.SortDirection = SortDirection.Asc;
            else
                filter.SortDirection = SortDirection.Desc;
            #endregion

            return filter;
        }

        public static ReportsIntegrationFilterDto AjaxHandlerReportsIntegration(HttpRequestBase request, JQueryDataTableParamModel param)
        {
            var filter = new ReportsIntegrationFilterDto();

            filter.DisplayStart = param.iDisplayStart;

            #region GenericFilter
            if (!string.IsNullOrEmpty(param.sSearch) && param.sSearch != "undefined")
                filter.GenericSearch = param.sSearch.ToLower();
            #endregion

            #region CustomFilters

            if (!string.IsNullOrEmpty(request["ExternalRequestType"]))
                filter.ExternalRequestType = Convert.ToInt32(request["ExternalRequestType"]);

            DateTime tmpDateTime;

            filter.DateFrom = DateTime.TryParse(request["DateFrom"], out tmpDateTime) ? tmpDateTime : DateTime.MinValue;
            filter.DateFromString = filter.DateFrom != DateTime.MinValue ? filter.DateFrom.ToString("dd/MM/yyyy") : string.Empty;

            filter.DateTo = DateTime.TryParse(request["DateTo"], out tmpDateTime) ? tmpDateTime : DateTime.MinValue;
            filter.DateToString = filter.DateTo != DateTime.MinValue ? filter.DateTo.ToString("dd/MM/yyyy") : string.Empty;

            if (!string.IsNullOrEmpty(request["IdOperation"]))
                filter.IdOperation = request["IdOperation"];

            if (!string.IsNullOrEmpty(request["IdApp"]))
                filter.IdApp = request["IdApp"];

            if (!string.IsNullOrEmpty(request["TransactionNumber"]))
                filter.TransactionNumber = request["TransactionNumber"];
            #endregion

            #region Sort
            var sortColumnIndex = Convert.ToInt32(request["iSortCol_0"]);

            filter.OrderBy = "0";
            #endregion

            #region SortDirection
            var sortDirection = request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filter.SortDirection = SortDirection.Asc;
            else
                filter.SortDirection = SortDirection.Desc;
            #endregion

            return filter;
        }

        public static ChangeTrackerFilterDto AjaxHandlerChangeTracker(HttpRequestBase request, JQueryDataTableParamModel param)
        {
            var filter = new ChangeTrackerFilterDto();

            filter.DisplayStart = param.iDisplayStart;

            #region GenericFilter
            if (!string.IsNullOrEmpty(param.sSearch) && param.sSearch != "undefined")
                filter.GenericSearch = param.sSearch.ToLower();
            #endregion

            #region CustomFilters
            DateTime tmpDateTime;

            if (DateTime.TryParse(request["From"], out tmpDateTime))
                filter.From = tmpDateTime;

            filter.From = filter.From.AddHours(Convert.ToInt32(request["HoursFrom"])); //ADENTRO DEL IF??
            filter.From = filter.From.AddMinutes(Convert.ToInt32(request["MinutesFrom"]));

            if (DateTime.TryParse(request["To"], out tmpDateTime))
                filter.To = tmpDateTime;

            filter.To = filter.To.AddHours(Convert.ToInt32(request["HoursTo"]));
            filter.To = filter.To.AddMinutes(Convert.ToInt32(request["MinutesTo"]));

            if (!string.IsNullOrEmpty(request["UserName"]))
                filter.UserName = request["UserName"].Trim();

            EventTypeDto eventTypeDto;
            if (Enum.TryParse(request["EventType"], out eventTypeDto))
                filter.EventType = eventTypeDto;

            if (!string.IsNullOrEmpty(request["TableName"]))
                filter.TableName = request["TableName"];

            if (!string.IsNullOrEmpty(request["AditionalInfo"]))
                filter.AditionalInfo = request["AditionalInfo"];

            #endregion

            #region Sort
            var sortColumnIndex = Convert.ToInt32(request["iSortCol_0"]);

            var sortableDateTime = Convert.ToBoolean(request["bSortable_0"]);
            var sortableIp = Convert.ToBoolean(request["bSortable_1"]);
            var sortableEventType = Convert.ToBoolean(request["bSortable_2"]);
            var sortableTableName = Convert.ToBoolean(request["bSortable_3"]);
            var sortableUserName = Convert.ToBoolean(request["bSortable_4"]);
            var sortableAditionalInfo = Convert.ToBoolean(request["bSortable_5"]);

            Func<int, string> orderingFunction = (c => sortColumnIndex == 0 && sortableDateTime ? GetPropertiesNames.GetPropertyName<AuditLogDto>(p => p.EventDate) :
                                                       sortColumnIndex == 1 && sortableIp ? GetPropertiesNames.GetPropertyName<AuditLogDto>(p => p.IP) :
                                                       sortColumnIndex == 2 && sortableEventType ? GetPropertiesNames.GetPropertyName<AuditLogDto>(p => p.EventType) :
                                                       sortColumnIndex == 3 && sortableTableName ? GetPropertiesNames.GetPropertyName<AuditLogDto>(p => p.TableName) :
                                                       sortColumnIndex == 4 && sortableUserName ? GetPropertiesNames.GetPropertyName<AuditLogDto>(p => p.UserName) :
                                                       sortColumnIndex == 5 && sortableAditionalInfo ? GetPropertiesNames.GetPropertyName<AuditLogDto>(p => p.AditionalInfo) :
                                                       "");

            filter.OrderBy = orderingFunction(sortColumnIndex);
            #endregion

            #region SortDirection
            var sortDirection = request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filter.SortDirection = SortDirection.Asc;
            else
                filter.SortDirection = SortDirection.Desc;
            #endregion

            return filter;
        }

        public static FixedNotificationFilterDto AjaxHandlerFixedNotification(HttpRequestBase request, JQueryDataTableParamModel param)
        {
            var filter = new FixedNotificationFilterDto();

            #region GenericFilter
            if (!string.IsNullOrEmpty(param.sSearch))
                filter.GenericSearch = param.sSearch.ToLower();
            #endregion

            #region CustomFilters
            DateTime tmpDateTime;
            if (DateTime.TryParse(request["From"], out tmpDateTime))
                filter.From = tmpDateTime;

            if (DateTime.TryParse(request["To"], out tmpDateTime))
                filter.To = tmpDateTime.AddDays(1);

            if (!string.IsNullOrWhiteSpace(request["Description"])
               || !string.IsNullOrWhiteSpace(request["Detail"])
               || !string.IsNullOrWhiteSpace(request["Resolved"])
               || !string.IsNullOrWhiteSpace(request["Level"])
               || !string.IsNullOrWhiteSpace(request["Category"]))
            {
                var description = Convert.ToString(request["Description"]);
                var detail = Convert.ToString(request["Detail"]);
                bool? resolved = string.IsNullOrWhiteSpace(request["Resolved"]) ? (bool?)null : Convert.ToBoolean(request["Resolved"]);
                var level = string.IsNullOrEmpty(request["Level"]) ? null : (FixedNotificationLevelDto?)int.Parse(request["Level"]);
                var category = string.IsNullOrEmpty(request["Category"]) ? null : (FixedNotificationCategoryDto?)int.Parse(request["Category"]);


                filter.Description = description;
                filter.Detail = detail;
                filter.Resolved = resolved;
                filter.Level = level;
                filter.Category = category;
            }
            #endregion

            #region Sort
            var sortColumnIndex = Convert.ToInt32(request["iSortCol_0"]);

            var sortableDateTime = Convert.ToBoolean(request["bSortable_0"]);
            var sortableDescription = Convert.ToBoolean(request["bSortable_1"]);
            var sortableDetail = Convert.ToBoolean(request["bSortable_2"]);
            var sortableResolved = Convert.ToBoolean(request["bSortable_3"]);
            var sortableLevel = Convert.ToBoolean(request["bSortable_4"]);
            var sortableCategory = Convert.ToBoolean(request["bSortable_5"]);


            Func<int, string> orderingFunction = (c => sortColumnIndex == 0 && sortableDateTime ? GetPropertiesNames.GetPropertyName<FixedNotificationDto>(x => x.DateTime) :
                                                       sortColumnIndex == 1 && sortableDescription ? GetPropertiesNames.GetPropertyName<FixedNotificationDto>(x => x.Description) :
                                                       sortColumnIndex == 2 && sortableDetail ? GetPropertiesNames.GetPropertyName<FixedNotificationDto>(x => x.Detail) :
                                                       sortColumnIndex == 3 && sortableResolved ? GetPropertiesNames.GetPropertyName<FixedNotificationDto>(x => x.Resolved) :
                                                       sortColumnIndex == 4 && sortableLevel ? GetPropertiesNames.GetPropertyName<FixedNotificationDto>(x => x.Level) :
                                                       sortColumnIndex == 5 && sortableCategory ? GetPropertiesNames.GetPropertyName<FixedNotificationDto>(x => x.Category) : "");

            filter.OrderBy = orderingFunction(sortColumnIndex);

            #endregion

            #region SortDirection
            var sortDirection = request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filter.SortDirection = SortDirection.Asc;
            else
                filter.SortDirection = SortDirection.Desc;
            #endregion

            filter.DisplayStart = param.iDisplayStart;

            return filter;
        }

        public static ReportsAutomaticPaymentsFilterDto AjaxHandlerAutomaticPayments(HttpRequestBase request, JQueryDataTableParamModel param)
        {
            var filter = new ReportsAutomaticPaymentsFilterDto();

            #region GenericFilter
            if (!string.IsNullOrEmpty(param.sSearch) && param.sSearch != "undefined")
                filter.GenericSearch = param.sSearch.ToLower();
            #endregion

            #region CustomFilters

            DateTime tmpDateTime;

            filter.CreationDateFrom = DateTime.TryParse(request["CreationDateFrom"], out tmpDateTime) ? tmpDateTime : DateTime.MinValue;
            filter.CreationDateFromString = filter.CreationDateFrom != DateTime.MinValue ? filter.CreationDateFrom.ToString("dd/MM/yyyy") : string.Empty;

            filter.CreationDateTo = DateTime.TryParse(request["CreationDateTo"], out tmpDateTime) ? tmpDateTime : DateTime.MinValue;
            filter.CreationDateToString = filter.CreationDateTo != DateTime.MinValue ? filter.CreationDateTo.ToString("dd/MM/yyyy") : string.Empty;

            if (!string.IsNullOrWhiteSpace(request["ClientEmail"]))
                filter.ClientEmail = request["ClientEmail"];

            if (!string.IsNullOrWhiteSpace(request["ServiceNameAndDesc"]))
                filter.ServiceNameAndDesc = request["ServiceNameAndDesc"];

            //para GET por reporte de servicios asociados
            filter.ServiceAssociatedId = !string.IsNullOrWhiteSpace(request["ServiceAssociatedId"]) ? Guid.Parse(request["ServiceAssociatedId"]) : default(Guid);

            #endregion

            #region Sort
            var sortColumnIndex = Convert.ToInt32(request["iSortCol_0"]);

            var sortableClientEmail = Convert.ToBoolean(request["bSortable_0"]);
            var sortableServiceNameAndDesc = Convert.ToBoolean(request["bSortable_1"]);
            var sortableMaximumAmount = Convert.ToBoolean(request["bSortable_2"]);
            var sortableDaysBeforeDueDate = Convert.ToBoolean(request["bSortable_3"]);
            var sortableQuotas = Convert.ToBoolean(request["bSortable_4"]);
            var sortableSuciveAnual = Convert.ToBoolean(request["bSortable_5"]);
            var sortablePaymentsCount = Convert.ToBoolean(request["bSortable_6"]);
            var sortablePaymentsAmountPesos = Convert.ToBoolean(request["bSortable_7"]);
            var sortablePaymentsAmountDollars = Convert.ToBoolean(request["bSortable_8"]);
            var sortableCreationDate = Convert.ToBoolean(request["bSortable_9"]);

            Func<int, string> orderingFunction = (c => sortColumnIndex == 0 && sortableClientEmail ? "0" :
                                                       sortColumnIndex == 1 && sortableServiceNameAndDesc ? "1" :
                                                       sortColumnIndex == 2 && sortableMaximumAmount ? "2" :
                                                       sortColumnIndex == 3 && sortableDaysBeforeDueDate ? "3" :
                                                       sortColumnIndex == 4 && sortableQuotas ? "4" :
                                                       sortColumnIndex == 5 && sortableSuciveAnual ? "5" :
                                                       sortColumnIndex == 6 && sortablePaymentsCount ? "6" :
                                                       sortColumnIndex == 7 && sortablePaymentsAmountPesos ? "7" :
                                                       sortColumnIndex == 8 && sortablePaymentsAmountDollars ? "8" :
                                                       sortColumnIndex == 9 && sortableCreationDate ? "9" : "");

            filter.OrderBy = orderingFunction(sortColumnIndex);
            #endregion

            #region SortDirection

            if (sortColumnIndex == 0)
            {
                var sortDirection = request["sSortDir_0"]; // asc or desc
                filter.SortDirection = sortDirection == "asc" ? SortDirection.Desc : SortDirection.Asc;
            }
            else
            {
                var sortDirection = request["sSortDir_0"]; // asc or desc
                filter.SortDirection = sortDirection == "asc" ? SortDirection.Asc : SortDirection.Desc;
            }

            #endregion

            filter.DisplayStart = param.iDisplayStart;
            filter.DisplayLength = 10;

            return filter;
        }

        public static DailyConciliationFilterDto AjaxHandlerDailyConciliation(HttpRequestBase request, JQueryDataTableParamModel param)
        {
            var filter = new DailyConciliationFilterDto();

            filter.DisplayStart = param.iDisplayStart;
            filter.DisplayLength = param.iDisplayLength;

            #region GenericFilter
            if (!string.IsNullOrEmpty(param.sSearch) && param.sSearch != "undefined")
                filter.GenericSearch = param.sSearch.ToLower();

            #endregion

            #region CustomFilters

            DateTime tmpDateTime;

            if (DateTime.TryParse(request["From"], out tmpDateTime))
                filter.From = tmpDateTime;

            if (DateTime.TryParse(request["To"], out tmpDateTime))
            {
                filter.To = tmpDateTime.AddDays(1);
            }

            if (!string.IsNullOrEmpty(request["State"]))
                filter.State = int.Parse(request["State"]);

            if (!string.IsNullOrEmpty(request["Applications"]))
                filter.Applications = request["Applications"].Split(',').Select(int.Parse).ToList();
            #endregion

            return filter;
        }

        public static ReportsUserFilterDto AjaxHandlerReportsUser(HttpRequestBase request, JQueryDataTableParamModel param)
        {
            var filter = new ReportsUserFilterDto();

            #region CustomFilters
            DateTime tmpDateTime;
            filter.DateFrom = DateTime.TryParse(request["DateFrom"], out tmpDateTime) ? tmpDateTime : new DateTime(2015, 01, 01);
            filter.DateTo = DateTime.TryParse(request["DateTo"], out tmpDateTime) ? tmpDateTime : DateTime.Now;
            filter.Email = request["Email"];
            int tmpInt;
            filter.UserType = (UserType)(Int32.TryParse(request["UserType"], out tmpInt) ? tmpInt : 0);
            filter.ActiveOrInactive = (ActiveOrInactiveEnumDto)(Int32.TryParse(request["ActiveOrInactive"], out tmpInt) ? tmpInt : 0);
            #endregion

            #region Sort
            var sortColumnIndex = Convert.ToInt32(request["iSortCol_0"]);

            var sortableDate = Convert.ToBoolean(request["bSortable_0"]);
            var sortableEmail = Convert.ToBoolean(request["bSortable_1"]);
            var sortableActive = Convert.ToBoolean(request["bSortable_5"]);
            var sortableBlocked = Convert.ToBoolean(request["bSortable_11"]);

            Func<int, string> orderingFunction = (c =>
                sortColumnIndex == 0 && sortableDate ? "0" :
                sortColumnIndex == 1 && sortableEmail ? "1" :
                sortColumnIndex == 5 && sortableActive ? "5" :
                sortColumnIndex == 11 && sortableBlocked ? "11" : "");

            filter.OrderBy = orderingFunction(sortColumnIndex);
            #endregion

            #region SortDirection

            var sortDirection = request["sSortDir_0"]; // asc or desc
            filter.SortDirection = sortDirection == "asc" ? SortDirection.Asc : SortDirection.Desc;

            #endregion

            filter.DisplayStart = param.iDisplayStart;
            filter.DisplayLength = 10;

            return filter;
        }

        public static ReportsUserVonFilterDto AjaxHandlerReportsUserVON(HttpRequestBase request, JQueryDataTableParamModel param)
        {
            var filter = new ReportsUserVonFilterDto();

            #region CustomFilters
            DateTime tmpDateTime;
            filter.DateFrom = DateTime.TryParse(request["DateFrom"], out tmpDateTime) ? tmpDateTime : new DateTime(2015, 01, 01);
            filter.DateTo = DateTime.TryParse(request["DateTo"], out tmpDateTime) ? tmpDateTime : DateTime.Now;
            filter.Email = request["Email"];
            filter.Service = request["Service"];

            #endregion

            var sortColumnIndex = Convert.ToInt32(request["iSortCol_0"]);

            var sortableDate = Convert.ToBoolean(request["bSortable_1"]);
            var sortableEmail = Convert.ToBoolean(request["bSortable_2"]);
            var sortableService = Convert.ToBoolean(request["bSortable_7"]);

            Func<int, string> orderingFunction = (c => sortColumnIndex == 0 ? "1" :
                                                       sortColumnIndex == 1 && sortableDate ? "1" :
                                                       sortColumnIndex == 2 && sortableEmail ? "2" :
                                                       sortColumnIndex == 7 && sortableService ? "7" : "");

            filter.OrderBy = orderingFunction(sortColumnIndex);

            #region SortDirection

            var sortDirection = request["sSortDir_0"]; // asc or desc
            filter.SortDirection = sortDirection == "asc" ? SortDirection.Asc : SortDirection.Desc;

            #endregion

            filter.DisplayStart = param.iDisplayStart;
            filter.DisplayLength = 10;

            return filter;
        }

        public static BinGroupFilterDto AjaxHandlerBinGroup(HttpRequestBase request, JQueryDataTableParamModel param)
        {
            var filter = new BinGroupFilterDto();

            #region GenericFilter
            //if (!string.IsNullOrEmpty(param.sSearch))
            //    filter.GenericSearch = param.sSearch.ToLower();
            #endregion

            #region CustomFilters
            if (!string.IsNullOrWhiteSpace(request["Name"]))
            {
                var nameFilter = Convert.ToString(request["Name"]);

                filter.Name = nameFilter;
            }
            #endregion

            #region Sort
            var sortColumnIndex = Convert.ToInt32(request["iSortCol_0"]);

            var sortableName = Convert.ToBoolean(request["bSortable_0"]);


            Func<int, string> orderingFunction = (c => sortColumnIndex == 0 && sortableName ? "Name" : "");

            filter.OrderBy = orderingFunction(sortColumnIndex);
            #endregion

            #region SortDirection

            var sortDirection = request["sSortDir_0"]; // asc or desc
            filter.SortDirection = sortDirection == "asc" ? SortDirection.Asc : SortDirection.Desc;

            #endregion

            filter.DisplayStart = param.iDisplayStart;
            filter.DisplayLength = 10;

            return filter;
        }

        public static InterpreterFilterDto AjaxHandlerInterpreter(HttpRequestBase request, JQueryDataTableParamModel param)
        {
            var filter = new InterpreterFilterDto();

            #region CustomFilters
            filter.Name = request["Name"];
            #endregion

            #region Sort
            var sortColumnIndex = Convert.ToInt32(request["iSortCol_0"]);
            var sortableName = Convert.ToBoolean(request["bSortable_0"]);
            var sortableCode = Convert.ToBoolean(request["bSortable_1"]);

            Func<int, string> orderingFunction = (c => sortColumnIndex == 0 && sortableName ? "0" : "");

            filter.OrderBy = orderingFunction(sortColumnIndex);
            #endregion

            #region SortDirection
            var sortDirection = request["sSortDir_0"]; // asc or desc
            filter.SortDirection = sortDirection == "asc" ? SortDirection.Asc : SortDirection.Desc;
            #endregion

            filter.DisplayStart = param.iDisplayStart;
            filter.DisplayLength = 10;

            return filter;
        }

        public static BankFilterDto AjaxHandlerBank(HttpRequestBase request, JQueryDataTableParamModel param)
        {
            var filter = new BankFilterDto();

            #region CustomFilters
            filter.Name = request["Name"];
            filter.Code = string.IsNullOrEmpty(request["Code"]) ? 0 : int.Parse(request["Code"]);
            filter.BinValue = string.IsNullOrEmpty(request["BinValue"]) ? 0 : int.Parse(request["BinValue"]);
            #endregion

            #region Sort
            var sortColumnIndex = Convert.ToInt32(request["iSortCol_0"]);
            var sortableName = Convert.ToBoolean(request["bSortable_0"]);
            var sortableCode = Convert.ToBoolean(request["bSortable_1"]);

            Func<int, string> orderingFunction = (c => sortColumnIndex == 0 && sortableName ? "0" :
                                                       sortColumnIndex == 1 && sortableCode ? "1" : "");

            filter.OrderBy = orderingFunction(sortColumnIndex);
            #endregion

            #region SortDirection
            var sortDirection = request["sSortDir_0"]; // asc or desc
            filter.SortDirection = sortDirection == "asc" ? SortDirection.Asc : SortDirection.Desc;
            #endregion

            filter.DisplayStart = param.iDisplayStart;
            filter.DisplayLength = 10;

            return filter;
        }

        public static LifApiBillFilterDto AjaxHandlerLifApiBill(HttpRequestBase request, JQueryDataTableParamModel param)
        {
            var filter = new LifApiBillFilterDto();

            #region CustomFilters
            filter.IdOperation = request["IdOperation"];
            filter.IdApp = request["IdApp"];
            filter.LawIndi = string.IsNullOrEmpty(request["LawIndi"]) ? string.Empty : request["LawIndi"];

            DateTime tmpDateTime;

            filter.DateFrom = DateTime.TryParse(request["CreationDateFrom"], out tmpDateTime) ? tmpDateTime : DateTime.MinValue;
            filter.DateFromString = filter.DateFrom != DateTime.MinValue ? filter.DateFrom.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty;

            filter.DateTo = DateTime.TryParse(request["CreationDateTo"], out tmpDateTime) ? tmpDateTime : DateTime.MinValue;
            filter.DateToString = filter.DateTo != DateTime.MinValue ? filter.DateTo.ToString("dd/MM/yyyy HH:mm:ss") : string.Empty;
            #endregion

            #region Sort
            var sortColumnIndex = Convert.ToInt32(request["iSortCol_0"]);

            var sortableDate = Convert.ToBoolean(request["bSortable_0"]);
            var sortableIdApp = Convert.ToBoolean(request["bSortable_1"]);
            var sortableIdOperation = Convert.ToBoolean(request["bSortable_2"]);
            var sortableAmount = Convert.ToBoolean(request["bSortable_3"]);
            var sortableTaxAmount = Convert.ToBoolean(request["bSortable_4"]);

            Func<int, string> orderingFunction = (c =>
                sortColumnIndex == 0 && sortableDate ? "0" :
                sortColumnIndex == 1 && sortableIdApp ? "1" :
                sortColumnIndex == 2 && sortableIdOperation ? "2" :
                sortColumnIndex == 3 && sortableAmount ? "3" :
                sortColumnIndex == 4 && sortableTaxAmount ? "4" : "");

            filter.OrderBy = orderingFunction(sortColumnIndex);
            #endregion

            #region SortDirection
            var sortDirection = request["sSortDir_0"]; // asc or desc
            filter.SortDirection = sortDirection == "asc" ? SortDirection.Asc : SortDirection.Desc;
            #endregion

            filter.DisplayStart = param.iDisplayStart;
            filter.DisplayLength = 10;

            return filter;
        }

        public static CustomerSiteCommerceFilterDto AjaxHandlerCustomerSiteCommerce(HttpRequestBase request, JQueryDataTableParamModel param)
        {
            var filter = new CustomerSiteCommerceFilterDto();

            #region CustomFilters
            filter.Name = request["Name"];
            filter.Service = !string.IsNullOrWhiteSpace(request["Service"]) ? Guid.Parse(request["Service"]) : default(Guid);
            filter.IsDebitCommerce = false; //Se filtran solo los comercios que no son débito
            #endregion

            #region Sort
            var sortColumnIndex = Convert.ToInt32(request["iSortCol_0"]);

            var sortableDate = Convert.ToBoolean(request["bSortable_0"]);
            var sortableEmail = Convert.ToBoolean(request["bSortable_1"]);

            Func<int, string> orderingFunction = (c => sortColumnIndex == 0 && sortableDate ? "0" :
                                                       sortColumnIndex == 1 && sortableEmail ? "1" : "");

            filter.OrderBy = orderingFunction(sortColumnIndex);
            #endregion

            #region SortDirection

            var sortDirection = request["sSortDir_0"]; // asc or desc
            filter.SortDirection = sortDirection == "asc" ? SortDirection.Asc : SortDirection.Desc;

            #endregion

            filter.DisplayStart = param.iDisplayStart;
            filter.DisplayLength = 10;

            return filter;
        }

        public static CustomerSiteBranchFilterDto AjaxHandlerCustomerSiteBranch(HttpRequestBase request, JQueryDataTableParamModel param)
        {
            var filter = new CustomerSiteBranchFilterDto();

            #region CustomFilters
            filter.Name = request["Name"];
            filter.Service = !string.IsNullOrWhiteSpace(request["Service"]) ? Guid.Parse(request["Service"]) : default(Guid);
            filter.CommerceId = !string.IsNullOrWhiteSpace(request["CommerceId"]) ? Guid.Parse(request["CommerceId"]) : default(Guid);
            filter.IsDebitCommerce = false; //Se filtran solo los comercios que no son débito
            #endregion

            #region Sort
            var sortColumnIndex = Convert.ToInt32(request["iSortCol_0"]);

            var sortableDate = Convert.ToBoolean(request["bSortable_0"]);
            var sortableEmail = Convert.ToBoolean(request["bSortable_1"]);

            Func<int, string> orderingFunction = (c => sortColumnIndex == 0 && sortableDate ? "0" :
                                                       sortColumnIndex == 1 && sortableEmail ? "1" : "");

            filter.OrderBy = orderingFunction(sortColumnIndex);
            #endregion

            #region SortDirection

            var sortDirection = request["sSortDir_0"]; // asc or desc
            filter.SortDirection = sortDirection == "asc" ? SortDirection.Asc : SortDirection.Desc;

            #endregion

            filter.DisplayStart = param.iDisplayStart;
            filter.DisplayLength = 10;

            return filter;
        }

        public static CustomerSitesSystemUserFilterDto AjaxHandlerCustomerSiteSystemUser(HttpRequestBase request, JQueryDataTableParamModel param)
        {
            var filter = new CustomerSitesSystemUserFilterDto();

            #region CustomFilters
            filter.Email = request["Email"];
            filter.CommerceId = !string.IsNullOrWhiteSpace(request["CommerceId"]) ? Guid.Parse(request["CommerceId"]) : default(Guid);
            filter.IsDebitCommerce = false; //Se filtran solo los comercios que no son débito
            #endregion

            #region Sort
            var sortColumnIndex = Convert.ToInt32(request["iSortCol_0"]);

            var sortableDate = Convert.ToBoolean(request["bSortable_0"]);
            var sortableEmail = Convert.ToBoolean(request["bSortable_1"]);

            Func<int, string> orderingFunction = (c => sortColumnIndex == 0 && sortableDate ? "0" :
                                                       sortColumnIndex == 1 && sortableEmail ? "1" : "");

            filter.OrderBy = orderingFunction(sortColumnIndex);
            #endregion

            #region SortDirection

            var sortDirection = request["sSortDir_0"]; // asc or desc
            filter.SortDirection = sortDirection == "asc" ? SortDirection.Asc : SortDirection.Desc;

            #endregion

            filter.DisplayStart = param.iDisplayStart;
            filter.DisplayLength = 10;

            return filter;
        }

        public static CustomerSiteCommerceFilterDto AjaxHandlerCommercesDebit(HttpRequestBase request, JQueryDataTableParamModel param)
        {
            var filter = new CustomerSiteCommerceFilterDto();

            #region CustomFilters
            filter.Name = request["Name"];
            #endregion

            filter.OrderBy = "0";

            #region SortDirection
            var sortDirection = request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filter.SortDirection = SortDirection.Asc;
            else
                filter.SortDirection = SortDirection.Desc;
            #endregion

            filter.DisplayStart = param.iDisplayStart;

            return filter;
        }

        public static DebitRequestsFilterDto AjaxHandlerDebitSuscriptionList(HttpRequestBase request, JQueryDataTableParamModel param)
        {
            var filter = new DebitRequestsFilterDto();

            #region CustomFilters
            filter.Service = request["Service"];
            filter.Email = request["Email"];

            DateTime tmpDateTime;
            filter.DateFrom = DateTime.TryParse(request["DateFrom"], out tmpDateTime) ? tmpDateTime : DateTime.MinValue;
            filter.DateTo = DateTime.TryParse(request["DateTo"], out tmpDateTime) ? tmpDateTime : DateTime.MinValue;

            filter.DebitState = !string.IsNullOrWhiteSpace(request["DebitState"]) ? (DebitRequestStateDto)Convert.ToInt32(request["DebitState"]) : 0;
            filter.DebitType = !string.IsNullOrWhiteSpace(request["DebitType"]) ? (DebitRequestTypeDto)Convert.ToInt32(request["DebitType"]) : 0;
            #endregion

            filter.OrderBy = "0";

            #region SortDirection
            var sortDirection = request["sSortDir_0"]; // asc or desc
            if (sortDirection == "asc")
                filter.SortDirection = SortDirection.Asc;
            else
                filter.SortDirection = SortDirection.Desc;
            #endregion

            filter.DisplayStart = param.iDisplayStart;

            return filter;
        }

        public static AffiliationCardFilterDto AjaxHandlerAffiliationCard(HttpRequestBase request, JQueryDataTableParamModel param)
        {
            var filter = new AffiliationCardFilterDto();

            #region CustomFilters
            filter.Name = request["Name"];
            filter.Code = string.IsNullOrEmpty(request["Code"]) ? 0 : int.Parse(request["Code"]);
            filter.BankId = string.IsNullOrEmpty(request["BankId"]) ? Guid.Empty : Guid.Parse(request["BankId"]);
            if (!string.IsNullOrEmpty(request["Active"]))
            {
                var value = int.Parse(request["Active"]);
                if (value > 0)
                {
                    filter.Active = value == 1 ? true : false;
                }
            }

            #endregion

            #region Sort
            var sortColumnIndex = Convert.ToInt32(request["iSortCol_0"]);
            var sortableName = Convert.ToBoolean(request["bSortable_0"]);
            var sortableCode = Convert.ToBoolean(request["bSortable_1"]);
            var sortableBank = Convert.ToBoolean(request["bSortable_2"]);
            var sortableActive = Convert.ToBoolean(request["bSortable_3"]);

            Func<int, string> orderingFunction = (c =>
                sortColumnIndex == 0 && sortableName ? "0" :
                sortColumnIndex == 1 && sortableCode ? "1" :
                sortColumnIndex == 2 && sortableBank ? "2" :
                sortColumnIndex == 3 && sortableActive ? "3" : "");

            filter.OrderBy = orderingFunction(sortColumnIndex);
            #endregion

            #region SortDirection
            var sortDirection = request["sSortDir_0"]; // asc or desc
            filter.SortDirection = sortDirection == "asc" ? SortDirection.Asc : SortDirection.Desc;
            #endregion

            filter.DisplayStart = param.iDisplayStart;
            filter.DisplayLength = 10;

            return filter;
        }

        public static ReportsConciliationRunFilterDto AjaxHandlerReportsConciliationRun(HttpRequestBase request, JQueryDataTableParamModel param)
        {
            var filter = new ReportsConciliationRunFilterDto();

            #region CustomFilters
            DateTime tmpDateTime;
            filter.CreationDateFrom = DateTime.TryParse(request["CreationDateFrom"], out tmpDateTime) ? tmpDateTime : DateTime.Today.AddMonths(-1);
            filter.CreationDateTo = DateTime.TryParse(request["CreationDateTo"], out tmpDateTime) ? tmpDateTime : DateTime.Today;
            filter.App = !string.IsNullOrEmpty(request["App"]) ? (ConciliationAppDto?)(int.Parse(request["App"])) : null;
            filter.IsManualRun = !string.IsNullOrEmpty(request["IsManualRun"]) ? bool.Parse(request["IsManualRun"]) : (bool?)null;
            filter.State = !string.IsNullOrEmpty(request["State"]) ? (ConciliationRunStateDto?)(int.Parse(request["State"])) : null;
            filter.InputFileName = request["InputFileName"];
            filter.ConciliationDateFrom = !string.IsNullOrEmpty(request["ConciliationDateFrom"]) ? DateTime.TryParse(request["ConciliationDateFrom"], out tmpDateTime) ? tmpDateTime : (DateTime?)null : (DateTime?)null;
            filter.ConciliationDateTo = !string.IsNullOrEmpty(request["ConciliationDateTo"]) ? DateTime.TryParse(request["ConciliationDateTo"], out tmpDateTime) ? tmpDateTime : (DateTime?)null : (DateTime?)null;

            #endregion

            var sortColumnIndex = Convert.ToInt32(request["iSortCol_0"]);

            Func<int, string> orderingFunction = (c =>
                sortColumnIndex == 0 ? "CreationDate" :
                sortColumnIndex == 2 ? "App" :
                sortColumnIndex == 3 ? "IsManualRun" :
                sortColumnIndex == 4 ? "State" :
                sortColumnIndex == 5 ? "InputFileName" :
                sortColumnIndex == 6 ? "ConciliationDateFrom" :
                sortColumnIndex == 7 ? "ConciliationDateTo"
                : "CreationDate");

            filter.OrderBy = orderingFunction(sortColumnIndex);

            #region SortDirection

            var sortDirection = request["sSortDir_0"]; // asc or desc
            filter.SortDirection = sortDirection == "asc" ? SortDirection.Asc : SortDirection.Desc;

            #endregion

            filter.DisplayStart = param.iDisplayStart;
            filter.DisplayLength = 10;

            return filter;
        }

    }
}