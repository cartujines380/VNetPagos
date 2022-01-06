using System;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Ninject;
using VisaNet.Common.DependencyInjection;
using VisaNet.Common.Logging.NLog;
using VisaNet.ConsoleApplication.ConciliationReports.Reports;

namespace VisaNet.ConsoleApplication.ConciliationReports
{
    class Program
    {
        private static void Main(string[] args)
        {
            //args = new[] { "generateSummary" };
            //args = new[] { "conciliationCybersource" };
            //args = new[] { "conciliationBanred" };
            //args = new[] { "conciliationSistarbanc" };
            //args = new[] { "conciliationSucive" };
            //args = new[] { "visanetCallback" };

            var kernel = new StandardKernel();
            NinjectRegister.Register(kernel, true);
            var generator = NinjectRegister.Get<ReportGenerator>();

            string arg = null;
            var enableConsoleInputConfig = ConfigurationManager.AppSettings["EnableConsoleInput"];
            enableConsoleInputConfig = !string.IsNullOrEmpty(enableConsoleInputConfig) ? enableConsoleInputConfig.ToLower() : "false";
            var enableConsoleInput = enableConsoleInputConfig.Equals("true") || enableConsoleInputConfig.Equals("t") || enableConsoleInputConfig.Equals("s");

            if (args.Any() && !string.IsNullOrEmpty(args[0]))
            {
                arg = args[0];
            }
            else if (enableConsoleInput)
            {
                var validInput = ReadInputValid(ref arg);
                while (!validInput)
                {
                    Console.WriteLine("");
                    Console.WriteLine("¡ATENCIÓN! Ingresó una opción inválida.");
                    validInput = ReadInputValid(ref arg);
                }
                if (arg == "exit")
                {
                    return;
                }
            }

            if (!string.IsNullOrEmpty(arg))
            {
                var from = DateTime.Now.AddDays(-1);
                var to = DateTime.Now.AddDays(-1);

                var configFrom = ConfigurationManager.AppSettings["reportdayfrom"];
                var configTo = ConfigurationManager.AppSettings["reportdayto"];

                switch (arg.ToLower())
                {
                    //GENERA EL RESUMEN PARA EL REPORTE A PARTIR DE TODAS LAS TABLAS DE CONCILIACION
                    case "generatesummary":
                        Console.WriteLine("");
                        Console.WriteLine("GENERANDO SUMMARY DE LA CONCILIACIÓN");
                        generator.GenerateSummary();
                        break;

                    //CARGA LA CONCILIACION INDIVIDUAL DE CADA TABLA
                    case "conciliationcybersource":
                        if (!string.IsNullOrEmpty(configFrom) && !string.IsNullOrEmpty(configTo))
                        {
                            var fromSplit = configFrom.Split('/');
                            from = new DateTime(int.Parse(fromSplit[0]), int.Parse(fromSplit[1]), int.Parse(fromSplit[2]));
                            var toSplit = configTo.Split('/');
                            to = new DateTime(int.Parse(toSplit[0]), int.Parse(toSplit[1]), int.Parse(toSplit[2]));
                        }
                        Console.WriteLine("");
                        Console.WriteLine("PROCESANDO CONCILIACIÓN DE CYBERSOURCE");
                        Console.WriteLine(string.Format("Desde: {0}. Hasta: {1}.", from.ToString("dd/MM/yyyy"), to.ToString("dd/MM/yyyy")));
                        var getListTask = generator.ObtainCybersourceConciliationData(from, to);
                        Task.WaitAll(getListTask); // block while the task completes
                        break;

                    case "conciliationbanred":
                        Console.WriteLine("");
                        Console.WriteLine("PROCESANDO CONCILIACIÓN DE BANRED");
                        Console.WriteLine(string.Format("Directorio: {0}", ConfigurationManager.AppSettings["BanredUnprocessedFolder"]));
                        generator.ObtainBanredConciliationData();
                        break;

                    case "conciliationsistarbanc":
                        Console.WriteLine("");
                        Console.WriteLine("PROCESANDO CONCILIACIÓN DE SISTARBANC");
                        Console.WriteLine(string.Format("Directorio: {0}", ConfigurationManager.AppSettings["SistarbancUnprocessedFolder"]));
                        generator.ObtainSistarbancConciliationData();
                        break;

                    case "conciliationsucive":
                        if (!string.IsNullOrEmpty(configFrom) && !string.IsNullOrEmpty(configTo))
                        {
                            var fromSplit = configFrom.Split('/');
                            from = new DateTime(int.Parse(fromSplit[0]), int.Parse(fromSplit[1]), int.Parse(fromSplit[2]));
                            var toSplit = configTo.Split('/');
                            to = new DateTime(int.Parse(toSplit[0]), int.Parse(toSplit[1]), int.Parse(toSplit[2]));
                        }
                        Console.WriteLine("");
                        Console.WriteLine("PROCESANDO CONCILIACIÓN DE SUCIVE");
                        Console.WriteLine(string.Format("Desde: {0}. Hasta: {1}.", from.ToString("dd/MM/yyyy"), to.ToString("dd/MM/yyyy")));
                        generator.ObtainSuciveConciliationData(from, to);
                        break;

                    case "visanetcallback":
                        Console.WriteLine("");
                        Console.WriteLine("PROCESANDO CONCILIACIÓN DE VISANET CALLBACK");
                        Console.WriteLine(string.Format("Directorio: {0}", ConfigurationManager.AppSettings["VisanetCallbackUnprocessedFolder"]));
                        generator.ObtainVisanetCallbackConciliationData();
                        break;

                    default:
                        NLogLogger.LogEvent(NLogType.Info, string.Format("La opción ingresada no existe: {0}", arg));
                        break;
                }
            }
            else
            {
                Console.WriteLine("No se ingreso opción.");
                NLogLogger.LogEvent(NLogType.Info, string.Format("No se ingreso opción"));
            }
        }

        private static void ConsoleOptions()
        {
            Console.WriteLine("Ingrese una una de las siguientes opciones y presione Enter.");
            Console.WriteLine("");
            Console.WriteLine("------------------------------------");
            Console.WriteLine("OPCIONES");
            Console.WriteLine("------------------------------------");
            Console.WriteLine("cyb   => Ejecuta la conciliación de Cybersource");
            Console.WriteLine("ban   => Ejecuta la conciliación de Banred");
            Console.WriteLine("sis   => Ejecuta la conciliación de Sistarbanc");
            Console.WriteLine("suc   => Ejecuta la conciliación de Sucive");
            Console.WriteLine("call  => Ejecuta la conciliación de VisaNet Callback");
            Console.WriteLine("sum   => Ejecuta el proceso de generación del Summary de la conciliación");
            Console.WriteLine("s     => Salir ");
            Console.WriteLine("------------------------------------");
            Console.WriteLine("");
            Console.WriteLine("Opcion: ");
        }

        private static string TransformOption(string input)
        {
            input = input != null ? input.ToLower() : string.Empty;
            switch (input)
            {
                case "cyb":
                    return "conciliationcybersource";
                case "ban":
                    return "conciliationbanred";
                case "sis":
                    return "conciliationsistarbanc";
                case "suc":
                    return "conciliationsucive";
                case "call":
                    return "visanetcallback";
                case "sum":
                    return "generatesummary";
                case "s":
                    return "exit";
                default:
                    return "invalid";
            }
        }

        private static bool ReadInputValid(ref string arg)
        {
            ConsoleOptions();
            arg = Console.ReadLine();
            arg = TransformOption(arg);
            if (arg == "invalid")
            {
                return false;
            }
            return true;
        }

    }
}