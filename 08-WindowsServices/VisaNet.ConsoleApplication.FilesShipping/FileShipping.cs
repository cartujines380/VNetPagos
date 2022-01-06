using System;
using System.Configuration;
using VisaNet.Application.Interfaces;
using VisaNet.Common.DependencyInjection;
using VisaNet.Common.Logging.NLog;
using VisaNet.Domain.Entities.Enums;

namespace VisaNet.ConsoleApplication.FilesShipping
{
    public class FileShipping
    {
        private static readonly IServiceHighwayFile _serviceHighwayFile = NinjectRegister.Get<IServiceHighwayFile>();
        private static readonly IServiceFileShipping _serviceFileShipping = NinjectRegister.Get<IServiceFileShipping>();


        private void SendHighwayFile(DateTime date)
        {
            try
            {
                NLogLogger.LogHighwayEvent(NLogType.Info, "INICIA PROCESO GENERACION ARCHIVOS CARRETERA " + DateTime.Now.ToString("g"));
                _serviceHighwayFile.NotifyPaymentsToService(date);
                NLogLogger.LogHighwayEvent(NLogType.Info, "FINALIZA PROCESO GENERACION ARCHIVOS CARRETERA");
            }
            catch (Exception e)
            {
                NLogLogger.LogHighwayEvent(e);
                throw;
            }
            
        }

        private void SendPaymentsFileExtract(DateTime date, GatewayEnum gateway)
        {
            try
            {
                NLogLogger.LogExtractEvent(NLogType.Info, "INICIA PROCESO GENERACION ARCHIVOS " + gateway.ToString().ToUpper() + " " + DateTime.Now.ToString("g"));
                _serviceFileShipping.NotifyPaymentsToService(date, (int)gateway);
                NLogLogger.LogExtractEvent(NLogType.Info, "FINALIZA PROCESO GENERACION ARCHIVOS " + gateway.ToString().ToUpper());
            }
            catch (Exception e)
            {
                NLogLogger.LogExtractEvent(e);
                throw;
            }
        }

        public void SendPaymentsFileSucive()
        {
            try
            {
                NLogLogger.LogSuciveEvent(NLogType.Info, "INICIA PROCESO GENERACION ARCHIVOS SUCIVE" + " " + DateTime.Now.ToString("g"));
                var day = Int16.Parse(ConfigurationManager.AppSettings["SuciveBatchDay"]);
                var date = day > 0 ? DateTime.Today.AddDays(-day) : DateTime.Today.AddDays(-1);
                _serviceFileShipping.SuciveBatchConsiliation(date);
                NLogLogger.LogSuciveEvent(NLogType.Info, "FINALIZA PROCESO GENERACION ARCHIVOS SUCIVE");
            }
            catch (Exception e)
            {
                NLogLogger.LogSuciveEvent(e);
                throw;
            }
        }

        private void SendPaymentsFileGeocom(DateTime date)
        {
            try
            {
                NLogLogger.LogGeocomEvent(NLogType.Info, "INICIA PROCESO GENERACION ARCHIVOS GEOCOM" + " " + DateTime.Now.ToString("g"));
                _serviceFileShipping.NotifyPaymentsToGeocom(date);
                NLogLogger.LogGeocomEvent(NLogType.Info, "FINALIZA PROCESO GENERACION ARCHIVOS GEOCOM");
            }
            catch (Exception e )
            {
                NLogLogger.LogGeocomEvent(e);
                throw;
            }
        }

        public void SendPaymentsFileSistarbanc()
        {
            try
            {
                NLogLogger.LogSistarbancEvent(NLogType.Info, "INICIA PROCESO GENERACION ARCHIVOS SISTARBANC" + " " + DateTime.Now.ToString("g"));
                _serviceFileShipping.SistarbancBatchConsiliation();
                NLogLogger.LogSistarbancEvent(NLogType.Info, "FINALIZA PROCESO GENERACION ARCHIVOS SISTARBANC");
            }
            catch (Exception e)
            {
                NLogLogger.LogSistarbancEvent(e);
                throw;
            }
        }

        public void SendExtractToServices()
        {
            var day = Int16.Parse(ConfigurationManager.AppSettings["ExtractDay"]);
            var date = day > 0 ? DateTime.Today.AddDays(-day) : DateTime.Today.AddDays(-1);
            SendHighwayFile(date);
            SendPaymentsFileExtract(date, GatewayEnum.Banred);
            SendPaymentsFileExtract(date, GatewayEnum.Importe);
            SendPaymentsFileGeocom(date);
        }
    }
}
