using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisaNet.Application.Interfaces;
using VisaNet.Common.DependencyInjection;

namespace VisaNet.ConsoleApplication.DeleteOldTokens
{
    class Program
    {
        static void Main(string[] args)
        {
            var kernel = new StandardKernel();
            NinjectRegister.Register(kernel);

            var serviceCards = kernel.Get<IServiceCard>();

            try
            {
                serviceCards.DeleteOldCardsToken();
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
