using System;
using System.Diagnostics;
using VisaNet.Application.Interfaces;
using VisaNet.Common.DependencyInjection;
using VisaNet.Common.Logging.NLog;

namespace VisaNet.ConsoleApplication.BinFileProcessor
{
    public class BinFileProcessorHandler
    {
        private static readonly IServiceBinFile _serviceBinFile = NinjectRegister.Get<IServiceBinFile>();

        public void Execute()
        {
            var sw = new Stopwatch();
            try
            {
                NLogLogger.LogEvent(NLogType.Info, "INICIA PROCESO");
                sw.Start();
                var result = _serviceBinFile.ProcessFile();
                sw.Stop();
                NLogLogger.LogEvent(NLogType.Info, "FIN DEL PROCESO - DURACIÓN: " + sw.Elapsed.ToString(@"hh\:mm\:ss") + " - INSERTS: " + result.Inserts + " - UPDATES: " + result.Updates + " - DELETES: " + result.Deletes + " - ERRORES: " + result.Errors );
            }
            catch (Exception e)
            {
                NLogLogger.LogEvent(NLogType.Info, "FINALIZO EL PROCESO CON ERROR - DURACIÓN: " + sw.Elapsed.ToString(@"hh\:mm\:ss"));
                NLogLogger.LogEvent(NLogType.Error, "ERROR: " + e);
            }
        }
    }
}