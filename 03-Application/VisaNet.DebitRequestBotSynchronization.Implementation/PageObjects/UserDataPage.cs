using System;
using OpenQA.Selenium;
using VisaNet.DebitRequestBotSynchronization.Implementation.PageObjects.Base;
using VisaNet.DebitRequestBotSynchronization.Implementation.PageObjects.Interfaces;
using OpenQA.Selenium.Support.PageObjects;
using OpenQA.Selenium.Support.UI;
using VisaNet.Common.Exceptions;

namespace VisaNet.DebitRequestBotSynchronization.Implementation.PageObjects
{
    public class UserDataPage : BasePage, IUserDataPage
    {
        public UserDataPage(IWebDriver driver) : base(driver)
        {
        }

        [FindsBy(How = How.Id, Using = "txtName")]
        private IWebElement fullnameInput;

        [FindsBy(How = How.Id, Using = "txtDocNumber")]
        private IWebElement identityNumberInput;

        [FindsBy(How = How.Id, Using = "txtAdStreet")]
        private IWebElement streetAddressInput;

        [FindsBy(How = How.Id, Using = "txtAdNumber")]
        private IWebElement numberAddressInput;

        [FindsBy(How = How.Id, Using = "txtAdCity")]
        private IWebElement cityInput;

        [FindsBy(How = How.Id, Using = "txtAdLoc")]
        private IWebElement locationInput;

        [FindsBy(How = How.Id, Using = "txtPhone")]
        private IWebElement phoneNumberInput;

        [FindsBy(How = How.Id, Using = "txtEmail")]
        private IWebElement emailInput;

        [FindsBy(How = How.Id, Using = "btnRegister")]
        private IWebElement buttonRegister;

        public void SetUserData(string fullname, string identityNumber, string streetAddress, string numberAddress, string city, string location, string phone, string email)
        {
            fullnameInput.SendKeys(fullname);
            identityNumberInput.SendKeys(identityNumber);
            streetAddressInput.SendKeys(streetAddress);
            numberAddressInput.SendKeys(numberAddress);
            cityInput.SendKeys(city);
            locationInput.SendKeys(location);
            phoneNumberInput.SendKeys(phone);
            emailInput.SendKeys(email);

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
                buttonBack.Click();
                throw new PageObjectException(message);
            }
            catch (Exception)
            {
                try
                {
                    GetWait().Until(ExpectedConditions.ElementIsVisible(By.Id("lblUserExists")));
                    var element = _driver.FindElement(By.Id("lblUserExists"));
                    buttonRegister.Click();
                }
                catch (Exception)
                {
                    return;
                }
            }
        }
    }
}
