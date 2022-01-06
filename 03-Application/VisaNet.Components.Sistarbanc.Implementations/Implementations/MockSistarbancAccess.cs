using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using VisaNet.Application.Interfaces;
using VisaNet.Common.Exceptions;
using VisaNet.Common.Logging.Entities;
using VisaNet.Common.Logging.Resources;
using VisaNet.Common.Logging.Services;

using VisaNet.Components.Sistarbanc.Interfaces;
using VisaNet.Domain.EntitiesDtos;

namespace VisaNet.Components.Sistarbanc.Implementations.Implementations
{
    public class MockSistarbancAccess : ISistarbancAccess
    {

        private readonly IServiceServiceAssosiate _serviceAssosiate;
        private readonly ILoggerService _loggerService;

        public MockSistarbancAccess(IServiceServiceAssosiate serviceAssosiate, ILoggerService loggerService)
        {
            _serviceAssosiate = serviceAssosiate;
            _loggerService = loggerService;
        }

        /// <summary>
        /// Da de alta un usuario. Se debe ejecutar si es la primera ves que registra un pago
        /// </summary>
        /// <param name="idBanco">Identificador de visanet ante sistarbanc</param>
        /// <param name="idClienteBanco">Identidicador del cliente ante visanet</param>
        /// <param name="name">Nombre cliente</param>
        /// <param name="surname">Apellido cliente</param>
        /// <param name="transactionNumber">Numero de transaccion</param>
        public void AltaUsuario(string idBanco, string idClienteBanco, string name, string surname, string transactionNumber)
        {
            _loggerService.CreateLog(LogType.Info, LogOperationType.BillPayment, LogCommunicationType.Sistarbanc, string.Format(LogStrings.Payment_NotifySistarbanc_NewUser, name, surname, transactionNumber));

            //var sistarbancService = new WsAltasBajasImpClient();

            //var paramsArray = new[]
            //{
            //    idBanco,
            //    idClienteBanco,
            //    name,
            //    surname,
            //};


            //string signature = SistarbancDigitalSignature.GenerateSignature(paramsArray);
            //string firma = string.Empty;

            //ServicePointManager.MaxServicePointIdleTime = 5000;
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
            //System.Net.ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) => true);

            //var result = sistarbancService.altaCliente(idBanco, idClienteBanco, name, surname, signature);

            //if (result == null)
            //{
            //    _loggerService.CreateLog(LogType.Error, LogOperationType.BillPayment, LogCommunicationType.Sistarbanc, string.Format(LogStrings.Payment_NotifySistarbanc_ErrorNewUser,
            //        CodeExceptions.SISTARBANC_NORESPONSE, transactionNumber, name, surname));
            //    throw new ProviderFatalException(CodeExceptions.SISTARBANC_NORESPONSE);
            //}
            //if (result.codigoError == 0)
            //{
            //    return;
            //}
            //if (result.codigoError == 15)
            //{
            //    //EL USUARIO YA ESTA REGISTADO.
            //    _loggerService.CreateLog(LogType.Alert, LogOperationType.BillPayment, LogCommunicationType.Sistarbanc, string.Format(LogStrings.Payment_NotifySistarbanc_UserAlreadyCreated,
            //        name, surname, transactionNumber));
            //}
            //else
            //{
            //    throw new ProviderFatalException(GetCodeExceptions(result.codigoError, transactionNumber, name, surname));
            //}
        }

        /// <summary>
        /// Da de baja un usuario.
        /// </summary>
        /// <param name="idBanco">Identificador de visanet ante sistarbanc</param>
        /// <param name="idClienteBanco">Identidicador del cliente ante visanet</param>
        /// <param name="transactionNumber">Identidicador transaccion</param>
        public void BajaUsuario(string idBanco, string idClienteBanco, string transactionNumber)
        {
            //var sistarbancService = new WsAltasBajasImpClient();

            //var paramsArray = new[]
            //{
            //    idBanco,
            //    idClienteBanco
            //};

            //string signature = SistarbancDigitalSignature.GenerateSignature(paramsArray);
            //string firma = string.Empty;

            //ServicePointManager.MaxServicePointIdleTime = 5000;
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
            //System.Net.ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) => true);

            //var result = sistarbancService.bajaCliente(idBanco, idClienteBanco, signature);

            //if (result == null)
            //{
            //    _loggerService.CreateLog(LogType.Error, LogOperationType.BillPayment, LogCommunicationType.Sistarbanc, string.Format(LogStrings.Payment_NotifySistarbanc_ErrorNewUser,
            //        CodeExceptions.SISTARBANC_NORESPONSE, transactionNumber, "", ""));
            //    throw new ProviderFatalException(CodeExceptions.SISTARBANC_NORESPONSE);
            //}

            //if (result.codigoError != 0)
            //{
            //    throw new ProviderFatalException(GetCodeExceptions(result.codigoError, transactionNumber, "", ""));
            //}
        }

        public IEnumerable<BillSistarbancDto> GetBills(string idBancoVisa, string idBancoBrou, string idOrganismo, string tipoServicio,
            string[] refServicio)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Devuelve listado de facturas impagas para un servicio dado
        /// </summary>
        /// <param name="idBanco">Identificador de visanet ante sistarbanc</param>
        /// <param name="idOrganismo">Identificador del organismo emisor de las facturas </param>
        /// <param name="tipoServicio">Identificador del tipo del servicio</param>
        /// <param name="refServicio">Array con los parametros de referencia para el servicio</param>
        /// <returns></returns>
        public IEnumerable<BillSistarbancDto> GetBills(string idBanco, string idOrganismo, string tipoServicio, string[] refServicio)
        {
            var refServicioToSend = refServicio.Where(i => i != null).ToArray();

            var paramsList = new List<String> { idBanco, idOrganismo, tipoServicio };
            paramsList.AddRange(refServicioToSend);

            //se llama para verificar que este funcionando la firma
            var signature = SistarbancDigitalSignature.GenerateSignature(paramsList.ToArray());
            
            return new List<BillSistarbancDto>
            {
                new BillSistarbancDto
                {
                    Amount = (2300).ToString(),
                    Currency = "UYU",
                    ExpirationDate = new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.AddDays(5).Day),
                    Description = new []{"Descripción factura pendiente"},
                    BillExternalId = Guid.NewGuid().ToString(),
                    DateInit = DateTime.Now.ToShortDateString(),
                    FinalConsumer = true,
                    Id = Guid.NewGuid(),
                    IdTransaccionSTB = (new Random().Next(100000000,999999999)).ToString(),
                    Payable = true,
                    Precedence = 1,
                }
            };
        }

        public BillDto PagoRecibo(string idBanco, string idOrganismo, string tipoServicio, string[] refServicio, BillDto bill,
            string idClienteBanco, string nroTrasnferenciaVisa, AutomaticPaymentDto automaticPaymentDto)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Marca como pagado las facturas enviadas en billsIds con sistarbanc
        /// </summary>
        /// <param name="idBanco">Identificador de visanet ante sistarbanc</param>
        /// <param name="idOrganismo">Identificador del organismo emisor de las facturas </param>
        /// <param name="tipoServicio">Identificador del tipo del servicio</param>
        /// <param name="refServicio">Array con los parametros de referencia para el servicio</param>
        /// <param name="billsIds">Array con los ids de las facturas a pagar</param>
        /// <param name="idClienteBanco">Identidicador del clietnte</param>
        /// <param name="nroTrasnferenciaVisa">Nro de transferencia para visanet</param>
        /// <param name="automaticPaymentDto"></param>
        /// <param name="usertype"></param>
        public Collection<BillSistarbancDto> PagoRecibo(string idBanco, string idOrganismo, string tipoServicio, string[] refServicio, string[] billsIds, string idClienteBanco, string nroTrasnferenciaVisa, AutomaticPaymentDto automaticPaymentDto)
        {
            _loggerService.CreateLog(LogType.Info, LogOperationType.BillPayment, LogCommunicationType.Sistarbanc, string.Format(LogStrings.Payment_NotifySistarbanc, nroTrasnferenciaVisa));
            return new Collection<BillSistarbancDto>(new List<BillSistarbancDto>());
        }


        private string GetCodeExceptions(int codError, string transactionNumber, string name, string surename)
        {
            switch (codError)
            {
                case 29:
                    _loggerService.CreateLog(LogType.Error, LogOperationType.BillPayment, LogCommunicationType.Sistarbanc, string.Format(LogStrings.Payment_NotifySistarbanc_Error29,
                        CodeExceptions.SISTARBANC_NROERROR_50, transactionNumber, name, surename));
                    return CodeExceptions.SISTARBANC_NROERROR_50;
                case 50:
                    _loggerService.CreateLog(LogType.Error, LogOperationType.BillPayment, LogCommunicationType.Sistarbanc, string.Format(LogStrings.Payment_NotifySistarbanc_Error50,
                        CodeExceptions.SISTARBANC_NROERROR_50, transactionNumber, name, surename));
                    return CodeExceptions.SISTARBANC_NROERROR_50;
                case 57:
                    _loggerService.CreateLog(LogType.Error, LogOperationType.BillPayment, LogCommunicationType.Sistarbanc, string.Format(LogStrings.Payment_NotifySistarbanc_Error57,
                        CodeExceptions.SISTARBANC_NROERROR_57, transactionNumber, name, surename));
                    return CodeExceptions.SISTARBANC_NROERROR_57;
            }

            _loggerService.CreateLog(LogType.Error, LogOperationType.BillPayment, LogCommunicationType.Sistarbanc, string.Format(LogStrings.Payment_NotifySistarbanc_ErrorGeneral,
                       CodeExceptions.GENERAL_ERROR, transactionNumber, name, surename));
            return CodeExceptions.GENERAL_ERROR;
        }

        
        private IEnumerable<BillSistarbancDto> LoadData(string nroReferencia)
        {
            switch (nroReferencia)
            {
                case "95062533000186": return CaseFacturaImpaga();
                case "09028109000157": return CaseVariasFacturasAPagar();
                case "12003882000418": return FacturasPorArribaDelMaximo();
                case "12018882000268": throw new ProviderWithoutConectionException(CodeExceptions.BANRED_NORESPONSE);
                case "pruebaNotificaciones": return NotificationsBills();
            }
            return new List<BillSistarbancDto>();
        }


        private IList<BillSistarbancDto> NotificationsBills()
        {
            var result = "";
            result = result + "VENCIDA 1,20140328,vencida1,42000,N,1,0,1";
            result = result + "|" + "VENCIDA 2,20140428,vencida2,4000,N,1,0,1";
            result = result + "|" + "POR VENCER A FALTA DE UN DIA,20140605,porVencertieneQueGuardarla,48000,N,1,0,1";
            result = result + "|" + "EMITIDA,20140628,emitida,48000,N,1,0,1";
            //return result;
            return new List<BillSistarbancDto>();
        }

        /// <summary>
        /// Devuelve 1 factura impaga
        ///     BillExternalId: Generado random
        ///     ExpirationDate: fecha actual + 1 día
        ///     DateInit: fecha actual + 1 día
        ///     IdTransaccionSTB: random de 10 dígitos (1000000000, 1999999999)
        ///     Amount: random (1000,9999)
        ///     Currency: "UYU"
        ///     Description: una descripción
        ///     FinalConsumer = true
        ///     Payable = true
        ///     Precedence = 1
        ///     TaxedAmount = amount
        /// </summary>
        /// <returns></returns>
        private IList<BillSistarbancDto> CaseFacturaImpaga()
        {
            var rnd = new Random();

            rnd.Next(10000, 99999);
            rnd.Next(10000, 99999);
            rnd.Next(10000, 99999);

            var nroFactura = rnd.Next(10000, 99999);

            var fecha = DateTime.Now.AddDays(1);
            var amount = rnd.Next(1000, 9999);

            return new List<BillSistarbancDto>
            {
                new BillSistarbancDto
                {
                    BillExternalId = nroFactura.ToString(),
                    ExpirationDate = fecha.AddDays(1),
                    DateInit = fecha.ToString(),
                    IdTransaccionSTB = rnd.Next(1000000000, 1999999999).ToString(),
                    Amount = amount.ToString(),
                    Currency = "UYU",
                    Description = new[] {"una descripción"},
                    FinalConsumer = true,
                    Payable = true,
                    Precedence = 1,
                    TaxedAmount = amount,
                }
            };
        }

        /// <summary>
        /// Devuelve 3 factura impaga
        ///     BillExternalId: Generado random
        ///     ExpirationDate: fecha actual + 1 día
        ///     DateInit: fecha actual + 1 día
        ///     IdTransaccionSTB: random de 10 dígitos (1000000000, 1999999999)
        ///     Amount: random (1000,9999)
        ///     Currency: "UYU"
        ///     Description: una descripción
        ///     FinalConsumer = true
        ///     Payable = true
        ///     Precedence = 1
        ///     TaxedAmount = amount
        /// </summary>
        /// <returns></returns>
        private IList<BillSistarbancDto> CaseVariasFacturasAPagar()
        {
            var rnd = new Random();

            rnd.Next(10000, 99999);
            rnd.Next(10000, 99999);
            rnd.Next(10000, 99999);

            var nroFactura1 = rnd.Next(10000, 99999);
            var nroFactura2 = rnd.Next(10000, 99999);
            var nroFactura3 = rnd.Next(10000, 99999);
            var fecha = DateTime.Now.AddDays(1);
            var amount1 = rnd.Next(1000, 9999);
            var amount2 = rnd.Next(1000, 9999);
            var amount3 = rnd.Next(1000, 9999);

            return new List<BillSistarbancDto>
            {
                new BillSistarbancDto
                {
                    BillExternalId = nroFactura1.ToString(),
                    ExpirationDate = fecha.AddDays(1),
                    DateInit = fecha.ToString(),
                    IdTransaccionSTB = rnd.Next(1000000000, 1999999999).ToString(),
                    Amount = amount1.ToString(),
                    Currency = "UYU",
                    Description = new[] {"una descripción"},
                    FinalConsumer = true,
                    Payable = true,
                    Precedence = 1,
                    TaxedAmount = amount1,
                },
                new BillSistarbancDto
                {
                    BillExternalId = nroFactura2.ToString(),
                    ExpirationDate = fecha.AddDays(2),
                    DateInit = fecha.AddDays(1).ToString(),
                    IdTransaccionSTB = rnd.Next(1000000000, 1999999999).ToString(),
                    Amount = amount2.ToString(),
                    Currency = "UYU",
                    Description = new[] {"una descripción"},
                    FinalConsumer = true,
                    Payable = true,
                    Precedence = 1,
                    TaxedAmount = amount2,
                },
                new BillSistarbancDto
                {
                    BillExternalId = nroFactura3.ToString(),
                    ExpirationDate = fecha.AddDays(3),
                    DateInit = fecha.AddDays(1).ToString(),
                    IdTransaccionSTB = rnd.Next(1000000000, 1999999999).ToString(),
                    Amount = amount3.ToString(),
                    Currency = "UYU",
                    Description = new[] {"una descripción"},
                    FinalConsumer = true,
                    Payable = true,
                    Precedence = 1,
                    TaxedAmount = amount3,
                }
            };
        }

        /// <summary>
        /// Devuelve 3 factura impaga
        ///     BillExternalId: Generado random
        ///     ExpirationDate: fecha actual + 1 día
        ///     DateInit: fecha actual + 1 día
        ///     IdTransaccionSTB: random de 10 dígitos (1000000000, 1999999999)
        ///     Amount: random (1000,9999)
        ///     Currency: "UYU"
        ///     Description: una descripción
        ///     FinalConsumer = true
        ///     Payable = true
        ///     Precedence = 1
        ///     TaxedAmount = amount
        /// </summary>
        /// <returns></returns>
        private IList<BillSistarbancDto> FacturasPorArribaDelMaximo()
        {
            var rnd = new Random();

            rnd.Next(10000, 99999);
            rnd.Next(10000, 99999);
            rnd.Next(10000, 99999);

            var nroFactura1 = rnd.Next(10000, 99999);
            var nroFactura2 = rnd.Next(10000, 99999);
            var nroFactura3 = rnd.Next(10000, 99999);
            var fecha = DateTime.Now.AddDays(1);
            var amount1 = rnd.Next(10001, 99999);
            var amount2 = rnd.Next(10001, 99999);
            var amount3 = rnd.Next(10001, 99999);

            return new List<BillSistarbancDto>
            {
                new BillSistarbancDto
                {
                    BillExternalId = nroFactura1.ToString(),
                    ExpirationDate = fecha.AddDays(1),
                    DateInit = fecha.ToString(),
                    IdTransaccionSTB = rnd.Next(1000000000, 1999999999).ToString(),
                    Amount = amount1.ToString(),
                    Currency = "UYU",
                    Description = new[] {"una descripción"},
                    FinalConsumer = true,
                    Payable = true,
                    Precedence = 1,
                    TaxedAmount = amount1,
                },
                new BillSistarbancDto
                {
                    BillExternalId = nroFactura2.ToString(),
                    ExpirationDate = fecha.AddDays(2),
                    DateInit = fecha.AddDays(1).ToString(),
                    IdTransaccionSTB = rnd.Next(1000000000, 1999999999).ToString(),
                    Amount = amount2.ToString(),
                    Currency = "UYU",
                    Description = new[] {"una descripción"},
                    FinalConsumer = true,
                    Payable = true,
                    Precedence = 1,
                    TaxedAmount = amount2,
                },
                new BillSistarbancDto
                {
                    BillExternalId = nroFactura3.ToString(),
                    ExpirationDate = fecha.AddDays(3),
                    DateInit = fecha.AddDays(1).ToString(),
                    IdTransaccionSTB = rnd.Next(1000000000, 1999999999).ToString(),
                    Amount = amount3.ToString(),
                    Currency = "UYU",
                    Description = new[] {"una descripción"},
                    FinalConsumer = true,
                    Payable = true,
                    Precedence = 1,
                    TaxedAmount = amount3,
                }
            };
        }

        public IEnumerable<ConciliationSistarbancDto> GetConciliation(DateTime from, DateTime to)
        {
            var list = new List<ConciliationSistarbancDto>
            {
                new ConciliationSistarbancDto
                {
                    Id = Guid.NewGuid(),
                    Date = DateTime.Now,
                    IdTransaccionSTB = "234567",
                    VisaTransactionId = 200,
                    SistarbancUserId = 1000,
                    BillExternalId = "4444",
                    Currency = "UYU",
                    Amount = 5000
                },
                new ConciliationSistarbancDto
                {
                    Id = Guid.NewGuid(),
                    Date = DateTime.Now,
                    IdTransaccionSTB = "345678",
                    VisaTransactionId = 201,
                    SistarbancUserId = 1001,
                    BillExternalId = "5555",
                    Currency = "UYU",
                    Amount = 6000
                }
            };

            if (from != default(DateTime))
                list = list.Where(l => l.Date > from).ToList();

            if (to != default(DateTime))
                list = list.Where(l => l.Date < to).ToList();

            return list;
        }

        public BillDto PagoReciboLif(string idBanco, string idOrganismo, string tipoServicio, string[] refServicio, BillDto bill,
            string idClienteBanco, string nroTrasnferenciaVisa, AutomaticPaymentDto automaticPaymentDto)
        {
            throw new NotImplementedException();
        }

        public int CheckAccount(string idBanco, string idOrganismo, string tipoServicio, string[] refServicio)
        {
            return 1;
        }

        public IEnumerable<BillSistarbancDto> ServicioImpagoLif(string idBanco, string idOrganismo, string tipoServicio, string[] refServicio)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<BillSistarbancDto> ServicioImpagoNroCuentaSinMergue(string idBanco, string idOrganismo, string tipoServicio,
            string[] refServicio)
        {
            throw new NotImplementedException();
        }
    }
}
