using System;
using System.Diagnostics;
using System.Threading.Tasks;
using VisaNet.Application.Interfaces;
using VisaNet.Common.Logging.NLog;
using VisaNet.Domain.EntitiesDtos.TableFilters;

namespace VisaNet.ConsoleApplication.ConciliationReports.Reports
{
    public class ReportGenerator
    {
        private static IServiceConciliationCybersource _serviceConciliationCybersource;
        private static IServiceConciliationBanred _serviceServiceConciliationBanred;
        private static IServiceConciliationSistarbanc _serviceServiceConciliationSistarbanc;
        private static IServiceConciliationSucive _serviceServiceConciliationSucive;
        private static IServiceConciliationVisanetCallback _serviceServiceConciliationVisanetCallback;
        private static IServiceConciliationSummary _serviceServiceConciliationSummary;

        public ReportGenerator(IServiceConciliationCybersource serviceConciliationCybersource,
            IServiceConciliationBanred serviceServiceConciliationBanred, IServiceConciliationSistarbanc serviceServiceConciliationSistarbanc,
            IServiceConciliationSucive serviceServiceConciliationSucive, IServiceConciliationVisanetCallback serviceServiceConciliationVisanetCallback,
            IServiceConciliationSummary serviceServiceConciliationSummary)
        {
            _serviceConciliationCybersource = serviceConciliationCybersource;
            _serviceServiceConciliationBanred = serviceServiceConciliationBanred;
            _serviceServiceConciliationSistarbanc = serviceServiceConciliationSistarbanc;
            _serviceServiceConciliationSucive = serviceServiceConciliationSucive;
            _serviceServiceConciliationVisanetCallback = serviceServiceConciliationVisanetCallback;
            _serviceServiceConciliationSummary = serviceServiceConciliationSummary;
        }

        public async Task<bool> ObtainCybersourceConciliationData(DateTime? datefrom, DateTime? dateto)
        {
            var from = datefrom ?? DateTime.Now.AddDays(-5);
            var to = dateto ?? DateTime.Now.AddDays(-1);

            try
            {
                var sw = new Stopwatch();
                NLogLogger.LogEvent(NLogType.Info, "INICIA OBTENCION DE DATOS DE CYBERSOURCE PARA REPORTE CONCILIACION ");
                NLogLogger.LogEvent(NLogType.Info, "FECHA: " + DateTime.Now.ToString("d"));
                sw.Start();
                var result = await _serviceConciliationCybersource.GetConciliation(new ReportsConciliationFilterDto { From = from, To = to });
                sw.Stop();

                if (result)
                {
                    NLogLogger.LogEvent(NLogType.Info, "SE GENERARON LOS DATOS EN CYBERSOURCE PARA EL RANGO DE FECHAS : " + from.ToString("d") + " - " + to.ToString("d"));
                    return true;
                }
            }
            catch (Exception e)
            {
                NLogLogger.LogEvent(NLogType.Error, "SE PRODUJO UN ERROR AL GENERAR LOS DATOS EN CYBERSOURCE PARA EL RANGO DE FECHAS : " + from.ToString("d") + " - " + to.ToString("d"));
                NLogLogger.LogEvent(NLogType.Error, "MENSAJE DE EXCEPTION: " + e.Message);
                NLogLogger.LogEvent(e);
                throw;
            }
            return false;
        }

        public void ObtainBanredConciliationData()
        {
            try
            {
                var sw = new Stopwatch();
                NLogLogger.LogEvent(NLogType.Info, "INICIA OBTENCION DE DATOS DE BANRED PARA REPORTE CONCILIACION ");
                NLogLogger.LogEvent(NLogType.Info, "FECHA: " + DateTime.Now.ToString("d"));
                sw.Start();
                var result = _serviceServiceConciliationBanred.DirectoryConciliation();
                sw.Stop();
                if (result)
                {
                    NLogLogger.LogEvent(NLogType.Info, "SE GENERARON LOS DATOS CORRECTAMENTE ");
                }
            }
            catch (Exception e)
            {
                NLogLogger.LogEvent(NLogType.Error, "SE PRODUJO UN ERROR AL GENERAR LOS DATOS");
                NLogLogger.LogEvent(NLogType.Error, "MENSAJE DE EXCEPTION: " + e.Message);
                NLogLogger.LogEvent(e);
                throw;
            }
        }

        public void ObtainSistarbancConciliationData()
        {
            try
            {
                var sw = new Stopwatch();
                NLogLogger.LogEvent(NLogType.Info, "INICIA OBTENCION DE DATOS DE SISTARBANC PARA REPORTE CONCILIACION ");
                NLogLogger.LogEvent(NLogType.Info, "FECHA: " + DateTime.Now.ToString("d"));
                sw.Start();
                var result = _serviceServiceConciliationSistarbanc.DirectoryConciliation();
                sw.Stop();
                if (result)
                {
                    NLogLogger.LogEvent(NLogType.Info, "SE GENERARON LOS DATOS CORRECTAMENTE ");
                }
            }
            catch (Exception e)
            {
                NLogLogger.LogEvent(NLogType.Error, "SE PRODUJO UN ERROR AL GENERAR LOS DATOS");
                NLogLogger.LogEvent(NLogType.Error, "MENSAJE DE EXCEPTION: " + e.Message);
                NLogLogger.LogEvent(e);
                throw;
            }
        }

        public void ObtainSuciveConciliationData(DateTime? datefrom, DateTime? dateto)
        {
            try
            {
                var from = datefrom ?? DateTime.Now.AddDays(-1);
                var to = dateto ?? DateTime.Now.AddDays(-1);

                var sw = new Stopwatch();
                NLogLogger.LogEvent(NLogType.Info, "INICIA OBTENCION DE DATOS DE SUCIVE PARA REPORTE CONCILIACION ");
                NLogLogger.LogEvent(NLogType.Info, "FECHA: " + DateTime.Now.ToString("d"));
                sw.Start();
                var result = _serviceServiceConciliationSucive.GetConciliation(new ReportsConciliationFilterDto { From = from, To = to });
                sw.Stop();
                if (result)
                {
                    NLogLogger.LogEvent(NLogType.Info, "SE GENERARON LOS DATOS CORRECTAMENTE ");
                }
            }
            catch (Exception e)
            {
                NLogLogger.LogEvent(NLogType.Error, "SE PRODUJO UN ERROR AL GENERAR LOS DATOS");
                NLogLogger.LogEvent(NLogType.Error, "MENSAJE DE EXCEPTION: " + e.Message);
                NLogLogger.LogEvent(e);
                throw;
            }
        }

        public void ObtainVisanetCallbackConciliationData()
        {
            try
            {
                var sw = new Stopwatch();
                NLogLogger.LogEvent(NLogType.Info, "INICIA OBTENCION DE DATOS DEL PROCESAMIENTO DE VISA PARA REPORTE CONCILIACION");
                NLogLogger.LogEvent(NLogType.Info, "FECHA: " + DateTime.Now.ToString("d"));
                sw.Start();
                _serviceServiceConciliationVisanetCallback.DirectoryConciliation();
                sw.Stop();
                NLogLogger.LogEvent(NLogType.Info, "FIN PROCESAMIENTO DE VISA PARA REPORTE CONCILIACION");
            }
            catch (Exception e)
            {
                NLogLogger.LogEvent(NLogType.Error, "SE PRODUJO UN ERROR AL GENERAR LOS DATOS");
                NLogLogger.LogEvent(NLogType.Error, "MENSAJE DE EXCEPTION: " + e.Message);
                NLogLogger.LogEvent(e);
                throw;
            }
        }

        /// <summary>
        ///     POPULA LA TABLA ConciliationSummary
        ///     Los datos del tc33 (visanet) se populan cuando se sube un archivo.
        /// </summary>
        public void GenerateSummary()
        {
            try
            {
                NLogLogger.LogEvent(NLogType.Info, "INICIA PROCESAMIENTO DE DATOS PARA EL RESUMEN DE LA CONCILIACIÓN");
                Console.Write("INICIA PROCESAMIENTO DE DATOS PARA EL RESUMEN DE LA CONCILIACIÓN");
                NLogLogger.LogEvent(NLogType.Info, "FECHA: " + DateTime.Now.ToString("d"));
                _serviceServiceConciliationSummary.GenerateSummary();
                NLogLogger.LogEvent(NLogType.Info, "FINALIZA PROCESAMIENTO DE DATOS PARA EL RESUMEN DE LA CONCILIACIÓN");
                Console.Write("FINALIZA PROCESAMIENTO DE DATOS PARA EL RESUMEN DE LA CONCILIACIÓN");
            }
            catch (Exception e)
            {
                NLogLogger.LogEvent(NLogType.Error, "SE PRODUJO UN ERROR AL ANALIZAR LOS DATOS");
                Console.Write("SE PRODUJO UN ERROR AL ANALIZAR LOS DATOS");
                NLogLogger.LogEvent(e);
                throw;
            }
        }

    }
}