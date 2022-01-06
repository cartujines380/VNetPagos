using System;
using Ninject;
using VisaNet.Application.Interfaces;
using VisaNet.Common.DependencyInjection;
using VisaNet.Domain.Entities.BatchProcesses.CSAcknowledgement.Interfaces;

namespace VisaNet.ConsoleApplication.CSAcknowledgement
{
    class Program
    {
        static void Main(string[] args)
        {
            var kernel = new StandardKernel();
            NinjectRegister.RegisterThreadScope(kernel, true);
            var csAcknowledgementService = NinjectRegister.Get<IServiceCyberSourceAcknowledgement>();
            var loggerHelper = NinjectRegister.Get<ICsAckLoggerHelper>();

            try
            {
                Console.WriteLine("INICIA PROCESO DE VOID");
                loggerHelper.LogVoidProcessStarted();

                csAcknowledgementService.VoidPayments();

                loggerHelper.LogVoidProcessFinished();
                Console.WriteLine("FINALIZA PROCESO DE VOID");
            }
            catch (Exception e)
            {
                loggerHelper.LogVoidProcessException(e);
                Console.WriteLine("SE PRODUJO UN ERROR");
            }
        }

    }
}