using System;
using System.Linq;
using VisaNet.Testing.BankingWCF.Demouy448;

namespace VisaNet.Testing.BankingWCF
{
    class Program
    {
        const string AsurServiceId = "095CA96D-6D14-43E0-B8CB-FE8B76692245";
        const string CertificateThumbprint = "84CC0A008C6CBB7D25E986628CFD67B4F5FE19EF";
        const string Email = "gvarini1@hexacta.com";
        const string PaymentPlatform = "consoleAppClient";

        static void Main(string[] args)
        {

            while (true)
            {
                Console.Write("OPCION: ");
                var a = Console.ReadLine();
                if (a.Equals("p"))
                {
                    Excecute();
                }
                if (a.Equals("s"))
                {
                    Environment.Exit(0);
                }
            }


        }

        private static void Excecute()
        {
            try
            {
                var client = new VisaNetAccessClient();
                var bills = SearchBills(client);

                if (bills.Response != null && bills.Response.Any())
                {
                    var billToPay = bills.Response.First();
                    var processedBill = PreProcessBill(billToPay, client);
                    var billData = processedBill.Response.FirstOrDefault();

                   
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine("Excepcion: " + exception.Message);
            }

        }

        private static BillsResponse SearchBills(VisaNetAccessClient client)
        {
            var billsData = new BillsData
            {
                ServiceId = AsurServiceId,
                GatewayEnumDto = "Banred",
                ServiceReferenceNumber = "20130225N",
                PaymentPlatform = PaymentPlatform,
                UserData = new UserData()
                {
                    Email = Email
                },
                
            };

            var digitalSignature = DigitalSignature.GenerateSignature(billsData.ToParamsArray(), CertificateThumbprint);
            billsData.DigitalSignature = digitalSignature;

            var bills = client.SearchBills(billsData);

            return bills;
        }

        private static PreprocessPaymentResponse PreProcessBill(VisaNetBillResponse billToPay, VisaNetAccessClient client)
        {
            billToPay.CardBinNumbers = 411111;
            var data = new PreprocessPaymentData
            {
                Bills = new VisaNetBillResponse[] { billToPay },
                PaymentPlatform = PaymentPlatform,
                
            };
            var digitalSignature = DigitalSignature.GenerateSignature(data.ToParamsArray(), CertificateThumbprint);
            data.DigitalSignature = digitalSignature;

            var processedBill = client.PreprocessPayment(data);

            return processedBill;
        }

    }
}
