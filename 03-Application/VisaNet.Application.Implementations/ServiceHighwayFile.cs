using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using VisaNet.Application.Implementations.Base;
using VisaNet.Application.Interfaces;
using VisaNet.Common.Logging.NLog;
using VisaNet.Domain.Entities;
using VisaNet.Domain.Entities.Enums;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Repository.Interfaces.Interfaces;

namespace VisaNet.Application.Implementations
{
    public class ServiceHighwayFile : BaseService<HighwayBill, HighwayBillDto>, IServiceHighwayFile
    {
        private readonly IServicePayment _servicePayment;
        private readonly IRepositoryService _repositoryService;
        private readonly IServiceEmailMessage _serviceEmailMessage;
        
        public ServiceHighwayFile(IRepositoryHighwayBill repository, IServicePayment servicePayment, IRepositoryService repositoryService, IServiceEmailMessage serviceEmailMessage)
            : base(repository)
        {
            _servicePayment = servicePayment;
            _repositoryService = repositoryService;
            _serviceEmailMessage = serviceEmailMessage;
        }

        public override IQueryable<HighwayBill> GetDataForTable()
        {
            throw new NotImplementedException();
        }

        public override HighwayBillDto Converter(HighwayBill entity)
        {
            var highwayBillDto = new HighwayBillDto()
                                 {
                                     Id = entity.Id,
                                     CodSucursal = entity.CodSucursal,
                                     CodComercio = entity.CodComercio,
                                     ConsFinal = entity.ConsFinal,
                                     CreationDate = entity.CreationDate,
                                     Cuotas = entity.Cuotas,
                                     DiasPagoVenc = entity.DiasPagoVenc,
                                     Moneda = entity.Moneda,
                                     Descripcion = entity.Descripcion,
                                     RefCliente = entity.RefCliente,
                                     MontoGravado = entity.MontoGravado,
                                     MontoMinimo = entity.MontoMinimo,
                                     MontoTotal = entity.MontoTotal,
                                     NroFactura = entity.NroFactura,
                                     FchFactura = entity.FchFactura,
                                     FchVencimiento = entity.FchVencimiento,
                                     HighwayEmailId = entity.HighwayEmailId,
                                     ServiceId = entity.ServiceId
                                 };
            return highwayBillDto;
        }

        public override HighwayBill Converter(HighwayBillDto entity)
        {
            var highwayBill = new HighwayBill()
                              {
                                  Id = entity.Id,
                                  CodSucursal = entity.CodSucursal,
                                  CodComercio = entity.CodComercio,
                                  ConsFinal = entity.ConsFinal,
                                  CreationDate = entity.CreationDate,
                                  Cuotas = entity.Cuotas,
                                  DiasPagoVenc = entity.DiasPagoVenc,
                                  Moneda = entity.Moneda,
                                  Descripcion = entity.Descripcion,
                                  RefCliente = entity.RefCliente,
                                  MontoGravado = entity.MontoGravado,
                                  MontoMinimo = entity.MontoMinimo,
                                  MontoTotal = entity.MontoTotal,
                                  NroFactura = entity.NroFactura,
                                  FchFactura = entity.FchFactura,
                                  FchVencimiento = entity.FchVencimiento,
                                  HighwayEmailId = entity.HighwayEmailId,
                                  ServiceId = entity.ServiceId
                              };
            return highwayBill;
        }

        public void NotifyPaymentsToService(DateTime date)
        {
            var services = _repositoryService.All(s => s.Active && s.ServiceGateways.Any(g => g.Gateway.Enum == (int)GatewayEnum.Carretera && g.Active), s => s.ServiceGateways.Select(g => g.Gateway)).OrderBy(s => s.Name).ToList();
            NLogLogger.LogHighwayEvent(NLogType.Info, "    Servicios a enviar : " + services.Count);
            foreach (var service in services)
            {
                if (service.ServiceGateways.Any(g => g.Gateway.Enum == (int)GatewayEnum.Carretera && g.Active && g.SendExtract))
                {
                    //Armo archivo para cada servicio
                    NLogLogger.LogHighwayEvent(NLogType.Info, string.Format("    Servicio {0}, se prepara el archivo a enviar.", service.Name));
                    NotifyServiceFileMailgun(service, date);
                }
                else
                {
                    NLogLogger.LogHighwayEvent(NLogType.Info, string.Format("    Servicio {0} no tiene habilitado el envio del extract diario.", service.Name));
                }
            }
        }
        
        private void NotifyServiceFileMailgun(Service service, DateTime date)
        {
            try
            {
                var serGateway = service.ServiceGateways.FirstOrDefault(g => g.Gateway.Enum == (int)GatewayEnum.Carretera);
                if (serGateway == null)
                {
                    NLogLogger.LogHighwayEvent(NLogType.Info,
                        string.Format("        Servicio ({0}) no tiene pasarela cargada. ", service.Name));
                    return;
                }

                if (string.IsNullOrEmpty(service.ExtractEmail))
                {
                    NLogLogger.LogHighwayEvent(NLogType.Info,
                        string.Format("        Servicio ({0}) no tiene un email configurado para el envio del extract. ", service.Name));
                    return;
                }

                var codcom = serGateway.ReferenceId;
                var codBranch = serGateway.ServiceType;
                var payments = _servicePayment.GetPaymentBatch(date, GatewayEnum.Carretera, service.Id, -1);

                if (payments == null || !payments.Any())
                {
                    NLogLogger.LogHighwayEvent(NLogType.Info,
                        string.Format("        Servicio ({0}) no tiene transacciones realizadas. ", service.Name));

                    var result = _serviceEmailMessage.SendhighwayTransacctionReports(new HighwayEmailDataDto()
                    {
                        Subject = string.Format("VisaNet - Reporte de pagos {0} - {1}", service.Name, DateTime.Now.ToString("dd/MM/yyyy")),
                        Message = string.Format("No se realizaron pagos para la fecha {0}", date.ToString("dd/MM/yyyy")),
                        Email = service.ExtractEmail,
                        FileName = null,
                        MimeType = null,
                        FilePath = null
                    });

                    NLogLogger.LogHighwayEvent(NLogType.Info, result >  0 
                            ? string.Format("        Servicio ({0}) no pudo generar email por medio de mailgun. ", service.Name)
                            : string.Format("        Servicio ({0}) se genero email por medio de mailgun. ", service.Name));
                    return;
                }

                var fileName = "PAGOS_VNP_" + codcom + "_" + codBranch + "_" + DateTime.Now.ToString("yyyyMMdd") +
                               ".txt";
                var path = Path.Combine(ConfigurationManager.AppSettings["HighwayFilesGenerated"], fileName);

                if (File.Exists(path))
                {
                    NLogLogger.LogHighwayEvent(NLogType.Info,
                        string.Format("        Servicio ({0}) ya tiene un archivo generado ({1}). ", service.Name,
                            fileName));
                    return;
                }

                var raw = "";
                var count = 0;
                var countN = 0;
                var countD = 0;
                double valueN = 0;
                double valueD = 0;

                foreach (var dto in payments)
                {
                    var bill = dto.Bills.FirstOrDefault();
                    var amount = bill != null ? (bill.Amount - bill.DiscountAmount) * 100 : 0;
                    raw = raw + codcom + "|" + codBranch + "|";
                    raw = raw + dto.ReferenceNumber + "|";
                    raw = raw + (bill != null ? bill.BillExternalId : "") + "|";
                    raw = raw + amount + "|";
                    raw = raw + (bill != null ? bill.Currency.Equals("UYU") ? "N" : "D" : "") + "|";
                    raw = raw + dto.Date.ToString("yyyyMMdd") + "|";
                    raw = raw + "1" + "|";
                    raw = raw + (bill != null ? (bill.DiscountAmount * 100).ToString() : "") + "|";
                    raw = raw + Environment.NewLine;
                    count++;
                    if (bill.Currency.Equals("UYU"))
                    {
                        countN++;
                        valueN = valueN + amount;
                    }
                    if (bill.Currency.Equals("USD"))
                    {
                        countD++;
                        valueD = valueD + amount;
                    }
                }

                raw = raw + "RESUMEN" + count + countN + valueN + countD + valueD;

                var created = CreateFile(raw, path);
                if (created)
                {
                    NLogLogger.LogHighwayEvent(NLogType.Info,
                        string.Format("        Servicio ({0}) se creo archivo ({1}). ", service.Name, fileName));
                }
                else
                {
                    NLogLogger.LogHighwayEvent(NLogType.Info,
                        string.Format("        Servicio ({0}) no se pudo crear archivo ({1}). ", service.Name, fileName));
                }

                
                var resultOk = _serviceEmailMessage.SendhighwayTransacctionReports(new HighwayEmailDataDto()
                                                                    {
                                                                        Subject = string.Format("VisaNet - Reporte de pagos {0} - {1}", service.Name, DateTime.Now.ToString("dd/MM/yyyy")),
                                                                        Message = string.Format("Estas son las transacciones realizadas para la fecha {0}", date.ToString("dd/MM/yyyy")),
                                                                        AttachmentFile = new Attachment(path),
                                                                        Email = service.ExtractEmail,
                                                                        FilePath = path,
                                                                        FileName = fileName,
                                                                        MimeType = "text/plain"
                                                                    });

                NLogLogger.LogHighwayEvent(NLogType.Info, resultOk > 0 
                           ? string.Format("        Servicio ({0}) no pudo generar email por medio de mailgun. ", service.Name)
                           : string.Format("        Servicio ({0}) se genero email por medio de mailgun. ", service.Name));

            }
            catch (Exception exception)
            {
                NLogLogger.LogHighwayEvent(exception);
                NLogLogger.LogHighwayEvent(NLogType.Info, string.Format("        Servicio ({0}) no pudo generar email por medio de mailgun. ", service.Name));
            }

        }

        private bool CreateFile(string raw, string fileName)
        {
            try
            {
                TextWriter tw = new StreamWriter(fileName, true);
                tw.WriteLine(raw);
                tw.Close();
                return true;
            }
            catch (Exception e)
            {
                NLogLogger.LogHighwayEvent(e);
            }
            return false;

        }

    }
}