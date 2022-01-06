using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Threading;
using VisaNet.Testing.DummyBankWCFPrueba.VisaNetAccess;

namespace VisaNet.Testing.DummyBankWCFPrueba
{
    class Program
    {
        private static CultureInfo _culture;
        
        static void Main(string[] args)
        {
            _culture = new CultureInfo(ConfigurationManager.AppSettings["AppCulture"]);
            Thread.CurrentThread.CurrentCulture = _culture;
            Thread.CurrentThread.CurrentUICulture = _culture;

            //Todos();
            //ObtenerServicios();
            //ObtenerFacturas();
            //Preprocess();
            //Pagar();
            //Log();

            while (true)
            {
                ConsoleOptions();
                Console.Write("OPCION: ");
                var a = Console.ReadLine();

                switch (a)
                {
                    case "1":
                        Todos();
                        break;
                    case "2":
                        ObtenerServicios();
                        break;
                    case "3":
                        ObtenerFacturas();
                        break;
                    case "4":
                        Preprocess();
                        break;
                    case "5":
                        Pagar();
                        break;
                    case "6":
                        Log();
                        break;
                    case "0":
                        Environment.Exit(0);
                        break;
                }
            }
        }

        public static void Todos()
        {
            var service = new VisaNetAccessClient();

            /*****SEARCH BILLS*****/
            //string digitalSignature = PruebaSignature.GenerateSignature((new List<String>
            //{
            //    "Itau", "095CA96D-6D14-43E0-B8CB-FE8B76692245", "20151227", "Banred"
            //}).ToArray());

            //var result = service.SearchBills(new BillsData
            //{
            //    PaymentPlatform = "Itau",
            //    ServiceId = "095CA96D-6D14-43E0-B8CB-FE8B76692245",
            //    ServiceReferenceNumber = "20151227",
            //    GatewayEnumDto = "Banred",
            //    DigitalSignature = digitalSignature
            //});

            //string digitalSignature = PruebaSignature.GenerateSignature((new List<String>
            //{
            //    "Itau", "20819C35-A42D-4FCA-AD89-4C3C5CEB7415", "411111", "0", "Carretera"
            //}).ToArray());

            //var result = service.JsonServiceBills(new VisaNetBillData
            //{
            //    PaymentPlatform = "Itau",
            //    ServiceId = "20819C35-A42D-4FCA-AD89-4C3C5CEB7415",
            //    CardBinNumbers = 411111,
            //    ServiceReferenceNumber = "0",
            //    GatewayEnumDto = "Carretera",
            //    DigitalSignature = digitalSignature
            //});

            string digitalSignature = PruebaSignature.GenerateSignature((new List<String>
            {
                "Itau", "095CA96D-6D14-43E0-B8CB-FE8B76692245", "20150818", "Banred"
            }).ToArray());

            var result = service.SearchBills(new BillsData
            {
                PaymentPlatform = "Itau",
                ServiceId = "095CA96D-6D14-43E0-B8CB-FE8B76692245",
                ServiceReferenceNumber = "20150818",
                GatewayEnumDto = "Banred",
                DigitalSignature = digitalSignature
            });

            Console.WriteLine(result.Response);

            /*****PREPROCESS PAYMENT*****/
            var parameters =
            new List<String>
            {
                "Itau"
            };

            foreach (var bill in result.Response)
            {
                bill.CardBinNumbers = 411111;
                parameters.Add(String.Format(CultureInfo.GetCultureInfo("en-US"), "{0:0.00}", bill.AmountToCybersource));
                if (bill.BillId != null) parameters.Add(bill.BillId);
                if (bill.BillNumber != null) parameters.Add(bill.BillNumber);
                if (bill.Currency != null) parameters.Add(bill.Currency);
                if (bill.Description != null) parameters.Add(bill.Description);
                parameters.Add(String.Format(CultureInfo.GetCultureInfo("en-US"), "{0:0.00}", bill.Discount));
                parameters.Add(bill.DiscountApplyed.ToString().ToLower());
                if (bill.ExpirationDate != null) parameters.Add(DateTime.ParseExact(bill.ExpirationDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyyMMdd"));
                parameters.Add(bill.FinalConsumer.ToString().ToLower());
                if (bill.Gateway != null) parameters.Add(bill.Gateway);
                if (bill.GatewayTransactionId != null) parameters.Add(bill.GatewayTransactionId);
                if (bill.CensusId != null) parameters.Add(bill.CensusId);
                if (bill.Lines != null) parameters.Add(bill.Lines);
                if (bill.ServiceId != null) parameters.Add(bill.ServiceId);
                parameters.Add(bill.Payable.ToString().ToLower());
                parameters.Add(String.Format(CultureInfo.GetCultureInfo("en-US"), "{0:0.00}", bill.TotalAmount));
                parameters.Add(String.Format(CultureInfo.GetCultureInfo("en-US"), "{0:0.00}", bill.TotalTaxedAmount));
                parameters.Add(bill.CardBinNumbers.ToString());
                if (bill.ServiceReferenceNumber != null) parameters.Add(bill.ServiceReferenceNumber);
                if (bill.ServiceReferenceNumber2 != null) parameters.Add(bill.ServiceReferenceNumber2);
                if (bill.ServiceReferenceNumber3 != null) parameters.Add(bill.ServiceReferenceNumber3);
                if (bill.ServiceReferenceNumber4 != null) parameters.Add(bill.ServiceReferenceNumber4);
                if (bill.ServiceReferenceNumber5 != null) parameters.Add(bill.ServiceReferenceNumber5);
                if (bill.ServiceReferenceNumber6 != null) parameters.Add(bill.ServiceReferenceNumber6);
                if (bill.MerchantReferenceCode != null) parameters.Add(bill.MerchantReferenceCode);
                if (bill.MerchantId != null) parameters.Add(bill.MerchantId);
                if (bill.ServiceType != null) parameters.Add(bill.ServiceType);
                parameters.Add(bill.MultipleBillsAllowed.ToString().ToLower());
                if (bill.CreationDate != null) parameters.Add(bill.CreationDate);
            }

            string digitalSignature2 = PruebaSignature.GenerateSignature(parameters.ToArray());

            var result2 = service.PreprocessPayment(new PreprocessPaymentData
            {
                PaymentPlatform = "Itau",
                Bills = result.Response,
                DigitalSignature = digitalSignature2
            });

            Console.WriteLine(result2.Response);

            /*****PAYMENT*****/
            foreach (var bill in result2.Response)
            {
                var parameters2 =
                new List<String>
                {
                    "Itau"
                };

                parameters2.Add(String.Format(CultureInfo.GetCultureInfo("en-US"), "{0:0.00}", bill.AmountToCybersource));
                if (bill.BillId != null) parameters2.Add(bill.BillId);
                if (bill.BillNumber != null) parameters2.Add(bill.BillNumber);
                if (bill.Currency != null) parameters2.Add(bill.Currency);
                if (bill.Description != null) parameters2.Add(bill.Description);
                parameters2.Add(String.Format(CultureInfo.GetCultureInfo("en-US"), "{0:0.00}", bill.Discount));
                parameters2.Add(bill.DiscountApplyed.ToString().ToLower());
                if (bill.ExpirationDate != null) parameters2.Add(DateTime.ParseExact(bill.ExpirationDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyyMMdd"));
                parameters2.Add(bill.FinalConsumer.ToString().ToLower());
                if (bill.Gateway != null) parameters2.Add(bill.Gateway);
                if (bill.GatewayTransactionId != null) parameters2.Add(bill.GatewayTransactionId);
                if (bill.CensusId != null) parameters2.Add(bill.CensusId);
                if (bill.Lines != null) parameters2.Add(bill.Lines);
                if (bill.ServiceId != null) parameters2.Add(bill.ServiceId);
                parameters2.Add(bill.Payable.ToString().ToLower());
                parameters2.Add(String.Format(CultureInfo.GetCultureInfo("en-US"), "{0:0.00}", bill.TotalAmount));
                parameters2.Add(String.Format(CultureInfo.GetCultureInfo("en-US"), "{0:0.00}", bill.TotalTaxedAmount));
                parameters2.Add(bill.CardBinNumbers.ToString());
                if (bill.ServiceReferenceNumber != null) parameters2.Add(bill.ServiceReferenceNumber);
                if (bill.ServiceReferenceNumber2 != null) parameters2.Add(bill.ServiceReferenceNumber2);
                if (bill.ServiceReferenceNumber3 != null) parameters2.Add(bill.ServiceReferenceNumber3);
                if (bill.ServiceReferenceNumber4 != null) parameters2.Add(bill.ServiceReferenceNumber4);
                if (bill.ServiceReferenceNumber5 != null) parameters2.Add(bill.ServiceReferenceNumber5);
                if (bill.ServiceReferenceNumber6 != null) parameters2.Add(bill.ServiceReferenceNumber6);
                if (bill.MerchantReferenceCode != null) parameters2.Add(bill.MerchantReferenceCode);
                if (bill.MerchantId != null) parameters2.Add(bill.MerchantId);
                if (bill.ServiceType != null) parameters2.Add(bill.ServiceType);
                parameters2.Add(bill.MultipleBillsAllowed.ToString().ToLower());
                if (bill.CreationDate != null) parameters2.Add(bill.CreationDate);

                parameters2.Add("411111");
                parameters2.Add("1220");
                parameters2.Add("411111xxxxxx1111");
                parameters2.Add("Yanina");

                parameters2.Add("4504556322585000001521");
                parameters2.Add("100");

                parameters2.Add(bill.ServiceId);

                parameters2.Add("44269189");
                parameters2.Add("yespinosa@hexacta.com");
                parameters2.Add("Yanina");
                parameters2.Add("Espinosa");
                parameters2.Add("Roque Graseras");

                string digitalSignature3 = PruebaSignature.GenerateSignature(parameters2.ToArray());

                var result3 = service.Payment(new PaymentData
                {
                    PaymentPlatform = "Itau",
                    Bill = bill,
                    CardData = new CardData
                    {
                        CardBinNumbers = 411111,
                        DueDate = "1220",
                        MaskedNumber = "411111xxxxxx1111",
                        Name = "Yanina"
                    },
                    CyberSourceData = new VisaNetCyberSourceData
                    {
                        ReqAmount = bill.TotalAmount.ToString(),
                        TransactionId = "4504556322585000001521",
                        ReasonCode = "100"
                    },
                    ServiceId = bill.ServiceId,
                    UserInfo = new UserData
                    {
                        Ci = "44269189",
                        Email = "yespinosa@hexacta.com",
                        Name = "Yanina",
                        Surname = "Espinosa",
                        Address = "Roque Graseras"
                    },
                    DigitalSignature = digitalSignature3
                });

                Console.WriteLine(result3.Response);
            }

            /*****SEARCH TRANSACTIONS*****/
            string digitalSignature4 = PruebaSignature.GenerateSignature((new List<String>
            {
                "Itau"
            }).ToArray());

            var result4 = service.SearchPayments(new SearchPaymentsData
            {
                PaymentPlatform = "Itau",
                DigitalSignature = digitalSignature4
            });

            Console.WriteLine(result4.Response);

        }

        public static void ObtenerServicios()
        {
            string digitalSignature = PruebaSignature.GenerateSignature((new List<String>
            {
                "Itau"
            }).ToArray());

            var service = new VisaNetAccessClient();
            var result = service.GetServices(new ServicesData
            {
                PaymentPlatform = "Itau",
                DigitalSignature = digitalSignature
            });

            Console.WriteLine(result.Response);
        }

        public static void ObtenerFacturas()
        {
            var service = new VisaNetAccessClient();

            string digitalSignature = PruebaSignature.GenerateSignature((new List<String>
            {
                "Itau", "7877D332-7AA8-43C5-AED0-4A75D8748BBC", "043679175000095", "Sistarbanc", "gvarini@hexacta.com"
            }).ToArray());

            var result = service.SearchBills(new BillsData
            {
                PaymentPlatform = "Itau",
                ServiceId = "7877D332-7AA8-43C5-AED0-4A75D8748BBC",
                ServiceReferenceNumber = "043679175000095",
                GatewayEnumDto = "Sistarbanc",
                UserData = new UserData(){Email = "gvarini@hexacta.com"},
                DigitalSignature = digitalSignature
            });

            Console.WriteLine(result.Response);
        }

        public static void Preprocess()
        {
            var service = new VisaNetAccessClient();

            var parameters =
            new List<String>
            {
                "Itau"
            };

            var bills = new List<VisaNetBillResponse>
            {
                new VisaNetBillResponse
                {
                    BillId = "7f89fb43-7527-4ca3-89d0-85073cc144da",
                    BillNumber = "20141031",
                    ServiceId = "095CA96D-6D14-43E0-B8CB-FE8B76692245",
                    Gateway = "Banred",
                    ExpirationDate = "30/01/2016",
                    Currency = "UYU",
                    Description = "Fact. test SANT-1-N-20141031 20141031",
                    GatewayTransactionId = "GatewayTransactionId",
                    Payable = true,
                    FinalConsumer = true,
                    TotalAmount = 210.89,
                    ServiceType = "Emergencias",
                    DiscountApplyed = true
                }
            };

            foreach (var bill in bills)
            {
                parameters.Add(String.Format(CultureInfo.GetCultureInfo("en-US"), "{0:0.00}", bill.AmountToCybersource));
                if (bill.BillId != null) parameters.Add(bill.BillId);
                if (bill.BillNumber != null) parameters.Add(bill.BillNumber);
                if (bill.Currency != null) parameters.Add(bill.Currency);
                if (bill.Description != null) parameters.Add(bill.Description);
                parameters.Add(String.Format(CultureInfo.GetCultureInfo("en-US"), "{0:0.00}", bill.Discount));
                parameters.Add(bill.DiscountApplyed.ToString().ToLower());
                if (bill.ExpirationDate != null) parameters.Add(DateTime.ParseExact(bill.ExpirationDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyyMMdd"));
                parameters.Add(bill.FinalConsumer.ToString().ToLower());
                if (bill.Gateway != null) parameters.Add(bill.Gateway);
                if (bill.GatewayTransactionId != null) parameters.Add(bill.GatewayTransactionId);
                if (bill.CensusId != null) parameters.Add(bill.CensusId);
                if (bill.Lines != null) parameters.Add(bill.Lines);
                if (bill.ServiceId != null) parameters.Add(bill.ServiceId);
                parameters.Add(bill.Payable.ToString().ToLower());
                parameters.Add(String.Format(CultureInfo.GetCultureInfo("en-US"), "{0:0.00}", bill.TotalAmount));
                parameters.Add(String.Format(CultureInfo.GetCultureInfo("en-US"), "{0:0.00}", bill.TotalTaxedAmount));
                parameters.Add(bill.CardBinNumbers.ToString());
                if (bill.ServiceReferenceNumber != null) parameters.Add(bill.ServiceReferenceNumber);
                if (bill.ServiceReferenceNumber2 != null) parameters.Add(bill.ServiceReferenceNumber2);
                if (bill.ServiceReferenceNumber3 != null) parameters.Add(bill.ServiceReferenceNumber3);
                if (bill.ServiceReferenceNumber4 != null) parameters.Add(bill.ServiceReferenceNumber4);
                if (bill.ServiceReferenceNumber5 != null) parameters.Add(bill.ServiceReferenceNumber5);
                if (bill.ServiceReferenceNumber6 != null) parameters.Add(bill.ServiceReferenceNumber6);
                if (bill.MerchantReferenceCode != null) parameters.Add(bill.MerchantReferenceCode);
                if (bill.MerchantId != null) parameters.Add(bill.MerchantId);
                if (bill.ServiceType != null) parameters.Add(bill.ServiceType);
                parameters.Add(bill.MultipleBillsAllowed.ToString().ToLower());
                if (bill.CreationDate != null) parameters.Add(bill.CreationDate);
            }

            string digitalSignature2 = PruebaSignature.GenerateSignature(parameters.ToArray());

            var result2 = service.PreprocessPayment(new PreprocessPaymentData
            {
                PaymentPlatform = "Itau",
                Bills = bills.ToArray(),
                DigitalSignature = digitalSignature2
            });

            Console.WriteLine(result2.Response);
        }

        public static void Pagar()
        {
            var service = new VisaNetAccessClient();

            var parameters2 =
            new List<String>
            {
                "Itau"
            };

            var payment = new PaymentData
            {
                PaymentPlatform = "Itau",
                Bill = new VisaNetBillResponse
                {
                    BillId = "d51d4bdd-f231-4947-abcf-bf879a0ed72b",
                    BillNumber = "4604",
                    ExpirationDate = "30/01/2016",
                    Currency = "UYU",
                    Gateway = "Banred",
                    Description = "Fact. test 4604",
                    Payable = true,
                    FinalConsumer = true,
                    TotalAmount = 2444.42,
                    TotalTaxedAmount = 2354.42,
                    ServiceId = "095CA96D-6D14-43E0-B8CB-FE8B76692245",
                    Discount = 0,
                    DiscountApplyed = false,
                    AmountToCybersource = 2444.42,
                    ServiceReferenceNumber = "20141031",
                    CardBinNumbers = 411111
                },
                CardData = new CardData
                {
                    CardBinNumbers = 411111,
                    DueDate = "0920",
                    MaskedNumber = "411111xxxxxx1111",
                    Name = "Juan Perez"
                },
                CyberSourceData = new VisaNetCyberSourceData
                {
                    TransactionId = "4538396224286774001018",
                    ReasonCode = "100"
                },
                ServiceId = "095CA96D-6D14-43E0-B8CB-FE8B76692245",
                UserInfo = new UserData
                {
                    Ci = "",
                    Email = "juan@perez.com",
                    Name = "Juan",
                    Surname = "Perez",
                    Address = ""
                }
            };

            parameters2.Add(String.Format(CultureInfo.GetCultureInfo("en-US"), "{0:0.00}", payment.Bill.AmountToCybersource));
            if (payment.Bill.BillId != null) parameters2.Add(payment.Bill.BillId);
            if (payment.Bill.BillNumber != null) parameters2.Add(payment.Bill.BillNumber);
            if (payment.Bill.Currency != null) parameters2.Add(payment.Bill.Currency);
            if (payment.Bill.Description != null) parameters2.Add(payment.Bill.Description);
            parameters2.Add(String.Format(CultureInfo.GetCultureInfo("en-US"), "{0:0.00}", payment.Bill.Discount));
            parameters2.Add(payment.Bill.DiscountApplyed.ToString().ToLower());
            if (payment.Bill.ExpirationDate != null) parameters2.Add(DateTime.ParseExact(payment.Bill.ExpirationDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyyMMdd"));
            parameters2.Add(payment.Bill.FinalConsumer.ToString().ToLower());
            if (payment.Bill.Gateway != null) parameters2.Add(payment.Bill.Gateway);
            if (payment.Bill.GatewayTransactionId != null) parameters2.Add(payment.Bill.GatewayTransactionId);
            if (payment.Bill.CensusId != null) parameters2.Add(payment.Bill.CensusId);
            if (payment.Bill.Lines != null) parameters2.Add(payment.Bill.Lines);
            if (payment.Bill.ServiceId != null) parameters2.Add(payment.Bill.ServiceId);
            parameters2.Add(payment.Bill.Payable.ToString().ToLower());
            parameters2.Add(String.Format(CultureInfo.GetCultureInfo("en-US"), "{0:0.00}", payment.Bill.TotalAmount));
            parameters2.Add(String.Format(CultureInfo.GetCultureInfo("en-US"), "{0:0.00}", payment.Bill.TotalTaxedAmount));
            parameters2.Add(payment.Bill.CardBinNumbers.ToString());
            if (payment.Bill.ServiceReferenceNumber != null) parameters2.Add(payment.Bill.ServiceReferenceNumber);
            if (payment.Bill.ServiceReferenceNumber2 != null) parameters2.Add(payment.Bill.ServiceReferenceNumber2);
            if (payment.Bill.ServiceReferenceNumber3 != null) parameters2.Add(payment.Bill.ServiceReferenceNumber3);
            if (payment.Bill.ServiceReferenceNumber4 != null) parameters2.Add(payment.Bill.ServiceReferenceNumber4);
            if (payment.Bill.ServiceReferenceNumber5 != null) parameters2.Add(payment.Bill.ServiceReferenceNumber5);
            if (payment.Bill.ServiceReferenceNumber6 != null) parameters2.Add(payment.Bill.ServiceReferenceNumber6);
            if (payment.Bill.MerchantReferenceCode != null) parameters2.Add(payment.Bill.MerchantReferenceCode);
            if (payment.Bill.MerchantId != null) parameters2.Add(payment.Bill.MerchantId);
            if (payment.Bill.ServiceType != null) parameters2.Add(payment.Bill.ServiceType);
            parameters2.Add(payment.Bill.MultipleBillsAllowed.ToString().ToLower());
            if (payment.Bill.CreationDate != null) parameters2.Add(payment.Bill.CreationDate);

            parameters2.Add(payment.CardData.CardBinNumbers.ToString());
            if (payment.CardData.DueDate != null) parameters2.Add(payment.CardData.DueDate);
            if (payment.CardData.MaskedNumber != null) parameters2.Add(payment.CardData.MaskedNumber);
            if (payment.CardData.Name != null) parameters2.Add(payment.CardData.Name);

            if (payment.CyberSourceData.TransactionId != null) parameters2.Add(payment.CyberSourceData.TransactionId);
            if (payment.CyberSourceData.PaymentToken != null) parameters2.Add(payment.CyberSourceData.PaymentToken);
            if (payment.CyberSourceData.ReasonCode != null) parameters2.Add(payment.CyberSourceData.ReasonCode);

            parameters2.Add(payment.ServiceId);

            if (payment.UserInfo.Ci != null) parameters2.Add(payment.UserInfo.Ci);
            if (payment.UserInfo.Email != null) parameters2.Add(payment.UserInfo.Email);
            if (payment.UserInfo.Name != null) parameters2.Add(payment.UserInfo.Name);
            if (payment.UserInfo.Surname != null) parameters2.Add(payment.UserInfo.Surname);
            if (payment.UserInfo.Address != null) parameters2.Add(payment.UserInfo.Address);

            string digitalSignature3 = PruebaSignature.GenerateSignature(parameters2.ToArray());
            payment.DigitalSignature = digitalSignature3;

            var result3 = service.Payment(payment);

            Console.WriteLine(result3.Response);
        }

        public static void Log()
        {
            var paramsList =
            new List<String>
            {
                "Itau",
                NotificationType.Ok.ToString(),
                Operation.Payment.ToString(),
                "lala"
            };
            string digitalSignature = PruebaSignature.GenerateSignature((paramsList).ToArray());

            var service = new VisaNetAccessClient();
            var result = service.NotifyOperationResult(new NotificationData
            {
                PaymentPlatform = "Itau",
                NotificationType = NotificationType.Ok,
                Operation = Operation.Payment,
                Message = "lala",
                DigitalSignature = digitalSignature
            });

            Console.WriteLine(result);
        }

        private static void ConsoleOptions()
        {
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("------------------------------------");
            Console.WriteLine("OPCIONES");
            Console.WriteLine("------------------------------------");
            Console.WriteLine("1   => Todos");
            Console.WriteLine("2   => ObtenerServicios");
            Console.WriteLine("3   => ObtenerFacturas");
            Console.WriteLine("4   => Preprocess");
            Console.WriteLine("5   => Pagar");
            Console.WriteLine("6   => Log");
            Console.WriteLine("0   => Salir ");
            Console.WriteLine("");
            Console.WriteLine("------------------------------------");
            Console.WriteLine("");
            Console.WriteLine("");
        }

    }
}
