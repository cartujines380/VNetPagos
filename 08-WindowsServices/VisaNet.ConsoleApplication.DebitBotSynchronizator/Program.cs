using Ninject;
using System;
using System.IO;
using VisaNet.Common.DependencyInjection;
using VisaNet.Common.Logging.NLog;
using VisaNet.DebitRequestBotSynchronization.Implementation;

namespace VisaNet.ConsoleApplication.DebitBotSynchronizator
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                if (!File.Exists("Logs"))
                {
                    Directory.CreateDirectory("Logs");
                }

                using (var kernel = new StandardKernel())
                {
                    NinjectRegister.RegisterSingleton(kernel);
                    var service = kernel.Get<IDebitRequestBotSynchronizatorService>();
                    service.StartSynchronization();
                }
            }
            catch (Exception ex)
            {
                NLogLogger.LogEvent(ex);
            }
        }
    }
}
