using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using VisaNet.Application.Implementations.Base;
using VisaNet.Application.Interfaces;
using VisaNet.Application.Interfaces.Cache;
using VisaNet.Common.Exceptions;
using VisaNet.Common.Logging.NLog;
using VisaNet.Components.Banred.Interfaces;
using VisaNet.Components.Geocom.Interfaces;
using VisaNet.Components.Sistarbanc.Interfaces;
using VisaNet.Components.Sucive.Interfaces;
using VisaNet.Domain.Entities;
using VisaNet.Domain.Entities.Enums;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Domain.EntitiesDtos.ExternalRequest;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Repository.Interfaces.Interfaces;

namespace VisaNet.Application.Implementations
{
    public class ServiceBill : BaseService<Bill, BillDto>, IServiceBill
    {
        private readonly IServiceParameters _serviceParameters;
        private readonly IServiceBanred _serviceBanred;
        private readonly ISistarbancAccess _sistarbancAccess;
        private readonly IServiceServiceAssosiate _serviceAssosiate;
        private readonly IServiceApplicationUser _appUser;
        private readonly IServiceAnonymousUser _anonymousUser;
        private readonly IRepositorySistarbancUser _repositorySistarbancUser;
        private readonly ISuciveAccess _suciveAccess;
        private readonly IServiceBin _serviceBin;
        private readonly IGeocomAccess _geocomAccess;
        private readonly IServiceHighway _serviceHighway;
        private readonly IServiceService _serviceService;
        private readonly IRepositoryBank _repositoryBank;
        private readonly IRepositoryBill _repositoryBill;
        private readonly IServiceMemoryCache _serviceMemoryCache;

        private int cacheDuration;
        public ServiceBill(IRepositoryBill repository, IServiceBanred serviceBanred, ISistarbancAccess sistarbancAccess,
            IServiceParameters serviceParameters, IServiceServiceAssosiate serviceAssosiate, IServiceApplicationUser appUser,
            IServiceAnonymousUser anonymousUser, IRepositorySistarbancUser repositorySistarbancUser, ISuciveAccess suciveAccess,
            IServiceBin serviceBin, IGeocomAccess geocomAccess, IServiceHighway serviceHighway, IServiceService serviceService,
            IRepositoryBank repositoryBank, IServiceMemoryCache serviceMemoryCache)
            : base(repository)
        {
            _repositoryBill = repository;
            _serviceBanred = serviceBanred;
            _sistarbancAccess = sistarbancAccess;
            _serviceParameters = serviceParameters;
            _serviceAssosiate = serviceAssosiate;
            _appUser = appUser;
            _anonymousUser = anonymousUser;
            _repositorySistarbancUser = repositorySistarbancUser;
            _suciveAccess = suciveAccess;
            _serviceBin = serviceBin;
            _geocomAccess = geocomAccess;
            _serviceHighway = serviceHighway;
            _serviceService = serviceService;
            _repositoryBank = repositoryBank;           

            _serviceMemoryCache = serviceMemoryCache;
            cacheDuration = !string.IsNullOrEmpty(ConfigurationManager.AppSettings["CacheDuration"]) ? int.Parse(ConfigurationManager.AppSettings["CacheDuration"]) : 0;
        }

        public override IQueryable<Bill> GetDataForTable()
        {
            return Repository.AllNoTracking();
        }

        private ICollection<BillDto> GetBills(IBillFilterDto filterDto)
        {
            var service = _serviceService.All(null, x => x.Id == filterDto.ServiceId,
                x => x.ServiceGateways, x => x.ServiceGateways.Select(g => g.Gateway), x => x.ServiceContainer).First();

            var idAppContainer = service.ServiceContainerDto != null ? service.ServiceContainerDto.UrlName : service.UrlName;
            IsServiceMomentarilyDisabled(service.UrlName, idAppContainer, service.Name);

            NLogLogger.LogEvent(NLogType.Info, string.Format("Bills - intento de consulta por facturas. Ente: {0}", service.Name), OperationType.GetBills, Repository.GetDataLog());

            var servicesGatewaysList = service.ServiceGatewaysDto.Where(x => x.Active).ToList();
            if (!servicesGatewaysList.Any())
            {
                NLogLogger.LogEvent(NLogType.Info, string.Format("Bills - el servidor {0} no tiene gateways activos", service.Name), OperationType.GetBills, Repository.GetDataLog());
                _serviceService.NotifyServiceWithoutActiveGateway(service);
            }
            var refs = service.LoadReferences(filterDto.References);
            ICollection<BillDto> bills = null;
            var exceptionList = new List<Exception>();
            ServiceGatewayDto bestServiceGateway = null;
            var filter = filterDto as RegisteredUserBillFilterDto;
            while (servicesGatewaysList.Any() && (bills == null || !bills.Any()))
            {
                try
                {
                    bestServiceGateway = _serviceService.GetBestGateway(service, servicesGatewaysList, filter != null ? filter.CardId : null);

                    var bestServiceGatewayObject = new { bestServiceGateway = bestServiceGateway };
                    NLogLogger.LogEvent(NLogType.Info, string.Format("Bills - Mejor puerta de enlace {0}", bestServiceGateway.Gateway.Name), OperationType.GetBills, Repository.GetDataLog(), bestServiceGatewayObject);

                    bills = GetBillsFromGateway(service, bestServiceGateway, refs, filter != null ? filter.UserId : null, filterDto.ScheduledPayment);

                }
                catch (Exception exception)
                {
                    exceptionList.Add(exception);
                    NLogLogger.LogEvent(exception, OperationType.GetBills, Repository.GetDataLog());
                }

                servicesGatewaysList.Remove(bestServiceGateway);
            }

            if (bills != null && bills.Any())
            {
                var billsObject = new { bills = bills };
                NLogLogger.LogEvent(NLogType.Info, string.Format("Bills - Cantidad de facturas del servicio {0}: {1}", service.Name, bills.Count), OperationType.GetBills, Repository.GetDataLog(), billsObject);
                return bills;
            }

            if (exceptionList.Any())
            {
                throw exceptionList.First();
            }

            return null;
        }

        public ApplicationUserBillDto GetBillsForRegisteredUser(RegisteredUserBillFilterDto filter)
        {
            ApplicationUserDto user = null;
            if (filter != null && filter.UserId.HasValue)
            {
                user = _appUser.GetById(filter.UserId.Value);
            }
            var bills = GetBills(filter);

            return new ApplicationUserBillDto()
            {
                Bills = bills,
                User = user
            };
        }

        public AnonymousUserBillDto GetBillsForAnonymousUser(AnonymousUserBillFilterDto filter)
        {
            AnonymousUserDto user = null;
            if (filter == null || filter.AnonymousUserDto == null)
            {
                throw new BusinessException(CodeExceptions.ANONYMOUS_USER_MISSING);
            }
            user = _anonymousUser.CreateOrEditAnonymousUser(filter.AnonymousUserDto);
            var bills = GetBills(filter);
            return new AnonymousUserBillDto()
            {
                Bills = bills,
                User = user
            };
        }

        public ICollection<BillDto> TestGatewayGetBills(TestGatewaysFilterDto filterDto)
        {
            var service = _serviceService.GetById(filterDto.ServiceId);

            NLogLogger.LogEvent(NLogType.Info, "Intento de obtencion de factura para " + filterDto.GatewayDto.Gateway.Name);
            var parameters = _serviceParameters.GetDataForTable().First();
            var brou = _repositoryBank.AllNoTracking().FirstOrDefault(b => b.Name.Equals("BROU", StringComparison.InvariantCultureIgnoreCase));
            ICollection<BillDto> billsDtos = null;
            var gate = filterDto.GatewayDto;
            if (filterDto.GatewayDto.Gateway.Enum == (int)GatewayEnumDto.Banred)
            {
                string idAgenteExterno = parameters.Banred.Code;
                billsDtos = _serviceBanred.ConsultaFacturas(idAgenteExterno, filterDto.GatewayDto.ReferenceId, filterDto.References).Select(x => ToDto(x, gate.Id)).ToList();
            }
            if (filterDto.GatewayDto.Gateway.Enum == (int)GatewayEnumDto.Sistarbanc)
            {
                var idBanco = parameters.Sistarbanc.Code;
                billsDtos = _sistarbancAccess.GetBills(idBanco, brou.Code.ToString(), filterDto.GatewayDto.ReferenceId, filterDto.GatewayDto.ServiceType, filterDto.References).Select(x => ToDto(x, gate.Id)).ToList();
            }
            if (filterDto.GatewayDto.Gateway.Enum == (int)GatewayEnumDto.Sucive)
            {
                var list = _suciveAccess.GetBills(filterDto.References, filterDto.GatewayDto.ReferenceId, filterDto.GatewayDto.ServiceType, (int)service.Departament);
                billsDtos = list.Select(x => ToDto(x, gate.Id)).ToList();
            }
            if (filterDto.GatewayDto.Gateway.Enum == (int)GatewayEnumDto.Geocom)
            {
                var list = _geocomAccess.GetBills(filterDto.References, filterDto.GatewayDto.ReferenceId, filterDto.GatewayDto.ServiceType, (int)service.Departament);
                billsDtos = list.Select(x => ToDto(x, gate.Id)).ToList();
            }
            if (filterDto.GatewayDto.Gateway.Enum == (int)GatewayEnumDto.Carretera)
            {
                var list = _serviceHighway.GetBills(filterDto.References, int.Parse(filterDto.GatewayDto.ReferenceId), int.Parse(filterDto.GatewayDto.ServiceType));
                var updated = RemoveHighwayBillsAlreadyInVisaNet(list, filterDto.ServiceId);
                if (updated == null || !updated.Any())
                    return billsDtos;

                var data = updated.Min(x => x.FchVencimiento);
                //SOLO PUEDO PAGAR LAS FACTURAS DE MENOR FECHA
                billsDtos = updated.Select(x => ToDto(x, gate.Id, data, false)).ToList();
                return billsDtos;
            }
            if (filterDto.GatewayDto.Gateway.Enum == (int)GatewayEnumDto.Importe)
            {
            }
            if (filterDto.GatewayDto.Gateway.Enum == (int)GatewayEnumDto.Apps)
            {
            }
            return RemoveBillsAlreadyInVisaNet(billsDtos, filterDto.ServiceId);
        }

        public int CheckAccount(IBillFilterDto filterDto)
        {
            var service = _serviceService.All(null, x => x.Id == filterDto.ServiceId, x => x.ServiceGateways, x => x.ServiceGateways.Select(g => g.Gateway)).First();
            var servicesGatewaysList = service.ServiceGatewaysDto.Where(x => x.Active).ToList();
            var refs = service.LoadReferences(filterDto.References);
            var result = -1;
            var exceptionList = new List<Exception>();
            ServiceGatewayDto bestServiceGateway = null;
            var filter = filterDto as RegisteredUserBillFilterDto;
            while (servicesGatewaysList.Any() && (result < 1))
            {
                try
                {
                    bestServiceGateway = _serviceService.GetBestGateway(service, servicesGatewaysList, filter != null ? filter.CardId : null);
                    result = CheckAccountFromGateway(service, bestServiceGateway, refs);
                }
                catch (Exception exception)
                {
                    NLogLogger.LogEvent(NLogType.Info, "ServiceBill - CheckAccount - Exception - " + exception.Message);
                    exceptionList.Add(exception);
                }

                servicesGatewaysList.Remove(bestServiceGateway);
            }

            if (result > 0)
                return result;

            if (exceptionList.Any())
                throw exceptionList.First();

            return -1;
        }

        private int CheckAccountFromGateway(ServiceDto service, ServiceGatewayDto bestServiceGateway, string[] references)
        {
            var parameters = _serviceParameters.GetDataForTable().First();
            int result = 0;
            if (bestServiceGateway.Gateway.Enum == (int)GatewayEnumDto.Banred)
            {
                string idAgenteExterno = parameters.Banred.Code;
                result = _serviceBanred.CheckAccount(idAgenteExterno, bestServiceGateway.ReferenceId, references);
            }
            if (bestServiceGateway.Gateway.Enum == (int)GatewayEnumDto.Sistarbanc)
            {
                string idBanco = parameters.Sistarbanc.Code;
                result = _sistarbancAccess.CheckAccount(idBanco, bestServiceGateway.ReferenceId, bestServiceGateway.ServiceType, references);
            }
            if (bestServiceGateway.Gateway.Enum == (int)GatewayEnumDto.Sucive)
            {
                result = _suciveAccess.CheckAccount(references, bestServiceGateway.ReferenceId, bestServiceGateway.ServiceType, (int)service.Departament);
            }
            if (bestServiceGateway.Gateway.Enum == (int)GatewayEnumDto.Geocom)
            {
                result = _geocomAccess.CheckAccount(references, bestServiceGateway.ReferenceId, bestServiceGateway.ServiceType, (int)service.Departament);
            }
            if (bestServiceGateway.Gateway.Enum == (int)GatewayEnumDto.Carretera)
            {
                result = _serviceHighway.CheckAccount(int.Parse(bestServiceGateway.ReferenceId), int.Parse(bestServiceGateway.ServiceType), references);
            }
            if (bestServiceGateway.Gateway.Enum == (int)GatewayEnumDto.Importe)
            {
                result = 1;
            }
            return result;
        }

        private ICollection<BillDto> GetBillsFromGateway(ServiceDto service, ServiceGatewayDto bestServiceGateway, string[] references, Guid? userId, bool scheduledPayment)
        {
            ICollection<BillDto> billsDtos = null;
            var parameters = _serviceParameters.GetDataForTable().First();
            var brou = _repositoryBank.AllNoTracking().FirstOrDefault(b => b.Name.Equals("BROU", StringComparison.InvariantCultureIgnoreCase));

            if (bestServiceGateway.Gateway.Enum == (int)GatewayEnumDto.Banred)
            {
                string idAgenteExterno = parameters.Banred.Code;
                string cacheKey = string.Format("{0}_{1}_{2}", GatewayEnumDto.Banred.ToString(), bestServiceGateway.ReferenceId,string.Join("_", references.Where(w => !string.IsNullOrWhiteSpace(w)).ToArray()) );
                billsDtos = _serviceMemoryCache.GetOrSet(cacheKey, cacheDuration, () => _serviceBanred.ConsultaFacturas(idAgenteExterno, bestServiceGateway.ReferenceId, references).Select(x => ToDto(x, bestServiceGateway.GatewayId)).ToList());
            }
            if (bestServiceGateway.Gateway.Enum == (int)GatewayEnumDto.Sistarbanc)
            {
                var idBanco = parameters.Sistarbanc.Code;
                billsDtos = _sistarbancAccess.GetBills(idBanco, brou.Code.ToString(), bestServiceGateway.ReferenceId, bestServiceGateway.ServiceType, references).Select(x => ToDto(x, bestServiceGateway.GatewayId)).ToList();
            }
            if (bestServiceGateway.Gateway.Enum == (int)GatewayEnumDto.Sucive)
            {
                if (userId.HasValue)
                {
                    var idPadron = CheckIfHasIdPadron(service.Id, userId.Value, references);
                    references[5] = idPadron;
                }
                var list = _suciveAccess.GetBills(references, bestServiceGateway.ReferenceId, bestServiceGateway.ServiceType, (int)service.Departament);
                billsDtos = list.Select(x => ToDto(x, bestServiceGateway.GatewayId)).ToList();
            }
            if (bestServiceGateway.Gateway.Enum == (int)GatewayEnumDto.Geocom)
            {
                if (userId.HasValue)
                {
                    var idPadron = CheckIfHasIdPadron(service.Id, userId.Value, references);
                    references[5] = idPadron;
                }
                var list = _geocomAccess.GetBills(references, bestServiceGateway.ReferenceId, bestServiceGateway.ServiceType, (int)service.Departament);
                billsDtos = list.Select(x => ToDto(x, bestServiceGateway.GatewayId)).ToList();
            }
            if (bestServiceGateway.Gateway.Enum == (int)GatewayEnumDto.Carretera)
            {
                var list = _serviceHighway.GetBills(references, int.Parse(bestServiceGateway.ReferenceId), int.Parse(bestServiceGateway.ServiceType));
                var updated = RemoveHighwayBillsAlreadyInVisaNet(list, service.Id).ToList();
                if (!updated.Any())
                    return billsDtos;

                var data = updated.Min(x => x.FchVencimiento);
                //PARA PAGO MANUAL SOLO PUEDO PAGAR LAS FACTURAS DE MENOR FECHA
                billsDtos = updated.Select(x => ToDto(x, bestServiceGateway.GatewayId, data, scheduledPayment)).ToList();
                return billsDtos;
            }
            if (bestServiceGateway.Gateway.Enum == (int)GatewayEnumDto.Importe)
            {
            }
            if (bestServiceGateway.Gateway.Enum == (int)GatewayEnumDto.Apps)
            {

            }
            return RemoveBillsAlreadyInVisaNet(billsDtos, service.Id);
        }

        public ICollection<BillDto> PayBills(NotifyPaymentDto notify)
        {
            try
            {
                var parameters = _serviceParameters.GetDataForTable().First();
                var brouBankCode = parameters.SistarbancBrou.Code;

                if (notify.GatewayEnum == GatewayEnumDto.Banred)
                {
                    string idAgenteExterno = parameters.Banred.Code;
                    foreach (var bill in notify.Bills)
                    {
                        var banredTransfer = _serviceBanred.PagarFactura(idAgenteExterno, notify.ServiceGatewayReferenceId, notify.References, bill.BillExternalId, bill.Amount, bill.DiscountAmount, bill.Currency, bill.ExpirationDate.Year.ToString() + bill.ExpirationDate.Month + bill.ExpirationDate.Day, notify.TransactionNumber);
                        bill.GatewayTransactionId = banredTransfer;
                    }
                    return notify.Bills;
                }
                if (notify.GatewayEnum == GatewayEnumDto.Sistarbanc)
                {
                    string idBanco = parameters.Sistarbanc.Code;
                    var binDto = _serviceBin.Find(int.Parse(notify.Bin)) ?? _serviceBin.GetDefaultBin();
                    //ESTO ESTA ECHO A MANO A PEDIDO DE MATIAS. UN USUARIO REGISTRADO O NO, TIENE DOS USUARIOS DE SISTARBANC (BROU O NO)
                    //SI LA TARJETA ES DEL BROU SE PASA EL CODIGO DEL BANCO REGISTRADO EN EL BIN. DE CASO CONTRARIO, SE PASA EL DE VISA
                    foreach (var bill in notify.Bills)
                    {
                        var userIdentifier = "";
                        if (notify.UserRegistred)
                        {
                            var user = _appUser.GetById(notify.UserId, u => u.SistarbancUser, u => u.SistarbancBrouUser);
                            if (binDto.BankDtoId != null && binDto.BankDtoId != Guid.Empty)
                            {
                                //CODIGO DE BROU
                                if (binDto.BankDto.Name.Equals("BROU", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    if (user.SistarbancBrouUser != null && !user.SistarbancBrouUser.Active)
                                    {
                                        _sistarbancAccess.AltaUsuario(brouBankCode,
                                            user.SistarbancBrouUser.UniqueIdentifier.ToString(), notify.Name, notify.Surname,
                                            notify.TransactionNumber);
                                        _repositorySistarbancUser.ActiveUserSistarbanc(user.SistarbancBrouUserId);
                                    }
                                    userIdentifier = user.SistarbancBrouUser.UniqueIdentifier.ToString();
                                    _sistarbancAccess.PagoReciboLif(brouBankCode, notify.ServiceGatewayReferenceId,
                                        notify.ServiceType, notify.References, bill, userIdentifier, notify.TransactionNumber,
                                        notify.AutomaticPaymentDto);
                                }
                                else
                                {
                                    if (user.SistarbancUser != null && !user.SistarbancUser.Active)
                                    {
                                        _sistarbancAccess.AltaUsuario(idBanco, user.SistarbancUser.UniqueIdentifier.ToString(), notify.Name, notify.Surname, notify.TransactionNumber);
                                        _repositorySistarbancUser.ActiveUserSistarbanc(user.SistarbancUserId);
                                    }
                                    userIdentifier = user.SistarbancUser.UniqueIdentifier.ToString();
                                    _sistarbancAccess.PagoReciboLif(idBanco, notify.ServiceGatewayReferenceId, notify.ServiceType, notify.References, bill, userIdentifier, notify.TransactionNumber, notify.AutomaticPaymentDto);
                                }
                            }
                            else
                            {
                                if (user.SistarbancUser != null && !user.SistarbancUser.Active)
                                {
                                    _sistarbancAccess.AltaUsuario(idBanco, user.SistarbancUser.UniqueIdentifier.ToString(), notify.Name, notify.Surname, notify.TransactionNumber);
                                    _repositorySistarbancUser.ActiveUserSistarbanc(user.SistarbancUserId);
                                }
                                userIdentifier = user.SistarbancUser.UniqueIdentifier.ToString();
                                _sistarbancAccess.PagoReciboLif(idBanco, notify.ServiceGatewayReferenceId, notify.ServiceType, notify.References, bill, userIdentifier, notify.TransactionNumber, notify.AutomaticPaymentDto);
                            }

                        }
                        else
                        {
                            var user = _anonymousUser.GetById(notify.UserId, u => u.SistarbancUser, u => u.SistarbancBrouUser);
                            if (binDto.BankDtoId != null && binDto.BankDtoId != Guid.Empty)
                            {
                                //CODIGO DE BROU
                                if (binDto.BankDto.Name.Equals("BROU", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    if (user.SistarbancBrouUser != null && !user.SistarbancBrouUser.Active)
                                    {
                                        _sistarbancAccess.AltaUsuario(brouBankCode,
                                            user.SistarbancBrouUser.UniqueIdentifier.ToString(), notify.Name, notify.Surname,
                                            notify.TransactionNumber);
                                        _repositorySistarbancUser.ActiveUserSistarbanc(user.SistarbancBrouUserId);
                                    }
                                    userIdentifier = user.SistarbancBrouUser.UniqueIdentifier.ToString();
                                    _sistarbancAccess.PagoReciboLif(brouBankCode, notify.ServiceGatewayReferenceId,
                                        notify.ServiceType, notify.References, bill, userIdentifier, notify.TransactionNumber,
                                        notify.AutomaticPaymentDto);
                                }
                                else
                                {
                                    if (user.SistarbancUser != null && !user.SistarbancUser.Active)
                                    {
                                        _sistarbancAccess.AltaUsuario(idBanco, user.SistarbancUser.UniqueIdentifier.ToString(), notify.Name, notify.Surname, notify.TransactionNumber);
                                        _repositorySistarbancUser.ActiveUserSistarbanc(user.SistarbancUserId);
                                    }
                                    userIdentifier = user.SistarbancUser.UniqueIdentifier.ToString();
                                    _sistarbancAccess.PagoReciboLif(idBanco, notify.ServiceGatewayReferenceId, notify.ServiceType, notify.References, bill, userIdentifier, notify.TransactionNumber, notify.AutomaticPaymentDto);
                                }
                            }
                            else
                            {
                                if (user.SistarbancUser != null && !user.SistarbancUser.Active)
                                {
                                    _sistarbancAccess.AltaUsuario(idBanco, user.SistarbancUser.UniqueIdentifier.ToString(), notify.Name, notify.Surname, notify.TransactionNumber);
                                    _repositorySistarbancUser.ActiveUserSistarbanc(user.SistarbancUserId);
                                }
                                userIdentifier = user.SistarbancUser.UniqueIdentifier.ToString();
                                _sistarbancAccess.PagoReciboLif(idBanco, notify.ServiceGatewayReferenceId, notify.ServiceType, notify.References, bill, userIdentifier, notify.TransactionNumber, notify.AutomaticPaymentDto);
                            }

                        }
                    }

                    return notify.Bills;
                }
                if (notify.GatewayEnum == GatewayEnumDto.Sucive)
                {
                    var bill = notify.Bills.FirstOrDefault();
                    var codigo = _suciveAccess.ConfirmPayment(Int64.Parse(bill.SucivePreBillNumber), "0", notify.TransactionNumber, notify.ServiceDepartament);
                    notify.Bills.FirstOrDefault().BillExternalId = codigo + "";
                    notify.Bills.FirstOrDefault().GatewayTransactionId = codigo + "";
                    return notify.Bills;
                }
                if (notify.GatewayEnum == GatewayEnumDto.Geocom)
                {
                    var bill = notify.Bills.FirstOrDefault();
                    var codigo = _geocomAccess.ConfirmPayment(Int32.Parse(bill.SucivePreBillNumber), "0", notify.TransactionNumber, notify.ServiceDepartament, notify.ServiceGatewayReferenceId);
                    notify.Bills.FirstOrDefault().BillExternalId = codigo + "";
                    notify.Bills.FirstOrDefault().GatewayTransactionId = codigo + "";
                    return notify.Bills;
                }
                if (notify.GatewayEnum == GatewayEnumDto.Carretera)
                {
                    var result = _serviceHighway.ConfirmPayment(notify.ServiceGatewayReferenceId, notify.ServiceType, notify.References, notify.Bills, notify.TransactionNumber);
                    return result ? notify.Bills : null;
                }
                if (notify.GatewayEnum == GatewayEnumDto.Importe)
                {
                    notify.Bills.FirstOrDefault().BillExternalId = "visa-" + notify.CybersourceTransactionNumber;
                    return notify.Bills;
                }
                if (notify.GatewayEnum == GatewayEnumDto.Apps)
                {
                    //Antes se notificaba al comercio aca pero se saco y se tiene que encargar de hacerlo el que llama al proceso de pago luego de obtener la respuesta
                    return notify.Bills;
                }
                if (notify.GatewayEnum == GatewayEnumDto.PagoLink)
                {
                    //se notifica al servicio post persistencia del pago
                    return notify.Bills;
                }
            }
            catch (Exception ex)
            {
                NLogLogger.LogEvent(NLogType.Info, "Service bill Excepcion");
                NLogLogger.LogEvent(ex);
                throw;
            }
            return null;
        }

        public ICollection<BillDto> GetBillsForDashboard(GatewayEnumDto gateway, Guid serviceId, string gatewayReference, string serviceType, string referenceNumber, string referenceNumber2, string referenceNumber3, string referenceNumber4, string referenceNumber5, string referenceNumber6, int serviceDepartament)
        {
            var parameters = _serviceParameters.GetDataForTable().First();
            var refs = new[] { referenceNumber, referenceNumber2, referenceNumber3, referenceNumber4, referenceNumber5, referenceNumber6 };
            var gate = _serviceService.GetGateway(gateway);
            ICollection<BillDto> billsDtos = null;
            if (gateway == GatewayEnumDto.Banred)
            {
                string idAgenteExterno = parameters.Banred.Code;
                billsDtos = _serviceBanred.ConsultaFacturas(idAgenteExterno, gatewayReference, refs).Select(x => ToDto(x, gate.Id)).ToList();
            }
            if (gateway == GatewayEnumDto.Sistarbanc)
            {
                string idBanco = parameters.Sistarbanc.Code;
                billsDtos = _sistarbancAccess.ServicioImpagoLif(idBanco, gatewayReference, serviceType, refs).Select(x => ToDto(x, gate.Id)).ToList();
            }
            if (gateway == GatewayEnumDto.Sucive)
            {
                var list = _suciveAccess.GetBills(refs, gatewayReference, serviceType, serviceDepartament);
                billsDtos = list.Select(x => ToDto(x, gate.Id)).ToList();
            }
            if (gateway == GatewayEnumDto.Geocom)
            {
                var list = _geocomAccess.GetBills(refs, gatewayReference, serviceType, serviceDepartament);
                billsDtos = list.Select(x => ToDto(x, gate.Id)).ToList();
            }

            if (gateway == GatewayEnumDto.Carretera)
            {
                var list = _serviceHighway.GetBills(refs, int.Parse(gatewayReference), int.Parse(serviceType));
                var updated = RemoveHighwayBillsAlreadyInVisaNet(list, serviceId);
                if (updated == null || !updated.Any())
                    return billsDtos;

                var data = updated.Min(x => x.FchVencimiento);
                //PARA PAGO MANUAL SOLO PUEDO PAGAR LAS FACTURAS DE MENOR FECHA
                billsDtos = updated.Select(x => ToDto(x, gate.Id, data, false)).ToList();
                return billsDtos;
            }
            return RemoveBillsAlreadyInVisaNet(billsDtos, serviceId);
        }

        public BillDto ToDto(BillBanredDto entity, Guid gatewayId)
        {
            return new BillDto
            {
                Id = entity.Id == Guid.Empty ? Guid.NewGuid() : entity.Id,
                ExpirationDate = entity.ExpirationDate,
                Description = entity.Description,
                BillExternalId = entity.Number,
                Payable = entity.Payable,
                FinalConsumer = entity.FinalConsumer,
                TaxedAmount = entity.TaxedAmount,
                Amount = entity.Amount,
                Currency = entity.Currency,
                GatewayTransactionId = entity.BanredTransactionId,
                Gateway = GatewayEnumDto.Banred,
                DashboardDescription = entity.DashboardDescription,
                ItauPayable = !string.IsNullOrEmpty(entity.PayableType) && (entity.PayableType.Equals("1") || entity.PayableType.Equals("3")),
                GatewayAcceptsPreBills = false, //Banred no acepta Pre-Facturas
                GatewayId = gatewayId,
            };
        }

        public BillDto ToDto(BillSistarbancDto entity, Guid gatewayId)
        {
            return new BillDto
            {
                Id = entity.Id == Guid.Empty ? Guid.NewGuid() : entity.Id,
                ExpirationDate = entity.ExpirationDate,
                Amount = Double.Parse(entity.Amount) / 100,
                Currency = entity.Currency,
                Description = entity.Description[0],
                BillExternalId = entity.BillExternalId,
                FinalConsumer = entity.FinalConsumer,
                TaxedAmount = entity.TaxedAmount / 100,
                GatewayTransactionId = entity.IdTransaccionSTB,
                Gateway = GatewayEnumDto.Sistarbanc,
                Payable = entity.Payable,
                DateInitTransaccion = entity.DateInit,
                //AmountStbWithDiscount = Double.Parse(entity.AmountWithDiscount) / 100,
                GatewayTransactionBrouId = entity.IdTransaccionStbBrou,
                DashboardDescription = entity.DashboardDescription,
                GatewayAcceptsPreBills = false, //Sistarbanc no acepta Pre-Facturas
                GatewayId = gatewayId,
            };
        }

        public BillDto ToDto(BillSuciveDto entity, Guid gatewayId)
        {
            var bill = new BillDto
            {
                Id = entity.Id == Guid.Empty ? Guid.NewGuid() : entity.Id,
                ExpirationDate = entity.ExpirationDate,
                Amount = entity.Amount,
                Currency = entity.Currency,
                Description = entity.Description,
                Gateway = GatewayEnumDto.Sucive,
                Payable = entity.Payable,
                IdPadron = entity.IdPadron,
                Line = entity.Line,
                SucivePreBillNumber = entity.SucivePreBillNumber,
                BillExternalId = entity.Codigo,
                //uso el discount para el pago programado, saber el año en q aplica la patente
                Discount = !string.IsNullOrEmpty(entity.Year) ? int.Parse(entity.Year) : 0,
                DashboardDescription = entity.DashboardDescription,
                GatewayAcceptsPreBills = true, //Sucive acepta Pre-Facturas
                GatewayId = gatewayId,
            };
            if (entity.Details != null && entity.Details.Any())
            {
                bill.Bills = entity.Details.Select(x => new BillDto()
                {
                    Amount = x.Amount,
                    Line = x.Line,
                    Description = x.Description,
                    Currency = x.Currency,
                    ExpirationDate = x.ExpirationDate
                }).ToList();
            }
            return bill;
        }

        public BillDto ToDto(BillGeocomDto entity, Guid gatewayId)
        {
            var bill = new BillDto
            {
                Id = entity.Id == Guid.Empty ? Guid.NewGuid() : entity.Id,
                ExpirationDate = entity.ExpirationDate,
                Amount = entity.Amount,
                Currency = entity.Currency,
                Description = entity.Description,
                Gateway = GatewayEnumDto.Geocom,
                Payable = entity.Payable,
                IdPadron = entity.IdPadron,
                Line = entity.Line,
                SucivePreBillNumber = entity.GeocomPreBillNumber,
                BillExternalId = entity.Codigo,
                //uso el discount para el pago programado, saber el año en q aplica la patente
                Discount = !string.IsNullOrEmpty(entity.Year) ? int.Parse(entity.Year) : 0,
                DashboardDescription = entity.DashboardDescription,
                GatewayAcceptsPreBills = true, //Geocom acepta Pre-Facturas
                GatewayId = gatewayId,
            };
            if (entity.Details != null && entity.Details.Any())
            {
                bill.Bills = entity.Details.Select(x => new BillDto()
                {
                    Amount = x.Amount,
                    Line = x.Line,
                    Description = x.Description,
                    Currency = x.Currency,
                    ExpirationDate = x.ExpirationDate
                }).ToList();
            }
            return bill;
        }

        public BillDto ToDto(HighwayBillDto entity, Guid gatewayId, DateTime data, bool scheduledPayment)
        {
            var billDto = new BillDto
            {
                Id = Guid.NewGuid(),
                ExpirationDate = entity.FchVencimiento,
                Description = entity.Descripcion,
                Gateway = GatewayEnumDto.Carretera,
                BillExternalId = entity.NroFactura,
                FinalConsumer = entity.ConsFinal,
                TaxedAmount = entity.MontoGravado,
                Amount = entity.MontoTotal,
                Currency = entity.Moneda,
                GeneratedDate = entity.FchFactura,
                CreationDate = entity.CreationDate,
                GatewayAcceptsPreBills = false,
                GatewayId = gatewayId,
            };

            if (scheduledPayment)
            {
                //Si es pago programado solo verifico vencimiento
                billDto.Payable = DateTime.Now <= entity.FchVencimiento.AddDays(entity.DiasPagoVenc);
            }
            else
            {
                //Si no, verifico vencimiento y también que sea la factura mas antigua
                billDto.Payable = DateTime.Now <= entity.FchVencimiento.AddDays(entity.DiasPagoVenc) &&
                                  entity.FchVencimiento.CompareTo(data) == 0;
            }

            return billDto;
        }

        public override BillDto Converter(Bill entity)
        {
            if (entity == null) return null;

            return new BillDto
            {
                Id = entity.Id,
                ExpirationDate = entity.ExpirationDate,
                Amount = entity.Amount,
                Currency = entity.Currency,
                Description = entity.Description,
                BillExternalId = entity.BillExternalId,
                PaymentId = entity.PaymentId,
                FinalConsumer = entity.FinalConsumer,
                TaxedAmount = entity.TaxedAmount,
                Discount = entity.Discount,
                DiscountAmount = entity.DiscountAmount,
                GatewayTransactionId = entity.GatewayTransactionId,
                SucivePreBillNumber = entity.SucivePreBillNumber,
                CreationDate = entity.CreationDate
            };
        }

        public override Bill Converter(BillDto entity)
        {
            if (entity == null) return null;

            return new Bill
            {
                Id = entity.Id,
                ExpirationDate = entity.ExpirationDate,
                Amount = entity.Amount,
                Currency = entity.Currency,
                Description = entity.Description,
                BillExternalId = entity.BillExternalId,
                PaymentId = entity.PaymentId,
                FinalConsumer = entity.FinalConsumer,
                TaxedAmount = entity.TaxedAmount,
                Discount = entity.Discount,
                DiscountAmount = entity.DiscountAmount,
                GatewayTransactionId = entity.GatewayTransactionId,
                SucivePreBillNumber = entity.SucivePreBillNumber

            };
        }

        public override void Edit(BillDto entity)
        {
            Repository.ContextTrackChanges = true;
            var entity_db = Repository.GetById(entity.Id);

            entity_db.ExpirationDate = entity.ExpirationDate;
            entity_db.Amount = entity.Amount;
            entity_db.Currency = entity.Currency;
            entity_db.Description = entity.Description;
            entity_db.BillExternalId = entity.BillExternalId;
            entity_db.PaymentId = entity.PaymentId;
            entity_db.FinalConsumer = entity.FinalConsumer;
            entity_db.TaxedAmount = entity.TaxedAmount;
            entity_db.Discount = entity.Discount;
            entity_db.DiscountAmount = entity.DiscountAmount;
            entity_db.GatewayTransactionId = entity.GatewayTransactionId;

            Repository.Edit(entity_db);
            Repository.Save();
            Repository.ContextTrackChanges = false;
        }

        public double GetAmount(ICollection<Guid> billsId)
        {
            return Repository.AllNoTracking(b => billsId.Contains(b.Id)).Sum(b => b.Amount);
        }

        public BillDto ChekBills(string lines, int idPadron, int depto, GatewayEnumDto gateway, string param)
        {
            var gate = _serviceService.GetGateway(gateway);
            if (gateway == GatewayEnumDto.Sucive)
            {
                var preBill = _suciveAccess.CheckIfBillPayable(lines, idPadron, depto, param);
                return ToDto(preBill, gate.Id);
            }
            if (gateway == GatewayEnumDto.Geocom)
            {
                var preBill = _geocomAccess.CheckIfBillPayable(lines, idPadron, depto, param);
                return ToDto(preBill, gate.Id);
            }
            return null;
        }

        private ICollection<BillDto> RemoveBillsAlreadyInVisaNet(ICollection<BillDto> billsDtos, Guid serviceId)
        {
            if (billsDtos == null || !billsDtos.Any())
                return billsDtos;

            var ids = billsDtos.Select(dto => dto.BillExternalId.ToUpper()).ToList();
            var billsAlreadyIn = CheckBillsIds(ids);
            var bills = billsDtos.Where(dto => !(billsAlreadyIn.Any(x => x.BillExternalId.Equals(dto.BillExternalId, StringComparison.InvariantCultureIgnoreCase) &&
                x.Payment.ServiceId == serviceId))).OrderBy(b => b.ExpirationDate).ToList();

            return bills;
        }

        private ICollection<HighwayBillDto> RemoveDuplicatedBills(ICollection<HighwayBillDto> billsDtos)
        {
            //saca las facturas repetidas de la lista, dejando las creadas mas recientemente
            var bills = billsDtos.OrderBy(t => t.NroFactura).ThenByDescending(t => t.CreationDate).ToList();
            var result = new List<HighwayBillDto>();

            while (bills.Any())
            {
                result.Add(bills.FirstOrDefault());

                var billId = bills.FirstOrDefault().NroFactura;
                bills.RemoveAll(x => x.NroFactura == billId);
            }
            return result.OrderBy(b => b.NroFactura).ToList();
        }

        private ICollection<HighwayBillDto> RemoveHighwayBillsAlreadyInVisaNet(ICollection<HighwayBillDto> billsDtos, Guid serviceId)
        {
            if (billsDtos == null || !billsDtos.Any())
                return billsDtos;

            var ids = billsDtos.Select(dto => dto.NroFactura.ToUpper()).ToList();
            var billsAlreadyIn = CheckBillsIds(ids);

            var bills = billsDtos.Where(dto => !(billsAlreadyIn.Any(x => x.BillExternalId.Equals(dto.NroFactura, StringComparison.InvariantCultureIgnoreCase) &&
                x.Payment.ServiceId == serviceId))).OrderBy(b => b.FchVencimiento).ToList();

            return RemoveDuplicatedBills(bills);
        }

        private IEnumerable<BillDto> CheckBillsIds(List<String> ids)
        {
            var query = Repository.AllNoTracking(b => ids.Contains(b.BillExternalId), x => x.Payment).Select(b => new BillDto()
            {
                BillExternalId = b.BillExternalId.ToUpper(),
                Payment = new PaymentDto()
                {
                    ServiceId = b.Payment.ServiceId
                }
            }).ToList();
            return query;
        }

        public List<BillDto> GetBillsIdPadron(int idPadron, int depto, GatewayEnumDto gateway, string param)
        {
            var gate = _serviceService.GetGateway(gateway);
            if (gateway == GatewayEnumDto.Sucive)
            {
                return _suciveAccess.GetBillsIdPadron(idPadron, depto, param).Select(x => ToDto(x, gate.Id)).ToList();
            }
            if (gateway == GatewayEnumDto.Geocom)
            {
                return _geocomAccess.GetBillsIdPadron(idPadron, depto, param).Select(x => ToDto(x, gate.Id)).ToList();
            }
            return null;
        }

        public BillDto ChekGeocomBills(string lines, int idPadron, int depto, string param)
        {
            //devuelve una pre factura, con el codigo que debo enviar para realizar un pago
            var preBill = _geocomAccess.CheckIfBillPayable(lines, idPadron, depto, param);
            return new BillDto()
            {
                Amount = preBill.Amount,
                Currency = preBill.Currency,
                Description = preBill.Description,
                BillExternalId = preBill.Codigo,
                Gateway = GatewayEnumDto.Sucive,
                ExpirationDate = DateTime.Today,
                SucivePreBillNumber = preBill.GeocomPreBillNumber
            };
        }
        public List<BillGeocomDto> GetGeocomBillsIdPadron(int idPadron, int depto, string param)
        {
            return _geocomAccess.GetBillsIdPadron(idPadron, depto, param);
        }

        public IQueryable<HighwayBill> StatusBIlls(WebServiceBillsStatusInputDto dto)
        {
            return _serviceHighway.StatusBIlls(dto);
        }

        private string CheckIfHasIdPadron(Guid serviceId, Guid registeredUserId, string[] references)
        {
            var idPadron = string.Empty;
            var serviceAssociated = _serviceAssosiate.ServiceAssosiatedToUser(registeredUserId, serviceId, references[0], references[1], references[2],
                references[3], references[4], references[5]);
            if (serviceAssociated != null && !string.IsNullOrEmpty(serviceAssociated.ReferenceNumber6))
            {
                idPadron = serviceAssociated.ReferenceNumber6;
            }
            return idPadron;
        }

        public BillDto GeneratePreBill(GeneratePreBillDto generatePreBillDto)
        {
            var service = _serviceService.AllNoTracking(null, x => x.Id == generatePreBillDto.ServiceId, x => x.ServiceGateways, x => x.ServiceGateways.Select(g => g.Gateway)).FirstOrDefault();
            var lines = generatePreBillDto.SelectedBills.Select(x => x.Line).Aggregate((i, j) => i + "" + j);
            var gateway = generatePreBillDto.SelectedBills.First().Gateway;
            var idPadron = generatePreBillDto.SelectedBills.First().IdPadron;
            var serGateway = service.ServiceGatewaysDto.FirstOrDefault(x => x.Gateway.Enum == (int)gateway);

            if (serGateway == null)
            {
                var gatewayId = generatePreBillDto.SelectedBills.First(x => x.GatewayId != Guid.Empty).GatewayId;
                serGateway = service.ServiceGatewaysDto.FirstOrDefault(x => x.GatewayId == gatewayId);
            }

            if (serGateway.Gateway.Enum == (int)GatewayEnumDto.Sucive)
            {
                var preBill = _suciveAccess.CheckIfBillPayable(lines, idPadron, (int)service.Departament, serGateway.ReferenceId);
                preBill.IdPadron = idPadron;
                return ToDto(preBill, serGateway.GatewayId);
            }
            if (serGateway.Gateway.Enum == (int)GatewayEnumDto.Geocom)
            {
                var preBill = _geocomAccess.CheckIfBillPayable(lines, idPadron, (int)service.Departament, serGateway.ReferenceId);
                preBill.IdPadron = idPadron;
                return ToDto(preBill, serGateway.GatewayId);
            }
            return null;
        }

        public AnonymousUserBillDto GetInputAmountBillForAnonymousUser(AnonymousUserInputAmountBillFilterDto filter)
        {
            var billdto = GetInputAmountBill(filter.Amount, filter.Currency);
            AnonymousUserDto user = null;
            if (filter == null || filter.AnonymousUserDto == null)
            {
                throw new BusinessException(CodeExceptions.ANONYMOUS_USER_MISSING);
            }
            var userDb = _anonymousUser.GetUserByEmailIdentityNumber(filter.AnonymousUserDto.Email, string.Empty);
            if (userDb != null)
            {
                user = userDb;
            }
            else
            {
                user = _anonymousUser.CreateOrEditAnonymousUser(filter.AnonymousUserDto);
            }

            return new AnonymousUserBillDto()
            {
                Bills = new List<BillDto>() { billdto },
                User = user
            };
        }

        public ApplicationUserBillDto GetInputAmountBillForRegisteredUser(RegisteredUserInputAmountBillFilterDto filter)
        {
            var billdto = GetInputAmountBill(filter.Amount, filter.Currency);
            ApplicationUserDto user = null;
            if (filter != null && filter.UserId.HasValue)
            {
                user = _appUser.GetById(filter.UserId.Value);
            }
            return new ApplicationUserBillDto()
            {
                Bills = new List<BillDto>() { billdto },
                User = user
            };
        }

        public BillDto GetInputAmountBill(double amount, CurrencyDto currency)
        {
            var gate = _serviceService.GetGateway(GatewayEnumDto.Importe);
            return new BillDto()
            {
                Amount = amount,
                Currency = currency.ToString(),
                Payable = true,
                BillExternalId = "123456",
                Discount = 0,
                Gateway = GatewayEnumDto.Importe,
                FinalConsumer = false,
                GatewayId = gate.Id
            };
        }

        private void IsServiceMomentarilyDisabled(string serviceIdApp, string containerIdApp, string name)
        {
            try
            {
                var fileName = ConfigurationManager.AppSettings["ServiceIdMomentarilyDisabled"];

                using (var r = new StreamReader(fileName))
                {
                    string json = r.ReadToEnd();
                    var services = JsonConvert.DeserializeObject<IEnumerable<ServiceOfflineDto>>(json);
                    var today = DateTime.Now;

                    var servicesOffline =
                        services.FirstOrDefault(
                            x =>
                                today >= x.DateFrom &&
                                today <= x.DateTo &&
                                (
                                    x.IdApps.Any(y => y.Equals(serviceIdApp)) ||
                                    x.IdApps.Any(y => y.Equals(containerIdApp))
                                )
                            );

                    if (servicesOffline != null)
                        throw new BillException(string.Format(ExceptionMessages.SERVICE_MOMENTARILY_DISABLED, name, servicesOffline.DateTo.ToString("dd/MM/yyyy hh:mm")));
                }
            }
            catch (BillException e)
            {
                NLogLogger.LogEvent(e, OperationType.GetBills, Repository.GetDataLog());
                throw e;
            }
            catch (Exception e)
            {
                NLogLogger.LogEvent(e, OperationType.GetBills, Repository.GetDataLog());
            }
        }

        public bool IsBillExlternalIdRepitedByServiceId(string billExternalId, Guid serviceId)
        {
            return _repositoryBill.IsBillExlternalIdRepitedByServiceId(billExternalId, serviceId);
        }

        public bool IsBillExlternalIdRepitedByMerchantId(string billExternalId, string merchantId)
        {
            return _repositoryBill.IsBillExlternalIdRepitedByMerchantId(billExternalId, merchantId);
        }

        public AnonymousUserBillDto GenerateAnnualPatenteForAnonymousUser(AnonymousUserBillFilterDto filter)
        {
            AnonymousUserDto user = null;
            if (filter == null || filter.AnonymousUserDto == null)
            {
                throw new BusinessException(CodeExceptions.ANONYMOUS_USER_MISSING);
            }
            user = _anonymousUser.CreateOrEditAnonymousUser(filter.AnonymousUserDto);
            var bill = GenerateAnnualPatente(filter);
            return new AnonymousUserBillDto()
            {
                Bills = bill != null ? new List<BillDto>() { bill } : null,
                User = user
            };
        }

        public ApplicationUserBillDto GenerateAnnualPatenteForRegisteredUser(RegisteredUserBillFilterDto filter)
        {
            ApplicationUserDto user = null;
            if (filter != null && filter.UserId.HasValue)
            {
                user = _appUser.GetById(filter.UserId.Value);
            }
            var bill = GenerateAnnualPatente(filter);
            return new ApplicationUserBillDto()
            {
                Bills = bill != null ? new List<BillDto>() { bill } : null,
                User = user
            };
        }

        private BillDto GenerateAnnualPatente(IBillFilterDto filter)
        {
            var service = _serviceService.All(null, x => x.Id == filter.ServiceId, x => x.ServiceGateways, x => x.ServiceGateways.Select(g => g.Gateway)).First();
            var bestServiceGateway = service.ServiceGatewaysDto.FirstOrDefault(x => x.Active);

            var refs = service.LoadReferences(filter.References);

            var registeredUserBillFilter = filter as RegisteredUserBillFilterDto; //si es registrado, puede q tenga id padron

            if (registeredUserBillFilter != null)
            {
                if (registeredUserBillFilter.UserId.HasValue)
                {
                    var idPadron = CheckIfHasIdPadron(service.Id, registeredUserBillFilter.UserId.Value, refs);
                    refs[5] = idPadron;
                }
            }

            var bill = _suciveAccess.GenerateAnnualPatente(refs, bestServiceGateway.ReferenceId, bestServiceGateway.ServiceType, (int)service.Departament);

            return bill != null ? ToDto(bill, bestServiceGateway.GatewayId) : null;
        }
    }
}