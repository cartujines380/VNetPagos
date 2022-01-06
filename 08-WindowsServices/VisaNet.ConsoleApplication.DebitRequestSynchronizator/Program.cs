using Ninject;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisaNet.Common.DependencyInjection;
using VisaNet.Common.Logging.NLog;
using VisaNet.DebitRequestSynchronization.Implementation;

namespace VisaNet.ConsoleApplication.DebitRequestSynchronizator
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
                    var service = kernel.Get<IDebitRequestSynchronizatorService>();
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
