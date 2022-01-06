using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using CyberSource;
using VisaNet.Application.Interfaces;
using VisaNet.Common.DependencyInjection;
using VisaNet.Common.Exceptions;
using VisaNet.Common.Logging.Entities;
using VisaNet.Common.Logging.NLog;
using VisaNet.Common.Logging.Services;
using VisaNet.Domain.Entities.Enums;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Repository.Interfaces.Interfaces;
using VisaNet.Utilities.Cybersource;
using VisaNet.Utilities.ExtensionMethods;

namespace VisaNet.ConsoleApplication.PaymentProcess.PaymentsProcessor
{
    public class DebitProcessorHandler
    {
        private static readonly IServiceHighway _serviceHighway = NinjectRegister.Get<IServiceHighway>();
        private static readonly IServiceBill _serviceBill = NinjectRegister.Get<IServiceBill>();
        private static readonly IServiceServiceAssosiate _serviceServiceAssosiate = NinjectRegister.Get<IServiceServiceAssosiate>();
        private static readonly IServiceDiscountCalculator _serviceDiscountCalculator = NinjectRegister.Get<IServiceDiscountCalculator>();
        private static readonly IServicePayment _servicePayment = NinjectRegister.Get<IServicePayment>();
        private static readonly IServiceMailgun _serviceMailgun = NinjectRegister.Get<IServiceMailgun>();

        int billscount = 0;
        int paymentsDone = 0;
        int paymentCancelled = 0;

        public void Execute()
        {
            try
            {
                NLogLogger.LogEvent(NLogType.Info, "INICIA PROCESO PAGO DEBITO");
                NLogLogger.LogEvent(NLogType.Info, "FECHA: " + DateTime.Now.ToString("G"));
                NLogLogger.LogEvent(NLogType.Info, "Proceso Pago débito -- Obtengo lista de facturas");

                var bills = _serviceHighway.All(null, x => x.PagoDebito, x => x.Service).Where(x => x.Type == HighwayBillTypeDto.Pending).OrderBy(x => x.ServiceDto.Name).ToList();
                billscount = bills.Count();
                NLogLogger.LogEvent(NLogType.Info, "Proceso Pago débito -- Cantidad de facturas: " + billscount);

                if (bills.Any())
                {
                    foreach (var billDto in bills)
                    {
                        var msg = string.Format("   Proceso Pago débito -- Inicio proceso de pago para servicio: {0} (ref: {1}), Nro factura {2}",
                                billDto.ServiceDto != null ? billDto.ServiceDto.Name : "NULL",
                                billDto.RefCliente,
                                billDto.NroFactura);
                        NLogLogger.LogEvent(NLogType.Info, msg);
                        try
                        {
                            var service = _serviceServiceAssosiate.GetServicesDebit(billDto.RefCliente, billDto.ServiceId);

                            if (!service.Any())
                            {
                                //no hay servicio asociado
                                var error = string.Format("     Proceso Pago débito -- No hay servicio asociado para Nro factura {0}, Nro referencia {1}",
                                billDto.NroFactura,
                                billDto.RefCliente);
                                NLogLogger.LogEvent(NLogType.Error, error);
                                _serviceHighway.ChangeType(billDto.Id, HighwayBillType.Cancelled, 50, "MATRICULA SIN SERVICIO ASOCIADO.");
                                continue;
                            }
                            if (service.Count() > 1)
                            {
                                //NOTIFICO QUE HAY MAS DE DOS SERVICIOS CON MISMO NRO DE REFERENCIA
                                var error = string.Format("     Proceso Pago débito -- Hay mas de un servicio asociado para Nro factura {0}, Nro referencia {1}",
                                billDto.NroFactura,
                                billDto.RefCliente);
                                NLogLogger.LogEvent(NLogType.Info, error);
                            }
                            var associatedDto = service.FirstOrDefault();

                            #region Validaciones tarjeta default

                            if (associatedDto != null)
                            {
                                //no existe tarjeta
                                if (associatedDto.DefaultCard == null)
                                {
                                    var logerMsg = string.Format(
                                            "     El servicio {0} (ref: {1}) no tiene seleccionada una tarjeta para realizar el pago.",
                                            associatedDto.ServiceDto.Name,
                                            associatedDto.ReferenceNumber);
                                    NLogLogger.LogEvent(NLogType.Error, logerMsg);
                                    _serviceHighway.ChangeType(billDto.Id, HighwayBillType.Cancelled);
                                    _serviceHighway.ChangeType(billDto.Id, HighwayBillType.Cancelled, 51, "SERVICIO SIN TARJETA PARA PAGAR.");

                                    continue;
                                }
                                //no existe token
                                if (associatedDto.DefaultCard != null && string.IsNullOrEmpty(associatedDto.DefaultCard.PaymentToken))
                                {
                                    var logerMsg = string.Format(
                                            "     El token de la tarjeta se encuentra vacío en el sistema. Servicio: {0} (ref: {1}). Tarjeta: {2}",
                                            associatedDto.ServiceDto.Name,
                                            associatedDto.ReferenceNumber,
                                            associatedDto.DefaultCard.MaskedNumber);
                                    NLogLogger.LogEvent(NLogType.Error, logerMsg);
                                    _serviceHighway.ChangeType(billDto.Id, HighwayBillType.Cancelled);
                                    _serviceHighway.ChangeType(billDto.Id, HighwayBillType.Cancelled, 52, "SERVICIO SIN TOKEN PARA PAGAR.");
                                    continue;
                                }

                                //tarjeta vencida
                                if (associatedDto.DefaultCard != null && associatedDto.DefaultCard.DueDate < new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1))
                                {
                                    var logerMsg = string.Format("     La tarjeta de crédito {0} se encuentra vencida desde el {1}.",
                                        associatedDto.DefaultCard.MaskedNumber,
                                        associatedDto.DefaultCard.DueDate.ToString("MM/yyyy"));
                                    NLogLogger.LogEvent(NLogType.Error, logerMsg);
                                    _serviceHighway.ChangeType(billDto.Id, HighwayBillType.Cancelled, 53, "SERVICIO CON TARJETA VENCIDA.");
                                    continue;
                                }
                            }

                            #endregion

                            //Calculo y registro los descuentos aplicados, en el caso de tarjetas de debito
                            NLogLogger.LogEvent(NLogType.Info, string.Format("     Proceso Pago débito -- Calculando descuento Nro factura {0}", billDto.NroFactura));
                            var binNumber = Int32.Parse(associatedDto.DefaultCard.MaskedNumber.Substring(0, 6));

                            //TODO: verificar si se debe pasar otra fecha (la fecha de vencimiento de la factura mas proxima a vencer)
                            var payBills = _serviceBill.ToDto(billDto, Guid.Empty, DateTime.MinValue, false);

                            #region Calculate Discount

                            var currency = billDto.Moneda;
                            var discountQuery = new DiscountQueryDto
                            {
                                Bills = new List<BillDto>() { payBills },
                                BinNumber = binNumber,
                                ServiceId = associatedDto.ServiceDto.Id
                            };

                            //obtengo los valores con los descuentos correspondientes
                            var cyberSourceExtraDataList = _serviceDiscountCalculator.Calculate(discountQuery);
                            var cyberSourceExtraData = cyberSourceExtraDataList.FirstOrDefault();

                            var payment = new PaymentDto
                            {
                                Bills = new List<BillDto>() { cyberSourceExtraData.BillDto },
                                CardId = associatedDto.DefaultCardId,
                                Card = associatedDto.DefaultCard,
                                Date = DateTime.Now,
                                ReferenceNumber = associatedDto.ReferenceNumber,
                                ReferenceNumber2 = associatedDto.ReferenceNumber2,
                                ReferenceNumber3 = associatedDto.ReferenceNumber3,
                                ReferenceNumber4 = associatedDto.ReferenceNumber4,
                                ReferenceNumber5 = associatedDto.ReferenceNumber5,
                                ReferenceNumber6 = associatedDto.ReferenceNumber6,
                                ServiceAssociatedId = associatedDto.Id,
                                ServiceAssociatedDto = associatedDto,
                                ServiceDto = associatedDto.ServiceDto,
                                ServiceId = associatedDto.ServiceId,
                                RegisteredUserId = associatedDto.UserId,
                                PaymentType = PaymentTypeDto.Debit,
                                Currency = currency,
                                Discount = cyberSourceExtraData.BillDto.Discount,
                                DiscountApplyed = cyberSourceExtraData.BillDto.DiscountAmount > 0,
                                TotalAmount = cyberSourceExtraData.BillDto.Amount,
                                TotalTaxedAmount = cyberSourceExtraData.BillDto.TaxedAmount,
                                AmountTocybersource = cyberSourceExtraData.CybersourceAmount,
                                PaymentPlatform = PaymentPlatformDto.VisaNet,
                                DiscountObjId = cyberSourceExtraData.DiscountDto != null ? cyberSourceExtraData.DiscountDto.Id : Guid.Empty,
                                DiscountObj = cyberSourceExtraData.DiscountDto,
                            };

                            NLogLogger.LogEvent(NLogType.Info,
                                "         Proceso Pago débito -- Llamo a metodo para pagar por medio del WCF Cybersource.");
                            #endregion

                            if (payment.AmountTocybersource > 0)
                            {
                                #region Cybersource Payment
                                var paymentData = PayWithCybersource(payment, associatedDto, cyberSourceExtraData);

                                payment.TransactionNumber = paymentData != null ? paymentData.TransactionId : null;
                                payment.CyberSourceData = paymentData;

                                #endregion

                                if (paymentData != null)
                                {
                                    if (paymentData.ReasonCode.Equals("100"))
                                    {
                                        NLogLogger.LogEvent(NLogType.Info, string.Format("         Proceso Pago débito -- Se registro el pago en cybersource, REQUIEST ID : {0}", paymentData.TransactionId));
                                        #region Notify Gateway
                                        NLogLogger.LogEvent(NLogType.Info, "         Proceso Pago débito -- Notifico a los entes");
                                        var paymentDto = _servicePayment.NotifyGateways(payment);
                                        #endregion
                                        if (paymentDto.NewPaymentDto != null)
                                        {
                                            paymentsDone++;
                                            //modifico la factura a paga
                                            _serviceHighway.ChangeType(billDto.Id, HighwayBillType.Paid);
                                            var message = string.Format(
                                                        "El págo débito para el servicio {0} {1} factura {2} se ejecuto correctamente en la fecha {3}.",
                                                        associatedDto.ServiceDto.Name,
                                                        associatedDto.Description,
                                                        billDto.NroFactura,
                                                        DateTime.Now.ToString("dd/MM/yyyy"));
                                            NLogLogger.LogEvent(NLogType.Error, message);

                                            //Se notifica al ente externo
                                            _servicePayment.NotifyExternalSourceNewPayment(paymentDto.NewPaymentDto);
                                        }
                                        else
                                        {
                                            #region Error la intentar notificar a las pasarelas
                                            paymentCancelled++;
                                            NLogLogger.LogEvent(NLogType.Info, "          Proceso Pago débito -- paymentDto null");
                                            var message = string.Format(
                                                        "El págo débito para el servicio {0} {1} no pudo ser ejecutado correctamente en la fecha {2}. Error en notificación al comercio.",
                                                        associatedDto.ServiceDto.Name,
                                                        associatedDto.Description,
                                                        DateTime.Now.ToString("dd/MM/yyyy"));
                                            NLogLogger.LogEvent(NLogType.Error, message);
                                            _serviceHighway.ChangeType(billDto.Id, HighwayBillType.Cancelled, 54, "ERROR AL INENTAR GUARDAR LA TRANSACCIÓN.");
                                            #endregion
                                        }
                                    }
                                    else
                                    {
                                        NLogLogger.LogEvent(NLogType.Info, "      Proceso Pago débito -- No se pudo realizar el pago contra Cybresource. Codigo de error: " +
                                            paymentData.ReasonCode);
                                        paymentCancelled++;
                                        var message = string.Format(
                                                        "El págo débito para el servicio {0} {1} no pudo ser ejecutado correctamente en la fecha {2}. Rechazado en cybersource.",
                                                        associatedDto.ServiceDto.Name,
                                                        associatedDto.Description,
                                                        DateTime.Now.ToString("dd/MM/yyyy"));
                                        NLogLogger.LogEvent(NLogType.Error, message);
                                        _serviceHighway.ChangeType(billDto.Id, HighwayBillType.Cancelled, int.Parse(paymentData.ReasonCode), "ERROR AL INENTAR GUARDAR LA TRANSACCIÓN.");
                                    }
                                }
                                else
                                {
                                    NLogLogger.LogEvent(NLogType.Info, "      Proceso Pago débito -- No se pudo realizar el pago contra Cybresource");
                                    #region Error la intentar pagar en CS
                                    paymentCancelled++;
                                    var message = string.Format(
                                                    "El págo débito para el servicio {0} {1} no pudo ser ejecutado correctamente en la fecha {2}. Error en cybersource.",
                                                    associatedDto.ServiceDto.Name,
                                                    associatedDto.Description,
                                                    DateTime.Now.ToString("dd/MM/yyyy"));
                                    NLogLogger.LogEvent(NLogType.Error, message);
                                    _serviceHighway.ChangeType(billDto.Id, HighwayBillType.Cancelled, 54, "ERROR AL INENTAR GUARDAR LA TRANSACCIÓN.");

                                    #endregion
                                }
                            }
                            else
                            {
                                NLogLogger.LogEvent(NLogType.Info, "   Proceso Pago débito -- No se realizo pago contra Cybresource por no tener monto mayor a 0");
                                #region Error la intentar pagar. Monto de factura no mayor a 0
                                paymentCancelled++;
                                var message = string.Format(
                                            "El págo débito para el servicio {0} {1} no pudo ser ejecutado correctamente en la fecha {2}.",
                                            associatedDto.ServiceDto.Name,
                                            associatedDto.Description,
                                            DateTime.Now.ToString("dd/MM/yyyy"));
                                NLogLogger.LogEvent(NLogType.Error, message);
                                _serviceHighway.ChangeType(billDto.Id, HighwayBillType.Cancelled, 55, "ERROR AL INENTAR GENERAR LA TRANSACCIÓN");
                                #endregion
                            }

                        }
                        catch (Exception exception)
                        {
                            NLogLogger.LogEvent(NLogType.Error, "   Proceso Pago débito -- EXCEPTION");
                            NLogLogger.LogEvent(exception);
                            _serviceHighway.ChangeType(billDto.Id, HighwayBillType.Cancelled, 55, "ERROR AL INENTAR GENERAR LA TRANSACCIÓN");
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                NLogLogger.LogEvent(NLogType.Error, "   Proceso Pago débito -- EXCEPTION");
                NLogLogger.LogEvent(exception);
            }
            finally
            {
                #region Finally

                var txt = "<html> Se realizo el proceso de pago de debito. <br> ";
                txt = txt + "Total de facturas habilitadas para pagar: " + billscount + "<br>";
                txt = txt + "Cantidad de facturas pagas: " + paymentsDone + "<br>";
                //txt = txt + "Cantidad de facturas no pagas: " + paymentCancelled + "<br>";
                txt = txt + "</html>";

                NLogLogger.LogEvent(NLogType.Info, "FIN PROCESO PAGO DEBITO ");
                NLogLogger.LogEvent(NLogType.Info, "FECHA: " + DateTime.Now.ToString("G"));
                #endregion
            }
        }

        private CyberSourceDataDto PayWithCybersource(PaymentDto paymentDto, ServiceAssociatedDto serviceAssociated, CyberSourceExtraDataDto cyberSourceExtraData)
        {
            var tryCounter = 5;
            var exit = false;
            CyberSourceDataDto data = null;
            var transactionId = Guid.NewGuid();
            while (tryCounter > 0 && exit == false)
            {
                try
                {
                    var payment = new GeneratePayment
                    {
                        ApplicationUserId = serviceAssociated.UserId,
                        Currency = paymentDto.Currency,
                        MerchandId = serviceAssociated.ServiceDto.MerchantId,
                        Token = serviceAssociated.DefaultCard.PaymentToken,
                        TransaccionId = transactionId.ToString(),
                        GrandTotalAmount = paymentDto.AmountTocybersource.SignificantDigits(2).ToString(CultureInfo.CreateSpecificCulture("en-US")),
                        Key = serviceAssociated.ServiceDto.CybersourceTransactionKey,
                        UserType = LogUserType.Other
                    };

                    var merchantDefinedData = LoadMerchantDefinedData(serviceAssociated, cyberSourceExtraData);

                    if (!String.IsNullOrEmpty(payment.Token))
                    {
                        //REALIZO EL PAGO EN CYBERSOURCE. ME DEVUELVE EL REQUESTID. SI ES NULLO, SE PRODUJO UN ERROR
                        var access = new CyberSourceAccess(NinjectRegister.Get<ILoggerService>(), NinjectRegister.Get<IServiceService>(),
                            NinjectRegister.Get<IServiceFixedNotification>(), NinjectRegister.Get<IServiceApplicationUser>(),
                            NinjectRegister.Get<IServiceAnonymousUser>(), NinjectRegister.Get<IServiceParameters>(),
                            NinjectRegister.Get<IRepositoryPayment>(), NinjectRegister.Get<IRepositoryCard>(), NinjectRegister.Get<IRepositoryConciliationCybersource>(),
                            NinjectRegister.Get<IServiceBank>(), NinjectRegister.Get<IServiceVonData>(), NinjectRegister.Get<IServiceBin>());
                        data = access.GeneratePayment(payment, merchantDefinedData);
                        exit = true;
                    }
                    tryCounter--;
                }
                catch (CybersourceException e)
                {
                    tryCounter--;
                }
            }
            return data;
        }

        private CyberSourceMerchantDefinedDataDto LoadMerchantDefinedData(ServiceAssociatedDto serviceAssociated, CyberSourceExtraDataDto cyberSourceExtraData)
        {
            var paymentsCount = _servicePayment.CountPaymentsDone(serviceAssociated.UserId, Guid.Empty, serviceAssociated.ServiceId);

            var merchantDefinedData = new CyberSourceMerchantDefinedDataDto
            {
                ServiceType = serviceAssociated.ServiceDto.ServiceCategory.Name,
                OperationTypeDto = OperationTypeDto.UniquePayment.ToString(),
                UserRegistered = "Y",
                UserRegisteredDays = (DateTime.Now.Date - serviceAssociated.RegisteredUserDto.CreationDate.Date).Days.ToString(),
                ReferenceNumber1 = serviceAssociated.ReferenceNumber,
                ReferenceNumber2 = serviceAssociated.ReferenceNumber2,
                ReferenceNumber3 = serviceAssociated.ReferenceNumber3,
                ReferenceNumber4 = serviceAssociated.ReferenceNumber4,
                ReferenceNumber5 = serviceAssociated.ReferenceNumber5,
                ReferenceNumber6 = serviceAssociated.ReferenceNumber6,
                RedirctTo = 0.ToString(),
                DiscountApplyed = cyberSourceExtraData.BillDto.DiscountAmount > 0 ? ((int)cyberSourceExtraData.DiscountDto.DiscountLabel).ToString() : "0",
                TotalAmount = cyberSourceExtraData.BillDto.Amount.SignificantDigits(2).ToString(CultureInfo.CreateSpecificCulture("en-US")),
                TotalTaxedAmount = cyberSourceExtraData.BillDto.TaxedAmount.SignificantDigits(2).ToString(CultureInfo.CreateSpecificCulture("en-US")),
                Discount = cyberSourceExtraData.BillDto.DiscountAmount.SignificantDigits(2).ToString(CultureInfo.CreateSpecificCulture("en-US")),
                BillNumber = cyberSourceExtraData.BillDto.BillExternalId,
                AditionalNumberElectornicBill = "",
                NameTh = serviceAssociated.DefaultCard.Name,
                PaymentCount = paymentsCount,
                UserCi = serviceAssociated.RegisteredUserDto.IdentityNumber,
                UserMobile = serviceAssociated.RegisteredUserDto.MobileNumber,
                UserRegisteredAddress = serviceAssociated.RegisteredUserDto.Address,

                MerchandId = serviceAssociated.ServiceDto.MerchantId,
                CardBin = cyberSourceExtraData.BinNumber.ToString(),
                ServiceName = serviceAssociated.ServiceDto.Name,
                CallcenterUser = cyberSourceExtraData.CallcenterUser,
                Plataform = PaymentPlatformDto.VisaNet.ToString(),
                PaymentTypeDto = PaymentTypeDto.Debit

            };
            return merchantDefinedData;
        }

    }
}