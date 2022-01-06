using System;
using System.Configuration;
using System.Threading.Tasks;
using System.Globalization;
using VisaNet.Common.Exceptions;

namespace VisaNet.Testing.LifClientApp
{
    class Program
    {
        static void Main(string[] args)
        {


            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("es-UY");
            System.Threading.Thread.CurrentThread.CurrentUICulture = new CultureInfo("es-UY");

            Console.WriteLine("******* LIF CLIENT APP *******");
            Console.WriteLine();
            var exit = false;
            while (!exit)
            {
                try
                {
                    Console.WriteLine();
                    Console.WriteLine("Metodos de la API.LIF");
                    Console.WriteLine("\t1-> card/data");
                    Console.WriteLine("\t2-> card/nationalData");
                    Console.WriteLine("\t3-> discount/app");
                    Console.WriteLine("\t4-> Salir");
                    Console.Write("\tOpción: ");
                    var line = Console.ReadLine();
                    int option;
                    if (int.TryParse(line, out option))
                    {
                        switch (option)
                        {
                            case 1:
                                CartDataEndpointInvocation();
                                break;
                            case 2:
                                CartNationalDataEndpointInvocation();
                                break;
                            case 3:
                                DiscountEndpointInvocation();
                                break;
                            case 4:
                                exit = true;
                                break;
                            case 666:
                                DiscountEndpointInvocationWithDummyData();
                                break;
                            default:
                                PrintInvalidOption(line, 1);
                                break;
                        }
                    }
                    else
                    {
                        PrintInvalidOption(line, 1);
                    }
                }
                catch (BusinessException e)
                {
                    Console.WriteLine();
                    Console.WriteLine(e.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine();
                    Console.WriteLine("Ha ocurrido un error no controlado");
                }
            }
        }

        private static void DiscountEndpointInvocationWithDummyData()
        {
            var bin2 = "424242";
            var amount = "50000";
            var currency = "UYU";
            var final = true;
            var lawId = "1";
            var taxedAmount = "45000";


            var discountData = AdjustData(LifClientAppHandler.DiscountApp(bin2, amount, currency, final, lawId, taxedAmount));

            Console.WriteLine("\t\tDatos");
            Console.WriteLine(discountData);
        }

        private static void DiscountEndpointInvocation()
        {
            Console.WriteLine("\t\tParámetros");
            Console.Write("\t\t\tBin: ");
            var bin2 = Console.ReadLine();
            Console.Write("\t\t\tMonto: ");
            var amount = Console.ReadLine();
            Console.Write("\t\t\tMoneda: ");
            var currency = Console.ReadLine();
            var y = true;
            var final = true;
            while (y)
            {
                Console.Write("\t\t\tEs consumidor final? (S/N): ");
                var l = Console.ReadLine();
                if (l != null && (l.Equals("S") || l.Equals("s") || l.Equals("N") || l.Equals("n")))
                {
                    y = false;
                    final = l.Equals("S") || l.Equals("s");
                }
                else
                {
                    PrintInvalidOption(l, 3);
                }
            }
            Console.Write("\t\t\tLey: ");
            var lawId = Console.ReadLine();
            Console.Write("\t\t\tMonto gravado de la factura: ");
            var TaxedAmount = Console.ReadLine();

            // Invocación a la API
            var discountData = AdjustData(LifClientAppHandler.DiscountApp(bin2, amount, currency, final, lawId, TaxedAmount));

            Console.WriteLine("\t\tDatos");
            Console.WriteLine(discountData);
        }

        private static void CartNationalDataEndpointInvocation()
        {
            var nationalData = LifClientAppHandler.NationalData();
            var path = ConfigurationManager.AppSettings["filePath"];
            try
            {
                System.IO.File.WriteAllText(path, nationalData);
                Console.WriteLine(
                    "\t\tEl resultado es muy largo para mostrarlo en pantalla. Se guardo un archivo en la siguiente ruta: " +
                    path);
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "\t\tEl resultado es muy largo para mostrarlo en pantalla. Se trato de guardar un archivo en la siguiente ruta: '" +
                    path + "', pero ocurrió un error inseperado");
            }
        }

        private static void CartDataEndpointInvocation()
        {
            Console.WriteLine("\t\tParámetros");
            Console.Write("\t\t\tBin: ");
            var bin = Console.ReadLine();
            var cardData = AdjustData(LifClientAppHandler.CardData(bin));
            Console.WriteLine("\t\tDatos");
            Console.WriteLine(cardData);
        }

        private static void PrintInvalidOption(string option, int indent = 0)
        {
            var line = "";
            for (var i = 0; i < indent; i++)
            {
                line += "\t";
            }
            line += "'" + option + "' no es una opción valida";
            Console.WriteLine(line);
        }

        private static string AdjustData(string data)
        {
            data = data.Replace("\r\n", "\r\n\t\t\t");
            data = "\t\t\t" + data;
            return data;
        }
    }
}
