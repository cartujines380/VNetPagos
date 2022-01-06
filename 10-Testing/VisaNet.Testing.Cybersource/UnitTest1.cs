using System;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VisaNet.Common.Exceptions;
using VisaNet.Common.Logging.NLog;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Utilities.Cybersource;

namespace VisaNet.Testing.Cybersource
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestField48_Wcf_Debit_Success()
        {
            var payment = GeneratePaymentDto(CardTypeDto.Debit, DiscountLabelTypeDto.TaxReintegration, 1);
            var cyberSourceMerchantDefinedData = GenerateCyberSourceMerchantDefinedDataDto("14.02", "4391315");

            var expectedRsult = "003000         40000000014024391315";
            var result = CsMethod.GenerateField48(payment, cyberSourceMerchantDefinedData);

            //Assert
            Assert.IsTrue(result.Equals(expectedRsult));
        }

        [TestMethod]
        public void TestField48_Wcf_Debit_NoDecimal_Success()
        {
            var payment = GeneratePaymentDto(CardTypeDto.Debit, DiscountLabelTypeDto.TaxReintegration, 1);
            var cyberSourceMerchantDefinedData = GenerateCyberSourceMerchantDefinedDataDto("14", "4391315");

            var expectedRsult = "003000         40000000014004391315";
            var result = CsMethod.GenerateField48(payment, cyberSourceMerchantDefinedData);

            //Assert
            Assert.IsTrue(result.Equals(expectedRsult));
        }

        [TestMethod]
        public void TestField48_Wcf_Debit_NoDiscount_Success()
        {
            var payment = GeneratePaymentDto(CardTypeDto.Debit, DiscountLabelTypeDto.TaxReintegration, 1);
            var cyberSourceMerchantDefinedData = GenerateCyberSourceMerchantDefinedDataDto("0", "4391315");

            var expectedRsult = "003000         40000000000004391315";
            var result = CsMethod.GenerateField48(payment, cyberSourceMerchantDefinedData);

            //Assert
            Assert.IsTrue(result.Equals(expectedRsult));
        }

        [TestMethod]
        public void TestField48_Wcf_Debit_Fail()
        {
            var payment = GeneratePaymentDto(CardTypeDto.Debit, DiscountLabelTypeDto.TaxReintegration, 1);
            var cyberSourceMerchantDefinedData = GenerateCyberSourceMerchantDefinedDataDto("14,02", "4391315");

            var expectedRsult = "003000         40000000014024391315";
            var result = CsMethod.GenerateField48(payment, cyberSourceMerchantDefinedData);

            //Assert
            Assert.IsFalse(result.Equals(expectedRsult));
            
        }

        [TestMethod]
        [ExpectedException(typeof(BusinessException))]
        public void TestField48_Wcf_Debit_NoCardType()
        {
            var payment = GeneratePaymentDto(0, DiscountLabelTypeDto.TaxReintegration, 1);
            var cyberSourceMerchantDefinedData = GenerateCyberSourceMerchantDefinedDataDto("14,02", "4391315");

            var expectedRsult = "003000         40000000014024391315";
            var result = CsMethod.GenerateField48(payment, cyberSourceMerchantDefinedData);
        }

        [TestMethod]
        public void TestField48_Wcf_Credit_Success()
        {
            var payment = GeneratePaymentDto(CardTypeDto.Credit, DiscountLabelTypeDto.FinancialInclusion, 1);
            var cyberSourceMerchantDefinedData = GenerateCyberSourceMerchantDefinedDataDto("16.6", "4408027");

            var expectedRsult = "000010         60000000016604408027";
            var result = CsMethod.GenerateField48(payment, cyberSourceMerchantDefinedData);

            //Assert
            Assert.IsTrue(result.Equals(expectedRsult));
        }

        [TestMethod]
        public void TestField48_Wcf_Credit_NoDiscount_Success()
        {
            var payment = GeneratePaymentDto(CardTypeDto.Credit, DiscountLabelTypeDto.FinancialInclusion, 1);
            var cyberSourceMerchantDefinedData = GenerateCyberSourceMerchantDefinedDataDto("0", "4408027");

            var expectedRsult = "000010         60000000000004408027";
            var result = CsMethod.GenerateField48(payment, cyberSourceMerchantDefinedData);

            //Assert
            Assert.IsTrue(result.Equals(expectedRsult));
        }

        [TestMethod]
        public void TestField48_Wcf_Credit__NoDecimal_Success()
        {
            var payment = GeneratePaymentDto(CardTypeDto.Credit, DiscountLabelTypeDto.FinancialInclusion, 1);
            var cyberSourceMerchantDefinedData = GenerateCyberSourceMerchantDefinedDataDto("15", "4408027");

            var expectedRsult = "000010         60000000015004408027";
            var result = CsMethod.GenerateField48(payment, cyberSourceMerchantDefinedData);

            //Assert
            Assert.IsTrue(result.Equals(expectedRsult));
        }

        [TestMethod]
        public void TestField48_Wcf_Credit_Quota12_Success()
        {
            var payment = GeneratePaymentDto(CardTypeDto.Credit, DiscountLabelTypeDto.FinancialInclusion, 12);
            var cyberSourceMerchantDefinedData = GenerateCyberSourceMerchantDefinedDataDto("16.6", "4408027");

            var expectedRsult = "000120         60000000016604408027";
            var result = CsMethod.GenerateField48(payment, cyberSourceMerchantDefinedData);

            //Assert
            Assert.IsTrue(result.Equals(expectedRsult));
        }

        [TestMethod]
        [ExpectedException(typeof(BusinessException))]
        public void TestField48_Wcf_Credit_NoCardType()
        {
            var payment = GeneratePaymentDto(0, DiscountLabelTypeDto.TaxReintegration, 1);
            var cyberSourceMerchantDefinedData = GenerateCyberSourceMerchantDefinedDataDto("16.6", "4408027");

            var expectedRsult = "000010         10000000016604408027";
            var result = CsMethod.GenerateField48(payment, cyberSourceMerchantDefinedData);
        }

        private GeneratePayment GeneratePaymentDto(CardTypeDto cardTypeDto, DiscountLabelTypeDto discountLabelTypeDto, int quota)
        {
            return new GeneratePayment()
            {
                AdditionalInfo = new AdditionalInfo()
                {
                    CardTypeDto = cardTypeDto,
                    DiscountLabelTypeDto = discountLabelTypeDto,
                },
                Quota = quota,
            };
        }

        private CyberSourceMerchantDefinedDataDto GenerateCyberSourceMerchantDefinedDataDto(string discount, string billNumber)
        {
            return new CyberSourceMerchantDefinedDataDto()
            {
                Discount = discount,
                BillNumber = billNumber
            };
        }

        [TestMethod]
        public void TestField48_Web_Debit_Success()
        {
            var payment = GenerateKeysInfoForPayment(CardTypeDto.Debit, 1, 4, 14.02, "4391315", string.Empty);

            var expectedRsult = "003000         40000000014024391315";
            var result = CsMethod.GenerateField48(payment);

            //Assert
            Assert.IsTrue(result.Equals(expectedRsult));
        }

        [TestMethod]
        public void TestField48_Web_Debit_NoDecimal_Success()
        {
            var payment = GenerateKeysInfoForPayment(CardTypeDto.Debit, 1, 4, 14, "4391315", string.Empty);

            var expectedRsult = "003000         40000000014004391315";
            var result = CsMethod.GenerateField48(payment);

            //Assert
            Assert.IsTrue(result.Equals(expectedRsult));
        }

        [TestMethod]
        public void TestField48_Web_Debit_NoDiscount_Success()
        {
            var payment = GenerateKeysInfoForPayment(CardTypeDto.Debit, 1, 4, 0, "4391315", string.Empty);

            
            var expectedRsult = "003000         40000000000004391315";
            var result = CsMethod.GenerateField48(payment);

            //Assert
            Assert.IsTrue(result.Equals(expectedRsult));
        }
        
        [TestMethod]
        [ExpectedException(typeof(BusinessException))]
        public void TestField48_Web_Debit_NoCardType()
        {
            var payment = GeneratePaymentDto(0, DiscountLabelTypeDto.TaxReintegration, 1);
            var cyberSourceMerchantDefinedData = GenerateCyberSourceMerchantDefinedDataDto("14,02", "4391315");

            var expectedRsult = "003000         40000000014024391315";
            var result = CsMethod.GenerateField48(payment, cyberSourceMerchantDefinedData);
        }

        [TestMethod]
        public void TestField48_Web_Credit_Success()
        {
            var payment = GenerateKeysInfoForPayment(CardTypeDto.Credit, 1, 1, 16.6, "4408027", string.Empty);

            var expectedRsult = "000010         10000000016604408027";
            var result = CsMethod.GenerateField48(payment);

            //Assert
            Assert.IsTrue(result.Equals(expectedRsult));
        }

        [TestMethod]
        public void TestField48_Web_Credit_NoDiscount_Success()
        {
            var payment = GenerateKeysInfoForPayment(CardTypeDto.Credit, 1, 1, 0, "4408027", string.Empty);

            var expectedRsult = "000010         10000000000004408027";
            var result = CsMethod.GenerateField48(payment);

            //Assert
            Assert.IsTrue(result.Equals(expectedRsult));
        }

        [TestMethod]
        public void TestField48_Web_Credit__NoDecimal_Success()
        {
            var payment = GenerateKeysInfoForPayment(CardTypeDto.Credit, 1, 1, 15, "4408027", string.Empty);

            var expectedRsult = "000010         10000000015004408027";
            var result = CsMethod.GenerateField48(payment);

            //Assert
            Assert.IsTrue(result.Equals(expectedRsult));
        }

        [TestMethod]
        public void TestField48_Web_Credit_Quota12_Success()
        {
            var payment = GenerateKeysInfoForPayment(CardTypeDto.Credit, 12, 1, 16.6, "4408027", string.Empty);

            var expectedRsult = "000120         10000000016604408027";
            var result = CsMethod.GenerateField48(payment);

            //Assert
            Assert.IsTrue(result.Equals(expectedRsult));
        }

        [TestMethod]
        [ExpectedException(typeof(BusinessException))]
        public void TestField48_Web_Credit_NoCardType()
        {
            var payment = GenerateKeysInfoForPayment(0, 1, 1, 16.6, "4408027", string.Empty);

            var expectedRsult = "000010         10000000016604408027";
            var result = CsMethod.GenerateField48(payment);
        }

        private KeysInfoForPayment GenerateKeysInfoForPayment(CardTypeDto cardTypeDto, int quota, int discountType, double discountAmount, string billNumber, string billSucivePreBillNumber)
        {
            return new KeysInfoForPaymentNewUser()
            {
                CardTypeDto = cardTypeDto,
                Quotas = quota,
                Bill = new BillForToken()
                {
                    DiscountType = discountType,
                    DiscountAmount = discountAmount,
                    BillNumber = billNumber,
                    BillSucivePreBillNumber = billSucivePreBillNumber
                }
            };
        }
    }

    /// <summary>
    /// CLASE PARA SIMULAR LOS DOS METODOS EN CUESTION
    /// </summary>
    public static class CsMethod
    {
        /// <summary>
        /// WEB SERVICE
        /// </summary>
        /// <param name="payment"></param>
        /// <param name="cyberSourceMerchantDefinedData"></param>
        /// <returns></returns>
        public static string GenerateField48(GeneratePayment payment, CyberSourceMerchantDefinedDataDto cyberSourceMerchantDefinedData)
        {
            var result = string.Empty;

            var max = Enum.GetValues(typeof(CardTypeDto)).Cast<int>().Max();
            var min = Enum.GetValues(typeof(CardTypeDto)).Cast<int>().Min();

            if ((int)payment.AdditionalInfo.CardTypeDto < min || (int)payment.AdditionalInfo.CardTypeDto > max)
            {
                throw new BusinessException(CodeExceptions.PAYMENT_FIELD48_WRONG);
            }

            #region Como se arma el string
            //1-2 2 Plan type. Set this value to 00. Specifies that the transaction is an e-commerce transaction.
            //3 1 Grace period. Number of months that the issuer waits before charging customers.
            //4-5 2 Total number of installments. Possible values: 00 through 99.
            //6 1 POS entry mode. Set this value to 0. Specifies that the transaction is an e-commerce transaction.
            //7-15 9 Identity document number. Set this value to the number on the cardholder’s identity document or leave it blank.
            //Format: right justified with 0 (zero) padding on the left.
            //16 1 Financial inclusion law indicator. Possible values:
            // 1: Law 17934
            // 2: Law 18099
            // 3: Asignaciones familiares (AFAM) (family allowance program)
            // 4: Real state law
            // 5: Law 19210
            //17-28 12 Financial inclusion amount. This value is the amount the bank returns to the cardholder.
            //29-35 7 Merchant-generated invoice number. 


            //digito del 1 - 6 credito

            //digito del 7 en adelante debito
            //<c:issuer>
            //    <c:additionalData>003000         60000000000681234567</c:additionalData>
            //</c:issuer>

            // no paso cedula en ningun caso. NO SE USA. Pedido ANDRES
            #endregion

            try
            {
                var plan = "00";
                var gracePeriod = "0";
                var quotas = payment.Quota.ToString().PadLeft(2, '0');
                var pos = "0";
                var userCi = string.Empty.PadLeft(9, ' ');

                var discountApplied = (int)payment.AdditionalInfo.DiscountLabelTypeDto;
                double billDiscountAmount = 0;
                var culture = CultureInfo.CreateSpecificCulture("en-US");

                if (!string.IsNullOrEmpty(cyberSourceMerchantDefinedData.Discount))
                {
                    double.TryParse(cyberSourceMerchantDefinedData.Discount, NumberStyles.AllowDecimalPoint, culture, out billDiscountAmount);
                }

                var discount = (billDiscountAmount * 100).ToString("####").PadLeft(12, '0');
                var billNumber = !string.IsNullOrEmpty(cyberSourceMerchantDefinedData.BillNumber)
                    ? cyberSourceMerchantDefinedData.BillNumber
                    : cyberSourceMerchantDefinedData.BillSucivePreBillNumber;

                billNumber = billNumber.PadLeft(7, '0');
                if (billNumber.Count() > 7)
                {
                    billNumber = billNumber.Substring(billNumber.Count() - 7, 7);
                }

                if (payment.AdditionalInfo.CardTypeDto == CardTypeDto.Debit || payment.AdditionalInfo.CardTypeDto == CardTypeDto.InternationalDebit ||
                    payment.AdditionalInfo.CardTypeDto == CardTypeDto.NationalPrepaid || payment.AdditionalInfo.CardTypeDto == CardTypeDto.InternationalPrepaid)
                {
                    result = string.Format("{0}{1}{2}{3}{4}",
                        "003000",
                        userCi,
                        discountApplied,
                        discount,
                        billNumber);
                }
                else
                {
                    result = string.Format("{0}{1}{2}{3}{4}{5}{6}{7}",
                        plan,
                        gracePeriod,
                        quotas,
                        pos,
                        userCi,
                        discountApplied,
                        discount,
                        billNumber);
                }
            }
            catch (Exception exception)
            {
                NLogLogger.LogEvent(NLogType.Info, "Excepcion - GenerateField48");
                NLogLogger.LogEvent(exception);
            }

            NLogLogger.LogEvent(NLogType.Info, "FIELD48: " + result);
            return result;
        }

        /// <summary>
        /// VISANETPAGOS WEB
        /// </summary>
        /// <param name="generateToken"></param>
        /// <returns></returns>
        public static string GenerateField48(IGenerateToken generateToken)
        {
            var item = (KeysInfoForPayment)generateToken;

            var result = string.Empty;

            var max = Enum.GetValues(typeof(CardTypeDto)).Cast<int>().Max();
            var min = Enum.GetValues(typeof(CardTypeDto)).Cast<int>().Min();

            if ((int)item.CardTypeDto < min || (int)item.CardTypeDto > max)
            {
                throw new BusinessException(CodeExceptions.PAYMENT_FIELD48_WRONG);
            }
            #region Como se arma el string
            //1-2 2 Plan type. Set this value to 00. Specifies that the transaction is an e-commerce transaction.
            //3 1 Grace period. Number of months that the issuer waits before charging customers.
            //4-5 2 Total number of installments. Possible values: 00 through 99.
            //6 1 POS entry mode. Set this value to 0. Specifies that the transaction is an e-commerce transaction.
            //7-15 9 Identity document number. Set this value to the number on the cardholder’s identity document or leave it blank.
            //Format: right justified with 0 (zero) padding on the left.
            //16 1 Financial inclusion law indicator. Possible values:
            // 1: Law 17934
            // 2: Law 18099
            // 3: Asignaciones familiares (AFAM) (family allowance program)
            // 4: Real state law
            // 5: Law 19210
            //17-28 12 Financial inclusion amount. This value is the amount the bank returns to the cardholder.
            //29-35 7 Merchant-generated invoice number. 


            //digito del 1 - 6 credito

            //digito del 7 en adelante debito
            //<c:issuer>
            //    <c:additionalData>003000         60000000000681234567</c:additionalData>
            //</c:issuer>

            // no paso cedula en ningun caso. NO SE USA. Pedido ANDRES
            #endregion

            try
            {
                const string plan = "00";
                const string gracePeriod = "0";
                var quotas = item.Quotas.ToString().PadLeft(2, '0');
                const string pos = "0";
                var userCi = string.Empty.PadLeft(9, ' ');

                var discountApplied = (int)item.Bill.DiscountType;
                var billDiscountAmount = item.Bill.DiscountAmount;

                var discount = (billDiscountAmount * 100).ToString("####").PadLeft(12, '0');
                var billNumber = !string.IsNullOrEmpty(item.Bill.BillNumber)
                    ? item.Bill.BillNumber
                    : item.Bill.BillSucivePreBillNumber;

                billNumber = billNumber.PadLeft(7, '0');
                if (billNumber.Count() > 7)
                {
                    billNumber = billNumber.Substring(billNumber.Count() - 7, 7);
                }

                if (item.CardTypeDto == CardTypeDto.Debit || item.CardTypeDto == CardTypeDto.InternationalDebit ||
                    item.CardTypeDto == CardTypeDto.NationalPrepaid || item.CardTypeDto == CardTypeDto.InternationalPrepaid)
                {
                    result = string.Format("{0}{1}{2}{3}{4}",
                        "003000",
                        userCi,
                        discountApplied,
                        discount,
                        billNumber);
                }
                else
                {
                    result = string.Format("{0}{1}{2}{3}{4}{5}{6}{7}",
                        plan,
                        gracePeriod,
                        quotas,
                        pos,
                        userCi,
                        discountApplied,
                        discount,
                        billNumber);
                }
            }
            catch (Exception exception)
            {
                NLogLogger.LogEvent(NLogType.Info, "Excepcion - GenerateField48");
                NLogLogger.LogEvent(exception);
            }

            NLogLogger.LogEvent(NLogType.Info, "FIELD48: " + result);
            return result;
        }
    }
}
