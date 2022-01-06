using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Ninject;
using VisaNet.Common.DependencyInjection;
using VisaNet.Common.Logging.NLog;

namespace VisaNet.ConsoleApplication.BinFileProcessor
{
    class Program
    {
        static void Main(string[] args)
        {
            var kernel = new StandardKernel();
            NinjectRegister.Register(kernel, true);

            var process = new BinFileProcessorHandler();
            process.Execute();
        }
    }
}
