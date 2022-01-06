using Ninject;
using VisaNet.Common.DependencyInjection;

namespace VisaNet.ConsoleApplication.NotificationGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var kernel = new StandardKernel();
            NinjectRegister.RegisterThreadScope(kernel);
        }
    }
}
