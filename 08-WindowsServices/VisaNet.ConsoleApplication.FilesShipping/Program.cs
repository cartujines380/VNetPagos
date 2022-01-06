using System.Linq;
using Ninject;
using VisaNet.Common.DependencyInjection;
using VisaNet.Common.Logging.NLog;

namespace VisaNet.ConsoleApplication.FilesShipping
{
    class Program
    {
        static void Main(string[] args)
        {
            // args = new[] { "servicesaskedextract" };
            //args = new[] { "sistarbancbatchfile" };
            //args = new[] { "sucivebatchfile" };

            var kernel = new StandardKernel();
            NinjectRegister.Register(kernel, true);

            if (args.Any())
            {
                var fileShipping = new FileShipping();
                var method = args.First();
                switch (method.ToLower())
                {
                    //case "highwayfiles":
                    //    fileShipping.SendHighwayFile(days);
                    //    break;
                    //case "extractbanred":
                    //    fileShipping.SendPaymentsFileExtract(days, GatewayEnum.Banred);
                    //    break;
                    //case "extractsistarbanc":
                    //    fileShipping.SendPaymentsFileExtract(days, GatewayEnum.Sistarbanc);
                    //    break;
                    //case "extractgeocom":
                    //    fileShipping.SendPaymentsFileGeocom(days);
                    //    break;
                    case "sucivebatchfile":
                        fileShipping.SendPaymentsFileSucive();
                        break;
                    case "sistarbancbatchfile":
                        fileShipping.SendPaymentsFileSistarbanc();
                        break;
                    case "sftptest":
                        var sftp = new SftpTest();
                        sftp.TestLoad();
                        break;
                    case "servicesaskedextract":
                        fileShipping.SendExtractToServices();
                        break;
                    default:
                        NLogLogger.LogEvent(NLogType.Info, string.Format("FileShipping Procces - La opción ingresada no existe: {0}", method));
                        break;
                }
            }
            else
            {
                NLogLogger.LogEvent(NLogType.Info, string.Format("FileShipping Procces - No se ingreso opción"));
            }
        }
    }
}
