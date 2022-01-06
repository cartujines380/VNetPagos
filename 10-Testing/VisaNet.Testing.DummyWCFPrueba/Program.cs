using System.Linq;
using VisaNet.Testing.DummyWCFPrueba.VersionTests;

namespace VisaNet.Testing.DummyWCFPrueba
{
    class Program
    {
        static void Main(string[] args)
        {
            //args = new[] { "v02" };
            //args = new[] { "v020211" };
            //args = new[] { "v030000" };
            args = new[] { "v030006" };

            if (args.Any())
            {
                switch (args[0].ToLower())
                {
                    case "v02":
                        FirstVersionWcf.Tests();
                        break;
                    case "v020211":
                        SecondVersionWcf.Tests();
                        break;
                    case "v030000":
                        FourthVersionWcf.Tests();
                        break;
                    case "v030006":
                        FifthVersionWcf.Tests();
                        break;
                }
            }
        }

    }
}