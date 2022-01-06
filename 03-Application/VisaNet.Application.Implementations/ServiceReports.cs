using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using Microsoft.Reporting.WebForms;
using Newtonsoft.Json;
using VisaNet.Application.Implementations.Base;
using VisaNet.Application.Interfaces;
using VisaNet.Common.Exceptions;
using VisaNet.Common.FrameworkExtensions;
using VisaNet.Common.Logging.Entities;
using VisaNet.Common.Logging.Resources;
using VisaNet.Common.Logging.Services;
using VisaNet.Common.Resource.EntitiesDto;
using VisaNet.Common.Resource.Enums;
using VisaNet.Common.Resource.Helpers;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Domain.Entities;
using VisaNet.Domain.Entities.ComplexTypes;
using VisaNet.Domain.Entities.Enums;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Repository.Interfaces.Interfaces;
using VisaNet.Utilities.Cybersource;
using VisaNet.Utilities.Cybersource.Enums;
using VisaNet.Utilities.ExtensionMethods;
using VisaNet.Utilities.Helpers;
using TransactionType = VisaNet.Utilities.Cybersource.Enums.TransactionType;

namespace VisaNet.Application.Implementations
{
    public class ServiceReports : IServiceReports
    {
        private readonly IServicePayment _servicePayment;
        private readonly IServiceService _serviceService;
        private readonly IServiceQuotation _serviceQuotation;
        private readonly IServiceServiceCategory _serviceServiceCategory;
        private readonly IRepositoryGateway _repositoryGateway;
        private readonly ILoggerService _loggerService;
        private readonly IServiceCybersourceErrorGroup _serviceCybersourceErrorGroup;
        private readonly IRepositoryReports _repositoryReports;
        private readonly IRepositoryPayment _repositoryPayments;

        public ServiceReports(IServicePayment servicePayment, IServiceService serviceService, IServiceQuotation serviceQuotation,
            IServiceServiceCategory serviceServiceCategory, IRepositoryGateway repositoryGateway, ILoggerService loggerService,
            IServiceCybersourceErrorGroup serviceCybersourceErrorGroup, IRepositoryReports repositoryReports, IRepositoryPayment repositoryPayments)
        {
            _servicePayment = servicePayment;
            _serviceService = serviceService;
            _serviceQuotation = serviceQuotation;
            _serviceServiceCategory = serviceServiceCategory;
            _repositoryGateway = repositoryGateway;
            _loggerService = loggerService;
            _serviceCybersourceErrorGroup = serviceCybersourceErrorGroup;
            _repositoryReports = repositoryReports;
            _repositoryPayments = repositoryPayments;
        }

        public IEnumerable<TransactionsAmountDto> GetTransactionsAmountData(ReportsTransactionsAmountFilterDto filtersDto)
        {
            var quotationDolar = _serviceQuotation.GetQuotationForDate(DateTime.Now, CurrencyDto.USD);

            var model = new List<TransactionsAmountDto>();

            var query = _repositoryPayments.AllNoTracking(null, p => p.Bills, p => p.Service);

            if (filtersDto.From != default(DateTime))
            {
                filtersDto.From = filtersDto.From.Date;
                query = query.Where(p => p.Date > filtersDto.From);
            }
            if (filtersDto.To != default(DateTime))
            {
                filtersDto.To = filtersDto.To.Date.AddDays(1);
                query = query.Where(p => p.Date < filtersDto.To);
            }

            if (filtersDto.TransactionStatus == (int)TransactionStatusDto.Completed)
                query = query.Where(p => p.PaymentStatus == PaymentStatus.Done || p.PaymentStatus == PaymentStatus.Processed);
            else
                query = query.Where(p => p.PaymentStatus != PaymentStatus.Done && p.PaymentStatus != PaymentStatus.Processed);


            var payments = query.Select(t => new PaymentDto
            {
                PaymentType = (PaymentTypeDto)(int)t.PaymentType,
                ServiceId = t.ServiceId,
                ServiceDto = new ServiceDto
                {
                    ServiceCategoryId = t.Service.ServiceCategoryId
                },
                Date = t.Date,
                GatewayId = t.GatewayId,
                Bills = t.Bills.Select(b => new BillDto
                {
                    Currency = b.Currency,
                    Amount = b.Amount,
                }).ToList()
            }).ToList();

            //Agrupo por fecha
            #region Por fecha
            if (filtersDto.Parameter == (int)ParameterDto.Date)
            {
                IOrderedEnumerable<IGrouping<int, PaymentDto>> paymentGroups = null;

                if (filtersDto.DateParameter == (int)DateParameterDto.Year)
                {
                    paymentGroups = payments.GroupBy(p => p.Date.Year).OrderBy(p => p.Key);
                }
                if (filtersDto.DateParameter == (int)DateParameterDto.Month)
                {
                    paymentGroups = payments.GroupBy(p => p.Date.Month).OrderBy(p => p.Key);
                }
                if (filtersDto.DateParameter == (int)DateParameterDto.Day)
                {
                    paymentGroups = payments.GroupBy(p => p.Date.Day).OrderBy(p => p.Key);
                }
                if (filtersDto.DateParameter == (int)DateParameterDto.Hour)
                {
                    paymentGroups = payments.GroupBy(p => p.Date.Hour).OrderBy(p => p.Key);
                }
                if (filtersDto.DateParameter == (int)DateParameterDto.YearAndMonth)
                {
                    paymentGroups = payments.GroupBy(p => (p.Date.Year * 100) + p.Date.Month).OrderBy(p => p.Key);
                }
                if (filtersDto.DateParameter == (int)DateParameterDto.YearMonthAndDay)
                {
                    paymentGroups = payments.GroupBy(p => (p.Date.Year * 10000) + (p.Date.Month * 100) + p.Date.Day).OrderBy(p => p.Key);
                }
                if (filtersDto.DateParameter == (int)DateParameterDto.DayOfWeek)
                {
                    paymentGroups = payments.GroupBy(p => (int)p.Date.DayOfWeek).OrderBy(p => p.Key);
                }

                var culture = new System.Globalization.CultureInfo("es-ES");

                foreach (var paymentGroup in paymentGroups)
                {
                    if (filtersDto.ExcludeZeros)
                    {
                        var valueTotal = CalculateTotalValueInCurrency(paymentGroup, filtersDto.Dimension,
                            filtersDto.Currency, quotationDolar);
                        if (valueTotal > 0)
                        {
                            model.Add(new TransactionsAmountDto
                            {
                                Name =
                                    filtersDto.DateParameter == (int)DateParameterDto.YearAndMonth ? paymentGroup.Key.ToString().Substring(0, 4) + "-" + paymentGroup.Key.ToString().Substring(4, 2) :

                                    filtersDto.DateParameter == (int)DateParameterDto.YearMonthAndDay ? paymentGroup.Key.ToString().Substring(0, 4) + "-" + paymentGroup.Key.ToString().Substring(4, 2) + "-" + paymentGroup.Key.ToString().Substring(6, 2) :

                                    filtersDto.DateParameter == (int)DateParameterDto.DayOfWeek ? culture.DateTimeFormat.GetDayName((DayOfWeek)paymentGroup.Key) :

                                    paymentGroup.Key.ToString(),

                                ValuePesos = CalculateValueInCurrency(paymentGroup, filtersDto.Dimension, (int)CurrencyDto.UYU),
                                ValueDollars = CalculateValueInCurrency(paymentGroup, filtersDto.Dimension, (int)CurrencyDto.USD),
                                ValueTotal = valueTotal,
                            });
                        }
                    }
                    else
                    {
                        model.Add(new TransactionsAmountDto
                        {
                            Name =
                                filtersDto.DateParameter == (int)DateParameterDto.YearAndMonth ? paymentGroup.Key.ToString().Substring(0, 4) + "-" + paymentGroup.Key.ToString().Substring(4, 2) :

                                filtersDto.DateParameter == (int)DateParameterDto.YearMonthAndDay ? paymentGroup.Key.ToString().Substring(0, 4) + "-" + paymentGroup.Key.ToString().Substring(4, 2) + "-" + paymentGroup.Key.ToString().Substring(6, 2) :

                                filtersDto.DateParameter == (int)DateParameterDto.DayOfWeek ? culture.DateTimeFormat.GetDayName((DayOfWeek)paymentGroup.Key) :

                                paymentGroup.Key.ToString(),

                            ValuePesos = CalculateValueInCurrency(paymentGroup, filtersDto.Dimension, (int)CurrencyDto.UYU),
                            ValueDollars = CalculateValueInCurrency(paymentGroup, filtersDto.Dimension, (int)CurrencyDto.USD),
                            ValueTotal = CalculateTotalValueInCurrency(paymentGroup, filtersDto.Dimension, filtersDto.Currency, quotationDolar),
                        });
                    }
                }
            }
            #endregion

            //Agrupo por servicios
            #region Por servicios
            if (filtersDto.Parameter == (int)ParameterDto.Service)
            {
                var services = _serviceService.AllNoTracking();

                foreach (var service in services.OrderBy(s => s.Name))
                {
                    var servicePayments = payments.Where(p => p.ServiceId == service.Id).ToList();
                    if (filtersDto.ExcludeZeros)
                    {
                        var valueTotal = CalculateTotalValueInCurrency(servicePayments, filtersDto.Dimension,
                            filtersDto.Currency, quotationDolar);
                        if (valueTotal > 0)
                        {
                            model.Add(new TransactionsAmountDto
                            {
                                Name = service.Name,
                                ValuePesos = CalculateValueInCurrency(servicePayments, filtersDto.Dimension, (int)CurrencyDto.UYU),
                                ValueDollars = CalculateValueInCurrency(servicePayments, filtersDto.Dimension, (int)CurrencyDto.USD),
                                ValueTotal = valueTotal,
                            });
                        }
                    }
                    else
                    {
                        model.Add(new TransactionsAmountDto
                        {
                            Name = service.Name,
                            ValuePesos = CalculateValueInCurrency(servicePayments, filtersDto.Dimension, (int)CurrencyDto.UYU),
                            ValueDollars = CalculateValueInCurrency(servicePayments, filtersDto.Dimension, (int)CurrencyDto.USD),
                            ValueTotal = CalculateTotalValueInCurrency(servicePayments, filtersDto.Dimension, filtersDto.Currency, quotationDolar),
                        });
                    }
                }
            }
            #endregion

            //Agrupo por categorías de servicios
            #region Por categorias de servicios
            if (filtersDto.Parameter == (int)ParameterDto.ServiceCategory)
            {
                var serviceCategories = _serviceServiceCategory.AllNoTracking();

                foreach (var serviceCategory in serviceCategories.OrderBy(sc => sc.Name))
                {
                    var serviceCategoryPayments =
                        payments.Where(p => p.ServiceDto.ServiceCategoryId == serviceCategory.Id).ToList();

                    if (filtersDto.ExcludeZeros)
                    {
                        var valueTotal = CalculateTotalValueInCurrency(serviceCategoryPayments, filtersDto.Dimension,
                            filtersDto.Currency, quotationDolar);
                        if (valueTotal > 0)
                        {
                            model.Add(new TransactionsAmountDto
                            {
                                Name = serviceCategory.Name,
                                ValuePesos = CalculateValueInCurrency(serviceCategoryPayments, filtersDto.Dimension, (int)CurrencyDto.UYU),
                                ValueDollars = CalculateValueInCurrency(serviceCategoryPayments, filtersDto.Dimension, (int)CurrencyDto.USD),
                                ValueTotal = valueTotal,
                            });
                        }
                    }
                    else
                    {
                        model.Add(new TransactionsAmountDto
                        {
                            Name = serviceCategory.Name,
                            ValuePesos = CalculateValueInCurrency(serviceCategoryPayments, filtersDto.Dimension, (int)CurrencyDto.UYU),
                            ValueDollars = CalculateValueInCurrency(serviceCategoryPayments, filtersDto.Dimension, (int)CurrencyDto.USD),
                            ValueTotal = CalculateTotalValueInCurrency(serviceCategoryPayments, filtersDto.Dimension, filtersDto.Currency, quotationDolar),
                        });
                    }
                }
            }
            #endregion

            //Agrupo por pasarela
            #region Por pasarela
            if (filtersDto.Parameter == (int)ParameterDto.Gateway)
            {
                foreach (var gateway in _repositoryGateway.AllNoTracking().OrderBy(g => g.Name))
                {
                    var gatewayPayments = payments.Where(p => p.GatewayId == gateway.Id).ToList();
                    if (filtersDto.ExcludeZeros)
                    {
                        var valueTotal = CalculateTotalValueInCurrency(gatewayPayments, filtersDto.Dimension,
                            filtersDto.Currency, quotationDolar);
                        if (valueTotal > 0)
                        {
                            model.Add(new TransactionsAmountDto
                            {
                                Name = gateway.Name,
                                ValuePesos = CalculateValueInCurrency(gatewayPayments, filtersDto.Dimension, (int)CurrencyDto.UYU),
                                ValueDollars = CalculateValueInCurrency(gatewayPayments, filtersDto.Dimension, (int)CurrencyDto.USD),
                                ValueTotal = valueTotal,
                            });
                        }
                    }
                    else
                    {
                        model.Add(new TransactionsAmountDto
                        {
                            Name = gateway.Name,
                            ValuePesos = CalculateValueInCurrency(gatewayPayments, filtersDto.Dimension, (int)CurrencyDto.UYU),
                            ValueDollars = CalculateValueInCurrency(gatewayPayments, filtersDto.Dimension, (int)CurrencyDto.USD),
                            ValueTotal = CalculateTotalValueInCurrency(gatewayPayments, filtersDto.Dimension, filtersDto.Currency, quotationDolar),
                        });
                    }
                }
            }
            #endregion

            //Agrupo por tipo de transacciones
            #region Por tipo de transacciones
            if (filtersDto.Parameter == (int)ParameterDto.PaymentType)
            {
                foreach (var paymentType in
                        EnumHelpers.ConvertToSelectList(typeof(PaymentTypeDto), EnumsStrings.ResourceManager).OrderBy(p => p.Text))
                {
                    var paymentTypePayments = payments.Where(p => (int)p.PaymentType == Int32.Parse(paymentType.Value)).ToList();
                    if (filtersDto.ExcludeZeros)
                    {
                        var valueTotal = CalculateTotalValueInCurrency(paymentTypePayments, filtersDto.Dimension,
                            filtersDto.Currency, quotationDolar);
                        if (valueTotal > 0)
                        {
                            model.Add(new TransactionsAmountDto
                            {
                                Name = EnumHelpers.GetName(typeof(PaymentTypeDto), Int32.Parse(paymentType.Value),
                                EnumsStrings.ResourceManager),
                                ValuePesos = CalculateValueInCurrency(paymentTypePayments, filtersDto.Dimension, (int)CurrencyDto.UYU),
                                ValueDollars = CalculateValueInCurrency(paymentTypePayments, filtersDto.Dimension, (int)CurrencyDto.USD),
                                ValueTotal = valueTotal,
                            });
                        }
                    }
                    else
                    {
                        model.Add(new TransactionsAmountDto
                        {
                            Name = EnumHelpers.GetName(typeof(PaymentTypeDto), Int32.Parse(paymentType.Value),
                                    EnumsStrings.ResourceManager),
                            ValuePesos = CalculateValueInCurrency(paymentTypePayments, filtersDto.Dimension, (int)CurrencyDto.UYU),
                            ValueDollars = CalculateValueInCurrency(paymentTypePayments, filtersDto.Dimension, (int)CurrencyDto.USD),
                            ValueTotal = CalculateTotalValueInCurrency(paymentTypePayments, filtersDto.Dimension, filtersDto.Currency, quotationDolar),
                        });
                    }
                }
            }
            #endregion

            return model;
        }

        private double CalculateValueInCurrency(IEnumerable<PaymentDto> payments, int dimension, int currency)
        {
            var ret = 0.0;
            if (dimension == (int)DimensionDto.Count)
            {
                if (currency == (int)CurrencyDto.UYU)
                {
                    ret = payments.Any() ? payments.Sum(p => p.Bills.Sum(b => b.Currency == Currency.PESO_URUGUAYO
                            ? 1
                            : 0)) : 0;
                }
                if (currency == (int)CurrencyDto.USD)
                {
                    ret = payments.Any() ? payments.Sum(p => p.Bills.Sum(b => b.Currency == Currency.DOLAR_AMERICANO
                            ? 1
                            : 0)) : 0;
                }
            }
            if (dimension == (int)DimensionDto.Amount)
            {
                if (currency == (int)CurrencyDto.UYU)
                {
                    ret = payments.Any() ? payments.Sum(p => p.Bills.Sum(b => b.Currency == Currency.PESO_URUGUAYO
                            ? b.Amount
                            : 0)) : 0;
                }
                if (currency == (int)CurrencyDto.USD)
                {
                    ret = payments.Any() ? payments.Sum(p => p.Bills.Sum(b => b.Currency == Currency.DOLAR_AMERICANO
                            ? b.Amount
                            : 0)) : 0;
                }
            }
            if (dimension == (int)DimensionDto.AverageAmount)
            {
                if (currency == (int)CurrencyDto.UYU)
                {
                    ret = payments.Any() ? payments.Average(p => p.Bills.Sum(b => b.Currency == Currency.PESO_URUGUAYO
                            ? b.Amount
                            : 0)) : 0;
                }
                if (currency == (int)CurrencyDto.USD)
                {
                    ret = payments.Any() ? payments.Average(p => p.Bills.Sum(b => b.Currency == Currency.DOLAR_AMERICANO
                            ? b.Amount
                            : 0)) : 0;
                }
            }
            return Math.Round(ret);
        }

        private double CalculateTotalValueInCurrency(IEnumerable<PaymentDto> payments, int dimension, int currency, QuotationDto quotationDolar)
        {
            var ret = 0.0;
            if (dimension == (int)DimensionDto.Count)
            {
                ret = payments.Count();
            }
            if (dimension == (int)DimensionDto.Amount)
            {
                if (currency == (int)CurrencyDto.UYU)
                {
                    ret = payments.Any() ? payments.Sum(p => p.Bills.Sum(b => b.Currency == Currency.PESO_URUGUAYO
                            ? b.Amount
                            : b.Amount * quotationDolar.ValueInPesos)) : 0;
                }
                if (currency == (int)CurrencyDto.USD)
                {
                    ret = payments.Any() ? payments.Sum(p => p.Bills.Sum(b => b.Currency == Currency.DOLAR_AMERICANO
                            ? b.Amount
                            : b.Amount / quotationDolar.ValueInPesos)) : 0;
                }
            }
            if (dimension == (int)DimensionDto.AverageAmount)
            {
                if (currency == (int)CurrencyDto.UYU)
                {
                    ret = payments.Any() ? payments.Average(p => p.Bills.Sum(b => b.Currency == Currency.PESO_URUGUAYO
                            ? b.Amount
                            : b.Amount * quotationDolar.ValueInPesos)) : 0;
                }
                if (currency == (int)CurrencyDto.USD)
                {
                    ret = payments.Any() ? payments.Average(p => p.Bills.Sum(b => b.Currency == Currency.DOLAR_AMERICANO
                            ? b.Amount
                            : b.Amount / quotationDolar.ValueInPesos)) : 0;
                }
            }
            return Math.Round(ret);
        }

        public IEnumerable<LogPaymentCyberSourceDto> GetCybersourceTransactionsDetails(ReportsCybersourceTransactionsFilterDto filtersDto)
        {
            var logList = new List<LogPaymentCyberSourceDto>();

            IEnumerable<LogPaymentCyberSourceDto> creationTokenLogs;
            IEnumerable<LogPaymentCyberSourceDto> saleLogs;
            IEnumerable<LogPaymentCyberSourceDto> saleAndCreationTokenLogs;
            IEnumerable<LogPaymentCyberSourceDto> voidOrRefundLogs;
            IEnumerable<CybersourceErrorGroupDto> errorGroups;

            switch (filtersDto.TransactionType)
            {
                case 0:
                    //Todas las transacciones
                    creationTokenLogs = _loggerService.CybersourceTransactionsDetails(filtersDto.From, filtersDto.To, TransactionType.CreatePaymentToken, filtersDto.Bin);
                    saleLogs = _loggerService.CybersourceTransactionsDetails(filtersDto.From, filtersDto.To, TransactionType.Sale, filtersDto.Bin);
                    voidOrRefundLogs = _loggerService.CybersourceTransactionsDetails(filtersDto.From, filtersDto.To, null, filtersDto.Bin);
                    saleAndCreationTokenLogs = _loggerService.CybersourceTransactionsDetails(filtersDto.From, filtersDto.To, TransactionType.SaleAndCreatePaymentToken, filtersDto.Bin);
                    logList.AddRange(creationTokenLogs);
                    logList.AddRange(saleLogs);
                    logList.AddRange(saleAndCreationTokenLogs);
                    logList.AddRange(voidOrRefundLogs.Where(l => l.CyberSourceLogData.TransactionType == Domain.Entities.Enums.TransactionType.Void || l.CyberSourceLogData.TransactionType == Domain.Entities.Enums.TransactionType.Refund));
                    break;
                case 1:
                    //Creación de token completadas
                    creationTokenLogs = _loggerService.CybersourceTransactionsDetails(filtersDto.From, filtersDto.To, TransactionType.CreatePaymentToken, filtersDto.Bin);
                    logList.AddRange(creationTokenLogs.Where(l => Convert.ToInt32(l.CyberSourceLogData.ReasonCode) == 100));
                    break;
                case 2:
                    //Creación de token con errores
                    creationTokenLogs = _loggerService.CybersourceTransactionsDetails(filtersDto.From, filtersDto.To, TransactionType.CreatePaymentToken, filtersDto.Bin);
                    logList.AddRange(creationTokenLogs.Where(l => String.IsNullOrEmpty(l.CyberSourceLogData.ReasonCode)));
                    break;
                case 3:
                    //Creación de token totales
                    creationTokenLogs = _loggerService.CybersourceTransactionsDetails(filtersDto.From, filtersDto.To, TransactionType.CreatePaymentToken, filtersDto.Bin);
                    logList.AddRange(creationTokenLogs);
                    break;
                case 4:
                    //Pagos con creación de token completadas
                    saleAndCreationTokenLogs = _loggerService.CybersourceTransactionsDetails(filtersDto.From, filtersDto.To, TransactionType.SaleAndCreatePaymentToken, filtersDto.Bin);
                    logList.AddRange(saleAndCreationTokenLogs.Where(l => Convert.ToInt32(l.CyberSourceLogData.ReasonCode) == 100));
                    break;
                case 5:
                    //Pagos con token existente completadas
                    saleLogs = _loggerService.CybersourceTransactionsDetails(filtersDto.From, filtersDto.To, TransactionType.Sale, filtersDto.Bin);
                    logList.AddRange(saleLogs.Where(l => Convert.ToInt32(l.CyberSourceLogData.ReasonCode) == 100));
                    break;
                case 6:
                    //Pagos con errores
                    saleLogs = _loggerService.CybersourceTransactionsDetails(filtersDto.From, filtersDto.To, TransactionType.Sale, filtersDto.Bin);
                    saleAndCreationTokenLogs = _loggerService.CybersourceTransactionsDetails(filtersDto.From, filtersDto.To, TransactionType.SaleAndCreatePaymentToken, filtersDto.Bin);
                    errorGroups = _serviceCybersourceErrorGroup.AllNoTracking(null, null, c => c.CybersourceErrors).OrderBy(c => c.Order);
                    logList.AddRange(saleLogs.Where(l => errorGroups.ElementAt(0).CybersourceErrors.Any(st => st.ReasonCode == Convert.ToInt32(l.CyberSourceLogData.ReasonCode))));
                    logList.AddRange(saleAndCreationTokenLogs.Where(l => errorGroups.ElementAt(0).CybersourceErrors.Any(st => st.ReasonCode == Convert.ToInt32(l.CyberSourceLogData.ReasonCode))));
                    break;
                case 7:
                    saleLogs = _loggerService.CybersourceTransactionsDetails(filtersDto.From, filtersDto.To, TransactionType.Sale, filtersDto.Bin);
                    saleAndCreationTokenLogs = _loggerService.CybersourceTransactionsDetails(filtersDto.From, filtersDto.To, TransactionType.SaleAndCreatePaymentToken, filtersDto.Bin);
                    errorGroups = _serviceCybersourceErrorGroup.AllNoTracking(null, null, c => c.CybersourceErrors).OrderBy(c => c.Order);
                    logList.AddRange(saleLogs.Where(l => errorGroups.ElementAt(1).CybersourceErrors.Any(st => st.ReasonCode == Convert.ToInt32(l.CyberSourceLogData.ReasonCode))));
                    logList.AddRange(saleAndCreationTokenLogs.Where(l => errorGroups.ElementAt(1).CybersourceErrors.Any(st => st.ReasonCode == Convert.ToInt32(l.CyberSourceLogData.ReasonCode))));
                    break;
                case 8:
                    saleLogs = _loggerService.CybersourceTransactionsDetails(filtersDto.From, filtersDto.To, TransactionType.Sale, filtersDto.Bin);
                    saleAndCreationTokenLogs = _loggerService.CybersourceTransactionsDetails(filtersDto.From, filtersDto.To, TransactionType.SaleAndCreatePaymentToken, filtersDto.Bin);
                    errorGroups = _serviceCybersourceErrorGroup.AllNoTracking(null, null, c => c.CybersourceErrors).OrderBy(c => c.Order);
                    logList.AddRange(saleLogs.Where(l => errorGroups.ElementAt(2).CybersourceErrors.Any(st => st.ReasonCode == Convert.ToInt32(l.CyberSourceLogData.ReasonCode))));
                    logList.AddRange(saleAndCreationTokenLogs.Where(l => errorGroups.ElementAt(2).CybersourceErrors.Any(st => st.ReasonCode == Convert.ToInt32(l.CyberSourceLogData.ReasonCode))));
                    break;
                case 9:
                    saleLogs = _loggerService.CybersourceTransactionsDetails(filtersDto.From, filtersDto.To, TransactionType.Sale, filtersDto.Bin);
                    saleAndCreationTokenLogs = _loggerService.CybersourceTransactionsDetails(filtersDto.From, filtersDto.To, TransactionType.SaleAndCreatePaymentToken, filtersDto.Bin);
                    errorGroups = _serviceCybersourceErrorGroup.AllNoTracking(null, null, c => c.CybersourceErrors).OrderBy(c => c.Order);
                    logList.AddRange(saleLogs.Where(l => errorGroups.ElementAt(3).CybersourceErrors.Any(st => st.ReasonCode == Convert.ToInt32(l.CyberSourceLogData.ReasonCode))));
                    logList.AddRange(saleAndCreationTokenLogs.Where(l => errorGroups.ElementAt(3).CybersourceErrors.Any(st => st.ReasonCode == Convert.ToInt32(l.CyberSourceLogData.ReasonCode))));
                    break;
                case 10:
                    //Pagos con otros errores
                    saleLogs = _loggerService.CybersourceTransactionsDetails(filtersDto.From, filtersDto.To, TransactionType.Sale, filtersDto.Bin);
                    saleAndCreationTokenLogs = _loggerService.CybersourceTransactionsDetails(filtersDto.From, filtersDto.To, TransactionType.SaleAndCreatePaymentToken, filtersDto.Bin);
                    logList.AddRange(saleLogs.Where(l => String.IsNullOrEmpty(l.CyberSourceLogData.ReasonCode)));
                    logList.AddRange(saleAndCreationTokenLogs.Where(l => String.IsNullOrEmpty(l.CyberSourceLogData.ReasonCode)));
                    break;
                case 11:
                    //Transacciones void
                    voidOrRefundLogs = _loggerService.CybersourceTransactionsDetails(filtersDto.From, filtersDto.To, null, filtersDto.Bin);
                    logList.AddRange(voidOrRefundLogs.Where(l => l.CyberSourceLogData.TransactionType == Domain.Entities.Enums.TransactionType.Void));
                    break;
                case 12:
                    //Transacciones refund
                    voidOrRefundLogs = _loggerService.CybersourceTransactionsDetails(filtersDto.From, filtersDto.To, null, filtersDto.Bin);
                    logList.AddRange(voidOrRefundLogs.Where(l => l.CyberSourceLogData.TransactionType == Domain.Entities.Enums.TransactionType.Refund));
                    break;
                case 13:
                    //Pagos total
                    saleLogs = _loggerService.CybersourceTransactionsDetails(filtersDto.From, filtersDto.To, TransactionType.Sale, filtersDto.Bin);
                    saleAndCreationTokenLogs = _loggerService.CybersourceTransactionsDetails(filtersDto.From, filtersDto.To, TransactionType.SaleAndCreatePaymentToken, filtersDto.Bin);
                    voidOrRefundLogs = _loggerService.CybersourceTransactionsDetails(filtersDto.From, filtersDto.To, null, filtersDto.Bin);
                    logList.AddRange(saleLogs);
                    logList.AddRange(saleAndCreationTokenLogs);
                    logList.AddRange(voidOrRefundLogs.Where(l => l.CyberSourceLogData.TransactionType == Domain.Entities.Enums.TransactionType.Void || l.CyberSourceLogData.TransactionType == Domain.Entities.Enums.TransactionType.Refund));
                    break;
            }

            return logList;
        }

        public IEnumerable<CybersourceTransactionsDto> GetCybersourceTransactionsData(ReportsCybersourceTransactionsFilterDto filtersDto)
        {
            List<CybersourceTransactionsDto> stepList;

            if (filtersDto.Sale)
            {
                var saleLogs = _loggerService.CybersourceTransactions(filtersDto.From, filtersDto.To, TransactionType.Sale, filtersDto.Bin);
                var saleAndCreationTokenLogs = _loggerService.CybersourceTransactions(filtersDto.From, filtersDto.To, TransactionType.SaleAndCreatePaymentToken, filtersDto.Bin);

                var errorGroups = _serviceCybersourceErrorGroup.AllNoTracking(null, null, c => c.CybersourceErrors).OrderBy(c => c.Order);

                stepList = new List<CybersourceTransactionsDto>
                {
                    //Pagos con creación de token completadas
                    new CybersourceTransactionsDto
                    {
                        Name = PresentationAdminStrings.Cybersource_Payment_Transactions_Token_Completed,
                        Value = saleAndCreationTokenLogs.Count(l => Convert.ToInt32(l.ReasonCode) == 100)
                    },
                    //Pagos con token existente completadas
                    new CybersourceTransactionsDto
                    {
                        Name = PresentationAdminStrings.Cybersource_Payment_Transactions_Completed,
                        Value = saleLogs.Count(l => Convert.ToInt32(l.ReasonCode) == 100)
                    }
                };

                //Pagos con errores
                foreach (var errorGroup in errorGroups)
                {
                    stepList.Add(new CybersourceTransactionsDto
                    {
                        Name = PresentationAdminStrings.Cybersource_Transactions + errorGroup.Name,
                        Value = saleLogs.Count(l => errorGroup.CybersourceErrors.Any(st => st.ReasonCode == Convert.ToInt32(l.ReasonCode))) + saleAndCreationTokenLogs.Count(l => errorGroup.CybersourceErrors.Any(st => st.ReasonCode == Convert.ToInt32(l.ReasonCode)))
                    });
                }

                //Pagos con otros errores
                stepList.Add(new CybersourceTransactionsDto
                {
                    Name = PresentationAdminStrings.Cybersource_Payment_Transactions_Other,
                    Value = saleLogs.Count(l => String.IsNullOrEmpty(l.ReasonCode)) + saleAndCreationTokenLogs.Count(l => String.IsNullOrEmpty(l.ReasonCode))
                });

                //Transacciones void / refund
                var voidOrRefundLogs = _loggerService.CybersourceTransactions(filtersDto.From, filtersDto.To, null, filtersDto.Bin);

                stepList.Add(new CybersourceTransactionsDto
                {
                    Name = PresentationAdminStrings.Cybersource_Transactions_Void,
                    Value = voidOrRefundLogs.Count(l => l.TransactionType == Domain.Entities.Enums.TransactionType.Void)
                });

                stepList.Add(new CybersourceTransactionsDto
                {
                    Name = PresentationAdminStrings.Cybersource_Transactions_Refund,
                    Value = voidOrRefundLogs.Count(l => l.TransactionType == Domain.Entities.Enums.TransactionType.Refund)
                });

                //Pagos total
                stepList.Add(new CybersourceTransactionsDto
                {
                    Name = PresentationAdminStrings.Cybersource_Transactions_Total,
                    Value = saleLogs.Count() + saleAndCreationTokenLogs.Count() + voidOrRefundLogs.Count(l => l.TransactionType == Domain.Entities.Enums.TransactionType.Void || l.TransactionType == Domain.Entities.Enums.TransactionType.Refund)
                });
            }
            else
            {
                var creationTokenLogs = _loggerService.CybersourceTransactions(filtersDto.From, filtersDto.To, TransactionType.CreatePaymentToken, filtersDto.Bin);

                stepList = new List<CybersourceTransactionsDto>
                {
                    //Creación de token completadas
                    new CybersourceTransactionsDto
                    {
                        Name = PresentationAdminStrings.Cybersource_Transactions_Completed,
                        Value = creationTokenLogs.Count(l => Convert.ToInt32(l.ReasonCode) == 100)
                    },
                    //Creación de token con errores
                    new CybersourceTransactionsDto
                    {
                        Name = PresentationAdminStrings.Cybersource_Transactions_Error,
                        Value = creationTokenLogs.Count(l => String.IsNullOrEmpty(l.ReasonCode))
                    },
                    //Creación de token totales
                    new CybersourceTransactionsDto
                    {
                        Name = PresentationAdminStrings.Cybersource_Transactions_Total,
                        Value = creationTokenLogs.Count()
                    }
                };
            }

            return stepList;
        }

        public DashboardDto GetDashboardSP(ReportsDashboardFilterDto filtersDto)
        {
            var list = _repositoryReports.GetDashboardSP(filtersDto.From, filtersDto.To.AddDays(1), filtersDto.Currency == 1 ? "USD" : "UYU");

            var first = list.FirstOrDefault();
            return new DashboardDto
            {
                complaintContactQuantity = first == null ? 0 : first.ComplaintContactQuantity,
                contactQuantity = first == null ? 0 : first.ContactQuantity,
                notificationQuantity = first == null ? 0 : first.NotificationQuantity,
                otherContactQuantity = first == null ? 0 : first.OtherContactQuantity,
                questionContactQuantity = first == null ? 0 : first.QuestionContactQuantity,
                subscriberQuantity = first == null ? 0 : first.SubscriberQuantity,
                suggestionContactQuantity = first == null ? 0 : first.SuggestionContactQuantity,
                userQuantity = first == null ? 0 : first.UserQuantity,
                manualAmount = list.FirstOrDefault(x => x.PaymentType == (int)PaymentTypeDto.Manual) == null ? 0 : list.FirstOrDefault(x => x.PaymentType == (int)PaymentTypeDto.Manual).Amount,
                manualQuantity = list.FirstOrDefault(x => x.PaymentType == (int)PaymentTypeDto.Manual) == null ? 0 : list.FirstOrDefault(x => x.PaymentType == (int)PaymentTypeDto.Manual).Count,
                notRegisteredAmount = list.FirstOrDefault(x => x.PaymentType == (int)PaymentTypeDto.AnonymousUser) == null ? 0 : list.FirstOrDefault(x => x.PaymentType == (int)PaymentTypeDto.AnonymousUser).Amount,
                notRegisteredQuantity = list.FirstOrDefault(x => x.PaymentType == (int)PaymentTypeDto.AnonymousUser) == null ? 0 : list.FirstOrDefault(x => x.PaymentType == (int)PaymentTypeDto.AnonymousUser).Count,
                automaticAmount = list.FirstOrDefault(x => x.PaymentType == (int)PaymentTypeDto.Automatic) == null ? 0 : list.FirstOrDefault(x => x.PaymentType == (int)PaymentTypeDto.Automatic).Amount,
                automaticQuantity = list.FirstOrDefault(x => x.PaymentType == (int)PaymentTypeDto.Automatic) == null ? 0 : list.FirstOrDefault(x => x.PaymentType == (int)PaymentTypeDto.Automatic).Count,
                appsAmount = list.FirstOrDefault(x => x.PaymentType == (int)PaymentTypeDto.App) == null ? 0 : list.FirstOrDefault(x => x.PaymentType == (int)PaymentTypeDto.App).Amount,
                appsQuantity = list.FirstOrDefault(x => x.PaymentType == (int)PaymentTypeDto.App) == null ? 0 : list.FirstOrDefault(x => x.PaymentType == (int)PaymentTypeDto.App).Count,
                bankAmount = list.FirstOrDefault(x => x.PaymentType == (int)PaymentTypeDto.Bank) == null ? 0 : list.FirstOrDefault(x => x.PaymentType == (int)PaymentTypeDto.Bank).Amount,
                bankQuantity = list.FirstOrDefault(x => x.PaymentType == (int)PaymentTypeDto.Bank) == null ? 0 : list.FirstOrDefault(x => x.PaymentType == (int)PaymentTypeDto.Bank).Count,
                totalAmount = list.Sum(x => x.Amount),
                totalQuantity = list.Sum(x => x.Count),
            };
        }

        public List<List<object>> GetDashboardPieChartData(ReportFilterDto filterDto)
        {
            var payments = _servicePayment.GetDataForUserReports(filterDto);

            //Lista de id de servicios
            var idsFilters = string.IsNullOrWhiteSpace(filterDto.Categories) ? _serviceService.AllNoTracking().Select(z => z.Id).ToList() : filterDto.Categories.Split(',').Select(Guid.Parse).ToList();
            var idList = new List<Guid>();

            if (filterDto.GroupedBy == ReportFilterGrouper.Category)
            {
                idList.AddRange(from idServicio in idsFilters select _serviceService.GetById(idServicio) into service where service != null select service.ServiceCategoryId);
            }
            else
            {
                idList = idsFilters;
            }


            //Filtros de Categorias
            if (!string.IsNullOrEmpty(filterDto.Categories))
            {
                payments = filterDto.GroupedBy == ReportFilterGrouper.Category ?
                    payments.Where(x => idList.Any(f => x.ServiceDto.ServiceCategoryId == f)) :
                    payments.Where(x => idList.Any(f => x.ServiceId == f));
            }

            var data = new Dictionary<string, double>();
            //Recorro todos los pagos
            foreach (var payment in payments)
            {
                //Si filtro por categoria
                if (filterDto.GroupedBy == ReportFilterGrouper.Category)
                {
                    //Si no existe la categoría la agrego y sumo
                    if (!data.ContainsKey(payment.ServiceDto.ServiceCategoryName))
                    {
                        data.Add(payment.ServiceDto.ServiceCategoryName, payment.TotalAmount);
                    }
                    else //Si existe solo sumo
                    {
                        data[payment.ServiceDto.ServiceCategoryName] += payment.TotalAmount;
                    }
                }
                else //Si filtro por servicio
                {
                    //Si no existe el servicio lo agrego y sumo
                    if (!data.ContainsKey(payment.ServiceDto.Name))
                    {
                        data.Add(payment.ServiceDto.Name, payment.TotalAmount);
                    }
                    else //Si existe solo sumo
                    {
                        data[payment.ServiceDto.Name] += payment.TotalAmount;
                    }
                }
            }

            var ret = filterDto.GroupedBy == ReportFilterGrouper.Category ?
                new List<List<object>> { new List<object> { "Rubro", "Ammount" } } :
                new List<List<object>> { new List<object> { "Servicio", "Ammount" } };

            ret.AddRange(data.OrderByDescending(x => x.Value).Select(d => new List<object> { d.Key, Math.Round(d.Value, 2) }));

            return ret;
        }

        public List<List<object>> GetDashboardLineChartData(ReportFilterDto filterDto)
        {
            var payments = _servicePayment.GetDataForUserReports(filterDto);

            //Lista de id de servicios
            var idsFilters = string.IsNullOrWhiteSpace(filterDto.Categories) ? _serviceService.AllNoTracking().Select(z => z.Id).ToList() : filterDto.Categories.Split(',').Select(Guid.Parse).ToList();
            var idList = new List<Guid>();

            if (filterDto.GroupedBy == ReportFilterGrouper.Category)
            {
                idList.AddRange(from idServicio in idsFilters select _serviceService.GetById(idServicio) into service where service != null select service.ServiceCategoryId);
            }
            else
            {
                idList = idsFilters;
            }


            //Filtros de Categorias
            if (!string.IsNullOrEmpty(filterDto.Categories))
            {
                payments = filterDto.GroupedBy == ReportFilterGrouper.Category ?
                    payments.Where(x => idList.Any(f => x.ServiceDto.ServiceCategoryId == f)) :
                    payments.Where(x => idList.Any(f => x.ServiceId == f));
            }

            ////Filtros de Categorias
            //if (!string.IsNullOrEmpty(filterDto.Categories))
            //{
            //    var categoriesFilters = filterDto.Categories.Split(',').Select(Guid.Parse);
            //    payments = payments.Where(x => categoriesFilters.Any(f => x.ServiceDto.ServiceCategoryId == f));
            //}

            //MES                //rubro //monto
            var data = new Dictionary<DateTime, Dictionary<string, double>>();

            var indexDate = filterDto.From.Value;

            while (indexDate <= filterDto.To)
            {
                var paymentsOfMonth =
                    payments.Where(p => p.Date.Year == indexDate.Year && p.Date.Month == indexDate.Month);

                //Recorro todos los pagos del mes
                foreach (var payment in paymentsOfMonth)
                {
                    //Si no existe el mes la agrego
                    if (!data.ContainsKey(new DateTime(payment.Date.Year, payment.Date.Month, 1)))
                    {
                        if (filterDto.GroupedBy == ReportFilterGrouper.Category)
                            data.Add(new DateTime(payment.Date.Year, payment.Date.Month, 1), new Dictionary<string, double> { { payment.ServiceDto.ServiceCategoryName, payment.TotalAmount } });
                        else
                            data.Add(new DateTime(payment.Date.Year, payment.Date.Month, 1), new Dictionary<string, double> { { payment.ServiceDto.Name, payment.TotalAmount } });
                    }
                    else //Si existe el mes veo si existe el servicio
                    {
                        if (filterDto.GroupedBy == ReportFilterGrouper.Category)
                        {
                            if (
                                data[new DateTime(payment.Date.Year, payment.Date.Month, 1)].ContainsKey(
                                    payment.ServiceDto.ServiceCategoryName))
                            {
                                //Si existe el servicio sumo
                                data[new DateTime(payment.Date.Year, payment.Date.Month, 1)][
                                    payment.ServiceDto.ServiceCategoryName] += payment.TotalAmount;
                            }
                            else
                            {
                                //Si no existe lo creo
                                data[new DateTime(payment.Date.Year, payment.Date.Month, 1)].Add(
                                    payment.ServiceDto.ServiceCategoryName, payment.TotalAmount);
                            }
                        }
                        else
                        {
                            if (
                               data[new DateTime(payment.Date.Year, payment.Date.Month, 1)].ContainsKey(
                                   payment.ServiceDto.Name))
                            {
                                //Si existe el servicio sumo
                                data[new DateTime(payment.Date.Year, payment.Date.Month, 1)][
                                    payment.ServiceDto.Name] += payment.TotalAmount;
                            }
                            else
                            {
                                //Si no existe lo creo
                                data[new DateTime(payment.Date.Year, payment.Date.Month, 1)].Add(
                                    payment.ServiceDto.Name, payment.TotalAmount);
                            }
                        }

                    }
                }

                indexDate = indexDate.AddMonths(1);
            }

            var servicesCategories = filterDto.GroupedBy == ReportFilterGrouper.Category ? payments.GroupBy(x => x.ServiceDto.ServiceCategoryName) : payments.GroupBy(x => x.ServiceDto.Name);
            var header = new List<object> { "Mes" };
            header.AddRange(servicesCategories.Select(servicesCategory => servicesCategory.Key));

            var ret = new List<List<object>> { header };
            foreach (var month in data.ToList())
            {
                var listOfValues = new List<double>();
                foreach (var serviceName in servicesCategories.Select(servicesCategory => servicesCategory.Key))
                {
                    if (month.Value.ContainsKey(serviceName))
                    {
                        listOfValues.Add(Math.Round(month.Value[serviceName], 2));
                    }
                    else
                    {
                        listOfValues.Add(0);
                    }
                }
                var row = new List<object> { month.Key.ToString("MMM-yy") };
                listOfValues.ForEach(v => row.Add(v));
                ret.Add(row);
            }
            return ret;
        }

        public List<ServiceCategoryDto> ServicesCategories(Guid userId)
        {
            var payments = _servicePayment.AllNoTracking(null, x => x.RegisteredUserId == userId && (x.PaymentStatus != PaymentStatus.Voided || x.PaymentStatus != PaymentStatus.Reversed || x.PaymentStatus != PaymentStatus.Refunded), x => x.Service);
            var servicesCategories = payments.Select(x => new { Id = x.ServiceDto.ServiceCategoryId }).Distinct();

            var categories = new List<ServiceCategoryDto>();
            servicesCategories.ForEach(x => categories.Add(_serviceServiceCategory.GetById(x.Id)));

            return categories;
        }

        public Dictionary<Guid, List<ServiceDto>> ServicesWithPayments(Guid userId)
        {
            var payments = _servicePayment.AllNoTracking(null, x => x.RegisteredUserId == userId && (x.PaymentStatus != PaymentStatus.Voided || x.PaymentStatus != PaymentStatus.Reversed || x.PaymentStatus != PaymentStatus.Refunded), x => x.Service);
            var servicesCategories = payments.Select(x => new { Id = x.ServiceDto.ServiceCategoryId }).Distinct();

            var retAux = new Dictionary<Guid, List<ServiceDto>>();

            foreach (var category in servicesCategories.Where(category => !retAux.ContainsKey(category.Id)))
            {
                var kk =
                    payments.Select(x => x.ServiceDto)
                        .Where(x => x.ServiceCategoryId == category.Id)
                        .Distinct(new ServiceComparer())
                        .ToList();

                retAux.Add(category.Id, kk);
            }

            return retAux;
        }
    }

    // Custom comparer for the Product class
    class ServiceComparer : IEqualityComparer<ServiceDto>
    {
        public bool Equals(ServiceDto x, ServiceDto y)
        {
            return x.Id == y.Id;
        }

        public int GetHashCode(ServiceDto obj)
        {
            //Check whether the object is null
            if (Object.ReferenceEquals(obj, null)) return 0;

            //Get hash code for the Name field if it is not null.
            int hashProductName = obj.Name == null ? 0 : obj.Name.GetHashCode();

            //Get hash code for the Code field.
            int hashProductCode = obj.Id.GetHashCode();

            //Calculate the hash code for the product.
            return hashProductName ^ hashProductCode;
        }
    }

}