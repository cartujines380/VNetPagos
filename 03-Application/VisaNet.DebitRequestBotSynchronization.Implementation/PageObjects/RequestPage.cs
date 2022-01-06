using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.PageObjects;
using OpenQA.Selenium.Support.UI;
using VisaNet.DebitRequestBotSynchronization.Implementation.PageObjects.Base;
using VisaNet.DebitRequestBotSynchronization.Implementation.PageObjects.Interfaces;
using VisaNet.Common.Exceptions;

namespace VisaNet.DebitRequestBotSynchronization.Implementation.PageObjects
{
    public class RequestPage : BasePage, IRequestPage
    {
        public RequestPage(IWebDriver driver) : base(driver)
        {
        }

        [FindsBy(How = How.Id, Using = "drpGroup")]
        private IWebElement dropdownCommerceGroup;
        [FindsBy(How = How.Id, Using = "drpMerchant")]
        private IWebElement dropdownCommerce;
        [FindsBy(How = How.Id, Using = "drpProduct")]
        private IWebElement dropdownService;

        [FindsBy(How = How.Id, Using = "txtCardPan")]
        private IWebElement cardNumberInput;
        [FindsBy(How = How.Id, Using = "drpYearExp")]
        private IWebElement dropdownYear;
        [FindsBy(How = How.Id, Using = "drpMonthExp")]
        private IWebElement dropdownMonth;

        [FindsBy(How = How.Id, Using = "txtTitular")]
        private IWebElement fullnameInput;

        #region Public methods

        public void SelectCommerceGroup(int commerceGroupId)
        {
            try
            {
                SelectDropdownValue(dropdownCommerceGroup, commerceGroupId.ToString());                
            }
            catch (Exception e)
            {
                buttonBack.Click();
                throw new PageObjectException(string.Format("{0}: {1}", "Error al seleccionar el grupo de comercio", e.Message));
            }
        }

        public void SelectCommerce(int commerceId)
        {
            try
            {
                GetWait(20).Until(ExpectedConditions.ElementIsVisible(By.Id(dropdownCommerce.GetAttribute("id"))));
                SelectDropdownValue(dropdownCommerce, commerceId.ToString());                
            }
            catch (Exception e)
            {
                buttonBack.Click();
                throw new PageObjectException(string.Format("{0}: {1}", "Error al seleccionar el comercio", e.Message));
            }
        }

        public void SelectService(int serviceId)
        {
            try
            {
                GetWait(20).Until(ExpectedConditions.ElementIsVisible(By.Id(dropdownService.GetAttribute("id"))));
                SelectDropdownValue(dropdownService, serviceId.ToString());
            }
            catch (Exception e)
            {
                buttonBack.Click();
                throw new PageObjectException(string.Format("{0}: {1}", "Error al seleccionar el producto", e.Message));
            }
        }

        public void SetCreditCard(string cardNumber, int month, int year)
        {
            cardNumberInput.SendKeys(cardNumber);
            SelectDropdownValue(dropdownMonth, MonthToString(month));
            SelectDropdownValue(dropdownYear, year.ToString());
        }

        public void SetReferences(string fullname, IEnumerable<string> references)
        {
            fullnameInput.SendKeys(fullname);
            for (var i = 0; i < references.Count(); i++)
            {
                var id = string.Format("Property{0}", i);
                var propertyInput = _driver.FindElement(By.Id(id));

                propertyInput.SendKeys(references.ElementAt(i));
            }
        }

        public void ValidateAndNext()
        {
            buttonNext.Click();
            ValidatePage();
        }

        public override void ValidatePage()
        {
            string message;
            try
            {
                GetWait().Until(ExpectedConditions.ElementIsVisible(By.Id(labelError.GetAttribute("id"))));
                message = "Error en los datos de la solicitud: " + labelError.Text;
            }
            catch (Exception)
            {
                try
                {
                    GetWait().Until(ExpectedConditions.ElementIsVisible(By.Id("ValidationSummary1")));
                    var validationElement = _driver.FindElement(By.Id("ValidationSummary1"));
                    message = "Validación en los datos de la solicitud: " + validationElement.Text;
                }
                catch (Exception)
                {
                    return;
                }
            }

            buttonBack.Click();
            throw new PageObjectException(message);
        }

        #endregion Public methods

        #region Private methods

        private static string MonthToString(int month)
        {
            var monthString = month.ToString();
            return monthString.Length == 1 ? string.Format("0{0}", monthString) : monthString;
        }

        #endregion Private methods
    }
}
