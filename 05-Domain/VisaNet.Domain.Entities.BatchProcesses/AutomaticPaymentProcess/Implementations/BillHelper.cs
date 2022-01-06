using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using VisaNet.Application.Interfaces;
using VisaNet.Common.Exceptions;
using VisaNet.Domain.Entities.BatchProcesses.AutomaticPaymentProcess.Interfaces;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Domain.EntitiesDtos.TableFilters;

namespace VisaNet.Domain.Entities.BatchProcesses.AutomaticPaymentProcess.Implementations
{
    public class BillHelper : IBillHelper
    {
        private readonly IServiceBill _serviceBill;
        private readonly IServiceService _serviceService;
        private readonly ISystemNotificationFactory _systemNotificationFactory;
        private readonly IUserNotificationFactory _userNotificationFactory;
        private readonly ILoggerHelper _loggerHelper;

        public BillHelper(IServiceBill serviceBill, IServiceService serviceService,
            ISystemNotificationFactory systemNotificationFactory, ILoggerHelper loggerHelper,
            IUserNotificationFactory userNotificationFactory)
        {
            _serviceBill = serviceBill;
            _serviceService = serviceService;
            _systemNotificationFactory = systemNotificationFactory;
            _loggerHelper = loggerHelper;
            _userNotificationFactory = userNotificationFactory;
        }

        public ICollection<BillDto> ObtainBillsForService(ServiceAssociatedDto serviceAssociatedDto)
        {
            ICollection<BillDto> bills;
            try
            {
                var references = new Dictionary<string, string>();

                if (!string.IsNullOrEmpty(serviceAssociatedDto.ServiceDto.ReferenceParamName))
                    references.Add(serviceAssociatedDto.ServiceDto.ReferenceParamName, serviceAssociatedDto.ReferenceNumber);
                if (!string.IsNullOrEmpty(serviceAssociatedDto.ServiceDto.ReferenceParamName2))
                    references.Add(serviceAssociatedDto.ServiceDto.ReferenceParamName2, serviceAssociatedDto.ReferenceNumber2);
                if (!string.IsNullOrEmpty(serviceAssociatedDto.ServiceDto.ReferenceParamName3))
                    references.Add(serviceAssociatedDto.ServiceDto.ReferenceParamName3, serviceAssociatedDto.ReferenceNumber3);
                if (!string.IsNullOrEmpty(serviceAssociatedDto.ServiceDto.ReferenceParamName4))
                    references.Add(serviceAssociatedDto.ServiceDto.ReferenceParamName4, serviceAssociatedDto.ReferenceNumber4);
                if (!string.IsNullOrEmpty(serviceAssociatedDto.ServiceDto.ReferenceParamName5))
                    references.Add(serviceAssociatedDto.ServiceDto.ReferenceParamName5, serviceAssociatedDto.ReferenceNumber5);
                if (!string.IsNullOrEmpty(serviceAssociatedDto.ServiceDto.ReferenceParamName6))
                    references.Add(serviceAssociatedDto.ServiceDto.ReferenceParamName6, serviceAssociatedDto.ReferenceNumber6);
                var userBills = _serviceBill.GetBillsForRegisteredUser(new RegisteredUserBillFilterDto
                {
                    ServiceId = serviceAssociatedDto.ServiceId,
                    References = references,
                    UserId = serviceAssociatedDto.UserId,
                    CardId = serviceAssociatedDto.DefaultCardId,
                    ScheduledPayment = true
                });

                bills = userBills.Bills;
            }
            catch (BillException e)
            {
                GetBillsHandledException(e, serviceAssociatedDto);
                throw;
            }
            catch (Exception e)
            {
                GetBillsException(e, serviceAssociatedDto);
                throw;
            }

            if (bills != null)
            {
                bills = bills.OrderBy(x => x.ExpirationDate).ToList();
            }
            else
            {
                bills = new Collection<BillDto>(); //para no devolver null
            }
            return bills;
        }

        public ICollection<BillDto> FilterBills(ICollection<BillDto> bills, ServiceAssociatedDto serviceAssociatedDto,
            ref Dictionary<Guid, PaymentResultTypeDto> billsDictionary)
        {
            ICollection<BillDto> filteredBills = null;
            if (bills.FirstOrDefault().Gateway == GatewayEnumDto.Sucive ||
                bills.FirstOrDefault().Gateway == GatewayEnumDto.Geocom)
            {
                filteredBills = BillsEnableToPaySucive(serviceAssociatedDto, bills, ref billsDictionary);
            }
            else
            {
                filteredBills = BillsEnableToPay(bills, serviceAssociatedDto, ref billsDictionary);
            }

            _loggerHelper.LogFilteredBillsForServiceAssociate(filteredBills.Count, serviceAssociatedDto.Id);
            return filteredBills;
        }

        private ICollection<BillDto> BillsEnableToPay(IEnumerable<BillDto> bills, ServiceAssociatedDto serviceAssociatedDto,
            ref Dictionary<Guid, PaymentResultTypeDto> billsDictionary)
        {
            var finalList = new List<BillDto>();
            var billsUpdate = bills.Where(b => b.Payable).OrderBy(b => b.ExpirationDate).ToList();

            int quotasDone = serviceAssociatedDto.AutomaticPaymentDto.QuotasDone;
            int maxQuotas = serviceAssociatedDto.AutomaticPaymentDto.Quotas;
            int countBills = billsUpdate.Count();
            var subtotal = quotasDone + countBills;

            foreach (var bill in billsUpdate)
            {
                // Facturas que caen en el rango de pago
                if (bill.ExpirationDate.AddDays(-serviceAssociatedDto.AutomaticPaymentDto.DaysBeforeDueDate).CompareTo(DateTime.Today.Date) <= 0)
                {
                    //La cantidad facturas a pagar no puede ser mayor al maximo ingresado por el usuario.
                    //Si hay mas, pago las mas viejas
                    if (maxQuotas >= subtotal || serviceAssociatedDto.AutomaticPaymentDto.UnlimitedQuotas)
                    {
                        if (serviceAssociatedDto.AutomaticPaymentDto.UnlimitedAmount ||
                            bill.Amount <= serviceAssociatedDto.AutomaticPaymentDto.Maximum)
                        {
                            finalList.Add(bill);
                            billsDictionary[bill.Id] = PaymentResultTypeDto.BillOk;
                        }
                        else
                        {
                            BillExceedsAmount(serviceAssociatedDto, bill);
                            billsDictionary[bill.Id] = PaymentResultTypeDto.ExceededPaymentsAmount;
                        }
                    }
                    else
                    {
                        BillExceedsQuotas(serviceAssociatedDto, bill);
                        billsDictionary[bill.Id] = PaymentResultTypeDto.ExceededPaymentsCount;
                    }
                }
                subtotal--;
            }

            finalList = finalList.OrderBy(x => x.ExpirationDate).ToList();
            return finalList;
        }

        private ICollection<BillDto> BillsEnableToPaySucive(ServiceAssociatedDto serviceAssociatedDto,
            ICollection<BillDto> bills, ref Dictionary<Guid, PaymentResultTypeDto> billsDictionary)
        {
            var billsUpdate = bills.Where(b => b.Payable).OrderBy(b => b.ExpirationDate).ToList();

            var days = serviceAssociatedDto.AutomaticPaymentDto.DaysBeforeDueDate;
            var patentToPayLines = serviceAssociatedDto.AutomaticPaymentDto.SuciveAnnualPatent ?
                CheckSucivePatent(billsUpdate, days) : "";

            var linesToPay = "";
            //Si no hay patente anual a pagar, pagos las vencidas y dentro del rango de dias del usuario. 
            //Si hay, pago todas hasta la primera cuota
            linesToPay = LinesWithAnnualPatent(billsUpdate, days, patentToPayLines);

            //Chequeo si se puede pagar este listado
            var dtpo = GetDeptoFromService(serviceAssociatedDto);
            if (string.IsNullOrEmpty(linesToPay))
            {
                return new List<BillDto>();
            }
            var gateway =
                serviceAssociatedDto.ServiceDto.ServiceGatewaysDto.FirstOrDefault(
                    x => x.Gateway.Enum == (int)bills.FirstOrDefault().Gateway);

            var idPadron = GetIdPadron(serviceAssociatedDto);
            var billDto = _serviceBill.ChekBills(linesToPay, int.Parse(idPadron), dtpo, bills.FirstOrDefault().Gateway, gateway.ReferenceId);

            if (billDto != null)
            {
                int quotasDone = serviceAssociatedDto.AutomaticPaymentDto.QuotasDone;
                int maxQuotas = serviceAssociatedDto.AutomaticPaymentDto.Quotas;

                //La cantidad facturas a pagar no puede ser mayor al maximo ingresado por el usuario. 
                //Si hay mas, pago las mas viejas
                if (!serviceAssociatedDto.AutomaticPaymentDto.UnlimitedQuotas && maxQuotas < quotasDone)
                {
                    BillExceedsQuotas(serviceAssociatedDto, billDto);
                    foreach (var bill in bills)
                    {
                        billsDictionary[bill.Id] = PaymentResultTypeDto.ExceededPaymentsCount;
                    }
                    return new List<BillDto>();
                }

                //Chequeo que el monto maximo no sobrepase el configurado
                if (!serviceAssociatedDto.AutomaticPaymentDto.UnlimitedAmount &&
                    billDto.Amount > serviceAssociatedDto.AutomaticPaymentDto.Maximum)
                {
                    BillExceedsAmount(serviceAssociatedDto, billDto);
                    foreach (var bill in bills)
                    {
                        billsDictionary[bill.Id] = PaymentResultTypeDto.ExceededPaymentsAmount;
                    }
                    return new List<BillDto>();
                }
            }
            return new List<BillDto> { billDto };
        }

        private string CheckSucivePatent(IEnumerable<BillDto> bills, int days)
        {
            var now = DateTime.Today;
            var patente = bills.Where(b => b.Description.Contains("PATENTE") && b.ExpirationDate.CompareTo(now) >= 0).OrderBy(b => b.ExpirationDate).GroupBy(b => b.Discount).ToList();

            //cuotas de patentes no vencidas
            foreach (var pat in patente)
            {
                //el año de la patente anual a pagar no puede ser menor a este año
                if (pat.Key >= now.Year)
                {
                    //para ser anual tienen que ser 6 cuotas
                    if (pat.Count() == 6)
                    {
                        var bill = pat.First();
                        //Tiene que estar dentro del rango de dias habilitados por el usuario para pagar
                        if (bill.ExpirationDate.AddDays(-days).CompareTo(DateTime.Today.Date) <= 0)
                        {
                            var lines = pat.Select(b => b.Line);
                            return String.Join("", lines);
                        }
                    }
                }
            }
            return "";
        }

        private string LinesWithAnnualPatent(IEnumerable<BillDto> bills, int days, string linePatents)
        {
            var result = "";
            if (string.IsNullOrEmpty(linePatents))
            {
                //PAGO SOLO UNA
                foreach (var bill in bills)
                {
                    if (DateTime.Today.Date.CompareTo(bill.ExpirationDate) < 0)
                    {
                        //facturas con fecha de vencimineto mayor a la fecha de hoy
                        if (bill.ExpirationDate.AddDays(-days).CompareTo(DateTime.Today.Date) <= 0)
                        {
                            result = result + bill.Line;
                        }
                    }
                    else
                    {
                        //ESTA VENCIDA. SE AGREGAR AL PAGO
                        result = result + bill.Line;
                    }
                }
            }
            else
            {
                //PAGO ANUAL
                var next = true;
                var firstPatent = String.IsNullOrEmpty(linePatents) ? "" : linePatents.Split(';').First();
                foreach (var bill in bills)
                {
                    if (next)
                    {
                        //ESTA VENCIDA. SE AGREGAR AL PAGO
                        result = result + bill.Line;
                        if (bill.Line.Contains(firstPatent))
                            next = false;
                    }
                }
                return result + linePatents;
            }

            return result;
        }

        private int GetDeptoFromService(ServiceAssociatedDto dto)
        {
            var service = _serviceService.GetById(dto.ServiceId);
            var dtpo = (int)service.Departament;

            return dtpo;
        }

        private void BillExceedsQuotas(ServiceAssociatedDto serviceAssociatedDto, BillDto bill)
        {
            _loggerHelper.LogBillExceedsQuotas(serviceAssociatedDto, bill);

            //Notificacion al usuario
            _userNotificationFactory.BillExceedsQuotasNotification(serviceAssociatedDto);

            //Notificacion al sistema
            _systemNotificationFactory.BillExceedsQuotasLoggerNotification(serviceAssociatedDto);
        }

        private void BillExceedsAmount(ServiceAssociatedDto serviceAssociatedDto, BillDto bill)
        {
            _loggerHelper.LogBillExceedsAmount(serviceAssociatedDto, bill);

            //Notificacion al usuario
            _userNotificationFactory.BillExceedsAmountNotification(serviceAssociatedDto, bill);

            //Notificacion al sistema
            _systemNotificationFactory.BillExceedsAmountLoggerNotification(serviceAssociatedDto, bill);
        }

        private void GetBillsHandledException(Exception e, ServiceAssociatedDto serviceAssociatedDto)
        {
            _loggerHelper.LogGetBillsHandledException(e, serviceAssociatedDto.Id);
        }

        private void GetBillsException(Exception e, ServiceAssociatedDto serviceAssociatedDto)
        {
            _loggerHelper.LogGetBillsException(e, serviceAssociatedDto.Id);
        }

        private string GetIdPadron(ServiceAssociatedDto serviceAssociatedDto)
        {
            //Si el servicio asociado no tiene idPadron (ref6), entonces lo busca
            var idPadron = serviceAssociatedDto.ReferenceNumber6;
            if (string.IsNullOrEmpty(idPadron))
            {
                var references = new Dictionary<string, string>();

                if (!string.IsNullOrEmpty(serviceAssociatedDto.ServiceDto.ReferenceParamName))
                    references.Add(serviceAssociatedDto.ServiceDto.ReferenceParamName, serviceAssociatedDto.ReferenceNumber);
                if (!string.IsNullOrEmpty(serviceAssociatedDto.ServiceDto.ReferenceParamName2))
                    references.Add(serviceAssociatedDto.ServiceDto.ReferenceParamName2, serviceAssociatedDto.ReferenceNumber2);
                if (!string.IsNullOrEmpty(serviceAssociatedDto.ServiceDto.ReferenceParamName3))
                    references.Add(serviceAssociatedDto.ServiceDto.ReferenceParamName3, serviceAssociatedDto.ReferenceNumber3);
                if (!string.IsNullOrEmpty(serviceAssociatedDto.ServiceDto.ReferenceParamName4))
                    references.Add(serviceAssociatedDto.ServiceDto.ReferenceParamName4, serviceAssociatedDto.ReferenceNumber4);
                if (!string.IsNullOrEmpty(serviceAssociatedDto.ServiceDto.ReferenceParamName5))
                    references.Add(serviceAssociatedDto.ServiceDto.ReferenceParamName5, serviceAssociatedDto.ReferenceNumber5);
                if (!string.IsNullOrEmpty(serviceAssociatedDto.ServiceDto.ReferenceParamName6))
                    references.Add(serviceAssociatedDto.ServiceDto.ReferenceParamName6, serviceAssociatedDto.ReferenceNumber6);

                var accountIdPadron = _serviceBill.CheckAccount(new RegisteredUserBillFilterDto
                {
                    References = references,
                    ServiceId = serviceAssociatedDto.ServiceId,
                    ScheduledPayment = true,
                });

                _loggerHelper.LogIdPadronForServiceAssociate(serviceAssociatedDto.Id, accountIdPadron);

                if (accountIdPadron != -1)
                {
                    idPadron = accountIdPadron.ToString();
                }
            }
            return idPadron;
        }

    }
}